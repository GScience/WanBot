using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Agent.Server
{
    internal struct ChatAccountInfo
    {
        public global::Agent Agent { get; }
        public uint Id { get; }
    }
    internal class ChatSession
    {
        public ChatAccountInfo ChatAccount { get; }
        public string Session { get; internal set; }
        public IAsyncStreamReader<ToHostMessage>? RequestStream { get; internal set; }
        public IServerStreamWriter<ToClientMessage>? ResponseStream { get; internal set; }
        public ChatSession(ChatAccountInfo chatAccount, string session)
        {
            ChatAccount = chatAccount;
            Session = session;
        }
    }
    internal class ChatSessionManager
    {
        private Dictionary<uint, ChatSession> _agentSessionDict = new();
        private Dictionary<ChatAccountInfo, ChatSession> _agentAccountDict = new();
    }
}
