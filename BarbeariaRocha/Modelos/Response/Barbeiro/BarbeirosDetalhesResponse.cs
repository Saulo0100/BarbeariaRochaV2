namespace BarbeariaRocha.Modelos.Response.Barbeiro
{
    public class BarbeirosDetalhesResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public required string Agenda { get; set; }
    }
}
