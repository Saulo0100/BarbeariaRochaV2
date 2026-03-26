using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Aplicacao.Helper;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
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
            var config = ObterConfigDia(data);

            if (!config.Aberto)
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

            var todosHorarios = HelperGenerico.MontarHorariosPorConfig(config);
            todosHorarios = FiltrarPorPeriodoTrabalho(todosHorarios, barbeiroId, config);
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
            var config = ObterConfigDia(data);
            var servico = _contexto.Servico.Find(servicoId) ?? throw new Exception("Serviço não encontrado.");

            if (!config.Aberto)
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

            var todosHorarios = HelperGenerico.MontarHorariosPorConfig(config);
            todosHorarios = FiltrarPorPeriodoTrabalho(todosHorarios, barbeiroId, config);
            var horariosOcupados = ObterHorariosOcupados(barbeiroId, data);
            var agora = DateTime.Now;

            var tempoTotal = servico.TempoEstimado;
            var intervaloSlot = config.IntervaloMinutos > 0 ? config.IntervaloMinutos : 40;
            var tempoTotalMinutos = tempoTotal.Hour * 60 + tempoTotal.Minute;
            var ocupaMaisDeUmSlot = tempoTotalMinutos > intervaloSlot;

            List<string> horariosDisponiveis;

            if (servico.RequerDuasEtapas)
            {
                var horariosDisponiveisTimeOnly = FiltrarHorariosDisponiveisTimeOnly(todosHorarios, horariosOcupados, data, agora);

                if (ocupaMaisDeUmSlot)
                    horariosDisponiveisTimeOnly = FiltrarSlotsConsecutivosDisponiveis(horariosDisponiveisTimeOnly, todosHorarios);

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
                var horariosDisponiveisTimeOnly = FiltrarHorariosDisponiveisTimeOnly(todosHorarios, horariosOcupados, data, agora);
                var filtrados = FiltrarSlotsConsecutivosDisponiveis(horariosDisponiveisTimeOnly, todosHorarios);
                horariosDisponiveis = filtrados.Select(h => h.ToString("HH:mm")).ToList();
            }
            else
            {
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

            var config = ObterConfigDia(data);
            var todosHorarios = HelperGenerico.MontarHorariosPorConfig(config);
            todosHorarios = FiltrarPorPeriodoTrabalho(todosHorarios, barbeiroId, config);
            var horariosOcupados = ObterHorariosOcupados(barbeiroId, data);
            var agora = DateTime.Now;

            var intervaloSlot = config.IntervaloMinutos > 0 ? config.IntervaloMinutos : 40;
            var ocupaMaisDeUmSlot = (servico.TempoEstimado.Hour * 60 + servico.TempoEstimado.Minute) > intervaloSlot;

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
            var config = ObterConfigDia(data);
            return HelperGenerico.MontarHorariosPorConfig(config)
                .Select(h => h.ToString("HH:mm"))
                .ToList();
        }

        /// <summary>
        /// Retorna todos os horários válidos para um barbeiro em um dia da semana específico.
        /// Considera a configuração dinâmica do dia e o período de trabalho do barbeiro.
        /// Não filtra por horários ocupados nem por horários passados.
        /// Usado para cadastro de mensalistas.
        /// </summary>
        public List<string> ObterHorariosMensalista(int barbeiroId, int diaSemana)
        {
            var config = _contexto.ConfiguracaoHorario
                .FirstOrDefault(c => c.DiaSemana == diaSemana)
                ?? CriarConfigPadrao(diaSemana);

            var todosHorarios = HelperGenerico.MontarHorariosPorConfig(config);
            todosHorarios = FiltrarPorPeriodoTrabalho(todosHorarios, barbeiroId, config);

            return todosHorarios
                .Select(h => h.ToString("HH:mm"))
                .ToList();
        }

        // ==================== MÉTODOS AUXILIARES ====================

        /// <summary>
        /// Obtém a ConfiguracaoHorario do banco para o dia da semana da data informada.
        /// Se não existir, usa valores padrão para não quebrar o sistema.
        /// </summary>
        private ConfiguracaoHorario ObterConfigDia(DateTime data)
        {
            var diaSemana = (int)data.DayOfWeek;
            return _contexto.ConfiguracaoHorario
                .FirstOrDefault(c => c.DiaSemana == diaSemana)
                ?? CriarConfigPadrao(diaSemana);
        }

        /// <summary>
        /// Fallback com os valores originais hardcoded, caso a tabela ainda não tenha sido populada.
        /// </summary>
        private static ConfiguracaoHorario CriarConfigPadrao(int diaSemana) => diaSemana switch
        {
            0 => new ConfiguracaoHorario { DiaSemana = 0, Aberto = false, IntervaloMinutos = 40 },
            1 => new ConfiguracaoHorario { DiaSemana = 1, Aberto = true, HoraInicio = TimeOnly.Parse("13:20"), HoraFim = TimeOnly.Parse("20:00"), IntervaloMinutos = 40 },
            6 => new ConfiguracaoHorario { DiaSemana = 6, Aberto = true, HoraInicio = TimeOnly.Parse("09:00"), AlmocoInicio = TimeOnly.Parse("12:20"), AlmocoFim = TimeOnly.Parse("13:20"), HoraFim = TimeOnly.Parse("17:20"), IntervaloMinutos = 40 },
            _ => new ConfiguracaoHorario { DiaSemana = diaSemana, Aberto = true, HoraInicio = TimeOnly.Parse("10:00"), AlmocoInicio = TimeOnly.Parse("11:20"), AlmocoFim = TimeOnly.Parse("13:20"), HoraFim = TimeOnly.Parse("20:00"), IntervaloMinutos = 40 }
        };

        /// <summary>
        /// Filtra horários com base no período de trabalho do barbeiro (Manhã, Tarde, Dia Todo).
        /// O divisor manhã/tarde é o AlmocoFim do dia, ou HoraInicio se não houver almoço.
        /// </summary>
        private List<TimeOnly> FiltrarPorPeriodoTrabalho(List<TimeOnly> horarios, int barbeiroId, ConfiguracaoHorario config)
        {
            var barbeiro = _contexto.Usuario.Find(barbeiroId);
            if (barbeiro == null || string.IsNullOrEmpty(barbeiro.PeriodoTrabalho) || barbeiro.PeriodoTrabalho == "DiaTodo")
                return horarios;

            // Divisor: fim do almoço (retorno), ou início do expediente se não houver almoço
            var divisor = config.AlmocoFim ?? config.HoraInicio ?? new TimeOnly(13, 20);

            if (barbeiro.PeriodoTrabalho == "Manha")
                return horarios.Where(h => h < divisor).ToList();

            if (barbeiro.PeriodoTrabalho == "Tarde")
                return horarios.Where(h => h >= divisor).ToList();

            return horarios;
        }

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
        /// Usado para serviços que ocupam mais de um slot.
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
                        resultado.Add(h);
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

            // Se o horário mínimo da etapa 2 ultrapassar meia-noite, não há horário válido no mesmo dia.
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
                disponiveisEtapa2 = FiltrarSlotsConsecutivosDisponiveis(disponiveisEtapa2, todosHorarios);

            return disponiveisEtapa2;
        }
    }
}
