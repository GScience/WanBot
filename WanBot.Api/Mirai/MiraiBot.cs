using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Adapter;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Network;
using WanBot.Api.Util;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// Mirai 机器人
    /// </summary>
    public partial class MiraiBot : IDisposable
    {
        private Dictionary<Type, MiraiEvent> _eventDict = new();
        internal ILogger _logger;

        private Dictionary<Type, IAdapter> _adapterDict = new();

        public bool IsConnected { get; private set; } = false;

        internal string SessionKey
        {
            get
            {
                return ((WebSocketAdapter)_adapterDict[typeof(WebSocketAdapter)]).SessionId;
            }
        }

        public MiraiBot(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 连接到Mirai
        /// </summary>
        /// <param name="config"></param>
        public async Task ConnectAsync(MiraiConfig config)
        {
            if (IsConnected)
                throw new Exception("Mirai bot already connect to mirai");

            var url = $"{config.Host}:{config.Port}";

            _logger.Info("Init Http adapter");
            var httpAdapter = new HttpAdapter($"http://{url}", config.VerifyKey, config.QQ);
            _adapterDict[typeof(HttpAdapter)] = httpAdapter;
            await httpAdapter.ConnectAsync();

            _logger.Info("Init Websocket adapter");
            var wsAdapter = new WebSocketAdapter($"ws://{url}", config.VerifyKey, config.QQ);
            _adapterDict[typeof(WebSocketAdapter)] = wsAdapter;
            await wsAdapter.ConnectAsync();

            // 监听事件同步Id的消息
            wsAdapter.SetSyncListener(config.EventSyncId, OnWSMessage);
            IsConnected = true;
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="priority"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public MiraiEventHandler Subscripe<T>(int priority, Func<MiraiEventArgs<T>, Task> action) 
            where T:BaseEvent
        {
            var eventHandler = new MiraiEventHandler<T>(priority, action);

            if (_eventDict.TryGetValue(typeof(T), out var e))
                e.Add(eventHandler);
            else
            {
                e = new();
                e.Add(eventHandler);
                _eventDict[typeof(T)] = e;
            }
            return eventHandler;
        }

        private void OnWSMessage(string msg)
        {
            var wsResponse = JsonSerializer.Deserialize<WsAdapterResponse<BaseEvent>>(msg, MiraiJsonContext.Default.Options);
            var baseEvent = wsResponse!.Data;
            if (_eventDict.TryGetValue(baseEvent!.GetType(), out var e))
            {
                try
                {
                    e.InvokeAsync(new MiraiEventArgs(baseEvent)).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error("Error while deal with event. {ex}\nPayload: \n{msg}", ex, msg);
                }
            }
        }

        public void Dispose()
        {
            foreach (var adapter in _adapterDict.Values)
                adapter.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 获取适配器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetAdapter<T>() where T : class, IAdapter
        {
            if (_adapterDict.TryGetValue(typeof(T), out var adapter))
                return adapter as T;
            return null;
        }
    }
}
