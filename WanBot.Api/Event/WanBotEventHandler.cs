using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public class WanBotEventHandler
    {
        public int Priority { get; }

        public WanBotEventHandler(int priority, Func<MiraiBot, BlockableEventArgs, Task> handler)
        {
            Priority = priority;
            Handler = handler;
        }

        public Func<MiraiBot, BlockableEventArgs, Task> Handler { get; }
    }

    /// <summary>
    /// 泛形事件处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MiraiEventHandler<T> : WanBotEventHandler where T : BlockableEventArgs
    {
        public MiraiEventHandler(int priority, Func<MiraiBot, T, Task> handler)
            : base(priority, (s, e) => handler(s, (T)e)) { }
    }
}
