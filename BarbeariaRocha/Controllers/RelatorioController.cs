using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Relatorio;
using BarbeariaRocha.Modelos.Response.Relatorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/relatorio")]
    [Authorize]
    public class RelatorioController(IRelatorioApp app) : BaseController
    {
        private readonly IRelatorioApp _app = app;

        /// <summary>
        /// Retorna o relatorio geral com totais de cortes, faturamento, etc.
        /// Pode filtrar por barbeiro e periodo.
        /// </summary>
        [HttpGet("geral")]
        public ActionResult<RelatorioGeralResponse> ObterRelatorioGeral([FromQuery] RelatorioFiltroRequest filtro)
        {
            var resultado = _app.ObterRelatorioGeral(filtro);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna os servicos mais pedidos, ordenados por quantidade.
        /// </summary>
        [HttpGet("servicos-mais-pedidos")]
        public ActionResult<IEnumerable<ServicoMaisPedidoResponse>> ObterServicosMaisPedidos(
            [FromQuery] RelatorioFiltroRequest filtro,
            [FromQuery] int top = 10)
        {
            var resultado = _app.ObterServicosMaisPedidos(filtro, top);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna os clientes mais frequentes, ordenados por total de cortes.
        /// </summary>
        [HttpGet("clientes-frequentes")]
        public ActionResult<IEnumerable<ClienteFrequenteResponse>> ObterClientesFrequentes(
            [FromQuery] RelatorioFiltroRequest filtro,
            [FromQuery] int top = 10)
        {
            var resultado = _app.ObterClientesFrequentes(filtro, top);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna o faturamento diario para o periodo especificado (padrao: ultimos 30 dias).
        /// </summary>
        [HttpGet("faturamento-diario")]
        public ActionResult<IEnumerable<FaturamentoPorPeriodoResponse>> ObterFaturamentoDiario(
            [FromQuery] RelatorioFiltroRequest filtro)
        {
            var resultado = _app.ObterFaturamentoDiario(filtro);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna o faturamento agrupado por metodo de pagamento.
        /// </summary>
        [HttpGet("faturamento-por-metodo")]
        public ActionResult<IEnumerable<FaturamentoPorMetodoResponse>> ObterFaturamentoPorMetodo(
            [FromQuery] RelatorioFiltroRequest filtro)
        {
            var resultado = _app.ObterFaturamentoPorMetodo(filtro);
            return Ok(resultado);
        }

        /// <summary>
        /// Retorna o relatorio comparativo entre barbeiros.
        /// </summary>
        [HttpGet("por-barbeiro")]
        public ActionResult<IEnumerable<RelatorioBarbeiroResponse>> ObterRelatorioPorBarbeiro(
            [FromQuery] RelatorioFiltroRequest filtro)
        {
            var resultado = _app.ObterRelatorioPorBarbeiro(filtro);
            return Ok(resultado);
        }
    }
}
