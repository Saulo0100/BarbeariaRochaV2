namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class RelatorioBarbeiroResponse
    {
        public int BarbeiroId { get; set; }
        public required string NomeBarbeiro { get; set; }
        public int TotalCortes { get; set; }
        public decimal Faturamento { get; set; }
        public decimal TicketMedio { get; set; }
        public int CancelamentosTotal { get; set; }
    }
}
