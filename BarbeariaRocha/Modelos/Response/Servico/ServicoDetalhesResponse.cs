namespace BarbeariaRocha.Modelos.Response.Servico
{
    public class ServicoDetalhesResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public required string Categoria { get; set; }

    }
}
