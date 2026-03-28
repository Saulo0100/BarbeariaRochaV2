namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class ServicoMaisPedidoResponse
    {
        public int ServicoId { get; set; }
        public required string NomeServico { get; set; }
        public string? Categoria { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal PercentualTotal { get; set; }
    }
}
