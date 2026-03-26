using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Infraestrutura.Contexto;
using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Request.ConfiguracaoBarbearia;
using BarbeariaRocha.Modelos.Response.ConfiguracaoBarbearia;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ConfiguracaoBarbeariaApp(Contexto contexto) : IConfiguracaoBarbeariaApp
    {
        private readonly Contexto _contexto = contexto;

        public ConfiguracaoBarbeariaResponse Obter()
        {
            var config = _contexto.ConfiguracaoBarbearia.FirstOrDefault()
                ?? throw new Exception("Configuração da barbearia não encontrada.");

            return Mapear(config);
        }

        public ConfiguracaoBarbeariaResponse Criar(ConfiguracaoBarbeariaRequest request)
        {
            if (_contexto.ConfiguracaoBarbearia.Any())
                throw new Exception("Já existe uma configuração cadastrada. Use o endpoint de edição.");

            var config = new ConfiguracaoBarbearia
            {
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
            var config = _contexto.ConfiguracaoBarbearia.Find(id)
                ?? throw new Exception("Configuração não encontrada.");

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
            var config = _contexto.ConfiguracaoBarbearia.Find(id)
                ?? throw new Exception("Configuração não encontrada.");

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
