namespace BarbeariaRocha.Modelos.Request.Autenticacao
{
    public class RedefinirSenhaRequest
    {
        public required string Token { get; set; }
        public required string NovaSenha { get; set; }
    }
}
