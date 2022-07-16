using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        MiraiEventHandler Subscript<T>(int priority, Func<MiraiEventArgs<T>, Task> func)
            where T : BaseEvent;
    }
}
