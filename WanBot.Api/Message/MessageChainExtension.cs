using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WanBot.Api.Mirai.Message;

namespace WanBot.Api.Message
{
    /// <summary>
    /// 消息链扩展
    /// </summary>
    public static class MessageChainExtension
    {
        /// <summary>
        /// 比较消息链与字符串
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Compare(this MessageChain chain, string str)
        {
            var plain = chain.AsPlain();
            if (plain == null)
                return false;
            return plain.Text == str;
        }
        
        /// <summary>
        /// 消息内是否包含关键词
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Contain(this MessageChain chain, string str)
        {
            foreach (var msg in chain)
            {
                if (msg is not Plain plain)
                    continue;

                if (plain.Text.Contains(str))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 消息内是否包含正则表达式
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool Contain(this MessageChain chain, Regex regex)
        {
            foreach (var msg in chain)
            {
                if (msg is not Plain plain)
                    continue;

                var match = regex.Match(plain.Text);
                if (match.Success)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 由某字符串开始
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool StartsWith(this MessageChain chain, string str)
        {
            foreach (var msg in chain)
            {
                if (msg is Source)
                    continue;

                if (msg is not Plain plain)
                    break;

                if (plain.Text.StartsWith(str))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 将消息链转换为纯文本类型，若无法转换则返回Null
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public static Plain? AsPlain(this MessageChain chain)
        {
            Plain? plain = null;

            var enumerator = chain.GetEnumerator();
            do
            {
                if (!enumerator.MoveNext())
                    break;

                var baseChain = enumerator.Current;
                if (baseChain is Source)
                    continue;
                else if (baseChain is Plain)
                    plain = (Plain)baseChain;
                else
                {
                    plain = null;
                    break;
                }
            } while (true);

            return plain;
        }
    }
}
