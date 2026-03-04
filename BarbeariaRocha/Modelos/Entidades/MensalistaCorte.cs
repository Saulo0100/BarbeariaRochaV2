namespace BarbeariaRocha.Modelos.Entidades
{
    public class MensalistaCorte
    {
        public int Id { get; set; }
        public int MensalistaId { get; set; }
        public DateTime DataCorte { get; set; }
        public string? Observacao { get; set; }
    }
}
