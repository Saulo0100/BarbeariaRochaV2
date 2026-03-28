namespace BarbeariaRocha.Modelos.Request.Autenticacao
{
    public class EsqueceuSenhaRequest
    {
        public required string Numero { get; set; }
        public required string Email { get; set; }
    }
}
