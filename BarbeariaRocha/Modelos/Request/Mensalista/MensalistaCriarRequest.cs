namespace BarbeariaRocha.Modelos.Request.Mensalista
{
    public class MensalistaCriarRequest
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public decimal Valor { get; set; }
        public required string Dia { get; set; }
        public required string Tipo { get; set; }
        public required string Status { get; set; }
    }
}
