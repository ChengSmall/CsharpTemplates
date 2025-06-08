using System;

namespace Cheng.Experiences
{

    /// <summary>
    /// 升级规则和算法
    /// </summary>
    public interface IUpgradeRules
    {

        /// <summary>
        /// 获取指定等级的升级所需经验
        /// </summary>
        /// <remarks>
        /// <para>
        /// 这是一个算法实现函数，参数<paramref name="level"/>表示等级，返回值表示该从<paramref name="level"/>级升级到下一级时所需要的经验值；<br/>
        /// 同一个<paramref name="level"/>参数返回的只有一个定值；
        /// </para>
        /// <para>返回值不得小于或等于0</para>
        /// </remarks>
        /// <param name="level">要判断的等级</param>
        /// <returns>
        /// 等级为<paramref name="level"/>时升级所需经验
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">等级参数超出指定范围</exception>
        double LevelToExperience(long level);

        /// <summary>
        /// 最小等级
        /// </summary>
        /// <returns>从此值开始为起始等级</returns>
        long MinLevel { get; }

        /// <summary>
        /// 最大等级
        /// </summary>
        /// <returns>当升级到此值时将停止升级</returns>
        long MaxLevel { get; }

    }

    /// <summary>
    /// 升级规则实现基类
    /// </summary>
    public abstract class UpgradeRules : IUpgradeRules
    {

        /// <summary>
        /// 最小等级
        /// </summary>
        /// <returns>从此值开始为起始等级</returns>
        public virtual long MinLevel
        {
            get => 0;
        }

        /// <summary>
        /// 最大等级
        /// </summary>
        /// <returns>当升级到此值时将停止升级</returns>
        public abstract long MaxLevel { get; }

        /// <summary>
        /// 获取指定等级的升级所需经验
        /// </summary>
        /// <remarks>
        /// <para>
        /// 这是一个算法实现函数，参数<paramref name="level"/>表示等级，返回值表示该从<paramref name="level"/>级升级到下一级时所需要的经验值；<br/>
        /// 同一个<paramref name="level"/>参数返回的只有一个定值；
        /// </para>
        /// <para>返回值不得小于或等于0</para>
        /// </remarks>
        /// <param name="level">要判断的等级</param>
        /// <returns>
        /// 等级为<paramref name="level"/>时升级所需经验
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">等级参数超出指定范围</exception>
        public abstract double LevelToExperience(long level);

        /// <summary>
        /// 计算从指定等级到达指定等级的经验所需总量
        /// </summary>
        /// <param name="beginLevel">起始等级</param>
        /// <param name="toLevel">到达等级</param>
        /// <returns>从<paramref name="beginLevel"/>到<paramref name="toLevel"/>的所需经验总量</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数超出最小或最大等级范围</exception>
        public virtual double UpLevelTotal(long beginLevel, long toLevel)
        {
            double exp = 0;
            for (long i = beginLevel; i <= toLevel; i++)
            {
                exp += LevelToExperience(i);
            }

            return exp;
        }

        /// <summary>
        /// 当给定等级超出范围时引发异常<see cref="ArgumentOutOfRangeException"/>
        /// </summary>
        /// <param name="level"></param>
        protected void ThrowLevelOutOfRange(long level)
        {
            if(level < MinLevel || level > MaxLevel) throw new ArgumentOutOfRangeException("level", "等级超出指定范围");
        }

    }

}
