﻿using System.Diagnostics;
using System.Reflection;
using WanBot.Api;
using WanBot.Api.Event;
using WanBot.Api.Mirai;
using WanBot.Api.Util;
using WanBot.Plugin.EssentialPermission;

namespace WanBot.Plugin.Core
{
    /// <summary>
    /// 核心插件
    /// </summary>
    public class CorePlugin : WanBotPlugin
    {
        public override string PluginName => "WanBot";

        public override string PluginAuthor => "WanNeng";

        public override Version PluginVersion => Version.Parse("1.0.0");

        private CommandDispatcher _mainDispatcher = new CommandDispatcher();

        public override void PreInit()
        {
            _mainDispatcher["plugins"].Handle = OnListPluginCommand;
            _mainDispatcher["plugin"].Handle = OnGetPluginInfoCommand;
            _mainDispatcher["reload"].Handle = OnReloadCommand;
            _mainDispatcher["disable"].Handle = OnDisableCommand;
            _mainDispatcher["enable"].Handle = OnEnableCommand;
            base.PreInit();
        }

        [Command("core")]
        public async Task OnCoreCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            if (!commandEvent.Sender.HasCommandPermission(this, "core"))
                return;

            try
            {
                await _mainDispatcher.HandleCommandAsync(bot, commandEvent);
            }
            catch (PermissionException e)
            {
                await commandEvent.Sender.ReplyNoPermissionAsync(e.RequirePermission);
            }
            catch (InvalidOperationException)
            {
                await commandEvent.Sender.ReplyAsync("参数错误");
            }
            commandEvent.Blocked = true;
        }

        public async Task<bool> OnReloadCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            commandEvent.Sender.RequireCommandPermission(this, "core.admin.reload");
            Logger.Info("Reload");
            await commandEvent.Sender.ReplyAsync("重新加载机器人");
            (Application as Application)?.Reload(false, false);
            return true;
        }

        public async Task<bool> OnListPluginCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            commandEvent.Sender.RequireCommandPermission(this, "core.admin.listplugin");

            var pluginList = "当前插件列表：";

            foreach (var plugin in (Application.PluginManager as PluginManager)!.Plugins)
                pluginList += plugin.PluginName + ", ";

            pluginList += "\n输入 #core plugin <pluginName> 查看插件状态";

            pluginList += "\n被禁用的插件：";
            var pluginPath = (Application as Application)?.PluginPath!;

            foreach (var disabledPlugin in Directory.EnumerateFiles(pluginPath, "*.disabled"))
            {
                var fileInfo = new FileInfo(disabledPlugin);
                pluginList += fileInfo.Name[..^".disabled".Length] + ", ";
            }
            await commandEvent.Sender.ReplyAsync(pluginList);

            return true;
        }

        public async Task<bool> OnGetPluginInfoCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            commandEvent.Sender.RequireCommandPermission(this, "core.admin.plugininfo");

            var pluginName = commandEvent.GetNextArgs<string>();

            var plugin = (Application.PluginManager as PluginManager)?.Plugins.Where((plugin) => plugin.PluginName == pluginName).FirstOrDefault();
            if (plugin == null)
                await commandEvent.Sender.ReplyAsync("插件未找到");
            else
                await commandEvent.Sender.ReplyAsync($"插件名称：{plugin.PluginName}\n插件作者{plugin.PluginAuthor}\n插件版本：{plugin.PluginVersion}");

            return true;
        }

        private IEnumerable<Assembly> GetDependentAssemblies(Assembly analyzedAssembly)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetReferencedAssemblies()
                    .Select(assemblyName => assemblyName.FullName)
                    .Contains(analyzedAssembly.FullName));
        }

        public async Task<bool> OnDisableCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            commandEvent.Sender.RequireCommandPermission(this, "core.admin.disableplugin");

            var pluginName = commandEvent.GetNextArgs<string>();

            var plugin = (Application.PluginManager as PluginManager)?.Plugins.Where((plugin) => plugin.PluginName == pluginName).FirstOrDefault();
            if (plugin == null)
            {
                await commandEvent.Sender.ReplyAsync("插件未找到");
                return true;
            }

            // 检查是否为内部依赖项
            var pluginPath = (Application.PluginManager as PluginManager)?.GetPluginPath(plugin);
            if (string.IsNullOrEmpty(pluginPath))
            {
                await commandEvent.Sender.ReplyAsync("此插件无法禁用，由于该插件为完犊子Bot的内部依赖项");
                return true;
            }

            // 检查是否有依赖此程序集的插件依赖
            var pluginAsm = plugin.GetType().Assembly;
            var deps = GetDependentAssemblies(pluginAsm);
            if (deps.Any())
            {
                var depStr = string.Join(',', deps.Select(asm => asm.GetName().Name));
                await commandEvent.Sender.ReplyAsync($"此插件无法禁用，由于依赖该插件的{depStr}未被卸载");
                return true;
            }

            Logger.Info("Move {a} to {b}", pluginPath, $"{pluginPath}.disabled");

            if (File.Exists($"{pluginPath}.disabled"))
            {
                await commandEvent.Sender.ReplyAsync($"{pluginPath}.disabled 已存在，禁用插件失败");
                return true;
            }

            File.Move(pluginPath, $"{pluginPath}.disabled");

            (Application as Application)?.Reload(false, false);

            await commandEvent.Sender.ReplyAsync($"已卸载插件");
            return true;
        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="commandEvent"></param>
        /// <returns></returns>
        public async Task<bool> OnEnableCommand(MiraiBot bot, CommandEventArgs commandEvent)
        {
            commandEvent.Sender.RequireCommandPermission(this, "core.admin.enableplugin");

            var dllName = commandEvent.GetNextArgs<string>();
            var pluginPath = (Application as Application)?.PluginPath!;

            var fullPath = Path.Combine(pluginPath, $"{dllName}");
            var fullPathDisabled = Path.Combine(pluginPath, $"{dllName}.disabled");

            if (File.Exists(fullPath))
            {
                await commandEvent.Sender.ReplyAsync($"插件已启动");
                return true;
            }

            if (!File.Exists(fullPathDisabled))
            {
                await commandEvent.Sender.ReplyAsync($"插件不存在");
                return true;
            }

            Logger.Info("Move {a} to {b}", fullPathDisabled, fullPath);
            File.Move(fullPathDisabled, fullPath);

            (Application as Application)?.Reload(false, false);

            await commandEvent.Sender.ReplyAsync($"已加载插件");
            return true;
        }
    }
}