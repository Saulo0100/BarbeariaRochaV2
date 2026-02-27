using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Mensalista;
using BarbeariaRocha.Modelos.Response.Mensalista;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MensalistaController(IMensalistaApp app) : BaseController
{
    private readonly IMensalistaApp _app = app;

    [HttpPost]
    public IActionResult CadastrarMensalista([FromBody] MensalistaCriarRequest request)
    {
        _app.CadastrarMensalista(request, UserId());
        return Ok();
    }

    [HttpDelete]
    public IActionResult CancelarMensalista([FromBody] int idMensalista)
    {
        _app.CancelarMensalista(idMensalista);
        return Ok();
    }

    [HttpGet]
    public ActionResult<IEnumerable<MensalistaResponse>> ObterTodos()
    {
        var mensalistas = _app.ObterTodos();
        return Ok(mensalistas);
    }
}
