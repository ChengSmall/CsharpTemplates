using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cheng.DataStructure;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure.Colors;

using UObj = UnityEngine.Object;
using GObj = UnityEngine.GameObject;

namespace Cheng.Unitys
{

    unsafe static partial class UnityObjExtend
    {

        #region 值转化

        #region 坐标结构

        /// <summary>
        /// 转化为<see cref="Vector2"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Point2 point)
        {
            return new Vector2((float)point.x, (float)point.y);
        }

        /// <summary>
        /// 转化为<see cref="Point2"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2 ToPoint2(this Vector2 vector)
        {
            return new Point2(vector.x, vector.y);
        }

        /// <summary>
        /// 转化为<see cref="Vector3"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ToVector2(this Point3 point)
        {
            return new Vector3((float)point.x, (float)point.y);
        }

        /// <summary>
        /// 转化为<see cref="Point3"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3 ToPoint2(this Vector3 vector)
        {
            return new Point3(vector.x, vector.y);
        }

        #endregion

        #region 颜色

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color32 ToUColor32(this Colour color)
        {
            return new Color32(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Colour ToColour(this Color32 color)
        {
            return new Colour(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToUColor(this Colour color)
        {
            return new Color(color.R01, color.G01, color.B01, color.Alpha01);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Colour ToColour(this Color color)
        {
            Color32 c32 = color;
            return new Colour(c32.r, c32.g, c32.b, c32.a);
        }

        #endregion

        #endregion

    }

}
