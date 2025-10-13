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
    public sealed class MersenneTwisterRandom : BaseRandom
    {

        #region 结构

        /// <summary>
        /// 随机器状态的数据备份
        /// </summary>
        [Serializable]
        public struct RandomState
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
        }

        #endregion

        #region 构造

        /// <summary>
        /// 使用64位整数初始化随机器
        /// </summary>
        /// <param name="seed">初始化时使用的随机值</param>
        public MersenneTwisterRandom(long seed)
        {
            f_init(seed);
        }

        /// <summary>
        /// 实例化随机器，并使用<see cref="Environment.TickCount"/>作为初始化随机值
        /// </summary>
        public MersenneTwisterRandom() : this(Environment.TickCount)
        {
        }

        private void f_init(long seed)
        {
            mt = new ulong[N];
            mt[0] = (ulong)seed;
            //this.mti = 0;
            for (int i = 1; i < N; i++)
            {
                mt[i] = (6364136223846793005 * (mt[i - 1] ^ (mt[i - 1] >> 62)) + (ulong)i);
            }
            f_twist();
        }

        /// <summary>
        /// 使用随机器状态数据实例化随机器
        /// </summary>
        /// <param name="state">随机器的状态副本</param>
        /// <exception cref="ArgumentException">参数错误</exception>
        public MersenneTwisterRandom(RandomState state)
        {
            if(state.matrix is null || state.matrix.Length != N)
            {
                throw new ArgumentException(Cheng.Properties.Resources.Exception_FuncArgError, nameof(state));
            }
            this.mt = (ulong[])state.matrix;
            this.mti = state.index;
        }

        #endregion

        #region 参数

        private const int N = 312;
        private const int M = 156;

        private ulong[] mt;
        private int mti;

        #endregion

        #region 功能

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
        /// 获取一个随机长整型值
        /// </summary>
        /// <returns>一个随机长整型值，范围在[0,9223372036854775807)</returns>
        public override long NextLong()
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

            //确保返回值为非负数
            return (long)(x >> 1);
        }

        /// <summary>
        /// 获取一个随机整数
        /// </summary>
        /// <returns>一个随机整数，范围在[0, 2147483647)</returns>
        public override int Next()
        {
            return (int)(NextLong() % int.MaxValue);
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
            return (int)(min + (NextLong() % (max - min)));
        }

        public override long NextLong(long min, long max)
        {
            if (min >= max) throw new ArgumentOutOfRangeException();
            return min + (NextLong() % (max - min));
        }

        #endregion

    }

}
