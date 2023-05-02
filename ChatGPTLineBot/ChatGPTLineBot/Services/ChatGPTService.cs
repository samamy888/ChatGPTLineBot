using ChatGPTLineBot.Models;
using Microsoft.Extensions.Options;
using System.Text;
using ChatGPTLineBot.Configs;
using Lib.Redis.Client;
using isRock.LineBot;
using Microsoft.Extensions.Logging;
using ChatGPT.Net;
using isRock.LIFF;

namespace ChatGPTLineBot.Services
{
    public class ChatGPTService
    {
        private readonly Config _config;
        private readonly ILogger<ChatGPTService> _logger;
        private ChatGpt _chatGPT;

        public ChatGPTService(IOptions<Config> config,ILogger<ChatGPTService> logger)
        {
            _config = config.Value;
            _logger = logger;
            _chatGPT = new ChatGpt(_config.ChatGPTKey);
        }
        public async Task<string> CallChatGPT(string msg, string userId)
        {
            _logger.LogWarning(msg);
            var result = await _chatGPT.Ask(msg, userId);
            _logger.LogWarning(result);
            return result;
        }
        public ChatGpt GetChatGPT()
        {
            return _chatGPT;
        }
        //public async Task<string> CallChatGPT(string msg)
        //{
        //    _logger.LogWarning(msg);
        //    var result = await openAIAPI.Completions.GetCompletion(msg);
        //    _logger.LogWarning(result);
        //    return result;
        //}
        //public async Task<Result> CallChatGPT(string msg)
        //{
        //    _logger.LogWarning(msg);
        //    HttpClient client = new HttpClient();
        //    string uri = "https://api.openai.com/v1/completions";

        //    // Request headers.
        //    client.DefaultRequestHeaders.Add(
        //        "Authorization", $"Bearer {_config.ChatGPTKey}");

        //    var JsonString = @"
        //    {
        //        ""model"": ""text-davinci-003"",
        //        ""prompt"": ""question"",
        //        ""max_tokens"": 4000,
        //        ""temperature"": 0
        //    }
        //    ".Replace("question", msg);
        //    var content = new StringContent(JsonString, Encoding.UTF8, "application/json");
        //    var response = await client.PostAsync(uri, content);
        //    var JSON = await response.Content.ReadAsStringAsync();
        //    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(JSON);
        //    _logger.LogWarning(JSON);
        //    return result;
        //}
    }
}
