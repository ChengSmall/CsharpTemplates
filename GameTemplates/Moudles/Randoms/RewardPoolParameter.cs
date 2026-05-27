using System;


namespace Cheng.Algorithm.Randoms.RewardPools
{

    /// <summary>
    /// 奖池参数
    /// </summary>
    public abstract class RewardPoolParameter
    {

        #region 参数

        /// <summary>
        /// 该奖池是否允许设置固定抽奖概率
        /// </summary>
        public virtual bool CanSetProbability => false;

        /// <summary>
        /// 该奖池是否允许获取固定抽奖概率
        /// </summary>
        public virtual bool CanGetProbability => false;

        /// <summary>
        /// 设置或访问固定抽奖概率
        /// </summary>
        /// <value>范围在[0,1]的百分比浮点数</value>
        /// <exception cref="NotSupportedException">没有指定权限；如果<see cref="HaveDynamicalProbability"/>为true，则该属性失效</exception>
        public virtual double Probability
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 该奖池是否拥有抽取到指定次数后必须出奖的机制
        /// </summary>
        public virtual bool HaveSuccessfulCount => false;

        /// <summary>
        /// 访问触发必须出奖的抽取次数
        /// </summary>
        /// <exception cref="NotSupportedException">参数<see cref="HaveSuccessfulCount"/>为false</exception>
        public virtual int SuccessfulCount
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 该奖池是否拥有因为抽取次数而改变抽奖概率的机制
        /// </summary>
        public virtual bool HaveDynamicalProbability => false;

        /// <summary>
        /// 该奖池是否能够访问当前抽奖概率
        /// </summary>
        public virtual bool CanGetNowProbability => false;

        /// <summary>
        /// 该奖池是否能够设置当前抽奖概率
        /// </summary>
        public virtual bool CanSetNowProbability => false;

        /// <summary>
        /// 该奖池是否能够访问当前最大抽奖概率
        /// </summary>
        public virtual bool CanGetMaxProbability => false;

        /// <summary>
        /// 该奖池是否能够设置当前最大抽奖概率
        /// </summary>
        public virtual bool CanSetMaxProbability => false;

        /// <summary>
        /// 该奖池是否能够访问当前最小抽奖概率
        /// </summary>
        public virtual bool CanGetMinProbability => false;

        /// <summary>
        /// 该奖池是否能够设置当前最小抽奖概率
        /// </summary>
        public virtual bool CanSetMinProbability => false;

        /// <summary>
        /// 访问该奖池当前的抽奖概率
        /// </summary>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        public virtual double NowProbability
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 访问该奖池最大抽奖概率
        /// </summary>
        public virtual double MaxProbability
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 访问该奖池最小抽奖概率
        /// </summary>
        public virtual double MinProbability
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        #endregion

        #region 功能

        /// <summary>
        /// 在派生类实现以进行一次抽奖
        /// </summary>
        /// <param name="random">基础随机器</param>
        /// <returns>抽取的结果，true表示抽取成功，false表示抽取失败</returns>
        internal protected abstract bool Pull(BaseRandom random);

        /// <summary>
        /// 重置当前奖池内的所有动态参数到初始值
        /// </summary>
        public virtual void ResetBuffer()
        {
        }

        #endregion

    }

}
