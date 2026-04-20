namespace BarbeariaRocha.Modelos.Entidades
{
    public class Produto
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeMinima { get; set; }
        public bool Excluido { get; set; } = false;
    }
}
