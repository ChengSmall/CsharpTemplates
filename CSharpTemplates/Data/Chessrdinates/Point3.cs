using System;
using Cheng.Algorithm.HashCodes;
using System.Runtime.InteropServices;
using Cheng.Algorithm;

namespace Cheng.DataStructure.Cherrsdinates
{

    /// <summary>
    /// 可表示空间坐标或向量的结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point3 : IEquatable<Point3>, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 初始化3维结构
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 初始化3维结构
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point3(double x, double y)
        {
            this.x = x;
            this.y = y;
            z = 0;
        }

        #endregion

        #region 参数

        /// <summary>
        /// x
        /// </summary>
        public readonly double x;

        /// <summary>
        /// y
        /// </summary>
        public readonly double y;

        /// <summary>
        /// z
        /// </summary>
        public readonly double z;

        #endregion

        #region 功能

        #region 参数设置

        /// <summary>
        /// 返回一个x为新值的结构
        /// </summary>
        /// <param name="value">新的x</param>
        /// <returns></returns>
        public Point3 SetX(double value)
        {
            return new Point3(value, y, z);
        }

        /// <summary>
        /// 返回一个y为新值的结构
        /// </summary>
        /// <param name="value">新的y</param>
        /// <returns></returns>
        public Point3 SetY(double value)
        {
            return new Point3(x, value, z);
        }

        /// <summary>
        /// 返回一个z为新值的结构
        /// </summary>
        /// <param name="value">新的z</param>
        /// <returns></returns>
        public Point3 SetZ(double value)
        {
            return new Point3(x, y, value);
        }

        #endregion

        #region 运算符

        #region 四则计算

        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3 operator +(Point3 p1, Point3 p2)
        {
            return new Point3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3 operator -(Point3 p1, Point3 p2)
        {
            return new Point3(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

        /// <summary>
        /// 乘法运算；将每一个分量乘法计算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Point3 operator *(Point3 p1, double num)
        {
            return new Point3(p1.x * num, p1.y * num, p1.z * num);
        }

        /// <summary>
        /// 除法运算；将每一个分量除法计算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Point3 operator /(Point3 p1, double num)
        {
            return new Point3(p1.x / num, p1.y / num, p1.z / num);
        }
        
        #endregion

        #region 比较

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Point3 p1, Point3 p2)
        {
            return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
        }

        /// <summary>
        /// 判断不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Point3 p1, Point3 p2)
        {
            return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
        }

        #endregion

        #region 转换

        public static explicit operator Point2(Point3 point)
        {
            return new Point2(point.x, point.y);
        }

        public static explicit operator Point3(Point2 point)
        {
            return new Point3(point.x, point.y);
        }

        #endregion

        #endregion

        #region 计算

        /// <summary>
        /// 计算从此坐标到指定坐标的距离
        /// </summary>
        /// <param name="other">指定坐标</param>
        /// <returns>从此坐标到<paramref name="other"/>的距离</returns>
        public double Distance(Point3 other)
        {
            var xn = other.x - x;
            var yn = other.y - y;
            var zn = other.z - z;
            return System.Math.Sqrt(xn * xn + yn * yn + zn * zn);
        }

        /// <summary>
        /// 计算从原点(0,0,0)到此坐标的距离的平方
        /// </summary>
        public double SqrMagnitude
        {
            get => x * x + y * y + z * z;
        }

        /// <summary>
        /// 计算从原点(0,0,0)到此坐标的距离
        /// </summary>
        /// <returns>距离</returns>
        public double DistanceByZero
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y + z * z);
            }
        }

        /// <summary>
        /// 返回单位向量
        /// </summary>
        /// <returns>将该对象当作向量并把长度缩放为1的新向量</returns>
        public Point3 Normalized
        {
            get
            {
                var re = System.Math.Sqrt(x * x + y * y + z * z);
                return new Point3(this.x / re, this.y / re, this.z / re);
                //return this / System.Math.Sqrt(x * x + y * y + z * z);
            }
        }

        /// <summary>
        /// 计算和另一个向量的点积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>点积</returns>
        public double DotPro(Point3 other)
        {
            return x * other.x + y * other.y;
        }

        /// <summary>
        /// 计算从此坐标到另一个坐标的向量
        /// </summary>
        /// <param name="other">另一个坐标</param>
        /// <returns>表示3维向量的结构</returns>
        public Point3 Vector(Point3 other)
        {
            return new Point3(other.x - x, other.y - y, other.z - z);
        }

        /// <summary>
        /// 计算与另一个向量的叉积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>叉乘的结果</returns>
        public Point3 CrossPro(Point3 other)
        {
            return new Point3(y * other.z - other.y * z, -(x * other.z - other.x * z), x * other.y - other.x * y);
        }

        /// <summary>
        /// 返回此向量与另一个向量之间的角度
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>返回的角度为两个向量之间的无符号角度，角度不超过180</returns>
        public double Angle(Point3 other)
        {
            var num = Maths.Clamp(this.DotPro(other) / Math.Sqrt(this.SqrMagnitude * other.SqrMagnitude), -1f, 1f);
            
            return Math.Acos(num) / (Math.PI / 180d);
        }

        /// <summary>
        /// 返回此向量与另一个向量之间的弧度
        /// </summary>
        /// <param name="other"></param>
        /// <returns>返回的角度为两个向量之间的无符号弧度，弧度不超过<see cref="System.Math.PI"/></returns>
        public double Radian(Point3 other)
        {
            var num = Maths.Clamp(this.DotPro(other) / System.Math.Sqrt(this.SqrMagnitude * other.SqrMagnitude), -1f, 1f);

            return System.Math.Acos(num);
        }

        /// <summary>
        /// 将此坐标点与另一个坐标点计算线性插值
        /// </summary>
        /// <param name="other">另一个坐标</param>
        /// <param name="t">插值</param>
        /// <returns>
        /// 根据参数<paramref name="t"/>从此坐标到另一个坐标的插值；
        /// <para>当t等于0时返回原坐标，t等于1时返回<paramref name="other"/>，0.5则返回两坐标的中间点</para>
        /// </returns>
        public Point3 Lerp(Point3 other, double t)
        {
            return new Point3(this.x + (other.x - this.x) * t, this.y + (other.y - this.y) * t, this.z + (other.z - this.z) * t);
        }

        #endregion

        #region 派生

        public bool Equals(Point3 other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3 p) return this == p;
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public long GetHashCode64()
        {
            return x.GetHashCode64() ^ y.GetHashCode64() ^ z.GetHashCode64();
        }

        /// <summary>
        /// 以字符串的形式返回3维坐标
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("(", x.ToString(), ",", y.ToString(), ",", z.ToString(), ")");
        }

        /// <summary>
        /// 以字符串的形式返回3维坐标
        /// </summary>
        /// <param name="format">每个值的<see cref="double.ToString(string)"/>格式</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return string.Concat("(", x.ToString(format), ",", y.ToString(format), ",", z.ToString(format), ")");
        }

        #endregion

        #endregion

    }

}
