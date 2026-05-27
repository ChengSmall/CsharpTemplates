using Cheng.Algorithm.Randoms.Extends;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Randoms.Point
{



    /// <summary>
    /// 柏林噪声随机点生成器
    /// </summary>
    public sealed class PloRandom : IPointRandom
    {

        #region 结构

        /// <summary>
        /// Perm值提取器
        /// </summary>
        public sealed class PermGetting
        {

            /// <summary>
            /// 初始化数组
            /// </summary>
            /// <param name="bytes">元素为256且值不重复的字节数组</param>
            /// <exception cref="ArgumentException">元素数不为256</exception>
            public PermGetting(byte[] bytes)
            {
                if (bytes is null) throw new ArgumentNullException();
                if (bytes.Length != 256) throw new ArgumentException();

                this.bytes = bytes;
                this.length = bytes.Length;
            }

            private readonly byte[] bytes;
            private readonly int length;

            /// <summary>
            /// 访问或设置指定索引下的值
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public byte this[int index]
            {
                get
                {
                    return bytes[index % length];
                }
                set
                {
                    bytes[index % length] = value;
                }
            }

            /// <summary>
            /// 访问内部封装数组
            /// </summary>
            public byte[] Array
            {
                get => bytes;
            }
        }

        #endregion

        #region 参数

        /// <summary>
        /// 实例化柏林噪声算法随机向量生成器，使用随机器随机指定<see cref="PloArray"/>数组
        /// </summary>
        /// <param name="random">用于填充柏林向量随机集合的随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public PloRandom(BaseRandom random)
        {
            if (random is null) throw new ArgumentNullException("random", Cheng.Properties.Resources.Exception_ArgNullError);

            var perm = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                perm[i] = (byte)i;
            }
            random.RandomDisrupt<byte>(perm, 0, 256, 2);
            f_init(perm);
        }

        /// <summary>
        /// 指定柏林噪声的向量指向随机集合
        /// </summary>
        /// <param name="perm">长度是256且元素不重复的数组</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">数组元素数不是256个</exception>
        public PloRandom(byte[] perm)
        {
            f_init(perm);
        }

        private void f_init(byte[] perm)
        {
            p_getPerm = new PermGetting(perm);
        }

        private PermGetting p_getPerm;

        #endregion

        #region 访问

        /// <summary>
        /// 获取PLO随机数集合
        /// </summary>
        /// <returns>一个元素数为256且无重复的byte数组</returns>
        public PermGetting Perm
        {
            get => p_getPerm;
        }
        
        #endregion

        #region 方法

        static double f_fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        static double f_grad(int hash, double x, double y, double z)
        {
            switch (hash & 0xF)
            {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                case 0x4: return x + z;
                case 0x5: return -x + z;
                case 0x6: return x - z;
                case 0x7: return -x - z;
                case 0x8: return y + z;
                case 0x9: return -y + z;
                case 0xA: return y - z;
                case 0xB: return -y - z;
                case 0xC: return y + x;
                case 0xD: return -y + z;
                case 0xE: return y - x;
                default: return -y - z;
                //default:
                //    throw new ArgumentException();
            }
        }

        static double f_lerp(double a, double b, double t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>[0,1]的浮点值</returns>
        public double Generate(double x, double y, double z)
        {
            //计算输入点所在的“单位立方体”。0xff = 255

            var X = (int)System.Math.Floor(x) & 0xff;
            var Y = (int)System.Math.Floor(y) & 0xff;
            var Z = (int)System.Math.Floor(z) & 0xff;

            //左边界为(|x|,|y|,|z|)，右边界为 +1。接下来计算出该点在立方体中的位置(0.0~1.0)。

            x -= System.Math.Floor(x);
            y -= System.Math.Floor(y);
            z -= System.Math.Floor(z);

            var u = f_fade(x);
            var v = f_fade(y);
            var w = f_fade(z);

            var A = (p_getPerm[X] + Y) & 0xff;
            var B = (p_getPerm[X + 1] + Y) & 0xff;
            var AA = (p_getPerm[A] + Z) & 0xff;
            var BA = (p_getPerm[B] + Z) & 0xff;
            var AB = (p_getPerm[A + 1] + Z) & 0xff;
            var BB = (p_getPerm[B + 1] + Z) & 0xff;

            var AAA = p_getPerm[AA];
            var BAA = p_getPerm[BA];
            var ABA = p_getPerm[AB];
            var BBA = p_getPerm[BB];
            var AAB = p_getPerm[AA + 1];
            var BAB = p_getPerm[BA + 1];
            var ABB = p_getPerm[AB + 1];
            var BBB = p_getPerm[BB + 1];

            double x1, x2, y1, y2;
            x1 = f_lerp(f_grad(AAA, x, y, z), f_grad(BAA, x - 1, y, z), u);
            x2 = f_lerp(f_grad(ABA, x, y - 1, z), f_grad(BBA, x - 1, y - 1, z), u);
            y1 = f_lerp(x1, x2, v);

            x1 = f_lerp(f_grad(AAB, x, y, z - 1), f_grad(BAB, x - 1, y, z - 1), u);
            x2 = f_lerp(f_grad(ABB, x, y - 1, z - 1), f_grad(BBB, x - 1, y - 1, z - 1), u);
            y2 = f_lerp(x1, x2, v);

            return (f_lerp(y1, y2, w) + 1) / 2;

        }

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值，规定倍频和振幅
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="octave">倍频，最小为1</param>
        /// <param name="persistence">振幅，默认为0.5</param>
        /// <returns>[0,1]的浮点值</returns>
        /// <exception cref="ArgumentOutOfRangeException">倍频小于或等于0</exception>
        public double Generate(double x, double y, double z, int octave, double persistence)
        {
            if (octave <= 0) throw new ArgumentOutOfRangeException(nameof(octave));

            double total = 0.0;
            double frequency = 1;
            double amplitude = 1;
            //用于将结果归一化

            double maxValue = 0;
            for (int i = 0; i < octave; i++)
            {
                total += amplitude * Generate(x * frequency, y * frequency, z * frequency);
                maxValue += amplitude;
                frequency *= 2;
                amplitude *= persistence;
            }
            return total / maxValue;
        }

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值，规定倍频和振幅
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="octave">倍频，最小为1</param>
        /// <returns>[0,1]的浮点值</returns>
        /// <exception cref="ArgumentOutOfRangeException">倍频小于或等于0</exception>
        public double Generate(double x, double y, double z, int octave)
        {
            return Generate(x, y, z, octave, 0.5);
        }

        #endregion

        #region 外部api

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值
        /// </summary>
        /// <param name="perm">perm数组</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>[0,1]的浮点值</returns>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static double Generate(PermGetting perm, double x, double y, double z)
        {
            if (perm is null) throw new ArgumentNullException();
            //计算输入点所在的“单位立方体”。0xff = 255

            var X = (int)System.Math.Floor(x) & 0xff;
            var Y = (int)System.Math.Floor(y) & 0xff;
            var Z = (int)System.Math.Floor(z) & 0xff;

            //左边界为(|x|,|y|,|z|)，右边界为 +1。接下来计算出该点在立方体中的位置(0.0~1.0)。

            x -= System.Math.Floor(x);
            y -= System.Math.Floor(y);
            z -= System.Math.Floor(z);

            var u = f_fade(x);
            var v = f_fade(y);
            var w = f_fade(z);

            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;
            var AA = (perm[A] + Z) & 0xff;
            var BA = (perm[B] + Z) & 0xff;
            var AB = (perm[A + 1] + Z) & 0xff;
            var BB = (perm[B + 1] + Z) & 0xff;

            var AAA = perm[AA];
            var BAA = perm[BA];
            var ABA = perm[AB];
            var BBA = perm[BB];
            var AAB = perm[AA + 1];
            var BAB = perm[BA + 1];
            var ABB = perm[AB + 1];
            var BBB = perm[BB + 1];

            double x1, x2, y1, y2;
            x1 = f_lerp(f_grad(AAA, x, y, z), f_grad(BAA, x - 1, y, z), u);
            x2 = f_lerp(f_grad(ABA, x, y - 1, z), f_grad(BBA, x - 1, y - 1, z), u);
            y1 = f_lerp(x1, x2, v);

            x1 = f_lerp(f_grad(AAB, x, y, z - 1), f_grad(BAB, x - 1, y, z - 1), u);
            x2 = f_lerp(f_grad(ABB, x, y - 1, z - 1), f_grad(BBB, x - 1, y - 1, z - 1), u);
            y2 = f_lerp(x1, x2, v);

            return (f_lerp(y1, y2, w) + 1) / 2;

        }

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值，规定倍频和振幅
        /// </summary>
        /// <param name="perm">perm数组</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="octave">倍频，最小为1</param>
        /// <param name="persistence">振幅，默认为0.5</param>
        /// <returns>[0,1]的浮点值</returns>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">倍频等于或小于0</exception>
        public static double Generate(PermGetting perm, double x, double y, double z, int octave, double persistence)
        {
            if (octave <= 0) throw new ArgumentOutOfRangeException(nameof(octave));

            double total = 0.0;
            double frequency = 1;
            double amplitude = 1;
            //用于将结果归一化

            double maxValue = 0;
            for (int i = 0; i < octave; i++)
            {
                total += amplitude * Generate(perm, x * frequency, y * frequency, z * frequency);
                maxValue += amplitude;
                frequency *= 2;
                amplitude *= persistence;
            }
            return total / maxValue;
        }

        /// <summary>
        /// 根据指定坐标返回一个范围[0,1]的浮点值，规定倍频和振幅
        /// </summary>
        /// <param name="perm">perm数组</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="octave">倍频，最小为1</param>
        /// <returns>[0,1]的浮点值</returns>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">倍频等于或小于0</exception>
        public static double Generate(PermGetting perm, double x, double y, double z, int octave)
        {
            return Generate(perm, x, y, z, octave, 0.5);
        }

        #endregion

    }


}
