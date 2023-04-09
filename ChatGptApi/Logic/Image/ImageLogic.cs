using ChatGptApi.Interfaces;
using ChatGptApi.Models;
using System.Text;
using System.Text.Json;

namespace ChatGptApi.Logic.Image
{
    public class ImageLogic : IImageLogic
    {
        private readonly IConfiguration _mySettings;
        public ImageLogic(IConfiguration mySettings)
        {
            _mySettings = mySettings;
        }

        public async Task<string> CreateImage(ImageProperties image)
        {
            try
            {
                var ApiKey = _mySettings.GetValue<string>("Credentials:RapidApiKey");
                var ApiHost = _mySettings.GetValue<string>("Credentials:RapidApiHost");

                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_mySettings.GetValue<string>("Endpoints:CreateImageURI")),
                    Headers =
                {
                    { "X-RapidAPI-Key", ApiKey },
                    { "X-RapidAPI-Host", ApiHost },
                },
                    Content = new StringContent(JsonSerializer.Serialize(image), Encoding.UTF8, "application/json")
                };

                using var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                return body;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
