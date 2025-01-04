using System;
using System.Collections.Generic;
using System.Text;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.Algorithm
{

    public unsafe static partial class Maths
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
        /// <param name="position">空间坐标系中的点</param>
        /// <param name="vector">长度为1的方向向量</param>
        /// <param name="radian">旋转弧度</param>
        /// <returns>旋转后的点</returns>
        public static Point3 RotationByVector3(Point3 position, Point3 vector, double radian)
        {

            var c = global::System.Math.Cos(radian);
            var s = global::System.Math.Sin(radian);

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
        }

        #endregion
    }

}
