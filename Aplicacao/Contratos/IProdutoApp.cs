using BarbeariaRocha.Modelos.Request.Produto;
using BarbeariaRocha.Modelos.Response.Produto;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IProdutoApp
    {
        List<ProdutoDetalhesResponse> Listar();
        List<ProdutoDetalhesResponse> ListarPublico();
        ProdutoDetalhesResponse Criar(ProdutoCriarRequest request);
        void Editar(int id, ProdutoEditarRequest request);
        void Excluir(int id);
        void RegistrarMovimentacao(int produtoId, MovimentacaoCriarRequest request);
        List<MovimentacaoEstoqueResponse> ObterHistorico(int produtoId);
        int ContarEstoqueBaixo();
        void RegistrarVendas(int agendamentoId, List<ProdutoVendaRequest> produtos);
    }
}
