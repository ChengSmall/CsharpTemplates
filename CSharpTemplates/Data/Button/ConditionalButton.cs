using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cheng.DataStructure;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 附带条件判断的只读按钮
    /// </summary>
    /// <remarks>
    /// <para>将按钮和一个条件函数封装，在获取各种按钮状态时只有条件为true时才会获取封装的按钮状态，否则返回false</para>
    /// <para>对于力度值，在条件为false时返回最小力度值，如果没有最小力度值则返回0</para>
    /// </remarks>
    public sealed class ConditionalButton : BaseButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个条件判断按钮
        /// </summary>
        /// <param name="button">要封装的按钮</param>
        /// <param name="predicate">条件函数</param>
        public ConditionalButton(BaseButton button, Func<bool> predicate)
        {
            if (button is null || predicate is null) throw new ArgumentNullException();
            p_button = button;
            p_pre = predicate;
        }

        #endregion

        #region 参数

        private BaseButton p_button;

        private Func<bool> p_pre;

        #endregion

        #region 功能

        #region 派生

        public override bool CanGetInternalButton => true;

        public override bool CanSetInternalButton => true;

        public override BaseButton InternalButton 
        {
            get => p_button; 
            set => p_button = value ?? throw new ArgumentNullException(); 
        }

        public override bool ButtonState
        {
            get => p_pre.Invoke() && p_button.ButtonState;
            set => ThrowSupportedException();
        }

        public override float Power 
        {
            get
            {
                if (p_pre.Invoke())
                {
                    return p_button.Power;
                }
                else
                {
                    if (CanGetMinPower)
                    {
                        return p_button.MinPower;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set => ThrowSupportedException();
        }

        public override double PowerDouble 
        {
            get
            {
                if (p_pre.Invoke())
                {
                    return p_button.PowerDouble;
                }
                else
                {
                    if (CanGetMinPower)
                    {
                        return p_button.MinPowerDouble;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set => ThrowSupportedException();
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

        public override bool ButtonDown 
        {
            get => p_pre.Invoke() && p_button.ButtonDown;
            set => ThrowSupportedException();
        }

        public override bool ButtonUp 
        {
            get => p_pre.Invoke() && p_button.ButtonUp;
            set => ThrowSupportedException();
        }

        public override long NowFrame => p_button.NowFrame;

        public override bool CanGetState => p_button.CanGetState;

        public override bool CanSetState => p_button.CanSetState;

        public override bool CanGetPower => p_button.CanGetPower;

        public override bool CanSetPower => p_button.CanSetPower;

        public override bool CanGetMaxPower => p_button.CanGetMaxPower;

        public override bool CanGetMinPower => p_button.CanGetMinPower;

        public override bool CanSetMaxPower => p_button.CanSetMaxPower;

        public override bool CanSetMinPower => p_button.CanSetMinPower;

        public override bool CanButtonDownEvent => false;

        public override bool CanButtonUpEvent => false;

        public override bool CanButtonClick => false;

        public override bool CanGetChangeFrameButtonDown => p_button.CanGetChangeFrameButtonDown;

        public override bool CanGetChangeFrameButtonUp => p_button.CanGetChangeFrameButtonUp;

        public override bool CanSetChangeFrameButtonDown => p_button.CanSetChangeFrameButtonDown;

        public override bool CanSetChangeFrameButtonUp => p_button.CanSetChangeFrameButtonUp;

        public override bool CanGetFrameValue => p_button.CanGetFrameValue;

        public override bool CanDoubleValueIsPower => p_button.CanDoubleValueIsPower;

        public override bool IsThreadSafe => p_button.IsThreadSafe;


        public override double MaxPowerDouble { get => p_button.MaxPowerDouble; set => p_button.MaxPowerDouble = value; }

        public override double MinPowerDouble { get => p_button.MinPowerDouble; set => p_button.MinPowerDouble = value; }

        #endregion

        #region 条件

        /// <summary>
        /// 返回内部的判断条件
        /// </summary>
        /// <returns>条件布尔值</returns>
        public bool Conditional()
        {
            return p_pre.Invoke();
        }

        #endregion

        #endregion

    }

}
