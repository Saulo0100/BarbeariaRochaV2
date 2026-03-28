namespace BarbeariaRocha.Modelos.Request.Tenant;

public class TenantCriarRequest
{
    public required string Nome { get; set; }
    public required string Dominio { get; set; }
    public int PlanoId { get; set; }
}
