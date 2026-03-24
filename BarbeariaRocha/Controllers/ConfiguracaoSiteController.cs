using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/configuracao-site")]
public class ConfiguracaoSiteController(IConfiguracaoSiteApp app) : BaseController
{
    private readonly IConfiguracaoSiteApp _app = app;

    [HttpGet]
    public IActionResult ObterConfiguracao([FromQuery] string dominio)
    {
        var resultado = _app.ObterConfiguracao(dominio);
        return Ok(resultado);
    }

    [HttpGet("verificar-dominio")]
    public IActionResult VerificarDominio([FromQuery] string dominio)
    {
        var autorizado = _app.VerificarDominio(dominio);
        return Ok(new { autorizado });
    }
}
