using System;
using System.Runtime.InteropServices;

namespace Cheng.DataStructure.Colors
{

    /// <summary>
    /// 一个通用的RGB颜色结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Colour : IEquatable<Colour>
    {

        #region 构造

        /// <summary>
        /// 初始化颜色，指定RGB参数
        /// </summary>
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
        public readonly byte r;
        /// <summary>
        /// RGB颜色值的绿色通道
        /// </summary>
        public readonly byte g;
        /// <summary>
        /// RGB颜色值的蓝色通道
        /// </summary>
        public readonly byte b;
        /// <summary>
        /// alpha参数；可表示透明度
        /// </summary>
        public readonly byte a;
        #endregion

        #region 功能

        #region 参数访问        

        /// <summary>
        /// 返回一个重新分配a色值的颜色结构
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
        /// <returns>表示rgb颜色值的32位整数</returns>
        public uint ToInt32()
        {
            return ((uint)b << 16) | ((uint)g << 8) | ((uint)r) | ((uint)a << 24);
        }

        /// <summary>
        /// 将32位颜色值转化为颜色结构
        /// </summary>
        /// <param name="rgb">表示rgb颜色值的32位整数</param>
        /// <returns>一个颜色结构</returns>
        public static Colour ToColor(uint rgb)
        {
            return new Colour((byte)((rgb) & byte.MaxValue), (byte)((rgb >> 8) & byte.MaxValue), (byte)((rgb >> 16) & byte.MaxValue), (byte)((rgb >> 24) & byte.MaxValue));
        }

        #endregion

        #region 颜色

        static double f_max(double a, double b, double c)
        {
            if (a > b)
                return a > c ? a : c;

            return b > c ? b : c;
        }
        static double f_min(double a, double b, double c)
        {
            if (a < b)
                return b < c ? b : c;

            return a < c ? a : c;
        }

        /// <summary>
        /// 获取HSL色值中的色相
        /// </summary>
        /// <value>值的范围在[0,360]</value>
        /// <exception cref="ArgumentOutOfRangeException">设置的参数超出范围</exception>
        public double H
        {
            get
            {
                double rf, gf, bf;

                rf = r / 255f;
                gf = g / 255f;
                bf = b / 255f;

                double max = f_max(rf, gf, bf);
                double min = f_min(rf, gf, bf);

                double dt = max - min;

                if (dt == 0)
                {
                    return 0;
                }
                if (max == rf)
                {
                    return 60f * (((gf - bf) / dt) % 6f);
                }
                if (max == gf)
                {
                    return 60f * (((bf - rf) / dt) + 2f);
                }
                else /*if (max == bf)*/
                {
                    return 60f * (((rf - gf) / dt) + 4f);
                }
            }
        }

        /// <summary>
        /// 获取HSL色值中的饱和度
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">设置的参数超出范围</exception>
        public double S
        {
            get
            {
                double rf, gf, bf;

                rf = r / 255f;
                gf = g / 255f;
                bf = b / 255f;

                double max = f_max(rf, gf, bf);
                double min = f_min(rf, gf, bf);

                double dt = max - min;

                if (dt == 0) return 0;
                return dt / (1f - System.Math.Abs(2f * L - 1f));
            }
        }

        /// <summary>
        /// 获取HSL色值中的亮度
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">设置的参数超出范围</exception>
        public double L
        {
            get
            {
                double rf, gf, bf;

                rf = r / 255f;
                gf = g / 255f;
                bf = b / 255f;

                var max = f_max(rf, gf, bf);
                var min = f_min(rf, gf, bf);

                return (max + min) / 2f;
            }
        }

        /// <summary>
        /// 把RGB颜色值转化为HSL色值
        /// </summary>
        /// <param name="H">色相，范围在[0,360)</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="L">亮度，范围在[0,1]</param>
        public void ToHSL(out double H, out double S, out double L)
        {

            double rf, gf, bf;

            rf = r / 255d;
            gf = g / 255d;
            bf = b / 255d;

            var max = f_max(rf, gf, bf);
            var min = f_min(rf, gf, bf);

            var dt = max - min;

            L = (max + min) / 2f;

            if (dt == 0) S = 0;
            else S = dt / (1f - System.Math.Abs(2f * L - 1f));


            if (dt == 0)
            {
                H = 0;
            }
            else if (max == rf)
            {
                H = 60f * (((gf - bf) / dt) % 6f);
            }
            else if (max == gf)
            {
                H = 60f * (((bf - rf) / dt) + 2f);
            }
            else /*if (max == bf)*/
            {
                H = 60f * (((rf - gf) / dt) + 4f);
            }
        }

        /// <summary>
        /// 使用HSL色值实例化一个颜色结构
        /// </summary>
        /// <param name="H">色相，范围在[0,360]</param>
        /// <param name="S">饱和度，范围在[0,1]</param>
        /// <param name="L">亮度，范围在[0,1]</param>
        /// <returns>转化后的颜色结构</returns>
        /// <exception cref="ArgumentOutOfRangeException">HSL参数超出指定范围</exception>
        public static Colour HSLToColor(double H, double S, double L)
        {
            if (S < 0 || S > 1 || L < 0 || L > 1 || H < 0 || H > 360d) throw new ArgumentOutOfRangeException();
            var C = (1d - System.Math.Abs(2 * L - 1d)) * S;
            var X = C * (1d - System.Math.Abs(((H / 60d) % 2d) - 1d));
            var m = L - (C / 2d);

            double rf, gf, bf;

            if (0f <= H && H < 60f)
            {
                rf = C;
                gf = X;
                bf = 0;
            }
            else if (H < 120f)
            {
                rf = X;
                gf = C;
                bf = 0;
            }
            else if (H < 180f)
            {
                rf = 0;
                gf = C;
                bf = X;
            }
            else if (H < 240f)
            {
                rf = 0;
                gf = X;
                bf = C;
            }
            else if (H < 300f)
            {
                rf = X;
                gf = 0;
                bf = C;
            }
            else if (H <= 360f)
            {
                rf = C;
                gf = 0;
                bf = X;
            }
            else throw new ArgumentException();

            return new Colour((byte)((rf + m) * 255), (byte)((gf + m) * 255), (byte)((bf + m) * 255));

        }

        #endregion

        #region 派生
        /// <summary>
        /// 以字符串返回RPG颜色值
        /// </summary>
        /// <returns>(r,g,b)</returns>
        public override string ToString()
        {
            return "(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + ")";
        }

        /// <summary>
        /// 返回指定格式颜色
        /// </summary>
        /// <param name="format">格式参数；参数为"RGB"时以RGB三色值返回，参数是"HSL"时以HSL色值返回，参数是"value"时返回32位整数型色值</param>
        /// <returns>指定格式颜色</returns>
        public string ToString(string format)
        {
            if (format == "rgb" || format == "RGB") return ToString();
            if(format == "HSL" || format == "hsl")
            {
                double H, S, L;
                ToHSL(out H, out S, out L);
                return "(H:" + H.ToString() + ",S:" + S.ToString() + ",L:" + L.ToString() + ")";
            }
            if (format == "value") return ToInt32().ToString("x8").ToUpper();

            return ToString();
        }

        /// <summary>
        /// 比较两个颜色结构是否相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Colour c1, Colour c2)
        {
            return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
        }

        /// <summary>
        /// 比较两个颜色结构是否不相等
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Colour c1, Colour c2)
        {
            return c1.r != c2.r || c1.g != c2.g || c1.b != c2.b || c1.a != c2.a;
        }

        /// <summary>
        /// 比较两个颜色结构是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Colour other)
        {
            return this == other;
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
            return (int)ToInt32();
        }

        #endregion

        #endregion

    }

}
