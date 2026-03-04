namespace BarbeariaRocha.Modelos.Entidades
{
    public class AgendamentoAdicional
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}
