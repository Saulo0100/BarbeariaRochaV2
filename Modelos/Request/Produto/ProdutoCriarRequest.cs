namespace BarbeariaRocha.Modelos.Request.Produto
{
    public class ProdutoCriarRequest
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeInicial { get; set; }
        public int QuantidadeMinima { get; set; }
    }
}
