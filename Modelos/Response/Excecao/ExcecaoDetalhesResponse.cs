namespace BarbeariaRocha.Modelos.Response.Excecao
{
    public class ExcecaoDetalhesResponse
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public required string Descricao { get; set; }
        public int? BarbeiroId { get; set; }
        public string? NomeBarbeiro { get; set; }
    }
}
