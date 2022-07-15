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

        public MiraiEventHandler(int priority, Func<MiraiEventArgs, Task> handler)
        {
            Priority = priority;
            Handler = handler;
        }

        public Func<MiraiEventArgs, Task> Handler { get; }
    }

    /// <summary>
    /// 泛形事件处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MiraiEventHandler<T> : MiraiEventHandler where T : BaseEvent
    {
        public MiraiEventHandler(int priority, Func<MiraiEventArgs<T>, Task> handler)
            : base(priority, (e) => handler(new MiraiEventArgs<T>(e))) { }
    }
}
