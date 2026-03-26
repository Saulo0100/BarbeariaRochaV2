namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public class TenantService(IHttpContextAccessor httpContextAccessor) : ITenantService
{
    // Chave usada para armazenar o tenant resolvido no HttpContext.Items
    public const string TenantKey = "TenantAtual";

    public TenantInfo ObterTenantAtual()
    {
        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext não disponível. Verifique se o serviço está sendo usado dentro de uma requisição HTTP.");

        return httpContext.Items[TenantKey] as TenantInfo
            ?? throw new InvalidOperationException("Tenant não foi resolvido para esta requisição. Certifique-se de que o TenantMiddleware está registrado no pipeline.");
    }

    public string ObterTenantId() => ObterTenantAtual().TenantId;
}
