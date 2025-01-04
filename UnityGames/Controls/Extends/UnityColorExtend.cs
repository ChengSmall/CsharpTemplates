using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Cheng.Unitys.Colors
{

    /// <summary>
    /// Unity颜色扩展方法
    /// </summary>
    public static class UnityColorExtend
    {

        #region 颜色转化

        /// <summary>
        /// 将32位颜色转化为rgb通用色值
        /// </summary>
        /// <param name="color">要转化的颜色</param>
        /// <returns>rgb通用色值</returns>
        public static uint ColorToInt32(this Color32 color)
        {
            return ((uint)color.a << 24) | ((uint)color.b << 16) | ((uint)color.g << 8) | color.r;
        }
        /// <summary>
        /// 将rgb通用色值转化为32位颜色
        /// </summary>
        /// <param name="rgbValue">要转化的通用rgb色值</param>
        /// <returns>转化后的32位颜色</returns>
        public static Color32 Int32ToColor(this uint rgbValue)
        {
            return new Color32((byte)(rgbValue & 0xFF), (byte)((rgbValue >> 8) & 0xFF), (byte)((rgbValue >> 16) & 0xFF), (byte)((rgbValue >> 24) & 0xFF));
        }

        /// <summary>
        /// 将颜色值转化为HSV颜色模型
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="H">色相</param>
        /// <param name="S">饱和度</param>
        /// <param name="V">亮度</param>
        public static void ToHSV(this Color color, out float H, out float S, out float V)
        {
            Color.RGBToHSV(color, out H, out S, out V);
        }
        /// <summary>
        /// 将颜色值转化为HSV颜色模型
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="H">色相</param>
        /// <param name="S">饱和度</param>
        /// <param name="V">亮度</param>
        public static void ToHSV(this Color32 color, out float H, out float S, out float V)
        {
            Color.RGBToHSV((Color)color, out H, out S, out V);
        }

        #endregion

    }

}
