﻿using ChatGptApi.Interfaces;
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

        public async Task<string> CreateImage(string text)
        {
            var model = new ImageProperties { Prompt = text, Number = 1, Size = "1024x1024" };
            try
            {
                var apiKey = _mySettings.GetValue<string>("Credentials:RapidApiKey");
                var apiHost = _mySettings.GetValue<string>("Credentials:RapidApiHost");

                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_mySettings.GetValue<string>("Endpoints:CreateImageURI")),
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
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
