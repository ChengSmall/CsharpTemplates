using System;

namespace Cheng.Timers.Unitys
{

    /// <summary>
    /// Unity真实时间计时器；使用 <see cref="UnityEngine.Time.realtimeSinceStartup"/> 参数作为时间源
    /// </summary>
    public sealed class UnityRealTimer : SingleTimer
    {
        public UnityRealTimer()
        {
        }

        protected override double NowTime => UnityEngine.Time.realtimeSinceStartup;

        protected override bool SingleTimeToTimeSpan(double elapsed, out TimeSpan span)
        {
            span = TimeSpan.FromSeconds(elapsed);
            return true;
        }

        protected override bool TimeSpanToDouble(TimeSpan span, out double tick)
        {
            tick = span.TotalSeconds;
            return true;
        }
    }

}
