using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Plugin.AI.AIAdapter
{
    internal class TongYi : IAIAdapter
    {
        private const string TongYiUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
        
        private HttpClient _httpClient;
        private string _model;

        private static JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public TongYi(string apiKey, string model)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _model = model;
        }
        public async Task<string> ProcessAsync(string systemMessage, IEnumerable<GroupChat> chats)
        {
            var tongYiChats = new List<TongYiHttpPostMessage>
            {
                new(
                    "system",
                    systemMessage),
            };
            foreach (var chat in chats)
            {
                var role = chat.IsBotMessage ? "assistant" : "user";
                var chatContent = chat.Content.Length >= 100 ? chat.Content[..100] : chat.Content;
                tongYiChats.Add(new(role, chatContent));
            }
            var postPayload = new TongYiHttpPost(
                _model,
                new TongYiHttpPostInput(
                    tongYiChats,
                    new TongYiHttpPostParams(
                        "text",
                        true)
                    )
                );
            var content = JsonContent.Create(postPayload, options : _serializerOptions);
            var response = await _httpClient.PostAsync(TongYiUrl, content);
            var responsePayload = await response.Content.ReadFromJsonAsync<TongYiHttpResponse>(_serializerOptions);
            if (string.IsNullOrEmpty(responsePayload.Output.Text))
            {
                if (!string.IsNullOrEmpty(responsePayload.Message))
                    return $"八嘎！本完犊子又{responsePayload.Message}啦。";
                return "出现了奇怪的错误";
            }
            return responsePayload.Output.Text;
        }
    }

    internal record struct TongYiHttpPost(
        string Model,
        TongYiHttpPostInput Input);

    internal record struct TongYiHttpPostInput(
        IEnumerable<TongYiHttpPostMessage> Messages,
        TongYiHttpPostParams Parameters);

    internal record struct TongYiHttpPostMessage(
        string Role,
        string Content);

    internal record struct TongYiHttpPostParams(
        string ResultFormat,
        bool EnableSearch);

    internal record struct TongYiHttpResponseOutput(
        string Text,
        string FinishReason);

    internal record struct TongYiHttpResponse(
        string Code,
        string Message,
        TongYiHttpResponseOutput Output);
}
