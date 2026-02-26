namespace BarbeariaRocha.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public int Excluido { get; set; }
        public required string Categoria { get; set; }
    }
}
