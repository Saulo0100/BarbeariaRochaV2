namespace BarbeariaRocha.Modelos.Entidades;

public class TenantDominio
{
    public int Id { get; set; }
    public required string Dominio { get; set; }
    public Guid TenantId { get; set; }
    public bool Autorizado { get; set; }

    public Tenant Tenant { get; set; } = null!;
}
