using Grpc.Net.Client;
using WanBot.Agent;

var channel = GrpcChannel.ForAddress("http://localhost:5000");
var chatClient = new ChatAgentClient(channel);
var version = chatClient.GetVersion(new GetVersionRequest());
var post = new PostChatMessageRequest
{
    ChatMessage = new ChatMessage
    {
    }
};
post.ChatMessage.Chain.Add([
    new MessageBlock { Text = new MessageBlock.Types.TextMessageBlock { Text = "Hello World" } },
    new MessageBlock { Mention = new MessageBlock.Types.MentionMessageBlock { Nickname = "123" } },
    new MessageBlock { Advance = new MessageBlock.Types.AdvanceMessageBlock { AdvanceMessage = "Message1111111111111111111111111" } },
]);
int count = 0;
var time = DateTime.Now;
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
        chatClient.PostChatMessage(post);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
return 0;