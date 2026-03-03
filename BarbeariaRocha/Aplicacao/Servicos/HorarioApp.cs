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

            // Obter horarios ocupados
            var agendamentos = _contexto.Agendamento
                .Where(a => a.BarbeiroId == barbeiroId &&
                           a.DataHora.Date == data.Date &&
                           a.Status != AgendamentoStatus.CanceladoPeloCliente.ToString() &&
                           a.Status != AgendamentoStatus.CanceladoPeloBarbeiro.ToString())
                .ToList();

            var horariosOcupados = agendamentos
                .Select(a => TimeOnly.FromDateTime(a.DataHora))
                .ToHashSet();

            var agora = DateTime.Now;
            var horariosDisponiveis = todosHorarios
                .Where(h =>
                {
                    // Nao mostrar horarios passados para hoje
                    if (data.Date == agora.Date)
                    {
                        var horaAtual = TimeOnly.FromDateTime(agora);
                        if (h <= horaAtual) return false;
                    }
                    return !horariosOcupados.Contains(h);
                })
                .Select(h => h.ToString("HH:mm"))
                .ToList();

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

        public List<string> ObterTodosHorariosPorData(DateTime data)
        {
            return HelperGenerico.ObterHorariosPorData(data)
                .Select(h => h.ToString("HH:mm"))
                .ToList();
        }
    }
}
