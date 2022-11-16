using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api.Mirai
{
    /// <summary>
    /// MiraiBot报错
    /// </summary>
    public class MiraiBotEventException : Exception
    {
        public string Payload { get; }

        public BaseMiraiEvent Event { get; }

        public MiraiBot Bot { get; }
        public MiraiBotEventException(MiraiBot bot, string payload, string message, BaseMiraiEvent e, Exception inner) 
            : base(message, inner)
        {
            Payload = payload;
            Event = e;
            Bot = bot;
        }

        public override string ToString()
        {
            var msg = $"{base.ToString()}\nPayload: {Payload}";
            return msg.Replace(Bot.SessionKey, new string('*', Bot.SessionKey.Length));
        }
    }
}
