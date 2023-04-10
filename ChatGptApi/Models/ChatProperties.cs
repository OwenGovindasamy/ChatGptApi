using System.Text.Json.Serialization;

namespace ChatGptApi.Models
{
    public class ChatProperties
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; }

    }
}
