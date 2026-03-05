using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Request.Mensalista;
using BarbeariaRocha.Modelos.Response.Mensalista;
using System.Globalization;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class MensalistaApp(Contexto contexto) : IMensalistaApp
    {
        private readonly Contexto _contexto = contexto;

        public void CadastrarMensalista(MensalistaCriarRequest request, int idUsuario)
        {
            ValidarDadosMensalista(request);
            Usuario.ValidarNumero(request.Numero);

            var dia = CultureInfo
                    .GetCultureInfo("pt-BR")
                    .DateTimeFormat
                    .GetDayName(request.Dia);

            // Validar se já existe mensalista com mesmo barbeiro, dia e horário
            if (request.BarbeiroId.HasValue && !string.IsNullOrEmpty(request.Horario))
            {
                var duplicado = _contexto.Mensalista
                    .Any(m => m.BarbeiroId == request.BarbeiroId.Value
                        && m.Dia == dia
                        && m.Horario == request.Horario
                        && m.Status == MensalistaStatus.Ativo.ToString());

                if (duplicado)
                    throw new Exception("Já existe um mensalista ativo para este barbeiro neste dia e horário.");
            }

            var mensalista = new Mensalista
            {
                Nome = request.Nome,
                Numero = request.Numero,
                Valor = request.Valor,
                Dia = dia,
                Tipo = request.Tipo.ToString(),
                Status = MensalistaStatus.Ativo.ToString(),
                Horario = request.Horario,
                BarbeiroId = request.BarbeiroId,
                ServicoId = request.ServicoId
            };

            _contexto.Mensalista.Add(mensalista);
            _contexto.SaveChanges();

            // Gerar agendamentos para este mensalista (mês atual + próximo)
            if (!string.IsNullOrEmpty(request.Horario) && request.BarbeiroId.HasValue)
            {
                GerarAgendamentosParaMensalista(mensalista, request.Dia);
            }
        }

        public void CancelarMensalista(int idMensalista)
        {
            var mensalista = _contexto.Mensalista.Find(idMensalista)
                ?? throw new Exception("Mensalista não encontrado.");

            // Cancelar agendamentos futuros auto-gerados deste mensalista
            var agora = DateTime.Now;
            var prefixo = $"Mensalista: {mensalista.Nome}";
            var agendamentosFuturos = _contexto.Agendamento
                .Where(a => a.NomeCliente == prefixo
                    && a.NumeroCliente == mensalista.Numero
                    && a.DataHora > agora
                    && a.Status == AgendamentoStatus.Pendente.ToString())
                .ToList();

            foreach (var ag in agendamentosFuturos)
            {
                ag.Status = AgendamentoStatus.CanceladoPeloBarbeiro.ToString();
            }

            _contexto.Mensalista.Remove(mensalista);
            _contexto.SaveChanges();
        }

        public IEnumerable<MensalistaResponse> ObterTodos()
        {
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fimMes = inicioMes.AddMonths(1);
            var statusConcluido = AgendamentoStatus.Concluido.ToString();

            var mensalistas = _contexto.Mensalista
                .Select(m => new MensalistaResponse
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Numero = m.Numero,
                    Tipo = m.Tipo,
                    Status = m.Status,
                    Dia = m.Dia,
                    Valor = m.Valor,
                    Horario = m.Horario,
                    BarbeiroId = m.BarbeiroId,
                    NomeBarbeiro = m.BarbeiroId.HasValue
                        ? _contexto.Usuario.Where(u => u.Id == m.BarbeiroId.Value).Select(u => u.Nome).FirstOrDefault()
                        : null,
                    ServicoId = m.ServicoId,
                    NomeServico = m.ServicoId.HasValue
                        ? _contexto.Servico.Where(s => s.Id == m.ServicoId.Value).Select(s => s.Descricao).FirstOrDefault()
                        : null,
                    CortesNoMes = _contexto.MensalistaCorte
                        .Count(c => c.MensalistaId == m.Id && c.DataCorte >= inicioMes && c.DataCorte < fimMes),
                    AtendimentosNoMes = _contexto.Agendamento
                        .Count(a => a.NumeroCliente == m.Numero
                            && a.Status == statusConcluido
                            && a.DataHora >= inicioMes
                            && a.DataHora < fimMes),
                    Cortes = _contexto.MensalistaCorte
                        .Where(c => c.MensalistaId == m.Id)
                        .OrderByDescending(c => c.DataCorte)
                        .Select(c => new MensalistaCorteResponse
                        {
                            Id = c.Id,
                            DataCorte = c.DataCorte,
                            Observacao = c.Observacao
                        })
                        .ToList()
                })
                .ToList();

            return mensalistas;
        }

        public void RegistrarCorte(MensalistaRegistrarCorteRequest request)
        {
            var mensalista = _contexto.Mensalista.Find(request.MensalistaId)
                ?? throw new Exception("Mensalista não encontrado.");

            if (mensalista.Status != MensalistaStatus.Ativo.ToString())
                throw new Exception("Mensalista não está ativo.");

            var corte = new MensalistaCorte
            {
                MensalistaId = request.MensalistaId,
                DataCorte = DateTime.SpecifyKind(request.DataCorte, DateTimeKind.Unspecified),
                Observacao = request.Observacao
            };

            _contexto.MensalistaCorte.Add(corte);
            _contexto.SaveChanges();
        }

        public IEnumerable<MensalistaCorteResponse> ListarCortes(int mensalistaId, int? mes = null, int? ano = null)
        {
            var query = _contexto.MensalistaCorte
                .Where(c => c.MensalistaId == mensalistaId);

            if (mes.HasValue && ano.HasValue)
            {
                var inicio = new DateTime(ano.Value, mes.Value, 1);
                var fim = inicio.AddMonths(1);
                query = query.Where(c => c.DataCorte >= inicio && c.DataCorte < fim);
            }

            return query
                .OrderByDescending(c => c.DataCorte)
                .Select(c => new MensalistaCorteResponse
                {
                    Id = c.Id,
                    DataCorte = c.DataCorte,
                    Observacao = c.Observacao
                })
                .ToList();
        }

        public void DeletarCorte(int corteId)
        {
            var corte = _contexto.MensalistaCorte.Find(corteId)
                ?? throw new Exception("Corte não encontrado.");

            _contexto.MensalistaCorte.Remove(corte);
            _contexto.SaveChanges();
        }

        /// <summary>
        /// Gera agendamentos para todos os mensalistas ativos que possuem horário e barbeiro definidos.
        /// Cria agendamentos para o mês atual e o próximo mês, pulando datas já ocupadas ou no passado.
        /// </summary>
        public void GerarAgendamentosMensalistas()
        {
            var mensalistas = _contexto.Mensalista
                .Where(m => m.Status == MensalistaStatus.Ativo.ToString()
                    && m.Horario != null
                    && m.BarbeiroId != null)
                .ToList();

            var culture = CultureInfo.GetCultureInfo("pt-BR");

            foreach (var mensalista in mensalistas)
            {
                // Descobrir o DayOfWeek a partir do nome do dia em português
                DayOfWeek? diaSemana = null;
                for (int i = 0; i < 7; i++)
                {
                    var dow = (DayOfWeek)i;
                    if (culture.DateTimeFormat.GetDayName(dow).Equals(mensalista.Dia, StringComparison.OrdinalIgnoreCase))
                    {
                        diaSemana = dow;
                        break;
                    }
                }

                if (diaSemana == null) continue;

                GerarAgendamentosParaMensalista(mensalista, diaSemana.Value);
            }
        }

        /// <summary>
        /// Gera agendamentos para um mensalista específico no mês atual e próximo.
        /// </summary>
        private void GerarAgendamentosParaMensalista(Mensalista mensalista, DayOfWeek diaSemana)
        {
            if (string.IsNullOrEmpty(mensalista.Horario) || !mensalista.BarbeiroId.HasValue)
                return;

            var horario = TimeOnly.Parse(mensalista.Horario);
            var barbeiroId = mensalista.BarbeiroId.Value;
            var prefixo = $"Mensalista: {mensalista.Nome}";
            var agora = DateTime.Now;

            // Datas do mês atual e próximo mês
            var inicioMesAtual = new DateTime(agora.Year, agora.Month, 1);
            var fimProximoMes = inicioMesAtual.AddMonths(2);

            // Encontrar todas as ocorrências do dia da semana no período
            var datasParaAgendar = new List<DateTime>();
            var data = inicioMesAtual;
            while (data < fimProximoMes)
            {
                if (data.DayOfWeek == diaSemana)
                {
                    var dataHora = data.Add(horario.ToTimeSpan());
                    // Não criar agendamentos no passado
                    if (dataHora > agora)
                    {
                        datasParaAgendar.Add(dataHora);
                    }
                }
                data = data.AddDays(1);
            }

            // Verificar quais datas já possuem agendamento (deste mensalista ou outro)
            var statusCancelado1 = AgendamentoStatus.CanceladoPeloCliente.ToString();
            var statusCancelado2 = AgendamentoStatus.CanceladoPeloBarbeiro.ToString();

            foreach (var dataHora in datasParaAgendar)
            {
                // Verificar se já existe agendamento deste mensalista nesta data/hora
                var jaExisteMensalista = _contexto.Agendamento
                    .Any(a => a.NomeCliente == prefixo
                        && a.NumeroCliente == mensalista.Numero
                        && a.BarbeiroId == barbeiroId
                        && a.DataHora == dataHora
                        && a.Status != statusCancelado1
                        && a.Status != statusCancelado2);

                if (jaExisteMensalista) continue;

                // Verificar se o horário está ocupado por outro agendamento
                var horarioOcupado = _contexto.Agendamento
                    .Any(a => a.BarbeiroId == barbeiroId
                        && a.DataHora == dataHora
                        && a.Status != statusCancelado1
                        && a.Status != statusCancelado2);

                if (horarioOcupado) continue;

                // Verificar se existe exceção para esta data
                var existeExcecao = _contexto.Excecao
                    .Any(e => !e.Excluido
                        && e.Data.Date == dataHora.Date
                        && (e.BarbeiroId == null || e.BarbeiroId == barbeiroId));

                if (existeExcecao) continue;

                // Criar o agendamento
                var agendamento = new Agendamento
                {
                    NomeCliente = prefixo,
                    NumeroCliente = mensalista.Numero,
                    BarbeiroId = barbeiroId,
                    ServicoId = mensalista.ServicoId,
                    DataHora = dataHora,
                    Status = AgendamentoStatus.Pendente.ToString()
                };

                _contexto.Agendamento.Add(agendamento);
            }

            _contexto.SaveChanges();
        }

        private static void ValidarDadosMensalista(MensalistaCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome do mensalista é obrigatório.", nameof(request.Nome));

            if (string.IsNullOrWhiteSpace(request.Numero))
                throw new ArgumentException("O número do mensalista é obrigatório.", nameof(request.Numero));

            if (request.Valor <= 0)
                throw new ArgumentException("O valor deve ser maior que zero.", nameof(request.Valor));

            if (!Enum.IsDefined(typeof(DayOfWeek), request.Dia))
                throw new ArgumentException("O dia é obrigatório.", nameof(request.Dia));

            if (!Enum.IsDefined(typeof(MensalistaTipo), request.Tipo))
                throw new ArgumentException("O tipo é obrigatório.", nameof(request.Tipo));

        }
    }
}
