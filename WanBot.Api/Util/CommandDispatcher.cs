using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;

namespace WanBot.Api.Util
{
    /// <summary>
    /// 命令分发器，负责处理子命令的调用
    /// </summary>
    public class CommandDispatcher
    {
        private Dictionary<string, CommandDispatcher> _dispatcherDict = new();

        public Func<CommandEventArgs, Task<bool>>? Handle { get; set; }

        public async Task<bool> HandleCommandAsync(CommandEventArgs commandEvent)
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
                    return await dispatcher.Handle.Invoke(commandEvent);
                return await dispatcher.HandleCommandAsync(commandEvent);
            }
            return false;
        }

        public CommandDispatcher this[string cmd]
        {
            get
            {
                var newDispatcher = new CommandDispatcher();
                _dispatcherDict[cmd] = newDispatcher;
                return newDispatcher;
            }
            set
            {
                _dispatcherDict[cmd] = value;
            }
        }
    }
}
