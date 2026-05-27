using System;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 封装一个将力度值映射到按钮状态的按钮
    /// </summary>
    /// <remarks>
    /// 该按钮将封装一个按钮，忽略按钮的状态值，将按钮力度映射到按钮状态
    /// </remarks>
    public sealed class ButtonPowerMapState : BaseButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个力度映射按钮，将中间值设为最小力度值和最大力度值的平均值
        /// </summary>
        /// <param name="button">封装的按钮</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ButtonPowerMapState(BaseButton button)
        {
            if (button is null) throw new ArgumentNullException();

            p_button = button;
            p_mid = (button.MinPower + button.MaxPower) / 2f;
        }

        /// <summary>
        /// 实例化一个力度映射按钮
        /// </summary>
        /// <param name="button">封装的按钮</param>
        /// <param name="mid">指定状态变化阈值，以使力度值大于该值时状态为true，小于或等于该值时为false</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ButtonPowerMapState(BaseButton button, float mid)
        {
            if (button is null) throw new ArgumentNullException();

            p_button = button;
            p_mid = mid;
            p_neg = false;
        }

        /// <summary>
        /// 实例化一个力度映射按钮
        /// </summary>
        /// <param name="button">封装的按钮</param>
        /// <param name="mid">指定状态变化阈值，以使力度值大于该值时状态为true，小于或等于该值时为false</param>
        /// <param name="negate">是否取反；该参数为false时正常映射，该参数为true时，将状态变化做反向映射，小于<paramref name="mid"/>为true，大于<paramref name="mid"/>为false</param>
        public ButtonPowerMapState(BaseButton button, float mid, bool negate)
        {
            if (button is null) throw new ArgumentNullException();

            p_button = button;
            p_mid = mid;
            p_neg = negate;
        }

        #endregion

        #region 参数

        private BaseButton p_button;
        private float p_mid;
        private bool p_neg;

        #endregion

        #region 派生

        public override bool CanGetInternalButton => true;

        public override bool CanSetInternalButton => true;

        public override bool CanGetState => true;

        public override bool CanSetState => false;


        public override bool CanGetPower => p_button.CanGetPower;

        public override bool CanSetPower => p_button.CanSetPower;

        public override bool CanGetMaxPower => p_button.CanGetMaxPower;

        public override bool CanGetMinPower => p_button.CanGetMinPower;

        public override bool CanSetMaxPower => p_button.CanSetMaxPower;

        public override bool CanSetMinPower => p_button.CanSetMinPower;

        public override bool CanButtonDownEvent => p_button.CanButtonDownEvent;

        public override bool CanButtonUpEvent => p_button.CanButtonUpEvent;

        public override bool CanButtonClick => p_button.CanButtonClick;

        public override bool CanGetChangeFrameButtonDown => p_button.CanGetChangeFrameButtonDown;

        public override bool CanGetChangeFrameButtonUp => p_button.CanGetChangeFrameButtonUp;

        public override bool CanSetChangeFrameButtonDown => p_button.CanSetChangeFrameButtonDown;

        public override bool CanSetChangeFrameButtonUp => p_button.CanSetChangeFrameButtonUp;

        public override bool CanGetFrameValue => p_button.CanGetFrameValue;

        public override bool CanDoubleValueIsPower => p_button.CanDoubleValueIsPower;

        public override bool IsThreadSafe => p_button.IsThreadSafe;

        public override BaseButton InternalButton 
        {
            get => p_button; 
            set => p_button = value;
        }

        /// <summary>
        /// 访问按钮状态
        /// </summary>
        /// <returns>
        /// 当按钮力度小于或等于阈值<see cref="Mid"/>时，返回false，否则返回true；
        /// <para>若<see cref="Negate"/>为true，则当按钮力度大于或等于<see cref="Mid"/>时返回false，否则返回true</para>
        /// </returns>
        /// <exception cref="NotSupportedException">无法设置</exception>
        public override bool ButtonState 
        { 
            get
            {
                return p_neg ? (p_button.Power <= p_mid) : (p_button.Power >= p_mid);
            }
            set => ThrowSupportedException(); 
        }

        public override float Power 
        {
            get => p_button.Power;
            set => p_button.Power = value; 
        }

        /// <summary>
        /// 该按钮的状态变化阈值
        /// </summary>
        public float Mid
        {
            get => p_mid;
            set
            {
                p_mid = value;
            }
        }

        /// <summary>
        /// 该按钮的力度值映射是否取负值
        /// </summary>
        public bool Negate
        {
            get => p_neg;
            set
            {
                p_neg = value;
            }
        }

        public override float MaxPower 
        {
            get => p_button.MaxPower; 
            set => p_button.MaxPower = value; 
        }

        public override float MinPower 
        {
            get => p_button.MinPower; 
            set => p_button.MinPower = value; 
        }

        public override bool ButtonDown => p_button.ButtonDown;

        public override bool ButtonUp => p_button.ButtonUp;

        public override long NowFrame => p_button.NowFrame;

        public override double PowerDouble { get => p_button.PowerDouble; set => p_button.PowerDouble = value; }

        public override double MaxPowerDouble { get => p_button.MaxPowerDouble; set => p_button.MaxPowerDouble = value; }

        public override double MinPowerDouble { get => p_button.MinPowerDouble; set => p_button.MinPowerDouble = value; }

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

        public override string ToString()
        {
            return p_button.ToString();
        }
        #endregion

    }

}
