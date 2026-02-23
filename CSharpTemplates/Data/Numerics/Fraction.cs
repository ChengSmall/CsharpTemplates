using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using Cheng.Algorithm;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Numerics
{

    /// <summary>
    /// 由两个整数构成的分数
    /// </summary>
    public struct Fraction : IEquatable<Fraction>, IHashCode64
    {

        #region 初始化

        /// <summary>
        /// 初始化分数值
        /// </summary>
        /// <param name="n">分子</param>
        /// <param name="d">分母</param>
        public Fraction(in BigInteger n, in BigInteger d)
        {
            this.d = d; this.n = n;
        }

        /// <summary>
        /// 初始化整数值到分数
        /// </summary>
        /// <param name="n">整数值</param>
        public Fraction(in BigInteger n)
        {
            this.n = n; this.d = BigInteger.One;
        }

        /// <summary>
        /// 初始化分数值
        /// </summary>
        /// <param name="n">分子</param>
        /// <param name="d">分母</param>
        public Fraction(long n, long d)
        {
            this.d = d; this.n = n;
        }

        /// <summary>
        /// 初始化分数值
        /// </summary>
        /// <param name="n">分子</param>
        /// <param name="d">分母</param>
        public Fraction(int n, int d)
        {
            this.d = d; this.n = n;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 分子
        /// </summary>
        public BigInteger n;

        /// <summary>
        /// 分母
        /// </summary>
        public BigInteger d;

        #endregion

        #region 功能实现

        #region 功能

        /// <summary>
        /// 两值相加 x + y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>运算结果</returns>
        public static Fraction Add(in Fraction x, in Fraction y)
        {
            //分母相乘得出共母
            var fd = x.d * y.d;
            if (fd.IsZero) return new Fraction(BigInteger.One, in fd);
            // 计算 x 分子 -- x的分子 * y的分母
            var xn = x.n * y.d;
            //计算 y 分子 -- y分子 * x分母
            var yn = y.n * x.d;
            //相加
            return new Fraction(xn + yn, in fd);
        }

        /// <summary>
        /// 两值相减求差 x - y
        /// </summary>
        /// <param name="x">被减数</param>
        /// <param name="y">减数</param>
        /// <returns>两值差</returns>
        public static Fraction Sub(in Fraction x, in Fraction y)
        {
            //分母相乘得出共母
            var fd = x.d * y.d;
            // 计算 x 分子 -- x的分子 * y的分母
            var xn = x.n * y.d;
            //计算 y 分子 -- y分子 * x分母
            var yn = y.n * x.d;
            //相减
            return new Fraction(xn - yn, fd);
        }

        /// <summary>
        /// 两值相乘 x * y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>运算结果</returns>
        public static Fraction Mult(in Fraction x, in Fraction y)
        {
            //相乘
            return new Fraction(x.n * y.n, x.d * y.d);
        }

        /// <summary>
        /// 除法运算 <![CDATA[x / y]]>
        /// </summary>
        /// <param name="x">被除数</param>
        /// <param name="y">除数</param>
        /// <returns>商</returns>
        public static Fraction Div(in Fraction x, in Fraction y)
        {
            //相除
            return new Fraction(x.n * y.d, x.d * y.n);
        }

        /// <summary>
        /// 正负号
        /// </summary>
        /// <value>true表示小于0；false表示大于或等于0</value>
        public bool IsNeg
        {
            get
            {
                return (n.Sign >= 0) != (d.Sign >= 0);
            }
        }

        /// <summary>
        /// 将当前分数约分后的值
        /// </summary>
        /// <returns>约分后的值</returns>
        public Fraction ReductionOf()
        {
            //最大公约数
            var ng = !IsNeg;
            var ad = BigInteger.Abs(this.d);
            var an = BigInteger.Abs(this.n);
            var bg = BigInteger.GreatestCommonDivisor(an, ad);
            if (bg > BigInteger.One)
            {
                //公约数大于1
                //上下除公约数
                var abg = BigInteger.Abs(an / bg);
                return new Fraction(ng ? abg : -abg, ad / bg);
            }
            return new Fraction(ng ? an : -an, ad);
        }

        /// <summary>
        /// 将分数转化为整数
        /// </summary>
        /// <param name="mod">分数计算后的余数</param>
        /// <returns>分数计算后的整数</returns>
        /// <exception cref="DivideByZeroException">分母是0</exception>
        public BigInteger ToInteger(out BigInteger mod)
        {
            if (BigInteger.Abs(d).IsOne)
            {
                return n;
            }
            return BigInteger.DivRem(n, d, out mod);
        }

        /// <summary>
        /// 比较两值大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>如果x小于y，返回值小于0；x大于y，返回值大于0；x等于y，返回0</returns>
        public static int Comparer(in Fraction x, in Fraction y)
        {
            //分母相乘得出共母
            var fd = x.d * y.d;
            if (fd.IsZero) return x.d.IsZero ? -1 : (y.d.IsZero ? 0 : 1);

            // 计算 x 分子 -- x的分子 * y的分母
            var xn = x.n * y.d;
            //计算 y 分子 -- y分子 * x分母
            var yn = y.n * x.d;
            return xn.CompareTo(yn);
        }

        /// <summary>
        /// 将分数转化成双精度浮点值
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            if (d.IsZero)
            {
                var ns = n.Sign;
                if (ns == 0) return 0;
                return ns > 0 ? double.PositiveInfinity : double.NegativeInfinity;
            }

            try
            {
                return (double)(((decimal)n) / ((decimal)d));
            }
            catch (Exception)
            {
                return ((double)n) / ((double)d);
            }
        }

        #endregion

        #region 运算符重载

        public static Fraction operator +(Fraction x, Fraction y)
        {
            return Add(in x, in y);
        }

        public static Fraction operator -(Fraction x, Fraction y)
        {
            return Sub(in x, in y);
        }

        public static Fraction operator *(Fraction x, Fraction y)
        {
            return Mult(in x, in y);
        }

        public static Fraction operator /(Fraction x, Fraction y)
        {
            return Div(in x, in y);
        }

        /// <summary>
        /// 取负值
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Fraction operator -(Fraction x)
        {
            var ng = x.IsNeg;
            var ad = BigInteger.Abs(x.d);
            var an = BigInteger.Abs(x.n);

            if (ng)
            {
                return new Fraction(-an, ad);
            }
            else
            {
                return new Fraction(an, ad);
            }
        }

        public static bool operator <(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) < 0;
        }

        public static bool operator >(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) > 0;
        }

        public static bool operator ==(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) == 0;
        }

        public static bool operator !=(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) != 0;
        }

        public static bool operator <=(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) <= 0;
        }

        public static bool operator >=(Fraction x, Fraction y)
        {
            return Comparer(in x, in y) >= 0;
        }

        #endregion

        #region 类型转换

        public static implicit operator Fraction(BigInteger value)
        {
            return new Fraction(in value);
        }

        public static implicit operator Fraction(int value)
        {
            return new Fraction(new BigInteger(value));
        }

        public static implicit operator Fraction(long value)
        {
            return new Fraction(new BigInteger(value));
        }

        public static implicit operator Fraction(uint value)
        {
            return new Fraction(new BigInteger(value));
        }

        public static implicit operator Fraction(ulong value)
        {
            return new Fraction(new BigInteger(value));
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 返回分数的字符串表现形式
        /// </summary>
        /// <returns><![CDATA[n/d]]></returns>
        public override string ToString()
        {
            return n.ToString() + "/" + d.ToString();
        }

        public bool Equals(Fraction other)
        {
            return Comparer(in this, in other) == 0;
        }

        public override int GetHashCode()
        {
            var t = ReductionOf();
            return t.n.GetHashCode() ^ t.d.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Fraction f) return Comparer(in this, in f) == 0; return false;
        }

        public long GetHashCode64()
        {
            var t = ReductionOf();
            var bi = long.MaxValue;
            return (long)((t.n ^ t.d) % bi);
        }

        #endregion

    }

}
