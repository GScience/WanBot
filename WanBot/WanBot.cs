using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot;
using WanBot.Agent;

namespace WanBot
{
    internal class WanBot
    {
        private ChatAgentService _chatAgentService = new();

        public void Run(string[] args)
        {
            _chatAgentService.Run(args);
        }
        static void Main(string[] args)
        {
            new WanBot().Run(args);
        }
    }
}