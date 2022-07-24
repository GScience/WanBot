using WanBot;
using WanBot.Api.Message;

using var application = new Application();

for (var i = 0; i < args.Length; i++)
{
    if (args[i].ToLower() == "-config")
        application.ConfigPath = args[++i];

    if (args[i].ToLower() == "-plugin")
        application.PluginPath = args[++i];
}

try
{
    application.Start();
}
catch (Exception e)
{
    var logger = new Logger("CRASH!!!");
    logger.Fatal($"Unknown exception {e}");
}