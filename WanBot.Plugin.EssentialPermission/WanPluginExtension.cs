using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot.Plugin.EssentialPermission
{
    public static class WanPluginExtension
    {
        public static bool HasCommandPermission(this ISender sender, BasePlugin plugin, string cmdName, params string[] args)
        {
            return Permission.CheckCommand(sender, plugin, cmdName, args);
        }

        public static bool HasPermission(this ISender sender, BasePlugin plugin, string permission)
        {
            return Permission.Check(sender, plugin, permission);
        }

        /// <summary>
        /// 请求命令权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="plugin"></param>
        /// <param name="cmdName"></param>
        /// <param name="args"></param>
        /// <exception cref="PermissionException"></exception>
        public static void RequireCommandPermission(this ISender sender, BasePlugin plugin, string cmdName, params string[] args)
        {
            var commandPermission = Permission.GetCommandPermission(plugin, cmdName, args);
            if (!Permission.CheckSender(sender, commandPermission))
                throw new PermissionException(commandPermission);
        }

        /// <summary>
        /// 请求权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="plugin"></param>
        /// <param name="permission"></param>
        /// <exception cref="PermissionException"></exception>
        public static void RequirePermission(this ISender sender, BasePlugin plugin, string permission)
        {
            var pluginPermission = Permission.GetPluginPermission(plugin, permission);
            if (!Permission.CheckSender(sender, pluginPermission))
                throw new PermissionException(pluginPermission);
        }
    }
}
