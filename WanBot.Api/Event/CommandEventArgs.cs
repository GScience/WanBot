using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Event;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令事件参数
    /// </summary>
    public class CommandEventArgs : CancellableEventArgs
    {
        private MessageChainDivider _divider;
        private object? _next;

        /// <summary>
        /// 读到结尾
        /// </summary>
        public bool EOF { get; private set; }

        /// <summary>
        /// 事件发送者
        /// </summary>
        public ISender Sender { get; }

        public string Command { get; }

        public CommandEventArgs(ISender sender, MessageChainDivider divider, string cmd)
        {
            Sender = sender;
            _divider = divider;
            Command = cmd;
        }

        private bool TryMoveNextWhenReaded()
        {
            if (EOF)
                return false;

            if (_next == null)
                _next = _divider.ReadNext();

            if (_next == null)
            {
                EOF = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 尝试获取下一个字符串
        /// </summary>
        /// <returns></returns>
        public string? TryGetNextString()
        {
            if (!TryMoveNextWhenReaded())
                return null;

            if (_next is string nextString)
            {
                _next = null;
                return nextString;
            }
            return null;
        }

        /// <summary>
        /// 尝试获取下一个消息链对象
        /// </summary>
        /// <returns></returns>
        public BaseChain? TryGetNextChain()
        {
            if (!TryMoveNextWhenReaded())
                return null;

            if (_next is BaseChain nextChain)
            {
                _next = null;
                return nextChain;
            }
            return null;
        }

        /// <summary>
        /// 读取所有剩余的内容
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseChain>? GetRemain()
        {
            if (EOF)
                return null;
            EOF = true;
            return _divider.ReadAll();
        }

        public string GetEventName()
        {
            return GetEventName(Command);
        }

        public static string GetEventName(string command)
        {
            return $"{typeof(CommandEventArgs).Name}.{command}";
        }
    }
}
