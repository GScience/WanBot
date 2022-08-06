using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Message;
using WanBot.Api.Mirai;
using WanBot.Api.Util;

namespace WanBot.Plugin.EssentialPermission
{
    public class PermissionPlugin : WanBotPlugin
    {
        public override string PluginName => "Permission";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private CommandDispatcher _mainDispatcher = new CommandDispatcher();

        public override void PreInit()
        {
            Permission.config = GetConfig<PermissionConfig>();
            Permission.database = new(Permission.config);

            _mainDispatcher["group"].Handle = OnGroupPermissionCommand;
        }

        [Command("permission")]
        public async Task OnPermissionCommand(MiraiBot bot, CommandEventArgs args)
        {
            try
            {
                if (await _mainDispatcher.HandleCommandAsync(bot, args))
                    return;
                await args.Sender.ReplyAsync($"参数错误");
            }
            catch (PermissionException)
            { 
            }
            catch (Exception)
            {
                await args.Sender.ReplyAsync($"出现未知错误，详情请查看日志");
                throw;
            }
        }

        public async Task<bool> OnGroupPermissionCommand(MiraiBot bot, CommandEventArgs args)
        {
            var groupIdStr = args.GetNextArgs<string>();
            long groupId;
            if (groupIdStr == "this" && args.Sender is GroupSender groupSender)
                groupId = groupSender.GroupId;
            else
                groupId = long.Parse(groupIdStr);

            var groupDispatcher = new CommandDispatcher();
            groupDispatcher["add"].Handle = (b, a) => OnGroupPermissionAddCommand(b, a, groupId);
            groupDispatcher["remove"].Handle = (b, a) => OnGroupPermissionRemoveCommand(b, a, groupId);
            groupDispatcher["list"].Handle = (b, a) => OnGroupPermissionListCommand(b, a, groupId);
            groupDispatcher["check"].Handle = (b, a) => OnGroupPermissionCheckCommand(b, a, groupId);
            return await groupDispatcher.HandleCommandAsync(bot, args);
        }

        public async Task<bool> OnGroupPermissionAddCommand(MiraiBot bot, CommandEventArgs args, long groupId)
        {
            args.Sender.RequirePermission(this, "Admin");

            var permission = args.GetNextArgs<string>();
            Permission.database.AddGroupPermission(groupId, permission);
            await args.Sender.ReplyAsync($"成功将权限 {permission} 添加到群 {groupId}");
            Logger.Info("Add permission {permission} to group {group}", permission, groupId);
            return true;
        }

        public async Task<bool> OnGroupPermissionRemoveCommand(MiraiBot bot, CommandEventArgs args, long groupId)
        {
            args.Sender.RequirePermission(this, "Admin");

            var permission = args.GetNextArgs<string>();
            Permission.database.RemoveGroupPermission(groupId, permission);
            await args.Sender.ReplyAsync($"成功将权限 {permission} 从群 {groupId} 中移除");
            Logger.Info("Remove permission {permission} from group {group}", permission, groupId);
            return true;
        }

        public async Task<bool> OnGroupPermissionListCommand(MiraiBot bot, CommandEventArgs args, long groupId)
        {
            args.Sender.RequirePermission(this, "Admin");

            var entry = Permission.database.GetGroupPermission(groupId);
            var group = Permission.config.GroupPermissionGroups.Where((group) => group.Name == entry.PermissionGroup).FirstOrDefault();
            var permissions = string.Join('\n', group!.Permissions);
            var additionPermission = string.Join('\n', entry.AdditionPermissions);
            var removedPermission = string.Join('\n', entry.RemovedPermissions);

            await args.Sender.ReplyAsync(
                $"权限组：{entry.PermissionGroup}\n" +
                $"默认权限：\n{permissions}\n" +
                $"额外权限：\n{additionPermission}\n" +
                $"禁止权限：\n{removedPermission}");

            return true;
        }

        public async Task<bool> OnGroupPermissionCheckCommand(MiraiBot bot, CommandEventArgs args, long groupId)
        {
            args.Sender.RequirePermission(this, "User");

            var permission = args.GetNextArgs<string>();

            if (Permission.CheckSender(args.Sender, permission))
                await args.Sender.ReplyAsync($"你可以 {permission} 哦");
            else
                await args.Sender.ReplyAsync($"你不可以 {permission} 哦");

            return true;
        }
    }
}
