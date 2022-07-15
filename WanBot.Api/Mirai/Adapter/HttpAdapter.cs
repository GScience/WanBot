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
        public string BaseUrl { get; protected set; }
        public string VerifyKey { get; protected set; }
        public long QQ { get; protected set; }
        public bool IsConnected { get; private set; }

        private HttpClient _httpClient;

        public HttpAdapter(string baseUrl, string verifyKey, long qq)
        {
            BaseUrl = baseUrl;
            VerifyKey = verifyKey;
            QQ = qq;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public async Task<ResponsePayload?> SendAsync<ResponsePayload, RequestPayload>(RequestPayload request)
            where RequestPayload : class
            where ResponsePayload : class
        {
            using var httpResponse = await HttpRequestHelper<RequestPayload>.SendHandleAsync(this, request);

            if (!httpResponse.IsSuccessStatusCode)
                throw new Exception($"Http request with error code: {httpResponse.StatusCode}");

            if (typeof(IResponse).IsAssignableFrom(typeof(ResponsePayload)))
            {
                var obj = await httpResponse.Content.ReadFromJsonAsync<ResponsePayload>(MiraiJsonContext.Default.Options);

                return obj!;
            }
            else
            {
                var obj = await httpResponse.Content.ReadFromJsonAsync<Response<ResponsePayload>>(MiraiJsonContext.Default.Options);

                if (obj!.Code != ResponseCode.Ok)
                    throw new Exception($"{ResponseCode.Reason(obj!.Code)}");
                return obj!.Data;
            }
        }

        internal async Task<HttpResponseMessage> GetAsync(string apiPath)
        {
            return await _httpClient.GetAsync($"{BaseUrl}/{apiPath}");
        }

        internal async Task<HttpResponseMessage> PostAsync(string apiName, HttpContent content)
        {
            return await _httpClient.PostAsync($"{BaseUrl}/{apiName}", content);
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
