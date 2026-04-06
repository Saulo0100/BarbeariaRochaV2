using BarbeariaRocha.Aplicacao.Contratos;
using BarbeariaRocha.Modelos.Whatsapp;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BarbeariaRocha.Aplicacao.Servicos;

public class WhatsappService(IConfiguration configuration, ILogger<WhatsappService> logger) : IWhatsappService
{
    private readonly string _token = configuration["Whatsapp:Token"]
        ?? throw new InvalidOperationException("Whatsapp:Token não configurado.");
    private readonly string _endpoint = configuration["Whatsapp:endpoint"]
        ?? throw new InvalidOperationException("Whatsapp:endpoint não configurado.");

    public async Task EnviarMensagemAsync(string codigo, string numeroDestino)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var envio = new WhatsappEnvio(codigo, numeroDestino);
        var json = JsonSerializer.Serialize(envio);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(_endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                logger.LogWarning("WhatsApp retornou {StatusCode}: {Body}", (int)response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao enviar mensagem WhatsApp para {Numero}", numeroDestino);
            throw;
        }
    }
}
