using System;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 表示一个谓词筛选器或条件判断委托
    /// </summary>
    /// <returns>判断是否成立</returns>
    public delegate bool ConditionFunc();
    /// <summary>
    /// 表示一个谓词筛选器或条件判断委托
    /// </summary>
    /// <typeparam name="T">要筛选或判断的参数类型</typeparam>
    /// <param name="obj">要筛选或判断的参数</param>
    /// <returns>判断是否成立</returns>
    public delegate bool ConditionFunc<T>(T obj);

    /// <summary>
    /// 表示一个带有条件的Unity按钮
    /// </summary>
    /// <remarks>
    /// 该按钮封装一个按钮，使用条件委托判断按钮是否起用
    /// </remarks>
    public sealed class ConditionButton : UnityButton
    {

        #region 初始化参数

        /// <summary>
        /// 实例化一个条件按钮
        /// </summary>
        /// <param name="button">要封装的Unity按钮</param>
        /// <exception cref="ArgumentNullException">按钮参数为null</exception>
        public ConditionButton(UnityButton button)
        {
            p_button = button ?? throw new ArgumentNullException();
            p_cond = null;
            p_nullPower = 0;
            p_nullRet = true;
        }

        /// <summary>
        /// 实例化一个条件按钮
        /// </summary>
        /// <param name="button">要封装的Unity按钮</param>
        /// <param name="cond">要加入的条件</param>
        /// <exception cref="ArgumentNullException">按钮参数为null</exception>
        public ConditionButton(UnityButton button, ConditionFunc cond)
        {
            p_button = button ?? throw new ArgumentNullException();
            p_cond = cond;
            p_nullRet = true;
            p_nullPower = 0;
        }

        private UnityButton p_button;
        private ConditionFunc p_cond;
        private float p_nullPower;
        private bool p_nullRet;

        #endregion

        #region 派生

        #region
        public override bool CanGetState => p_button.CanGetState;
        public override bool CanSetState => p_button.CanSetState;

        public override bool CanGetChangeFrameButtonDown => p_button.CanGetChangeFrameButtonDown;

        public override bool CanGetChangeFrameButtonUp => p_button.CanGetChangeFrameButtonUp;

        public override bool CanGetPower => p_button.CanGetPower;
        public override bool CanSetPower => p_button.CanSetPower;
        public override bool CanButtonDownEvent => p_button.CanButtonDownEvent;
        public override bool CanButtonUpEvent => p_button.CanButtonUpEvent;

        public override event ButtonEvent<BaseButton> ButtonDownEvent
        {
            add
            {
                p_button.ButtonDownEvent += value;
            }
            remove
            {
                p_button.ButtonDownEvent -= value;
            }
        }
        public override event ButtonEvent<BaseButton> ButtonUpEvent
        {
            add
            {
                p_button.ButtonUpEvent += value;
            }
            remove
            {
                p_button.ButtonUpEvent -= value;
            }
        }

        public override bool CanButtonClick => p_button.CanButtonClick;

        public override event ButtonEvent<BaseButton> ButtonClickEvent
        {
            add
            {
                p_button.ButtonClickEvent += value;
            }
            remove
            {
                p_button.ButtonClickEvent -= value;
            }
        }

        public override bool CanGetFrameValue => p_button.CanGetFrameValue;
        public override long NowFrame => p_button.NowFrame;
        #endregion

        #region 条件
        /// <summary>
        /// 访问或设置条件委托
        /// </summary>
        public ConditionFunc Condition
        {
            get => p_cond;
            set
            {
                p_cond = value;
            }
        }

        /// <summary>
        /// 访问或设置条件委托实例为null时条件返回值，默认为true
        /// </summary>
        public bool NullRet
        {
            get
            {
                return p_nullRet;
            }
            set
            {
                p_nullRet = value;
            }
        }

        /// <summary>
        /// 访问或设置条件委托实例为null时的力度值，默认为0
        /// </summary>
        public float NullPower
        {
            get => p_nullPower;
            set => p_nullPower = value;
        }

        /// <summary>
        /// 获取内部封装的按钮
        /// </summary>
        public UnityButton BaseButton
        {
            get => p_button;
        }

        /// <summary>
        /// 访问或设置按钮状态
        /// </summary>
        /// <returns>true表示按钮被按下，false表示按钮被抬起；当条件为false时返回false</returns>
        /// <exception cref="NotSupportedException">此按钮不允许访问或设置</exception>
        public override bool ButtonState 
        { 
            get => p_button.ButtonState && IsCondition(p_nullRet); 
            set => p_button.ButtonState = value;
        }

        /// <summary>
        /// 是否在当前帧抬起；当条件为false时返回false
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="CanGetChangeFrameButtonUp"/>为false</exception>
        public override bool ButtonUp
        {
            get
            {
                return p_button.ButtonUp && IsCondition(p_nullRet);
            }
        }

        /// <summary>
        /// 是否在当前帧按下；当条件为false时返回false
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="CanGetChangeFrameButtonDown"/>为false</exception>
        public override bool ButtonDown
        {
            get => p_button.ButtonUp && IsCondition(p_nullRet);
        }

        /// <summary>
        /// 访问或设置力度
        /// </summary>
        /// <value>范围为[0,1]的值</value>
        /// <returns>返回力度值，若按钮条件为false，则返回<see cref="NullPower"/></returns>
        /// <exception cref="NotSupportedException">没有访问权限</exception>
        public override float Power
        {
            get
            {
                return IsCondition(p_nullRet) ? p_button.Power : p_nullPower;
            }
            set => p_button.Power = value;
        }

        public override bool CanGetMaxPower => true;

        public override float MaxPower => p_button.MaxPower;

        public override float MinPower => p_button.MinPower;

        /// <summary>
        /// 判断条件结果
        /// </summary>
        /// <param name="nullRet">委托为null时返回的参数</param>
        /// <returns>执行条件委托并返回值，若为null则返回参数</returns>
        public bool IsCondition(bool nullRet)
        {
            return (p_cond is null) ? nullRet : p_cond.Invoke();
        }

        /// <summary>
        /// 判断条件结果
        /// </summary>
        /// <returns>执行条件委托并返回值，若为null则返回<see cref="NullRet"/></returns>
        public bool IsCondition()
        {
            return (p_cond is null) ? p_nullRet : p_cond.Invoke();
        }

        public override bool CanGetInternalButton => true;
        public override bool CanSetInternalButton => true;

        public override BaseButton InternalButton
        {
            get => p_button;
            set => p_button = value as UnityButton;
        }

        #endregion

        #endregion

    }

}
