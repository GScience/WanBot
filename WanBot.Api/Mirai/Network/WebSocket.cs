using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Network
{
    /// <summary>
    /// WebSocket封装
    /// </summary>
    public class SimpleWebSocketClient : IDisposable
    {
        public delegate void CloseHandler(SimpleWebSocketClient sender, WebSocketCloseStatus? status, string? desc);
        public delegate void MessageHandler(SimpleWebSocketClient sender, string message);

        /// <summary>
        /// 当远程客户端断开连接
        /// </summary>
        public event CloseHandler? OnClose;

        /// <summary>
        /// 当接收到消息
        /// </summary>
        public event MessageHandler? OnMessage;

        /// <summary>
        /// 是否连接到远程服务器
        /// </summary>
        public bool IsConnected { get; private set; }

        public ClientWebSocket _wsClient = new();

        private CancellationTokenSource _cts = new();

        private byte[] _buffer = new byte[1024];

        private Task? _recvTask;

        /// <summary>
        /// 连接到Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string url)
        {
            _wsClient.Options.Cookies = new CookieContainer();
            await _wsClient.ConnectAsync(new Uri(url), CancellationToken.None);
            _recvTask = Task.Run(RecvThread);
            IsConnected = true;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="status"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task CloseAsync(WebSocketCloseStatus? status, string? desc = null)
        {
            _cts.Cancel();

            if (_wsClient.State == WebSocketState.Open)
                await _wsClient.CloseAsync(status ?? WebSocketCloseStatus.NormalClosure, desc, CancellationToken.None);

            if (_recvTask != null)
            {
                try
                {
                    _recvTask.Wait();
                }
                catch (AggregateException e)
                {
                    if (e.InnerException?.GetType() != typeof(TaskCanceledException))
                        throw e;
                }

                _recvTask.Dispose();
                _recvTask = null;
            }
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            IsConnected = false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SendAsync(string msg)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Websocket is not connected");

            try
            {
                await _wsClient.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception e)
            {
                HandleExceptedClose(null, e.ToString());
            }
        }

        /// <summary>
        /// 处理异常断开连接
        /// </summary>
        private void HandleExceptedClose(WebSocketCloseStatus? status, string? desc)
        {
            IsConnected = false;
            CloseAsync(status, desc).Wait();
            OnClose?.Invoke(this, status, desc);
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        private async Task RecvThread()
        {
            var strBuffer = "";

            while (!_cts.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await _wsClient.ReceiveAsync(_buffer, _cts.Token);
                }
                catch (WebSocketException e)
                {
                    HandleExceptedClose(_wsClient.CloseStatus, _wsClient.CloseStatusDescription);
                    break;
                }
                
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    HandleExceptedClose(result.CloseStatus, result.CloseStatusDescription);
                    break;
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                    throw new NotImplementedException();

                strBuffer += Encoding.UTF8.GetString(_buffer, 0, result.Count);

                if (result.EndOfMessage)
                {
                    OnMessage?.Invoke(this, strBuffer);
                    strBuffer = "";
                }
            }
        }

        public void Dispose()
        {
            CloseAsync(WebSocketCloseStatus.NormalClosure, "Disposed").Wait();

            _cts.Dispose();
            _wsClient.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
