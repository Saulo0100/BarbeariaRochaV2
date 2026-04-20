namespace BarbeariaRocha.Modelos.Entidades
{
    public class AgendamentoProduto
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public int AgendamentoId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal PrecoProduto { get; set; }
    }
}
