using Cheng.DataStructure.Receptacles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cheng.Memorys;

namespace Cheng.Experiences
{

    /// <summary>
    /// 游戏经验等级模板事件委托
    /// </summary>
    public delegate void LevelTemplateAction();

    /// <summary>
    /// 游戏经验等级模板事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args">事件参数</param>
    public delegate void LevelTemplateAction<T>(T args);

    /// <summary>
    /// 游戏经验等级模板
    /// </summary>
    public sealed class LevelTemplate : SafreleaseUnmanagedResources
    {

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                pe_levelUp = null;
                pe_maxLevelGainExp = null;
            }
            return false;
        }

        /// <summary>
        /// 注销事件系统
        /// </summary>
        public sealed override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 初始化等级模板
        /// </summary>
        /// <param name="rules">升级规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public LevelTemplate(IUpgradeRules rules)
        {
            if (rules is null) throw new ArgumentNullException();

            p_rules = rules;
            p_level = rules.MinLevel;
            p_experience = new ReceptacleDouble(0, rules.LevelToExperience(p_level));
        }

        #endregion

        #region 参数

        private ReceptacleDouble p_experience;

        private long p_level;

        private IUpgradeRules p_rules;

        private LevelTemplateAction<long> pe_levelUp;
        private LevelTemplateAction<double> pe_maxLevelGainExp;
        private LevelTemplateAction<long> pe_lvlUpToOnceEnd;
        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置经验条
        /// </summary>
        /// <value>设置该属性时仅设置值，不会触发任何所关联算法和事件</value>
        public ReceptacleDouble Experience
        {
            get => p_experience;
            set => p_experience = value;
        }

        /// <summary>
        /// 访问或设置等级
        /// </summary>
        /// <value>设置该属性时仅设置值，不会触发任何所关联算法和事件</value>
        public long LLevel
        {
            get => p_level;
            set => p_level = value;
        }

        /// <summary>
        /// 访问或设置等级
        /// </summary>
        /// <value>设置该属性时仅设置值，不会触发任何所关联算法和事件</value>
        public int Level
        {
            get => (int)p_level;
            set => p_level = value;
        }

        /// <summary>
        /// 每升级时引发的事件
        /// </summary>
        /// <remarks>
        /// 参数为升级后的等级；
        /// </remarks>
        public event LevelTemplateAction<long> LevelUpEvent
        {
            add
            {
                if (IsDispose) throw new ObjectDisposedException("", "事件系统已注销");
                lock (this) pe_levelUp += value;
            }
            remove
            {
                if (IsDispose) throw new ObjectDisposedException("", "事件系统已注销");
                lock (this) pe_levelUp -= value;
            }
        }

        /// <summary>
        /// 无法升级后再次获取经验时引发的事件
        /// </summary>
        /// <remarks>
        /// <para>调用一次<see cref="GainExp(double)"/>函数后，如果到达最大等级仍留有经验，则引发该事件，参数表示溢出的经验值</para>
        /// </remarks>
        public event LevelTemplateAction<double> MaxLevelGainExpEvent
        {
            add
            {
                if (IsDispose) throw new ObjectDisposedException("", "事件系统已注销");
                lock (this) pe_maxLevelGainExp += value;
            }
            remove
            {
                if (IsDispose) throw new ObjectDisposedException("", "事件系统已注销");
                lock (this) pe_maxLevelGainExp -= value;
            }
        }

        /// <summary>
        /// 每次执行一次经验获取功能后，如果提升等级，则在提升到最后一级时引发事件
        /// </summary>
        /// <remarks>
        /// <para>当调用<see cref="GainExp(double)"/>函数后提升等级，则在函数执行后，则引发一次该事件，参数为提升后的等级</para>
        /// </remarks>
        public event LevelTemplateAction<long> LevelUpToOnceEndEvent
        {
            add
            {
                ThrowObjectDisposeException();
                lock (this) pe_lvlUpToOnceEnd += value;
            }
            remove
            {
                ThrowObjectDisposeException();
                lock (this) pe_lvlUpToOnceEnd -= value;
            }
        }

        /// <summary>
        /// 获取该等级模板的升级规则
        /// </summary>
        public IUpgradeRules UpgradeRules
        {
            get => p_rules;
        }

        /// <summary>
        /// 当前等级升级的剩余所需经验
        /// </summary>
        public double RemainingExperience
        {
            get
            {
                return p_experience.maxValue - p_experience.value;
            }
        }

        /// <summary>
        /// 当前等级升级的所需经验
        /// </summary>
        public double RequiredExperience
        {
            get => p_experience.maxValue;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 获取指定经验
        /// </summary>
        /// <param name="experience">此次要获取的经验值</param>
        public void GainExp(double experience)
        {

            if (p_level == p_rules.MaxLevel)
            {
                //当前等级是最大等级
                pe_maxLevelGainExp?.Invoke(experience);
                return;
            }

            //剩余当前经验
            double remNowExp;
            //剩余获取经验
            double remGetExp;
            //当前等级
            long nowlvl;
            bool isUp = false;
            ReceptacleDouble nowExp;

            remGetExp = experience;
            nowExp = p_experience;
            nowlvl = p_level;

            Loop:

            //计算剩余升级所需
            remNowExp = nowExp.maxValue - nowExp.value;

            if (remGetExp < remNowExp)
            {
                //获取经验小于剩余升级所需
                //添加exp
                nowExp = nowExp.Add(remGetExp);
                goto Over;
            }

            //获取经验大于或等于剩余升级所需

            //升级
            isUp = true;
            nowlvl += 1;
            pe_levelUp?.Invoke(nowlvl);

            //减少获取经验
            remGetExp -= remNowExp;

            //升级后为最大等级
            if (nowlvl == p_rules.MaxLevel)
            {
                pe_maxLevelGainExp?.Invoke(remGetExp);
                //nowExp = new ReceptacleDouble(p_rules.LevelToExperience(nowlvl - 1));
                goto Over;
            }

            //重新计算下一级升级所需
            nowExp = new ReceptacleDouble(0, p_rules.LevelToExperience(nowlvl));

            goto Loop;

            Over:
            
            p_experience = nowExp;
            p_level = nowlvl;
            if(isUp) pe_lvlUpToOnceEnd?.Invoke(nowlvl);
        }

        /// <summary>
        /// 强制将等级设置为指定级
        /// </summary>
        /// <param name="level">要设置的等级</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出等级约束范围</exception>
        public void SetLevel(long level)
        {

            if (level < p_rules.MinLevel || level > p_rules.MaxLevel) throw new ArgumentOutOfRangeException();

            p_level = level;
            p_experience = new ReceptacleDouble(0, p_rules.LevelToExperience(level));
        }

        /// <summary>
        /// 强制将等级设置为指定级
        /// </summary>
        /// <param name="level">要设置的等级</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出等级约束范围</exception>
        public void SetLevel(int level)
        {
            SetLevel((long)level);
        }

        /// <summary>
        /// 将等级和经验重置为初始值
        /// </summary>
        public void ResetLevel()
        {
            p_level = p_rules.MinLevel;
            p_experience = new ReceptacleDouble(0, p_rules.LevelToExperience(p_level));
        }

        /// <summary>
        /// 以字符串格式返回当前等级
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat("level:", p_level.ToString(), " experience:", p_experience.ToString());
        }

        #endregion

        #endregion

    }

}
