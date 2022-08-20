using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Util;

namespace WanBot.Api
{
    /// <summary>
    /// WanBot插件基类，对BasePlugin进行更简单的封装
    /// </summary>
    public abstract class WanBotPlugin : BasePlugin
    {
        public sealed override void PostInit()
        {
            foreach (var method in GetType().GetMethods())
            {
                var eventAttributes = method.GetCustomAttributes<EventAttribute>();

                foreach (var attribute in eventAttributes)
                {
                    switch (attribute)
                    {
                        case MiraiEventAttribute miraiEvent:
                            AddMiraiEvent(method, miraiEvent);
                            break;
                        case CommandAttribute commandEvent:
                            AddCommandEvent(method, commandEvent);
                            break;
                        case AtAttribute atEvent:
                            AddAtEvent(method, atEvent);
                            break;
                        case RegexAttribute keywordEvent:
                            AddRegexEvent(method, keywordEvent);
                            break;
                        case NudgeAttribute nudgeEvent:
                            AddNudgeEvent(method, nudgeEvent);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取插件配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetConfig<T>() where T : new()
        {
            return Application.ReadConfig<T>(PluginName);
        }

        public string GetConfigPath()
        {
            var configPath = Application.GetConfigPath(PluginName);
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            return configPath;
        }

        private bool CheckArgs<T>(MethodInfo method) where T : BlockableEventArgs
        {
            return CheckArgs(method, typeof(T));
        }

        private void AddNudgeEvent(MethodInfo method, NudgeAttribute commandEvent)
        {
            if (!CheckArgs<NudgeEventArgs>(method))
                return;

            // 注册事件
            Application.BotManager.Subscript(
                typeof(NudgeEventArgs),
                commandEvent.Priority,
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );
        }

        private bool CheckArgs(MethodInfo method, Type type)
        {
            // 检查插件参数
            var args = method.GetParameters();
            if (args.Length != 2 ||
                args[0].ParameterType != typeof(MiraiBot) ||
                args[1].ParameterType != type ||
                method.IsStatic ||
                method.ReturnType != typeof(Task))
            {
                Logger.Error(
                    "Not a valid mirai event handler. Require:\n {RequireFunc}\nBut Get:\n {GetFunc}",
                    $"{typeof(Task)} {method.Name}({typeof(MiraiBot)}, {type});",
                    method.ToString());
                return false;
            }
            return true;
        }

        private void AddRegexEvent(MethodInfo method, RegexAttribute regexEvent)
        {
            // 检查插件参数
            if (!CheckArgs<RegexEventArgs>(method))
                return;

            // 注册事件
            Application.BotManager.Subscript(
                RegexEventArgs.GetEventName(regexEvent.Regex),
                regexEvent.Priority,
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );

            // 注册正则监听到事件插件
            Application.PluginManager.GetPlugin<SimpleEventsPlugin>()!.RegexTable.Add(regexEvent);
        }

        private void AddAtEvent(MethodInfo method, AtAttribute commandEvent)
        {
            if (!CheckArgs<AtEventArgs>(method))
                return;

            // 注册事件
            Application.BotManager.Subscript(
                typeof(AtEventArgs),
                commandEvent.Priority,
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );
        }

        private void AddCommandEvent(MethodInfo method, CommandAttribute commandEvent)
        {
            // 检查插件参数
            if (!CheckArgs<CommandEventArgs>(method))
                return;

            // 注册事件

            foreach (var cmd in commandEvent.Commands)
            {
                Application.BotManager.Subscript(
                    CommandEventArgs.GetEventName(cmd),
                    commandEvent.Priority,
                    (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                    );
            }
        }

        private void AddMiraiEvent(MethodInfo method, MiraiEventAttribute miraiEvent)
        {
            // 检查插件参数
            if (!CheckArgs(method, miraiEvent.EventType))
                return;

            // 注册事件
            Application.BotManager.Subscript(
                miraiEvent.EventType, 
                miraiEvent.Priority, 
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );
        }
    }
}
