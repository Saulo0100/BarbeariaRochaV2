using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Usuario;
using BarbeariaRocha.Modelos.Response.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController(IUsuarioApp app) : BaseController
    {
        public readonly IUsuarioApp _app = app;

        // POST: api/usuarios
        [Authorize]
        [HttpPost]
        public ActionResult Criar([FromBody] UsuarioCriarRequest request)
        {
            _app.Criar(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT: api/usuarios/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] UsuarioEditarRequest request)
        {
            _app.Editar(id, request);
            return NoContent();
        }

        // DELETE: api/usuarios/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            _app.Excluir(id);
            return NoContent();
        }

        // GET: api/usuarios/{id}
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<UsuarioDetalhesResponse> ObterPorId(int id)
        {
            var usuario = _app.ObterPorId(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // GET: api/usuarios
        [HttpGet]
        public ActionResult<PaginacaoResultado<UsuarioListarResponse>> Listar([FromQuery] PaginacaoFiltro<UsuarioFiltroRequest> filtro)
        {
            var usuarios = _app.ObterTodos(filtro);
            return Ok(usuarios);
        }

        // GET: api/usuarios/me
        [Authorize]
        [HttpGet("me")]
        public ActionResult<UsuarioDetalhesResponse> ObterUsuarioLogado()
        {
            var usuario = _app.ObterPorId(UserId());
            return Ok(usuario);
        }
    }
}