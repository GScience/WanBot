using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WanBot.Agent.gRPC
{
    public class gRPCUser
    {

    }
    public interface IChatAgentServiceImpl
    {
        internal Task CreateChatStream(IAsyncStreamReader<ToHostMessage> requestStream, IServerStreamWriter<ToClientMessage> responseStream, ServerCallContext context);
    }
    internal sealed class ChatAgentService : WanBotAgent.WanBotAgentBase
    {
        private static IChatAgentServiceImpl _impl;
        public override Task<GetVersionReply> GetVersion(GetVersionRequest request, ServerCallContext context)
            => Task.FromResult(new GetVersionReply
            {
                Version = "1.0"
            });
        [Authorize]
        public override async Task CreateChatStream(IAsyncStreamReader<ToHostMessage> requestStream, IServerStreamWriter<ToClientMessage> responseStream, ServerCallContext context)
            => await _impl.CreateChatStream(requestStream, responseStream, context);
        public static void Run(string[] args, IChatAgentServiceImpl impl)
        {
            _impl = impl;
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication().AddBearerToken();
            builder.Services.AddAuthorization();
            builder.Services.AddGrpc();
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
            var app = builder.Build();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGrpcService<ChatAgentService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.MapGet("/login", async (HttpContext context, global::Agent agent, int id, string token) =>
            {
                var identity = new ClaimsIdentity(BearerTokenDefaults.AuthenticationScheme);
                var cp = new ClaimsPrincipal(identity);
                await context.SignInAsync(cp);
                Console.WriteLine($"[{agent}]{id} login!");
            });
            app.Run();
        }
    }
}
