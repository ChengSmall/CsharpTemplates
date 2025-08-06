using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 基于Xorshift128+算法的随机数生成器
    /// </summary>
    public sealed unsafe class XorshiftRandom : BaseRandom
    {

        #region 构造

        /// <summary>
        /// 初始化随机器
        /// </summary>
        /// <param name="defaultSeedValue">true表示将种子值初始化为0，false采用<see cref="Environment.TickCount"/>参数作为随机种子</param>
        public XorshiftRandom(bool defaultSeedValue)
        {
            f_setSeed(defaultSeedValue ? 0 : (ulong)Environment.TickCount);
        }

        /// <summary>
        /// 初始化随机器，并将随机种子设为0
        /// </summary>
        public XorshiftRandom()
        {
            f_setSeed(0);
        }

        /// <summary>
        /// 初始化随机器并使用指定种子
        /// </summary>
        /// <param name="seed">随机种子</param>
        public XorshiftRandom(long seed)
        {
            f_setSeed((ulong)seed);
        }

        /// <summary>
        /// 初始化随机器并使用指定种子
        /// </summary>
        /// <param name="seed">随机种子</param>
        public XorshiftRandom(ulong seed)
        {
            f_setSeed(seed);
        }

        /// <summary>
        /// 初始化随机器并使用指定种子
        /// </summary>
        /// <param name="seed">随机种子</param>
        public XorshiftRandom(int seed)
        {
            f_setSeed((ulong)seed);
        }

        #endregion

        #region 参数

        private ulong p_state0;

        private ulong p_state1;

        #endregion

        #region 功能

        public override bool CanSetSeed => true;

        public override ulong SeedUInt64 
        {
            set => f_setSeed(value);
        }

        public override long SeedInt64 
        {
            set => f_setSeed((ulong)value);
        }

        public override SeedType SeedValueType
        {
            get => SeedType.UInt64 | SeedType.Int64;
        }

        private void f_setSeed(ulong seed)
        {
            p_state0 = f_splitMix64(ref seed);
            p_state1 = f_splitMix64(ref seed);
        }

        /// <summary>
        /// Xorshift128+核心算法
        /// </summary>
        /// <returns></returns>
        private ulong f_generate()
        {
            ulong s1 = p_state0;
            ulong s0 = p_state1;
            p_state0 = s0;
            s1 ^= s1 << 23;
            p_state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return p_state1 + s0;
        }

        /// <summary>
        /// SplitMix64种子扩展算法
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static ulong f_splitMix64(ref ulong x)
        {
            ulong re = (x += 0x9E3779B97F4A7C15UL);
            re = (re ^ (re >> 30)) * 0xBF58476D1CE4E5B9UL;
            re = (re ^ (re >> 27)) * 0x94D049BB133111EBUL;
            return re ^ (re >> 31);
        }

        /// <summary>
        /// 将新的随机种子设置到随机器
        /// </summary>
        /// <param name="seed">随机种子</param>
        public void SetSeed(long seed)
        {
            f_setSeed((ulong)seed);
        }

        /// <summary>
        /// 直接设置Xorshift128+算法的随机种子值
        /// </summary>
        /// <param name="state0">参数0</param>
        /// <param name="state1">参数1</param>
        public void SetSeedValue(ulong state0, ulong state1)
        {
            p_state0 = state0;
            p_state1 = state1;
        }

        /// <summary>
        /// 获取Xorshift128+算法的随机种子值
        /// </summary>
        /// <param name="state0">参数0</param>
        /// <param name="state1">参数1</param>
        public void GetSeedValue(out ulong state0, out ulong state1)
        {
            state0 = p_state0;
            state1 = p_state1;
        }

        public override int Next()
        {
            //const uint maxs = 0b01111111_11111111_11111111_11111111U;
            var re = (f_generate() >> 33);
            if (re == int.MaxValue) return 0;
            return (int)re;
            //return (int)((f_generate() >> 33) % int.MaxValue);

        }

        public override long NextLong()
        {
            //const ulong maxs = 
                //0b01111111_11111111_11111111_11111111__11111111_11111111_11111111_11111111UL;

            var re = f_generate() >> 1;
            if (re == long.MaxValue) return 0;
            return (long)re;
            //return (long)((f_generate() >> 1) % long.MaxValue);

        }

        public override int Next(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)((ulong)min + ((f_generate()) % (ulong)((long)max - min)));
        }

        public override float NextFloat()
        {
            return (f_generate() % 8388607) / 8388607F;
        }

        public override double NextDouble()
        {
            return (f_generate() >> 33) / 2147483648D;
        }

        #endregion

    }

}
