using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Cheng.Algorithm.HashCodes;

namespace Cheng.GameTemplates.ERA
{

    /// <summary>
    /// ERA纪年时间刻
    /// </summary>
    [Serializable] public unsafe struct ERADateTime : IEquatable<ERADateTime>, IComparable<ERADateTime>, IHashCode64
    {

        #region 介绍

        /*
        纪元历
        单位：
        秒(tick),分,时,日,月,年

        1 秒 == 1 tick;
        1 分 == 60 秒;
        1 时 == 60 分;
        1 日 == 24 时;
        1 月 == 30 日;
        1 年 == 12 月;
        1 纪元 == 100 年;
         */

        #endregion

        #region 构造

        /// <summary>
        /// 初始化日期时间刻，使用基础计量单位tick
        /// </summary>
        /// <param name="tick">基础计量单位</param>
        public ERADateTime(ulong tick)
        {
            this.tick = tick;
        }

        /// <summary>
        /// 初始化日期时间刻
        /// </summary>
        /// <param name="sec">秒数</param>
        /// <param name="min">分钟数</param>
        /// <param name="hour">时</param>
        /// <param name="day">日</param>
        /// <param name="month">月</param>
        /// <param name="year">年</param>
        /// <param name="era">纪元</param>
        /// <exception cref="ArgumentException">有参数超出范围</exception>
        public ERADateTime(uint sec, uint min, uint hour, uint day, uint month, uint year, uint era)
        {
            if ((sec >= cp_SecToMin) || (min >= cp_MinToHour) || (hour >= cp_HourToDay) || (day >= cp_DayToMonth) || (month >= cp_MonthToYear))
            {
                throw new ArgumentException("有参数超出范围");
            }

            tick = sec;
            tick += (min * cp_SecToMin);
            tick += (hour * cp_OneHourToSec);
            tick += (day * cp_OneDayToSec);
            tick += (month * cp_OneMonthToSec);
            tick += (year * cp_OneYearToSec);
            tick += (era * cp_OneEraToSec);
        }

        /// <summary>
        /// 初始化日期时间刻
        /// </summary>
        /// <param name="sec">秒数</param>
        /// <param name="min">分钟数</param>
        /// <param name="hour">时</param>
        /// <param name="day">日</param>
        /// <param name="month">月</param>
        /// <param name="year">年</param>
        /// <exception cref="ArgumentException">有参数超出范围</exception>
        public ERADateTime(uint sec, uint min, uint hour, uint day, uint month, uint year)
        {
            if ((sec >= cp_SecToMin) || (min >= cp_MinToHour) || (hour >= cp_HourToDay) || (day >= cp_DayToMonth) || (month >= cp_MonthToYear))
            {
                throw new ArgumentException("有参数超出范围");
            }

            tick = sec;
            tick += (min * cp_SecToMin);
            tick += (hour * cp_OneHourToSec);
            tick += (day * cp_OneDayToSec);
            tick += (month * cp_OneMonthToSec);
            tick += (year * cp_OneYearToSec);
        }

        #endregion

        #region 参数

        #region 常量

        /// <summary>
        /// 1分的秒数
        /// </summary>
        public const uint cp_SecToMin = 60;

        /// <summary>
        /// 1时的分钟数
        /// </summary>
        public const uint cp_MinToHour = 60;

        /// <summary>
        /// 1日的时数
        /// </summary>
        public const uint cp_HourToDay = 24;

        /// <summary>
        /// 1个月的天数
        /// </summary>
        public const uint cp_DayToMonth = 30;

        /// <summary>
        /// 1年的月数
        /// </summary>
        public const uint cp_MonthToYear = 12;

        /// <summary>
        /// 一个纪元的年数
        /// </summary>
        public const uint cp_YearToEra = 100;

        /// <summary>
        /// 1时的秒数
        /// </summary>
        public const uint cp_OneHourToSec = cp_SecToMin * cp_MinToHour;

        /// <summary>
        /// 1日的秒数
        /// </summary>
        public const ulong cp_OneDayToSec = cp_OneHourToSec * cp_HourToDay;

        /// <summary>
        /// 1月的秒数
        /// </summary>
        public const ulong cp_OneMonthToSec = cp_OneDayToSec * cp_DayToMonth;

        /// <summary>
        /// 1年的秒数
        /// </summary>
        public const ulong cp_OneYearToSec = cp_OneMonthToSec * cp_MonthToYear;

        /// <summary>
        /// 1纪元的秒数
        /// </summary>
        public const ulong cp_OneEraToSec = cp_OneYearToSec * cp_YearToEra;
        #endregion

        #region 实例

        /// <summary>
        /// tick值（秒数）
        /// </summary>
        public readonly ulong tick;
        
        #endregion

        #endregion

        #region 功能

        #region 访问

        #endregion

        #region 日期转换

        /// <summary>
        /// 时间的秒数，范围在[0,60)
        /// </summary>
        public int Second
        {
            get => (int)(tick % cp_SecToMin);
        }

        /// <summary>
        /// 时间的分，范围在[0,60)
        /// </summary>
        public int Minute
        {
            get => (int)((tick / cp_SecToMin) % cp_MinToHour);
        }

        /// <summary>
        /// 时间的时，范围在[0,24)
        /// </summary>
        public int Hour
        {
            get => (int)((tick / cp_OneHourToSec) % cp_HourToDay);
        }

        /// <summary>
        /// 时间的天，范围在[0,30)
        /// </summary>
        public int Day
        {
            get => (int)((tick / cp_OneDayToSec) % cp_DayToMonth);
        }

        /// <summary>
        /// 时间的月，范围在[0,12)
        /// </summary>
        public int Month
        {
            get => (int)((tick / cp_OneMonthToSec) % cp_MonthToYear);
        }

        /// <summary>
        /// 时间的年份
        /// </summary>
        public int Year
        {
            get => (int)((tick / cp_OneYearToSec));
        }

        /// <summary>
        /// 纪元内的年份，范围在[0,100)
        /// </summary>
        public int OnlyYear
        {
            get => (int)((tick / cp_OneYearToSec) % cp_YearToEra);
        }

        /// <summary>
        /// 时间的纪元
        /// </summary>
        public int Era
        {
            get => (int)((tick / cp_OneEraToSec));
        }

        #endregion

        #region 运算

        #region 运算符重载
        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick == t2.tick;
        }

        /// <summary>
        /// 比较是否不相等
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator !=(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick != t2.tick;
        }

        /// <summary>
        /// 添加一个时间间隔
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static ERADateTime operator +(ERADateTime t1, ERATimeSpan t2)
        {
            return new ERADateTime((ulong)((long)t1.tick + t2.tick));
        }

        /// <summary>
        /// 添加一个时间间隔
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static ERADateTime operator +(ERATimeSpan t1, ERADateTime t2)
        {
            return new ERADateTime((ulong)(t1.tick + (long)t2.tick));
        }

        /// <summary>
        /// 减去一个时间间隔
        /// </summary>
        /// <param name="time"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static ERADateTime operator -(ERADateTime time, ERATimeSpan span)
        {
            return new ERADateTime((ulong)((long)time.tick - span.tick));
        }

        /// <summary>
        /// 将两个时间刻相减返回时间间隔
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static ERATimeSpan operator -(ERADateTime t1, ERADateTime t2)
        {
            return new ERATimeSpan((long)t1.tick - (long)t2.tick);
        }

        public static bool operator <(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick < t2.tick;
        }

        public static bool operator >(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick > t2.tick;
        }

        public static bool operator <=(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick <= t2.tick;
        }

        public static bool operator >=(ERADateTime t1, ERADateTime t2)
        {
            return t1.tick >= t2.tick;
        }
        #endregion

        #region 方法

        /// <summary>
        /// 返回一个添加时间间隔的新实例
        /// </summary>
        /// <param name="span">要添加的时间间隔</param>
        /// <returns>新时间刻</returns>
        public ERADateTime AddSpan(ERATimeSpan span)
        {
            return new ERADateTime(tick + (ulong)span.tick);
        }

        /// <summary>
        /// 返回一个减去时间间隔的新实例
        /// </summary>
        /// <param name="span">要减去的时间间隔</param>
        /// <returns>新时间刻</returns>
        public ERADateTime SubSpan(ERATimeSpan span)
        {
            return new ERADateTime(tick - (ulong)span.tick);
        }

        /// <summary>
        /// 返回此实例与另一个时间刻的时间间隔
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public ERATimeSpan SubDateTime(ERADateTime span)
        {
            return new ERATimeSpan((long)tick - (long)span.tick);
        }

        #endregion

        #endregion

        #region 派生

        public bool Equals(ERADateTime other)
        {
            return tick == other.tick;
        }

        public override bool Equals(object obj)
        {
            if(obj is ERADateTime t) return tick == t.tick;

            return false;
        }

        public override int GetHashCode()
        {
            return tick.GetHashCode();
        }

        public int CompareTo(ERADateTime other)
        {
            return (tick < other.tick ? -1 : (tick == other.tick ? 0 : 1));
        }

        public long GetHashCode64()
        {
            return (long)tick;
        }

        /// <summary>
        /// 返回此实例的时间
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Year}:{Month}:{Day}:{Hour}:{Minute}:{Second}";
        }

        #endregion

        #endregion

    }


}
