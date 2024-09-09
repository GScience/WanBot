using Grpc.Core;

namespace WanBot.Agent
{
    public class ChatAgentClient : WanBotAgent.WanBotAgentClient
    {
        public ChatAgentClient(ChannelBase channel) : base(channel) { }
    }
}
