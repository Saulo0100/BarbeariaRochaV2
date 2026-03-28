using BarbeariaRocha.Modelos.Request.ConfiguracaoBarbearia;
using BarbeariaRocha.Modelos.Response.ConfiguracaoBarbearia;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IConfiguracaoBarbeariaApp
    {
        ConfiguracaoBarbeariaResponse Obter();
        ConfiguracaoBarbeariaResponse Criar(ConfiguracaoBarbeariaRequest request);
        ConfiguracaoBarbeariaResponse Editar(int id, ConfiguracaoBarbeariaRequest request);
        void Deletar(int id);
    }
}
