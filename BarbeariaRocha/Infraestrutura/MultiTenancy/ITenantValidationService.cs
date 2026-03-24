using BarbeariaRocha.Modelos.Tenant;

namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public interface ITenantValidationService
{
    Task<TenantDto?> GetByDomain(string domain);
}
