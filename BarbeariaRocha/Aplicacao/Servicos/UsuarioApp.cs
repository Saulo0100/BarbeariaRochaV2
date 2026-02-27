using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Usuario;
using BarbeariaRocha.Modelos.Response.Barbeiro;
using BarbeariaRocha.Modelos.Response.Usuario;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class UsuarioApp(Contexto contexto) : IUsuarioApp
    {
        public readonly Contexto _contexto = contexto;

        public void Criar(UsuarioCriarRequest request)
        {
            var usuario = new Usuario(request);
            _contexto.Usuario.Add(usuario);
            _contexto.SaveChanges();
        }

        public void Editar(int id, UsuarioEditarRequest request)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new Exception();

            if (!string.IsNullOrWhiteSpace(request.Nome))
                usuario.Nome = request.Nome;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                Usuario.ValidarEmail(request.Email);
                usuario.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Numero))
            {
                Usuario.ValidarNumero(request.Numero);
                usuario.Numero = request.Numero;
            }

            if (!string.IsNullOrWhiteSpace(request.Descricao))
                usuario.Descricao = request.Descricao;

            if (!string.IsNullOrWhiteSpace(request.Agenda))
                usuario.Agenda = request.Agenda;

            _contexto.SaveChanges();
        }

        public void Excluir(int id)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new Exception();
            usuario.Excluido = 1;
            _contexto.SaveChanges();
        }

        public IEnumerable<BarbeirosDetalhesResponse> ObterBarbeiros()
        {
            var barbeiros = _contexto.Usuario.Where(u => u.Perfil == Perfil.Barbeiro.ToString() && u.Excluido == 0).ToList();
            return barbeiros.Select(b => new BarbeirosDetalhesResponse
            {
                Id = b.Id,
                Nome = b.Nome,
                Descricao = b.Descricao,
                Agenda = b.Agenda!
            });
        }

        public UsuarioDetalhesResponse ObterPorId(int id)
        {
            var usuario = _contexto.Usuario.Find(id) ?? throw new Exception();

            return new UsuarioDetalhesResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Descricao = usuario.Descricao,
                Numero = usuario.Numero,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Foto = usuario.Foto,
                Agenda = usuario.Agenda,
            };
        }

        public PaginacaoResultado<UsuarioDetalhesResponse> ObterTodos(PaginacaoFiltro<UsuarioFiltroRequest> filtro)
        {
            var query = _contexto.Usuario.Where(u => u.Excluido == 0).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Filtro?.Nome))
                query = query.Where(u => u.Nome.Contains(filtro.Filtro.Nome));

            if (!string.IsNullOrWhiteSpace(filtro.Filtro?.Perfil))
                query = query.Where(u => u.Perfil == filtro.Filtro.Perfil);

            var totalRegistros = query.Count();

            var usuarios = query
                .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                .Take(filtro.ItensPorPagina)
                .Select(u => new UsuarioDetalhesResponse
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    Descricao = u.Descricao,
                    Numero = u.Numero,
                    Email = u.Email,
                    Perfil = u.Perfil,
                    Foto = u.Foto,
                    Agenda = u.Agenda
                })
                .ToList();

            return new PaginacaoResultado<UsuarioDetalhesResponse>
            {
                Items = usuarios,
                TotalRegistros = totalRegistros,
                PaginaAtual = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina
            };
        }
    }
}
