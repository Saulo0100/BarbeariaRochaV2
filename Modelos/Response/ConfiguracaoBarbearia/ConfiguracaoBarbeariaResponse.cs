namespace BarbeariaRocha.Modelos.Response.ConfiguracaoBarbearia
{
    public class ConfiguracaoBarbeariaResponse
    {
        public int Id { get; set; }
        public string NumeroCelular { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
    }
}
