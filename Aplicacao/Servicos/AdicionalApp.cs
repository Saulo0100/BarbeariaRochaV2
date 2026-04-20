using BarbeariaRocha.Infraestrutura.Excecoes;
using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Infraestrutura.Repositorios;
using BarbeariaRocha.Modelos.Entidades;
using Microsoft.Extensions.Caching.Memory;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AdicionalApp(IRepositorio<Adicional> repositorio, ITenantService tenantService, IMemoryCache cache) : IAdicionalApp
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public List<object> ListarAdicionais()
        {
            var tenantId = tenantService.ObterTenantId();
            var cacheKey = $"adicionais:{tenantId}";

            if (cache.TryGetValue(cacheKey, out List<object>? cached) && cached != null)
                return cached;

            var resultado = repositorio.Query()
                .Where(a => a.TenantId == tenantId && !a.Excluido)
                .Select(a => (object)new { id = a.Id, nome = a.Nome, valor = a.Valor })
                .ToList();

            cache.Set(cacheKey, resultado, CacheDuration);
            return resultado;
        }

        public void CriarAdicional(string nome, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new AppException("O nome do adicional é obrigatório.");

            if (valor <= 0)
                throw new AppException("O valor do adicional deve ser maior que zero.");

            var tenantId = tenantService.ObterTenantId();

            var duplicado = repositorio.Query()
                .Any(a => a.TenantId == tenantId && a.Nome.ToLower() == nome.ToLower().Trim() && !a.Excluido);

            if (duplicado)
                throw new AppException("Já existe um adicional com este nome.");

            var adicional = new Adicional
            {
                TenantId = tenantId,
                Nome = nome.Trim(),
                Valor = valor
            };

            repositorio.AdicionarAsync(adicional).GetAwaiter().GetResult();
            repositorio.SalvarAsync().GetAwaiter().GetResult();
            cache.Remove($"adicionais:{tenantId}");
        }

        public void DeletarAdicional(int id)
        {
            var tenantId = tenantService.ObterTenantId();
            var adicional = repositorio.Query()
                .FirstOrDefault(a => a.Id == id && a.TenantId == tenantId)
                ?? throw new AppException("Adicional não encontrado.");

            adicional.Excluido = true;
            repositorio.Atualizar(adicional);
            repositorio.SalvarAsync().GetAwaiter().GetResult();
            cache.Remove($"adicionais:{tenantId}");
        }
    }
}
