using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    public class AtEventArgs : BlockableEventArgs
    {
        public ISender Sender { get; }

        public MessageChain Chain { get; }

        public AtEventArgs(ISender sender, MessageChain chain)
        {
            Sender = sender;
            Chain = chain;
        }
    }
}
