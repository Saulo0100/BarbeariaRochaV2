using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Produto;
using BarbeariaRocha.Modelos.Response.Produto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProdutoController(IProdutoApp app) : BaseController
    {
        private readonly IProdutoApp _app = app;

        [AllowAnonymous]
        [HttpGet("loja")]
        public ActionResult<List<ProdutoDetalhesResponse>> ListarPublico()
        {
            return Ok(_app.ListarPublico());
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<ProdutoDetalhesResponse>> Listar()
        {
            var perfil = PerfilUsuario();
            if (perfil == "Cliente")
                return Forbid();
            return Ok(_app.Listar());
        }

        [Authorize]
        [HttpGet("estoque-baixo/count")]
        public ActionResult<int> ContarEstoqueBaixo()
        {
            var perfil = PerfilUsuario();
            if (perfil == "Cliente")
                return Forbid();
            return Ok(_app.ContarEstoqueBaixo());
        }

        [Authorize]
        [HttpPost]
        public ActionResult<ProdutoDetalhesResponse> Criar([FromBody] ProdutoCriarRequest request)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador" && perfil != "Administrador")
                return Forbid();

            var resultado = _app.Criar(request);
            return StatusCode(StatusCodes.Status201Created, resultado);
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Editar(int id, [FromBody] ProdutoEditarRequest request)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador" && perfil != "Administrador")
                return Forbid();

            _app.Editar(id, request);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador" && perfil != "Administrador")
                return Forbid();

            _app.Excluir(id);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/movimentacao")]
        public IActionResult RegistrarMovimentacao(int id, [FromBody] MovimentacaoCriarRequest request)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador" && perfil != "Administrador")
                return Forbid();

            _app.RegistrarMovimentacao(id, request);
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize]
        [HttpGet("{id}/historico")]
        public ActionResult<List<MovimentacaoEstoqueResponse>> ObterHistorico(int id)
        {
            var perfil = PerfilUsuario();
            if (perfil == "Cliente")
                return Forbid();

            return Ok(_app.ObterHistorico(id));
        }
    }
}
