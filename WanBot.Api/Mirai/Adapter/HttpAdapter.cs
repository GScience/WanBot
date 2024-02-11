using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Network;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Adapter
{
    /// <summary>
    /// Http 适配器
    /// </summary>
    public class HttpAdapter : IAdapter
    {
        private readonly ILogger _logger;
        public string BaseUrl { get; protected set; }
        public string VerifyKey { get; protected set; }
        public long QQ { get; protected set; }
        public bool IsConnected { get; private set; }

        private HttpClient _httpClient;

        public HttpAdapter(ILogger logger, string baseUrl, string verifyKey, long qq)
        {
            _logger = logger;
            BaseUrl = baseUrl;
            VerifyKey = verifyKey;
            QQ = qq;

            var socketsHandler = new SocketsHttpHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                EnableMultipleHttp2Connections = false,
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                UseProxy = false,
                Proxy = null
            };

            _httpClient = new HttpClient(socketsHandler)
            {
                DefaultRequestVersion = new Version(1, 1),
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };
        }

        public async Task<ResponsePayload?> SendAsync<ResponsePayload, RequestPayload>(RequestPayload? request)
            where RequestPayload : IRequest
            where ResponsePayload : IResponse
        {
            if (request == null)
                return default;

            using var httpResponse = await HttpRequestHelper<RequestPayload>.SendHandleAsync(this, request);

            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception($"Http request with error code: {httpResponse.StatusCode}");

            if (typeof(Response).IsAssignableFrom(typeof(ResponsePayload)))
            {
                var obj = await httpResponse.Content.ReadFromJsonAsync<ResponsePayload>(MiraiJsonContext.Default.Options);
                var response = obj as Response;
                if (response!.Code != ResponseCode.Ok)
                    throw new Exception($"{ResponseCode.Reason(response!.Code)}");
                return obj;
            }
            else
            {
                var obj = await httpResponse.Content.ReadFromJsonAsync<ResponsePayload>(MiraiJsonContext.Default.Options);

                return obj!;
            }
        }

        internal async Task<HttpResponseMessage> GetAsync(string apiPath)
        {
            return await _httpClient.GetAsync($"{BaseUrl}/{apiPath}");
        }

        internal async Task<HttpResponseMessage> PostAsync(string apiName, HttpContent content)
        {
            var url = $"{BaseUrl}/{apiName}";
            try
            {
                return await _httpClient.PostAsync(url, content);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to post content to {url} because: {e}\nPayload:\n{await content.ReadAsStringAsync()}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task ConnectAsync()
        {
            IsConnected = true;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Api名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpApiAttribute : Attribute
    {
        public string Name { get; }
        public HttpAdapterMethod Method { get; }

        public HttpApiAttribute(string name, HttpAdapterMethod method = HttpAdapterMethod.Get)
        {
            Name = name;
            Method = method;
        }
    }

    /// <summary>
    /// 适配器方法
    /// </summary>
    public enum HttpAdapterMethod
    {
        Get, PostJson, PostMultipart
    }
}
