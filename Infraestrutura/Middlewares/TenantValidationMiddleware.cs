using BarbeariaRocha.Infraestrutura.MultiTenancy;

namespace BarbeariaRocha.Infraestrutura.Middlewares;

public class TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
{
    private static readonly string[] PathsExcluidas = ["/swagger", "/health", "/hangfire", "/favicon.ico"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (DeveIgnorar(context.Request.Path))
        {
            await next(context);
            return;
        }

        var dominio = context.Request.Headers.Origin;
        var tenantService = context.RequestServices.GetRequiredService<ITenantValidationService>();
        var tenant = await tenantService.GetByDomain(dominio);

        if (tenant is null)
        {
            logger.LogWarning("Domínio '{Domain}' não encontrado na API de tenant.", dominio);
            await ResponderForbidden(context);
            return;
        }

        if (!tenant.Autorizado)
        {
            logger.LogWarning("Domínio '{Domain}' está inativo.", dominio);
            await ResponderForbidden(context);
            return;
        }
        context.Items[TenantService.TenantKey] = new TenantInfo(dominio, dominio);

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
