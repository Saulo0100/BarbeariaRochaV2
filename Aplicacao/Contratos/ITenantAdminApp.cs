using BarbeariaRocha.Modelos.Request.Tenant;
using BarbeariaRocha.Modelos.Response.Tenant;

namespace BarbeariaRocha.Aplicacao.Contratos;

public interface ITenantAdminApp
{
    Task<TenantDetalhesResponse> Criar(TenantCriarRequest request);
    Task Deletar(string dominio);
    Task EditarPlano(Guid tenantId, TenantEditarPlanoRequest request);
}
