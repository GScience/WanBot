using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot.Api.Util
{
    /// <summary>
    /// 命令分发器，负责处理子命令的调用
    /// </summary>
    public class CommandDispatcher
    {
        private ConcurrentDictionary<string, CommandDispatcher> _dispatcherDict = new();

        public Func<MiraiBot, CommandEventArgs, Task<bool>>? Handle { get; set; }

        public async Task<bool> HandleCommandAsync(MiraiBot bot, CommandEventArgs commandEvent)
        {
            string subCmd;
            try
            {
                subCmd = commandEvent.GetNextArgs<string>();
            }
            catch (Exception)
            {
                return false;
            }

            if (_dispatcherDict.TryGetValue(subCmd, out var dispatcher))
            {
                if (dispatcher.Handle != null)
                    return await dispatcher.Handle.Invoke(bot, commandEvent);
                return await dispatcher.HandleCommandAsync(bot, commandEvent);
            }
            return false;
        }

        public CommandDispatcher this[string cmd]
        {
            get
            {
                return _dispatcherDict.GetOrAdd(cmd, static (str) => new CommandDispatcher());
            }
            set
            {
                _dispatcherDict[cmd] = value;
            }
        }
    }
}
