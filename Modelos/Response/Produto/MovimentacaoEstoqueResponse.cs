namespace BarbeariaRocha.Modelos.Response.Produto
{
    public class MovimentacaoEstoqueResponse
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime DataMovimentacao { get; set; }
        public int? AgendamentoId { get; set; }
    }
}
