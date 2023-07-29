using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;
using WanBot.Api.Mirai;
using WanBot.Plugin.Essential.Permission;
using System.Security.Cryptography;
using System.Buffers.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WanBot.Plugin.Essential
{
    /// <summary>
    /// 加密命令插件
    /// </summary>
    internal class EncryptCommandPlugin : WanBotPlugin
    {
        public override string PluginName => "EncryptCommand";

        public override string PluginDescription => "可发送加密命令";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private SimpleEventsPlugin _eventsPlugin = null!;

        private Aes _aes = Aes.Create();

        public override void PreInit()
        {
            base.PreInit();
            _eventsPlugin = Application.PluginManager.GetPlugin<SimpleEventsPlugin>()!;
            if (_eventsPlugin == null)
                throw new Exception("Simple event plugin not found");
        }

        [Command("dc")]
        public async Task OnDecryptCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!Permission.Permission.IsAdmin(args.Sender.Id))
                return;
            args.Blocked = true;
            var msgChain = args.GetRemain()?.ToArray();
            if (msgChain == null)
            {
                await args.Sender.ReplyAsync("解密内容为空");
                return;
            }
            // 对文本进行解密
            using (var decryptor = _aes.CreateDecryptor())
            {
                foreach (var baseChain in msgChain)
                {
                    if (baseChain is not Plain plain)
                        continue;
                    using var msDecrypt = new MemoryStream(Convert.FromBase64String(plain.Text));
                    using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using var swDecrypt = new StreamReader(csDecrypt);
                    plain.Text = swDecrypt.ReadToEnd();
                }
            }

            await _eventsPlugin.OnCommandMessage(bot, args.Sender, new MessageChain(msgChain));
            return;
        }

        [Command("ec")]
        public async Task OnEncryptCommand(MiraiBot bot, CommandEventArgs args)
        {
            if (!Permission.Permission.IsAdmin(args.Sender.Id))
                return;
            args.Blocked = true;
            var msgChain = args.GetRemain()?.ToArray();
            if (msgChain == null)
            {
                await args.Sender.ReplyAsync("加密内容为空");
                return;
            }
            using (var encryptor = _aes.CreateEncryptor())
            {
                foreach (var baseChain in msgChain)
                {
                    if (baseChain is not Plain plain)
                        continue;
                    using var msEncrypt = new MemoryStream();
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using var swEncrypt = new StreamWriter(csEncrypt);
                        swEncrypt.Write(plain.Text);
                    }
                    var encrypted = msEncrypt.ToArray();
                    plain.Text = Convert.ToBase64String(encrypted);
                }
            }

            await args.Sender.ReplyAsync(new MessageChain(msgChain));
        }
    }
}
