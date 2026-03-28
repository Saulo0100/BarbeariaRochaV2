namespace BarbeariaRocha.Modelos.Request.Autenticacao
{
    public class LoginRequest
    {
        public required string Numero { get; set; }
        public required string Senha { get; set; }
    }
}
