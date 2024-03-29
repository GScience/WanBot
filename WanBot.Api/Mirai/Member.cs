﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// 群成员
    /// </summary>
    public struct Member
    {
        public Member()
        {
        }

        /// <summary>
        /// 成员QQ Id
        /// </summary>
        public long Id { get; set; } = 0;

        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; set; } = string.Empty;

        /// <summary>
        /// 成员头衔
        /// </summary>
        public string SpecialTitle { get; set; } = string.Empty;

        /// <summary>
        /// 成员群内权限
        /// </summary>
        public string Permission { get; set; } = string.Empty;

        /// <summary>
        /// 加入时间
        /// </summary>
        public int JoinTimestamp { get; set; } = 0;

        /// <summary>
        /// 上次发言时间
        /// </summary>
        public int LastSpeakTimestamp { get; set; } = 0;

        /// <summary>
        /// 禁言剩余时间
        /// </summary>
        public int MuteTimeRemaining { get; set; } = 0;

        /// <summary>
        /// 所在群组
        /// </summary>
        public Group Group { get; set; } = new();

        public string GetFormatedName()
        {
            return $"{Group.GetFormatedName()} {MemberName}({Id})";
        }
    }
}
