using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Api;

namespace WanBot.Plugin.Essential.Extension
{
    /// <summary>
    /// 计划任务
    /// </summary>
    public class Scheduler
    {
        private ILogger _logger;

        public Scheduler(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建计划任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeSpan"></param>
        public void Run(Action action, TimeSpan timeSpan)
        {
            StartTimer(action, (int)timeSpan.TotalMilliseconds);
        }

        private void StartTimer(Action action, int time)
        {
            var t = new Timer((obj) =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    _logger.Error("Error in timer: {e}", e);
                }
                finally
                {
                    var t = obj as Timer;
                    t?.Dispose();
                }
            });
            t.Change(time, 0);
        }
    }
}
