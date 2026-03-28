using BarbeariaRocha.Modelos.Request.Horario;
using BarbeariaRocha.Modelos.Response.Horario;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IConfiguracaoHorarioApp
    {
        List<ConfiguracaoHorarioResponse> ListarTodos();
        ConfiguracaoHorarioResponse ObterPorDiaSemana(int diaSemana);
        void Salvar(ConfiguracaoHorarioSalvarRequest request);
        void SalvarTodos(List<ConfiguracaoHorarioSalvarRequest> requests);
    }
}
