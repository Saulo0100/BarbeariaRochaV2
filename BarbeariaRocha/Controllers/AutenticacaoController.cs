using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Autenticacao;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController(IAutenticacaoApp app) : BaseController
    {
        public readonly IAutenticacaoApp _app = app;

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] LoginRequest login)
        {
            return _app.Login(login);
        }

        [HttpPost("EsqueceuSenha")]
        public ActionResult EsqueceuSenha([FromBody] EsqueceuSenhaRequest request)
        {
            _app.EsqueceuSenha(request);
            return Ok();
        }

        [HttpPatch("NovaSenha")]
        public ActionResult EditarSenhaBarbeiro(string novaSenha)
        {
            _app.AtualizarSenha(UserId(), novaSenha);
            return Ok();
        }
    }
}
