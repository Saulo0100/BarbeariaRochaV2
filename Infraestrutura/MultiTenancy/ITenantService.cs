namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public interface ITenantService
{
    /// <summary>Retorna as informações do tenant da requisição atual.</summary>
    TenantInfo ObterTenantAtual();

    /// <summary>Retorna o TenantId da requisição atual.</summary>
    string ObterTenantId();
}
