using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Agendamento;
using BarbeariaRocha.Modelos.Response.Agendamento;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AgendamentoApp(Contexto contexto) : IAgendamentoApp
    {
        private readonly Contexto _contexto = contexto;
        public AgendamentoDetalheResponse AgendamentoAtual(int barbeiroId)
        {
            var agendamento = _contexto.Agendamento
                .Where(a => a.BarbeiroId == barbeiroId &&
                           a.Status != AgendamentoStatus.Concluido.ToString() &&
                           a.Status != AgendamentoStatus.CanceladoPeloCliente.ToString() &&
                           a.Status != AgendamentoStatus.CanceladoPeloBarbeiro.ToString() &&
                           a.Status != AgendamentoStatus.ClienteFaltou.ToString() &&
                           a.Status != AgendamentoStatus.SlotReservado.ToString())
                .OrderBy(a => a.DataHora)
                .FirstOrDefault() ?? throw new Exception("Nenhum agendamento ativo encontrado.");

            var barbeiro = _contexto.Usuario.Find(agendamento.BarbeiroId) ?? throw new Exception("Barbeiro não encontrado.");
            var servico = agendamento.ServicoId.HasValue ? _contexto.Servico.Find(agendamento.ServicoId.Value) : null;

            return new AgendamentoDetalheResponse
            {
                Id = agendamento.Id,
                NomeCliente = agendamento.NomeCliente,
                NomeBarbeiro = barbeiro.Nome,
                NumeroCliente = agendamento.NumeroCliente,
                Status = agendamento.Status,
                Data = agendamento.DataHora,
                Servico = servico?.Descricao ?? "Serviço não encontrado",
                DescricaoEtapa = agendamento.DescricaoEtapa,
                AgendamentoPrincipalId = agendamento.AgendamentoPrincipalId
            };
        }

        public void EditarECompletarAgendamento(int id, AgendamentoEditarRequest request)
        {
            var agendamento = _contexto.Agendamento
                .FirstOrDefault(a => a.Id == id) ?? throw new Exception("Agendamento não encontrado.");

            var novoServico = _contexto.Servico.Find(request.ServicoId) ?? throw new Exception("Serviço não encontrado.");

            agendamento.ServicoId = novoServico.Id;

            agendamento.MetodoPagamento = request.MetodoPagamento.ToString();
            agendamento.Status = AgendamentoStatus.Concluido.ToString();

            // Completar slots complementares associados
            CompletarSlotsComplementares(id);

            _contexto.SaveChanges();
        }

        public void CancelarAgendamento(int id)
        {
            var agendamento = _contexto.Agendamento.Find(id) ?? throw new Exception("Agendamento não encontrado.");
            agendamento.Status = AgendamentoStatus.CanceladoPeloBarbeiro.ToString();

            // Cancelar slots complementares e etapas vinculadas
            CancelarVinculados(id);

            _contexto.SaveChanges();
        }

        public void MarcarClienteFaltou(int id)
        {
            var agendamento = _contexto.Agendamento.Find(id) ?? throw new Exception("Agendamento não encontrado.");
            agendamento.Status = AgendamentoStatus.ClienteFaltou.ToString();

            // Marcar falta em slots complementares e etapas vinculadas
            CancelarVinculados(id);

            _contexto.SaveChanges();
        }

        public void CompletarAgendamento(int id, AgendamentoCompletarRequest request)
        {
            var agendamento = _contexto.Agendamento.Find(id) ?? throw new Exception("Agendamento não encontrado.");
            agendamento.Status = AgendamentoStatus.Concluido.ToString();
            agendamento.MetodoPagamento = request.MetodoPagamento.ToString();

            // Completar slots complementares associados
            CompletarSlotsComplementares(id);

            _contexto.SaveChanges();
        }

        public void CriarAgendamento(AgendamentoCriarRequest request)
        {
            var tokenValido = _contexto.CodigoConfirmacao
               .FirstOrDefault(t => t.Numero == request.Numero
                                   && t.Codigo == request.CodigoConfirmacao
                                   && !t.Confirmado
                                   && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow) ?? throw new Exception("Código de confirmação inválido ou expirado.");

            tokenValido.Confirmado = true;

            if (request.DtAgendamento == default)
                throw new ArgumentException("Data e hora do agendamento são obrigatórios.", nameof(request.DtAgendamento));

            // Verificar se existe exceção para essa data
            var dataAgendamento = request.DtAgendamento.Date;
            var existeExcecao = _contexto.Excecao
                .Any(e => e.Excluido == false &&
                         e.Data.Date == dataAgendamento &&
                         (e.BarbeiroId == null || e.BarbeiroId == request.BarbeiroId));

            if (existeExcecao)
            {
                var excecao = _contexto.Excecao
                    .FirstOrDefault(e => e.Excluido == false &&
                                        e.Data.Date == dataAgendamento &&
                                        (e.BarbeiroId == null || e.BarbeiroId == request.BarbeiroId));
                throw new Exception($"Não é possível agendar nesta data. Motivo: {excecao?.Descricao}");
            }

            var barbeiro = _contexto.Usuario.Find(request.BarbeiroId) ?? throw new Exception("Barbeiro não encontrado.");
            var servico = _contexto.Servico.Find(request.ServicoId) ?? throw new Exception("Serviço não encontrado.");

            var usuarioLogado = _contexto.Usuario.Find(request.UsuarioId);

            if (usuarioLogado != null)
            {
                request.Numero = usuarioLogado.Numero;
                request.Nome = usuarioLogado.Nome;
            }

            var ultimoAgendamento = _contexto.Agendamento
            .Where(x => x.NumeroCliente == request.Numero && x.Status == AgendamentoStatus.Concluido.ToString())
            .OrderByDescending(x => x.DataHora)
            .FirstOrDefault();

            if (ultimoAgendamento != null && ultimoAgendamento.DataHora.AddDays(6) >= request.DtAgendamento)
                throw new Exception("Seu ultimo corte foi a menos de 7 dias");

            var tempoTotal = servico.TempoEstimado;
            var ocuparMaisSlot = tempoTotal.Hour > 0 || tempoTotal.Minute > 40;

            // ==================== SERVIÇO COM 2 ETAPAS (ex: Platinado com Corte) ====================
            if (servico.RequerDuasEtapas)
            {
                if (request.DtAgendamentoEtapa2 == null || request.DtAgendamentoEtapa2 == default)
                    throw new ArgumentException("Para este serviço, é necessário informar o horário da segunda etapa.");

                var dataEtapa1 = DateTime.SpecifyKind(request.DtAgendamento, DateTimeKind.Unspecified);
                var dataEtapa2 = DateTime.SpecifyKind(request.DtAgendamentoEtapa2.Value, DateTimeKind.Unspecified);

                // Validar intervalo mínimo
                var diferencaHoras = (dataEtapa2 - dataEtapa1).TotalHours;
                if (diferencaHoras < servico.IntervaloMinimoHoras)
                    throw new Exception($"O intervalo entre as etapas deve ser de pelo menos {servico.IntervaloMinimoHoras} horas.");

                // Para serviços com 2 etapas, cada etapa ocupa seus próprios slots.
                // Se o tempo > 40min, cada etapa ocupa 2 slots consecutivos (validados pelo tempoTotal).
                // Se o tempo <= 40min, cada etapa ocupa 1 slot (validado com tempo de 40min).
                var tempoValidacao = tempoTotal;

                // Verificar conflito etapa 1
                ValidarConflito(request.BarbeiroId, dataEtapa1, tempoValidacao);
                // Verificar conflito etapa 2
                ValidarConflito(request.BarbeiroId, dataEtapa2, tempoValidacao);

                // Criar agendamento etapa 1
                var agendamentoEtapa1 = new Agendamento(request)
                {
                    DescricaoEtapa = servico.DescricaoEtapa1 ?? "Etapa 1"
                };
                agendamentoEtapa1.DataHora = dataEtapa1;
                _contexto.Agendamento.Add(agendamentoEtapa1);
                _contexto.SaveChanges();

                // Criar slot complementar etapa 1 se necessário
                if (ocuparMaisSlot)
                {
                    CriarSlotComplementar(request.BarbeiroId, agendamentoEtapa1);
                }

                // Criar agendamento etapa 2
                var agendamentoEtapa2 = new Agendamento
                {
                    UsuarioId = agendamentoEtapa1.UsuarioId,
                    BarbeiroId = request.BarbeiroId,
                    ServicoId = request.ServicoId,
                    NomeCliente = agendamentoEtapa1.NomeCliente,
                    NumeroCliente = agendamentoEtapa1.NumeroCliente,
                    Status = AgendamentoStatus.Pendente.ToString(),
                    MetodoPagamento = null,
                    DataHora = dataEtapa2,
                    AgendamentoPrincipalId = agendamentoEtapa1.Id,
                    DescricaoEtapa = servico.DescricaoEtapa2 ?? "Etapa 2"
                };
                _contexto.Agendamento.Add(agendamentoEtapa2);
                _contexto.SaveChanges();

                // Criar slot complementar etapa 2 se necessário
                if (ocuparMaisSlot)
                {
                    CriarSlotComplementar(request.BarbeiroId, agendamentoEtapa2);
                }

                return;
            }

            // ==================== SERVIÇO NORMAL ====================
            var agendamento = new Agendamento(request);

            var dataInicio = DateTime.SpecifyKind(request.DtAgendamento, DateTimeKind.Unspecified);
            ValidarConflito(request.BarbeiroId, dataInicio, tempoTotal);

            agendamento.DataHora = DateTime.SpecifyKind(agendamento.DataHora, DateTimeKind.Unspecified);

            _contexto.Agendamento.Add(agendamento);
            _contexto.SaveChanges();

            if (ocuparMaisSlot)
            {
                CriarSlotComplementar(request.BarbeiroId, agendamento);
            }
        }

        private void ValidarConflito(int barbeiroId, DateTime dataInicio, TimeOnly tempoTotal)
        {
            var dataFim = dataInicio;
            if (tempoTotal.Hour > 0)
                dataFim = dataFim.AddHours(tempoTotal.Hour);
            if (tempoTotal.Minute > 0)
                dataFim = dataFim.AddMinutes(tempoTotal.Minute);

            var conflito = _contexto.Agendamento.Any(a =>
                            a.BarbeiroId == barbeiroId &&
                            a.Status != AgendamentoStatus.CanceladoPeloCliente.ToString() &&
                            a.Status != AgendamentoStatus.CanceladoPeloBarbeiro.ToString() &&
                            a.DataHora >= dataInicio &&
                            a.DataHora < dataFim);

            if (conflito)
                throw new Exception("O barbeiro já possui um agendamento nesse horário. Por favor, escolha outro horário.");
        }

        private void CompletarSlotsComplementares(int agendamentoId)
        {
            var slots = _contexto.Agendamento
                .Where(a => a.AgendamentoPrincipalId == agendamentoId &&
                           a.Status == AgendamentoStatus.SlotReservado.ToString())
                .ToList();

            foreach (var slot in slots)
                slot.Status = AgendamentoStatus.Concluido.ToString();
        }

        private void CancelarVinculados(int agendamentoId)
        {
            var vinculados = _contexto.Agendamento
                .Where(a => a.AgendamentoPrincipalId == agendamentoId &&
                           a.Status != AgendamentoStatus.Concluido.ToString())
                .ToList();

            foreach (var v in vinculados)
                v.Status = AgendamentoStatus.CanceladoPeloBarbeiro.ToString();
        }

        private void CriarSlotComplementar(int barbeiroId, Agendamento agendamentoPrincipal)
        {
            var agendamentoSlot = new Agendamento
            {
                BarbeiroId = barbeiroId,
                DataHora = DateTime.SpecifyKind(agendamentoPrincipal.DataHora.AddMinutes(40), DateTimeKind.Unspecified),
                Status = AgendamentoStatus.SlotReservado.ToString(),
                NomeCliente = $"Slot complementar do agendamento {agendamentoPrincipal.Id}",
                NumeroCliente = "00000000000",
                AgendamentoPrincipalId = agendamentoPrincipal.Id
            };
            _contexto.Agendamento.Add(agendamentoSlot);
            _contexto.SaveChanges();
        }

        public List<HorariosOcupadosResponse> HorariosOcupadosBarbeiro(HorarioRequest request)
        {
            var agendamentos = _contexto.Agendamento
                .Where(a => a.BarbeiroId == request.IdBarbeiro &&
                           a.DataHora.Date == request.Data.Date &&
                           a.Status != AgendamentoStatus.CanceladoPeloCliente.ToString() &&
                           a.Status != AgendamentoStatus.CanceladoPeloBarbeiro.ToString())
                .OrderBy(a => a.DataHora)
                .ToList();

            var horariosOcupadosPorData = agendamentos
                .GroupBy(a => a.DataHora.Date)
                .Select(g => new HorariosOcupadosResponse
                {
                    Data = g.Key,
                    Horarios = g.Select(a => a.DataHora.TimeOfDay).OrderBy(h => h)
                })
                .ToList();

            return horariosOcupadosPorData;
        }

        public PaginacaoResultado<AgendamentoDetalheResponse> ListarAgendamentos(PaginacaoFiltro<AgendamentoFiltroRequest> request)
        {
            var query = _contexto.Agendamento
                .Where(a => a.Status != AgendamentoStatus.SlotReservado.ToString())
                .AsQueryable();

            if (request.Filtro?.BarbeiroId != null)
                query = query.Where(a => a.BarbeiroId == request.Filtro.BarbeiroId.Value);

            if (request.Filtro?.UsuarioId != null)
                query = query.Where(a => a.UsuarioId == request.Filtro.UsuarioId.Value);

            if (request.Filtro?.DtAgendamento != null)
            {
                var dataInicio = request.Filtro.DtAgendamento.Value.Date;
                query = query.Where(a => a.DataHora >= dataInicio);
            }

            if (request.Filtro?.Servicos != null && request.Filtro.Servicos.Any())
            {
                query = query.Where(a => a.ServicoId != null && request.Filtro.Servicos.Contains(a.ServicoId.Value));
            }

            if (!string.IsNullOrEmpty(request.Filtro?.Status))
            {
                query = query.Where(a => a.Status == request.Filtro.Status);
            }

            var totalRegistros = query.Count();

            var agendamentos = query
                .OrderBy(a => a.DataHora)
                .Skip((request.Pagina - 1) * request.ItensPorPagina)
                .Take(request.ItensPorPagina)
                .ToList();

            var resultado = agendamentos.Select(a =>
            {
                var barbeiro = _contexto.Usuario.Find(a.BarbeiroId);
                var servico = a.ServicoId.HasValue ? _contexto.Servico.Find(a.ServicoId.Value) : null;
                return new AgendamentoDetalheResponse
                {
                    Id = a.Id,
                    NomeCliente = a.NomeCliente,
                    NomeBarbeiro = barbeiro?.Nome ?? "Barbeiro não encontrado",
                    NumeroCliente = a.NumeroCliente,
                    Status = a.Status,
                    Data = a.DataHora,
                    Servico = servico?.Descricao ?? "Serviço não encontrado",
                    DescricaoEtapa = a.DescricaoEtapa,
                    AgendamentoPrincipalId = a.AgendamentoPrincipalId
                };
            }).ToList();

            return new PaginacaoResultado<AgendamentoDetalheResponse>
            {
                Items = resultado,
                TotalRegistros = totalRegistros,
                PaginaAtual = request.Pagina,
                ItensPorPagina = request.ItensPorPagina
            };
        }

        public AgendamentoDetalheResponse ObterPorId(int id)
        {
            var agendamento = _contexto.Agendamento
                .FirstOrDefault(a => a.Id == id) ?? throw new Exception("Agendamento não encontrado.");

            var servicoAgendamento = _contexto.Servico.Find(agendamento.ServicoId) ?? throw new Exception("Serviço não encontrado.");

            var barbeiro = _contexto.Usuario.Find(agendamento.BarbeiroId) ?? throw new Exception("Barbeiro não encontrado.");

            return new AgendamentoDetalheResponse
            {
                Id = agendamento.Id,
                NomeCliente = agendamento.NomeCliente,
                NomeBarbeiro = barbeiro.Nome,
                NumeroCliente = agendamento.NumeroCliente,
                Status = agendamento.Status,
                Data = agendamento.DataHora,
                Servico = servicoAgendamento.Descricao,
                DescricaoEtapa = agendamento.DescricaoEtapa,
                AgendamentoPrincipalId = agendamento.AgendamentoPrincipalId
            };
        }

        public void GerarToken(string numero)
        {
            var tokenAtivo = _contexto.CodigoConfirmacao
                .FirstOrDefault(t => t.Numero == numero && !t.Confirmado && t.DtExpiracao.ToUniversalTime() > DateTime.UtcNow);

            var ultimoAgendamento = _contexto.Agendamento
                .Where(x => x.NumeroCliente == numero && x.Status == AgendamentoStatus.Concluido.ToString())
                .OrderByDescending(x => x.DataHora)
                .FirstOrDefault();

            if (ultimoAgendamento != null && ultimoAgendamento.DataHora.AddDays(7) >= DateTime.UtcNow)
                throw new Exception("Seu ultimo corte foi a menos de 7 dias");

            if (tokenAtivo != null && tokenAtivo.Reenviado)
                throw new Exception("Já foi enviado um código de confirmação. Por favor, verifique seu telefone.");

            if (tokenAtivo != null && !tokenAtivo.Reenviado)
            {
                HelperGenerico.EnviarMensagem(tokenAtivo.Codigo.ToString(), numero);
                tokenAtivo.Reenviado = true;
                _contexto.SaveChanges();
                return;
            }

            var codigo = HelperGenerico.GerarCodigoConfirmacao();
            var salvarToken = new CodigoConfirmacao(numero, codigo);

            _contexto.CodigoConfirmacao.Add(salvarToken);
            _contexto.SaveChanges();
            HelperGenerico.EnviarMensagem(codigo.ToString(), numero);
        }
    }
}
