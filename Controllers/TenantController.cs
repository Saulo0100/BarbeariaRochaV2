using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Request.Tenant;
using BarbeariaRocha.Modelos.Response.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaRocha.Controllers;

[ApiController]
[Route("api/v1/[controller]")]

public class TenantController(ITenantAdminApp app) : BaseController
{
    private readonly ITenantAdminApp _app = app;

    // POST: api/tenant
    [HttpPost]
    public async Task<ActionResult<TenantDetalhesResponse>> Criar([FromBody] TenantCriarRequest request)
    {
        var resultado = await _app.Criar(request);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }

    // DELETE: api/tenant/{dominio}
    [HttpDelete("{dominio}")]
    public async Task<IActionResult> Deletar(string dominio)
    {
        await _app.Deletar(dominio);
        return NoContent();
    }

    // PATCH: api/tenant/{tenantId}/plano
    [HttpPatch("{tenantId:guid}/plano")]
    public async Task<IActionResult> EditarPlano(Guid tenantId, [FromBody] TenantEditarPlanoRequest request)
    {
        await _app.EditarPlano(tenantId, request);
        return NoContent();
    }
}
