using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : EventAttribute
    {
        public string Command { get; }

        public int Priority { get; }

        public CommandAttribute(string command, int priority = 0)
        {
            Command = command;
            Priority = priority;
        }
    }
}
