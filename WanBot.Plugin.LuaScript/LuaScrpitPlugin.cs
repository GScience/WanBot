using Neo.IronLua;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Permission;
using WanBot.Plugin.Essential.Extension;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Message;

namespace WanBot.Plugin.LuaScript
{
    public class LuaScrpitPlugin : WanBotPlugin, IDisposable
    {
        public override string PluginName => "LuaScrpit";

        public override string PluginDescription => "可运行lua脚本";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private LuaEnv _luaEnv = new();

        public override void PreInit()
        {
            base.PreInit();
        }

        public void Dispose()
        {
            _luaEnv.Dispose();
        }
        public override void Start()
        {
            this.GetBotHelp()
                .Category("运行Lua")
                .Command("#lua <脚本>", "执行黑暗魔法")
                .Info("别企图炸服...");

            base.Start();
        }

        [Command("lua")]
        public async Task OnLua(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "lua"))
                return;

            args.Blocked = true;
            var firstChain = args.GetRemain()?.FirstOrDefault();
            if (firstChain == null || firstChain is not Plain plain)
            {
                await args.Sender.ReplyAsync("请输入脚本");
                return;
            }

            var resultObj = await _luaEnv.RunAsync(plain.Text);
            if (resultObj is string resultStr)
            {
                await args.Sender.ReplyAsync(GetLimitedString(resultStr, 10, 200));
            }
            else if (resultObj is MessageBuilder resultMessageBuilder)
            {
                BaseChain?[] messageChain;
                if (args.Sender is GroupSender)
                    messageChain = resultMessageBuilder.Build(bot, MessageType.Group, false).Take(3).ToArray();
                else if (args.Sender is FriendSender)
                    messageChain = resultMessageBuilder.Build(bot, MessageType.Friend, false).Take(3).ToArray();
                else if (args.Sender is StrangerSender)
                    messageChain = resultMessageBuilder.Build(bot, MessageType.Temp, false).Take(3).ToArray();
                else
                    return;

                // 只允许纯文本和At
                for (var i = 0; i < messageChain.Length; i++)
                {
                    var chain = messageChain[i];
                    if (chain is Plain plainChain)
                        plainChain.Text = GetLimitedString(plainChain.Text, 2, 20);
                    else if (chain is At atChain)
                    {
                    }
                    else
                        messageChain[i] = null;
                }
                messageChain = messageChain.Where(x => x != null).ToArray();
                await args.Sender.ReplyAsync(new MessageChain(messageChain!));
            }
        }

        private string GetLimitedString(string str, int lineCount = 10, int length = 200)
        {
            if (str == null || str == "")
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
            if (str.Length > endIndex)
                str = str[..endIndex] + "...";

            return str;
        }
    }
}