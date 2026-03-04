using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Usuario
{
    public class UsuarioCriarRequest
    {
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public required string Email { get; set; }
        public Perfil Perfil { get; set; }
        public TipoAgenda? Agenda { get; set; }
        public required string Descricao { get; set; }
        public required string Senha { get; set; }
        public decimal? Porcentagem { get; set; }
    }
}
