namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class FaturamentoPorPeriodoResponse
    {
        public required string Periodo { get; set; }
        public int TotalCortes { get; set; }
        public decimal Faturamento { get; set; }
    }
}
