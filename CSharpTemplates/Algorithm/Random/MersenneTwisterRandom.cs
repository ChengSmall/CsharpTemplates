using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 基于梅森旋转算法的随机数生成器
    /// </summary>
    public unsafe sealed class MersenneTwisterRandom : BaseRandom
    {

        #region 结构

        /// <summary>
        /// 随机器状态的数据备份
        /// </summary>
        [Serializable]
        public readonly struct RandomState : ICloneable
        {

            /// <summary>
            /// 初始化随机器状态
            /// </summary>
            /// <param name="matrix">存放循环枚举值的随机表</param>
            /// <param name="index">当前索引</param>
            public RandomState(ulong[] matrix, int index)
            {
                this.matrix = matrix;
                this.index = index;
            }

            /// <summary>
            /// 存放循环枚举值的随机表
            /// </summary>
            public readonly ulong[] matrix;

            /// <summary>
            /// 指向表的索引
            /// </summary>
            public readonly int index;

            /// <summary>
            /// 随机表数组的元素数量
            /// </summary>
            public const int MatrixLength = N;

            /// <summary>
            /// 将当前的状态对象克隆到新的值
            /// </summary>
            /// <returns>参数相同的新的状态对象，克隆后的<see cref="RandomState.matrix"/>与原先的数组是不同的实例</returns>
            public RandomState Clone()
            {
                return new RandomState((ulong[])matrix.Clone(), index);
            }

            object ICloneable.Clone()
            {
                return Clone();
            }
        }

        #endregion

        #region 构造

        /// <summary>
        /// 使用64位整数初始化随机器
        /// </summary>
        /// <param name="seed">初始化时使用的随机值</param>
        public MersenneTwisterRandom(long seed)
        {
            mt = new ulong[N];
            f_init(seed);
        }

        /// <summary>
        /// 实例化随机器，并使用<see cref="Environment.TickCount"/>作为初始化随机值
        /// </summary>
        public MersenneTwisterRandom() : this(Environment.TickCount)
        {
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">要初始化的字节数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public MersenneTwisterRandom(byte[] seed)
        {
            if (seed is null) throw new ArgumentNullException();

            fixed (byte* buffer = seed)
            {
                mt = new ulong[N];
                f_init(buffer, seed.Length);
            }
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">要初始化的字节数组</param>
        /// <param name="offset">要从字节数组指定位置开始读取</param>
        /// <param name="size">从字节数组读取的字节大小，有效最大字节大小是2496字节</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public MersenneTwisterRandom(byte[] seed, int offset, int size)
        {
            if (seed is null) throw new ArgumentNullException();
            if(offset < 0 || size < 0 || (offset + size > seed.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* buffer = seed)
            {
                mt = new ulong[N];
                f_init(buffer + offset, size);
            }
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">指向要初始化的字节值的首地址</param>
        /// <param name="size"><paramref name="seed"/>指向的位置可用字节容量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public MersenneTwisterRandom(CPtr<byte> seed, int size)
        {
            if (seed.IsEmpty) throw new ArgumentNullException();
            mt = new ulong[N];
            f_init(seed, size);
        }

        /// <summary>
        /// 使用随机器状态数据实例化随机器
        /// </summary>
        /// <param name="state">随机器的状态对象</param>
        /// <exception cref="ArgumentException">参数错误，例如随机表数组元素数量不是<see cref="RandomState.MatrixLength"/></exception>
        public MersenneTwisterRandom(RandomState state)
        {
            if(state.matrix is null || state.matrix.Length != N)
            {
                ThrowByRandomStateInit(); return;
            }
            this.mt = (ulong[])state.matrix;
            this.mti = Maths.Clamp(state.index, 0, N);

            void ThrowByRandomStateInit()
            {
                throw new ArgumentException(Cheng.Properties.Resources.Exception_FuncArgError, "state");
            }
        }

        /// <summary>
        /// 实例化随机器
        /// </summary>
        /// <remarks>如果<paramref name="init"/>是false，则在使用随机器前需要调用<see cref="InitRandom(long)"/>或其它重载函数进行初始化</remarks>
        /// <param name="init">如果是true，则使用默认时间戳<see cref="Environment.TickCount"/>初始化随机器</param>
        public MersenneTwisterRandom(bool init)
        {
            mt = new ulong[N];
            if (init)
            {
                f_init(Environment.TickCount);
            }
        }

        private unsafe void f_init(byte* seed, int size)
        {
            var lenb8 = (size / 8);
            var mod8 = size % 8;
            int overIndex = 1;
            if (lenb8 == 0 && (mod8 == 0)) goto StartInit;

            int i;
            int sc = Math.Min(lenb8, N);
            overIndex = Math.Max(1, sc);
            //byte* bps = seed;
            ulong* LP = (ulong*)(seed);
            for (i = 0; i < sc; i++)
            {
                mt[i] = LP[i];
            }
            if (lenb8 < N)
            {
                //var mod8 = (size % 8);
                if (mod8 != 0)
                {
                    int mtidex = i;
                    byte* bmp = (byte*)(LP + i);
                    ulong temp = 0;
                    for (i = 0; i < mod8; i++)
                    {
                        *(((byte*)&temp) + i) = bmp[i];
                    }
                    mt[mtidex] = temp;
                }
            }

            StartInit:

            for (i = overIndex; i < N; i++)
            {
                mt[i] ^= (6364136223846793005 * (mt[i - 1] ^ (mt[i - 1] >> 62)) + (ulong)i);
            }
            f_twist();
        }

        private void f_init(long seed)
        {
            mt[0] = (ulong)seed;

            for (int i = 1; i < N; i++)
            {
                mt[i] = (6364136223846793005 * (mt[i - 1] ^ (mt[i - 1] >> 62)) + (ulong)i);
            }
            f_twist();
        }

        #endregion

        #region 参数

        private const int N = 312;
        private const int M = 156;

        private ulong[] mt;
        private int mti;

        #endregion

        #region 功能

        #region 初始化

        /// <summary>
        /// 使用64位整数初始化随机器
        /// </summary>
        /// <param name="seed">要进行初始化的64位整数值</param>
        public void InitRandom(long seed)
        {
            Array.Clear(mt, 0, N);
            f_init(seed);
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">要初始化的字节数组</param>
        /// <param name="offset">要从字节数组指定位置开始读取</param>
        /// <param name="size">从字节数组读取的字节大小，有效最大字节大小是2496字节</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void InitRandom(byte[] seed, int offset, int size)
        {
            if (seed is null) throw new ArgumentNullException();
            if (offset < 0 || size < 0 || (offset + size > seed.Length))
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Clear(mt, 0, N);
            fixed (byte* buffer = seed)
            {
                f_init(buffer + offset, size);
            }
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">要初始化的字节数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void InitRandom(byte[] seed)
        {
            if (seed is null) throw new ArgumentNullException();
            Array.Clear(mt, 0, N);
            fixed (byte* buffer = seed)
            {
                f_init(buffer, seed.Length);
            }
        }

        /// <summary>
        /// 使用随机字节初始化随机器
        /// </summary>
        /// <param name="seed">指向要初始化的字节值的首地址</param>
        /// <param name="size"><paramref name="seed"/>指向的位置可用字节容量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void InitRandom(CPtr<byte> seed, int size)
        {
            if (seed.IsEmpty) throw new ArgumentNullException();
            Array.Clear(mt, 0, N);
            f_init(seed.p_ptr, size);
        }

        #endregion

        public override SeedType SeedValueType => SeedType.None;

        private void f_twist()
        {
            const ulong Upper_Mask = 0xFFFFFFFF80000000;
            const ulong Lower_Mask = 0x7FFFFFFF;
            const ulong Matrix_A = 0xB5026F5AA96619E9;

            for (int i = 0; i < N; i++)
            {
                ulong x = (mt[i] & Upper_Mask) | (mt[(i + 1) % N] & Lower_Mask);
                ulong xA = x >> 1;

                if ((x & 1) == 1)
                {
                    xA ^= Matrix_A;
                }

                mt[i] = mt[(i + M) % N] ^ xA;
            }
            mti = 0;
        }

        /// <summary>
        /// 生成一个随机值
        /// </summary>
        /// <returns>随机值</returns>
        public ulong Generate()
        {
            if (mti >= N)
            {
                f_twist();
            }

            ulong x = mt[mti++];
            x ^= (x >> 29) & 0x5555555555555555;
            x ^= (x << 17) & 0x71D67FFFEDA60000;
            x ^= (x << 37) & 0xFFF7EEE000000000;
            x ^= (x >> 43);
            return x;
        }

        /// <summary>
        /// 获取一个随机长整型值
        /// </summary>
        /// <returns>一个随机长整型值，范围在[0,9223372036854775807)</returns>
        public override long NextLong()
        {
            return (long)(Generate() % long.MaxValue);
        }

        /// <summary>
        /// 获取一个随机整数
        /// </summary>
        /// <returns>一个随机整数，范围在[0, 2147483647)</returns>
        public override int Next()
        {
            return (int)(Generate() % int.MaxValue);
        }

        /// <summary>
        /// 获取随机器当前状态
        /// </summary>
        /// <returns>表示随机器当前状态的副本数据</returns>
        public RandomState GetNowState()
        {
            return new RandomState((ulong[])this.mt.Clone(), this.mti);
        }

        public override int Next(int min, int max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return (int)(min + (long)(Generate() % ((ulong)(max - min))));
        }

        public override long NextLong(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (NextLong() % (max - min));
        }

        public override void NextPtr(IntPtr ptr, int length)
        {
            if (length == 0) return;
            int len = length / sizeof(uint);
            ulong re;
            uint* buffer = (uint*)ptr;
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

        public override float NextFloat()
        {
            return ((float)(Generate() % 8388607)) / 8388607F;
        }

        public override double NextDouble()
        {
            return ((Generate() % int.MaxValue)) / 2147483647d;
        }

        #endregion

    }

}
