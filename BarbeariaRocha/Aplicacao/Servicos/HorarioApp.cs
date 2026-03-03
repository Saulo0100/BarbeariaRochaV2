using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Response.Horario;
using System.Globalization;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class HorarioApp(Contexto contexto) : IHorarioApp
    {
        private readonly Contexto _contexto = contexto;

        public HorariosDisponiveisResponse ObterHorariosDisponiveis(int barbeiroId, DateTime data)
        {
            var diaSemana = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(data.DayOfWeek);
            var aberto = HelperGenerico.BarbeariaAberta(data);

            if (!aberto)
            {
                return new HorariosDisponiveisResponse
                {
                    Data = data,
                    Aberto = false,
                    DiaSemana = diaSemana,
                    HorariosDisponiveis = new List<string>(),
                    HorariosOcupados = new List<string>()
                };
            }

            // Verificar excecoes para essa data
            var existeExcecao = _contexto.Excecao
                .Any(e => !e.Excluido &&
                         e.Data.Date == data.Date &&
                         (e.BarbeiroId == null || e.BarbeiroId == barbeiroId));

            if (existeExcecao)
            {
                return new HorariosDisponiveisResponse
                {
                    Data = data,
                    Aberto = false,
                    DiaSemana = diaSemana,
                    HorariosDisponiveis = new List<string>(),
                    HorariosOcupados = new List<string>()
                };
            }

            var todosHorarios = HelperGenerico.ObterHorariosPorData(data);
            var horariosOcupados = ObterHorariosOcupados(barbeiroId, data);

            var agora = DateTime.Now;
            var horariosDisponiveis = FiltrarHorariosDisponiveis(todosHorarios, horariosOcupados, data, agora);

            var ocupados = horariosOcupados
                .Select(h => h.ToString("HH:mm"))
                .OrderBy(h => h)
                .ToList();

            return new HorariosDisponiveisResponse
            {
                Data = data,
                Aberto = true,
                DiaSemana = diaSemana,
                HorariosDisponiveis = horariosDisponiveis,
                HorariosOcupados = ocupados
            };
        }

        public HorariosDisponiveisServicoResponse ObterHorariosDisponiveisPorServico(int barbeiroId, DateTime data, int servicoId)
        {
            var diaSemana = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(data.DayOfWeek);
            var aberto = HelperGenerico.BarbeariaAberta(data);
            var servico = _contexto.Servico.Find(servicoId) ?? throw new Exception("Serviço não encontrado.");

            if (!aberto)
            {
                return new HorariosDisponiveisServicoResponse
                {
                    Data = data,
                    Aberto = false,
                    DiaSemana = diaSemana,
                    RequerDuasEtapas = servico.RequerDuasEtapas,
                    IntervaloMinimoHoras = servico.IntervaloMinimoHoras
                };
            }

            var existeExcecao = _contexto.Excecao
                .Any(e => !e.Excluido &&
                         e.Data.Date == data.Date &&
                         (e.BarbeiroId == null || e.BarbeiroId == barbeiroId));

            if (existeExcecao)
            {
                return new HorariosDisponiveisServicoResponse
                {
                    Data = data,
                    Aberto = false,
                    DiaSemana = diaSemana,
                    RequerDuasEtapas = servico.RequerDuasEtapas,
                    IntervaloMinimoHoras = servico.IntervaloMinimoHoras
                };
            }

            var todosHorarios = HelperGenerico.ObterHorariosPorData(data);
            var horariosOcupados = ObterHorariosOcupados(barbeiroId, data);
            var agora = DateTime.Now;

            var tempoTotal = servico.TempoEstimado;
            var ocupaMaisDeUmSlot = tempoTotal.Hour > 0 || tempoTotal.Minute > 40;

            List<string> horariosDisponiveis;

            if (servico.RequerDuasEtapas)
            {
                // Para serviços com 2 etapas (ex: Platinado com Corte):
                // Mostrar apenas horários onde exista pelo menos 1 horário disponível
                // com o intervalo mínimo de horas de diferença.
                // Cada etapa pode ocupar mais de 1 slot se o tempo estimado > 40min.
                var horariosDisponiveisTimeOnly = FiltrarHorariosDisponiveisTimeOnly(todosHorarios, horariosOcupados, data, agora);

                if (ocupaMaisDeUmSlot)
                {
                    // Filtrar para slots consecutivos disponíveis na etapa 1
                    horariosDisponiveisTimeOnly = FiltrarSlotsConsecutivosDisponiveis(horariosDisponiveisTimeOnly, todosHorarios);
                }

                // Filtrar etapa 1: deve existir pelo menos um horário de etapa 2 válido
                var horariosEtapa1Validos = new List<TimeOnly>();
                foreach (var h in horariosDisponiveisTimeOnly)
                {
                    var horariosEtapa2 = ObterHorariosEtapa2Disponiveis(h, todosHorarios, horariosOcupados, servico.IntervaloMinimoHoras, data, agora, ocupaMaisDeUmSlot);
                    if (horariosEtapa2.Count > 0)
                        horariosEtapa1Validos.Add(h);
                }

                horariosDisponiveis = horariosEtapa1Validos.Select(h => h.ToString("HH:mm")).ToList();
            }
            else if (ocupaMaisDeUmSlot)
            {
                // Para serviços de 1h20m: mostrar apenas horários onde 2 slots consecutivos estão disponíveis
                var horariosDisponiveisTimeOnly = FiltrarHorariosDisponiveisTimeOnly(todosHorarios, horariosOcupados, data, agora);
                var filtrados = FiltrarSlotsConsecutivosDisponiveis(horariosDisponiveisTimeOnly, todosHorarios);
                horariosDisponiveis = filtrados.Select(h => h.ToString("HH:mm")).ToList();
            }
            else
            {
                // Serviço normal de 40min
                horariosDisponiveis = FiltrarHorariosDisponiveis(todosHorarios, horariosOcupados, data, agora);
            }

            var ocupados = horariosOcupados
                .Select(h => h.ToString("HH:mm"))
                .OrderBy(h => h)
                .ToList();

            return new HorariosDisponiveisServicoResponse
            {
                Data = data,
                Aberto = true,
                DiaSemana = diaSemana,
                HorariosDisponiveis = horariosDisponiveis,
                HorariosOcupados = ocupados,
                RequerDuasEtapas = servico.RequerDuasEtapas,
                IntervaloMinimoHoras = servico.IntervaloMinimoHoras
            };
        }

        public HorariosDisponiveisServicoResponse ObterHorariosEtapa2(int barbeiroId, DateTime data, int servicoId, string horaEtapa1)
        {
            var diaSemana = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(data.DayOfWeek);
            var servico = _contexto.Servico.Find(servicoId) ?? throw new Exception("Serviço não encontrado.");

            if (!servico.RequerDuasEtapas)
                throw new Exception("Este serviço não requer duas etapas.");

            var todosHorarios = HelperGenerico.ObterHorariosPorData(data);
            var horariosOcupados = ObterHorariosOcupados(barbeiroId, data);
            var agora = DateTime.Now;

            var tempoTotal = servico.TempoEstimado;
            var ocupaMaisDeUmSlot = tempoTotal.Hour > 0 || tempoTotal.Minute > 40;

            var etapa1Time = TimeOnly.Parse(horaEtapa1);
            var horariosEtapa2 = ObterHorariosEtapa2Disponiveis(etapa1Time, todosHorarios, horariosOcupados, servico.IntervaloMinimoHoras, data, agora, ocupaMaisDeUmSlot);

            return new HorariosDisponiveisServicoResponse
            {
                Data = data,
                Aberto = true,
                DiaSemana = diaSemana,
                HorariosDisponiveis = new List<string>(),
                HorariosOcupados = new List<string>(),
                HorariosDisponiveisEtapa2 = horariosEtapa2.Select(h => h.ToString("HH:mm")).ToList(),
                RequerDuasEtapas = servico.RequerDuasEtapas,
                IntervaloMinimoHoras = servico.IntervaloMinimoHoras
            };
        }

        public List<string> ObterTodosHorariosPorData(DateTime data)
        {
            return HelperGenerico.ObterHorariosPorData(data)
                .Select(h => h.ToString("HH:mm"))
                .ToList();
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private HashSet<TimeOnly> ObterHorariosOcupados(int barbeiroId, DateTime data)
        {
            var agendamentos = _contexto.Agendamento
                .Where(a => a.BarbeiroId == barbeiroId &&
                           a.DataHora.Date == data.Date &&
                           a.Status != AgendamentoStatus.CanceladoPeloCliente.ToString() &&
                           a.Status != AgendamentoStatus.CanceladoPeloBarbeiro.ToString())
                .ToList();

            return agendamentos
                .Select(a => TimeOnly.FromDateTime(a.DataHora))
                .ToHashSet();
        }

        private static List<string> FiltrarHorariosDisponiveis(List<TimeOnly> todosHorarios, HashSet<TimeOnly> horariosOcupados, DateTime data, DateTime agora)
        {
            return todosHorarios
                .Where(h =>
                {
                    if (data.Date == agora.Date)
                    {
                        var horaAtual = TimeOnly.FromDateTime(agora);
                        if (h <= horaAtual) return false;
                    }
                    return !horariosOcupados.Contains(h);
                })
                .Select(h => h.ToString("HH:mm"))
                .ToList();
        }

        private static List<TimeOnly> FiltrarHorariosDisponiveisTimeOnly(List<TimeOnly> todosHorarios, HashSet<TimeOnly> horariosOcupados, DateTime data, DateTime agora)
        {
            return todosHorarios
                .Where(h =>
                {
                    if (data.Date == agora.Date)
                    {
                        var horaAtual = TimeOnly.FromDateTime(agora);
                        if (h <= horaAtual) return false;
                    }
                    return !horariosOcupados.Contains(h);
                })
                .ToList();
        }

        /// <summary>
        /// Filtra horários que possuem o próximo slot consecutivo também disponível.
        /// Usado para serviços de 1h20m que ocupam 2 slots.
        /// </summary>
        private static List<TimeOnly> FiltrarSlotsConsecutivosDisponiveis(List<TimeOnly> horariosDisponiveis, List<TimeOnly> todosHorarios)
        {
            var disponiveisSet = horariosDisponiveis.ToHashSet();
            var resultado = new List<TimeOnly>();

            foreach (var h in horariosDisponiveis)
            {
                var idx = todosHorarios.IndexOf(h);
                if (idx >= 0 && idx < todosHorarios.Count - 1)
                {
                    var proximoSlot = todosHorarios[idx + 1];
                    if (disponiveisSet.Contains(proximoSlot))
                    {
                        resultado.Add(h);
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Retorna os horários disponíveis para a etapa 2, dado o horário da etapa 1 e o intervalo mínimo.
        /// </summary>
        private static List<TimeOnly> ObterHorariosEtapa2Disponiveis(
            TimeOnly etapa1Time,
            List<TimeOnly> todosHorarios,
            HashSet<TimeOnly> horariosOcupados,
            int intervaloMinimoHoras,
            DateTime data,
            DateTime agora,
            bool ocupaMaisDeUmSlot)
        {
            var minimoEtapa2 = etapa1Time.AddHours(intervaloMinimoHoras);

            // Se o horário mínimo da etapa 2 ultrapassar meia-noite (overflow do TimeOnly),
            // não há horário válido no mesmo dia para a etapa 2.
            if (minimoEtapa2 <= etapa1Time)
                return new List<TimeOnly>();

            var disponiveisEtapa2 = todosHorarios
                .Where(h =>
                {
                    if (h < minimoEtapa2) return false;
                    if (data.Date == agora.Date)
                    {
                        var horaAtual = TimeOnly.FromDateTime(agora);
                        if (h <= horaAtual) return false;
                    }
                    return !horariosOcupados.Contains(h);
                })
                .ToList();

            if (ocupaMaisDeUmSlot)
            {
                // Para serviços que ocupam mais de um slot, a etapa 2 também precisa de slots consecutivos
                disponiveisEtapa2 = FiltrarSlotsConsecutivosDisponiveis(disponiveisEtapa2, todosHorarios);
            }

            return disponiveisEtapa2;
        }
    }
}
