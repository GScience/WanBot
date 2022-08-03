using WanBot;
using WanBot.Api.Mirai;

using var application = new Application();

var logger = new Logger("Launcher");
logger.Info("WanBot By GScience Studio Ver {WanBotVersion} (Api {WanBotApiVersion})",
    typeof(Application).Assembly.GetName().Version,
    typeof(MiraiBot).Assembly.GetName().Version);
logger.Info(
    "System info: \n" +
    "\t.net version: {netVersion}\n" +
    "\tOS version: {osVersion}",
    Environment.Version,
    Environment.OSVersion);

logger.Info(Environment.CommandLine);

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
    logger.Fatal(
        $"============================\n" +
        $"=         CRASH!!!         =\n" +
        $"============================\n" +
        $"Unknown exception: \n{e}");
}