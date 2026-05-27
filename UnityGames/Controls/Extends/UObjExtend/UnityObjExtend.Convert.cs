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

using UColor = UnityEngine.Color;
using UColor32 = UnityEngine.Color32;

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
        public static Point3 ToPoint3(this Vector3 vector)
        {
            return new Point3(vector.x, vector.y);
        }

        /// <summary>
        /// 转化为<see cref="Vector2"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Point2F point)
        {
            return new Vector2((float)point.x, (float)point.y);
        }

        /// <summary>
        /// 转化为<see cref="Point2"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point2F ToPoint2F(this Vector2 vector)
        {
            return new Point2F(vector.x, vector.y);
        }

        /// <summary>
        /// 转化为<see cref="Vector3"/>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ToVector2(this Point3F point)
        {
            return new Vector3((float)point.x, (float)point.y);
        }

        /// <summary>
        /// 转化为<see cref="Point3"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point3F ToPoint3F(this Vector3 vector)
        {
            return new Point3F(vector.x, vector.y);
        }

        #endregion

        #region 颜色

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static UColor32 ToUColor32(this Colour color)
        {
            return new Color32(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Colour ToColour(this UColor32 color)
        {
            return new Colour(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static UColor ToUColor(this Colour color)
        {
            return new Color(color.R01, color.G01, color.B01, color.Alpha01);
        }

        /// <summary>
        /// 颜色转化
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Colour ToColour(this UColor color)
        {
            Color32 c32 = color;
            return new Colour(c32.r, c32.g, c32.b, c32.a);
        }

        /// <summary>
        /// 将32位颜色转化为rgb通用色值
        /// </summary>
        /// <param name="color">要转化的颜色</param>
        /// <returns>ARGB通用色值</returns>
        public static uint ColorToInt32(this UColor32 color)
        {
            return ((uint)color.a << 24) | ((uint)color.r << 16) | ((uint)color.g << 8) | color.b;
        }

        /// <summary>
        /// 将rgb通用色值转化为32位颜色
        /// </summary>
        /// <param name="rgbValue">要转化的通用ARGB色值</param>
        /// <returns>转化后的32位颜色</returns>
        public static UColor32 Int32ToColor(this uint rgbValue)
        {
            return new UColor32((byte)((rgbValue >> 16) & 0xFF), (byte)((rgbValue >> 8) & 0xFF), (byte)((rgbValue) & 0xFF), (byte)((rgbValue >> 24) & 0xFF));
        }

        /// <summary>
        /// 将颜色值转化为HSV颜色模型
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="H">色相</param>
        /// <param name="S">饱和度</param>
        /// <param name="V">亮度</param>
        public static void ToHSV(this UColor color, out float H, out float S, out float V)
        {
            UColor.RGBToHSV(color, out H, out S, out V);
        }

        /// <summary>
        /// 将颜色值转化为HSV颜色模型
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="H">色相</param>
        /// <param name="S">饱和度</param>
        /// <param name="V">亮度</param>
        public static void ToHSV(this UColor32 color, out float H, out float S, out float V)
        {
            UColor.RGBToHSV((UColor)color, out H, out S, out V);
        }

        #endregion

        #endregion

    }

}
