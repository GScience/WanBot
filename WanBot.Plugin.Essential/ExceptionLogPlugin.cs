using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Hook;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;
using WanBot.Graphic.UI.Layout;
using WanBot.Graphic.UI;
using WanBot.Plugin.Essential.Graphic;
using WanBot.Api.Mirai.Event;
using WanBot.Plugin.Essential.Permission;
using WanBot.Api.Event;
using WanBot.Graphic;
using WanBot.Plugin.Essential.Extension;

namespace WanBot.Plugin.Essential
{
    /// <summary>
    /// 异常日志插件
    /// </summary>
    internal class ExceptionLogPlugin : WanBotPlugin
    {
        public override string PluginName => "ExceptionLog";

        public override string PluginDescription => "将错误日志发送到群内";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        public override void Start()
        {
            HookTable.Instance.ExceptionHook = SendExceptionAsImageAsync;
        }

        [Command("来个报错")]
        public async Task OnGenExceptionCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!args.Sender.HasCommandPermission(this, "genException"))
                return;
            await Task.CompletedTask;
            throw new Exception("你有病病");
        }

        private async Task<Exception?> SendExceptionAsImageAsync(MiraiBot bot, Exception e)
        {
            if (e is not MiraiBotEventException miraiException) return e;

            if (GraphicPlugin.GlobalRenderer == null)
                throw new Exception("GraphicPlugin.GlobalRenderer 是null！");

            var showErrorPermission = Permission.Permission.GetPluginPermission(this, "ShowError");

            if (miraiException.Event is GroupMessage groupMsg)
            {
                if (Permission.Permission.config.Admin.Contains(groupMsg.Sender.Id) ||
                    Permission.Permission.CheckGroup(groupMsg.Sender.Group.Id, showErrorPermission))
                {
                    using var grid = GenExceptionUI(e);
                    using var image = GraphicPlugin.GlobalRenderer.Draw(grid);
                    var msgBuilder = new MessageBuilder().Image(image);
                    await bot.SendGroupMessageAsync(groupMsg.Sender.Group.Id, null, msgBuilder);
                }
            }
            else if (miraiException.Event is TempMessage tempMsg)
            {
                if (Permission.Permission.config.Admin.Contains(tempMsg.Sender.Id) || 
                    Permission.Permission.CheckGroup(tempMsg.Sender.Group.Id, showErrorPermission))
                {
                    using var grid = GenExceptionUI(e);
                    using var image = GraphicPlugin.GlobalRenderer.Draw(grid);
                    var msgBuilder = new MessageBuilder().Image(image);
                    await bot.SendTempMessageAsync(tempMsg.Sender.Id, tempMsg.Sender.Group.Id, null, msgBuilder);
                }
            }
            else if (miraiException.Event is FriendMessage friendMsg)
            {
                using var grid = GenExceptionUI(e);
                using var image = GraphicPlugin.GlobalRenderer.Draw(grid);
                var msgBuilder = new MessageBuilder().Image(image);
                await bot.SendFriendMessageAsync(friendMsg.Sender.Id, null, msgBuilder);
            }

            return e;
        }

        private Grid GenExceptionUI(Exception e)
        {
            var grid = new Grid();

            var content = new VerticalLayout();

            var titleText = new TextBox();
            titleText.Text = $"  :(";
            titleText.Width = 800;
            titleText.Margin = new Margin(20, 20);
            titleText.FontPaint.TextSize = 84;
            titleText.FontPaint.Color = SKColors.White;
            titleText.FontPaint.TextAlign = SKTextAlign.Left;
            content.Children.Add(titleText);

            var sorryText = new TextBox();
            sorryText.Text = $"Sorry. But WanBot report an error....\n{e.Message}";
            sorryText.FontPaint.TextSize = 28;
            sorryText.FontPaint.Color = SKColors.White;
            sorryText.FontPaint.TextAlign = SKTextAlign.Center;
            content.Children.Add(sorryText);

            var msgText = new TextBox();
            msgText.Text = e.ToString();
            msgText.Width = 800;
            msgText.Margin = new Margin(0, 0);
            msgText.FontPaint.TextSize = 16;
            msgText.FontPaint.Color = SKColors.White;
            msgText.FontPaint.TextAlign = SKTextAlign.Left;
            content.Children.Add(msgText);

            var groupText = new TextBox();
            groupText.Text = $"Add to group for more information";
            groupText.FontPaint.TextSize = 21;
            groupText.FontPaint.Color = SKColors.White;
            groupText.FontPaint.TextAlign = SKTextAlign.Center;
            content.Children.Add(groupText);

            var bg = new Rectangle();
            bg.Margin = new Margin(0, 0, 0, 0);
            bg.Paint.Color = SKColors.Blue;
            
            grid.Children.Add(bg);
            grid.Children.Add(content);

            return grid;
        }
    }
}
