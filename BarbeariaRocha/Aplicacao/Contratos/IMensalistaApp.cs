using BarbeariaRocha.Modelos.Request.Mensalista;
using BarbeariaRocha.Modelos.Response.Mensalista;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IMensalistaApp
    {
        void CadastrarMensalista(MensalistaCriarRequest request, int idUsuario);
        void CancelarMensalista(int idMensalista);
        IEnumerable<MensalistaResponse> ObterTodos();
    }
}
