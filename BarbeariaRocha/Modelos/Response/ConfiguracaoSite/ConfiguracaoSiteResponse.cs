namespace BarbeariaRocha.Modelos.Response.ConfiguracaoSite
{
    public class ConfiguracaoSiteResponse
    {
        public string NomeSite { get; set; } = string.Empty;
        public string? LogoBase64 { get; set; }
        public string CorPrimaria { get; set; } = string.Empty;
        public string CorSecundaria { get; set; } = string.Empty;
        public string CorFundo { get; set; } = string.Empty;
        public string CorTexto { get; set; } = string.Empty;
        public string? CorAcento { get; set; }
    }
}
