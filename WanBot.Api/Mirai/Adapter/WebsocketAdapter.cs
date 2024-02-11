﻿using System;
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
        private readonly ILogger _logger;

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

        public WebSocketAdapter(ILogger logger, string baseUrl, string verifyKey, long qq)
        {
            _logger = logger;
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
            try
            {
                var syncId = GetJsonSyncId(json);

                if (_syncListener.TryGetValue(syncId, out var listener))
                    listener(json);
            }
            catch (Exception e)
            {
                _logger.Error("Error while deal with websocket message {ex}\nPayload: {payload}", e, json);
            }
        }

        /// <summary>
        /// 当接收到ws关闭
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        private void OnWebSocketClose(SimpleWebSocketClient socket, WebSocketCloseStatus? status, string? desc)
        {
            _logger.Warn("Socket closed because {reason}({status})", desc, status);

            var connected = false;

            while (!connected && IsConnected)
            {
                try
                {
                    _logger.Info("Try to reconnect to mirai");
                    ConnectAsync().Wait();
                    connected = true;
                    _logger.Info("Connected");
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to reconnect to mirai because: \n{e}", e);
                    _logger.Info("Retry after 5 secons...");
                    Thread.Sleep(5000);

                    try
                    {
                        socket.Close(null);
                    }
                    catch (Exception)
                    {
                    }
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

        public async Task<ResponsePayload?> SendAsync<ResponsePayload, RequestPayload>(RequestPayload? request)
            where RequestPayload : IRequest
            where ResponsePayload : IResponse
        {
            if (request == null)
                return default;

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
