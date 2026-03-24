namespace BarbeariaRocha.Modelos.Entidades
{
    public class DominioAutorizado
    {
        public int Id { get; set; }
        public required string Dominio { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
