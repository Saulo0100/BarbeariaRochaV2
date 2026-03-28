using BarbeariaRocha.Modelos.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AppContexto = BarbeariaRocha.Infraestrutura.Contexto.Contexto;

namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public class TenantValidationService(
    AppContexto contexto,
    IMemoryCache cache,
    ILogger<TenantValidationService> logger) : ITenantValidationService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public async Task<TenantDto?> GetByDomain(string domain)
    {
        if (cache.TryGetValue(domain, out TenantDto? cached))
            return cached;

        var tenantDominio = await contexto.TenantDominio
            .AsNoTracking()
            .FirstOrDefaultAsync(td => td.Dominio == domain);

        if (tenantDominio is null)
        {
            logger.LogWarning("Domínio '{Domain}' não encontrado no banco de dados.", domain);
            return null;
        }

        var dto = new TenantDto
        {
            TenantId = tenantDominio.TenantId.ToString(),
            Domain = domain,
            Autorizado = tenantDominio.Autorizado
        };

        if (tenantDominio.Autorizado)
            cache.Set(domain, dto, CacheDuration);

        return dto;
    }
}
