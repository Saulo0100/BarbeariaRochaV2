namespace BarbeariaRocha.Modelos.Entidades
{
    public class Adicional
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public decimal Valor { get; set; }
        public bool Excluido { get; set; } = false;
    }
}
