namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class ClienteFrequenteResponse
    {
        public required string NomeCliente { get; set; }
        public required string NumeroCliente { get; set; }
        public int TotalCortes { get; set; }
        public DateTime? UltimoCorte { get; set; }
        public decimal TotalGasto { get; set; }
    }
}
