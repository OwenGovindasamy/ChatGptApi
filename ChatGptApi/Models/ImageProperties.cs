using System.Text.Json.Serialization;

namespace ChatGptApi.Models
{
    public class ImageProperties
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("n")]
        public int Number { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }
    }
}
