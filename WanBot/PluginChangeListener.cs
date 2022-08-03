using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot
{
    /// <summary>
    /// 插件变动监听器
    /// </summary>
    internal class PluginChangeListener : IDisposable
    {
        public event Action<string>? OnPluginChange;

        private FileSystemWatcher _fileSystemWatcher;

        public PluginChangeListener(string pluginDir)
        {
            _fileSystemWatcher = new(pluginDir);
            _fileSystemWatcher.Path = pluginDir;
            _fileSystemWatcher.Created += OnChanged;
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Deleted += OnChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object s, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".dll", true, null))
                OnPluginChange?.Invoke(e.FullPath);
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
