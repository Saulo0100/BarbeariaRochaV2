using BarbeariaRocha.Infraestrutura.Excecoes;
using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Infraestrutura.MultiTenancy;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.ConfiguracaoBarbearia;
using BarbeariaRocha.Modelos.Response.ConfiguracaoBarbearia;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ConfiguracaoBarbeariaApp(Contexto contexto, ITenantService tenantService) : IConfiguracaoBarbeariaApp
    {
        private readonly Contexto _contexto = contexto;
        private readonly ITenantService _tenantService = tenantService;

        public ConfiguracaoBarbeariaResponse? Obter()
        {
            var tenantId = _tenantService.ObterTenantId();
            var config = _contexto.ConfiguracaoBarbearia.FirstOrDefault(c => c.TenantId == tenantId);

            if (config == null)
                return null;

            return Mapear(config);
        }

        public ConfiguracaoBarbeariaResponse Criar(ConfiguracaoBarbeariaRequest request)
        {
            var tenantId = _tenantService.ObterTenantId();

            if (_contexto.ConfiguracaoBarbearia.Any(c => c.TenantId == tenantId))
                throw new AppException("Já existe uma configuração cadastrada. Use o endpoint de edição.");

            var config = new ConfiguracaoBarbearia
            {
                TenantId = tenantId,
                NumeroCelular = request.NumeroCelular,
                Rua = request.Rua,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = request.Cep
            };

            _contexto.ConfiguracaoBarbearia.Add(config);
            _contexto.SaveChanges();

            return Mapear(config);
        }

        public ConfiguracaoBarbeariaResponse Editar(int id, ConfiguracaoBarbeariaRequest request)
        {
            var tenantId = _tenantService.ObterTenantId();
            var config = _contexto.ConfiguracaoBarbearia.FirstOrDefault(c => c.Id == id && c.TenantId == tenantId)
                ?? throw new AppException("Configuração não encontrada.");

            config.NumeroCelular = request.NumeroCelular;
            config.Rua = request.Rua;
            config.Bairro = request.Bairro;
            config.Cidade = request.Cidade;
            config.Estado = request.Estado;
            config.Cep = request.Cep;

            _contexto.SaveChanges();

            return Mapear(config);
        }

        public void Deletar(int id)
        {
            var tenantId = _tenantService.ObterTenantId();
            var config = _contexto.ConfiguracaoBarbearia.FirstOrDefault(c => c.Id == id && c.TenantId == tenantId)
                ?? throw new AppException("Configuração não encontrada.");

            _contexto.ConfiguracaoBarbearia.Remove(config);
            _contexto.SaveChanges();
        }

        private static ConfiguracaoBarbeariaResponse Mapear(ConfiguracaoBarbearia c) => new()
        {
            Id = c.Id,
            NumeroCelular = c.NumeroCelular,
            Rua = c.Rua,
            Bairro = c.Bairro,
            Cidade = c.Cidade,
            Estado = c.Estado,
            Cep = c.Cep
        };
    }
}
