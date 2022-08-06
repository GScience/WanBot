using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WanBot.Api.Util;

namespace WanBot.Plugin.EssentialPermission
{
    internal class PermissionDatabase
    {
        internal JsonSerializerOptions serializerOption;
        internal PermissionConfig permissionConfig;

        public ConcurrentDictionary<long, PermissionEntry>? GroupPermission { get; private set; }

        public PermissionDatabase(PermissionConfig config)
        {
            permissionConfig = config;
            serializerOption = new()
            {
                WriteIndented = true
            };
        }

        public PermissionEntry GetGroupPermission(long groupId)
        {
            if (GroupPermission == null)
            {
                Permission.logger.Error("Permission database not loaded");
                return new PermissionEntry();
            }

            return GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });
        }

        public void AddGroupPermission(long groupId, string permission)
        {
            if (GroupPermission == null)
            {
                Permission.logger.Error("Permission database not loaded");
                return;
            }

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
            if (GroupPermission == null)
            {
                Permission.logger.Error("Permission database not loaded");
                return;
            }

            var group = GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });

            group.AdditionPermissions.Remove(permission);

            if (!group.RemovedPermissions.Contains(permission))
                group.RemovedPermissions.Add(permission);
        }

        public bool TryMoveGroup(long groupId, string newGroup)
        {
            if (GroupPermission == null)
            {
                Permission.logger.Error("Permission database not loaded");
                return false;
            }

            var group = GroupPermission.GetOrAdd(groupId, (id) => new PermissionEntry
            {
                Id = id,
                PermissionGroup = permissionConfig.DefaultGroupGroup
            });

            if (permissionConfig.GroupPermissionGroups.Where(group => group.Name == newGroup).Any())
            {
                group.PermissionGroup = newGroup;
                return true;
            }

            return false;
        }

        public void Save(string path)
        {
            if (GroupPermission == null)
            {
                Permission.logger.Error("Permission database not loaded");
                return;
            }

            lock (this)
            {
                using var outputFile = File.Create(path);
                var dict = new Dictionary<long, PermissionEntry>();

                foreach (var entry in GroupPermission)
                {
                    if (entry.Value.PermissionGroup == permissionConfig.DefaultGroupGroup &&
                        entry.Value.AdditionPermissions.Count == 0 &&
                        entry.Value.RemovedPermissions.Count == 0)
                        continue;

                    dict[entry.Key] = entry.Value;
                }
                JsonSerializer.Serialize(outputFile, GroupPermission, serializerOption);
            }
        }

        public void Load(string path)
        {
            lock (this)
            {
                if (!File.Exists(path))
                {
                    GroupPermission = new();
                    return;
                }

                using var inputFile = File.OpenRead(path);
                GroupPermission = JsonSerializer.Deserialize<ConcurrentDictionary<long, PermissionEntry>>(inputFile, serializerOption);

                if (GroupPermission == null)
                    Permission.logger.Error("Failed to load permission database");
                else
                {
                    foreach (var entry in GroupPermission)
                    {
                        entry.Value.Id = entry.Key;
                        var permissionGroup = entry.Value.PermissionGroup;

                        if (!permissionConfig.GroupPermissionGroups.Where(group => group.Name == permissionGroup).Any())
                            Permission.logger.Error("Permission group {group} not found. Group {groupId} has no permission", permissionGroup, entry.Key);
                    }
                }
            }
        }
    }

    internal class PermissionEntry
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// 所属权限组
        /// </summary>
        public string PermissionGroup { get; set; } = string.Empty;

        /// <summary>
        /// 额外权限
        /// </summary>
        public List<string> AdditionPermissions { get; set; } = new();

        /// <summary>
        /// 移除权限
        /// </summary>
        public List<string> RemovedPermissions { get; set; } = new();
    }
}
