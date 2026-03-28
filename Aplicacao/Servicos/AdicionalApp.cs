using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Modelos.Entidades;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class AdicionalApp(Contexto contexto, ITenantService tenantService) : IAdicionalApp
    {
        private readonly Contexto _contexto = contexto;
        private readonly ITenantService _tenantService = tenantService;

        public List<object> ListarAdicionais()
        {
            var tenantId = _tenantService.ObterTenantId();
            return _contexto.Adicional
                .Where(a => a.TenantId == tenantId && !a.Excluido)
                .Select(a => (object)new { id = a.Id, nome = a.Nome, valor = a.Valor })
                .ToList();
        }

        public void CriarAdicional(string nome, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new Exception("O nome do adicional é obrigatório.");

            if (valor <= 0)
                throw new Exception("O valor do adicional deve ser maior que zero.");

            var tenantId = _tenantService.ObterTenantId();

            var duplicado = _contexto.Adicional
                .Any(a => a.TenantId == tenantId && a.Nome.ToLower() == nome.ToLower().Trim() && !a.Excluido);

            if (duplicado)
                throw new Exception("Já existe um adicional com este nome.");

            var adicional = new Adicional
            {
                TenantId = tenantId,
                Nome = nome.Trim(),
                Valor = valor
            };

            _contexto.Adicional.Add(adicional);
            _contexto.SaveChanges();
        }

        public void DeletarAdicional(int id)
        {
            var tenantId = _tenantService.ObterTenantId();
            var adicional = _contexto.Adicional.FirstOrDefault(a => a.Id == id && a.TenantId == tenantId)
                ?? throw new Exception("Adicional não encontrado.");

            adicional.Excluido = true;
            _contexto.SaveChanges();
        }
    }
}
