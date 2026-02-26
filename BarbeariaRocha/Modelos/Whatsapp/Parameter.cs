using System.Text.Json.Serialization;

namespace BarbeariaRocha.Modelos.Whatsapp
{
    public class Parameter
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
