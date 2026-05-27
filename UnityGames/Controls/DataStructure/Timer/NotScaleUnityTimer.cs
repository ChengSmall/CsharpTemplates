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

        protected sealed override double TimeSpanToDouble(TimeSpan span)
        {
            return span.TotalSeconds;
        }

        protected sealed override TimeSpan SingleTimeToTimeSpan(double elapsed)
        {
            return TimeSpan.FromSeconds(elapsed);
        }

    }

}
