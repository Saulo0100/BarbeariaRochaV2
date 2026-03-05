using BarbeariaRocha.Aplicacao.Contratos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdicionalController(IAdicionalApp app) : BaseController
    {
        private readonly IAdicionalApp _app = app;

        // GET: api/Adicional
        // Lista todos os adicionais ativos (usado também pelo fluxo de agendamento)
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<object>> Listar()
        {
            var resultado = _app.ListarAdicionais();
            return Ok(resultado);
        }

        // POST: api/Adicional
        // Apenas BarbeiroAdministrador pode criar
        [Authorize]
        [HttpPost]
        public IActionResult Criar([FromBody] AdicionalCriarRequest request)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador")
                return Forbid();

            _app.CriarAdicional(request.Nome, request.Valor);
            return StatusCode(StatusCodes.Status201Created);
        }

        // DELETE: api/Adicional/{id}
        // Apenas BarbeiroAdministrador pode deletar (soft delete)
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var perfil = PerfilUsuario();
            if (perfil != "BarbeiroAdministrador")
                return Forbid();

            _app.DeletarAdicional(id);
            return NoContent();
        }
    }

    public class AdicionalCriarRequest
    {
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}
