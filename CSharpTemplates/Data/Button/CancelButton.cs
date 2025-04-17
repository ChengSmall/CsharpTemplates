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

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                const ButtonAvailablePermissions or =
                    ButtonAvailablePermissions.CanGetInternalButton |
                    ButtonAvailablePermissions.CanSetInternalButton;

                const ButtonAvailablePermissions and =
                    ~(ButtonAvailablePermissions.CanButtonDownEvent |
                    ButtonAvailablePermissions.CanButtonUpEvent |
                    ButtonAvailablePermissions.CanButtonClick);

                return (p_button.AvailablePermissions | or) & and;
            }
        }

        public override BaseButton InternalButton
        {
            get => p_button;
            set => p_button = value;
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

        public override float MaxPower => p_button.MaxPower;

        public override float MinPower => p_button.MinPower;

        public override long NowFrame => p_button.NowFrame;

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
