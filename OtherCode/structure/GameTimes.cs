using Cheng.Algorithm.HashCodes;
using System;

namespace Cheng.DataStructure
{
    /// <summary>
    /// 表示游戏中的时间结构
    /// </summary>
    public struct TrirrengTime : IEquatable<TrirrengTime>, IHashCode64, IComparable<TrirrengTime>
    {

        #region 构造
        public TrirrengTime(ulong tick)
        {
            p_tick = tick;
        }
        /// <summary>
        /// 实例化一个泰拉时间
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="yue">月份，范围在[0,10)</param>
        /// <param name="day">日分，范围在[0,50)</param>
        /// <param name="hour">小时分,范围在[0,24)</param>
        /// <param name="min">分钟，范围在[0,60)</param>
        /// <param name="ms">秒，范围在[0,60)</param>
        /// <exception cref="ArgumentNullException">超出范围</exception>
        public TrirrengTime(int year, int yue, int day, int hour, int min, int ms)
        {
            if (ms < 0 || ms >= cp_MStoMin || min < 0 || min >= cp_MinToHour || hour < 0 || hour >= cp_HourToDay || day < 0 || day >= cp_DayToYue || yue < 0 || yue >= cp_YueToYear) throw new ArgumentOutOfRangeException();

            p_tick = (((uint)ms) * cp_oneMS);

            p_tick += (((uint)min) * cp_oneMin);

            p_tick += (((uint)hour) * cp_oneHour);

            p_tick += (((uint)day) * cp_oneDay);

            p_tick += ((uint)yue) * cp_oneYue;

            p_tick += (((uint)year) * cp_oneYear);
        }
        /// <summary>
        /// 实例化一个泰拉时间
        /// </summary>
        /// <param name="day">日分，范围在[0,50)</param>
        /// <param name="hour">小时分,范围在[0,24)</param>
        /// <param name="min">分钟，范围在[0,60)</param>
        /// <param name="ms">秒，范围在[0,60)</param>
        /// <exception cref="ArgumentNullException">超出范围</exception>
        public TrirrengTime(int day, int hour, int min, int ms) : this(0, 0, day, hour, min, ms)
        {
        }
        /// <summary>
        /// 实例化一个泰拉时间
        /// </summary>
        /// <param name="hour">小时分,范围在[0,24)</param>
        /// <param name="min">分钟，范围在[0,60)</param>
        /// <param name="ms">秒，范围在[0,60)</param>
        /// <exception cref="ArgumentNullException">超出范围</exception>
        public TrirrengTime(int hour, int min, int ms) : this(0, 0, 0, hour, min, ms)
        {
        }
        #endregion

        #region 参数

        #region 静态常量
        /// <summary>
        /// 1秒的tick
        /// </summary>
        internal const uint cp_tickToMsMul = 1;
        /// <summary>
        /// 1分的秒数
        /// </summary>
        internal const uint cp_MStoMin = 60;
        /// <summary>
        /// 1小时的分钟数
        /// </summary>
        internal const uint cp_MinToHour = 60;
        /// <summary>
        /// 1天的小时数
        /// </summary>
        internal const uint cp_HourToDay = 24;
        /// <summary>
        /// 1月的天数
        /// </summary>
        internal const uint cp_DayToYue = 50;
        /// <summary>
        /// 1年的月数
        /// </summary>
        internal const uint cp_YueToYear = 10;

        /// <summary>
        /// 1秒tick量
        /// </summary>
        internal const ulong cp_oneMS = cp_tickToMsMul;
        /// <summary>
        /// 1分tick量
        /// </summary>
        internal const ulong cp_oneMin = cp_MStoMin * cp_oneMS;
        /// <summary>
        /// 1小时tick量
        /// </summary>
        internal const ulong cp_oneHour = cp_oneMin * cp_MinToHour;
        /// <summary>
        /// 1天tick量
        /// </summary>
        internal const ulong cp_oneDay = cp_oneHour * cp_HourToDay;
        /// <summary>
        /// 1个月tick量
        /// </summary>
        internal const ulong cp_oneYue = cp_oneDay * cp_DayToYue;
        /// <summary>
        /// 1年tick量
        /// </summary>
        internal const ulong cp_oneYear = cp_oneYue * cp_YueToYear;
        /// <summary>
        /// 表示0或默认值的泰拉时间
        /// </summary>
        public static readonly TrirrengTime Zero = new TrirrengTime();

        #endregion

        #region 成员变量

        private ulong p_tick;

        #endregion

        #endregion

        #region 属性参数
        /// <summary>
        /// 获取泰拉时间实例的tick
        /// </summary>
        public ulong Tick
        {
            get => p_tick;
        }

        /// <summary>
        /// 返回秒时
        /// </summary>
        public int MS
        {
            get
            {
                return (int)((p_tick % cp_oneMin) / cp_tickToMsMul);
            }
        }
        /// <summary>
        /// 返回分钟时
        /// </summary>
        public int Min
        {
            get
            {
                return (int)((p_tick % cp_oneHour) / cp_MStoMin);
            }
        }
        /// <summary>
        /// 返回小时
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)((p_tick % cp_oneDay) / (cp_oneHour));
            }
        }
        /// <summary>
        /// 返回天数
        /// </summary>
        public int Day
        {
            get
            {
                return (int)((p_tick % cp_oneYue) / (cp_oneDay));
            }
        }
        /// <summary>
        /// 返回月份
        /// </summary>
        public int Yue
        {
            get => (int)((p_tick % cp_oneYear) / (cp_oneYue));
        }
        /// <summary>
        /// 返回年份
        /// </summary>
        public int Year
        {
            get => (int)((p_tick) / (cp_oneYear));
        }

        #endregion

        #region 功能

        #region 运算

        #region 运算符重载

        public static bool operator ==(TrirrengTime t1, TrirrengTime t2)
        {
            return t1.p_tick == t2.p_tick;
        }
        public static bool operator !=(TrirrengTime t1, TrirrengTime t2)
        {
            return t1.p_tick != t2.p_tick;
        }

        public static TrirrengTime operator +(TrirrengTime time, TrirrengSpan span)
        {
            return new TrirrengTime(time.p_tick + span.p_tick);
        }
        public static TrirrengTime operator -(TrirrengTime time, TrirrengSpan span)
        {
            return new TrirrengTime(time.p_tick - span.p_tick);
        }
        public static TrirrengSpan operator -(TrirrengTime t1, TrirrengTime t2)
        {
            if (t2.p_tick > t1.p_tick) return new TrirrengSpan();
            return new TrirrengSpan(t1.p_tick - t2.p_tick);
        }

        public static bool operator <(TrirrengTime left, TrirrengTime right)
        {
            return left.p_tick < right.p_tick;
        }

        public static bool operator <=(TrirrengTime left, TrirrengTime right)
        {
            return left.p_tick <= right.p_tick;
        }

        public static bool operator >(TrirrengTime left, TrirrengTime right)
        {
            return left.p_tick > right.p_tick;
        }

        public static bool operator >=(TrirrengTime left, TrirrengTime right)
        {
            return left.p_tick >= right.p_tick;
        }

        #endregion

        #endregion

        #region 派生

        public bool Equals(TrirrengTime other)
        {
            return p_tick == other.p_tick;
        }
        public override bool Equals(object obj)
        {
            if(obj is TrirrengTime)
            {
                return Equals((TrirrengTime)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return p_tick.GetHashCode();
        }
        public long GetHashCode64()
        {
            return (long)p_tick;
        }
        public int CompareTo(TrirrengTime other)
        {
            return p_tick < other.p_tick ? -1 : (p_tick > other.p_tick ? 1 : 0);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 表示游戏中的时间量
    /// </summary>
    public struct TrirrengSpan : IEquatable<TrirrengSpan>, IHashCode64, IComparable<TrirrengSpan>
    {

        #region 构造
        public TrirrengSpan(ulong tick)
        {
            p_tick = tick;
        }

        public TrirrengSpan(int day, int hour, int min, int ms)
        {
            if (ms < 0 || ms >= TrirrengTime.cp_MStoMin || min < 0 || min >= TrirrengTime.cp_MinToHour || hour < 0 || hour >= TrirrengTime.cp_HourToDay || day < 0 || day >= TrirrengTime.cp_DayToYue) throw new ArgumentOutOfRangeException();

            p_tick = (((uint)ms) * TrirrengTime.cp_oneMS);

            p_tick += (((uint)min) * TrirrengTime.cp_oneMin);

            p_tick += (((uint)hour) * TrirrengTime.cp_oneHour);

            p_tick += (((uint)day) * TrirrengTime.cp_oneDay);
        }
        #endregion

        #region 参数

        internal ulong p_tick;

        #endregion

        #region 属性参数
        /// <summary>
        /// 获取泰拉时间间隔
        /// </summary>
        public ulong Tick
        {
            get => p_tick;
        }
        /// <summary>
        /// 返回秒
        /// </summary>
        public int MS
        {
            get
            {
                return (int)((p_tick % TrirrengTime.cp_oneMin) / TrirrengTime.cp_tickToMsMul);
            }
        }
        /// <summary>
        /// 返回分钟
        /// </summary>
        public int Min
        {
            get
            {
                return (int)((p_tick % TrirrengTime.cp_oneHour) / TrirrengTime.cp_MStoMin);
            }
        }
        /// <summary>
        /// 返回小时
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)((p_tick % TrirrengTime.cp_oneDay) / (TrirrengTime.cp_oneHour));
            }
        }
        /// <summary>
        /// 返回天数
        /// </summary>
        public long Day
        {
            get
            {
                return (long)(p_tick / TrirrengTime.cp_oneDay);
            }
        }


        #endregion

        #region 功能

        #region 转化
        /// <summary>
        /// 转化泰拉秒数到泰拉时间量
        /// </summary>
        /// <param name="second">泰拉秒数</param>
        /// <returns>泰拉时间量</returns>
        public TrirrengSpan FromSeconds(double second)
        {
            if(second < 0 || second > 18446744073709551615d)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new TrirrengSpan((ulong)(second * TrirrengTime.cp_oneMS));
        }
        /// <summary>
        /// 转化泰拉小时数到泰拉时间
        /// </summary>
        /// <param name="hour">泰拉小时数</param>
        /// <returns>泰拉时间量</returns>
        public TrirrengSpan FormHours(double hour)
        {
            if (hour < 0 || hour > (18446744073709551615d / TrirrengTime.cp_oneHour))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new TrirrengSpan((ulong)(hour * TrirrengTime.cp_oneHour));
        }
        /// <summary>
        /// 转化泰拉天数到泰拉时间
        /// </summary>
        /// <param name="day">泰拉天数</param>
        /// <returns>泰拉时间量</returns>
        public TrirrengSpan FormDays(double day)
        {
            if (day < 0 || day > (18446744073709551615d / TrirrengTime.cp_oneDay))
            {
                throw new ArgumentOutOfRangeException();
            }

            return new TrirrengSpan((ulong)(day * TrirrengTime.cp_oneDay));
        }

        #endregion

        #region 运算符重载

        public static bool operator ==(TrirrengSpan t1, TrirrengSpan t2)
        {
            return t1.p_tick == t2.p_tick;
        }

        public static bool operator !=(TrirrengSpan t1, TrirrengSpan t2)
        {
            return t1.p_tick != t2.p_tick;
        }

        public static TrirrengSpan operator +(TrirrengSpan t1, TrirrengSpan t2)
        {
            return new TrirrengSpan(t1.p_tick + t2.p_tick);
        }
        public static TrirrengSpan operator -(TrirrengSpan t1, TrirrengSpan t2)
        {
            return new TrirrengSpan(t1.p_tick - t2.p_tick);
        }

        public static bool operator <(TrirrengSpan left, TrirrengSpan right)
        {
            return left.p_tick < right.p_tick;
        }

        public static bool operator <=(TrirrengSpan left, TrirrengSpan right)
        {
            return left.p_tick <= right.p_tick;
        }

        public static bool operator >(TrirrengSpan left, TrirrengSpan right)
        {
            return left.p_tick > right.p_tick;
        }

        public static bool operator >=(TrirrengSpan left, TrirrengSpan right)
        {
            return left.p_tick >= right.p_tick;
        }


        #endregion

        #region 派生
        public bool Equals(TrirrengSpan other)
        {
            return p_tick == other.p_tick;
        }
        public long GetHashCode64()
        {
            return p_tick.GetHashCode64();
        }
        public int CompareTo(TrirrengSpan other)
        {
            return p_tick < other.p_tick ? -1 : (p_tick > other.p_tick ? 1 : 0);
        }
        public override bool Equals(object obj)
        {
            if(obj is TrirrengSpan)
            {
                return Equals((TrirrengSpan)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return p_tick.GetHashCode();
        }


        #endregion

        #endregion

    }

}
