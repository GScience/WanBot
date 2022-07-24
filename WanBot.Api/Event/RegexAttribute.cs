using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegexAttribute : EventAttribute
    {
        public Regex Regex { get; }

        public int Priority { get; }

        public RegexAttribute(string regex, int priority = 0)
        {
            Regex = new Regex(regex, RegexOptions.Compiled);
            Priority = priority;
        }
    }
}
