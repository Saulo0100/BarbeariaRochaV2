namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public interface ITenantService
{
    /// <summary>Retorna as informações do tenant da requisição atual.</summary>
    TenantInfo ObterTenantAtual();

    /// <summary>Retorna a connection string do tenant da requisição atual.</summary>
    string ObterConnectionString();
}
