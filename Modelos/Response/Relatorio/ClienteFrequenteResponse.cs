namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class ClienteFrequenteResponse
    {
        public required string NomeCliente { get; set; }
        public required string NumeroCliente { get; set; }
        public int TotalAtendimentos { get; set; }
        public DateTime? UltimoAtendimento { get; set; }
        public decimal TotalGasto { get; set; }
    }
}
