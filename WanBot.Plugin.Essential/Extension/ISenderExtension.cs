using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Message;
using WanBot.Graphic.UI.Layout;
using WanBot.Graphic.UI;
using WanBot.Plugin.Essential.Graphic;
using WanBot.Api.Mirai;
using SkiaSharp;

namespace WanBot.Plugin.Essential.Extension
{
    public static class ISenderExtension
    {
        /// <summary>
        /// 等待用户回答
        /// </summary>
        public static MessageChain? WaitForReply(this ISender sender, TimeSpan timeout)
        {
            using var autoResetEvent = new AutoResetEvent(false);

            MessageChain? messageChain = null;

            // 注册监听
            switch (sender)
            {
                case GroupSender groupSender:
                    ExtensionPlugin.Instance.waitGroupMessageHandler[groupSender] = (args) =>
                    {
                        messageChain = args.MessageChain;
                        autoResetEvent.Set();
                    };

                    break;

                case StrangerSender strangerSender:
                    ExtensionPlugin.Instance.waitTempMessageHandler[strangerSender] = (args) =>
                    {
                        messageChain = args.MessageChain;
                        autoResetEvent.Set();
                    };

                    break;

                case FriendSender friendSender:
                    ExtensionPlugin.Instance.waitFriendMessageHandler[friendSender] = (args) =>
                    {
                        messageChain = args.MessageChain;
                        autoResetEvent.Set();
                    };

                    break;
            }

            // 等待信号
            autoResetEvent.WaitOne(timeout);

            // 移除超时监听
            switch (sender)
            {
                case GroupSender groupSender:
                    ExtensionPlugin.Instance.waitGroupMessageHandler.TryRemove(groupSender, out var _);
                    break;

                case StrangerSender strangerSender:
                    ExtensionPlugin.Instance.waitTempMessageHandler.TryRemove(strangerSender, out var _);
                    break;

                case FriendSender friendSender:
                    ExtensionPlugin.Instance.waitFriendMessageHandler.TryRemove(friendSender, out var _);
                    break;
            }
            return messageChain;
        }

        public static async Task ReplyAsImageAsync(this ISender sender, string message, int? replyId = null)
        {
            try
            {
                await SendMessageAsImage(sender, message, replyId);
            }
            catch (Exception e)
            {
                sender.Bot.BotLogger.Warn("Failed to send message {msg} as image because {e}", message, e);
                await sender.ReplyAsync(message, replyId);
            }
        }

        /// <summary>
        /// 以图像的形式回复文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="replyId"></param>
        /// <returns></returns>
        private static async Task SendMessageAsImage(ISender sender, string message, int? replyId = null)
        {
            if (GraphicPlugin.GlobalRenderer == null)
                throw new Exception("GraphicPlugin.GlobalRenderer 是null！");

            using var grid = new Grid();

            var content = new VerticalLayout();
            content.Width = 180;

            var text = new TextBox();
            text.Height = 60;
            text.Text = message;
            text.FontPaint.TextSize = 16;
            text.FontPaint.TextAlign = SKTextAlign.Left;
            text.Margin = new Margin(8, 8, 8, 8);
            content.Children.Add(text);

            var bg = new Rectangle();
            bg.Margin = new Margin(0, 0, 0, 0);
            bg.Paint.Color = SKColors.LightGray;
            bg.Radius = new SKSize(10, 10);

            grid.Children.Add(bg);
            grid.Children.Add(content);

            using var image = GraphicPlugin.GlobalRenderer.Draw(grid);
            var msgBuilder = new MessageBuilder().Image(image);

            await sender.ReplyAsync(msgBuilder, replyId);
        }
    }
}
