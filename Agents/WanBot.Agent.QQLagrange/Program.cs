using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Claims;
using WanBot.Agent;
using WanBot.Agent.gRPC;


var channel = GrpcChannel.ForAddress("http://localhost:5000");
var chatClient = new ChatAgentClient(channel);

var version = chatClient.GetVersion(new GetVersionRequest());
ChatMessage msg = new();
msg.Chain.Add([
    new MessageBlock { Text = new MessageBlock.Types.TextMessageBlock { Text = "Hello World" } },
    new MessageBlock { Mention = new MessageBlock.Types.MentionMessageBlock { Nickname = "123" } },
    new MessageBlock { Advance = new MessageBlock.Types.AdvanceMessageBlock { AdvanceMessage = "Message1111111111111111111111111" } },
]);

int count = 0;
var time = DateTime.Now;
var token = "DEFAULT_TOKEN";
var sessionId = await chatClient.LoginAsync(Agent.Qq, 12345, token);

var metadata = new Metadata();
metadata.Add("Authorization", $"Bearer {sessionId.AccessToken}");
var s = chatClient.CreateChatStream(metadata);
while (true)
{
    var now = DateTime.Now;
    if (now - time > TimeSpan.FromSeconds(1))
    {
        Console.WriteLine($"{count / (now - time).TotalSeconds}/s");
        count = 0;
        time = now;
    }
    ++count;
    try
    {
        s.RequestStream.WriteAsync(new ToHostMessage { ChatMessage = msg }).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
return 0;