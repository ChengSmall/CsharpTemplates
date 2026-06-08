using System;
using Cheng.Algorithm.HashCodes;
using System.Runtime.InteropServices;
using Cheng.Algorithm;

using tv = System.Int64;
using TP = Cheng.DataStructure.Cherrsdinates.Point2I64;

namespace Cheng.DataStructure.Cherrsdinates
{

    /// <summary>
    /// 可表示空间坐标或向量的结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Point2I64 : IEquatable<TP>, IHashCode64, IFormattable
    {

        #region 构造

        /// <summary>
        /// 初始化一个二维坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2I64(tv x, tv y)
        {
            this.x = x; this.y = y;
        }

        #endregion

        #region 参数

        /// <summary>
        /// x值
        /// </summary>
        public readonly tv x;

        /// <summary>
        /// y值
        /// </summary>
        public readonly tv y;

        #endregion

        #region 功能

        #region 参数设置

        /// <summary>
        /// 返回一个x为新值的结构
        /// </summary>
        /// <param name="value">新的x</param>
        /// <returns></returns>
        public TP SetX(tv value)
        {
            return new TP(value, y);
        }

        /// <summary>
        /// 返回一个y为新值的结构
        /// </summary>
        /// <param name="value">新的y</param>
        /// <returns></returns>
        public TP SetY(tv value)
        {
            return new TP(x, value);
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(in TP p1, in TP p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(in TP p1, in TP p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }

        /// <summary>
        /// 将两值分别相加 (x1 + x2, y1 + y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static TP operator +(in TP p1, in TP p2)
        {
            return new TP(p1.x + p2.x, p1.y + p2.y);
        }

        /// <summary>
        /// 将左值减去右值 (x1 - x2, y1 - y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static TP operator -(in TP p1, in TP p2)
        {
            return new TP(p1.x - p2.x, p1.y - p2.y);
        }

        /// <summary>
        /// 乘法运算 (x * num, y * num)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static TP operator *(TP p, tv num)
        {
            return new TP(p.x * num, p.y * num);
        }

        /// <summary>
        /// 除法运算 (x / num, y / num)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static TP operator /(TP p, tv num)
        {
            return new TP(p.x / num, p.y / num);
        }

        public static explicit operator TP(Point2I32 p)
        {
            return new TP(p.x, p.y);
        }

        public static implicit operator Point2(TP p)
        {
            return new Point2(p.x, p.y);
        }

        public static explicit operator Point2F(TP p)
        {
            return new Point2F((float)p.x, (float)p.y);
        }

        public static implicit operator TP((tv, tv) tuple)
        {
            return new TP(tuple.Item1, tuple.Item2);
        }

        public static explicit operator (tv, tv)(TP p)
        {
            return ValueTuple.Create(p.x, p.y);
        }

        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            if (obj is TP p)
            {
                return this == p;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (x ^ y).GetHashCode();
        }

        public bool Equals(TP other)
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
        /// 返回字符串格式的坐标 (x,y)，指定值的格式
        /// </summary>
        /// <param name="format">表示格式的字符串</param>
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
            return x ^ y;
        }

        #endregion

        #region 坐标运算

        /// <summary>
        /// 计算和另一个向量的点积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>点积</returns>
        public tv DotPro(in TP other)
        {
            return x * other.x + y * other.y;
        }

        /// <summary>
        /// 计算从此坐标到另一个坐标的向量
        /// </summary>
        /// <param name="other">另一个坐标</param>
        /// <returns>表示二维向量的结构</returns>
        public TP Vector(in TP other)
        {
            return new TP(other.x - x, other.y - y);
        }

        /// <summary>
        /// 计算与另一个向量的叉积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns></returns>
        public tv CrossPro(in TP other)
        {
            return x * other.y - other.x * y;
        }

        #endregion

        #endregion

    }

}