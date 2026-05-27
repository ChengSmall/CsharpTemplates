using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.Algorithm.Randoms
{
    
    /// <summary>
    /// 在一个随机数生成器上添加一层缓冲层以提高随机数提取效率
    /// </summary>
    public sealed class BufferedRandom : BaseRandom
    {

        #region 构造

        /// <summary>
        /// 实例化缓冲层
        /// </summary>
        /// <param name="random">要封装的基础随机数生成器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BufferedRandom(BaseRandom random) : this(random, 128)
        {
        }

        /// <summary>
        /// 实例化缓冲层
        /// </summary>
        /// <param name="random">要封装的基础随机数生成器</param>
        /// <param name="bufferSize">一次缓存的随机数数量，该值要大于0；默认值为128</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓存数没有大于0</exception>
        public BufferedRandom(BaseRandom random, int bufferSize)
        {
            p_random = random ?? throw new ArgumentNullException();

            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            p_intBuf = new int[bufferSize];
            p_longBuf = new long[bufferSize];
            p_intBufPos = bufferSize;
            p_longBufPos = bufferSize;
        }      

        #endregion

        #region 参数

        private BaseRandom p_random;

        private int[] p_intBuf;

        private long[] p_longBuf;

        private int p_intBufPos;
        private int p_longBufPos;

        #endregion

        #region 派生

        public override bool CanGetSeed => false;

        public override bool CanSetSeed => false;

        public override SeedType SeedValueType => SeedType.None;

        private void f_nextBufInt()
        {
            int length = p_intBuf.Length;

            for (int i = 0; i < length; i++)
            {
                p_intBuf[i] = p_random.Next();
            }

            p_intBufPos = 0;
        }

        private void f_nextBufLong()
        {
            int length = p_longBuf.Length;

            for (int i = 0; i < length; i++)
            {
                p_longBuf[i] = p_random.Next();
            }

            p_longBufPos = 0;
        }

        public override int Next()
        {
            if(p_intBufPos == p_intBuf.Length)
            {
                f_nextBufInt();
            }

            return p_intBuf[p_intBufPos++];
        }

        public override long NextLong()
        {
            if(p_longBufPos == p_longBuf.Length)
            {
                f_nextBufLong();
            }

            return p_longBuf[p_longBufPos++];
        }

        /// <summary>
        /// 将剩余缓存的随机数清空
        /// </summary>
        /// <remarks>该函数强制将剩余缓存数清空，之后调用<see cref="Next"/>则会先从封装的随机器提取新的随机数到缓存</remarks>
        public void FlushNext()
        {
            p_intBufPos = p_intBuf.Length;
            p_longBufPos = p_longBuf.Length;
        }

        /// <summary>
        /// 清空剩余缓存并从封装的随机器中提取随机数到缓存
        /// </summary>
        public void ResetNext()
        {
            f_nextBufInt();
            f_nextBufLong();
        }

        /// <summary>
        /// 获取封装的随机数生成器
        /// </summary>
        public BaseRandom BaseRandom
        {
            get => p_random;
        }

        #endregion

    }

}
