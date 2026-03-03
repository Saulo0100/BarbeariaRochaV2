namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class FaturamentoPorPeriodoResponse
    {
        public required string Periodo { get; set; }
        public int TotalAtendimentos { get; set; }
        public decimal Faturamento { get; set; }
    }
}
