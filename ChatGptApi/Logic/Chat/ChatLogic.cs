using ChatGptApi.Interfaces;
using ChatGptApi.Models;
using System.Text.Json;
using System.Text;

namespace ChatGptApi.Logic.Chat
{
    public class ChatLogic : IChatLogic
    {
        private readonly IConfiguration _mySettings;
        public ChatLogic(IConfiguration mySettings)
        {
            _mySettings = mySettings;
        }
        public async Task<string> CreateChat(string chat)
        {
            ChatProperties model = new()
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage> 
                { 
                    new ChatMessage {
                        Role= "user",
                        Content = chat
                    }
                }
            };

            try
            {
                var apiKey = _mySettings.GetValue<string>("Credentials:RapidApiKey");
                var apiHost = _mySettings.GetValue<string>("Credentials:RapidApiHost");

                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_mySettings.GetValue<string>("Endpoints:ChatCompletions")),
                    Headers =
                {
                    { "X-RapidAPI-Key", apiKey },
                    { "X-RapidAPI-Host", apiHost },
                },
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                };
                using var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
