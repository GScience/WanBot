using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

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
            return Application.ReadConfig<T>(GetType().Name);
        }

        private void AddCommandEvent(MethodInfo method, CommandAttribute commandEvent)
        {
            // 检查插件参数
            var args = method.GetParameters();
            if (args.Length != 2 ||
                args[0].ParameterType != typeof(MiraiBot) ||
                args[1].ParameterType != typeof(CommandEventArgs) ||
                method.IsStatic ||
                method.ReturnType != typeof(Task))
            {
                Logger.Error(
                    "Not a valid mirai event handler. Require:\n {RequireFunc}\nBut Get:\n {GetFunc}",
                    $"{typeof(Task)} {method.Name}({typeof(MiraiBot)}, {typeof(CommandEventArgs)});",
                    method.ToString());
                return;
            }
            
            // 注册事件
            Application.BotManager.Subscript(
                CommandEventArgs.GetEventName(commandEvent.Command),
                commandEvent.Priority,
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );
        }

        private void AddMiraiEvent(MethodInfo method, MiraiEventAttribute miraiEvent)
        {
            // 检查插件参数
            var args = method.GetParameters();
            if (args.Length != 2 ||
                args[0].ParameterType != typeof(MiraiBot) ||
                args[1].ParameterType != miraiEvent.EventType ||
                method.IsStatic ||
                method.ReturnType != typeof(Task))
            {
                Logger.Error(
                    "Not a valid mirai event handler. Require:\n {RequireFunc}\nBut Get:\n {GetFunc}",
                    $"{typeof(Task)} {method.Name}({typeof(MiraiBot)}, {miraiEvent.EventType});",
                    method.ToString());
                return;
            }

            // 注册事件
            Application.BotManager.Subscript(
                miraiEvent.EventType, 
                miraiEvent.Priority, 
                (bot, e) => (Task)method.Invoke(this, new object?[] { bot, e })!
                );
        }
    }
}
