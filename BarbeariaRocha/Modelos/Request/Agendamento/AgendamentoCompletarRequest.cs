using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoCompletarRequest
    {
        public int Id { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
    }
}
