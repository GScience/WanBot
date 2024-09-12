using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot;
using WanBot.Agent.gRPC;
using WanBot.Agent.Server;

namespace WanBot
{
    internal class WanBot : ChatHost
    {
        static void Main(string[] args)
        {
            new WanBot().Run(args);
        }
    }
}