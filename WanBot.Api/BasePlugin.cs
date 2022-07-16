using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api
{
    /// <summary>
    /// 插件基类，建议使用BaseWanBotPlugin
    /// </summary>
    public abstract class BasePlugin
    {
        protected BasePlugin() { }

        /// <summary>
        /// 应用程序
        /// </summary>
        public IApplication Application { get; private set; } = null!;

        public ILogger Logger { get; private set; } = null!;

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="priority"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public MiraiEventHandler Subscripe<T>(int priority, Func<MiraiBot, T, Task> func)
            where T : MiraiEventArgs
            => Application.BotManager.Subscript<T>(priority, func);

        /// <summary>
        /// 创建插件实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static BasePlugin CreatePluginInstance(Type type, IApplication application, ILogger logger)
        {
            var plugin = (BasePlugin)Activator.CreateInstance(type)!;
            plugin.Application = application;
            plugin.Logger = logger;
            return plugin;
        }

        /// <summary>
        /// 插件预初始化，此阶段不保证其他插件已经加载
        /// </summary>
        public virtual void PreInit()
        {
        }

        /// <summary>
        /// 插件初始化，此阶段不保证所有Bot均已连接
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// 插件初始化，此阶段可进行事件注册
        /// </summary>
        public virtual void PostInit()
        {
        }
    }
}
