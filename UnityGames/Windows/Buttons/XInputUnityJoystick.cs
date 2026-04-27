#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;
using Cheng.Algorithm;
using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;
using Cheng.ButtonTemplates.Joysticks;

using GObj = UnityEngine.GameObject;
using UObj = UnityEngine.Object;
using XIn = Cheng.Unitys.Windows.XInput.Win32API.WindowsXInput;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

#if UNITY_EDITOR
    /// <summary>
    /// XInput控制器摇杆
    /// </summary>
    /// <remarks>
    /// <para>允许作为脚本参数在 Inspector 中设置索引和摇杆类型</para>
    /// </remarks>
#else
    /// <summary>
    /// XInput控制器摇杆
    /// </summary>
#endif
    [Serializable]
    public sealed class XInputUnityJoystick : XInputBaseUnityJoystick
    {

        #region 构造

        public XInputUnityJoystick()
        {
            p_t = XInputJoystickType.None; p_i = 0;
        }

        public XInputUnityJoystick(XInputJoystickType type)
        {
            p_t = type; p_i = 0;
        }

        public XInputUnityJoystick(XInputJoystickType type, int index)
        {
            p_t = type; p_i = index;
        }

        #endregion

        #region 参数

        [SerializeField] private int p_i;
        [SerializeField] private XInputJoystickType p_t;

#if UNITY_EDITOR

        internal const string cp_fieldName_index = nameof(p_i);

        internal const string cp_fieldName_button = nameof(p_t);

#endif

        #endregion

        #region 派生

        public override float Horizontal
        {
            get
            {
                GetAxis(out var re, out _);
                return re;
            }
        }

        public override float Vertical
        {
            get
            {
                GetAxis(out _, out var re);
                return re;
            }
        }

        public override void GetAxis(out float horizontal, out float vertical)
        {
            horizontal = 0; vertical = 0;
            if (p_t == XInputJoystickType.None)
            {
                return;
            }

            if (XIn.TryGetState(p_i, out var s) == 0)
            {
                if (p_t == XInputJoystickType.LeftJoystick)
                {
                    horizontal = XInputGamePad.GetJoystickF(s.Gamepad.thumbLX);
                    vertical = XInputGamePad.GetJoystickF(s.Gamepad.thumbLY);
                }
                else if (p_t == XInputJoystickType.RightJoystick)
                {
                    horizontal = XInputGamePad.GetJoystickF(s.Gamepad.thumbRX);
                    vertical = XInputGamePad.GetJoystickF(s.Gamepad.thumbRY);
                }
            }
        }

        public override void GetAxisD(out double horizontal, out double vertical)
        {
            GetAxis(out var h, out var v);
            horizontal = h; vertical = v;
        }

        public override void GetVector(out float radian, out float length)
        {
            GetAxis(out var h, out var v);
            GetVectorRadionAndLength(h, v, out radian, out length);
        }

        public override void GetVectorD(out double radian, out double length)
        {
            GetAxis(out var h, out var v);
            GetVectorRadionAndLength(h, v, out radian, out length);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 摇杆绑定的控制器索引
        /// </summary>
        public override int PadIndex
        {
            get => p_i;
            set
            {
                p_i = value.Clamp(0, 3);
            }
        }

        /// <summary>
        /// 摇杆类型
        /// </summary>
        public override XInputJoystickType XInputType
        {
            get => p_t;
            set => p_t = value;
        }

        #endregion

    }

}

#endif