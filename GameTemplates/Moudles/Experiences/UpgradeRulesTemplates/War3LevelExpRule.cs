using System;

namespace Cheng.Experiences
{

    /// <summary>
    /// 《魔兽争霸3：冰封王座》英雄等级经验规则
    /// </summary>
    public sealed class War3LevelExpRule : UpgradeRules
    {

        #region
        public War3LevelExpRule()
        {
        }
        #endregion

        #region

        public override long MinLevel => 1;

        public override long MaxLevel => int.MaxValue;

        public override double LevelToExperience(long level)
        {
            ThrowLevelOutOfRange(level);
            return 200 + (100 * (level - 1));
        }

        #endregion

    }

}
