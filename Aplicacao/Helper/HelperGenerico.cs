using BarbeariaRocha.Modelos.Entidades;
using BarbeariaRocha.Modelos.Enums;
using BarbeariaRocha.Modelos.Whatsapp;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BarbeariaRocha.Aplicacao.Helper
{
    public static class HelperGenerico
    {
        // ==================== HORARIOS DINAMICOS (via ConfiguracaoHorario do banco) ====================

        /// <summary>
        /// Gera a lista de slots de horário a partir de uma ConfiguracaoHorario do banco.
        /// Retorna lista vazia se o dia estiver fechado.
        /// </summary>
        public static List<TimeOnly> MontarHorariosPorConfig(ConfiguracaoHorario config)
        {
            if (!config.Aberto || config.HoraInicio == null || config.HoraFim == null)
                return new List<TimeOnly>();

            var intervalo = config.IntervaloMinutos > 0 ? config.IntervaloMinutos : 40;

            if (config.AlmocoInicio.HasValue && config.AlmocoFim.HasValue)
                return GerarSlots(config.HoraInicio.Value, config.AlmocoInicio.Value, config.AlmocoFim.Value, config.HoraFim.Value, intervalo);

            return GerarSlotsContinuos(config.HoraInicio.Value, config.HoraFim.Value, intervalo);
        }

        private static List<TimeOnly> GerarSlots(TimeOnly inicioManha, TimeOnly almocoInicio, TimeOnly almocoFim, TimeOnly fimDia, int intervalo)
        {
            var horarios = new List<TimeOnly>();

            var atual = inicioManha;
            while (atual <= almocoInicio)
            {
                horarios.Add(atual);
                atual = atual.AddMinutes(intervalo);
            }

            atual = almocoFim;
            while (atual <= fimDia)
            {
                horarios.Add(atual);
                atual = atual.AddMinutes(intervalo);
            }

            return horarios;
        }

        private static List<TimeOnly> GerarSlotsContinuos(TimeOnly inicio, TimeOnly fim, int intervalo)
        {
            var horarios = new List<TimeOnly>();
            var atual = inicio;
            while (atual <= fim)
            {
                horarios.Add(atual);
                atual = atual.AddMinutes(intervalo);
            }
            return horarios;
        }

        // ==================== UTILIDADES ====================

        public static int GerarCodigoConfirmacao()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public static int GerarDiasAgenda(TipoAgenda agenda)
        {
            switch (agenda)
            {
                case TipoAgenda.Diaria: return 1;
                case TipoAgenda.Semanal: return 7 - (int)DateTime.Now.DayOfWeek;
                case TipoAgenda.Quinzenal: return (7 - (int)DateTime.Now.DayOfWeek) + 7;
                case TipoAgenda.Mensal: return 30;
                case TipoAgenda.Fechada: return 0;
                default: throw new NotImplementedException();
            }
        }

        public static void EnviarMensagem(string codigo, string numeroEnvio)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            string? Token = configuration.GetSection("Whatsapp:Token").Value;
            string? Endpoint = configuration.GetSection("Whatsapp:Endpoint").Value;
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var envio = new WhatsappEnvio(codigo, numeroEnvio);
            var json = JsonSerializer.Serialize(envio);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(Endpoint, content).Result;
            var result = response.Content.ReadAsStringAsync();
        }

        public static string RemoveMask(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return Regex.Replace(value, @"\D", "");
        }
    }
}
