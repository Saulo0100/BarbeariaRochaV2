namespace BarbeariaRocha.Modelos.Response.Horario
{
    public class ConfiguracaoHorarioResponse
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public string NomeDia { get; set; } = string.Empty;
        public bool Aberto { get; set; }
        public string? HoraInicio { get; set; }
        public string? AlmocoInicio { get; set; }
        public string? AlmocoFim { get; set; }
        public string? HoraFim { get; set; }
        public int IntervaloMinutos { get; set; }
    }
}
