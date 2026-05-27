using System;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 将<see cref="System.Random"/>类封装到随机数生成器
    /// </summary>
    public sealed class SystemRandom : BaseRandom
    {
        /// <summary>
        /// 实例化一个官方API的随机数生成器
        /// </summary>
        public SystemRandom()
        {
            p_random = new global::System.Random();
        }
        /// <summary>
        /// 实例化一个官方API的随机数生成器
        /// </summary>
        /// <param name="seed">种子</param>
        public SystemRandom(int seed)
        {
            p_random = new global::System.Random(seed);
        }
        /// <summary>
        /// 实例化一个官方API的随机数生成器实例
        /// </summary>
        /// <param name="random">要初始化的实例</param>
        /// <exception cref="ArgumentNullException">实例为null</exception>
        public SystemRandom(global::System.Random random)
        {
            this.p_random = random ?? throw new global::System.ArgumentNullException();
        }

        private global::System.Random p_random;

        /// <summary>
        /// 获取内部封装对象
        /// </summary>
        public global::System.Random Random => p_random;
        
        public override void NextBytes(byte[] buffer)
        {
            p_random.NextBytes(buffer);
        }
        public override int Next()
        {
            return p_random.Next();
        }
        public override int Next(int min, int max)
        {
            return p_random.Next(min, max);
        }
        public override double NextDouble()
        {
            return p_random.NextDouble();
        }
        public override float NextFloat()
        {
            return (float)p_random.NextDouble();
        }

    }

}
