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

    // POST: api/mensalista/corte
    [HttpPost("corte")]
    public IActionResult RegistrarCorte([FromBody] MensalistaRegistrarCorteRequest request)
    {
        _app.RegistrarCorte(request);
        return StatusCode(StatusCodes.Status201Created);
    }

    // GET: api/mensalista/{mensalistaId}/cortes
    [HttpGet("{mensalistaId}/cortes")]
    public ActionResult<IEnumerable<MensalistaCorteResponse>> ListarCortes(int mensalistaId, [FromQuery] int? mes = null, [FromQuery] int? ano = null)
    {
        var cortes = _app.ListarCortes(mensalistaId, mes, ano);
        return Ok(cortes);
    }

    // DELETE: api/mensalista/corte/{corteId}
    [HttpDelete("corte/{corteId}")]
    public IActionResult DeletarCorte(int corteId)
    {
        _app.DeletarCorte(corteId);
        return Ok();
    }

    /// <summary>
    /// Gera/atualiza agendamentos para todos os mensalistas ativos.
    /// Deve ser chamado ao abrir o app para manter os horários bloqueados.
    /// </summary>
    [HttpPost("gerar-agendamentos")]
    public IActionResult GerarAgendamentos()
    {
        _app.GerarAgendamentosMensalistas();
        return Ok();
    }
}
