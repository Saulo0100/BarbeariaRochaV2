using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Response.ConfiguracaoSite;
using System.Text.Json;

namespace BarbeariaRocha.Aplicacao.Servicos
{
    public class ConfiguracaoSiteApp(HttpClient httpClient) : IConfiguracaoSiteApp
    {
        private readonly HttpClient _httpClient = httpClient;
        public async Task<ConfiguracaoSiteResponse> ObterConfiguracao(string dominio)
        {
            if (string.IsNullOrWhiteSpace(dominio))
                throw new ArgumentException("Domínio não informado.");

            var response = await _httpClient.GetAsync($"configuracao?dominio={dominio}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao buscar configuração. Status: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            var config = JsonSerializer.Deserialize<ConfiguracaoSiteResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return config == null
                ? throw new Exception("Configuração não encontrada.")
                : new ConfiguracaoSiteResponse
                {
                    NomeSite = config.NomeSite,
                    LogoBase64 = config.Logo != null ? Convert.ToBase64String(config.Logo) : null,
                    CorPrimaria = config.CorPrimaria,
                    CorSecundaria = config.CorSecundaria,
                    CorFundo = config.CorFundo,
                    CorTexto = config.CorTexto,
                    CorAcento = config.CorAcento
                };
        }

        public async Task<bool> VerificarDominio(string dominio)
        {
            if (string.IsNullOrWhiteSpace(dominio))
                throw new ArgumentException("Domínio não informado.");

            var response = await _httpClient.GetAsync($"verificar-dominio?dominio={dominio}");

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();

            // 🔥 Caso a API retorne: { "autorizado": true }
            var autorizado = JsonSerializer.Deserialize<bool>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return autorizado;
        }
    }
}
