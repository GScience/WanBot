using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot.Plugin.EssentialPermission
{
    public static class ISenderExtension
    {
        public static bool HasCommandPermission(this ISender sender, BasePlugin plugin, string cmdName, params string[] args)
        {
            var commandPermission = Permission.GetCommandPermission(plugin, cmdName, args);
            if (!Permission.CheckSender(sender, commandPermission))
            {
                LogPermissionRequirement(sender, commandPermission);
                return false;
            }
            return true;
        }

        public static bool HasPermission(this ISender sender, BasePlugin plugin, string permission)
        {
            var pluginPermission = Permission.GetPluginPermission(plugin, permission);
            if (!Permission.CheckSender(sender, pluginPermission))
            {
                LogPermissionRequirement(sender, pluginPermission);
                return false;
            }
            return true;
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
            {
                LogPermissionRequirement(sender, commandPermission);
                throw new PermissionException(commandPermission);
            }
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
            {
                LogPermissionRequirement(sender, pluginPermission);
                throw new PermissionException(pluginPermission);
            }
        }

        private static void LogPermissionRequirement(this ISender sender, string fullPermission)
        {
            Permission.logger.Warn("Sender {sender} do not have permission {permission}", sender.Name, fullPermission);
        }
    }
}
