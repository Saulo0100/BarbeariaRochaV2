using BarbeariaRocha.Aplicacao.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController(ITokenApp app) : BaseController
    {
        public readonly ITokenApp _app = app;

        [HttpPost]
        public ActionResult GerarToken([FromBody] string numero)
        {
            _app.GerarToken(numero);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
