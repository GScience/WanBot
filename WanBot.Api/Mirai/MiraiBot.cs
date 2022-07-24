using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Event;
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
        private ConcurrentDictionary<string, WanBotEventEvent> _eventDict = new();
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
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public WanBotEventHandler Subscripe<T>(MiraiEventHandler<T> eventHandler) 
            where T : CancellableEventArgs
        {
            if (_eventDict.TryGetValue(typeof(T).Name, out var e))
                e.Add(eventHandler);
            else
            {
                e = new();
                e.Add(eventHandler);
                _eventDict[typeof(T).Name] = e;
            }
            return eventHandler;
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public WanBotEventHandler Subscripe(Type type, WanBotEventHandler eventHandler)
        {
            return Subscripe(type.Name, eventHandler);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public WanBotEventHandler Subscripe(string eventName, WanBotEventHandler eventHandler)
        {
            if (_eventDict.TryGetValue(eventName, out var e))
                e.Add(eventHandler);
            else
            {
                e = new();
                e.Add(eventHandler);
                _eventDict[eventName] = e;
            }
            return eventHandler;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        public async Task PublishAsync(Type type, CancellableEventArgs eventArgs)
        {
            await PublishAsync(type.Name, eventArgs);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        public async Task PublishAsync(string eventType, CancellableEventArgs eventArgs)
        {
            if (!_eventDict.TryGetValue(eventType, out var e))
                return;
            
            await e.InvokeAsync(this, eventArgs);
        }

        private void OnWSMessage(string msg)
        {
            var wsResponse = JsonSerializer.Deserialize<WsAdapterResponse<BaseMiraiEvent>>(msg, MiraiJsonContext.Default.Options);
            var baseEvent = wsResponse!.Data;
            var eventType = baseEvent!.GetType();
            var task = PublishAsync(eventType, baseEvent);

            task.GetAwaiter().OnCompleted(() =>
            {
                if (task.Exception != null)
                    _logger.Error(
                        "Error while deal with event {EventType} because of:\n {ex}\nPayload: \n{msg}",
                        eventType,
                        task.Exception,
                        msg);
            });
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
    public static class Priority
    {
        public const int BottomMost = int.MinValue;
        public const int Lowest     = -10000;
        public const int Lower      = -1000;
        public const int Low        = -100;
        public const int Default    = 0;
        public const int High       = 100;
        public const int Higher     = 1000;
        public const int Highest    = 10000;
        public const int TopMost    = int.MaxValue;
    }
}
