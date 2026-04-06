using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Infraestrutura.Repositorios;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Paginacao;
using BarbeariaRocha.Modelos.Request.Servico;
using BarbeariaRocha.Modelos.Response.Servico;
using Microsoft.Extensions.Caching.Memory;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ServicoApp(IRepositorio<Servico> repositorio, ITenantService tenantService, IMemoryCache cache) : IServicoApp
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public void CriarServico(ServicoCriarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Descricao))
                throw new ArgumentException("A descrição é obrigatória.");

            if (request.Valor <= 0)
                throw new ArgumentException("O valor é obrigatório e deve ser maior que zero.");

            if (request.TempoEstimado <= 0)
                throw new ArgumentException("O tempo estimado é obrigatório e deve ser maior que zero.");

            if (!Enum.IsDefined(typeof(CategoriaServico), request.Categoria))
                throw new ArgumentException("A categoria é obrigatória e deve ser válida.");

            var tenantId = tenantService.ObterTenantId();

            var servico = new Servico
            {
                TenantId = tenantId,
                Descricao = request.Descricao,
                Valor = request.Valor,
                TempoEstimado = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(request.TempoEstimado)),
                Categoria = request.Categoria.ToString()
            };

            repositorio.AdicionarAsync(servico).GetAwaiter().GetResult();
            repositorio.SalvarAsync().GetAwaiter().GetResult();
            cache.Remove($"servicos:{tenantId}");
        }

        public void DeletarServico(int id)
        {
            var tenantId = tenantService.ObterTenantId();
            var servico = repositorio.Query()
                .FirstOrDefault(s => s.Id == id && s.TenantId == tenantId)
                ?? throw new Exception("Serviço não encontrado.");

            servico.Excluido = true;
            repositorio.Atualizar(servico);
            repositorio.SalvarAsync().GetAwaiter().GetResult();
            cache.Remove($"servicos:{tenantId}");
        }

        public PaginacaoResultado<ServicoDetalhesResponse> ListarServicos(PaginacaoFiltro<ServicoFiltroRequest> filtro)
        {
            if (filtro == null)
                throw new ArgumentNullException(nameof(filtro));

            var tenantId = tenantService.ObterTenantId();

            var query = repositorio.Query()
                .Where(s => s.TenantId == tenantId && s.Excluido == false);

            if (filtro.Filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Filtro.Nome))
                    query = query.Where(s => s.Descricao.Contains(filtro.Filtro.Nome));

                if (filtro.Filtro.Categoria != 0)
                    query = query.Where(s => s.Categoria == filtro.Filtro.Categoria.ToString());
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
                    Categoria = s.Categoria,
                    RequerDuasEtapas = s.RequerDuasEtapas,
                    IntervaloMinimoHoras = s.IntervaloMinimoHoras,
                    DescricaoEtapa1 = s.DescricaoEtapa1,
                    DescricaoEtapa2 = s.DescricaoEtapa2
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
