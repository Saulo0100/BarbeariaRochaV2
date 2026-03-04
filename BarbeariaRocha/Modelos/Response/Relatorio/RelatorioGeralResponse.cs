namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class RelatorioGeralResponse
    {
        public int TotalAtendimentos { get; set; }
        public int AtendimentosHoje { get; set; }
        public int AtendimentosSemana { get; set; }
        public int AtendimentosMes { get; set; }
        public decimal FaturamentoTotal { get; set; }
        public decimal FaturamentoHoje { get; set; }
        public decimal FaturamentoSemana { get; set; }
        public decimal FaturamentoMes { get; set; }
        public int AgendamentosPendentes { get; set; }
        public int CancelamentosTotal { get; set; }
        public int ClientesFaltaram { get; set; }
        public decimal TaxaFaltas { get; set; }
        public decimal TaxaCancelamento { get; set; }
        public decimal TaxaConclusao { get; set; }
    }
}
