namespace BarbeariaRocha.Infraestrutura.MultiTenancy;

public class TenantOptions
{
    public const string SectionName = "Tenants";

    /// <summary>
    /// Dicionário de domínio → connection string.
    /// Chave: domínio sem protocolo (ex: "cliente1.com").
    /// Valor: connection string do banco de dados do tenant.
    /// </summary>
    public Dictionary<string, string> Dominios { get; set; } = [];
}
