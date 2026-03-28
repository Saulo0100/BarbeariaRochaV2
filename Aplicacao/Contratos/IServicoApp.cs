using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Servico;
using BarbeariaRocha.Modelos.Response.Servico;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IServicoApp
    {
        // Listar serviços com paginação e filtro
        PaginacaoResultado<ServicoDetalhesResponse> ListarServicos(PaginacaoFiltro<ServicoFiltroRequest> filtro);

        // Criar novo serviço
        void CriarServico(ServicoCriarRequest request);

        // Deletar serviço por Id
        void DeletarServico(int id);
    }
}
