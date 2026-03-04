using BarbeariaRocha.Modelos.Request.Relatorio;
using BarbeariaRocha.Modelos.Response.Relatorio;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IRelatorioApp
    {
        RelatorioGeralResponse ObterRelatorioGeral(RelatorioFiltroRequest filtro);
        IEnumerable<ServicoMaisPedidoResponse> ObterServicosMaisPedidos(RelatorioFiltroRequest filtro, int top = 10);
        IEnumerable<ClienteFrequenteResponse> ObterClientesFrequentes(RelatorioFiltroRequest filtro, int top = 10);
        IEnumerable<FaturamentoPorPeriodoResponse> ObterFaturamentoDiario(RelatorioFiltroRequest filtro);
        IEnumerable<FaturamentoPorMetodoResponse> ObterFaturamentoPorMetodo(RelatorioFiltroRequest filtro);
        IEnumerable<RelatorioBarbeiroResponse> ObterRelatorioPorBarbeiro(RelatorioFiltroRequest filtro);
        RelatorioGeralResponse ObterRelatorioGeralBarbeiro(RelatorioFiltroRequest filtro, int barbeiroId);
    }
}
