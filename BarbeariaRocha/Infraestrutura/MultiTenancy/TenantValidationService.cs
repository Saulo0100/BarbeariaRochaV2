using BarbeariaRocha.Modelos.Tenant;
using Microsoft.Extensions.Caching.Memory;

namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public class TenantValidationService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<TenantValidationService> logger) : ITenantValidationService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private const string HttpClientName = "TenantValidation";

    public async Task<TenantDto?> GetByDomain(string domain)
    {
        if (cache.TryGetValue(domain, out TenantDto? cached))
            return cached;

        try
        {
            var client = httpClientFactory.CreateClient(HttpClientName);
            var response = await client.GetAsync($"/verificar-dominio?dominio={domain}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("API de tenant retornou {StatusCode} para o domínio '{Domain}'.", response.StatusCode, domain);
                return null;
            }

            var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();

            if (tenant is null || tenant.Autorizado == false)
                return null;

            tenant.Domain = domain;

            cache.Set(domain, tenant, CacheDuration);
            return tenant;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao consultar API de tenant para o domínio '{Domain}'.", domain);
            return null;
        }
    }
}
