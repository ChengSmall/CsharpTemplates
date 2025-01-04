using System;

namespace Cheng.Experiences
{

    /// <summary>
    /// Java版《Minecraft》等级升级公式
    /// </summary>
    public sealed class MinecraftLevelRule : UpgradeRules
    {

        #region 构造

        public MinecraftLevelRule()
        {
        }

        #endregion

        #region 派生

        public override long MinLevel => 0;

        public override long MaxLevel => int.MaxValue;

        public override double LevelToExperience(long level)
        {
            ThrowLevelOutOfRange(level);

            if(level <= 15)
            {
                return (2 * level) + 7;
            }
            else if (level <= 30)
            {
                return (5 * level) - 38;
            }

            return (9 * level) - 158;
        }

        #endregion

    }

}
