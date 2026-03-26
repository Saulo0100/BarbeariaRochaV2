namespace BarbeariaRocha.Modelos.Entidades
{
    public class MensalistaCorte
    {
        public int Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public int MensalistaId { get; set; }
        public DateTime DataCorte { get; set; }
        public string? Observacao { get; set; }
    }
}
