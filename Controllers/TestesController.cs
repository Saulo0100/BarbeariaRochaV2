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
            _app.LimparEPopular();
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("LimparBanco")]
        public IActionResult LimparBanco()
        {
            _app.LimparBanco();
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("PopularBanco")]
        public IActionResult PopularBanco()
        {
            _app.PopularDadosIniciais();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
