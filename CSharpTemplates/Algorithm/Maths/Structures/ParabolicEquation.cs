using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Memorys;
using Cheng.Algorithm;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.Algorithm.HashCodes;

using ty = System.Double;

namespace Cheng.Algorithm.DataStructure
{

    /// <summary>
    /// 抛物线方程
    /// </summary>
    /// <remarks>
    /// <para>表达二元一次方程的结构，表达式 f(x) = ax^2 + bx + c</para>
    /// </remarks>
    [Serializable]
    public readonly struct ParabolicEquation : IEquatable<ParabolicEquation>, IHashCode64, IFormattable
    {

        #region 结构

        /// <summary>
        /// 方程式的根数量
        /// </summary>
        public enum RootQuadraticValue
        {
            /// <summary>
            /// 无结果
            /// </summary>
            None = 0,

            /// <summary>
            /// 有一个解（Delta 等于 0)
            /// </summary>
            OnlyOne = 1,

            /// <summary>
            /// 有两个解
            /// </summary>
            TwoRootQuadratic = 2
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化抛物线方程式
        /// </summary>
        /// <param name="a">二次项系数</param>
        /// <param name="b">一次项系数</param>
        /// <param name="c">常数项</param>
        public ParabolicEquation(ty a, ty b, ty c)
        {
            this.a = a; this.b = b; this.c = c;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 二次项系数
        /// </summary>
        public readonly ty a;

        /// <summary>
        /// 一次项系数
        /// </summary>
        public readonly ty b;

        /// <summary>
        /// 常数项
        /// </summary>
        public readonly ty c;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 标准二元一次方程的 Δ 值
        /// </summary>
        public ty Delta
        {
            get
            {
                //b^2 - 4ac
                return (b * b) - (4 * a * c);
            }
        }

        /// <summary>
        /// 返回曲线方程的解（求x)
        /// </summary>
        /// <param name="fx">表示f(x)的值，使用 0 表示标准方程</param>
        /// <param name="x1">第一个解，表示为 -b + Sqrt(Delta) / 2a</param>
        /// <param name="x2">第二个解，表示为 -b - Sqrt(Delta) / 2a</param>
        /// <returns>解的数量</returns>
        public RootQuadraticValue QuadraticRoot(ty fx, out ty x1, out ty x2)
        {
            x1 = default;
            x2 = default;
            ty del;

            // ax^2 + bx + c == fx
            // ax^2 + bx + (c - fx) == 0

            del = (b * b) - (4 * a * (this.c - fx));

            if (del < 0)
            {
                return RootQuadraticValue.None;
            }
            var r = (2 * a);
            if (del == 0)
            {
                x1 = (-b) / r;
                x2 = x1;
                return RootQuadraticValue.OnlyOne;
            }

            del = Math.Sqrt(del);
            x1 = ((-b) + del) / r;
            x2 = ((-b) - del) / r;
            return RootQuadraticValue.TwoRootQuadratic;
        }

        /// <summary>
        /// 返回标准曲线方程的解（求x)
        /// </summary>
        /// <param name="x1">第一个解，表示为 -b + Sqrt(Delta) / 2a</param>
        /// <param name="x2">第二个解，表示为 -b - Sqrt(Delta) / 2a</param>
        /// <returns>解的数量</returns>
        public RootQuadraticValue QuadraticRoot(out ty x1, out ty x2)
        {
            return QuadraticRoot(0, out x1, out x2);
        }

        /// <summary>
        /// 已知x求y
        /// </summary>
        /// <param name="x">x值</param>
        /// <returns>y值</returns>
        public ty GetY(ty x)
        {
            // ax^2 + bx + c
            return (a * x * x) + (b * x) + c;
        }

        /// <summary>
        /// 曲线方程在顶点式中的 h
        /// </summary>
        /// <remarks>
        /// <para>
        /// 顶点式: a(x - h)^2 + k = 0
        /// </para>
        /// </remarks>
        public ty H
        {
            get
            {
                return - (b / (2 * a));
            }
        }

        /// <summary>
        /// 曲线方程在顶点式中的 k
        /// </summary>
        /// <remarks>
        /// <para>
        /// 顶点式: a(x - h)^2 + k = 0
        /// </para>
        /// </remarks>
        public ty K
        {
            get
            {
                return c - ((4 * a * c) / (2 * a));
            }
        }

        /// <summary>
        /// 抛物线的顶点坐标
        /// </summary>
        public Point2 Top
        {
            get
            {
                var h = -(b / (2 * a));
                return new Point2(h, c - (h * h));
            }
        }

        #endregion

        #region 创建方法

        /// <summary>
        /// 使用顶点式参数创建抛物线方程
        /// </summary>
        /// <remarks>
        /// <para>
        /// 顶点式: a(x - h)^2 + k = 0
        /// </para>
        /// </remarks>
        /// <param name="a">顶点式系数 a，决定抛物线宽度与开口方向</param>
        /// <param name="h">顶点式系数 h，决定抛物线高度</param>
        /// <param name="k">顶点式系数 k，决定抛物线位移</param>
        /// <returns>抛物线方程</returns>
        public static ParabolicEquation CreateFormVertex(ty a, ty h, ty k)
        {
            // b = -2 * a * h
            // c = (a * h * h) + k
            var ah = a * h;
            return new ParabolicEquation(a, -a * ah, (ah * h) + k);
        }

        /// <summary>
        /// 使用坐标系上三个不同的点创建一个抛物线方程
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="p3">点3</param>
        /// <returns>抛物线方程</returns>
        public static ParabolicEquation CreateFormPoint(in Point2 p1, in Point2 p2, in Point2 p3)
        {
            // 计算系数矩阵行列式D
            double D = determinant3x3(
                p1.x * p1.x, p1.x, 1,
                p2.x * p2.x, p2.x, 1,
                p3.x * p3.x, p3.x, 1
            );

            return new ParabolicEquation(
                determinant3x3(
                p1.y, p1.x, 1,
                p2.y, p2.x, 1,
                p3.y, p3.x, 1) / D,
                determinant3x3(
                p1.x * p1.x, p1.y, 1,
                p2.x * p2.x, p2.y, 1,
                p3.x * p3.x, p3.y, 1) / D,
                determinant3x3(
                p1.x * p1.x, p1.x, p1.y,
                p2.x * p2.x, p2.x, p2.y,
                p3.x * p3.x, p3.x, p3.y) / D);

            double determinant3x3(double a1, double a2, double a3,
                double b1, double b2, double b3,
                double c1, double c2, double c3)
            {
                return (a1 * ((b2 * c3) - (b3 * c2)))
                     - (a2 * (b1 * c3 - b3 * c1))
                     + (a3 * (b1 * c2 - b2 * c1));
            }

        }

        #endregion

        #region 运算符

        public static bool operator ==(in ParabolicEquation x, in ParabolicEquation y)
        {
            return x.a == y.a && x.b == y.b && x.c == y.c;
        }

        public static bool operator !=(in ParabolicEquation x, in ParabolicEquation y)
        {
            return x.a != y.a || x.b != y.b || x.c != y.c;
        }

        #endregion

        #region 派生接口

        bool IEquatable<ParabolicEquation>.Equals(ParabolicEquation other)
        {
            return this.a == other.a && this.b == other.b && this.c == other.c;
        }

        /// <summary>
        /// 判断与另一个抛物线系数是否一致
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(in ParabolicEquation other)
        {
            return this.a == other.a && this.b == other.b && this.c == other.c;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParabolicEquation other) return this.Equals(in other);
            return false;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode();
        }

        public long GetHashCode64()
        {
            return a.GetHashCode64() ^ b.GetHashCode64() ^ c.GetHashCode64();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder(48);

            sb.Append("f(x) = ")
                .Append(a.ToString(format, formatProvider))
                .Append("x^2 + ")
                .Append(b.ToString(format, formatProvider))
                .Append("x + ")
                .Append(c.ToString(format, formatProvider))
                ;

            return sb.ToString();
        }

        #endregion

        #endregion

    }

}
