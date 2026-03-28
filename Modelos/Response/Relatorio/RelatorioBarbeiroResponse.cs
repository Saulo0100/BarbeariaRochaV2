namespace BarbeariaRocha.Modelos.Response.Relatorio
{
    public class RelatorioBarbeiroResponse
    {
        public int BarbeiroId { get; set; }
        public required string NomeBarbeiro { get; set; }
        public int TotalAtendimentos { get; set; }
        public decimal Faturamento { get; set; }
        public int CancelamentosTotal { get; set; }
        public int ClientesFaltaram { get; set; }
        public decimal TaxaConclusao { get; set; }

        /// <summary>
        /// Porcentagem que o BarbeiroAdmin recebe sobre o faturamento deste barbeiro.
        /// Null se não houver porcentagem definida.
        /// </summary>
        public decimal? PorcentagemAdmin { get; set; }

        /// <summary>
        /// Faturamento bruto (sem descontar porcentagem do admin).
        /// </summary>
        public decimal? FaturamentoBruto { get; set; }

        /// <summary>
        /// Faturamento líquido (após descontar porcentagem do admin).
        /// </summary>
        public decimal? FaturamentoLiquido { get; set; }

        /// <summary>
        /// Valor que o admin recebe deste barbeiro.
        /// </summary>
        public decimal? ValorComissaoAdmin { get; set; }
    }
}
