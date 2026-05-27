#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;
using Cheng.Algorithm;
using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;

using XIn = Cheng.Unitys.Windows.XInput.Win32API.WindowsXInput;

namespace Cheng.ButtonTemplates.UnityButtons.Windows
{

    /*
    None            =>      00000000_00000000 == 0
    DPadUp          =>      00000000_00000001 >> 0
    DPadDown        =>      00000000_00000010 >> 1
    DPadLeft        =>      00000000_00000100 >> 2
    DPadRight       =>      00000000_00001000 >> 3
    Menu            =>      00000000_00010000 >> 4
    Back            =>      00000000_00100000 >> 5
    LeftThumb       =>      00000000_01000000 >> 6
    RightThumb      =>      00000000_10000000 >> 7
    LeftShoulder    =>      00000001_00000000 >> 8
    RightShoulder   =>      00000010_00000000 >> 9

    A               =>      00010000_00000000 >> 12
    B               =>      00100000_00000000 >> 13
    X               =>      01000000_00000000 >> 14
    Y               =>      10000000_00000000 >> 15
    */

    /// <summary>
    /// XInput控制器按键枚举
    /// </summary>
    public enum XInputPadButtonType
    {

        /// <summary>
        /// 无按键
        /// </summary>
        None = 0,

        /// <summary>
        /// 方向键 - 上
        /// </summary>
        Up = 0 + 1,

        /// <summary>
        /// 方向键 - 下
        /// </summary>
        Down = 1 + 1,

        /// <summary>
        /// 方向键 -  左
        /// </summary>
        Left = 2 + 1,

        /// <summary>
        /// 方向键 - 右
        /// </summary>
        Right = 3 + 1,

        /// <summary>
        /// 菜单\开始键
        /// </summary>
        Menu = 4 + 1,

        /// <summary>
        /// 返回键（Back）
        /// </summary>
        Back = 5 + 1,

        /// <summary>
        /// 左摇杆按压
        /// </summary>
        LeftJoystickDown = 6 + 1,

        /// <summary>
        /// 右摇杆按压
        /// </summary>
        RightJoystickDown = 7 + 1,

        /// <summary>
        /// 左肩键
        /// </summary>
        LeftShoulder = 8 + 1,

        /// <summary>
        /// 右肩键
        /// </summary>
        RightShoulder = 9 + 1,

        /// <summary>
        /// 按键 A
        /// </summary>
        A = 12 + 1,

        /// <summary>
        /// 按键 B
        /// </summary>
        B = 13 + 1,

        /// <summary>
        /// 按键 X
        /// </summary>
        X = 14 + 1,

        /// <summary>
        /// 按键 Y
        /// </summary>
        Y = 15 +  1

    }

#if UNITY_EDITOR
    /// <summary>
    /// XInput控制器按键
    /// </summary>
    /// <remarks>
    /// <para>启用<see cref="WindowsXInputReader"/>脚本后可以使用<see cref="BaseButton.ButtonDown"/>和<see cref="BaseButton.ButtonUp"/>参数</para>
    /// <para>允许作为脚本参数在 Inspector 中设置按键类型</para>
    /// </remarks>
#else
    /// <summary>
    /// XInput控制器按键
    /// </summary>
    /// <remarks>
    /// <para>启用<see cref="WindowsXInputReader"/>脚本后可以使用<see cref="BaseButton.ButtonDown"/>和<see cref="BaseButton.ButtonUp"/>参数</para>
    /// </remarks>
#endif
    [Serializable]
    public sealed class XInputPadUnityButton : XInputUnityButton
    {

        #region 构造

        /// <summary>
        /// 实例化控制器按钮
        /// </summary>
        public XInputPadUnityButton()
        {
            p_t = XInputPadButtonType.None; p_pi = 0;
        }

        /// <summary>
        /// 实例化控制器按钮
        /// </summary>
        /// <param name="type">设置按键类型</param>
        public XInputPadUnityButton(XInputPadButtonType type)
        {
            p_t = type; p_pi = 0;
        }

        /// <summary>
        /// 实例化控制器按钮
        /// </summary>
        /// <param name="type">设置按键类型</param>
        /// <param name="index">按钮要绑定到的控制器索引</param>
        public XInputPadUnityButton(XInputPadButtonType type, int index)
        {
            p_t = type; p_pi = index.Clamp(0, 3);
        }

        #endregion

        #region 参数

        [SerializeField] private int p_pi;
        [SerializeField] private XInputPadButtonType p_t;

#if UNITY_EDITOR

        internal const string cp_fieldName_index = nameof(p_pi);

        internal const string cp_fieldName_button = nameof(p_t);

#endif

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

        public override XInputButtonType ButtonType => XInputButtonType.PadButton;

        public override bool CanGetState => true;

        public override bool ButtonState
        {
            get
            {
                if (p_t == XInputPadButtonType.None) return false;
                var offset = (int)p_t - 1;
                var bv = (GamePadButtons)(1U << offset);

                var g = WindowsXInputReader.GlobalXInputReader;
                if (g is null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"未启用 {nameof(WindowsXInputReader)} 脚本");
#endif
                    if(XIn.TryGetState(p_pi, out var s) == 0)
                    {
                        return s.Gamepad.IsButton(bv);
                    }
                    return false;
                }
                else
                {
                    return g.GetXInputData(p_pi).gamePad.IsButton(bv);
                }
            }
        }

        public override bool CanGetPower => true;

        public override bool CanGetMinPower => true;

        public override bool CanGetMaxPower => true;

        public override float MaxPower => 1;

        public override float MinPower => 0;

        public override float Power => ButtonState ? 1 : 0;

        public override bool CanGetChangeFrameButtonDown
        {
            get => WindowsXInputReader.GlobalXInputReader is object;
        }

        public override bool CanGetChangeFrameButtonUp
        {
            get => WindowsXInputReader.GlobalXInputReader is object;
        }

        public override bool ButtonDown
        {
            get
            {
                if (p_t == XInputPadButtonType.None) return false;
                var g = WindowsXInputReader.GlobalXInputReader;
                if (g is null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"未启用 {nameof(WindowsXInputReader)} 脚本");
#endif
                    return false;
                }
                var offset = (int)p_t - 1;
                var bv = (GamePadButtons)(1U << offset);
                if (g.IsXInputLink(p_pi)) return (g.GetXInputData(p_pi).nowFrameDown & bv) == bv;
                return false;
            }
        }

        public override bool ButtonUp
        {
            get
            {
                if (p_t == XInputPadButtonType.None) return false;
                var g = WindowsXInputReader.GlobalXInputReader;
                if (g is null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"未启用 {nameof(WindowsXInputReader)} 脚本");
#endif
                    return false;
                }
                var offset = (int)p_t - 1;
                var bv = (GamePadButtons)(1U << offset);
                if(g.IsXInputLink(p_pi)) return (g.GetXInputData(p_pi).nowFrameUp & bv) == bv;
                return false;
            }
        }

        #endregion

        #region 功能

        /// <summary>
        /// 控制器按钮类型
        /// </summary>
        public XInputPadButtonType XInputType
        {
            get => p_t;
            set => p_t = value;
        }

        #endregion

    }

}

#endif