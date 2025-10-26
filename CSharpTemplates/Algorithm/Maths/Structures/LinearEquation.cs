using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Memorys;
using Cheng.Algorithm;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Cherrsdinates;

using ty = System.Double;
using Cheng.Algorithm.HashCodes;

namespace Cheng.Algorithm.DataStructure
{

    /// <summary>
    /// 直线方程
    /// </summary>
    /// <remarks>
    /// <para>表达一元一次方程的结构，表达式 f(x) = ax + b</para>
    /// </remarks>
    [Serializable]
    public readonly struct LinearEquation : IEquatable<LinearEquation>, IHashCode64, IFormattable
    {

        #region 初始化

        /// <summary>
        /// 初始化直线方程
        /// </summary>
        /// <param name="a">直线斜率</param>
        /// <param name="b">y轴截距</param>
        public LinearEquation(ty a, ty b)
        {
            this.a = a;
            this.b = b;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 直线斜率
        /// </summary>
        public readonly ty a;

        /// <summary>
        /// y轴截距
        /// </summary>
        public readonly ty b;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 根据x坐标获取y值
        /// </summary>
        /// <param name="x">x坐标值</param>
        /// <returns>得出的y坐标值</returns>
        public ty GetY(ty x)
        {
            if (double.IsNaN(a))
            {
                return b;
            }
            return a * x + b;
        }

        /// <summary>
        /// 根据y求出x值
        /// </summary>
        /// <param name="y">要指定的y值</param>
        /// <returns>求出的x值</returns>
        public ty GetX(ty y)
        {
            if (double.IsNaN(a))
            {
                return b;
            }
            if(a == 0)
            {
                return double.NaN;
            }
            //y = ax + b
            //y - b = ax
            //(y - b) / a = x
            return (y - b) / a;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 使用两个位置坐标初始化一个直线方程
        /// </summary>
        /// <param name="p1">坐标系上的一个点</param>
        /// <param name="p2">坐标系上第二个点</param>
        /// <returns>直线方程</returns>
        public static LinearEquation CreateFormTwoPoint(in Point2 p1, in Point2 p2)
        {
            if(p1.x == p2.x)
            {
                // 垂直x的直线 y = x
                return new LinearEquation(double.NaN, p1.x);
            }

            if (p1.y == p2.y)
            {
                // 平行于x的直线 y = b
                return new LinearEquation(0, p1.y);
            }

            double a;
            a = (p2.y - p1.y) / (p2.x - p1.x);
            //a * p1.x + b = p1.y
            return new LinearEquation(a, p1.y - (a * p1.x));
        }

        #endregion

        #region 判断

        /// <summary>
        /// 该直线是否与x轴保持水平
        /// </summary>
        public bool IsHorizontal
        {
            get
            {
                return a == 0;
            }
        }

        /// <summary>
        /// 该直线是否与x轴垂直
        /// </summary>
        public bool IsVertical
        {
            get
            {
                return double.IsNaN(a);
            }
        }

        /// <summary>
        /// 如果该直线与x轴平行，获得直线与x轴的距离，否则为<see cref="double.NaN"/>
        /// </summary>
        public double IfHerizontalY
        {
            get => a == 0 ? b : double.NaN;
        }

        /// <summary>
        /// 如果该直线与x轴垂直，获得直线与y轴的距离，否则为<see cref="double.NaN"/>
        /// </summary>
        public double IfVerticalX
        {
            get => double.IsNaN(a) ? b : double.NaN;
        }

        #endregion

        #region 派生

        #region 运算符重载

        /// <summary>
        /// 比较是否为相同位置的直线
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <returns></returns>
        public static bool operator ==(in LinearEquation L1, in LinearEquation L2)
        {
            return L1.a == L2.a && L1.b == L2.b;
        }

        /// <summary>
        /// 比较是否为不同位置的直线
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <returns></returns>
        public static bool operator !=(in LinearEquation L1, in LinearEquation L2)
        {
            return L1.a != L2.a || L1.b != L2.b;
        }

        #endregion

        #region 重载

        bool IEquatable<LinearEquation>.Equals(LinearEquation other)
        {
            return a == other.a && b == other.b;
        }

        /// <summary>
        /// 与另一个比较是否为相同位置的直线
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(in LinearEquation other)
        {
            return a == other.a && b == other.b;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinearEquation other) return a == other.a && b == other.b;
            return false;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode();
        }

        public long GetHashCode64()
        {
            return a.GetHashCode64() ^ b.GetHashCode64();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// 返回指定格式的字符串
        /// </summary>
        /// <param name="provider">要用于格式化值的提供程序，或使用null指定默认模式</param>
        /// <returns></returns>
        public string ToString(IFormatProvider provider)
        {
            return ToString(null, provider);
        }

        /// <summary>
        /// 返回指定格式的字符串
        /// </summary>
        /// <param name="format">要使用的格式，或使用null指定默认格式</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            if (IsHorizontal)
            {
                return "f(x) = " + b.ToString(format, provider);
            }
            if (IsVertical)
            {
                return "x = " + b.ToString(format, provider);
            }
            StringBuilder sb = new StringBuilder(24);
            sb.Append("f(x) = ")
                .Append(a.ToString(format, provider))
                .Append("x + ")
                .Append(b.ToString(format, provider))
            ;

            return sb.ToString();
        }

        #endregion

        #endregion

        #endregion

    }

}
