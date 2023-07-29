using Neo.IronLua;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Permission;
using WanBot.Plugin.Essential.Extension;
using WanBot.Api.Mirai.Message;

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

            var result = await _luaEnv.RunAsync(plain.Text);
            if (result == null || result == "")
                result = "<Empty>";
            // 限制200长度
            // 限制10行
            var endIndex = result.Length;
            var lineIndex = 0;
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i] == '\n')
                    ++lineIndex;
                if (lineIndex == 10)
                {
                    endIndex = i;
                    break;
                }
                if (i == 200)
                    endIndex = i;
            }
            if (result.Length > endIndex)
                result = result[..endIndex] + "...";
            await args.Sender.ReplyAsync(result);
        }
    }
}