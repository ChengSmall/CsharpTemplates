using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Cheng.DataStructure;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Windows
{

    /// <summary>
    /// 系统内存的状态信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatus
    {

        #region 参数

        internal uint dwLength;
        internal uint dwMemoryLoad;
        internal ulong ullTotalPhys;
        internal ulong ullAvailPhys;
        internal ulong ullTotalPageFile;
        internal ulong ullAvailPageFile;
        internal ulong ullTotalVirtual;
        internal ulong ullAvailVirtual;
        internal ulong ullAvailExtendedVirtual;

        #endregion

        #region 参数

        /// <summary>
        /// 实际物理内存量（以字节为单位）
        /// </summary>
        public ulong TotalPhysicsMemorySize
        {
            get => ullTotalPhys;
        }

        /// <summary>
        /// 当前可用的物理内存量（以字节为单位）
        /// </summary>
        public ulong AvailPhysicsMemorySize
        {
            get => ullAvailPhys;
        }

        /// <summary>
        /// 调用所在进程的虚拟地址空间，用户模式部分的大小（以字节为单位）
        /// </summary>
        public ulong TotalVirtual
        {
            get => ullTotalVirtual;
        }

        /// <summary>
        /// 调用所在进程的虚拟地址空间，用户模式部分中的未保留和未提交的内存量（以字节为单位）
        /// </summary>
        public ulong AvailVirtual
        {
            get => ullAvailVirtual;
        }

        /// <summary>
        /// 虚拟提交限制总大小
        /// </summary>
        /// <value>
        /// <para>
        /// 等于物理内存 + 页面文件(s) 的当前总大小<br/>
        /// 这是系统可以向所有进程承诺的最大虚拟内存总量，不是页面文件本身的大小
        /// </para>
        /// </value>
        public ulong TotalPageFile
        {
            get => ullTotalPageFile;
        }

        /// <summary>
        /// 当前可以提交的虚拟内存量
        /// </summary>
        /// <value>
        /// <para>代表在当前状态下，还能额外申请内存而不会出现“内存不足”错误的虚拟内存大小</para>
        /// </value>
        public ulong AvailPageFile
        {
            get => ullAvailPageFile;
        }

        /// <summary>
        /// 内存利用率
        /// </summary>
        /// <value>
        /// <para>返回一个内存利用率的近似值，范围在[0,100]的整数代表百分比</para>
        /// </value>
        public int UtilizationRate
        {
            get => (int)dwMemoryLoad;
        }

        #endregion

    }

    /// <summary>
    /// win32系统时间结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Win32SystemTime : IComparable<Win32SystemTime>, IEquatable<Win32SystemTime>, IHashCode64, IFormattable
    {

        #region 初始化

        /// <summary>
        /// 使用<see cref="DateTime"/>的时间参数转换到win32系统时间结构
        /// </summary>
        /// <param name="dateTime"></param>
        public Win32SystemTime(DateTime dateTime)
        {
            year = (ushort)dateTime.Year;
            month = (ushort)dateTime.Month;
            day = (ushort)dateTime.Day;
            hour = (ushort)dateTime.Hour;
            minute = (ushort)dateTime.Minute;
            second = (ushort)dateTime.Second;
            milliseconds = (ushort)dateTime.Millisecond;
            dayOfWeek = (ushort)dateTime.DayOfWeek;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 年份 有效值为 [1601,30827]
        /// </summary>
        public ushort year;

        /// <summary>
        /// 月份 1-12月，有效值为[1,12]
        /// </summary>
        public ushort month;

        /// <summary>
        /// 表示周几的参数; 1-6 对应周1-周6；0表示周末
        /// </summary>
        public ushort dayOfWeek;

        /// <summary>
        /// 每月的日期。 [1,31]
        /// </summary>
        public ushort day;

        /// <summary>
        /// 小时 此有效值为 [0,23]
        /// </summary>
        public ushort hour;

        /// <summary>
        /// 分钟 此有效值为 [0,59]
        /// </summary>
        public ushort minute;

        /// <summary>
        /// 秒 有效值为 [0,59]
        /// </summary>
        public ushort second;

        /// <summary>
        /// 毫秒 有效值为 [0,999]
        /// </summary>
        public ushort milliseconds;

        #endregion

        #region 功能

        #region 转化

        /// <summary>
        /// 将时间结构转换为<see cref="DateTime"/>
        /// </summary>
        /// <param name="kind">要转换的时间格式</param>
        /// <returns></returns>
        public DateTime ToDateTime(DateTimeKind kind)
        {
            return new DateTime(year, month, day, hour, minute, second, milliseconds, kind);
        }

        /// <summary>
        /// 将时间结构转换为<see cref="DateTime"/>
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return ToDateTime(DateTimeKind.Unspecified);
        }

        /// <summary>
        /// 转换到win32系统时间
        /// </summary>
        /// <param name="dateTime"></param>
        public static explicit operator Win32SystemTime(DateTime dateTime)
        {
            return new Win32SystemTime(dateTime);
        }

        /// <summary>
        /// 转换到.NET时间结构
        /// </summary>
        /// <param name="win32time"></param>
        public static explicit operator DateTime(Win32SystemTime win32time)
        {
            return win32time.ToDateTime(DateTimeKind.Unspecified);
        }

        #endregion

        #region 派生

        public int CompareTo(Win32SystemTime other)
        {
            var re = year - other.year; if (re != 0) return re;
            re = month - other.month; if (re != 0) return re;
            re = day - other.day; if (re != 0) return re;
            re = hour - other.hour; if (re != 0) return re;
            re = minute - other.minute; if (re != 0) return re;
            re = second - other.second; if (re != 0) return re;
            re = milliseconds - other.milliseconds;
            return re;
        }

        /// <summary>
        /// 比较相同
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) == 0;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) != 0;
        }

        public static bool operator <(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Win32SystemTime left, Win32SystemTime right)
        {
            return left.CompareTo(right) >= 0;
        }

        public bool Equals(Win32SystemTime other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Win32SystemTime other) return CompareTo(other) == 0;
            return false;
        }

        public long GetHashCode64()
        {
            var re = (((ulong)milliseconds)) |
            (((ulong)second) << 16) |
            (((ulong)minute) << (16 * 2)) |
            (((ulong)hour) << (16 * 3));

            var re2 = (((ulong)day)) |
            (((ulong)month) << 16) |
            (((ulong)year) << (16 * 2)) |
            (((ulong)dayOfWeek) << (16 * 3));

            return (long)(re ^ re2);
        }

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        /// <summary>
        /// 使用<see cref="DateTime.ToString"/>获取字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToDateTime().ToString();
        }

        /// <summary>
        /// 使用<see cref="DateTime.ToString(string, IFormatProvider)"/>获取字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToDateTime().ToString(format, formatProvider);
        }

        #endregion

        #endregion

    }

}
