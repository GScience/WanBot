﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api
{
    /// <summary>
    /// 消息发送者接口
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sender所在的bot
        /// </summary>
        public MiraiBot Bot { get; }

        /// <summary>
        /// 内部名称
        /// </summary>
        public string InternalName { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 消息发送源的QQ号
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="messageChain"></param>
        Task ReplyAsync(MessageChain messageChain);

        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message"></param>
        Task ReplyAsync(string message, int? replyId = null);

        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="messageBuilder"></param>
        Task ReplyAsync(IMessageBuilder messageBuilder, int? replyId = null);

        /// <summary>
        /// 戳一戳对方
        /// </summary>
        /// <returns></returns>
        Task NudgeAsync();

        /// <summary>
        /// 获取发送者信息
        /// </summary>
        /// <returns></returns>
        Task<Profile> GetProfileAsync();
    }
}
