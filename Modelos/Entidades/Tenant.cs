namespace BarbeariaRocha.Modelos.Entidades;

public class Tenant
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
    public int PlanoId { get; set; }

    public Plano Plano { get; set; } = null!;
    public ICollection<TenantDominio> Dominios { get; set; } = [];
}
