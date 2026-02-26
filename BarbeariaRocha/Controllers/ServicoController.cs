using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Servico;
using BarbeariaRocha.Modelos.Response.Servico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/servicos")]
    public class ServicoController(IServicoApp app) : BaseController
    {
        private readonly IServicoApp _app = app;

        // GET: api/servicos
        [HttpGet]
        public ActionResult<PaginacaoResultado<ServicoDetalhesResponse>> Listar([FromQuery] PaginacaoFiltro<ServicoFiltroRequest> filtro)
        {
            var resultado = _app.ListarServicos(filtro);
            return Ok(resultado);
        }

        // POST: api/servicos
        [Authorize]
        [HttpPost]
        public IActionResult Criar([FromBody] ServicoCriarRequest request)
        {
            _app.CriarServico(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        // DELETE: api/servicos/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            _app.DeletarServico(id);
            return NoContent();
        }
    }
}