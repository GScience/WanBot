using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Mirai;

namespace WanBot.Api.Event
{
    /// <summary>
    /// 事件
    /// </summary>
    public class WanBotEventEvent
    {
        private List<WanBotEventHandler> _handlers = new();

        /// <summary>
        /// 插入事件处理器，并保持顺序
        /// </summary>
        /// <param name="handler"></param>
        public void Add(WanBotEventHandler handler)
        {
            if (_handlers.Count == 0)
                _handlers.Add(handler);
            else
            {
                for (var i = 0; i < _handlers.Count; i++)
                {
                    if (_handlers[i].Priority > handler.Priority)
                        continue;
                    _handlers.Insert(i, handler);
                    return;
                }
                _handlers.Insert(_handlers.Count, handler);
            }
        }

        public async Task InvokeAsync(MiraiBot sender, BlockableEventArgs e)
        {
            foreach (var handler in _handlers)
            {
                await handler.Handler.Invoke(sender, e);
                if (e.Blocked)
                    break;
            }
        }
    }
}
