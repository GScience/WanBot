using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令支持插件
    /// </summary>
    public class CommandEventPlugin : BaseWanBotPlugin
    {
        [MiraiEvent<GroupMessage>(Priority.Highest)]
        public async Task OnGroupMessage(MiraiBot sender, GroupMessage e)
        {
            if (e.MessageChain.Compare("犊子开发机呢"))
                await sender.SendGroupMessageAsync(e.Sender.Group.Id, null, new MessageChain(new[] { new Plain { Text = "在这呢！" } }));

            var chain = e.MessageChain;

            if (!chain.StartsWith("#"))
                return;

            var messageDivider = new MessageChainDivider(e.MessageChain);
            if (messageDivider.ReadNext() is string str)
            {
                if (str == "#复读测试")
                    await sender.SendGroupMessageAsync(e.Sender.Group.Id, null, new MessageChain(messageDivider.ReadAll()));
                else if(str == "#图像测试")
                {
                    using var surface = SKSurface.Create(new SKImageInfo(250, 250));
                    var random = new Random((int)((uint)e.Sender.Id - uint.MaxValue / 2));

                    using var backColor = new SKPaint();
                    backColor.Color = new SKColor(
                        255,
                        255,
                        255);

                    using var frontColor = new SKPaint();
                    frontColor.Color = new SKColor(
                        (byte)random.Next(0, 64),
                        (byte)random.Next(0, 64),
                        (byte)random.Next(0, 64));

                    surface.Canvas.Clear(backColor.Color);
                    for (var x = 0; x < 5; ++x)
                        for (var y = 0; y < 10; ++y)
                        {
                            switch (random.Next(0, 5))
                            {
                                case 0:
                                    surface.Canvas.DrawRect(x * 25, y * 25, 25, 25, frontColor);
                                    surface.Canvas.DrawRect((9 - x) * 25, y * 25, 25, 25, frontColor);
                                    break;
                                default:
                                    break;
                            }
                        }
                    surface.Flush();
                    var messageBuilder =
                        new MessageBuilder().Text("这是你的NFT").Image(new MiraiImage(sender, surface.Snapshot()));
                    await sender.SendGroupMessageAsync(e.Sender.Group.Id, null, messageBuilder);

                }
            }
        }
    }
}
