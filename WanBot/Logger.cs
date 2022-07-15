using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot
{
    public class Logger : Api.ILogger
    {
        private const string DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{Category}] {Message:lj}{NewLine}{Exception}";
        private static readonly Serilog.Core.Logger _globalLogger;

        static Logger()
        {
            _globalLogger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.Console(outputTemplate: DefaultConsoleOutputTemplate)
              .CreateLogger();
        }

        private string _category;
        private Serilog.ILogger _logger;

        internal Logger(string category)
        {
            _category = category;
            _logger = _globalLogger.ForContext("Category", _category);
        }

        public Logger CreateSubLogger(string subCategory)
        {
            return new Logger($"_category.{subCategory}");
        }

        public void Error(string message, params object?[]? args)
        {
            _logger.Error(message, args);
        }

        public void Fatal(string message, params object?[]? args)
        {
            _logger.Fatal(message, args);
        }

        public void Info(string message, params object?[]? args)
        {
            _logger.Information(message, args);
        }

        public void Warn(string message, params object?[]? args)
        {
            _logger.Warning(message, args);
        }
    }
}
