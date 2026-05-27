using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm
{

    public unsafe static partial class Maths
    {

        //角度计算

        #region 常量

        /// <summary>
        /// π的两倍
        /// </summary>
        public const double PI2 = Math.PI * 2;

        /// <summary>
        /// 表示角度值为1的弧度值
        /// </summary>
        public const double OneRadian = System.Math.PI / 180;

        /// <summary>
        /// π的单浮点值
        /// </summary>
        public const float FPI = 3.14159274f;

        #endregion

        #region 扩展

        /// <summary>
        /// 将弧度制转化为角度制
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns>角度</returns>
        public static double ToAngle(this double radian)
        {
            return radian / OneRadian;
        }

        /// <summary>
        /// 将角度制转化为弧度制
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>弧度</returns>
        public static double ToRadian(this double angle)
        {
            return angle * OneRadian;
        }

        /// <summary>
        /// 将角度制转化为弧度制
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>弧度</returns>
        public static double ToRadian(this float angle)
        {
            return angle * OneRadian;
        }

        /// <summary>
        /// 将角度制转化为弧度制
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>弧度</returns>
        public static double ToRadian(this int angle)
        {
            return ((angle) * OneRadian);
        }

        #endregion

        /// <summary>
        /// 角度和平面直角坐标系
        /// </summary>
        public static class Angles
        {

            #region 角度计算

            /// <summary>
            /// 旋转角度计算
            /// </summary>
            /// <remarks>
            /// <para>角度方向以平面直角坐标系为基准，逆增顺减</para>
            /// 若所提供参数不属于指定范围，返回的结果可能有误
            /// </remarks>
            /// <param name="startAngle">起始角度，范围在[0,360]</param>
            /// <param name="angle">表示偏移到的目标角度，范围在[0,360]</param>
            /// <param name="clockwise">偏移方向；true表示顺时针偏移，false表示逆时针偏移</param>
            /// <returns>从起始角度偏移到目标角度的偏移角度，该角度始终大于0</returns>
            public static double AngleRotation(double startAngle, double angle, bool clockwise)
            {

                double re;
                if (clockwise)
                {
                    re = startAngle - angle;
                    return re < 0 ? (re + 360) : re;
                }

                re = angle - startAngle;
                return re < 0 ? (re + 360) : re;

            }

            /// <summary>
            /// 旋转弧度计算
            /// </summary>
            /// <remarks>
            /// <para>弧度方向以平面直角坐标系为基准，逆增顺减</para>
            /// 若所提供参数不属于指定范围，返回的结果可能有误
            /// </remarks>
            /// <param name="startRadian">起始弧度，范围在[0, 2PI]</param>
            /// <param name="radian">表示偏移到的目标弧度，范围在[0, 2PI]</param>
            /// <param name="clockwise">偏移方向；true表示顺时针偏移，false表示逆时针偏移</param>
            /// <returns>从起始弧度偏移到目标弧度的偏移弧度，该弧度始终为正弧度</returns>
            public static double RadianRotation(double startRadian, double radian, bool clockwise)
            {
                const double allR = System.Math.PI * 2;

                double re;
                if (clockwise)
                {
                    re = startRadian - radian;
                    return re < 0 ? (re + allR) : re;
                }

                re = radian - startRadian;
                return re < 0 ? (re + allR) : re;
            }

            /// <summary>
            /// 将给定角弧度旋转180度
            /// </summary>
            /// <param name="radian">角弧度</param>
            /// <returns>旋转180度后的角弧度</returns>
            public static double ReverseRadian(double radian)
            {
                var r = (radian - Math.PI);
                if (r < 0) r += PI2;
                return r;
            }

            /// <summary>
            /// 将给定的弧度角旋转指定弧度
            /// </summary>
            /// <param name="radian">给定的弧度</param>
            /// <param name="rotation">要旋转的弧度</param>
            /// <returns>旋转后的弧度</returns>
            public static double RotationTo(double radian, double rotation)
            {
                radian += rotation % PI2;
                if (radian > PI2) radian -= PI2;
                else if (radian < 0) radian += PI2;
                return radian;
            }

            /// <summary>
            /// 求两个弧度角在平面直角坐标系中的夹角的角弧度值
            /// </summary>
            /// <param name="radian1">弧度角1</param>
            /// <param name="radian2">弧度角2</param>
            /// <returns>两个参数的夹角的角弧度值，返回的弧度值不会超过一个平角弧度</returns>
            public static double IncludedAngleByRadian(double radian1, double radian2)
            {
                // 计算两个弧度的绝对差值，并取模确保差值在[0, 2π)区间内
                double d = Math.Abs(radian1 - radian2) % (2 * Math.PI);

                // 如果差值超过π弧度，则用2π减去差值得到最小夹角
                return d > Math.PI ? (2 * Math.PI - d) : d;
            }

            /// <summary>
            /// 求两个弧度角在平面直角坐标系的夹角中线的弧度角
            /// </summary>
            /// <remarks>
            /// <para>
            /// 参数<paramref name="radian1"/>和<paramref name="radian2"/>表示在平面直角坐标系上的两个弧度角，该函数将求出这两个弧度角构成的的夹角所在中线的角弧度
            /// </para>
            /// <para>
            /// 如果两个弧度角恰好构成平角，则中线表示为从弧度角<paramref name="radian1"/>逆时针旋转到弧度角<paramref name="radian2"/>的夹角中线
            /// </para>
            /// </remarks>
            /// <param name="radian1">弧度角1</param>
            /// <param name="radian2">弧度角2</param>
            /// <returns>两弧度夹角的中线所在弧度角，范围在[0,2PI)</returns>
            public static double TwoRadianCenterLine(double radian1, double radian2)
            {

                #region

                // 计算radian2相对于radian1的逆时针夹角
                double delta = (radian2 - radian1) % (2 * Math.PI);
                if (delta < 0)
                    delta += 2 * Math.PI;

                double center;
                if (delta <= Math.PI)
                {
                    // 当逆时针夹角不大于π时，中线为radian1 + delta/2
                    center = radian1 + delta / 2;
                }
                else
                {
                    // 当逆时针夹角大于π时，实际处理顺时针方向的补角，中线为radian1 - (2π - delta)/2
                    center = radian1 + (delta / 2 - Math.PI);
                }

                // 将结果规范到0~2π范围
                center = (center % (2 * Math.PI) + 2 * Math.PI) % (2 * Math.PI);
                return center;

                #endregion

            }

            #endregion

        }

        #region 三角形

        /// <summary>
        /// 三角形计算
        /// </summary>
        public static class TriAngle
        {

            #region 记
            /*
                三角函数定义：直角三角形内，对非直角进行计算，得：
                (正弦)sin => 对/斜
                (余弦)cos => 邻/斜
                (正切)tan => 对/邻

                余弦定理：面积S = ab * sin(A) / 2
                a,b表示三角形两边，A表示两边夹角
             */
            #endregion

            #region 面积

            /// <summary>
            /// 根据三角形三边长求面积
            /// </summary>
            /// <param name="a">三角形边长1</param>
            /// <param name="b">三角形边长2</param>
            /// <param name="c">三角形边长3</param>
            /// <returns>三角形面积</returns>
            public static double TriangleAreaSSS(double a, double b, double c)
            {
                double p = (a + b + c) / 2;
                return System.Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            }

            /// <summary>
            /// 根据三角形两个边长和一个夹角，计算三角形面积
            /// </summary>
            /// <param name="a">边长1</param>
            /// <param name="radian">两边夹角的角弧度</param>
            /// <param name="b">边长2</param>
            /// <returns>三角形面积</returns>
            public static double TriangleAreaSAS(double a, double radian, double b)
            {
                return (a * b * Math.Sin(radian)) / 2;
            }

            /// <summary>
            /// 根据三角形一个边长和左右两个角弧度，计算三角形面积
            /// </summary>
            /// <param name="A">弧度角1</param>
            /// <param name="length">三角形边长</param>
            /// <param name="B">弧度角2</param>
            /// <returns>三角形面积</returns>
            public static double TriangleAreaASA(double A, double length, double B)
            {

                return (((System.Math.Sin(A) * length) / System.Math.Sin(A + B)) * length * System.Math.Sin(B)) / 2;

            }

            /// <summary>
            /// 根据三角形两个角度一个边长，计算三角形面积
            /// </summary>
            /// <param name="A">弧度角A</param>
            /// <param name="B">弧度角B</param>
            /// <param name="length">边长</param>
            /// <returns>三角形面积</returns>
            public static double TriangleAreaAAS(double A, double B, double length)
            {
                /* 
                    正弦定理 a/sinA == b/sinB == c/sinC == 2r == D
                    b = (a / SinA) * SinB
                */
                ////角C弧度
                //double C = System.Math.PI - (A + B);
                ////另一边 b
                //double b = (length / System.Math.Sin(A)) * System.Math.Sin(B);
                //return (System.Math.Sin(C) * length * b) / 2;

                return (System.Math.Sin((System.Math.PI - (A + B))) * length * ((length / System.Math.Sin(A)) * System.Math.Sin(B))) / 2;
            }

            #endregion

            #region 内切圆

            /// <summary>
            /// 输入三边长，求三角形内切圆半径
            /// </summary>
            /// <param name="a">边长1</param>
            /// <param name="b">边长2</param>
            /// <param name="c">边长3</param>
            /// <returns>内切圆半径</returns>
            public static double TrianleInCircle(double a, double b, double c)
            {
                return (TriangleAreaSSS(a, b, c) * 2) / (a + b + c);
            }

            #endregion

        }

        #endregion

    }

}
