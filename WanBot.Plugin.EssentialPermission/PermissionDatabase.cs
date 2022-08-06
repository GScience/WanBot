using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api.Util;

namespace WanBot.Plugin.EssentialPermission
{
    public class PermissionDatabase
    {
        internal PermissionConfig permissionConfig;

        public ConcurrentDictionary<long, PermissionEntry> GroupPermission { get; set; } = new();

        public PermissionDatabase(PermissionConfig config)
        {
            permissionConfig = config;
        }

        public PermissionEntry GetGroupPermission(long groupId)
        {
            return GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });
        }

        public void AddGroupPermission(long groupId, string permission)
        {
            var group = GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });

            group.RemovedPermissions.Remove(permission);

            if (!group.AdditionPermissions.Contains(permission))
                group.AdditionPermissions.Add(permission);
        }

        public void RemoveGroupPermission(long groupId, string permission)
        {
            var group = GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });

            group.AdditionPermissions.Remove(permission);

            if (!group.RemovedPermissions.Contains(permission))
                group.RemovedPermissions.Add(permission);
        }
    }

    public class PermissionEntry
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 所属权限组
        /// </summary>
        public string PermissionGroup { get; set; } = string.Empty;

        /// <summary>
        /// 额外权限
        /// </summary>
        public List<string> AdditionPermissions = new();

        /// <summary>
        /// 移除权限
        /// </summary>
        public List<string> RemovedPermissions = new();
    }
}
