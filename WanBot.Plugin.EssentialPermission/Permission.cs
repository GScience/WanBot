using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Message;

namespace WanBot.Plugin.EssentialPermission
{
    public static class Permission
    {
        internal static ILogger logger = null!;
        internal static PermissionConfig config = null!;
        internal static PermissionDatabase database = null!;

        internal static bool IsContainTarget(string permission, string target)
        {
            var permissionArray = permission.Split('.');
            var targetArray = target.Split('.');

            for (var i = 0; i < Math.Min(permissionArray.Length, targetArray.Length); ++i)
            {
                if (permissionArray[i] == "*")
                    return true;

                if (permissionArray[i] != targetArray[i])
                    return false;
            }

            return permissionArray.Length == targetArray.Length;
        }

        internal static bool CheckPermission(List<string> permissionGroupPermission, List<string> addition, List<string> removed, string target)
        {
            foreach (var permission in removed)
            {
                if (IsContainTarget(permission, target))
                    return false;
            }

            foreach (var permission in addition)
            {
                if (IsContainTarget(permission, target))
                    return true;
            }

            foreach (var permission in permissionGroupPermission)
            {
                if (IsContainTarget(permission, target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 检查群组权限
        /// </summary>
        /// <param name="qq"></param>
        /// <param name="permission"></param>
        internal static bool CheckGroup(long group, string permission)
        {
            var entry = database.GetGroupPermission(group);
            var permissionGroup
                = config.GroupPermissionGroups.Where((group) => group.Name == entry.PermissionGroup).FirstOrDefault();

            if (permissionGroup == null)
                throw new Exception($"Permission group {entry.PermissionGroup} not found");

            return CheckPermission(permissionGroup.Permissions, entry.AdditionPermissions, entry.RemovedPermissions, permission);
        }

        /// <summary>
        /// 查看用户权限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        internal static bool CheckSender(ISender sender, string permission)
        {
            switch (sender)
            {
                case GroupSender groupSender:
                    return
                        config.Admin.Contains(groupSender.Id) ||
                        CheckGroup(groupSender.GroupId, permission);
                case StrangerSender strangerSender:
                    return
                        config.Admin.Contains(strangerSender.Id) ||
                        CheckGroup(strangerSender.GroupId, permission);
                case FriendSender friendSender:
                    return config.Admin.Contains(friendSender.Id);
                default:
                    return false;
            }
        }

        internal static string GetCommandPermission(BasePlugin plugin, string cmdName, params string[] args)
        {
            var permission = $"{GetPluginPermission(plugin, "Command")}.{cmdName}";

            foreach (var arg in args)
                permission += $".{arg}";
            return permission;
        }

        internal static string GetPluginPermission(BasePlugin plugin, string permission)
        {
            return $"{plugin.PluginName}.{permission}";
        }
    }
}
