namespace BarbeariaRocha.Modelos.Response.Produto
{
    public class ProdutoDetalhesResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeMinima { get; set; }
        public bool EstoqueBaixo { get; set; }
    }
}
