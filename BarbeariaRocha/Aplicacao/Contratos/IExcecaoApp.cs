using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Excecao;
using BarbeariaRocha.Modelos.Response.Excecao;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IExcecaoApp
    {
        void CriarExcecao(ExcecaoCriarRequest request);
        void DeletarExcecao(int id);
        ExcecaoDetalhesResponse ObterPorId(int id);
        PaginacaoResultado<ExcecaoDetalhesResponse> ListarExcecoes(PaginacaoFiltro<ExcecaoFiltroRequest> filtro);
        bool VerificarDisponibilidade(DateTime data, int? barbeiroId);
    }
}
