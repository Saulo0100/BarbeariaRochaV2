namespace BarbeariaRocha.Modelos.Request.Agendamento
{
    public class AgendamentoFiltroRequest
    {
        public int? BarbeiroId { get; set; }
        public int? UsuarioId { get; set; }
        public DateTime? DtAgendamento { get; set; }
        public IEnumerable<int>? Servicos { get; set; }
    }
}
