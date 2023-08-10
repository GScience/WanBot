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
        public async static Task<T?> HookAsync<T>(this T obj, MiraiBot bot, HookType type)
        {
            switch (type)
            {
                case HookType.Event:
                    return (T?)(object?)await HookEventAsync((BlockableEventArgs)(object)obj!, bot);
                case HookType.Exception:
                    return ((T?)(object?)await HookExceptionAsync((Exception)(object)obj!, bot))!;
                case HookType.Api:
                    return (T?)await HookApiAsync(obj!, bot);
            }
            return obj;
        }

        private async static Task<object?> HookApiAsync(object obj, MiraiBot bot)
        {
            if (HookTable.Instance.ApiHook == null)
                return obj;
            return await HookTable.Instance.ApiHook.Invoke(bot, obj);
        }

        private async static Task<Exception?> HookExceptionAsync(Exception e, MiraiBot bot)
        {
            if (HookTable.Instance.ExceptionHook == null)
                return e;
            return await HookTable.Instance.ExceptionHook.Invoke(bot, e);
        }

        private async static Task<BlockableEventArgs?> HookEventAsync(BlockableEventArgs e, MiraiBot bot)
        {
            if (HookTable.Instance.EventHook == null)
                return e;
            return await HookTable.Instance.EventHook.Invoke(bot, e);
        }
    }
}
