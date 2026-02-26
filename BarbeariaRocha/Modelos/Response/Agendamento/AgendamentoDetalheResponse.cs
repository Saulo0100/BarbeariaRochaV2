namespace BarbeariaRocha.Modelos.Response.Agendamento
{
    public class AgendamentoDetalheResponse
    {
        public int Id { get; set; }
        public required string NomeCliente { get; set; }
        public required string NomeBarbeiro { get; set; }
        public required string NumeroCliente { get; set; }
        public required string Status { get; set; }
        public DateTime Data { get; set; }
        public required string Servico { get; set; }
    }
}
