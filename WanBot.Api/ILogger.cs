using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Api
{
    /// <summary>
    /// 日志
    /// </summary>
    public interface ILogger
    {
        public void Info(string message, params object?[]? args);
        public void Warn(string message, params object?[]? args);
        public void Error(string message, params object?[]? args);
        public void Fatal(string message, params object?[]? args);
    }
}
