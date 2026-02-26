namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoCriarRequest
    {
        public int BarbeiroId { get; set; }
        public int UsuarioId { get; set; }
        public int ServicoId { get; set; }
        public DateTime DtAgendamento { get; set; }
        public required string Numero { get; set; }
        public required string Nome { get; set; }
        public required int CodigoConfirmacao { get; set; }
    }
}
