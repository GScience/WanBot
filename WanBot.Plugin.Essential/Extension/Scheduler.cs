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
    public class Scheduler : IDisposable
    {
        private ILogger _logger;

        private List<Timer> _loopTimers = new();

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

        /// <summary>
        /// 创建重复计划任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dueTime"></param>
        /// <param name="period"></param>
        public void RunLoop(Action action, TimeSpan dueTime, TimeSpan period)
        {
            StartTimerLoop(action, (int)dueTime.TotalMilliseconds, (int)period.TotalMilliseconds);
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

        private void StartTimerLoop(Action action, int dueTime, int period)
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
            });
            t.Change(0, period);
            _loopTimers.Add(t);
        }

        public void Dispose()
        {
            foreach (var t in _loopTimers)
                t.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
