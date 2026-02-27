using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Excecao;
using BarbeariaRocha.Modelos.Response.Excecao;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcecaoController(IExcecaoApp excecaoApp) : ControllerBase
    {
        private readonly IExcecaoApp _excecaoApp = excecaoApp;

        [HttpPost]
        public IActionResult CriarExcecao([FromBody] ExcecaoCriarRequest request)
        {
            _excecaoApp.CriarExcecao(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarExcecao(int id)
        {
            _excecaoApp.DeletarExcecao(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var excecao = _excecaoApp.ObterPorId(id);
            return Ok(excecao);
        }

        [HttpGet("listar")]
        public IActionResult ListarExcecoes([FromQuery] PaginacaoFiltro<ExcecaoFiltroRequest> filtro)
        {
            var resultado = _excecaoApp.ListarExcecoes(filtro);
            return Ok(resultado);
        }
        [HttpGet("Barbeiro/{id}")]
        public ActionResult<IEnumerable<ExcecaoDetalhesResponse>> ObterExcecaoBarbeiro(int id)
        {
            var excecao = _excecaoApp.ObterPorBarbeiro(id);
            if (excecao == null)
                return NotFound();
            return Ok(excecao);
        }
    }
}
