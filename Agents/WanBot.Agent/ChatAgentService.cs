using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Agent
{
    public class ChatAgentService : WanBotAgent.WanBotAgentBase
    {
        public override Task<GetVersionReply> GetVersion(GetVersionRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetVersionReply
            {
                Version = "1.0"
            });
        }

        public override Task<PostChatMessageReply> PostChatMessage(PostChatMessageRequest request, ServerCallContext context)
        {
            foreach (var msg in request.ChatMessage.Chain)
            {
                switch (msg.ResultCase)
                {
                    case MessageBlock.ResultOneofCase.Text:
                        Console.WriteLine(msg.Text.Text);
                        break;
                    case MessageBlock.ResultOneofCase.Mention:
                        Console.WriteLine(msg.Mention.Nickname);
                        break;
                }
            }
            return Task.FromResult(new PostChatMessageReply
            {
            });
        }

        public void Run(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddGrpc();
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
            var app = builder.Build();
            app.MapGrpcService<ChatAgentService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.Run();
        }
    }
}
