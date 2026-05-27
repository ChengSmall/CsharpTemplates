
using System;
using System.Collections.Generic;

using Cheng.Algorithm.Randoms;
using Cheng.Memorys;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 生成固定值的随机器
    /// </summary>
    public sealed class TestFixedRandom : BaseRandom
    {

        public TestFixedRandom(long seed)
        {
            this.seed = seed;
        }

        /// <summary>
        /// 测试用的固定种子值
        /// </summary>
        public long seed;

        public override bool CanGetSeed => true;

        public override bool CanSetSeed => true;

        public override SeedType SeedValueType => SeedType.Int64 | SeedType.UInt64;

        public override long SeedInt64 
        {
            get => seed; 
            set => seed = value;
        }

        public override ulong SeedUInt64
        {
            get => (ulong)seed;
            set => seed = (long)value;
        }

        public override int Next()
        {
            return (int)Math.Abs(seed % int.MaxValue);
        }

        public override long NextLong()
        {
            return Math.Abs(seed) % long.MaxValue;
        }

    }

}
