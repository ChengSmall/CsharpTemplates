using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SColor = System.Drawing.Color;

namespace Cheng.DataStructure.Colors
{

    /// <summary>
    /// 颜色结构与系统扩展
    /// </summary>
    public static class ColourExtend
    {

        #region 颜色转化

        /// <summary>
        /// 将<see cref="Colour"/>转化为等效的<see cref="SColor"/>值
        /// </summary>
        /// <param name="color"></param>
        /// <returns>与<paramref name="color"/>等效的<see cref="SColor"/>值</returns>
        public static SColor ToSysColor(this Colour color)
        {
            return SColor.FromArgb(color.a, color.r, color.g, color.b);
        }

        /// <summary>
        /// 将<see cref="SColor"/>转化为等效的<see cref="Colour"/>值
        /// </summary>
        /// <param name="color"></param>
        /// <returns>与<paramref name="color"/>等效的<see cref="Colour"/>值</returns>
        public static Colour ToColour(this SColor color)
        {
            return new Colour(color.R, color.G, color.B, color.A);
        }

        #endregion

    }

}
