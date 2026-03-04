using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Relatorio;
using BarbeariaRocha.Modelos.Response.Relatorio;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class RelatorioApp(Contexto contexto) : IRelatorioApp
    {
        private readonly Contexto _contexto = contexto;

        public RelatorioGeralResponse ObterRelatorioGeral(RelatorioFiltroRequest filtro)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status != AgendamentoStatus.SlotReservado.ToString())
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias (evita contar 2x)
                .AsQueryable();

            if (filtro.BarbeiroId.HasValue)
                query = query.Where(a => a.BarbeiroId == filtro.BarbeiroId.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var concluidos = query.Where(a => a.Status == AgendamentoStatus.Concluido.ToString());
            var hoje = DateTime.Today;
            var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            var todosConcluidos = concluidos.ToList();

            var faturamentoTotal = todosConcluidos.Sum(a =>
                a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);

            var cortesHoje = todosConcluidos.Where(a => a.DataHora.Date == hoje).ToList();
            var cortesSemana = todosConcluidos.Where(a => a.DataHora.Date >= inicioSemana).ToList();
            var cortesMes = todosConcluidos.Where(a => a.DataHora.Date >= inicioMes).ToList();

            var faturamentoHoje = cortesHoje.Sum(a =>
                a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);
            var faturamentoSemana = cortesSemana.Sum(a =>
                a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);
            var faturamentoMes = cortesMes.Sum(a =>
                a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);

            var cancelados = query.Where(a =>
                a.Status == AgendamentoStatus.CanceladoPeloCliente.ToString() ||
                a.Status == AgendamentoStatus.CanceladoPeloBarbeiro.ToString()).Count();

            var faltaram = query.Where(a =>
                a.Status == AgendamentoStatus.ClienteFaltou.ToString()).Count();

            var pendentes = query.Where(a =>
                a.Status == AgendamentoStatus.Pendente.ToString() ||
                a.Status == AgendamentoStatus.Confirmado.ToString() ||
                a.Status == AgendamentoStatus.LembreteEnviado.ToString()).Count();

            // Total de agendamentos finalizados (concluidos + faltas + cancelamentos) para calcular taxas
            var totalFinalizados = todosConcluidos.Count + cancelados + faltaram;

            return new RelatorioGeralResponse
            {
                TotalAtendimentos = todosConcluidos.Count,
                AtendimentosHoje = cortesHoje.Count,
                AtendimentosSemana = cortesSemana.Count,
                AtendimentosMes = cortesMes.Count,
                FaturamentoTotal = faturamentoTotal,
                FaturamentoHoje = faturamentoHoje,
                FaturamentoSemana = faturamentoSemana,
                FaturamentoMes = faturamentoMes,
                AgendamentosPendentes = pendentes,
                CancelamentosTotal = cancelados,
                ClientesFaltaram = faltaram,
                TaxaFaltas = totalFinalizados > 0 ? Math.Round((decimal)faltaram / totalFinalizados * 100, 2) : 0,
                TaxaCancelamento = totalFinalizados > 0 ? Math.Round((decimal)cancelados / totalFinalizados * 100, 2) : 0,
                TaxaConclusao = totalFinalizados > 0 ? Math.Round((decimal)todosConcluidos.Count / totalFinalizados * 100, 2) : 0
            };
        }

        public IEnumerable<ServicoMaisPedidoResponse> ObterServicosMaisPedidos(RelatorioFiltroRequest filtro, int top = 10)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString() && a.ServicoId.HasValue)
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias
                .AsQueryable();

            if (filtro.BarbeiroId.HasValue)
                query = query.Where(a => a.BarbeiroId == filtro.BarbeiroId.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var agendamentos = query.ToList();
            var totalGeral = agendamentos.Count;

            var agrupados = agendamentos
                .GroupBy(a => a.ServicoId!.Value)
                .Select(g =>
                {
                    var servico = _contexto.Servico.Find(g.Key);
                    return new ServicoMaisPedidoResponse
                    {
                        ServicoId = g.Key,
                        NomeServico = servico?.Descricao ?? "Desconhecido",
                        Categoria = servico?.Categoria,
                        Quantidade = g.Count(),
                        ValorTotal = g.Count() * (servico?.Valor ?? 0),
                        PercentualTotal = totalGeral > 0 ? Math.Round((decimal)g.Count() / totalGeral * 100, 2) : 0
                    };
                })
                .OrderByDescending(s => s.Quantidade)
                .Take(top)
                .ToList();

            return agrupados;
        }

        public IEnumerable<ClienteFrequenteResponse> ObterClientesFrequentes(RelatorioFiltroRequest filtro, int top = 10)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString())
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias
                .AsQueryable();

            if (filtro.BarbeiroId.HasValue)
                query = query.Where(a => a.BarbeiroId == filtro.BarbeiroId.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var agendamentos = query.ToList();

            var agrupados = agendamentos
                .GroupBy(a => a.NumeroCliente)
                .Select(g =>
                {
                    var ultimo = g.OrderByDescending(a => a.DataHora).First();
                    return new ClienteFrequenteResponse
                    {
                        NomeCliente = ultimo.NomeCliente,
                        NumeroCliente = g.Key,
                        TotalAtendimentos = g.Count(),
                        UltimoAtendimento = ultimo.DataHora,
                        TotalGasto = g.Sum(a =>
                            a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0)
                    };
                })
                .OrderByDescending(c => c.TotalAtendimentos)
                .Take(top)
                .ToList();

            return agrupados;
        }

        public IEnumerable<FaturamentoPorPeriodoResponse> ObterFaturamentoDiario(RelatorioFiltroRequest filtro)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString())
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias
                .AsQueryable();

            if (filtro.BarbeiroId.HasValue)
                query = query.Where(a => a.BarbeiroId == filtro.BarbeiroId.Value);

            // Padrao: ultimos 30 dias
            var dataInicio = filtro.DataInicio ?? DateTime.Today.AddDays(-30);
            var dataFim = filtro.DataFim ?? DateTime.Today;

            query = query.Where(a => a.DataHora >= dataInicio && a.DataHora < dataFim.AddDays(1));

            var agendamentos = query.ToList();

            var agrupados = agendamentos
                .GroupBy(a => a.DataHora.Date)
                .Select(g => new FaturamentoPorPeriodoResponse
                {
                    Periodo = g.Key.ToString("dd/MM"),
                    TotalAtendimentos = g.Count(),
                    Faturamento = g.Sum(a =>
                        a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0)
                })
                .OrderBy(f => f.Periodo)
                .ToList();

            return agrupados;
        }

        public IEnumerable<FaturamentoPorMetodoResponse> ObterFaturamentoPorMetodo(RelatorioFiltroRequest filtro)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString() && a.MetodoPagamento != null)
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias
                .AsQueryable();

            if (filtro.BarbeiroId.HasValue)
                query = query.Where(a => a.BarbeiroId == filtro.BarbeiroId.Value);

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var agendamentos = query.ToList();
            var total = agendamentos.Count;

            var agrupados = agendamentos
                .GroupBy(a => a.MetodoPagamento!)
                .Select(g => new FaturamentoPorMetodoResponse
                {
                    MetodoPagamento = g.Key,
                    Quantidade = g.Count(),
                    ValorTotal = g.Sum(a =>
                        a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0),
                    Percentual = total > 0 ? Math.Round((decimal)g.Count() / total * 100, 2) : 0
                })
                .OrderByDescending(f => f.Quantidade)
                .ToList();

            return agrupados;
        }

        public IEnumerable<RelatorioBarbeiroResponse> ObterRelatorioPorBarbeiro(RelatorioFiltroRequest filtro)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status != AgendamentoStatus.SlotReservado.ToString())
                .Where(a => a.AgendamentoPrincipalId == null) // Excluir etapas secundárias
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                query = query.Where(a => a.DataHora >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var agendamentos = query.ToList();

            var barbeiros = _contexto.Usuario
                .Where(u => u.Perfil == Perfil.Barbeiro.ToString() || u.Perfil == Perfil.BarbeiroAdministrador.ToString())
                .Where(u => !u.Excluido)
                .ToList();

            var resultado = barbeiros.Select(b =>
            {
                var agBarbeiro = agendamentos.Where(a => a.BarbeiroId == b.Id).ToList();
                var concluidos = agBarbeiro.Where(a => a.Status == AgendamentoStatus.Concluido.ToString()).ToList();
                var cancelados = agBarbeiro.Where(a =>
                    a.Status == AgendamentoStatus.CanceladoPeloCliente.ToString() ||
                    a.Status == AgendamentoStatus.CanceladoPeloBarbeiro.ToString()).Count();
                var faltaram = agBarbeiro.Where(a =>
                    a.Status == AgendamentoStatus.ClienteFaltou.ToString()).Count();

                var faturamentoServicos = concluidos.Sum(a =>
                    a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);

                // Somar valor dos adicionais nos agendamentos concluídos
                var faturamentoAdicionais = concluidos.Sum(a =>
                    _contexto.AgendamentoAdicional.Where(ad => ad.AgendamentoId == a.Id).Sum(ad => ad.Valor));

                var faturamento = faturamentoServicos + faturamentoAdicionais;

                var totalFinalizados = concluidos.Count + cancelados + faltaram;

                // Calcular comissão usando porcentagem histórica por agendamento
                var comissaoTotal = concluidos.Sum(a =>
                {
                    if (a.PorcentagemAdminNaEpoca.HasValue && a.PorcentagemAdminNaEpoca.Value > 0)
                    {
                        var valorServico = a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0;
                        var valorAdicionais = _contexto.AgendamentoAdicional.Where(ad => ad.AgendamentoId == a.Id).Sum(ad => ad.Valor);
                        return Math.Round((valorServico + valorAdicionais) * a.PorcentagemAdminNaEpoca.Value / 100, 2);
                    }
                    return 0m;
                });

                var response = new RelatorioBarbeiroResponse
                {
                    BarbeiroId = b.Id,
                    NomeBarbeiro = b.Nome,
                    TotalAtendimentos = concluidos.Count,
                    Faturamento = faturamento,
                    CancelamentosTotal = cancelados,
                    ClientesFaltaram = faltaram,
                    TaxaConclusao = totalFinalizados > 0 ? Math.Round((decimal)concluidos.Count / totalFinalizados * 100, 2) : 0
                };

                // Usar comissão histórica se houver dados, senão fallback para porcentagem atual
                if (comissaoTotal > 0)
                {
                    response.PorcentagemAdmin = b.Porcentagem ?? 0;
                    response.FaturamentoBruto = faturamento;
                    response.ValorComissaoAdmin = comissaoTotal;
                    response.FaturamentoLiquido = faturamento - comissaoTotal;
                }
                else if (b.Porcentagem.HasValue && b.Porcentagem.Value > 0)
                {
                    // Fallback para agendamentos antigos sem PorcentagemAdminNaEpoca
                    response.PorcentagemAdmin = b.Porcentagem.Value;
                    response.FaturamentoBruto = faturamento;
                    response.ValorComissaoAdmin = Math.Round(faturamento * b.Porcentagem.Value / 100, 2);
                    response.FaturamentoLiquido = faturamento - response.ValorComissaoAdmin.Value;
                }

                return response;
            }).OrderByDescending(r => r.TotalAtendimentos).ToList();

            return resultado;
        }

        public RelatorioGeralResponse ObterRelatorioGeralBarbeiro(RelatorioFiltroRequest filtro, int barbeiroId)
        {
            // Obter relatório geral filtrado pelo barbeiro
            filtro.BarbeiroId = barbeiroId;
            var relatorio = ObterRelatorioGeral(filtro);

            // Calcular comissão usando porcentagem histórica por agendamento
            var queryBarbeiro = _contexto.Agendamento
                .Where(a => a.BarbeiroId == barbeiroId && a.Status == AgendamentoStatus.Concluido.ToString())
                .Where(a => a.AgendamentoPrincipalId == null)
                .AsQueryable();

            if (filtro.DataInicio.HasValue)
                queryBarbeiro = queryBarbeiro.Where(a => a.DataHora >= filtro.DataInicio.Value);
            if (filtro.DataFim.HasValue)
                queryBarbeiro = queryBarbeiro.Where(a => a.DataHora < filtro.DataFim.Value.AddDays(1));

            var concluidosBarbeiro = queryBarbeiro.ToList();
            var hoje = DateTime.Today;
            var inicioSemana = hoje.AddDays(-(int)hoje.DayOfWeek);
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

            // Função para calcular comissão de uma lista de agendamentos
            decimal CalcularComissao(IEnumerable<Agendamento> agendamentos)
            {
                return agendamentos.Sum(a =>
                {
                    var pct = a.PorcentagemAdminNaEpoca;
                    if (!pct.HasValue || pct.Value <= 0)
                    {
                        // Fallback para porcentagem atual do barbeiro
                        var barb = _contexto.Usuario.Find(a.BarbeiroId);
                        pct = barb?.Porcentagem;
                    }
                    if (pct.HasValue && pct.Value > 0)
                    {
                        var valorServico = a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0;
                        var valorAdicionais = _contexto.AgendamentoAdicional.Where(ad => ad.AgendamentoId == a.Id).Sum(ad => ad.Valor);
                        return Math.Round((valorServico + valorAdicionais) * pct.Value / 100, 2);
                    }
                    return 0m;
                });
            }

            var comissaoTotal = CalcularComissao(concluidosBarbeiro);
            var comissaoHoje = CalcularComissao(concluidosBarbeiro.Where(a => a.DataHora.Date == hoje));
            var comissaoSemana = CalcularComissao(concluidosBarbeiro.Where(a => a.DataHora.Date >= inicioSemana));
            var comissaoMes = CalcularComissao(concluidosBarbeiro.Where(a => a.DataHora.Date >= inicioMes));

            if (comissaoTotal > 0)
            {
                relatorio.FaturamentoTotal -= comissaoTotal;
                relatorio.FaturamentoHoje -= comissaoHoje;
                relatorio.FaturamentoSemana -= comissaoSemana;
                relatorio.FaturamentoMes -= comissaoMes;
            }

            return relatorio;
        }
    }
}
