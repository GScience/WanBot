using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MiraiEventAttribute<T> : MiraiEventAttribute where T : BaseEvent
    {
        public MiraiEventAttribute(int priority = 0) : base(priority, typeof(T)) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MiraiEventAttribute : EventAttribute
    {
        public int Priority { get; }

        public Type EventType { get; }

        public MiraiEventAttribute(int priority, Type type)
        {
            Priority = priority;
            EventType = type;
        }
    }
}
