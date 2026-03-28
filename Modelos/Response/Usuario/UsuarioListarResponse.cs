using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Response.Usuario
{
    public class UsuarioListarResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public required string Email { get; set; }
        public Perfil Perfil { get; set; }
        public string? Foto { get; set; }
    }
}
