using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// Mirai事件
    /// </summary>
    public class MiraiEventArgs
    {
        /// <summary>
        /// 是否取消事件的传递
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public BaseEvent Event { get; set; }

        public MiraiEventArgs(BaseEvent e)
        {
            Event = e;
        }
    }

    /// <summary>
    /// 泛形Mirai事件
    /// </summary>
    public class MiraiEventArgs<T> where T : BaseEvent
    {
        private readonly MiraiEventArgs _ea;

        public MiraiEventArgs(MiraiEventArgs ea)
        {
            _ea = ea;
        }

        /// <summary>
        /// 是否取消事件的传递
        /// </summary>
        public bool Cancel
        {
            get => _ea.Cancel;
            set => _ea.Cancel = value;
        }

        /// <summary>
        /// 事件
        /// </summary>
        public T Event => (T)_ea.Event;
    }
}
