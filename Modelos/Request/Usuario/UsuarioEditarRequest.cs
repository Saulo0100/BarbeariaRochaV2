using BarbeariaRocha.Modelos.Enums;

namespace BarbeariaRocha.Modelos.Request.Usuario
{
    public class UsuarioEditarRequest
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Numero { get; set; }
        public string? Descricao { get; set; }
        public TipoAgenda? Agenda { get; set; }
        public string? Foto { get; set; }
    }
}
