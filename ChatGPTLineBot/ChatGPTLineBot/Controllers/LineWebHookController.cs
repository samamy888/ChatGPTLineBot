using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using ChatGPTLineBot.Configs;
using ChatGPTLineBot.Services;
using isRock.LineBot;
using Lib.Redis.Client;
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

        public LineWebHookController(IOptions<Config> config, ILogger<LineWebHookController> logger,
            ChatGPTService chatGPTService )
        {
            _config = config.Value;
            _logger = logger;
            _chatGPTService = chatGPTService;
        }
        [Route("api/LineBotWebHook")]
        [HttpPost]
        public async Task<IActionResult> POST()
        {
            //設定ChannelAccessToken
            var AdminUserId = _config.AdminUserId;
            this.ChannelAccessToken = _config.ChannelAccessToken;
            try
            {
                

                //配合Line Verify
                if (ReceivedMessage == null || ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000") return Ok();
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //準備回覆訊息
                if (LineEvent.type.ToLower() != "message" || LineEvent.message.type != "text")
                    return Ok();

                var db = new RedisClient().Database;
                var message = LineEvent.message.text;
                var userId = LineEvent.source.userId;
                var status = db.StringGet(userId).ToString();
                if (string.IsNullOrEmpty(status))
                {
                    db.StringSet(userId, "自由提問");
                }
                if (status == "品牌故事")
                {
                    if(message== "品牌故事")
                    {
                        this.ReplyMessage(LineEvent.replyToken,
                       "進入品牌故事功能，可輸入品牌回答品牌故事!\n" +
                       "範例 : 香奈兒\n" +
                       "輸入 : 退出品牌故事，可回到自由提問。"
                       );
                       return Ok();
                    }
                    message = message + "的品牌故事，字數兩百字左右，請用正體中文回答";
                }
                if (message.StartsWith("測試"))
                {
                    this.ReplyMessage(LineEvent.replyToken, "測試功能開發中");
                    db.StringSet(userId, message);
                    return Ok();
                }
                if (message == "品牌故事")
                {
                    this.ReplyMessage(LineEvent.replyToken,
                        "進入品牌故事功能，可輸入品牌回答品牌故事!\n" +
                        "範例 : 香奈兒\n"+
                        "輸入 : 退出品牌故事，可回到自由提問。"
                        );
                    db.StringSet(userId, message);
                    return Ok();
                }
                if(message.Contains("退出品牌故事"))
                {
                    this.ReplyMessage(LineEvent.replyToken, "目前狀態 : 自由提問");
                    db.StringSet(userId, "");
                    return Ok();
                }
               
                status = db.StringGet(userId).ToString();
                var history = _chatGPTService.GetChatGPT().GetConversation(userId);

                var result = await _chatGPTService.CallChatGPT(message,userId);
                history.Messages.Add(new ChatGptMessage()
                {
                    Role = "assistant",
                    Content = result,
                });
                _chatGPTService.GetChatGPT().SetConversation(userId,history);
                var responseMsg = result.Trim();
                this.ReplyMessage(LineEvent.replyToken, responseMsg);

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