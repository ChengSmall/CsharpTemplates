using System;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 基于线性同余随机算法的随机数生成器
    /// </summary>
    public sealed class LICRandom : BaseRandom
    {

        #region 构造

        /// <summary>
        /// 实例化一个随机数生成器，使用计算机时间戳作为种子
        /// </summary>
        public LICRandom()
        {
            Seed = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// 使用指定随机种子实例化一个随机数生成器
        /// </summary>
        /// <param name="seed">种子</param>
        public LICRandom(long seed)
        {
            p_seed = seed;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 下一次的生成种子
        /// </summary>
        private long p_seed;

        #endregion

        #region 派生

        public sealed override bool CanGetSeed => true;

        public sealed override bool CanSetSeed => true;

        public sealed override SeedType SeedValueType => SeedType.Int64;

        public sealed override long SeedInt64
        {
            get => p_seed;
            set => p_seed = value;
        }

        /// <summary>
        /// 访问或设置当前随机器的随机数种子
        /// </summary>
        public long Seed
        {
            get => p_seed;
            set
            {
                p_seed = value;
            }
        }

        /// <summary>
        /// 获取一个随机整数
        /// </summary>
        /// <returns>一个随机的整数，范围在[0, 2147483647)</returns>
        public sealed override int Next()
        {
            var re = NextLong();
            if(re > int.MaxValue) return (int)((re / 1000) % int.MaxValue);
            return (int)re;
            //return (int)((NextLong() / 100) % int.MaxValue);
        }

        /// <summary>
        /// 获取一个随机长整型值
        /// </summary>
        /// <returns>一个随机得长整型值，范围在[0,9223372036854775807)；由于实现机制，该函数每次返回的值总是会奇偶交替</returns>
        public sealed override long NextLong()
        {
            const long M = 1103515245;
            const long G = 32767;

            p_seed = (p_seed * M + G);
            if (p_seed == long.MaxValue) return 0;

            if (p_seed < 0) return p_seed & 0x7FFFFFFFFFFFFFFF;
            return p_seed;
        }

        public sealed override float NextFloat()
        {
            return (Next() / 2147483647f);
        }

        public sealed override double NextDouble()
        {
            return Next() / 2147483647d;
        }

        /// <summary>
        /// 获取一个指定范围的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值是此值-1</param>
        /// <returns>
        /// 指定范围内的随机数
        /// </returns>
        public sealed override int Next(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)(min + ((Next()) % (max - min)));
        }

        /// <summary>
        /// 获取一个指定范围的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值是此值-1</param>
        /// <returns>
        /// 指定范围内的随机数；
        /// <para>由于实现机制，该函数当最大值只比最小值大1时，将不会出现重复两次返回同一个值的情况；因此若要生成不稳定的随机数时，请使用<see cref="Next(int, int)"/>
        /// </para>
        /// </returns>
        public sealed override long NextLong(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (NextLong() % (max - min));
        }

        public sealed override double NextDouble(double min, double max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + ((max - min) * (Next() / 2147483647d));
        }

        public sealed override float NextFloat(float min, float max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (float)(min + ((max - min) * (Next(0, 8388607) / 8388607f)));
        }

        #endregion

        #region 核心函数抽出

        /// <summary>
        /// 获取一个随机长整型值
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <returns>一个随机的长整型值，范围在[0,9223372036854775807)；由于实现机制，该函数每次返回的值总是会奇偶交替</returns>
        public static long NextLong(ref long seed)
        {
            const long M = 1103515245;
            const long G = 32767;

            seed = (seed * M + G);

            if (seed < 0) return seed & 0x7FFFFFFFFFFFFFFF;
            return seed;
        }

        /// <summary>
        /// 获取一个随机整型值
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <returns>一个随机的整数，范围在[0, 2147483647)</returns>
        public static int NextInt(ref long seed)
        {
            var re = NextLong(ref seed);
            if (re > int.MaxValue) return (int)((re / 1000) % int.MaxValue);
            return (int)re;
        }

        /// <summary>
        /// 获取一个指定范围的随机长整型值，范围在[<paramref name="min"/>,<paramref name="max"/>)
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值会小于该值</param>
        /// <returns>
        /// 指定范围内的随机数；
        /// <para>由于实现机制，该函数当最大值只比最小值大1时，将不会出现重复两次返回同一个值的情况；因此若要生成不稳定的随机数时，请使用<see cref="NextInt(ref long, int, int)"/>
        /// </para>
        /// </returns>
        public static long NextLong(ref long seed, long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (NextLong(ref seed) % (max - min));
        }

        /// <summary>
        /// 获取一个指定范围的随机整型值，范围在[<paramref name="min"/>,<paramref name="max"/>)
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值会小于该值</param>
        /// <returns>随机数</returns>
        public static int NextInt(ref long seed, int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)(min + ((NextInt(ref seed)) % (max - min)));
        }

        /// <summary>
        /// 获取一个范围在[0,1)的随机单精度浮点值
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <returns>范围在[0,1)的单精度浮点数</returns>
        public static float NextFloat(ref long seed)
        {
            return (NextInt(ref seed) / 2147483647f);
        }

        /// <summary>
        /// 获取一个范围在[0,1)的随机双精度浮点值
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <returns>范围在[0,1)的双精度浮点数</returns>
        public static double NextDouble(ref long seed)
        {
            return NextInt(ref seed) / 2147483647d;
        }

        /// <summary>
        /// 获取一个指定范围内的单精度浮点值
        /// </summary>
        /// <param name="seed">随机算法的种子值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值不会超过或等于该值</param>
        /// <returns>在范围[<paramref name="min"/>,<paramref name="max"/>)的随机数</returns>
        public static float NextFloat(ref long seed, float min, float max)
        {
            return (float)(min + (((max) - min) * (NextDouble(ref seed))));
        }

        #endregion

    }

}
