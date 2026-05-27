#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;
using Cheng.Algorithm;
using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;

using GObj = UnityEngine.GameObject;
using UObj = UnityEngine.Object;
using XIn = Cheng.Unitys.Windows.XInput.Win32API.WindowsXInput;

namespace Cheng.ButtonTemplates.UnityButtons.Windows
{

    /// <summary>
    /// XInput控制器扳机键类型
    /// </summary>
    public enum XInputTriggerType
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 左扳机
        /// </summary>
        LT = 1,

        /// <summary>
        /// 右扳机
        /// </summary>
        RT = 2
    }

#if UNITY_EDITOR
    /// <summary>
    /// XInput扳机键
    /// </summary>
    /// <remarks>
    /// <para>允许作为脚本参数在 Inspector 中设置</para>
    /// </remarks>
#else
    /// <summary>
    /// XInput扳机键
    /// </summary>
#endif
    [Serializable]
    public sealed class XInputTriggerUnityButton : XInputUnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化扳机按钮
        /// </summary>
        public XInputTriggerUnityButton()
        {
            p_t = XInputTriggerType.None; p_pi = 0;
        }

        /// <summary>
        /// 实例化扳机按钮
        /// </summary>
        /// <param name="type">扳机类型</param>
        public XInputTriggerUnityButton(XInputTriggerType type)
        {
            p_t = type; p_pi = 0;
        }

        /// <summary>
        /// 实例化扳机按钮
        /// </summary>
        /// <param name="type">扳机类型</param>
        /// <param name="index">按钮要绑定到的控制器索引</param>
        public XInputTriggerUnityButton(XInputTriggerType type, int index)
        {
            p_t = type; p_pi = index;
        }

        #endregion

        #region 参数

        [SerializeField] private int p_pi;
        [SerializeField] private XInputTriggerType p_t;

#if UNITY_EDITOR

        internal const string cp_fieldName_index = nameof(p_pi);

        internal const string cp_fieldName_button = nameof(p_t);

#endif

        #endregion

        #region 参数访问

        /// <summary>
        /// 扳机力度值
        /// </summary>
        /// <value>范围[0,255]</value>
        public byte Value
        {
            get
            {
                if (p_t == XInputTriggerType.None) return 0;
                var g = WindowsXInputReader.GlobalXInputReader;
                if (g is null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"未启用 {nameof(WindowsXInputReader)} 脚本");
#endif
                    if (XIn.TryGetState(p_pi, out var s) == 0)
                    {
                        switch (p_t)
                        {
                            case XInputTriggerType.LT:
                                return s.Gamepad.leftTrigger;
                            case XInputTriggerType.RT:
                                return s.Gamepad.rightTrigger;
                        }
                    }
                }
                else
                {
                    switch (p_t)
                    {
                        case XInputTriggerType.LT:
                            return g.GetXInputData(p_pi).gamePad.leftTrigger;
                        case XInputTriggerType.RT:
                            return g.GetXInputData(p_pi).gamePad.rightTrigger;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 扳机类型
        /// </summary>
        public XInputTriggerType XInputType
        {
            get => p_t;
            set
            {
                p_t = value;
            }
        }

        #endregion

        #region 派生

        public override int PadIndex
        {
            get => p_pi;
            set
            {
                p_pi = value.Clamp(0, 3);
            }
        }

        public override XInputButtonType ButtonType => XInputButtonType.TriggerButton;

        public override bool CanGetPower => true;

        public override float Power
        {
            get
            {
                return Value / 255f;
            }
        }

        public override bool CanGetMaxPower => true;

        public override bool CanGetMinPower => true;

        public override float MaxPower => 1;

        public override float MinPower => 0;

        public override bool CanGetState => true;

        public override bool ButtonState
        {
            get => Value > sbyte.MaxValue;
        }

        public override double PowerDouble
        {
            get => Value / 255D;
        }

        #endregion

    }


}
#endif