namespace BarbeariaRocha.Modelos.Request.Excecao
{
    public class ExcecaoCriarRequest
    {
        public required DateTime Data { get; set; }
        public required string Descricao { get; set; }
        public int? BarbeiroId { get; set; }
    }
}
