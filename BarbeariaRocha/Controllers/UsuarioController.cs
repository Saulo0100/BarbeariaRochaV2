using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Usuario;
using BarbeariaRocha.Modelos.Response.Barbeiro;
using BarbeariaRocha.Modelos.Response.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController(IUsuarioApp app) : BaseController
    {
        public readonly IUsuarioApp _app = app;

        // POST: api/Usuarios
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Criar([FromBody] UsuarioCriarRequest request)
        {
            _app.Criar(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        // GET: api/Usuarios/ConfirmarEmail?token=xxx
        [AllowAnonymous]
        [HttpGet("ConfirmarEmail")]
        public ActionResult ConfirmarEmail([FromQuery] string token)
        {
            _app.ConfirmarEmail(token);
            return Ok("Email confirmado com sucesso!");
        }

        // PUT: api/Usuarios/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] UsuarioEditarRequest request)
        {
            _app.Editar(id, request);
            return NoContent();
        }

        // DELETE: api/Usuarios/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            _app.Excluir(id);
            return NoContent();
        }

        // GET: api/Usuarios/{id}
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<UsuarioDetalhesResponse> ObterPorId(int id)
        {
            var usuario = _app.ObterPorId(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // GET: api/Usuarios
        [HttpGet]
        public ActionResult<PaginacaoResultado<UsuarioListarResponse>> Listar([FromQuery] PaginacaoFiltro<UsuarioFiltroRequest> filtro)
        {
            var Usuarios = _app.ObterTodos(filtro);
            return Ok(Usuarios);
        }

        // GET: api/Usuarios/me
        [Authorize]
        [HttpGet("me")]
        public ActionResult<UsuarioDetalhesResponse> ObterUsuarioLogado()
        {
            var usuario = _app.ObterPorId(UserId());
            return Ok(usuario);
        }

        // GET: api/Usuarios/Barbeiro/Listar
        [AllowAnonymous]
        [HttpGet("/Barbeiro/Listar")]
        public ActionResult<BarbeirosDetalhesResponse> ObterBarbeiros()
        {
            var usuario = _app.ObterBarbeiros();
            return Ok(usuario);
        }
    }
}
