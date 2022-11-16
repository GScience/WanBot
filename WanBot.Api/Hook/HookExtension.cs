using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Event;
using WanBot.Api.Mirai;

namespace WanBot.Api.Hook
{
    internal static class HookExtension
    {
        public static T Hook<T>(this T obj, MiraiBot bot, HookType type)
        {
            switch (type)
            {
                case HookType.Event:
                    return (T)(object)HookEvent((BlockableEventArgs)(object)obj!, bot);
                case HookType.Exception:
                    return ((T?)(object?)HookException((Exception)(object)obj!, bot))!;
                case HookType.Api:
                    return (T)HookApi(obj!, bot);
            }
            return obj;
        }

        private static object HookApi(object obj, MiraiBot bot)
        {
            return HookTable.Instance.ApiHook?.Invoke(bot, obj) ?? obj;
        }

        private static Exception? HookException(Exception e, MiraiBot bot)
        {
            if (HookTable.Instance.ExceptionHook == null)
                return e;
            return HookTable.Instance.ExceptionHook.Invoke(bot, e);
        }

        private static BlockableEventArgs HookEvent(BlockableEventArgs e, MiraiBot bot)
        {
            return HookTable.Instance.EventHook?.Invoke(bot, e) ?? e;
        }
    }
}
