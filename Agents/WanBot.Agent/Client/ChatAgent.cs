using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WanBot.Agent.gRPC;

namespace WanBot.Agent.Client
{
    public enum AgentState
    {
        Stopped,
        Running,
    }

    public abstract class ChatAgentBase
    {

    }
}
