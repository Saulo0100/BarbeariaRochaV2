namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoCriarParaClienteRequest
    {
        public int BarbeiroId { get; set; }
        public int ServicoId { get; set; }
        public DateTime DtAgendamento { get; set; }
        public required string Numero { get; set; }
        public required string Nome { get; set; }

        /// <summary>
        /// Data/hora da segunda etapa (apenas para serviços com RequerDuasEtapas = true).
        /// </summary>
        public DateTime? DtAgendamentoEtapa2 { get; set; }

        /// <summary>
        /// Lista de adicionais selecionados pelo cliente.
        /// </summary>
        public List<AdicionalRequest>? Adicionais { get; set; }
    }
}
