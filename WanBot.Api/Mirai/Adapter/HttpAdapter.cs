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
            var obj = await httpResponse.Content.ReadFromJsonAsync<Response<ResponsePayload>>(MiraiJsonContext.Default.Options);

            return obj!.Data;
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
    }

    /// <summary>
    /// Api名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpApiAttribute : Attribute
    {
        public string Name { get; }

        public HttpApiAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 适配器方法
    /// </summary>
    public enum HttpAdapterMethod
    {
        Get, PostJson, PostMultipart
    }


    /// <summary>
    /// 适配器方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HttpAdapterMethodAttribute : Attribute
    {
        public HttpAdapterMethod Method { get; }

        public HttpAdapterMethodAttribute(HttpAdapterMethod method = HttpAdapterMethod.Get)
        {
            Method = method;
        }
    }
}
