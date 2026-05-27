using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Algorithm.DataStructure;
using Cheng.DataStructure;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.Memorys;

namespace Cheng.Algorithm
{

    static unsafe partial class Maths
    {

        /// <summary>
        /// 二维坐标与平面直角坐标系
        /// </summary>
        public static class Vector2
        {

            static double f_IncludedAngle(double a1, double a2)
            {
                // 法向量点积和模长乘积
                //double dotProduct = Math.Abs(a1 * a2 + 1);
                //double magnitudeProduct = Math.Sqrt(a1 * a1 + 1) * Math.Sqrt(a2 * a2 + 1);

                // 计算夹角余弦值并限制[-1,1]
                double cosTheta = (Math.Abs(a1 * a2 + 1)) / (Math.Sqrt(a1 * a1 + 1) * Math.Sqrt(a2 * a2 + 1));

                if (cosTheta > 1) cosTheta = 1;
                else if (cosTheta < -1) cosTheta = -1;

                // 使用反余弦计算角度
                return Math.Acos(cosTheta);
            }

            /// <summary>
            /// 计算两直线的夹角弧度
            /// </summary>
            /// <param name="L1"></param>
            /// <param name="L2"></param>
            /// <returns>两直线的夹角弧度</returns>
            public static double IncludedAngle(in LinearEquation L1, in LinearEquation L2)
            {
                /*
                斜率 k = (y2 - y1) / (x2 - x1)
                |k1 - k2| / |1 + k1*k2| = tan&
                */
                bool L1V = L1.IsVertical;
                bool L2V = L2.IsVertical;
                bool L1H = L1.IsHorizontal;
                bool L2H = L2.IsHorizontal;
                if ((L1V && L2H) || (L2V && L1H))
                {
                    //垂直
                    return Math.PI / 2;
                }
                if ((L1V && L2V) || (L1H && L2H))
                {
                    //平行
                    return 0;
                }
                return f_IncludedAngle(L1.a, L2.a);
            }

            /// <summary>
            /// 根据弧度值计算对应的单位向量
            /// </summary>
            /// <param name="radian">表示在平面直角坐标系中的一个弧度值</param>
            /// <param name="x">转化后的单位向量x轴水平分量值</param>
            /// <param name="y">转化后的单位向量y轴垂直分量值</param>
            public static void RadianToVector(double radian, out double x, out double y)
            {
                x = Math.Cos(radian);
                y = Math.Sin(radian);
            }

            /// <summary>
            /// 根据弧度值计算对应的单位向量
            /// </summary>
            /// <param name="radian">表示在平面直角坐标系中的一个弧度值</param>
            /// <returns><paramref name="radian"/>对应的从原点出发的单位向量</returns>
            public static Point2 RadianToVector(double radian)
            {
                return new Point2(Math.Cos(radian), Math.Sin(radian));
            }

        }

    }

}
