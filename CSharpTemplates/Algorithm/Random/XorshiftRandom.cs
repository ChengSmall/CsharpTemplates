using System;
using System.Collections.Generic;
using System.Text;

using Cheng.Memorys;
using Cheng.Algorithm.HashCodes;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 基于Xorshift128+算法的随机数生成器
    /// </summary>
    public sealed unsafe class XorshiftRandom : BaseRandom
    {

        #region 结构

        /// <summary>
        /// 随机器参数
        /// </summary>
        public struct XorshiftData : IEquatable<XorshiftData>, IHashCode64
        {

            /// <summary>
            /// 初始化随机器参数
            /// </summary>
            /// <param name="state0">参数1</param>
            /// <param name="state1">参数2</param>
            public XorshiftData(ulong state0, ulong state1)
            {
                this.state0 = state0; this.state1 = state1;
            }

            /// <summary>
            /// 参数1
            /// </summary>
            public ulong state0;

            /// <summary>
            /// 参数2
            /// </summary>
            public ulong state1;

            public static bool operator ==(XorshiftData left, XorshiftData right)
            {
                return left.state0 == right.state0 && left.state1 == right.state1;
            }

            public static bool operator !=(XorshiftData left, XorshiftData right)
            {
                return left.state0 != right.state0 && left.state1 == right.state1;
            }

            public override int GetHashCode()
            {
                return (state0 ^ state1).GetHashCode();
            }

            public bool Equals(XorshiftData other)
            {
                return state0 == other.state0 && state1 == other.state1;
            }

            public override bool Equals(object obj)
            {
                if (obj is XorshiftData other) return this == other; return false;
            }

            public long GetHashCode64()
            {
                return (long)(state0 ^ state1);
            }

        }

        #endregion

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

        private XorshiftData p_data;

        #endregion

        #region 功能

        #region 算法封装

        /// <summary>
        /// 使用种子初始化一个随机器算法参数
        /// </summary>
        /// <param name="seed">要用到的种子</param>
        /// <returns>随机器算法参数</returns>
        public static XorshiftData InitBySeed(ulong seed)
        {
            XorshiftData data;
            data.state0 = f_splitMix64(ref seed);
            data.state1 = f_splitMix64(ref seed);
            return data;
        }

        /// <summary>
        /// 使用种子初始化一个随机器算法参数
        /// </summary>
        /// <param name="seed">要用到的种子</param>
        /// <returns>随机器算法参数</returns>
        public static XorshiftData InitBySeed(long seed)
        {
            return InitBySeed((ulong)seed);
        }

        /// <summary>
        /// 生成一个随机的64位无符号整数
        /// </summary>
        /// <param name="data">随机算法需要的参数</param>
        /// <returns></returns>
        public static ulong Generate(ref XorshiftData data)
        {
            ulong s1 = data.state0;
            ulong s0 = data.state1;
            data.state0 = s0;
            s1 ^= s1 << 23;
            data.state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return data.state1 + s0;
        }

        /// <summary>
        /// 生成一个32位随机数
        /// </summary>
        /// <param name="data">随机算法需要的参数</param>
        /// <returns>一个随机整数，范围在[0, 2147483647)</returns>
        public static int Next(ref XorshiftData data)
        {
            return (int)(Generate(ref data) % int.MaxValue);
        }

        /// <summary>
        /// 获取一个随机长整型值
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <returns>一个随机长整型值，范围在[0,9223372036854775807)</returns>
        public static long NextLong(ref XorshiftData data)
        {
            return (long)(Generate(ref data) % long.MaxValue);
        }

        /// <summary>
        /// 获取一个范围在[<paramref name="min"/>,<paramref name="max"/>)的随机数
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过或等于此值</param>
        /// <returns>一个指定范围的随机数</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数min大于或等于max</exception>
        public static int Next(ref XorshiftData data, int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)((ulong)min + ((Generate(ref data)) % (ulong)((long)max - min)));
        }

        /// <summary>
        /// 获取一个范围在[<paramref name="min"/>,<paramref name="max"/>)的随机数
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">不超过或等于此值</param>
        /// <returns>一个指定范围的随机数</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数min大于或等于max</exception>
        public static long NextLong(ref XorshiftData data, long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (long)((ulong)min + ((Generate(ref data)) % (ulong)((long)max - min)));
        }

        /// <summary>
        /// 获取一个范围在[0,1)的随机浮点数
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <returns>范围在[0,1)的随机数</returns>
        public static float NextFloat(ref XorshiftData data)
        {
            return (Generate(ref data) % 8388607) / 8388607F;
        }

        /// <summary>
        /// 获取一个范围在[0,1)的随机双浮点数
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <returns>范围在[0,1)的随机数</returns>
        public static double NextDouble(ref XorshiftData data)
        {
            return (Generate(ref data) % int.MaxValue) / 2147483647D;
        }

        /// <summary>
        /// 用随机数填充指定一段内存中的每个字节
        /// </summary>
        /// <param name="data">随机器算法需要的参数</param>
        /// <param name="ptr">要填充的字节首地址</param>
        /// <param name="length">要填充的字节数</param>
        public static void NextBytes(ref XorshiftData data, CPtr<byte> ptr, int length)
        {
            if (length <= 0) return;
            int len = length / sizeof(uint);
            ulong re;
            uint* buffer = (uint*)ptr.p_ptr;
            int i;
            for (i = 0; i < len; i++)
            {
                re = Generate(ref data);
                buffer[i] = (uint)((re >> 32) ^ (re & uint.MaxValue));
            }

            var mod = (length % sizeof(uint));
            if (mod != 0)
            {
                byte* mp = (byte*)(buffer + i);
                re = Generate(ref data);
                len = (int)((re >> 32) ^ (re & uint.MaxValue));
                byte* reb = (byte*)&len;
                for (i = 0; i < mod; i++)
                {
                    mp[i] = reb[i];
                }
            }
        }

        #endregion

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
            p_data.state0 = f_splitMix64(ref seed);
            p_data.state1 = f_splitMix64(ref seed);
        }

        /// <summary>
        /// 随机器内部数据
        /// </summary>
        public XorshiftData Data
        {
            get => p_data;
            set => p_data = value;
        }

        /// <summary>
        /// 生成一个随机的64位无符号整数
        /// </summary>
        /// <returns></returns>
        public ulong Generate()
        {
            ulong s1 = p_data.state0;
            ulong s0 = p_data.state1;
            p_data.state0 = s0;
            s1 ^= s1 << 23;
            p_data.state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return p_data.state1 + s0;
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
            p_data.state0 = state0;
            p_data.state1 = state1;
        }

        /// <summary>
        /// 获取Xorshift128+算法的随机种子值
        /// </summary>
        /// <param name="state0">参数0</param>
        /// <param name="state1">参数1</param>
        public void GetSeedValue(out ulong state0, out ulong state1)
        {
            state0 = p_data.state0;
            state1 = p_data.state1;
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

        public override long NextLong(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (long)((ulong)min + ((Generate()) % (ulong)((long)max - min)));
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
            NextBytes(ref p_data, ptr, length);
        }

        #endregion

    }

}
