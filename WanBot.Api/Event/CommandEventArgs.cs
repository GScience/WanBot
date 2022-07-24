using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Message;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 命令事件参数
    /// </summary>
    public class CommandEventArgs : BlockableEventArgs
    {
        private MessageChainDivider _divider;

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

        /// <summary>
        /// 尝试获取下一个字符串
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetNextArgs<T>()
        {
            var next = _divider.ReadNext();
            if (next is not T nextT)
                throw new InvalidOperationException($"{next} is not {typeof(T)}");
            return nextT;
        }

        /// <summary>
        /// 读取所有剩余的内容
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseChain>? GetRemain()
        {
            var enumerator = _divider.ReadAll().GetEnumerator();

            // 第一个字符串去除开始空格
            if (!enumerator.MoveNext())
                yield break;

            var first = enumerator.Current;

            if (first is Plain firstPlain)
            {
                firstPlain.Text = firstPlain.Text.TrimStart();

                if (!string.IsNullOrEmpty(firstPlain.Text))
                    yield return first;
            }
            else
                yield return first;

            while (enumerator.MoveNext())
                yield return enumerator.Current;
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
