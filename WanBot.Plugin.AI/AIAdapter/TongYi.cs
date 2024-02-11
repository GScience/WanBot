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
        public async Task<string> ProcessAsync(string systemMessage, string userMessage)
        {
            var postPayload = new TongYiHttpPost(
                _model,
                new TongYiHttpPostInput(
                    new TongYiHttpPostMessage[]
                    {
                        new TongYiHttpPostMessage(
                            "system",
                            systemMessage),
                        new TongYiHttpPostMessage(
                            "user",
                            userMessage)
                    },
                    new TongYiHttpPostParams(
                        "text",
                        true)
                    )
                );
            var content = JsonContent.Create(postPayload, options : _serializerOptions);
            var response = await _httpClient.PostAsync(TongYiUrl, content);
            var responsePayload = 
                await response.Content.ReadFromJsonAsync<TongYiHttpResponse>(_serializerOptions) 
                ?? throw new Exception("Failed to get http response");
            return responsePayload.Output.Text;
        }
    }

    internal record TongYiHttpPost(
        string Model,
        TongYiHttpPostInput Input);

    internal record TongYiHttpPostInput(
        IEnumerable<TongYiHttpPostMessage> Messages,
        TongYiHttpPostParams Parameters);

    internal record TongYiHttpPostMessage(
        string Role,
        string Content);

    internal record TongYiHttpPostParams(
        string ResultFormat,
        bool EnableSearch);

    internal record TongYiHttpResponseOutput(
        string Text,
        string FinishReason);

    internal record TongYiHttpResponse(
        string Code,
        TongYiHttpResponseOutput Output);
}
