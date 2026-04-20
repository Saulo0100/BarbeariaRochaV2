using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Entidades
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public int ProdutoId { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public required string Motivo { get; set; }
        public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
        public int? AgendamentoId { get; set; }
    }
}
