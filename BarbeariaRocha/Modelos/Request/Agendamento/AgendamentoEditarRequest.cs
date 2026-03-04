using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoEditarRequest
    {
        public required MetodoPagamento MetodoPagamento { get; set; }
        public int ServicoId { get; set; }

        /// <summary>
        /// Lista de adicionais selecionados pelo barbeiro ao editar.
        /// </summary>
        public List<AdicionalRequest>? Adicionais { get; set; }
    }
}
