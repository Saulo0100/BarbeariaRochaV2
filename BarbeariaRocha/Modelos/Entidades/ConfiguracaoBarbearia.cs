namespace BarbeariaRocha.Modelos.Entidades
{
    public class ConfiguracaoBarbearia
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public required string NumeroCelular { get; set; }
        public required string Rua { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Cep { get; set; }
    }
}
