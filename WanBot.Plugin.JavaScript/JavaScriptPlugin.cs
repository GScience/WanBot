using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Plugin.Essential.Extension;
using WanBot.Plugin.Essential.Permission;

namespace WanBot.Plugin.JavaScript;

public class JavaScriptPlugin : WanBotPlugin, IDisposable
{
    public override string PluginName => "JavaScript";
    public override string PluginDescription => "可运行js脚本";
    public override string PluginAuthor => "WanNeng";
    public override Version PluginVersion => Version.Parse("1.0.0");

    private JsEnv _jsEnv = new();

    public void Dispose()
    {
        _jsEnv.Dispose();
    }

    public override void Start()
    {
        this.GetBotHelp()
            .Category("运行 JavaScript ")
            .Command("#js <脚本>", "执行黑暗魔法，上下文保存 1 小时")
            .Command("#jsf5", "重设 js 上下文")
            // .Command("#jss <脚本名称> <脚本>", "保存黑魔法，每个群最多只能保存3个")
            // .Command("#jsd <脚本名称>", "删除黑魔法")
            // .Command("#jsl", "获取黑魔法列表")
            // .Command("#jsr <脚本名称> <参数>", "加载黑魔法")
            .Info("别企图炸服...");

        base.Start();
    }

    [Command("jsf5")]
    public async Task OnJsReset(MiraiBot bot, CommandEventArgs args)
    {
        if (!args.Sender.HasCommandPermission(this, "js"))
            return;

        var sender = args.Sender;
        _jsEnv.Reset($"{sender.Id}");
        await sender.ReplyAsync($"已重设 js 上下文");
    }

    [Command("js")]
    public async Task OnJs(MiraiBot bot, CommandEventArgs args)
    {
        if (!args.Sender.HasCommandPermission(this, "js"))
            return;

        args.Blocked = true;
        var firstChain = args.GetRemain()?.FirstOrDefault();
        if (firstChain == null || firstChain is not Plain plain)
        {
            await args.Sender.ReplyAsync("请输入脚本");
            return;
        }
        var jsCode = plain.Text;
        await RunJs(jsCode, bot, args.Sender);
    }

    private async Task RunJs(string jsCode, MiraiBot bot, ISender sender, params object[] callArgs)
    {
        Logger.Info($"Run js: \n{jsCode}");
        var resultObj = await _jsEnv.RunAsync($"{sender.Id}", jsCode, TimeSpan.FromSeconds(0.5f), callArgs);
        if (resultObj is string resultStr)
        {
            var endPos = GetLimitedStringEndPos(resultStr, 10, 200);
            if (resultStr.Length == endPos)
                await sender.ReplyAsync(resultStr);
            else if (resultStr.Length <= endPos * 3)
                await sender.ReplyAsImageAsync(resultStr);
            else
                await sender.ReplyAsImageAsync(resultStr[..(endPos * 3)] + "...");
        }
        else if (resultObj is MessageBuilder resultMessageBuilder)
        {
            BaseChain?[] messageChain;
            if (sender is GroupSender)
                messageChain = resultMessageBuilder.Build(bot, MessageType.Group, false).Take(3).ToArray();
            else if (sender is FriendSender)
                messageChain = resultMessageBuilder.Build(bot, MessageType.Friend, false).Take(3).ToArray();
            else if (sender is StrangerSender)
                messageChain = resultMessageBuilder.Build(bot, MessageType.Temp, false).Take(3).ToArray();
            else
                return;

            // 只允许纯文本和At
            for (var i = 0; i < messageChain.Length; i++)
            {
                var chain = messageChain[i];
                if (chain is Plain plainChain)
                    plainChain.Text = GetLimitedString(plainChain.Text, 2, 20);
                else if (chain is At atChain) { }
                else
                    messageChain[i] = null;
            }
            messageChain = messageChain.Where(x => x != null).ToArray();
            try
            {
                await sender.ReplyAsync(new MessageChain(messageChain!));
            }
            catch (Exception e)
            {
                await sender.ReplyAsync($"消息发送失败，因为{e.Message}");
            }
        }
    }

    private string GetLimitedString(string str, int lineCount = 10, int length = 200)
    {
        var endPos = GetLimitedStringEndPos(str, lineCount, length);
        if (str.Length > endPos)
            str = str[..endPos] + "...";
        return str;
    }

    private int GetLimitedStringEndPos(string str, int lineCount = 10, int length = 200)
    {
        if (str == null! || str == "")
            str = "<Empty>";
        // 限制200长度
        // 限制10行
        var endIndex = str.Length;
        var lineIndex = 0;
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] == '\n')
                ++lineIndex;
            if (lineIndex == lineCount)
            {
                endIndex = i;
                break;
            }
            if (i == length)
            {
                endIndex = i;
                break;
            }
        }
        return endIndex;
    }
}
