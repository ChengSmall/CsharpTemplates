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
        protected override TimeSpan SingleTimeToTimeSpan(double elapsed)
        {
            return TimeSpan.FromSeconds(elapsed);
        }
        protected sealed override double TimeSpanToDouble(TimeSpan span)
        {
            return span.TotalSeconds;
        }
    }

}
