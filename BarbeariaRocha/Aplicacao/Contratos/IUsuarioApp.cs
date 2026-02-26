using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Usuario;
using BarbeariaRocha.Modelos.Response.Usuario;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IUsuarioApp
    {
        void Criar(UsuarioCriarRequest request);
        void Editar(int id, UsuarioEditarRequest request);
        void Excluir(int id);
        UsuarioDetalhesResponse ObterPorId(int id);
        PaginacaoResultado<UsuarioDetalhesResponse> ObterTodos(PaginacaoFiltro<UsuarioFiltroRequest> filtro);
    }
}
