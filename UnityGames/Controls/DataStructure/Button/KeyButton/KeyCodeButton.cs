using System;
using UnityEngine;

namespace Cheng.ButtonTemplates.UnityButtons
{

    /// <summary>
    /// 使用Unity的 <see cref="UnityEngine.KeyCode"/> 虚拟键码映射按钮
    /// </summary>
    /// <remarks>
    /// 该按钮封装一个 <see cref="UnityEngine.KeyCode"/> Unity虚拟键码，并使用 <see cref="Input"/> 类实现按钮状态；
    /// <para>
    /// 允许作为脚本参数在 Inspector 中设置；
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class KeyCodeButton : UnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个<see cref="KeyCode"/>按钮
        /// </summary>
        public KeyCodeButton()
        {
            p_buttonKey = default;
        }

        /// <summary>
        /// 实例化一个<see cref="KeyCode"/>按钮
        /// </summary>
        /// <param name="key">指定keyCode</param>
        public KeyCodeButton(KeyCode key)
        {
            p_buttonKey = key;
        }

        #endregion

        #region 参数

#if UNITY_EDITOR
        public const string EditorProperityFieldName = nameof(p_buttonKey);
#endif

        [SerializeField] private KeyCode p_buttonKey;

        #endregion

        #region 参数访问

        /// <summary>
        /// 访问或设置要映射的Unity虚拟键码
        /// </summary>
        public KeyCode Key
        {
            get => p_buttonKey;
            set => p_buttonKey = value;
        }

        public override ButtonAvailablePermissions AvailablePermissions
        {
            get
            {
                return UnityButtonAvailablePromissions |
                    ButtonAvailablePermissions.CanGetState |
                    ButtonAvailablePermissions.CanGetChangeFrameButtonDown |
                    ButtonAvailablePermissions.CanGetChangeFrameButtonUp |
                    ButtonAvailablePermissions.CanGetPower |
                    ButtonAvailablePermissions.CanGetMaxPower |
                    ButtonAvailablePermissions.CanGetMinPower;
            }
        }

        /// <summary>
        /// 访问按钮状态
        /// </summary>
        /// <returns>使用<see cref="Input.GetKey(KeyCode)"/>获取按钮状态</returns>
        /// <value>值不可修改</value>
        /// <exception cref="NotSupportedException">值不可修改</exception>
        public sealed override bool ButtonState
        {
            get => Input.GetKey(p_buttonKey);
            set
            {
                ThrowSupportedException();
            }
        }

        public sealed override bool ButtonDown
        {
            get => Input.GetKeyDown(p_buttonKey);
        }

        public sealed override bool ButtonUp
        {
            get => Input.GetKeyUp(p_buttonKey);
        }

        /// <summary>
        /// 使用按钮状态映射的按钮力度
        /// </summary>
        /// <returns>该按钮状态为true时返回1，false返回0</returns>
        public override float Power
        {
            get
            {
                return Input.GetKey(p_buttonKey) ? 1 : 0;
            }
            set => ThrowSupportedException();
        }

        public override float MinPower => 0;
        public override float MaxPower => 1;

        #endregion

    }

}
