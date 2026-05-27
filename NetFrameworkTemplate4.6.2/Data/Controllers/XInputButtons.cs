using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.Windows.XInput;

namespace Cheng.Controllers.XInput.Buttons
{

    /// <summary>
    /// XInput控制器按键
    /// </summary>
    public abstract class XInputButton : BaseButton
    {

        #region

        internal XInputButton(XInputXBox360GameController controller)
        {
            p_ct = controller;
        }

        #endregion

        #region

        internal XInputXBox360GameController p_ct;

        #endregion

    }

    internal sealed class XInputPadButton : XInputButton
    {

        #region 构造

        internal XInputPadButton(XInputXBox360GameController controller, GamePadButtons button) : base(controller)
        {
            p_but = button;
        }

        #endregion

        #region 参数

        private readonly GamePadButtons p_but;

        #endregion

        #region 派生

        public override bool CanGetState => Win32XInput.Connected(p_ct.p_index, out _);

        public override bool ButtonState
        {
            get
            {
                if (Win32XInput.TryGetState(p_ct.p_index, out var s) == 0)
                {
                    return s.Gamepad.IsButton(p_but);
                }
                return false;
            }
        }

        public override bool CanGetPower => CanGetState;

        public override float Power
        {
            get
            {
                return ButtonState ? 1 : 0;
            }
        }

        public override bool CanGetMinPower => true;

        public override bool CanGetMaxPower => true;

        public override float MinPower => 0;

        public override float MaxPower => 1;

        #endregion

    }

    internal sealed class XInputTriggerButton : XInputButton
    {

        #region 构造

        internal XInputTriggerButton(XInputXBox360GameController controller, bool RT, byte dsmv) : base(controller)
        {
            p_rt = RT; p_down = dsmv;
        }

        #endregion

        #region 参数

        internal byte p_down;
        private bool p_rt;

        #endregion

        #region 派生

        public override bool CanGetPower => Win32XInput.Connected(p_ct.p_index, out _);

        public override float Power
        {
            get
            {
                if (Win32XInput.TryGetState(p_ct.p_index, out var s) == 0)
                {
                    return p_rt ? s.Gamepad.RightTrigger : s.Gamepad.LeftTrigger;
                }
                return 0;
            }
        }

        public override bool CanGetMinPower => true;

        public override bool CanGetMaxPower => true;

        public override float MinPower => 0;

        public override float MaxPower => 1;

        public override bool CanGetState => CanGetPower;

        public override bool ButtonState
        {
            get
            {
                if (Win32XInput.TryGetState(p_ct.p_index, out var s) == 0)
                {
                    if (p_rt)
                    {
                        return s.Gamepad.rightTrigger > p_down;
                    }
                    else
                    {
                        return s.Gamepad.leftTrigger > p_down;
                    }
                }
                return false;
            }
        }

        #endregion

    }

    /// <summary>
    /// XInput控制器摇杆
    /// </summary>
    public sealed class XInputJoystick : BaseJoystick
    {

        #region 构造

        internal XInputJoystick(XInputXBox360GameController controller, bool right)
        {
            p_ct = controller; p_right = right;
        }

        #endregion

        #region 参数

        internal XInputXBox360GameController p_ct;

        private bool p_right;

        #endregion

        #region 派生

        public override bool CanGetVector => Win32XInput.Connected(p_ct.p_index, out _);

        public override bool CanGetHorizontalComponent => CanGetVector;

        public override bool CanGetVerticalComponent => CanGetVector;

        public override void GetAxis(out float horizontal, out float vertical)
        {
            if(Win32XInput.TryGetState(p_ct.p_index, out var state) == 0)
            {
                if (p_right)
                {
                    horizontal = XInputGamePad.GetJoystickF(state.Gamepad.thumbRX);
                    vertical = XInputGamePad.GetJoystickF(state.Gamepad.thumbRY);
                }
                else
                {
                    horizontal = XInputGamePad.GetJoystickF(state.Gamepad.thumbLX);
                    vertical = XInputGamePad.GetJoystickF(state.Gamepad.thumbLY);
                }
            }
            else
            {
                horizontal = 0; vertical = 0;
            }
        }

        public override void GetAxisD(out double horizontal, out double vertical)
        {
            GetAxis(out var x, out var y);
            horizontal = x; vertical = y;
        }

        public override void GetVector(out float radian, out float length)
        {
            GetAxis(out var x, out var y);
            GetVectorRadionAndLength(x, y, out radian, out length);
        }

        public override void GetVectorD(out double radian, out double length)
        {
            GetAxis(out var x, out var y);
            GetVectorRadionAndLength(x, y, out radian, out length);
        }

        public override float Horizontal
        {
            get
            {
                if (Win32XInput.TryGetState(p_ct.p_index, out var state) == 0)
                {
                    if (p_right)
                    {
                        return XInputGamePad.GetJoystickF(state.Gamepad.thumbRX);
                    }
                    else
                    {
                        return XInputGamePad.GetJoystickF(state.Gamepad.thumbLX);
                    }
                }
                return 0;
            }
        }

        public override float Vertical
        {
            get
            {
                if (Win32XInput.TryGetState(p_ct.p_index, out var state) == 0)
                {
                    if (p_right)
                    {
                        return XInputGamePad.GetJoystickF(state.Gamepad.thumbRY);
                    }
                    else
                    {
                        return XInputGamePad.GetJoystickF(state.Gamepad.thumbLY);
                    }
                }
                return 0;
            }
        }

        #endregion

    }

}