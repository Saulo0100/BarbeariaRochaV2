using BarbeariaRocha.Modelos.Response.Horario;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IHorarioApp
    {
        HorariosDisponiveisResponse ObterHorariosDisponiveis(int barbeiroId, DateTime data);
        List<string> ObterTodosHorariosPorData(DateTime data);
    }
}
