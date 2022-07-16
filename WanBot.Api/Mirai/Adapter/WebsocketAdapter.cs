using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Network;
using WanBot.Api.Mirai.Payload;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai.Adapter
{
    /// <summary>
    /// WS适配器
    /// </summary>
    public class WebSocketAdapter : IAdapter
    {
        public delegate void SessionKeyChangedEvent(string sesionKey);

        /// <summary>
        /// 会话key变更事件
        /// </summary>
        public event SessionKeyChangedEvent? OnSessionKeyChanged;

        public string BaseUrl { get; protected set; }
        public string VerifyKey { get; protected set; }
        public long QQ { get; protected set; }
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 同步消息监听
        /// </summary>
        private ConcurrentDictionary<string, Action<string>> _syncListener = new();

        /// <summary>
        /// Ws client
        /// </summary>
        private SimpleWebSocketClient _wsClient = new();

        /// <summary>
        /// 会话Id
        /// </summary>
        public string SessionId { get; private set; } = string.Empty;

        public WebSocketAdapter(string baseUrl, string verifyKey, long qq)
        {
            BaseUrl = baseUrl;
            VerifyKey = verifyKey;
            QQ = qq;

            _wsClient.OnMessage += OnWebSocketRecv;
            _wsClient.OnClose += OnWebSocketClose;
        }

        private static string GetJsonSyncId(string json)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            reader.Read();
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString()!;
            else if (reader.TokenType == JsonTokenType.Number)
                return reader.GetUInt32().ToString();

            throw new JsonException();
        }

        /// <summary>
        /// 当接收到ws消息
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="json"></param>
        private void OnWebSocketRecv(SimpleWebSocketClient socket, string json)
        {
            var syncId = GetJsonSyncId(json);

            if (_syncListener.TryGetValue(syncId, out var listener))
                listener(json);
        }

        /// <summary>
        /// 当接收到ws关闭
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        private void OnWebSocketClose(SimpleWebSocketClient socket, WebSocketCloseStatus? status, string? desc)
        {
            var connected = false;

            while (!connected && IsConnected)
            {
                try
                {
                    ConnectAsync().Wait();
                    connected = true;
                }
                catch (Exception)
                {
                    
                }
            }
        }

        /// <summary>
        /// 等待响应
        /// </summary>
        /// <typeparam name="ResponsePayload"></typeparam>
        /// <param name="syncId"></param>
        /// <returns></returns>
        private WsAdapterResponse<ResponsePayload> WaitForResponse<ResponsePayload>(string syncId)
        {
            var signal = new Semaphore(0, 1);
            var responseJson = "";
            _syncListener[syncId] = (json) =>
            {
                responseJson = json;
                _syncListener.TryRemove(syncId, out _);
                signal.Release();
            }; 
            signal.WaitOne();

            var response = JsonSerializer.Deserialize<WsAdapterResponse<ResponsePayload>>(responseJson, MiraiJsonContext.Default.Options)!;

            return response;
        }

        /// <summary>
        /// 设置同步监听器
        /// </summary>
        /// <param name="syncId"></param>
        /// <param name="func"></param>
        public void SetSyncListener(string syncId, Action<string> func)
        {
            _syncListener[syncId] = func;
        }

        /// <summary>
        /// 连接到mirai
        /// </summary>
        public async Task ConnectAsync()
        {
            await _wsClient.ConnectAsync($"{BaseUrl}/all?verifyKey={VerifyKey}&qq={QQ}");
            var verifyResponse = WaitForResponse<VerifyResponse>("");

            if (verifyResponse.Data!.Code != ResponseCode.Ok)
                throw new Exception(ResponseCode.Reason(verifyResponse.Data!.Code));

            var sessionId = verifyResponse!.Data!.Session;
            SessionId = sessionId;
            OnSessionKeyChanged?.Invoke(sessionId);
            IsConnected = true;
        }

        public async Task<ResponsePayload?> SendAsync<ResponsePayload, RequestPayload>(RequestPayload request)
            where RequestPayload : class
            where ResponsePayload : IResponse
        {
            // 等待连接
            var syncId = SyncIdHelper.Next();

            var requestJson = JsonSerializer.Serialize(new WsAdapterRequest<RequestPayload>(syncId, request), MiraiJsonContext.Default.Options);
            await _wsClient.SendAsync(requestJson);

            // 获取响应
            if (typeof(Response).IsAssignableFrom(typeof(ResponsePayload)))
            {
                var response = WaitForResponse<ResponsePayload>(syncId) as WsAdapterResponse<Response<ResponsePayload>>;
                if (response!.Data!.Code != ResponseCode.Ok)
                    throw new Exception($"{ResponseCode.Reason(response.Data!.Code)}. \nRequest payload: {requestJson}");
                return response.Data.Data;
            }
            else
            {
                var response = WaitForResponse<ResponsePayload>(syncId);
                return response.Data;
            }
        }

        public void Dispose()
        {
            IsConnected = false;
            _wsClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Api名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WsApiAttribute : Attribute
    {
        public string Command { get; }
        public string SubCommand { get; }

        public WsApiAttribute(string command, string subCommand = "")
        {
            Command = command;
            SubCommand = subCommand;
        }
    }
}
