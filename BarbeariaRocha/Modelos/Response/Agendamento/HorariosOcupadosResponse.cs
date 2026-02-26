namespace BarbeariaRocha.Modelos.Response.Agendamento
{
    public class HorariosOcupadosResponse
    {
        public DateTime Data { get; set; }
        public IEnumerable<TimeSpan>? Horarios { get; set; }
    }
}
