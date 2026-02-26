using BarbeariaRocha.Modelos.Response.Servico;

namespace BarbeariaRocha.Modelos.Response.Usuario
{
    public class UsuarioDetalhesResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Numero { get; set; }
        public required string Email { get; set; }
        public required string Perfil { get; set; }
        public byte[]? Foto { get; set; }
        public string? Descricao { get; set; }
        public string? Agenda { get; set; }
        public IEnumerable<ServicoDetalhesResponse>? Servicos { get; set; }
    }
}
