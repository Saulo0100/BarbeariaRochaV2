using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Excecao;
using BarbeariaRocha.Modelos.Response.Excecao;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ExcecaoApp(Contexto contexto) : IExcecaoApp
    {
        private readonly Contexto _contexto = contexto;

        public void CriarExcecao(ExcecaoCriarRequest request)
        {
            if (request.BarbeiroId.HasValue)
            {
                var barbeiro = _contexto.Usuario.Find(request.BarbeiroId.Value);
                if (barbeiro == null || barbeiro.Excluido)
                    throw new Exception("Barbeiro não encontrado.");

                var excecaoExistente = _contexto.Excecao
                                        .Where(x => x.BarbeiroId == request.BarbeiroId.Value
                                        && x.Data.Date == request.Data.Date)
                                        .FirstOrDefault();
                if (excecaoExistente != null)
                    throw new Exception("Já existe uma exceção para esse dia e barbeiro cadastradas.");

            }



            var excecao = new Excecao(request);
            _contexto.Excecao.Add(excecao);
            _contexto.SaveChanges();
        }

        public void DeletarExcecao(int id)
        {
            var excecao = _contexto.Excecao.Find(id) ?? throw new Exception("Exceção não encontrada.");
            excecao.Excluido = true;
            _contexto.SaveChanges();
        }

        public ExcecaoDetalhesResponse ObterPorId(int id)
        {
            var excecao = _contexto.Excecao.Find(id) ?? throw new Exception("Exceção não encontrada.");

            if (excecao.Excluido)
                throw new Exception("Exceção não encontrada.");

            string? nomeBarbeiro = null;
            if (excecao.BarbeiroId.HasValue)
            {
                var barbeiro = _contexto.Usuario.Find(excecao.BarbeiroId.Value);
                nomeBarbeiro = barbeiro?.Nome;
            }

            return new ExcecaoDetalhesResponse
            {
                Id = excecao.Id,
                Data = excecao.Data,
                Descricao = excecao.Descricao,
                BarbeiroId = excecao.BarbeiroId,
                NomeBarbeiro = nomeBarbeiro
            };
        }

        public PaginacaoResultado<ExcecaoDetalhesResponse> ListarExcecoes(PaginacaoFiltro<ExcecaoFiltroRequest> filtro)
        {
            var query = _contexto.Excecao
                .Where(e => e.Excluido == false)
                .AsQueryable();

            if (filtro.Filtro != null)
            {
                if (filtro.Filtro.DataInicio.HasValue)
                {
                    var dataInicio = filtro.Filtro.DataInicio.Value.Date;
                    query = query.Where(e => e.Data.Date >= dataInicio);
                }

                if (filtro.Filtro.DataFim.HasValue)
                {
                    var dataFim = filtro.Filtro.DataFim.Value.Date;
                    query = query.Where(e => e.Data.Date <= dataFim);
                }

                if (filtro.Filtro.BarbeiroId.HasValue)
                {
                    query = query.Where(e => e.BarbeiroId == filtro.Filtro.BarbeiroId.Value || e.BarbeiroId == null);
                }
            }

            var totalRegistros = query.Count();

            var excecoes = query
                .OrderBy(e => e.Data)
                .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                .Take(filtro.ItensPorPagina)
                .ToList();

            var resultado = excecoes.Select(e =>
            {
                string? nomeBarbeiro = null;
                if (e.BarbeiroId.HasValue)
                {
                    var barbeiro = _contexto.Usuario.Find(e.BarbeiroId.Value);
                    nomeBarbeiro = barbeiro?.Nome;
                }

                return new ExcecaoDetalhesResponse
                {
                    Id = e.Id,
                    Data = e.Data,
                    Descricao = e.Descricao,
                    BarbeiroId = e.BarbeiroId,
                    NomeBarbeiro = nomeBarbeiro
                };
            }).ToList();

            return new PaginacaoResultado<ExcecaoDetalhesResponse>
            {
                Items = resultado,
                TotalRegistros = totalRegistros,
                PaginaAtual = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina
            };
        }

        public IEnumerable<ExcecaoDetalhesResponse> ObterPorBarbeiro(int barbeiroId)
        {
            var excecoes = _contexto.Excecao
                .Where(e => e.Excluido == false
                            && e.BarbeiroId == barbeiroId
                            && e.Data.Date >= DateTime.Today)
                .ToList();

            var resultado = excecoes.Select(e =>
            {
                string? nomeBarbeiro = null;
                if (e.BarbeiroId.HasValue)
                {
                    var barbeiro = _contexto.Usuario.Find(e.BarbeiroId.Value);
                    nomeBarbeiro = barbeiro?.Nome;
                }

                return new ExcecaoDetalhesResponse
                {
                    Id = e.Id,
                    Data = e.Data,
                    Descricao = e.Descricao,
                    BarbeiroId = e.BarbeiroId,
                    NomeBarbeiro = nomeBarbeiro
                };
            }).ToList();

            return resultado;
        }
    }
}
