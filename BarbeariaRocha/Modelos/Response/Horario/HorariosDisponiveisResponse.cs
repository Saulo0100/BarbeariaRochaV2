namespace BarbeariaRocha.Modelos.Response.Horario
{
    public class HorariosDisponiveisResponse
    {
        public DateTime Data { get; set; }
        public bool Aberto { get; set; }
        public required string DiaSemana { get; set; }
        public List<string> HorariosDisponiveis { get; set; } = new();
        public List<string> HorariosOcupados { get; set; } = new();
    }
}
