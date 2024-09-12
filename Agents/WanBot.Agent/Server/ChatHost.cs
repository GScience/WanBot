using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WanBot.Agent.gRPC;

namespace WanBot.Agent.Server
{
    public abstract class ChatHost : IChatAgentServiceImpl
    {
        public ChatHost()
        {
            
        }

        public async Task CreateChatStream(IAsyncStreamReader<ToHostMessage> requestStream, IServerStreamWriter<ToClientMessage> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"Created! by {context.GetHttpContext().User}");
            await foreach (var item in requestStream.ReadAllAsync())
            {
            }
            await Task.Delay(-1);
        }

        public void Run(string[] args)
        {
            ChatAgentService.Run(args, this);
        }
    }
}
