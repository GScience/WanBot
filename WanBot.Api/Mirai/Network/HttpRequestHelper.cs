using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Network
{
    /// <summary>
    /// Http请求处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class HttpRequestHelper<T>
    {
        private const string _boundary = "--WanBotBoundaryAaBbCcDd";

        private static PropertyInfo[]? _properties;

        private static string _apiName;

        /// <summary>
        /// 发送请求
        /// </summary>
        public static Func<HttpAdapter, T, Task<HttpResponseMessage>> SendHandleAsync { get; }

        static HttpRequestHelper()
        {
            // 获取Api信息
            var httpApiAttr = typeof(T).GetCustomAttribute<HttpApiAttribute>();
            if (httpApiAttr == null)
                throw new Exception($"{typeof(T)} is not a http api payload");
            else
                _apiName = httpApiAttr.Name;

            if (httpApiAttr.Method == HttpAdapterMethod.Get)
            {
                _properties = typeof(T).GetProperties();
                SendHandleAsync = GetAsync;
            }
            else if (httpApiAttr.Method == HttpAdapterMethod.PostJson)
            {
                SendHandleAsync = PostJsonAsync;
            }
            else if (httpApiAttr.Method == HttpAdapterMethod.PostMultipart)
            {
                _properties = typeof(T).GetProperties();
                SendHandleAsync = PostMultipartAsync;
            }
            else
                throw new NotImplementedException();
        }

        private static async Task<HttpResponseMessage> GetAsync(HttpAdapter adapter, T payload)
        {
            var args = "";

            foreach (var property in _properties!)
            {
                var name = property.Name;
                name = name[..1].ToLower() + name[1..];
                var value = property.GetValue(payload);
                if (!string.IsNullOrEmpty(args))
                    args += "&";
                args += $"{name}={value}";
            }

            string url;
            if (string.IsNullOrEmpty(args))
                url = $"{_apiName}";
            else
                url = $"{_apiName}?{args}";
            return await adapter.GetAsync(url);
        }

        private static async Task<HttpResponseMessage> PostJsonAsync(HttpAdapter adapter, T payload)
        {
            using var content = JsonContent.Create(payload, payload!.GetType(), options: MiraiJsonContext.Default.Options);
            return await adapter.PostAsync(_apiName, content);
        }

        private static async Task<HttpResponseMessage> PostMultipartAsync(HttpAdapter adapter, T payload)
        {
            var content = new MultipartFormDataContent(_boundary);
            content.Headers.ContentType = new MediaTypeHeaderValue($"multipart/form-data");
            content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", _boundary));

            foreach (var property in _properties!)
            {
                var name = property.Name;
                name = name[..1].ToLower() + name[1..];
                var value = property.GetValue(payload);

                if (value is Stream stream)
                {
                    var streamContent = new StreamContent(stream);
                    content.Add(streamContent, name, "img.png");
                }
                else
                {
                    var stringContent = new StringContent(value?.ToString()!);
                    content.Add(stringContent, name);
                }
            }
            content.Add(new StringContent(""), "__end");
            return await adapter.PostAsync(_apiName, content);
        }
    }
}
