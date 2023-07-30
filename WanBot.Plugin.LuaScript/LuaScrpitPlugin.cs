using Neo.IronLua;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Permission;
using WanBot.Plugin.Essential.Extension;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Message;
using WanBot.Graphic.Template;
using System.Text.RegularExpressions;

namespace WanBot.Plugin.LuaScript
{
    public class LuaScrpitPlugin : WanBotPlugin, IDisposable
    {
        public override string PluginName => "LuaScrpit";

        public override string PluginDescription => "可运行lua脚本";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private LuaEnv _luaEnv = new();
        private Regex _luaFileRegex = new Regex(@"^[a-zA-Z0-9]+$");

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
                .Command("#lua <脚本>", "执行黑暗魔法，可通过sys调用C#类型")
                .Command("#luas <脚本名称> <脚本>", "保存黑魔法，每个群最多只能保存3个")
                .Command("#luad <脚本名称>", "删除黑魔法")
                .Command("#lual", "获取黑魔法列表")
                .Command("#luar <脚本名称> <参数>", "加载黑魔法")
                .Info("别企图炸服...");

            base.Start();
        }

        [Command("luas")]
        public async Task OnLuaSave(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "lua"))
                return;
            if (args.Sender is not GroupSender groupSender)
                return;

            args.Blocked = true;
            var luaName = args.GetNextArgs<string>();
            if (!_luaFileRegex.Match(luaName).Success)
            {
                await args.Sender.ReplyAsync("请确保脚本名仅包含数字和字母");
                return;
            }

            var groupScriptPath = $"{GetConfigPath()}/Scripts/{groupSender.GroupId}";
            if (!Directory.Exists(groupScriptPath))
                Directory.CreateDirectory(groupScriptPath);

            if (Directory.GetFiles(groupScriptPath).Length >= 3)
            {
                await args.Sender.ReplyAsync("脚本数量超过上限，请删除不需要的脚本");
                return;
            }

            var firstChain = args.GetRemain()?.FirstOrDefault();
            if (firstChain == null || firstChain is not Plain plain)
            {
                await args.Sender.ReplyAsync("请输入脚本");
                return;
            }
            var luaCode = plain.Text;
            System.IO.File.WriteAllText($"{groupScriptPath}/{luaName}.lua", luaCode);
            await args.Sender.ReplyAsync("已保存");
        }

        [Command("luad")]
        public async Task OnLuaDelete(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "lua"))
                return; 
            if (args.Sender is not GroupSender groupSender)
                return;

            args.Blocked = true;
            var luaName = args.GetNextArgs<string>();
            if (!_luaFileRegex.Match(luaName).Success)
            {
                await args.Sender.ReplyAsync("请确保脚本名仅包含数字和字母");
                return;
            }
            var groupScriptPath = $"{GetConfigPath()}/Scripts/{groupSender.GroupId}";
            if (System.IO.File.Exists($"{groupScriptPath}/{luaName}.lua"))
            {
                System.IO.File.Delete($"{groupScriptPath}/{luaName}.lua");
                await args.Sender.ReplyAsync("已删除");
            }
            else await args.Sender.ReplyAsync("不存在");
        }

        [Command("lual")]
        public async Task OnLuaList(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "lua"))
                return;
            if (args.Sender is not GroupSender groupSender)
                return;

            args.Blocked = true;
            var groupScriptPath = $"{GetConfigPath()}/Scripts/{groupSender.GroupId}";
            if (!Directory.Exists(groupScriptPath))
                Directory.CreateDirectory(groupScriptPath);

            var files = Directory.GetFiles(groupScriptPath);
            var resultList = "脚本列表：";
            foreach (var fileFullPath in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(fileFullPath);
                resultList += $"\n{fileName}";
            }
            await args.Sender.ReplyAsync(resultList);
        }

        [Command("luar")]
        public async Task OnLuaRun(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "lua"))
                return;
            if (args.Sender is not GroupSender groupSender)
                return;

            args.Blocked = true;
            var luaName = args.GetNextArgs<string>();
            if (!_luaFileRegex.Match(luaName).Success)
            {
                await args.Sender.ReplyAsync("请确保脚本名仅包含数字和字母");
                return;
            }
            var groupScriptPath = $"{GetConfigPath()}/Scripts/{groupSender.GroupId}";
            if (!System.IO.File.Exists($"{groupScriptPath}/{luaName}.lua"))
            {
                await args.Sender.ReplyAsync("不存在");
                return;
            }
            var luaCode = System.IO.File.ReadAllText($"{groupScriptPath}/{luaName}.lua");
            var remainCmd = args.GetRemain()?.FirstOrDefault();
            if (remainCmd == null || remainCmd is not Plain plain)
                await RunLua(luaCode, bot, groupSender);
            else
                await RunLua(luaCode, bot, groupSender, plain.Text.Split(' '));
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
            var luaCode = plain.Text;
            await RunLua(luaCode, bot, args.Sender);
        }

        private async Task RunLua(string luaCode, MiraiBot bot, ISender sender, params object[] callArgs)
        {
            Logger.Info($"Run lua: \n{luaCode}");
            var resultObj = await _luaEnv.RunAsync(luaCode, TimeSpan.FromSeconds(0.5f), callArgs);
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
                    else if (chain is At atChain)
                    {
                    }
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
            return endIndex;
        }
    }
}