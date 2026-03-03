namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class RelatorioGeralResponse
    {
        public int TotalCortes { get; set; }
        public int CortesHoje { get; set; }
        public int CortesSemana { get; set; }
        public int CortesMes { get; set; }
        public decimal FaturamentoTotal { get; set; }
        public decimal FaturamentoHoje { get; set; }
        public decimal FaturamentoSemana { get; set; }
        public decimal FaturamentoMes { get; set; }
        public decimal TicketMedio { get; set; }
        public int AgendamentosPendentes { get; set; }
        public int CancelamentosTotal { get; set; }
    }
}
