using Cheng.Algorithm.HashCodes;
using System;
using System.Collections.Generic;

namespace Cheng.GameTemplates.ERA
{

    /// <summary>
    /// ERA纪年时间间隔
    /// </summary>
    public struct ERATimeSpan : IHashCode64, IEquatable<ERATimeSpan>, IComparable<ERATimeSpan>
    {

        #region 注释

        #endregion

        #region 构造
        
        /// <summary>
        /// 初始化一个ERA时间间隔
        /// </summary>
        /// <param name="tick"></param>
        public ERATimeSpan(long tick)
        {
            this.tick = tick;
        }

        /// <summary>
        /// 初始化一个时间间隔
        /// </summary>
        /// <param name="sec">秒</param>
        /// <param name="min">分</param>
        /// <param name="hour">时</param>
        /// <param name="day">天数</param>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public ERATimeSpan(int sec, int min, int hour, long day)
        {
            if ((sec <= -ERADateTime.cp_SecToMin || sec >= ERADateTime.cp_SecToMin) || (min <= -ERADateTime.cp_MinToHour || min >= ERADateTime.cp_MinToHour) || (hour <= -ERADateTime.cp_HourToDay || hour >= ERADateTime.cp_HourToDay))
            {
                throw new ArgumentException("参数超出范围");
            }

            tick = sec + (min * (int)ERADateTime.cp_SecToMin) + (hour * (int)ERADateTime.cp_OneHourToSec);
            tick += day * (long)ERADateTime.cp_OneDayToSec;
        }

        #endregion

        #region 参数

        #region 常量

        /// <summary>
        /// 表示0的时间间隔
        /// </summary>
        public static readonly ERATimeSpan Zero = new ERATimeSpan(0);

        #endregion

        #region 实例

        /// <summary>
        /// 基础时间间隔单位（秒）
        /// </summary>
        public readonly long tick;

        #endregion

        #endregion

        #region 功能

        #region 访问

        /// <summary>
        /// 此实例的秒部分
        /// </summary>
        public int Second
        {
            get => (int)((tick) % ERADateTime.cp_SecToMin);
        }

        /// <summary>
        /// 此实例的分
        /// </summary>
        public int Minute
        {
            get => (int)((tick / ERADateTime.cp_SecToMin) % ERADateTime.cp_MinToHour);
        }

        /// <summary>
        /// 此实例的小时
        /// </summary>
        public int Hour
        {
            get => (int)((tick / ERADateTime.cp_OneHourToSec) % ERADateTime.cp_HourToDay);
        }

        /// <summary>
        /// 此实例的天数
        /// </summary>
        public long Day
        {
            get => ((tick / (long)ERADateTime.cp_OneDayToSec));
        }

        /// <summary>
        /// 返回以“分”为单位整数的浮点数
        /// </summary>
        public double TotalMinutes
        {
            get
            {
                return (double)tick / ERADateTime.cp_SecToMin;
            }
        }

        /// <summary>
        /// 返回以“时”为单位整数的浮点数
        /// </summary>
        public double TotalHours
        {
            get => (double)tick / ERADateTime.cp_OneHourToSec;
        }

        /// <summary>
        /// 返回以“天”为单位整数的浮点数
        /// </summary>
        public double TotalDays
        {
            get => (double)tick / ERADateTime.cp_OneDayToSec;
        }

        #endregion

        #region 运算

        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static ERATimeSpan operator +(ERATimeSpan t1, ERATimeSpan t2)
        {
            return new ERATimeSpan(t1.tick + t2.tick);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="t1">被减数</param>
        /// <param name="t2">减数</param>
        /// <returns></returns>
        public static ERATimeSpan operator -(ERATimeSpan t1, ERATimeSpan t2)
        {
            return new ERATimeSpan(t1.tick - t2.tick);
        }


        public static bool operator ==(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick == t2.tick;
        }


        public static bool operator !=(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick != t2.tick;
        }


        public static bool operator <(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick < t2.tick;
        }


        public static bool operator >(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick > t2.tick;
        }


        public static bool operator <=(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick <= t2.tick;
        }


        public static bool operator >=(ERATimeSpan t1, ERATimeSpan t2)
        {
            return t1.tick >= t2.tick;
        }
        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            if (obj is ERATimeSpan t) return tick == t.tick;
            return false;
        }

        public override int GetHashCode()
        {
            return tick.GetHashCode();
        }

        public bool Equals(ERATimeSpan other)
        {
            return tick == other.tick;
        }

        public int CompareTo(ERATimeSpan other)
        {
            return (tick < other.tick ? -1 : (tick == other.tick ? 0 : 1));
        }

        public long GetHashCode64()
        {
            return tick;
        }

        /// <summary>
        /// 返回时间间隔
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Day}:{Hour}:{Minute}:{Second}";
        }

        #endregion

        #endregion

    }

}
