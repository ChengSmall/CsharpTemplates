using System;
using Cheng.Algorithm.HashCodes;
using System.Runtime.InteropServices;
using Cheng.Algorithm;

using tv = System.Int32;
using TP = Cheng.DataStructure.Cherrsdinates.PointInt3;

namespace Cheng.DataStructure.Cherrsdinates
{

    /// <summary>
    /// 可表示空间坐标或向量的结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PointInt3 : IEquatable<TP>, IHashCode64, IFormattable
    {

        #region 构造

        /// <summary>
        /// 初始化3维结构
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public PointInt3(tv x, tv y, tv z)
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
        public PointInt3(tv x, tv y)
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
        public readonly tv x;

        /// <summary>
        /// y
        /// </summary>
        public readonly tv y;

        /// <summary>
        /// z
        /// </summary>
        public readonly tv z;

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
            return new TP(value, y, z);
        }

        /// <summary>
        /// 返回一个y为新值的结构
        /// </summary>
        /// <param name="value">新的y</param>
        /// <returns></returns>
        public TP SetY(tv value)
        {
            return new TP(x, value, z);
        }

        /// <summary>
        /// 返回一个z为新值的结构
        /// </summary>
        /// <param name="value">新的z</param>
        /// <returns></returns>
        public TP SetZ(tv value)
        {
            return new TP(x, y, value);
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
        public static TP operator +(TP p1, TP p2)
        {
            return new TP(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static TP operator -(TP p1, TP p2)
        {
            return new TP(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

        /// <summary>
        /// 乘法运算；将每一个分量乘法计算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static TP operator *(TP p1, tv num)
        {
            return new TP(p1.x * num, p1.y * num, p1.z * num);
        }

        /// <summary>
        /// 除法运算；将每一个分量除法计算
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static TP operator /(TP p1, tv num)
        {
            return new TP(p1.x / num, p1.y / num, p1.z / num);
        }

        #endregion

        #region 比较

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(TP p1, TP p2)
        {
            return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
        }

        /// <summary>
        /// 判断不相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(TP p1, TP p2)
        {
            return p1.x != p2.x || p1.y != p2.y || p1.z != p2.z;
        }

        #endregion

        #region 转换

        public static explicit operator PointInt2(PointInt3 point)
        {
            return new PointInt2(point.x, point.y);
        }

        public static explicit operator PointInt3(PointInt2 point)
        {
            return new PointInt3(point.x, point.y);
        }

        public static explicit operator Point3(PointInt3 point)
        {
            return new Point3(point.x, point.y, point.z);
        }

        public static explicit operator PointInt3(Point3 point)
        {
            return new PointInt3((tv)point.x, (tv)point.y, (tv)point.z);
        }

        public static explicit operator Point3F(PointInt3 point)
        {
            return new Point3F(point.x, point.y, point.z);
        }

        public static explicit operator PointInt3(Point3F point)
        {
            return new PointInt3((tv)point.x, (tv)point.y, (tv)point.z);
        }

        #endregion

        #endregion

        #region 计算

        /// <summary>
        /// 计算从此坐标到指定坐标的距离
        /// </summary>
        /// <param name="other">指定坐标</param>
        /// <returns>从此坐标到<paramref name="other"/>的距离</returns>
        public double Distance(TP other)
        {
            var xn = other.x - x;
            var yn = other.y - y;
            var zn = other.z - z;
            return System.Math.Sqrt(xn * xn + yn * yn + zn * zn);
        }

        /// <summary>
        /// 计算从原点(0,0,0)到此坐标的距离的平方
        /// </summary>
        public tv SqrMagnitude
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
        public TP Normalized
        {
            get
            {
                var re = System.Math.Sqrt(x * x + y * y + z * z);
                return new TP((int)(this.x / re), (int)(this.y / re), (int)(this.z / re));
            }
        }

        /// <summary>
        /// 计算和另一个向量的点积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>点积</returns>
        public double DotPro(TP other)
        {
            return x * other.x + y * other.y;
        }

        /// <summary>
        /// 计算从此坐标到另一个坐标的向量
        /// </summary>
        /// <param name="other">另一个坐标</param>
        /// <returns>表示3维向量的结构</returns>
        public TP Vector(TP other)
        {
            return new TP(other.x - x, other.y - y, other.z - z);
        }

        /// <summary>
        /// 计算与另一个向量的叉积
        /// </summary>
        /// <param name="other">另一个向量</param>
        /// <returns>叉乘的结果</returns>
        public TP CrossPro(TP other)
        {
            return new TP(y * other.z - other.y * z, -(x * other.z - other.x * z), x * other.y - other.x * y);
        }

        #endregion

        #region 派生

        public bool Equals(TP other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is TP p) return this == p;
            return false;
        }

        public override int GetHashCode()
        {
            return x ^ y ^ z;
        }

        public long GetHashCode64()
        {
            return (long)(((uint)x | (((ulong)y) << 32)) ^ ((ulong)z << 16));
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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Concat("(", x.ToString(format, formatProvider), ",", y.ToString(format, formatProvider), ",", z.ToString(format, formatProvider), ")");
        }

        #endregion

        #endregion

    }


}