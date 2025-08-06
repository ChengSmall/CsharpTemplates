using System;
using System.Runtime.InteropServices;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Cherrsdinates
{

    /// <summary>
    /// 平面坐标或向量结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Point2 : IEquatable<Point2>, IHashCode64, IFormattable
    {

        #region 构造
        /// <summary>
        /// 初始化一个二维坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2(double x, double y)
        {
            this.x = x; this.y = y;
        }

        #endregion

        #region 参数

        /// <summary>
        /// x值
        /// </summary>
        public readonly double x;

        /// <summary>
        /// y值
        /// </summary>
        public readonly double y;

        #endregion

        #region 功能

        #region 参数设置

        /// <summary>
        /// 返回一个x为新值的结构
        /// </summary>
        /// <param name="value">新的x</param>
        /// <returns></returns>
        public Point2 SetX(double value)
        {
            return new Point2(value, y);
        }

        /// <summary>
        /// 返回一个y为新值的结构
        /// </summary>
        /// <param name="value">新的y</param>
        /// <returns></returns>
        public Point2 SetY(double value)
        {
            return new Point2(x, value);
        }

        #endregion

        #region 运算符重载
        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Point2 p1, Point2 p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }
        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Point2 p1, Point2 p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }
        /// <summary>
        /// 将两值分别相加 (x1 + x2, y1 + y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point2 operator +(Point2 p1, Point2 p2)
        {
            return new Point2(p1.x + p2.x, p1.y + p2.y);
        }
        /// <summary>
        /// 将左值减去右值 (x1 - x2, y1 - y2)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point2 operator -(Point2 p1, Point2 p2)
        {
            return new Point2(p1.x - p2.x, p1.y - p2.y);
        }
        /// <summary>
        /// 乘法运算 (x * num, y * num)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Point2 operator *(Point2 p, double num)
        {
            return new Point2(p.x * num, p.y * num);
        }
        /// <summary>
        /// 除法运算 (x / num, y / num)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Point2 operator /(Point2 p, double num)
        {
            return new Point2(p.x / num, p.y / num);
        }

        public static explicit operator Point2(PointInt2 p)
        {
            return new Point2(p.x, p.y);
        }
        public static implicit operator PointInt2(Point2 p)
        {
            return new PointInt2((int)p.x, (int)p.y);
        }

        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            if (obj is Point2 p)
            {
                return this == p;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public bool Equals(Point2 other)
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
            return x.GetHashCode64() ^ y.GetHashCode64();
        }

        #endregion

        #region 坐标运算

        /// <summary>
        /// 计算从此坐标到指定坐标的距离
        /// </summary>
        /// <param name="other">指定坐标</param>
        /// <returns>一个表示距离的二维向量</returns>
        public double Distance(Point2 other)
        {
            var xn = other.x - x;
            var yn = other.y - y;

            return System.Math.Sqrt(xn * xn + yn * yn);
        }

        /// <summary>
        /// 计算从原点(0,0)到此实例坐标的距离
        /// </summary>
        /// <returns>距离</returns>
        public double DistanceByZero
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y);
            }
        }

        /// <summary>
        /// 返回单位向量
        /// </summary>
        /// <returns>将该向量长度缩放为1的新向量</returns>
        public Point2 Normalized
        {
            get
            {
                return this / System.Math.Sqrt(x * x + y * y);
            }
        }

        /// <summary>
        /// 计算和另一个向量的点积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>点积</returns>
        public double DotPro(Point2 other)
        {
            return x * other.x + y * other.y;
        }

        /// <summary>
        /// 计算从此坐标到另一个坐标的向量
        /// </summary>
        /// <param name="other">另一个坐标</param>
        /// <returns>表示二维向量的结构</returns>
        public Point2 Vector(Point2 other)
        {
            return new Point2(other.x - x, other.y - y);
        }

        /// <summary>
        /// 计算与另一个向量的叉积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns></returns>
        public double CrossPro(Point2 other)
        {
            return x * other.y - other.x * y;
        }

        #endregion

        #endregion

    }

}
