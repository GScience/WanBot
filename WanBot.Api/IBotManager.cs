using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

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
        MiraiEventHandler Subscript<T>(int priority, Func<MiraiBot, T, Task> func)
            where T : MiraiEventArgs;

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="priority"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        MiraiEventHandler Subscript(Type type, int priority, Func<MiraiBot, MiraiEventArgs, Task> func);
    }
}
