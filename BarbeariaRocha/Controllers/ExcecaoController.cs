using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Excecao;
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

        [HttpPost("listar")]
        public IActionResult ListarExcecoes([FromBody] PaginacaoFiltro<ExcecaoFiltroRequest> filtro)
        {
            var resultado = _excecaoApp.ListarExcecoes(filtro);
            return Ok(resultado);
        }

        [HttpGet("verificar-disponibilidade")]
        public IActionResult VerificarDisponibilidade([FromQuery] DateTime data, [FromQuery] int? barbeiroId)
        {
            var disponivel = _excecaoApp.VerificarDisponibilidade(data, barbeiroId);
            return Ok(new { disponivel, data, barbeiroId });
        }
    }
}
