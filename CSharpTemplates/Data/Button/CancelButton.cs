using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 封装一个按钮使其可随时取消按钮功能
    /// </summary>
    /// <remarks></remarks>
    public sealed class CancelButton : BaseButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个可取消按钮
        /// </summary>
        /// <param name="button">封装的按钮</param>
        /// <exception cref="ArgumentNullException">按钮是null</exception>
        public CancelButton(BaseButton button)
        {
            if (button is null) throw new ArgumentNullException();

            p_button = button;
            p_cancel = false;
        }

        /// <summary>
        /// 实例化一个可取消按钮
        /// </summary>
        /// <param name="button">封装的按钮</param>
        /// <param name="cancel">是否取消按钮</param>
        /// <exception cref="ArgumentNullException">按钮是null</exception>
        public CancelButton(BaseButton button, bool cancel)
        {
            if (button is null) throw new ArgumentNullException();
            
            p_button = button;
            p_cancel = cancel;
        }

        #endregion

        #region 参数

        private BaseButton p_button;
        private bool p_cancel;

        #endregion

        #region 功能

        #region 派生

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

        public override bool CanGetInternalButton => true;

        public override bool CanSetInternalButton => true;

        public override bool IsThreadSafe => p_button.IsThreadSafe;

        public override BaseButton InternalButton
        {
            get => p_button;
            set => p_button = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 访问按钮状态
        /// </summary>
        /// <returns>如果参数<see cref="Cancelled"/>为true则返回false，否则访问封装按钮的状态</returns>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonState 
        {
            get
            {
                return ((!p_cancel) && p_button.ButtonState);
            }
            set
            {
                p_button.ButtonState = value;
            }
        }

        /// <summary>
        /// 访问按钮力度
        /// </summary>
        /// <returns>如果参数<see cref="Cancelled"/>为true则返回封装按钮的最小力度或0，否则访问封装按钮的力度</returns>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override float Power 
        { 
            get
            {
                return p_cancel ? (p_button.CanGetMinPower ? p_button.MinPower : 0) : p_button.Power;
            }
            set
            {
                p_button.Power = value;
            }
        }

        public override bool CanDoubleValueIsPower => p_button.CanDoubleValueIsPower;

        public override double PowerDouble 
        { 
            get
            {
                return p_cancel ? (p_button.CanGetMinPower ? p_button.MinPowerDouble : 0) : p_button.PowerDouble;
            }
            set => p_button.PowerDouble = value;
        }


        /// <summary>
        /// 访问按钮当前帧是否按下
        /// </summary>
        /// <returns>如果参数<see cref="Cancelled"/>为true则返回false，否则访问封装按钮的参数</returns>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonDown
        {
            get
            {
                return ((!p_cancel) && p_button.ButtonDown);
            }
        }

        /// <summary>
        /// 访问按钮当前帧是否按下
        /// </summary>
        /// <returns>如果参数<see cref="Cancelled"/>为true则返回false，否则访问封装按钮的参数</returns>
        /// <exception cref="NotSupportedException">无法设置值</exception>
        public override bool ButtonUp
        {
            get
            {
                return ((!p_cancel) && p_button.ButtonUp);
            }
        }

        public override long NowFrame => p_button.NowFrame;

        public override float MaxPower { get => p_button.MaxPower; set => p_button.MaxPower = value; }

        public override float MinPower { get => p_button.MinPower; set => p_button.MinPower = value; }

        public override double MaxPowerDouble { get => p_button.MaxPowerDouble; set => p_button.MaxPowerDouble = value; }

        public override double MinPowerDouble { get => p_button.MinPowerDouble; set => p_button.MinPowerDouble = value; }

        #endregion

        #region 参数

        /// <summary>
        /// 访问或设置当前按钮是否处于被取消状态
        /// </summary>
        /// <value>
        /// true设为取消状态，false表示非取消状态
        /// <para>当设为true时，按钮状态访问总是返回false，力度访问总是最小值或0；设为false时则正常访问内部封装按钮的参数</para>
        /// </value>
        public bool Cancelled
        {
            get => p_cancel;
            set
            {
                 p_cancel = value;
            }
        }

        #endregion

        #endregion

    }

}
