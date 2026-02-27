using BarbeariaRocha.Aplicacao.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestesController(ITestesApp app) : BaseController
    {
        private readonly ITestesApp _app = app;
        [HttpPost("RecriarBanco")]
        public IActionResult RecriarBanco()
        {
            _app.RecriarBanco();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
