using Cheng.Algorithm;
using Cheng.Texts;
using System;
using System.Runtime.InteropServices;

namespace Cheng.DataStructure.Colors
{

    /// <summary>
    /// 一个通用的RGB颜色结构
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Colour : IEquatable<Colour>, IFormattable
    {

        #region 构造

        /// <summary>
        /// 初始化颜色，指定RGB参数
        /// </summary>
        /// <remarks>参数<see cref="a"/>默认为255</remarks>
        /// <param name="r">RGB颜色值的红色通道</param>
        /// <param name="g">RGB颜色值的绿色通道</param>
        /// <param name="b">RGB颜色值的蓝色通道</param>
        public Colour(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = byte.MaxValue;
        }

        /// <summary>
        /// 初始化颜色，指定RGB参数
        /// </summary>
        /// <param name="r">RGB颜色值的红色通道</param>
        /// <param name="g">RGB颜色值的绿色通道</param>
        /// <param name="b">RGB颜色值的蓝色通道</param>
        /// <param name="a">该参数为额外参数，表示透明度，若作为标准的rgb颜色将不会考虑该参数</param>
        public Colour(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        #endregion

        #region 参数

        /// <summary>
        /// RGB颜色值的红色通道
        /// </summary>
        public byte r;

        /// <summary>
        /// RGB颜色值的绿色通道
        /// </summary>
        public byte g;

        /// <summary>
        /// RGB颜色值的蓝色通道
        /// </summary>
        public byte b;

        /// <summary>
        /// alpha参数；可表示透明度
        /// </summary>
        public byte a;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 使用归一化参数访问或设置颜色的<see cref="r"/>参数
        /// </summary>
        /// <value>范围在[0,1]的浮点值，超出范围可能会导致预期外的结果</value>
        public float R01
        {
            get => (float)r / byte.MaxValue;
            set => r = (byte)(value * 255);
        }

        /// <summary>
        /// 使用归一化参数访问或设置颜色的<see cref="g"/>参数
        /// </summary>
        /// <value>范围在[0,1]的浮点值，超出范围可能会导致预期外的结果</value>
        public float G01
        {
            get => (float)g / byte.MaxValue;
            set => g = (byte)(value * 255);
        }

        /// <summary>
        /// 使用归一化参数访问或设置颜色的<see cref="b"/>参数
        /// </summary>
        /// <value>范围在[0,1]的浮点值，超出范围可能会导致预期外的结果</value>
        public float B01
        {
            get => (float)b / byte.MaxValue;
            set => b = (byte)(value * 255);
        }

        /// <summary>
        /// 使用归一化参数访问或设置颜色的<see cref="a"/>参数
        /// </summary>
        /// <value>范围在[0,1]的浮点值，超出范围可能会导致预期外的结果</value>
        public float Alpha01
        {
            get => (float)a / byte.MaxValue;
            set => a = (byte)(value * 255);
        }

        /// <summary>
        /// 返回一个重新分配alpha值的颜色结构
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Colour SetA(byte a)
        {
            return new Colour(r, g, b, a);
        }

        /// <summary>
        /// 返回一个重新分配r色值的颜色结构
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public Colour SetR(byte r)
        {
            return new Colour(r, g, b, a);
        }

        /// <summary>
        /// 返回一个重新分配b色值的颜色结构
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Colour SetB(byte b)
        {
            return new Colour(r, g, b, a);
        }

        /// <summary>
        /// 返回一个重新分配g色值的颜色结构
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public Colour SetG(byte g)
        {
            return new Colour(r, g, b, a);
        }

        #endregion

        #region 数据

        /// <summary>
        /// 将颜色结构转化为32位颜色值
        /// </summary>
        /// <returns>表示rgba颜色值的32位整数</returns>
        public uint ToInt32()
        {
            return ToARGB();
        }

        /// <summary>
        /// 将32位颜色值转化为颜色结构
        /// </summary>
        /// <param name="rgb">表示rgb颜色值的32位整数</param>
        /// <returns>颜色结构</returns>
        public static Colour ToColor(uint rgb)
        {
            return ToColorByARGB(rgb);
        }

        /// <summary>
        /// 将颜色值转化为ARGB格式的32位整数值
        /// </summary>
        /// <returns>位格式为ARGB的整数</returns>
        public uint ToARGB()
        {
            return ((uint)r << 16) | ((uint)g << 8) | ((uint)b) | ((uint)a << 24);
        }

        /// <summary>
        /// 使用位格式为ARGB的整数初始化颜色值
        /// </summary>
        /// <param name="argb">位格式为ARGB的整数</param>
        /// <returns>颜色结构</returns>
        public static Colour ToColorByARGB(uint argb)
        {
            return new Colour((byte)((argb >> 16) & byte.MaxValue), (byte)((argb >> 8) & byte.MaxValue), (byte)((argb) & byte.MaxValue), (byte)((argb >> 24) & byte.MaxValue));
        }

        #endregion

        #region 颜色

        #region ToHSLV

        /// <summary>
        /// 把RGB颜色值转化为HSL色值
        /// </summary>
        /// <param name="H">色相，范围在[0,1]</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="L">亮度，范围在[0,1]</param>
        public void ToHSL(out double H, out double S, out double L)
        {
            RGBToHSL(r, g, b, out H, out S, out L);
        }

        /// <summary>
        /// 把RGB颜色值转化为HSV色值
        /// </summary>
        /// <param name="H">色相，范围在[0,1]</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="V">明度，范围在[0,1]</param>
        public void ToHSV(out double H, out double S, out double V)
        {
            RGBToHSV(r, g, b, out H, out S, out V);
        }

        /// <summary>
        /// 将RGB颜色参数转化为HSL颜色模型
        /// </summary>
        /// <param name="R">红色颜色分量</param>
        /// <param name="G">绿色颜色分量</param>
        /// <param name="B">蓝色颜色分量</param>
        /// <param name="H">转化后的色相，范围在[0,1]</param>
        /// <param name="S">转化后的饱和度，范围在[0,1]</param>
        /// <param name="L">转化后的亮度，范围在[0,1]</param>
        public static void RGBToHSL(byte R, byte G, byte B, out double H, out double S, out double L)
        {
            // 将RGB分量归一化到[0,1]
            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;
            double max, min;
            //max = Math.Max(r, Math.Max(g, b));
            //min = Math.Min(r, Math.Min(g, b));
            max = Maths.Max(r, g, b);
            min = Maths.Min(r, g, b);

            // 计算亮度L
            L = (max + min) / 2.0;

            if (max == min)
            {
                // 灰度颜色，无色调和饱和度
                H = 0;
                S = 0;
            }
            else
            {
                double delta = max - min;

                // 计算饱和度S
                S = (L < 0.5) ? (delta / (max + min)) : (delta / (2.0 - max - min));

                // 计算色调H
                if (max == r)
                {
                    H = (g - b) / delta + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    H = (b - r) / delta + 2;
                }
                else // max == b
                {
                    H = (r - g) / delta + 4;
                }

                H /= 6; // 转换为[0,1)范围

                // 确保H在[0,1)范围内
                if (H < 0) H += 1.0;
            }
        }

        /// <summary>
        /// 将RGB颜色参数转化为HSL颜色模型
        /// </summary>
        /// <param name="R">红色颜色分量</param>
        /// <param name="G">绿色颜色分量</param>
        /// <param name="B">蓝色颜色分量</param>
        /// <param name="H">转化后的色相，范围在[0,1]</param>
        /// <param name="S">转化后的饱和度，范围在[0,1]</param>
        /// <param name="V">转化后的明度，范围在[0,1]</param>
        public static void RGBToHSV(byte R, byte G, byte B, out double H, out double S, out double V)
        {
            // 将RGB分量归一化到[0,1]
            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;

            //double max = Math.Max(r, Math.Max(g, b));
            //double min = Math.Min(r, Math.Min(g, b));
            double max = Maths.Max(r, g, b);
            double min = Maths.Min(r, g, b);

            // 计算明度V
            V = max;

            if (max == 0)
            {
                // 黑色，无色调和饱和度
                H = 0;
                S = 0;
            }
            else
            {
                // 计算饱和度S
                S = (max - min) / max;

                // 计算色调H
                if (max == min)
                {
                    H = 0;
                }
                else
                {
                    double delta = max - min;
                    if (max == r)
                    {
                        H = (g - b) / delta + (g < b ? 6 : 0);
                    }
                    else if (max == g)
                    {
                        H = (b - r) / delta + 2;
                    }
                    else // max == b
                    {
                        H = (r - g) / delta + 4;
                    }

                    H /= 6; // 转换为[0,1)范围
                }

                // 确保H在[0,1)范围内
                if (H < 0) H += 1.0;
            }
        }

        #endregion

        #region toRGB

        /// <summary>
        /// 使用HSL颜色模型实例化一个颜色结构
        /// </summary>
        /// <param name="H">色相，范围在[0,1]</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="L">亮度，范围在[0,1]</param>
        /// <returns>转化后的颜色结构，<see cref="Colour.a"/>始终为255</returns>
        public static Colour HSLToColor(double H, double S, double L)
        {
            Colour c;
            c.a = byte.MaxValue;
            HSLToRGB(H, S, L, out c.r, out c.g, out c.b);
            return c;
        }

        /// <summary>
        /// 将HSL颜色模型转化为RGB颜色分量
        /// </summary>
        /// <remarks>若参数不在正确的范围，可能会输出意外结果</remarks>
        /// <param name="H">色相，范围区间在[0,1]</param>
        /// <param name="S">饱和度，范围区间在[0,1]</param>
        /// <param name="L">亮度，范围区间在[0,1]</param>
        /// <param name="R">转化为RGB的红色分量</param>
        /// <param name="G">转化为RGB的绿色分量</param>
        /// <param name="B">转化为RGB的蓝色分量</param>
        public static void HSLToRGB(double H, double S, double L, out byte R, out byte G, out byte B)
        {
            if (S == 0)
            {
                byte value = (byte)(L * 255);
                R = value;
                G = value;
                B = value;
                return;
            }

            double chroma = (1 - Math.Abs(2 * L - 1)) * S;
            double H_prime = (H * 360) / 60;

            int sector = (int)Math.Floor(H_prime);
            double intermediate = chroma * (1 - Math.Abs(H_prime % 2 - 1));

            double redTemp, greenTemp, blueTemp;

            switch (sector % 6)
            {
                case 0:
                    redTemp = chroma;
                    greenTemp = intermediate;
                    blueTemp = 0;
                    break;
                case 1:
                    redTemp = intermediate;
                    greenTemp = chroma;
                    blueTemp = 0;
                    break;
                case 2:
                    redTemp = 0;
                    greenTemp = chroma;
                    blueTemp = intermediate;
                    break;
                case 3:
                    redTemp = 0;
                    greenTemp = intermediate;
                    blueTemp = chroma;
                    break;
                case 4:
                    redTemp = intermediate;
                    greenTemp = 0;
                    blueTemp = chroma;
                    break;
                default: // case 5
                    redTemp = chroma;
                    greenTemp = 0;
                    blueTemp = intermediate;
                    break;
            }

            double m = L - chroma / 2;
            //double r = Math.Max(0, Math.Min(1, redTemp + m));
            //double g = Math.Max(0, Math.Min(1, greenTemp + m));
            //double b = Math.Max(0, Math.Min(1, blueTemp + m));

            R = (byte)(Math.Max(0, Math.Min(1, redTemp + m)) * 255);
            G = (byte)(Math.Max(0, Math.Min(1, greenTemp + m)) * 255);
            B = (byte)(Math.Max(0, Math.Min(1, blueTemp + m)) * 255);

        }

        /// <summary>
        /// 使用HSV颜色模型实例化一个颜色结构
        /// </summary>
        /// <remarks>若参数不在正确的范围，可能会输出意外结果</remarks>
        /// <param name="H">色相，范围在[0,1]</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="V">明度，范围在[0,1]</param>
        /// <returns>转化后的颜色结构，<see cref="Colour.a"/>始终为255</returns>
        public static Colour HSVToColor(double H, double S, double V)
        {
            Colour c;
            c.a = byte.MaxValue;
            HSVToRGB(H, S, V, out c.r, out c.g, out c.b);
            return c;
        }

        /// <summary>
        /// 将HSV颜色模型转化为RGB颜色分量
        /// </summary>
        /// <remarks>若参数不在正确的范围，可能会输出意外结果</remarks>
        /// <param name="H">色相，范围区间在[0,1]</param>
        /// <param name="S">饱和度，范围区间在[0,1]</param>
        /// <param name="V">明度，范围区间在[0,1]</param>
        /// <param name="R">转化为RGB的红色分量</param>
        /// <param name="G">转化为RGB的绿色分量</param>
        /// <param name="B">转化为RGB的蓝色分量</param>
        public static void HSVToRGB(double H, double S, double V, out byte R, out byte G, out byte B)
        {
            //实现
            if (S == 0)
            {
                // 灰度情况，RGB分量均等于亮度V
                byte value = (byte)(V * 255 + 0.5);
                R = value;
                G = value;
                B = value;
                return;
            }

            double h = H * 6;
            int sector = (int)Math.Floor(h);
            double fraction = h - sector;
            double p = V * (1 - S);
            double q = V * (1 - S * fraction);
            double t = V * (1 - S * (1 - fraction));

            double r, g, b;
            switch (sector % 6)
            {
                case 0:
                    r = V;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = V;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = V;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = V;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = V;
                    break;
                default: // case 5
                    r = V;
                    g = p;
                    b = q;
                    break;
            }

            // 分量转换并四舍五入
            R = (byte)(r * 255 + 0.5);
            G = (byte)(g * 255 + 0.5);
            B = (byte)(b * 255 + 0.5);
        }

        #endregion

        #region 透明度

        /// <summary>
        /// 使用归一化参数置透明度并返回新的实例
        /// </summary>
        /// <param name="alpha">要设置的透明度，范围在[0,1]</param>
        /// <returns>新的透明度实例</returns>
        public Colour SetAlpha01(float alpha)
        {
            return new Colour(r, g, b, (byte)(alpha * 255f));
        }

        /// <summary>
        /// 将当前颜色与指定颜色进行透明度叠加
        /// </summary>
        /// <remarks>
        /// <para>将当前颜色的<see cref="a"/>值当作透明参数，与指定的颜色<paramref name="backColor"/>叠加，返回按照透明度叠加的新颜色</para>
        /// </remarks>
        /// <param name="backColor">指定的背景色</param>
        /// <returns>按照透明度叠加的新颜色；<see cref="a"/>是最大值</returns>
        public Colour ColorSynthesis(Colour backColor)
        {
            if (this.a == 0) return backColor;
            if (this.a == byte.MaxValue) return this;
            float a = this.a / 255f;
            Colour re;
            re.a = byte.MaxValue;
            re.r = (byte)((a * this.r) + ((1 - a) * backColor.r));
            re.g = (byte)((a * this.g) + ((1 - a) * backColor.g));
            re.b = (byte)((a * this.b) + ((1 - a) * backColor.b));
            return re;
        }

        /// <summary>
        /// 将当前颜色与另一个颜色进行颜色混合，返回新的混合后的颜色
        /// </summary>
        /// <remarks>
        /// <para>融合过程中将忽略透明通道，因此返回的新颜色的透明度永远是最大值</para>
        /// </remarks>
        /// <param name="other">另一个颜色</param>
        /// <returns>混合的新颜色</returns>
        public Colour Blending(Colour other)
        {
            //Colour c;
            //c.a = byte.MaxValue;
            //c.r = (byte)((((float)this.r) + ((float)other.r)) * 0.5f);
            //c.g = (byte)((((float)this.g) + ((float)other.g)) * 0.5f);
            //c.b = (byte)((((float)this.b) + ((float)other.b)) * 0.5f);
            //return c;

            //取得HSL模型
            this.ToHSL(out var thisH, out var thisS, out var thisL);
            other.ToHSL(out var otherH, out var otherS, out var otherL);

            //360弧度角
            const double allAngle = Maths.PI2;

            double TH, TS, TL;
            //收圈
            TH = Maths.Angles.TwoRadianCenterLine(allAngle * thisH, allAngle * otherH) / allAngle;

            TS = (thisS + otherS) / 2;
            TL = (thisL + otherL) / 2;

            return HSLToColor(TH, TS, TL);
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 以字符串返回RPG颜色值
        /// </summary>
        /// <returns>(r,g,b)</returns>
        public override string ToString()
        {
            return f_toStrDef(null);
        }

        /// <summary>
        /// 返回指定格式颜色
        /// </summary>
        /// <param name="format">格式参数；参数为"RGB"时以RGB三色值返回，参数是"HSL"时以HSL色值返回，参数是"value"时返回32位整数型色值；null使用默认的"RGB"格式</param>
        /// <returns>指定格式颜色</returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// 比较两个颜色结构是否相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Colour c1, Colour c2)
        {
            return (*((int*)&c1)) == (*((int*)&c2));
        }

        /// <summary>
        /// 比较两个颜色结构是否不相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Colour c1, Colour c2)
        {
            return (*((int*)&c1)) != (*((int*)&c2));
        }

        /// <summary>
        /// 比较两个颜色结构是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Colour other)
        {
            var t = this;
            return (*((int*)&t)) == (*((int*)&other));
        }

        /// <summary>
        /// 比较两个颜色结构是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is Colour c)
            {
                return this == c;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)ToARGB();
        }

        private string f_toStrDef(IFormatProvider formatProvider)
        {
            return "(" + r.ToString(formatProvider) + "," + g.ToString(formatProvider) + "," + b.ToString(formatProvider) + ")";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if(format is null) return f_toStrDef(formatProvider);
            format = TextManipulation.ToLower(format);
            double H, S, L;

            switch (format)
            {
                case "g":
                case "rgb":
                default:
                    return f_toStrDef(formatProvider);
                case "hsl":
                    ToHSL(out H, out S, out L);
                    return "(H:" + H.ToString() + ",S:" + S.ToString() + ",L:" + L.ToString() + ")";
                case "hsv":
                    ToHSV(out H, out S, out L);
                    return "(H:" + H.ToString() + ",S:" + S.ToString() + ",V:" + L.ToString() + ")";
                case "value":
                    var re = ToInt32();
                    char* cp = stackalloc char[8];
                    re.ValueToFixedX16Text(true, cp);
                    return new string(cp, 0, 8);
            }
        }

        #endregion

        #endregion

    }

}
