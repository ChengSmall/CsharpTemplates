using Cheng.Algorithm.HashCodes;
using System;
using System.Runtime.InteropServices;

namespace Cheng.DataStructure.Cherrsdinates
{

    /// <summary>
    /// 整数二维坐标结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PointInt2 : IEquatable<PointInt2>, IHashCode64, IFormattable
    {

        #region 构造

        /// <summary>
        /// 初始化一个二维坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointInt2(int x, int y)
        {
            this.x = x; this.y = y;
        }

        #endregion

        #region 参数

        /// <summary>
        /// x值
        /// </summary>
        public readonly int x;

        /// <summary>
        /// y值
        /// </summary>
        public readonly int y;
        #endregion

        #region 功能

        #region 参数设置

        /// <summary>
        /// 返回一个x为新值的结构
        /// </summary>
        /// <param name="value">新的x</param>
        /// <returns></returns>
        public PointInt2 SetX(int value)
        {
            return new PointInt2(value, y);
        }

        /// <summary>
        /// 返回一个y为新值的结构
        /// </summary>
        /// <param name="value">新的y</param>
        /// <returns></returns>
        public PointInt2 SetY(int value)
        {
            return new PointInt2(x, value);
        }

        #endregion

        #region 转化

        /// <summary>
        /// 将坐标结构转化为64位整形值
        /// </summary>
        /// <returns></returns>
        public long ToInt64()
        {
            return (((uint)x) | (((long)((uint)y)) << 32));
        }

        /// <summary>
        /// 使用64位整形值初始化一个坐标结构
        /// </summary>
        /// <param name="value">64位整形值</param>
        /// <returns></returns>
        public static PointInt2 ToPoint(long value)
        {
            return new PointInt2((int)(value & 0xFFFFFFFF), (int)((ulong)value >> 32));
        }

        /// <summary>
        /// 将64位整型值转化为2维整形结构
        /// </summary>
        /// <param name="value"></param>
        /// <returns>2维整形结构</returns>
        public static PointInt2 ToPoint(ulong value)
        {
            return new PointInt2((int)(value & 0xFFFFFFFF), (int)(value >> 32));
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(PointInt2 p1, PointInt2 p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(PointInt2 p1, PointInt2 p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }

        /// <summary>
        /// 将两值分别相加 (x1 + x2, y1 + y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static PointInt2 operator +(PointInt2 p1, PointInt2 p2)
        {
            return new PointInt2(p1.x + p2.x, p1.y + p2.y);
        }

        /// <summary>
        /// 将左值减去右值 (x1 - x2, y1 - y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static PointInt2 operator -(PointInt2 p1, PointInt2 p2)
        {
            return new PointInt2(p1.x - p2.x, p1.y - p2.y);
        }

        /// <summary>
        /// 强制转换为浮点结构
        /// </summary>
        /// <param name="point"></param>
        public static explicit operator Point2(PointInt2 point)
        {
            return new Point2(point.x, point.y);
        }

        /// <summary>
        /// 将二维浮点结构强制转换为整数结构
        /// </summary>
        /// <param name="point"></param>
        public static explicit operator PointInt2(Point2 point)
        {
            return new PointInt2((int)point.x, (int)point.y);
        }

        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            if(obj is PointInt2 p)
            {
                return this == p;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.x ^ this.y;
        }

        public bool Equals(PointInt2 other)
        {
            return x == other.x && y == other.y;
        }

        /// <summary>
        /// 返回字符串格式的坐标 (x,y)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        /// <summary>
        /// 以指定格式返回字符串格式的坐标 (x,y)
        /// </summary>
        /// <param name="format">数值格式字符串</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return "(" + x.ToString(format) + "," + y.ToString(format) + ")";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "(" + x.ToString(format, formatProvider) + "," + y.ToString(format, formatProvider) + ")";
        }

        public long GetHashCode64()
        {
            return (long)((ulong)x ^ ((ulong)y << 32));
        }

        /// <summary>
        /// 将2维整形结构转化为64位整型值
        /// </summary>
        /// <returns></returns>
        public ulong ToUInt64()
        {
            return ((ulong)x ^ ((ulong)y << 32));
        }

        #endregion

        #endregion

    }
}
