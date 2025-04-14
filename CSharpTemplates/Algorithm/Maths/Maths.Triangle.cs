using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm
{

    public unsafe static partial class Maths
    {

        #region 常量

        /// <summary>
        /// π的两倍
        /// </summary>
        public const double PI2 = Math.PI * 2;

        /// <summary>
        /// 表示角度值为1的弧度值
        /// </summary>
        public const double OneRadian = System.Math.PI / 180;

        #endregion

        #region 角度计算

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
        public static double ReverseRadian(this double radian)
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
        public static double RotationTo(this double radian, double rotation)
        {
            radian += rotation;
            if (radian > PI2) radian -= PI2;
            return radian;
        }

        #endregion

        #region 三角形

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
        /// <param name="radian">连边夹角的角弧度</param>
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
            #region
            //return ((length * Math.Sin(B)) / Math.Sin(Math.PI - (A + B))) * length * Math.Sin(A) * 0.5;

            //var x = length * Math.Sin(B);

            //var C = Math.PI - (A + B);

            //var b = x / Math.Sin(C);

            //return b * length * Math.Sin(A) * 0.5;
            #endregion

            #region
            //确保角度在0到π之间，否则取其余角

            //只需要一个角的正弦值来计算高，这里选择第一个角
            double sinValue = Math.Sin(Math.Abs(A) % (Math.PI * 2));

            //确保正弦值不为零，避免除以零的错误  
            if (sinValue == 0) sinValue = Math.Sin(Math.Abs(B) % (Math.PI * 2));

            // 计算面积  
            return 0.5 * length * (length * sinValue);
            #endregion
        }

        #endregion

        #endregion


    }

}
