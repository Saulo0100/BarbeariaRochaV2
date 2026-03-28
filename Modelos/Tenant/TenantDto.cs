namespace BarbeariaRocha.Modelos.Tenant;

public class TenantDto
{
    public string TenantId { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public bool Autorizado { get; set; }
}
