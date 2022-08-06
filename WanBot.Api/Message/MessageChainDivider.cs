using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    /// <summary>
    /// 消息链分割器
    /// </summary>
    public class MessageChainDivider
    {
        private MessageChain _baseChain;

        private IEnumerator _enumerator;

        /// <summary>
        /// 是否读取所有
        /// </summary>
        private bool _readAll;

        /// <summary>
        /// 获取消息Id，如果无法获取则返回Null
        /// </summary>
        /// <returns></returns>
        public int? GetMessageId()
        {
            if (_baseChain.FirstOrDefault() is not Source source)
                return null;
            return source.Id;
        }

        public MessageChainDivider(MessageChain chain)
        {
            _baseChain = chain;
            _enumerator = GetDividerEnumerator();
        }

        /// <summary>
        /// 读取所有
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseChain> ReadAll()
        {
            _readAll = true;
            while (_enumerator.MoveNext())
                yield return (BaseChain)_enumerator.Current;
        }

        /// <summary>
        /// 读取下一个
        /// </summary>
        /// <returns></returns>
        public object? ReadNext()
        {
            if (_readAll)
                return null;

            if (!_enumerator.MoveNext())
                return null;

            return _enumerator.Current;
        }

        private IEnumerator GetDividerEnumerator()
        {
            foreach (var msg in _baseChain)
            {
                if (msg is Source)
                    continue;

                if (msg is not Plain plain)
                {
                    yield return msg;
                    continue;
                }

                var text = plain.Text;
                var buffer = "";

                foreach (var c in text)
                {
                    if (c == ' ' && !_readAll)
                    {
                        if (!string.IsNullOrWhiteSpace(buffer))
                            yield return buffer;

                        buffer = "";
                    }
                    else
                        buffer += c;
                }

                if (_readAll)
                    yield return new Plain { Text = buffer };
                else if (!string.IsNullOrWhiteSpace(buffer))
                    yield return buffer;
            }
        }
    }
}
