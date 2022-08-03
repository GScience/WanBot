namespace WanBot.Api
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public interface IApplication
    {
        IBotManager BotManager { get; }

        IPluginManager PluginManager { get; }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ReadConfig<T>(string pluginName) where T : new();

        /// <summary>
        /// 获取配置目录
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        string GetConfigPath(string pluginName);
    }
}