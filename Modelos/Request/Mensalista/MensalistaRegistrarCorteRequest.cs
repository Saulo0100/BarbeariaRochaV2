namespace BarbeariaRocha.Modelos.Request.Mensalista
{
    public class MensalistaRegistrarCorteRequest
    {
        public int MensalistaId { get; set; }
        public DateTime DataCorte { get; set; }
        public string? Observacao { get; set; }
    }
}
