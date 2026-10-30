using System;

namespace Cheng.Timers.Unitys
{
    /// <summary>
    /// Unity无缩放时间计时器
    /// </summary>
    public sealed class NotScaleUnityTimer : SingleTimer
    {
        public NotScaleUnityTimer()
        {
        }

        protected sealed override double NowTime => UnityEngine.Time.unscaledTime;

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
