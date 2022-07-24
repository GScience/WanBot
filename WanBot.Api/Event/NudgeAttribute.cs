using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NudgeAttribute : EventAttribute
    {
        public int Priority { get; }

        public NudgeAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}
