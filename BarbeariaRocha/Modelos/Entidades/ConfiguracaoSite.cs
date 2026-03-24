namespace BarbeariaRocha.Modelos.Entidades
{
    public class ConfiguracaoSite
    {
        public int Id { get; set; }
        public int DominioId { get; set; }
        public DominioAutorizado? Dominio { get; set; }
        public required string NomeSite { get; set; }
        public byte[]? Logo { get; set; }
        public required string CorPrimaria { get; set; }
        public required string CorSecundaria { get; set; }
        public required string CorFundo { get; set; }
        public required string CorTexto { get; set; }
        public string? CorAcento { get; set; }
    }
}
