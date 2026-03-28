namespace BarbeariaRocha.Modelos.Entidades
{
    public class AgendamentoAdicional
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public int AgendamentoId { get; set; }
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}
