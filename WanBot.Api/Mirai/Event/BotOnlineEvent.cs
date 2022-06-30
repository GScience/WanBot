using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Bot登录成功
    /// </summary>
    public class BotOnlineEvent : BaseEvent
    {
        /// <summary>
        /// 登录成功的Bot的QQ号
        /// </summary>
        public long QQ { get; set; }
    }
}
