namespace BarbeariaRocha.Modelos.Entidades;

public class Plano
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public int MaxBarbeiros { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Tenant> Tenants { get; set; } = [];
}
