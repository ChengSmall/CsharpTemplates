using System;

namespace Cheng.Timers.Unitys
{

    /// <summary>
    /// Unity有缩放时间计时器
    /// </summary>
    public class ScaleUnityTimer : SingleTimer
    {
        public ScaleUnityTimer()
        {
        }
        protected sealed override double NowTime => UnityEngine.Time.time;

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
