using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Hook;
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

        /// <summary>
        /// Bot Logger
        /// </summary>
        public ILogger BotLogger => _logger;

        /// <summary>
        /// 机器人QQ Id
        /// </summary>
        public long Id { get; private set; }

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
            var httpAdapter = new HttpAdapter(_logger, $"http://{url}", config.VerifyKey, config.QQ);
            _adapterDict[typeof(HttpAdapter)] = httpAdapter;
            await httpAdapter.ConnectAsync();

            _logger.Info("Init Websocket adapter");
            var wsAdapter = new WebSocketAdapter(_logger, $"ws://{url}", config.VerifyKey, config.QQ);
            _adapterDict[typeof(WebSocketAdapter)] = wsAdapter;
            await wsAdapter.ConnectAsync();

            // 监听事件同步Id的消息
            wsAdapter.SetSyncListener(config.EventSyncId, OnWSMessage);
            IsConnected = true;

            // 设置QQ Id
            Id = config.QQ;
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public WanBotEventHandler Subscripe<T>(MiraiEventHandler<T> eventHandler) 
            where T : BlockableEventArgs
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
        public async Task PublishAsync(Type type, BlockableEventArgs eventArgs)
        {
            await PublishAsync(type.Name, eventArgs);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        public async Task PublishAsync(string eventType, BlockableEventArgs eventArgs)
        {
            // Hook事件
            eventArgs = eventArgs.Hook(this, HookType.Event);

            if (!_eventDict.TryGetValue(eventType, out var e))
                return;
            
            await e.InvokeAsync(this, eventArgs);
        }

        /// <summary>
        /// 清空所有事件监听
        /// </summary>
        public void ClearEventHandle()
        {
            _eventDict.Clear();
        }

        private void OnWSMessage(string msg)
        {
            var wsResponse = JsonSerializer.Deserialize<WsAdapterResponse<BaseMiraiEvent>>(msg, MiraiJsonContext.Default.Options);
            var baseEvent = wsResponse!.Data;
            var eventType = baseEvent!.GetType();
            var task = PublishAsync(eventType, baseEvent);

            task.GetAwaiter().OnCompleted(() =>
            {
                // 没有异常直接返回
                if (task.Exception == null) return;

                // 尝试调用Exception hook
                // 要保证无论发生什么都能捕获到
                // 不然会漏捕异常
                // 移除Session key
                Exception e = new MiraiBotEventException(
                    this,
                    msg, 
                    "Exception when publish a mirai event",
                    baseEvent,
                    task.Exception);
                Exception? hookedException = null;

                // 如果有异常，则调用异常hook
                if (e != null)
                {
                    try
                    {
                        // Hook后的异常
                        hookedException = e.Hook(this, HookType.Exception);
                    }
                    catch (Exception e2)
                    {
                        _logger.Error(
                            "Error while running exception hook because of:\n {ex}",
                            e2);
                    }
                }

                // 不管异常hook怎么处理，最后都要输出日志
                if (e != null)
                {
                    if (hookedException != null && hookedException != e)
                        // 如果hooked后有了新的异常返回则再输出一个新的异常
                        _logger.Error(
                            "Error while deal with event {EventType} because of:\n {ex}\nPayload: \n{msg}\nMaybe: {ex2}",
                            eventType,
                            e,
                            msg,
                            hookedException);
                    else
                        // 直接输出旧的异常
                        _logger.Error(
                            "Error while deal with event {EventType} because of:\n {ex}\nPayload: \n{msg}",
                            eventType,
                            e,
                            msg);

                }
            });
        }

        public void Dispose()
        {
            _logger.Info("Disconnecting");
            foreach (var adapter in _adapterDict.Values)
                adapter.Dispose();
            _logger.Info("Disconnected");
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
