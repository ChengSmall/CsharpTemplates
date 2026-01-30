using System;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 对一个随机数生成器进行线程安全同步封装
    /// </summary>
    public sealed class SynchronizedRandom : BaseRandom
    {

        #region 构造

        /// <summary>
        /// 在指定随机数生成器周围建立线程安全（同步）封装
        /// </summary>
        /// <param name="random">要封装的随机器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public SynchronizedRandom(BaseRandom random)
        {
            this.random = random ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数

        /// <summary>
        /// 封装的随机数生成器
        /// </summary>
        public readonly BaseRandom random;

        #endregion

        #region 派生

        public override bool CanGetSeed
        {
            get
            {
                return random.CanGetSeed;
            }
        }

        public override bool CanSetSeed
        {
            get
            {
                return random.CanSetSeed;
            }
        }

        public override SeedType SeedValueType
        {
            get
            {
                return random.SeedValueType;
            }
        }

        public override int SeedInt32 
        {
            get => random.SeedInt32; 
            set
            {
                lock (random) random.SeedInt32 = value;
            }
        }

        public override long SeedInt64 
        {
            get => random.SeedInt64; 
            set
            {
                lock (random) random.SeedInt64 = value;
            }
        }

        public override int SeedUInt32 
        { 
            get => random.SeedUInt32; 
            set 
            {
                lock (random) random.SeedUInt32 = value;
            }
        }

        public override ulong SeedUInt64 
        { 
            get => random.SeedUInt64; 
            set 
            {
                lock (random) random.SeedUInt64 = value;
            } 
        }

        public override int Next()
        {
            lock(random) return random.Next();
        }

        public override int Next(int min, int max)
        {
            lock (random) return random.Next(min, max);
        }

        public override long NextLong()
        {
            lock (random) return random.NextLong();
        }

        public override long NextLong(long min, long max)
        {
            lock (random) return random.NextLong(min, max);
        }

        public override double NextDouble()
        {
            lock (random) return random.NextDouble();
        }

        public override float NextFloat()
        {
            lock (random) return random.NextFloat();
        }

        public override float NextFloat(float min, float max)
        {
            lock (random) return random.NextFloat(min, max);
        }

        public override double NextDouble(double min, double max)
        {
            lock (random) return random.NextDouble(min, max);
        }

        public override void NextBytes(byte[] buffer)
        {
            lock (random) random.NextBytes(buffer);
        }

        public override void NextBytes(byte[] buffer, int offset, int count)
        {
            lock (random) random.NextBytes(buffer, offset, count);
        }

        public override bool NextDouble(double probability)
        {
            lock (random) return random.NextDouble(probability);
        }

        public override void NextPtr(IntPtr ptr, int length)
        {
            lock (random) random.NextPtr(ptr, length);
        }

        public override bool NextFloat(float probability)
        {
            lock(random) return random.NextFloat(probability);
        }

        #endregion

    }

}