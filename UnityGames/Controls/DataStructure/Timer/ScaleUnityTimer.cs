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
        protected sealed override TimeSpan SingleTimeToTimeSpan(double elapsed)
        {
            return TimeSpan.FromSeconds(elapsed);
        }
        protected sealed override double TimeSpanToDouble(TimeSpan span)
        {
            return span.TotalSeconds;
        }
    }

}
