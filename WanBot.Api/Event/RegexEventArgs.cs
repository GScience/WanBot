using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    public class RegexEventArgs : BlockableEventArgs
    {
        public ISender Sender { get; }

        public MessageChain Chain { get; }

        public Regex Regex { get; }

        public RegexEventArgs(ISender sender, MessageChain chain, Regex regex)
        {
            Sender = sender;
            Regex = regex;
            Chain = chain;
        }

        public string GetEventName()
        {
            return GetEventName(Regex);
        }

        public static string GetEventName(Regex regex)
        {
            return $"{typeof(RegexEventArgs).Name}.{regex}";
        }
    }
}
