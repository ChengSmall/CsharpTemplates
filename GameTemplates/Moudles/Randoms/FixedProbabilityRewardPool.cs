using System;


namespace Cheng.Algorithm.Randoms.RewardPools
{

    /// <summary>
    /// 一个固定概率的奖池机制
    /// </summary>
    public sealed class FixedProbabilityRewardPool : RewardPoolParameter
    {

        #region 构造

        /// <summary>
        /// 实例化一个固定奖池，指定默认参数
        /// </summary>
        public FixedProbabilityRewardPool()
        {
            p_pro = 0;
        }

        /// <summary>
        /// 实例化一个固定奖池
        /// </summary>
        /// <param name="fixedProbability">指定抽奖概率</param>
        public FixedProbabilityRewardPool(double fixedProbability)
        {
            p_pro = fixedProbability;
        }

        #endregion

        #region 参数

        private double p_pro;

        #endregion

        #region 派生

        public override bool CanSetProbability => true;

        public override bool CanGetProbability => true;

        public override double Probability 
        {
            get
            {
                return p_pro;
            }
            set
            {
                p_pro = value;
            }
        }

        protected internal override bool Pull(BaseRandom random)
        {
            return random.NextDouble() < p_pro;
        }

        #endregion

    }

}
