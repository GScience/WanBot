using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;

namespace WanBot.Api
{
    public interface IWanBot
    {
        public Version Version { get; }
        public IMessageBuilder MessageBuilder { get; }
    }
}
