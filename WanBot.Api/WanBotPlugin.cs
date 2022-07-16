using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Mirai.Event;

namespace WanBot.Api
{
    /// <summary>
    /// WanBot插件基类，对BasePlugin进行更简单的封装
    /// </summary>
    public abstract class BaseWanBotPlugin : BasePlugin
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
                    }
                }
            }
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
