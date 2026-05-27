using System;

namespace Cheng.Algorithm.Randoms.RewardPools
{

    /// <summary>
    /// 一个动态线性提升概率奖池
    /// </summary>
    public sealed class DynamicalProbabilityRweardPool : RewardPoolParameter
    {

        #region 构造

        /// <summary>
        /// 一个动态线性提升概率奖池
        /// </summary>
        /// <param name="upperPro"></param>
        public DynamicalProbabilityRweardPool(double upperPro)
        {
            p_startP = 0;
            p_maxP = 1;
            p_up = upperPro;
        }

        /// <summary>
        /// 实例化一个动态线性提升概率奖池
        /// </summary>
        /// <param name="startPro">抽奖的起始概率</param>
        /// <param name="maxPro">最大抽奖概率</param>
        /// <param name="upperPro">每次抽不中时提升的概率</param>
        public DynamicalProbabilityRweardPool(double startPro, double maxPro, double upperPro)
        {
            p_startP = startPro;
            p_maxP = maxPro;
            p_up = upperPro;
        }

        #endregion

        #region 参数

        private double p_startP;

        private double p_maxP;

        private double p_up;

        private double p_nowP;

        #endregion

        #region 功能

        /// <summary>
        /// 当一次抽取没有成功时则下一次提升的概率
        /// </summary>
        public double UpperProbability
        {
            get => p_up;
            set
            {
                p_up = Maths.Clamp(value, 0, 1);
            }
        }

        #endregion

        #region 派生

        public override bool HaveDynamicalProbability => true;

        public override bool CanGetMinProbability => true;

        public override bool CanGetNowProbability => true;

        public override bool CanGetMaxProbability => true;

        public override bool CanSetMinProbability => true;

        public override bool CanSetMaxProbability => true;

        public override bool CanSetNowProbability => true;

        public override double MinProbability 
        { 
            get => p_startP;
            set
            {
                p_startP = Maths.Clamp(value, 0d, 1d);
                if(p_maxP < p_startP)
                {
                    p_maxP = p_startP;
                }
                p_nowP = Maths.Clamp(p_nowP, p_startP, p_maxP);
            }
        }

        public override double MaxProbability 
        { 
            get => p_maxP; 
            set
            {
                p_maxP = Maths.Clamp(value, 0d, 1d);
                if(p_startP > p_maxP)
                {
                    p_startP = p_maxP;                    
                }
                p_nowP = Maths.Clamp(p_nowP, p_startP, p_maxP);
            }
        }

        public override double NowProbability 
        {
            get => p_nowP;
            set
            {
                p_nowP = Maths.Clamp(value, p_startP, p_maxP);
            }
        }

        protected internal override bool Pull(BaseRandom random)
        {
            var p = random.NextDouble();

            bool re = p < p_nowP;

            if (re)
            {
                p_nowP = p_startP;
            }
            else
            {
                p_nowP += p_up;
                if (p_nowP >= p_maxP)
                {
                    p_nowP = p_maxP;
                }
            }

            return re;
        }

        public override void ResetBuffer()
        {
            p_nowP = p_startP;
        }

        #endregion

    }

}
