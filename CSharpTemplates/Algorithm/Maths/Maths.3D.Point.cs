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

            #region

            /*
             new_x = (xx(1-c)+c) * old_x + (xy(1-c)-zs) * old_y + (xz(1-c)+ys) * old_z

            new_y = (yx(1-c)+zs) * old_x + (yy(1-c)+c) * old_y + (yz(1-c)-xs) * old_z

            new_z = (xz(1-c)-ys) * old_x + (yz(1-c)+xs) * old_y + (zz(1-c)+c) * old_z

            其中 c = cos(angle)，s = sin(angle)

            (old_x,old_y,old_z)是原来的点的坐标，(new_x,new_y,new_z)是旋转后的新的点的坐标
             */

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
                return RotationByVector3IgnoreDistance(position, vector.Normalized, radian);
            }

            /// <summary>
            /// 将三维空间中的点围绕指定向量旋转
            /// </summary>
            /// <remarks>
            /// <para>功能与<see cref="RotationByVector3(Point3, Point3, double)"/>一致，但是无视点<paramref name="position"/>与向量<paramref name="vector"/>的距离，仅计算相对角度，想要让结果准确，请确保向量<paramref name="vector"/>的长度为1</para>
            /// </remarks>
            /// <param name="position">空间坐标系中的点</param>
            /// <param name="vector">从原点向外延伸的向量</param>
            /// <param name="radian">旋转弧度</param>
            /// <returns>旋转后的点</returns>
            public static Point3 RotationByVector3IgnoreDistance(Point3 position, Point3 vector, double radian)
            {
                var c = System.Math.Cos(radian);
                var s = System.Math.Sin(radian);

                var fx = 1 - c;

                return new Point3((((vector.x * vector.x * (fx)) + c) * position.x) +
                 (((vector.x * vector.y * (fx)) - (vector.z * s)) * (position.y)) +
                 (position.z * ((vector.x * vector.z * (fx)) + (vector.y * s))),
                    (((vector.y * vector.x * (fx)) + (vector.z * s)) * position.x) +
                 (((vector.y * vector.y * (fx)) + (c)) * (position.y)) +
                 (position.z * ((vector.y * vector.z * (fx)) - (vector.x * s))),
                    (((vector.x * vector.z * fx) - (vector.y * s)) * position.x) +
                (((vector.y * vector.z * fx) + (vector.x * s)) * position.y) +
                (((vector.z * vector.z * fx) + c) * position.z)
                    );
                //double new_x, new_y, new_z;

                //new_x = (((vector.x * vector.x * (fx)) + c) * position.x) +
                // (((vector.x * vector.y * (fx)) - (vector.z * s)) * (position.y)) +
                // (position.z * ((vector.x * vector.z * (fx)) + (vector.y * s)));

                //new_y = (((vector.y * vector.x * (fx)) + (vector.z * s)) * position.x) +
                // (((vector.y * vector.y * (fx)) + (c)) * (position.y)) +
                // (position.z * ((vector.y * vector.z * (fx)) - (vector.x * s)));

                //new_z = (((vector.x * vector.z * (fx)) - (vector.y * s)) * position.x) +
                //(((vector.y * vector.z * (fx)) + (vector.x * s)) * (position.y)) +
                //(((vector.z * vector.z * (fx)) + (c)) * position.z);

                //return new Point3(new_x, new_y, new_z);
            }

            #endregion

        }


    }

}
