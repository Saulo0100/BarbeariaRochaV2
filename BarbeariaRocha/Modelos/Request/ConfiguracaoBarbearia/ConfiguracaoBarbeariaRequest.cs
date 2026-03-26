namespace BarbeariaRocha.Modelos.Request.ConfiguracaoBarbearia
{
    public class ConfiguracaoBarbeariaRequest
    {
        public required string NumeroCelular { get; set; }
        public required string Rua { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Cep { get; set; }
    }
}
