using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatGPTLineBot.Configs;
using ChatGPTLineBot.Services;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace isRock.Template
{
    public class LineWebHookController : LineWebHookControllerBase
    {
        private readonly Config _config;
        private readonly ILogger<LineWebHookController> _logger;
        private readonly ChatGPTService _chatGPTService;

        public LineWebHookController(IOptions<Config> config, ILogger<LineWebHookController> logger, ChatGPTService chatGPTService )
        {
            _config = config.Value;
            _logger = logger;
            _chatGPTService = chatGPTService;
        }
        [Route("api/LineBotWebHook")]
        [HttpPost]
        public async Task<IActionResult> POST()
        {
            var AdminUserId = _config.AdminUserId;
            this.ChannelAccessToken = _config.ChannelAccessToken;
            try
            {
                //設定ChannelAccessToken
                
                //配合Line Verify
                if (ReceivedMessage == null || ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000") return Ok();
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = string.Empty;
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    var result = await _chatGPTService.CallChatGPT(LineEvent.message.text);
                    responseMsg = result.Choices.FirstOrDefault().Text.Trim();
                    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}