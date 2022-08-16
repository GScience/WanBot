using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Message;

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
    }
}
