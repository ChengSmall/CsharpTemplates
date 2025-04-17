using System;
using System.Collections.Generic;
using System.Text;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.Algorithm
{

    public unsafe static partial class Maths
    {

        /// <summary>
        /// 空间坐标系和三维向量
        /// </summary>
        public static class Vector3
        {

            /*
             new_x = (xx(1-c)+c) * old_x + (xy(1-c)-zs) * old_y + (xz(1-c)+ys) * old_z

            new_y = (yx(1-c)+zs) * old_x + (yy(1-c)+c) * old_y + (yz(1-c)-xs) * old_z

            new_z = (xz(1-c)-ys) * old_x + (yz(1-c)+xs) * old_y + (zz(1-c)+c) * old_z

            其中 c = cos(angle)，s = sin(angle)

            (old_x,old_y,old_z)是原来的点的坐标，(new_x,new_y,new_z)是旋转后的新的点的坐标
             */

            #region

            /// <summary>
            /// 将三维空间中的点围绕指定向量旋转
            /// </summary>
            /// <remarks>
            /// <para>
            /// 在空间直角坐标系中，有一个点<paramref name="position"/>；一个从坐标原点延伸出去的三维向量<paramref name="vector"/>；<br/>
            /// 现在，有一个与向量<paramref name="vector"/>垂直且包含点<paramref name="position"/>的平面，从向量延伸的方向观察平面，把点<paramref name="position"/>围绕向量指向的平面位置，以逆时针角度旋转<paramref name="radian"/>弧度，返回新得到的点
            /// </para>
            /// </remarks>
            /// <param name="position">空间坐标系中的点</param>
            /// <param name="vector">从原点向外延伸的向量</param>
            /// <param name="radian">旋转弧度</param>
            /// <returns>旋转后的点</returns>
            public static Point3 RotationByVector3(Point3 position, Point3 vector, double radian)
            {

                vector = vector.Normalized;

                #region

                var c = System.Math.Cos(radian);
                var s = System.Math.Sin(radian);

                var fx = 1 - c;

                double new_x, new_y, new_z;

                //(xx(1-c)+c) * old_x + (xy(1-c)-zs) * old_y + (xz(1-c)+ys) * old_z

                new_x = (((vector.x * vector.x * (fx)) + c) * position.x) +
                 (((vector.x * vector.y * (fx)) - (vector.z * s)) * (position.y)) +
                 (position.z * ((vector.x * vector.z * (fx)) + (vector.y * s)));

                //(yx(1-c)+zs) * old_x + (yy(1-c)+c) * old_y + (yz(1-c)-xs) * old_z

                new_y = (((vector.y * vector.x * (fx)) + (vector.z * s)) * position.x) +
                 (((vector.y * vector.y * (fx)) + (c)) * (position.y)) +
                 (position.z * ((vector.y * vector.z * (fx)) - (vector.x * s)));

                //(xz(1-c)-ys) * old_x + (yz(1-c)+xs) * old_y + (zz(1-c)+c) * old_z

                new_z = (((vector.x * vector.z * (fx)) - (vector.y * s)) * position.x) +
                (((vector.y * vector.z * (fx)) + (vector.x * s)) * (position.y)) +
                (((vector.z * vector.z * (fx)) + (c)) * position.z);

                return new Point3(new_x, new_y, new_z);

                #endregion

                #region

                //// 归一化旋转轴向量
                //double length = Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
                //if (length == 0)
                //{
                //    throw new ArgumentException();
                //}
                //double kx = vector.x / length;
                //double ky = vector.y / length;
                //double kz = vector.z / length;

                //double cosTheta = Math.Cos(radian);
                //double sinTheta = Math.Sin(radian);

                //// 计算点与旋转轴的点积
                ////double dotProduct = kx * position.x + ky * position.y + kz * position.z;

                //// 计算叉乘 k × position
                ////double crossX = ky * position.z - kz * position.y;
                ////double crossY = kz * position.x - kx * position.z;
                ////double crossZ = kx * position.y - ky * position.x;

                //// 应用罗德里格斯旋转公式
                //double term1X = position.x * cosTheta;
                //double term1Y = position.y * cosTheta;
                //double term1Z = position.z * cosTheta;

                //double term2X = (ky * position.z - kz * position.y) * sinTheta;
                //double term2Y = (kz * position.x - kx * position.z) * sinTheta;
                //double term2Z = (kx * position.y - ky * position.x) * sinTheta;
                //double factor = (kx * position.x + ky * position.y + kz * position.z) * (1 - cosTheta);
                ////double term3X = kx * factor;
                ////double term3Y = ky * factor;
                ////double term3Z = kz * factor;

                //// 计算新坐标
                //double newX = term1X + term2X + (kx * factor);
                //double newY = term1Y + term2Y + (ky * factor);
                //double newZ = term1Z + term2Z + (kz * factor);
                //return new Point3(newX, newY, newZ);

                #endregion

            }

            #endregion

        }


    }

}
