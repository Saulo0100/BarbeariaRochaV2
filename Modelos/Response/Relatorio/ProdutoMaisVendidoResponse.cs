namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class ProdutoMaisVendidoResponse
    {
        public int ProdutoId { get; set; }
        public required string NomeProduto { get; set; }
        public int QuantidadeTotal { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
