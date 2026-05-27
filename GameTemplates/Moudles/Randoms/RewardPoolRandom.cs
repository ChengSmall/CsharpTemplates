using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Algorithm.Randoms.RewardPools
{

    /// <summary>
    /// 表示一个单独的抽奖奖池
    /// </summary>
    public class RewardPoolRandom
    {

        #region 构造

        /// <summary>
        /// 实例化一个奖池
        /// </summary>
        /// <param name="random">随机数生成器</param>
        /// <param name="pool">奖池参数</param>
        public RewardPoolRandom(BaseRandom random, RewardPoolParameter pool)
        {
            if (pool is null || random is null) throw new ArgumentNullException();
            p_pool = pool;
            p_count = 0;
            p_random = random;
            p_sucount = 0;
        }

        #endregion

        #region 参数

        private RewardPoolParameter p_pool;

        private BaseRandom p_random;

        /// <summary>
        /// 抽取的总次数
        /// </summary>
        protected uint p_count;

        /// <summary>
        /// 抽取成功的次数
        /// </summary>
        protected uint p_sucount;

        #endregion

        #region 功能

        /// <summary>
        /// 获取奖池参数
        /// </summary>
        public RewardPoolParameter PoolParameter
        {
            get
            {
                return p_pool;
            }
        }

        /// <summary>
        /// 获取基础随机器
        /// </summary>
        public BaseRandom Random
        {
            get => p_random;
        }

        /// <summary>
        /// 获取奖池参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>奖池参数，若类型无法匹配则返回null</returns>
        public T GetRewardPool<T>() where T : RewardPoolParameter
        {
            return p_pool as T;
        }

        /// <summary>
        /// 在派生类实现以进行一次抽奖
        /// </summary>
        /// <returns>是否成功抽取</returns>
        public bool Pull()
        {
            bool b = PoolParameter.Pull(p_random);

            p_count++;
            if (b)
            {
                p_sucount++;
            }

            return b;
        }

        /// <summary>
        /// 当前共抽奖次数
        /// </summary>
        public uint Count
        {
            get => p_count;
        }

        /// <summary>
        /// 当前共抽奖成功次数
        /// </summary>
        public uint SuccessCount
        {
            get => p_sucount;
        }

        /// <summary>
        /// 重置所有抽奖计次和动态参数
        /// </summary>
        public void ResetCount()
        {
            p_count = 0;
            p_sucount = 0;
            p_pool.ResetBuffer();
        }

        /// <summary>
        /// 计算当前总概率
        /// </summary>
        /// <returns></returns>
        public double CalProbability()
        {
            return p_sucount / (double)p_count;
        }

        #endregion

    }

}
