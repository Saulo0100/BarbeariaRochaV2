using BarbeariaRocha.Infraestrutura.MultiTenancy;

namespace BarbeariaRocha.Infraestrutura.Middlewares;

public class TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
{
    private static readonly string[] PathsExcluidas = ["/swagger", "/health", "/favicon.ico"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (DeveIgnorar(context.Request.Path))
        {
            await next(context);
            return;
        }

        var dominio = context.Request.Headers.Origin;

        if (dominio == "https://localhost:44396")
        {
            await next(context);
            return;
        }

        logger.LogInformation("Validando tenant para o domínio: {Domain}", dominio);
        var tenantService = context.RequestServices.GetRequiredService<ITenantValidationService>();
        var tenant = await tenantService.GetByDomain(dominio);

        if (tenant is null)
        {
            logger.LogWarning("Domínio '{Domain}' não encontrado.", dominio);
            await ResponderForbidden(context);
            return;
        }

        if (!tenant.Autorizado)
        {
            logger.LogWarning("Domínio '{Domain}' está inativo.", dominio);
            await ResponderForbidden(context);
            return;
        }
        context.Items[TenantService.TenantKey] = new TenantInfo(dominio, tenant.TenantId);

        await next(context);
    }

    private static bool DeveIgnorar(PathString path) =>
        PathsExcluidas.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));

    private static async Task ResponderForbidden(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new
        {
            status = StatusCodes.Status403Forbidden,
            message = "Domínio inativo"
        });
    }
}
