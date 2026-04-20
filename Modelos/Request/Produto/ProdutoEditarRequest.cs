namespace BarbeariaRocha.Modelos.Request.Produto
{
    public class ProdutoEditarRequest
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeMinima { get; set; }
    }
}
