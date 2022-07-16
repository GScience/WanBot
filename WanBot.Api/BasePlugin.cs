using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api
{
    /// <summary>
    /// 插件
    /// </summary>
    public abstract class BasePlugin
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract MiraiEventHandler Subscripe<T>();
    }
}
