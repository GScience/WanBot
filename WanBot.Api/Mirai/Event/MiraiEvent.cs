using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api.Mirai.Event
{
    /// <summary>
    /// 事件
    /// </summary>
    public class MiraiEvent
    {
        private List<MiraiEventHandler> _handlers = new();

        /// <summary>
        /// 插入事件处理器，并保持顺序
        /// </summary>
        /// <param name="handler"></param>
        public void Add(MiraiEventHandler handler)
        {
            if (_handlers.Count == 0)
                _handlers.Add(handler);
            else
                for (var i = 0; i < _handlers.Count; i++)
                {
                    if (_handlers[i].Priority > handler.Priority)
                        continue;
                    _handlers.Insert(i, handler);
                    break;
                }
        }

        public async Task InvokeAsync(MiraiBot sender, CancellableEventArgs e)
        {
            foreach (var handler in _handlers)
            {
                await handler.Handler.Invoke(sender, e);
                if (e.Cancel)
                    break;
            }
        }
    }
}
