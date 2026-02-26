using System.Text.Json.Serialization;

namespace BarbeariaRocha.Modelos.Whatsapp
{
    public class Language
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
