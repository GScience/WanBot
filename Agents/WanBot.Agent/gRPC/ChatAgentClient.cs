using Grpc.Core;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Channels;
using System.Web;

namespace WanBot.Agent.gRPC
{
    public sealed class ChatAgentClient : WanBotAgent.WanBotAgentClient
    {
        public ChatAgentClient(ChannelBase channel) : base(channel) { }

        public async Task<AccessTokenResponse> LoginAsync(global::Agent agentType, uint agentId, string token)
        {

            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"http://localhost:5000/login?agent={agentType}&id={agentId}&token={token}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0),
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
            };
            var tokenResponse = await httpClient.SendAsync(request);
            tokenResponse.EnsureSuccessStatusCode();

            return await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>() ?? throw new Exception();
        }
    }
}
