using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot.Api
{
    public interface IBotManager
    {
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="priority"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        WanBotEventHandler Subscript<T>(int priority, Func<MiraiBot, T, Task> func)
            where T : CancellableEventArgs;

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="priority"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        WanBotEventHandler Subscript(Type type, int priority, Func<MiraiBot, CancellableEventArgs, Task> func);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="priority"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        WanBotEventHandler Subscript(string eventName, int priority, Func<MiraiBot, CancellableEventArgs, Task> func);
    }
}
