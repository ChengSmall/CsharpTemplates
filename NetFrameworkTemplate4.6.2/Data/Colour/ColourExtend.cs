using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using SColor = System.Drawing.Color;

namespace Cheng.DataStructure.Colors
{

    public static class ColourExtend
    {

        #region 颜色转化

        public static SColor ToSysColor(this Colour color)
        {
            return SColor.FromArgb(color.a, color.r, color.g, color.b);
        }

        public static Colour ToColour(this SColor color)
        {
            return new Colour(color.R, color.G, color.B, color.A);
        }

        #endregion

    }

}
