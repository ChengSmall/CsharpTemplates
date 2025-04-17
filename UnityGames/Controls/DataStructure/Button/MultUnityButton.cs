using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// Unity多按钮状态封装
    /// </summary>
    public sealed class MultUnityButton : UnityButton, Cheng.DataStructure.Collections.IReadOnlyList<UnityButton>, System.Collections.Generic.IReadOnlyList<UnityButton>
    {

        #region 构造

        /// <summary>
        /// 使用多个按钮实例化多按钮
        /// </summary>
        /// <param name="buttons">按钮数组</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public MultUnityButton(params UnityButton[] buttons)
        {
            f_init(buttons);
        }

        /// <summary>
        /// 使用2个按钮实例化多按钮
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public MultUnityButton(UnityButton b1, UnityButton b2)
        {
            f_init(new UnityButton[] { b1, b2 });
        }

        /// <summary>
        /// 使用3个按钮实例化多按钮
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="b3"></param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public MultUnityButton(UnityButton b1, UnityButton b2, UnityButton b3)
        {
            f_init(new UnityButton[] { b1, b2, b3 });
        }

        private void f_init(UnityButton[] buttons)
        {
            if (buttons is null) throw new ArgumentNullException();
            if (buttons.Length == 0) throw new ArgumentException();
            
            int r = Array.IndexOf(buttons, null);
            if (r >= 0) throw new ArgumentNullException();

            p_buttons = buttons;
            p_and = false;
        }

        #endregion

        #region 参数
        private UnityButton[] p_buttons;
        private bool p_and;
        #endregion

        #region 功能

        #region 权限重写

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                const ButtonAvailablePermissions or = UnityButtonAvailablePromissions |
                    ButtonAvailablePermissions.CanGetMaxPower | ButtonAvailablePermissions.CanGetMinPower;

                ButtonAvailablePermissions ap = or | ButtonAvailablePermissions.CanGetState |
                    ButtonAvailablePermissions.CanGetChangeFrameButtonDown |
                    ButtonAvailablePermissions.CanGetChangeFrameButtonUp |
                    ButtonAvailablePermissions.CanGetPower;

                int length = p_buttons.Length;
                for (int i = 0; i < length; i++)
                {
                    var but = p_buttons[i];
                    var bap = but.AvailablePermissions;

                    if ((bap & ButtonAvailablePermissions.CanGetState) != ButtonAvailablePermissions.CanGetState)
                    {
                        ap &= ~(ButtonAvailablePermissions.AllGetStateAndPower);
                    }
                    if ((bap & ButtonAvailablePermissions.CanGetChangeFrameButtonDown) != ButtonAvailablePermissions.CanGetChangeFrameButtonDown)
                    {
                        ap &= ~(ButtonAvailablePermissions.CanGetChangeFrameButtonDown);
                    }
                    if ((bap & ButtonAvailablePermissions.CanGetChangeFrameButtonUp) != ButtonAvailablePermissions.CanGetChangeFrameButtonUp)
                    {
                        ap &= ~(ButtonAvailablePermissions.CanGetChangeFrameButtonUp);
                    }
                }


                return ap;
            }
        }

        #endregion

        #region 参数

        /// <summary>
        /// 访问设置按钮状态的映射方式
        /// </summary>
        /// <value>
        /// 当值为false时，任意一个按钮状态为true则此实例状态为true；值为true时只有全部按钮状态为true此实例为true；此属性默认为false
        /// </value>
        public bool IsAnd
        {
            get => p_and;
            set
            {
                p_and = value;
            }
        }

        public override bool ButtonState
        {
            get
            {
                int length = p_buttons.Length;
                int i;
                if (p_and)
                {
                    for (i = 0; i < length; i++)
                    {
                        if (!(p_buttons[i].ButtonState)) return false;
                    }
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    if (p_buttons[i].ButtonState) return true;
                }
                return false;
            }
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 使用按钮状态映射按钮力度，false表示0，true表示1
        /// </summary>
        public override float Power
        {
            get
            {
                return ButtonState ? 1f : 0f;
            }
            set => ThrowSupportedException();
        }

        public override float MinPower => 0;

        public override float MaxPower => 1;

        public override bool ButtonDown
        {
            get
            {
                int length = p_buttons.Length;
                int i;
                if (p_and)
                {
                    for (i = 0; i < length; i++)
                    {
                        if (!(p_buttons[i].ButtonDown)) return false;
                    }
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    if (p_buttons[i].ButtonDown) return true;
                }
                return false;
            }
        }

        public override bool ButtonUp
        {
            get
            {
                int length = p_buttons.Length;
                int i;
                if (p_and)
                {
                    for (i = 0; i < length; i++)
                    {
                        if (!(p_buttons[i].ButtonUp)) return false;
                    }
                    return true;
                }

                for (i = 0; i < length; i++)
                {
                    if (p_buttons[i].ButtonUp) return true;
                }
                return false;
            }
        }

        #endregion

        #region 集合功能

        /// <summary>
        /// 使用索引访问按钮
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>按钮</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public UnityButton this[int index]
        {
            get
            {
                if (index < 0 || index > p_buttons.Length) throw new ArgumentOutOfRangeException();
                return p_buttons[index];
            }
        }

        /// <summary>
        /// 访问按钮数量
        /// </summary>
        public int Count => p_buttons.Length;

        /// <summary>
        /// 获取一个循环访问按钮的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<UnityButton> GetEnumerator()
        {
            return ((IEnumerable<UnityButton>)p_buttons).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion

    }

}
