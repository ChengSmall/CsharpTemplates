using Cheng.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 将按钮封装为一个拥有力度范围的按钮
    /// </summary>
    public sealed class PowerButtonRange : BaseButton
    {

        #region 构造

        /// <summary>
        /// 实例化拥有力度范围的按钮
        /// </summary>
        /// <param name="button">按钮</param>
        /// <param name="min">最小力度值</param>
        /// <param name="max">最大力度值</param>
        public PowerButtonRange(BaseButton button, float min, float max)
        {
            p_button = button ?? throw new ArgumentNullException();
            if (min > max) throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
            p_min = min;
            p_max = max;
        }

        /// <summary>
        /// 实例化拥有力度范围的按钮，指定力度范围在[0,1]
        /// </summary>
        /// <param name="button">按钮</param>
        public PowerButtonRange(BaseButton button) : this(button, 0f, 1f)
        {
        }
        #endregion

        #region 参数
        private BaseButton p_button;
        private float p_min;
        private float p_max;
        #endregion

        #region 功能

        #region 派生

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                const ButtonAvailablePermissions or =
                    ButtonAvailablePermissions.CanGetInternalButton |
                    ButtonAvailablePermissions.CanSetInternalButton |
                    ButtonAvailablePermissions.CanGetMaxPower |
                    ButtonAvailablePermissions.CanGetMinPower | ButtonAvailablePermissions.CanSetMaxPower |
                    ButtonAvailablePermissions.CanSetMinPower;

                //const ButtonAvailablePermissions and =
                //    ~(ButtonAvailablePermissions.CanButtonDownEvent |
                //    ButtonAvailablePermissions.CanButtonUpEvent |
                //    ButtonAvailablePermissions.CanButtonClick);

                return (p_button.AvailablePermissions | or);
            }
        }

        public override bool ButtonState 
        {
            get => p_button.ButtonState;
            set => p_button.ButtonState = value; 
        }

        /// <summary>
        /// 访问或设置力度值
        /// </summary>
        /// <value>
        /// 值大于<see cref="MaxPower"/>时将其设置为<see cref="MaxPower"/>；小于<see cref="MinPower"/>设置为<see cref="MinPower"/>
        /// </value>
        public override float Power 
        {
            get => Maths.Clamp(p_button.Power, p_min, p_max);
            set => p_button.Power = Maths.Clamp(value, p_min, p_max); 
        }

        public override float MaxPower
        {
            get => p_max;
            set => p_max = value;
        }

        public override bool ButtonDown => p_button.ButtonDown;

        public override bool ButtonUp => p_button.ButtonUp;

        public override long NowFrame => p_button.NowFrame;

        public override BaseButton InternalButton
        {
            get => p_button;
            set => p_button = value;
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
        #endregion

        #region 参数
      
        #endregion

        #endregion

    }

}
