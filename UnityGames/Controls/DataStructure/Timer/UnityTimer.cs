using System;
using UnityEngine;

namespace Cheng.Timers.Unitys
{

    /// <summary>
    /// Unity计时器时间步进方式
    /// </summary>
    public enum UnityTimerType
    {

        /// <summary>
        /// 使用<see cref="UnityEngine.Time.time"/>作为时间步进单位
        /// </summary>
        time,

        /// <summary>
        /// 使用<see cref="UnityEngine.Time.unscaledTime"/>时间作为时间步进单位
        /// </summary>
        unscaledTime,

        /// <summary>
        /// 使用<see cref="UnityEngine.Time.fixedTime"/>时间作为时间步进单位
        /// </summary>
        fixedTime,

        /// <summary>
        /// 使用<see cref="UnityEngine.Time.realtimeSinceStartup"/>时间作为步进单位
        /// </summary>
        realtimeSinceStartup
    }

    /// <summary>
    /// Unity通用计时器
    /// </summary>
    [Serializable]
    public sealed class UnityTimer : SingleTimer
    {

        /// <summary>
        /// 实例化一个unity计时器
        /// </summary>
        public UnityTimer() : this(UnityTimerType.time)
        {
        }

        /// <summary>
        /// 实例化一个unity计时器，指定计时时间方式
        /// </summary>
        /// <param name="type">计时时间步进方式，该参数默认为<see cref="UnityTimerType.time"/></param>
        public UnityTimer(UnityTimerType type)
        {
            if (type < UnityTimerType.time || type > UnityTimerType.realtimeSinceStartup) throw new ArgumentException();
            this.type = type;
        }

        [SerializeField] private UnityTimerType type;

        /// <summary>
        /// 访问或设置计时器步进方式
        /// </summary>
        /// <remarks>当设置为新的时间步进方式时，之前记录的时间不会改变</remarks>
        /// <exception cref="ArgumentOutOfRangeException">设置的值不是预定的枚举量</exception>
        public UnityTimerType TimeType
        {
            get => type;
            set
            {
                if (type == value) return;
                if (value < UnityTimerType.time || value > UnityTimerType.realtimeSinceStartup) throw new ArgumentOutOfRangeException();
                Stop();
                type = value;
                Start();
            }
        }

        protected sealed override double NowTime
        {
            get
            {
                switch (type)
                {
                    case UnityTimerType.time:
                        return Time.time;
                    case UnityTimerType.unscaledTime:
                        return Time.unscaledTime;
                    case UnityTimerType.fixedTime:
                        return Time.fixedTime;
                    case UnityTimerType.realtimeSinceStartup:
                        return Time.realtimeSinceStartup;
                }
                throw new ArgumentException();
            }
        }

        protected sealed override TimeSpan SingleTimeToTimeSpan(double elapsed)
        {
            return TimeSpan.FromSeconds(elapsed);
        }
        protected sealed override double TimeSpanToDouble(TimeSpan span)
        {
            return span.TotalSeconds;
        }

#if UNITY_EDITOR

        public const string fieldName_type = nameof(type);

#endif

    }

}
