using BarbeariaRocha.Modelos.Response.Horario;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IHorarioApp
    {
        HorariosDisponiveisResponse ObterHorariosDisponiveis(int barbeiroId, DateTime data);
        HorariosDisponiveisServicoResponse ObterHorariosDisponiveisPorServico(int barbeiroId, DateTime data, int servicoId);
        HorariosDisponiveisServicoResponse ObterHorariosEtapa2(int barbeiroId, DateTime data, int servicoId, string horaEtapa1);
        List<string> ObterTodosHorariosPorData(DateTime data);
    }
}
