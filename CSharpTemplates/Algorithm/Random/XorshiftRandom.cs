using System;
using System.Collections.Generic;
using System.Text;

using Cheng.Memorys;

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
        /// 生成一个随机的64位无符号整数
        /// </summary>
        /// <returns></returns>
        public ulong Generate()
        {
            ulong s1 = p_state0;
            ulong s0 = p_state1;
            p_state0 = s0;
            s1 ^= s1 << 23;
            p_state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return p_state1 + s0;
        }


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
            return (int)(Generate() % int.MaxValue);
        }

        public override long NextLong()
        {
            return (long)(Generate() % long.MaxValue);
        }

        public override int Next(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)((ulong)min + ((Generate()) % (ulong)((long)max - min)));
        }

        public override float NextFloat()
        {
            return (Generate() % 8388607) / 8388607F;
        }

        public override double NextDouble()
        {
            return (Generate() % int.MaxValue) / 2147483647D;
        }

        public override void NextPtr(CPtr<byte> ptr, int length)
        {
            if (length <= 0) return;
            int len = length / sizeof(uint);
            ulong re;
            uint* buffer = (uint*)ptr.p_ptr;
            int i;
            for (i = 0; i < len; i++)
            {
                re = this.Generate();
                buffer[i] = (uint)((re >> 32) ^ (re & uint.MaxValue));
            }

            var mod = (length % sizeof(uint));
            if (mod != 0)
            {
                byte* mp = (byte*)(buffer + i);
                re = Generate();
                len = (int)((re >> 32) ^ (re & uint.MaxValue));
                byte* reb = (byte*)&len;
                for (i = 0; i < mod; i++)
                {
                    mp[i] = reb[i];
                }
            }
        }

        #endregion

    }

}
