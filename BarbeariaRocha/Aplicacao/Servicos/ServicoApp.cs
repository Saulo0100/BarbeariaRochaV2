using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Servico;
using BarbeariaRocha.Modelos.Response.Servico;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ServicoApp(Contexto contexto) : IServicoApp
    {
        private readonly Contexto _contexto = contexto;

        public void CriarServico(ServicoCriarRequest request)
        {
            var servico = new Servico
            {
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(request.TempoEstimado)),
                Categoria = request.Categoria,
                Excluido = 0
            };

            _contexto.Servico.Add(servico);
            _contexto.SaveChanges();
        }

        public void DeletarServico(int id)
        {
            var servico = _contexto.Servico.Find(id) ?? throw new Exception("Serviço não encontrado.");
            servico.Excluido = 1;
            _contexto.SaveChanges();
        }

        public PaginacaoResultado<ServicoDetalhesResponse> ListarServicos(PaginacaoFiltro<ServicoFiltroRequest> filtro)
        {
            if (filtro == null)
                throw new ArgumentNullException(nameof(filtro));

            var query = _contexto.Servico
                .Where(s => s.Excluido == 0);

            if (filtro.Filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Filtro.Nome))
                    query = query.Where(s =>
                        s.Descricao.Contains(filtro.Filtro.Nome));

                if (filtro.Filtro.Categoria != 0)
                    query = query.Where(s =>
                        s.Categoria == filtro.Filtro.Categoria.ToString());
            }

            var totalRegistros = query.Count();

            var servicos = query
                .Skip((filtro.Pagina - 1) * filtro.ItensPorPagina)
                .Take(filtro.ItensPorPagina)
                .Select(s => new ServicoDetalhesResponse
                {
                    Id = s.Id,
                    Nome = s.Descricao,
                    Descricao = s.Descricao,
                    Valor = s.Valor,
                    TempoEstimado = s.TempoEstimado,
                    Categoria = s.Categoria
                })
                .ToList();

            return new PaginacaoResultado<ServicoDetalhesResponse>
            {
                Items = servicos,
                TotalRegistros = totalRegistros,
                PaginaAtual = filtro.Pagina,
                ItensPorPagina = filtro.ItensPorPagina
            };
        }
    }
}
