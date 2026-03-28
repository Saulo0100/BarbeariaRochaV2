namespace BarbeariaRocha.Modelos.Response.Tenant;

public class TenantDetalhesResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Plano { get; set; } = string.Empty;
    public int MaxBarbeiros { get; set; }
    public List<TenantDominioResponse> Dominios { get; set; } = [];
}

public class TenantDominioResponse
{
    public string Dominio { get; set; } = string.Empty;
    public bool Autorizado { get; set; }
}
