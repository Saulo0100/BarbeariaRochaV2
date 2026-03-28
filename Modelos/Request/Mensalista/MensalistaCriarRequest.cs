using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Mensalista
{
    public class MensalistaCriarRequest
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public decimal Valor { get; set; }
        public DayOfWeek Dia { get; set; }
        public MensalistaTipo Tipo { get; set; }
        public string? Horario { get; set; }
        public int? BarbeiroId { get; set; }
        public int? ServicoId { get; set; }
    }
}
