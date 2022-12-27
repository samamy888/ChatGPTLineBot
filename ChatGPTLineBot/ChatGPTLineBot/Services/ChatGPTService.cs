using ChatGPTLineBot.Models;
using Microsoft.Extensions.Options;
using System.Text;
using ChatGPTLineBot.Configs;

namespace ChatGPTLineBot.Services
{
    public class ChatGPTService
    {
        private readonly Config _config;
        private readonly ILogger<ChatGPTService> _logger;

        public ChatGPTService(IOptions<Config> config,ILogger<ChatGPTService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }
        public async Task<Result> CallChatGPT(string msg)
        {
            _logger.LogWarning(msg);
            HttpClient client = new HttpClient();
            string uri = "https://api.openai.com/v1/completions";

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {_config.ChatGPTKey}");

            var JsonString = @"
            {
                ""model"": ""text-davinci-003"",
                ""prompt"": ""question"",
                ""max_tokens"": 4000,
                ""temperature"": 0
            }
            ".Replace("question", msg);
            var content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uri, content);
            var JSON = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(JSON);
            _logger.LogWarning(JSON);
            return result;
        }
    }
}
