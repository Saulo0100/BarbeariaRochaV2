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
        // ==================== CONFIGURACAO DE HORARIOS ====================
        // Intervalo padrao entre slots (em minutos) - facil de alterar
        private const int IntervaloMinutos = 40;

        // Segunda a Sexta
        private static readonly TimeOnly SemanaInicio = TimeOnly.Parse("09:00");
        private static readonly TimeOnly SemanaAlmocoInicio = TimeOnly.Parse("12:20");
        private static readonly TimeOnly SemanaAlmocoFim = TimeOnly.Parse("13:20");
        private static readonly TimeOnly SemanaFim = TimeOnly.Parse("20:00");

        // Sabado
        private static readonly TimeOnly SabadoInicio = TimeOnly.Parse("10:00");
        private static readonly TimeOnly SabadoAlmocoInicio = TimeOnly.Parse("12:00");
        private static readonly TimeOnly SabadoAlmocoFim = TimeOnly.Parse("13:00");
        private static readonly TimeOnly SabadoFim = TimeOnly.Parse("20:00");

        /// <summary>
        /// Retorna os horarios disponiveis para segunda a sexta.
        /// 09:00, 09:40, 10:20, 11:00, 11:40, 12:20 | 13:20, 14:00, 14:40 ... ate 20:00
        /// </summary>
        public static List<TimeOnly> MontarHorarioDiaSemana()
        {
            return GerarSlots(SemanaInicio, SemanaAlmocoInicio, SemanaAlmocoFim, SemanaFim);
        }

        /// <summary>
        /// Retorna os horarios disponiveis para sabado.
        /// 10:00, 10:40, 11:20, 12:00 | 13:00, 13:40 ... ate 20:00
        /// </summary>
        public static List<TimeOnly> MontarHorarioSabado()
        {
            return GerarSlots(SabadoInicio, SabadoAlmocoInicio, SabadoAlmocoFim, SabadoFim);
        }

        /// <summary>
        /// Retorna os horarios para uma data especifica, considerando o dia da semana.
        /// Domingo retorna lista vazia (barbearia fechada).
        /// </summary>
        public static List<TimeOnly> ObterHorariosPorData(DateTime data)
        {
            return data.DayOfWeek switch
            {
                DayOfWeek.Sunday => new List<TimeOnly>(),
                DayOfWeek.Saturday => MontarHorarioSabado(),
                _ => MontarHorarioDiaSemana()
            };
        }

        /// <summary>
        /// Verifica se a barbearia esta aberta no dia informado.
        /// </summary>
        public static bool BarbeariaAberta(DateTime data)
        {
            return data.DayOfWeek != DayOfWeek.Sunday;
        }

        private static List<TimeOnly> GerarSlots(TimeOnly inicioManha, TimeOnly almocoInicio, TimeOnly almocoFim, TimeOnly fimDia)
        {
            var horarios = new List<TimeOnly>();

            var atual = inicioManha;
            while (atual <= almocoInicio)
            {
                horarios.Add(atual);
                atual = atual.AddMinutes(IntervaloMinutos);
            }

            atual = almocoFim;
            while (atual <= fimDia)
            {
                horarios.Add(atual);
                atual = atual.AddMinutes(IntervaloMinutos);
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
