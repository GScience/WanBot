using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Plugin.AI
{
    internal interface IAIAdapter
    {
        public Task<string> ProcessAsync(string systemMessage, IEnumerable<GroupChat> chats);
    }
}
