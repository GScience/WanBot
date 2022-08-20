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
        public string[] Commands { get; }

        public int Priority { get; }

        public CommandAttribute(string command, int priority = 0)
        {
            Commands = new[] { command };
            Priority = priority;
        }

        public CommandAttribute(IEnumerable<string> commands, int priority = 0)
        {
            Commands = commands.ToArray();
            Priority = priority;
        }

        public CommandAttribute(params string[] commands)
        {
            Commands = commands;
            Priority = 0;
        }
    }
}
