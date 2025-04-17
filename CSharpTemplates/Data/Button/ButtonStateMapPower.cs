using System;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 封装一个将按钮状态映射到力度的按钮
    /// </summary>
    /// <remarks>
    /// 该按钮将封装一个按钮，忽略按钮的力度值，将按钮状态映射到按钮力度
    /// </remarks>
    public sealed class ButtonStateMapPower : BaseButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个仅显示状态的按钮
        /// </summary>
        /// <param name="button">要封装的按钮</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ButtonStateMapPower(BaseButton button)
        {
            p_button = button ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数
        private BaseButton p_button;
        #endregion

        #region 功能

        /// <summary>
        /// 访问封装的内部按钮
        /// </summary>
        public BaseButton BaseButton => p_button;

        #region 派生

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                const ButtonAvailablePermissions or =
                    ButtonAvailablePermissions.CanGetInternalButton |
                    ButtonAvailablePermissions.CanSetInternalButton;

                const ButtonAvailablePermissions and = 
                    ~ButtonAvailablePermissions.CanSetPower;

                return (p_button.AvailablePermissions | or) & and;
            }
        }

        public override BaseButton InternalButton
        {
            get => p_button;
            set => p_button = value;
        }

        public override bool ButtonState
        { 
            get => p_button.ButtonState; 
            set => p_button.ButtonState = value; 
        }

        /// <summary>
        /// 使用按钮状态映射按钮力度
        /// </summary>
        /// <returns>
        /// <para>按钮状态为true时，力度返回<see cref="MaxPower"/>，状态为false时返回<see cref="MinPower"/></para>
        /// <para>如果没有最大或最小力度的访问权限，则使用1和0代替</para>
        /// </returns>
        /// <exception cref="NotSupportedException">无法设置按钮力度</exception>
        public override float Power 
        {
            get
            {
                var ap = p_button.AvailablePermissions;

                if(((ap & ButtonAvailablePermissions.CanGetMaxPower) != ButtonAvailablePermissions.CanGetMaxPower) || ((ap & ButtonAvailablePermissions.CanGetMinPower) != ButtonAvailablePermissions.CanGetMinPower))
                {
                    return p_button.ButtonState ? 1 : 0;
                }

                return p_button.ButtonState ? p_button.MaxPower : p_button.MinPower;
            }
            set
            {
                ThrowSupportedException();
            }
        }

        public override float MinPower => p_button.MinPower;

        public override float MaxPower => p_button.MaxPower;

        public override bool ButtonUp => p_button.ButtonUp;

        public override bool ButtonDown => p_button.ButtonDown;

        public override long NowFrame => p_button.NowFrame;

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

        #endregion

        #endregion

    }

}
