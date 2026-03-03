using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
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

            var pendentes = query.Where(a =>
                a.Status == AgendamentoStatus.Pendente.ToString() ||
                a.Status == AgendamentoStatus.Confirmado.ToString() ||
                a.Status == AgendamentoStatus.LembreteEnviado.ToString()).Count();

            return new RelatorioGeralResponse
            {
                TotalCortes = todosConcluidos.Count,
                CortesHoje = cortesHoje.Count,
                CortesSemana = cortesSemana.Count,
                CortesMes = cortesMes.Count,
                FaturamentoTotal = faturamentoTotal,
                FaturamentoHoje = faturamentoHoje,
                FaturamentoSemana = faturamentoSemana,
                FaturamentoMes = faturamentoMes,
                TicketMedio = todosConcluidos.Count > 0 ? faturamentoTotal / todosConcluidos.Count : 0,
                AgendamentosPendentes = pendentes,
                CancelamentosTotal = cancelados
            };
        }

        public IEnumerable<ServicoMaisPedidoResponse> ObterServicosMaisPedidos(RelatorioFiltroRequest filtro, int top = 10)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString() && a.ServicoId.HasValue)
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
                        TotalCortes = g.Count(),
                        UltimoCorte = ultimo.DataHora,
                        TotalGasto = g.Sum(a =>
                            a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0)
                    };
                })
                .OrderByDescending(c => c.TotalCortes)
                .Take(top)
                .ToList();

            return agrupados;
        }

        public IEnumerable<FaturamentoPorPeriodoResponse> ObterFaturamentoDiario(RelatorioFiltroRequest filtro)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status == AgendamentoStatus.Concluido.ToString())
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
                    TotalCortes = g.Count(),
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

                var faturamento = concluidos.Sum(a =>
                    a.ServicoId.HasValue ? (_contexto.Servico.Find(a.ServicoId.Value)?.Valor ?? 0) : 0);

                return new RelatorioBarbeiroResponse
                {
                    BarbeiroId = b.Id,
                    NomeBarbeiro = b.Nome,
                    TotalCortes = concluidos.Count,
                    Faturamento = faturamento,
                    TicketMedio = concluidos.Count > 0 ? faturamento / concluidos.Count : 0,
                    CancelamentosTotal = cancelados
                };
            }).OrderByDescending(r => r.TotalCortes).ToList();

            return resultado;
        }
    }
}
