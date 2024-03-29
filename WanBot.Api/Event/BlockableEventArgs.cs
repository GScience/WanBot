﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 可阻塞的事件
    /// </summary>
    public class BlockableEventArgs
    {
        /// <summary>
        /// 是否取消事件的传递
        /// </summary>
        [JsonIgnore]
        public bool Blocked { get; set; }
    }
}
