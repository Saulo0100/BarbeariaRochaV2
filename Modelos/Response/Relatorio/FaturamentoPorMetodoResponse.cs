namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class FaturamentoPorMetodoResponse
    {
        public required string MetodoPagamento { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Percentual { get; set; }
    }
}
