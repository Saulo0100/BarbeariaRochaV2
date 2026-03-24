using BarbeariaRocha.Modelos.Response.ConfiguracaoSite;

namespace BarbeariaRocha.Aplicacao.Contratos
{
    public interface IConfiguracaoSiteApp
    {
        ConfiguracaoSiteResponse ObterConfiguracao(string dominio);
        bool VerificarDominio(string dominio);
    }
}
