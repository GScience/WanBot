using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    public class NudgeEventArgs : BlockableEventArgs
    {
        public ISender Sender { get; }

        public NudgeEventArgs(ISender sender)
        {
            Sender = sender;
        }
    }
}
