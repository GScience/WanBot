using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 事件处理器
    /// </summary>
    public class MiraiEventHandler
    {
        public int Priority { get; }

        public MiraiEventHandler(int priority, Func<MiraiBot, CancellableEventArgs, Task> handler)
        {
            Priority = priority;
            Handler = handler;
        }

        public Func<MiraiBot, CancellableEventArgs, Task> Handler { get; }
    }

    /// <summary>
    /// 泛形事件处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MiraiEventHandler<T> : MiraiEventHandler where T : CancellableEventArgs
    {
        public MiraiEventHandler(int priority, Func<MiraiBot, T, Task> handler)
            : base(priority, (s, e) => handler(s, (T)e)) { }
    }
}
