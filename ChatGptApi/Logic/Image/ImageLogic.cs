using System.Net.Http.Headers;

namespace ChatGptApi.Logic.Image
{
    public class ImageLogic
    {
        private readonly IConfiguration _mySettings;
        public ImageLogic(IConfiguration mySettings)
        {
            _mySettings = mySettings;
        }

        public async Task<object> CreateImage(string prompt, int num, string size)
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

                Content = new StringContent("{\r\n    \"prompt\": \"A cute baby sea otter\",\r\n    \"n\": 2,\r\n    \"size\": \"1024x1024\"\r\n}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                return body;
            }
        }
    }
}
