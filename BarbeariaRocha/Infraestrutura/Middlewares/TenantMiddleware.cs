using BarbeariaRocha.Infraestrutura.MultiTenancy;
using Microsoft.Extensions.Options;

namespace BarbeariaRocha.Infraestrutura.Middlewares;

public class TenantMiddleware(RequestDelegate next, IOptions<TenantOptions> tenantOptions)
{
    // Paths que não exigem resolução de tenant (ferramentas internas)
    private static readonly string[] PathsExcluidas = ["/hangfire", "/swagger", "/favicon.ico"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (DeveIgnorar(context.Request.Path))
        {
            await next(context);
            return;
        }

        var dominio = context.Request.Host.Host;

        if (string.IsNullOrWhiteSpace(dominio))
        {
            await ResponderErro(context, StatusCodes.Status400BadRequest, "Domínio não informado na requisição.");
            return;
        }

        if (!tenantOptions.Value.Dominios.TryGetValue(dominio, out var tenantId))
        {
            await ResponderErro(context, StatusCodes.Status400BadRequest, $"Domínio '{dominio}' não está cadastrado.");
            return;
        }

        context.Items[TenantService.TenantKey] = new TenantInfo(dominio, tenantId);

        await next(context);
    }

    private static bool DeveIgnorar(PathString path) =>
        PathsExcluidas.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));

    private static async Task ResponderErro(HttpContext context, int statusCode, string mensagem)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { status = statusCode, message = mensagem });
    }
}
