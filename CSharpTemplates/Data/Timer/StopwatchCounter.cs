using System;
using System.Diagnostics;

namespace Cheng.Timers
{

    /// <summary>
    /// 使用<see cref="Stopwatch.GetTimestamp"/>获取刻度数的计时器
    /// </summary>
    public sealed class StopwatchCounter : TickTimeTimer
    {

        /// <summary>
        /// 实例化计时器
        /// </summary>
        public StopwatchCounter()
        {
        }

        /// <summary>
        /// 实例化计时器
        /// </summary>
        /// <param name="span">设置计时器已经经过的时间</param>
        public StopwatchCounter(TimeSpan span)
        {
            p_elapsed = (ulong)span.Ticks;
        }

        protected override ulong NowTimeTick
        {
            get
            {
                return (ulong)Stopwatch.GetTimestamp();
            }
        }

        protected override DateTime NowTime => new DateTime(Stopwatch.GetTimestamp());

    }


}
