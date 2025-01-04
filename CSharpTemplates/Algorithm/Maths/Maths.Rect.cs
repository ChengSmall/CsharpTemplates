using System;
using System.Collections.Generic;
using System.Collections;

namespace Cheng.Algorithm
{

    public static partial class Maths
    {

        #region 正方形计算

        /// <summary>
        /// 根据正方形斜边求正方形边长
        /// </summary>
        /// <param name="hypotenuse">正方形所在对角线长度</param>
        /// <returns>正方形边长</returns>
        public static double SquareSideByHypotenuse(double hypotenuse)
        {
            return Math.Sqrt((hypotenuse * hypotenuse) / 2);
        }

        #endregion

        #region 矩形计算



        #endregion

    }

}
