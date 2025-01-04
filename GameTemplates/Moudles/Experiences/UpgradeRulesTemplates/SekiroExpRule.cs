

using System;

namespace Cheng.Experiences
{

    /// <summary>
    /// 《只狼：影逝二度》经验算法
    /// </summary>
    public sealed class SekiroExpRule : UpgradeRules
    {

        /*

        a(x+69)²+b+c(x+69-d)(x+69)²


        0.1(x+69)²+10，x∈[1,25]∩N

        0.1(x+69)²+10+0.02(x-25)(x+69)²，x∈[25,+∞]∩N

         */

        public SekiroExpRule()
        {
        }

        public sealed override long MinLevel => 1;

        public sealed override long MaxLevel => long.MaxValue;

        public sealed override double LevelToExperience(long level)
        {
            if (level < 1) throw new ArgumentOutOfRangeException();

            long t1 = (level + 69);
            if (level <= 25)
            {
                //0.1(x+69)²+10
                
                return (0.1 * (t1 * t1)) + 10;
            }

            //0.1(x+69)²+10+0.02(x-25)(x+69)²

            long t2 = level - 25;

            long tt = t1 * t1;

            return (0.1 * tt) + 10 + (0.02 * t2 * tt);
        }

    }

}
