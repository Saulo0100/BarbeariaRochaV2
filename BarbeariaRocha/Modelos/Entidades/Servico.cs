namespace BarbeariaRocha.Modelos.Entidades
{
    public class Servico
    {
        public int Id { get; set; }
        public required string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TimeOnly TempoEstimado { get; set; }
        public bool Excluido { get; set; } = false;
        public required string Categoria { get; set; }
    }
}
