namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoCancelarPorNumeroRequest
    {
        public int AgendamentoId { get; set; }
        public required string Numero { get; set; }
        public int CodigoConfirmacao { get; set; }
    }
}
