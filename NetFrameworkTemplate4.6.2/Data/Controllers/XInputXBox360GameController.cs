using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.Windows.XInput;
using Cheng.Controllers.XInput.Buttons;
using Cheng.Algorithm;

namespace Cheng.Controllers.XInput
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
    /// 用XInput控制器连接到XBox360按键分布的手柄
    /// </summary>
    public sealed class XInputXBox360GameController : XBox360GameController
    {

        #region 构造

        /// <summary>
        /// 实例化一个XInput控制器连接对象（创建后不会连接任何手柄）
        /// </summary>
        public XInputXBox360GameController() : this(-1)
        {
        }

        /// <summary>
        /// 实例化一个XInput控制器连接对象
        /// </summary>
        /// <param name="index">控制器索引，可用范围在[0,3]；-1表示不连接任何XInput控制器</param>
        public XInputXBox360GameController(int index)
        {
            p_index = index.Clamp(-1, 3);
            f_init();
        }

        private void f_init()
        {
            p_buttons = f_createButs();
            p_leftJ = new XInputJoystick(this, false);
            p_rightJ = new XInputJoystick(this, true);
        }

        private XInputButton[] f_createButs()
        {
            var arr = new XInputButton[16];

            arr[0] = new XInputPadButton(this, GamePadButtons.DPadUp);
            arr[1] = new XInputPadButton(this, GamePadButtons.DPadDown);
            arr[2] = new XInputPadButton(this, GamePadButtons.DPadLeft);
            arr[3] = new XInputPadButton(this, GamePadButtons.DPadRight);

            arr[4] = new XInputPadButton(this, GamePadButtons.Menu);
            arr[5] = new XInputPadButton(this, GamePadButtons.Back);

            arr[6] = new XInputPadButton(this, GamePadButtons.LeftThumb);
            arr[7] = new XInputPadButton(this, GamePadButtons.RightThumb);

            arr[8] = new XInputPadButton(this, GamePadButtons.LeftShoulder);
            arr[9] = new XInputPadButton(this, GamePadButtons.RightShoulder);

            arr[12] = new XInputPadButton(this, GamePadButtons.A);
            arr[13] = new XInputPadButton(this, GamePadButtons.B);
            arr[14] = new XInputPadButton(this, GamePadButtons.X);
            arr[15] = new XInputPadButton(this, GamePadButtons.Y);

            //扳机
            const byte deftrb = (byte)(255d * (0.4));
            arr[10] = new XInputTriggerButton(this, false, deftrb);
            arr[11] = new XInputTriggerButton(this, false, deftrb);

            return arr;
        }

        #endregion

        #region 参数

        internal int p_index;

        private XInputButton[] p_buttons;

        private XInputJoystick p_leftJ;
        private XInputJoystick p_rightJ;

        #endregion

        #region 功能

        /// <summary>
        /// 控制器连接索引
        /// </summary>
        /// <value>控制器连接到的索引，-1表示无连接</value>
        public int LinkIndex
        {
            get => p_index;
            set
            {
                p_index = value.Clamp(-1, 3);
            }
        }

        /// <summary>
        /// 获取XInput控制器按钮
        /// </summary>
        /// <param name="index">索引范围0-15</param>
        /// <returns></returns>
        public XInputButton GetXInputButton(int index)
        {
            if (index < 0 || index > 15) throw new NotSupportedException();
            return p_buttons[index];
        }

        /// <summary>
        /// 设置两个扳机力度映射到状态变化的中间值
        /// </summary>
        /// <param name="mid">扳机按钮状态变化的中间值，采用归一化参数，范围在[0,1]</param>
        public void SetTriggerStateMid01(float mid)
        {
            SetAllTriggerStateMid((byte)(mid * 255f));
        }

        /// <summary>
        /// 设置两个扳机力度映射到状态变化的中间值
        /// </summary>
        /// <param name="mid">扳机按钮状态变化的中间值</param>
        public void SetAllTriggerStateMid(byte mid)
        {
            var but = (XInputTriggerButton)p_buttons[10];
            but.p_down = mid;
            but = (XInputTriggerButton)p_buttons[11];
            but.p_down = mid;
        }

        /// <summary>
        /// 设置扳机力度映射到状态变化的中间值
        /// </summary>
        /// <remarks>
        /// <para>
        /// 扳机按钮使用线性变化的压力值作为参数，本身没有真值判断的方法<br/>
        /// 为了兼容按钮参数，该对象使用一个中间值，将按压力度映射到状态值<see cref="BaseButton.ButtonState"/><br/>
        /// 当扳机压力大于设置的<paramref name="mid"/>，则按钮状态是true，否则是false
        /// </para>
        /// <para></para>
        /// </remarks>
        /// <param name="right">要设置的扳机键，false是左扳机，true是右扳机</param>
        /// <param name="mid">扳机按钮状态变化的中间值</param>
        public void SetTriggerStateMid(bool right, byte mid)
        {
            XInputTriggerButton but;
            if (right)
            {
                but = (XInputTriggerButton)p_buttons[11];
            }
            else
            {
                but = (XInputTriggerButton)p_buttons[10];
            }
            but.p_down = mid;
        }

        /// <summary>
        /// 重新从0遍历XInput控制器的连接状态，并设置连接索引
        /// </summary>
        /// <returns>连接到的索引，如果不存在任何正在连接的手柄，则返回-1</returns>
        public int ResetForeachActive()
        {
            p_index = -1;
            int i;
            for (i = 0; i < 4; i++)
            {
                if (Win32XInput.Connected(i, out _))
                {
                    p_index = i;
                    break;
                }
            }
            return p_index;
        }

        /// <summary>
        /// 遍历XInput控制器的可用性，并创建一个XInput控制器连接对象
        /// </summary>
        /// <returns>XInput控制器连接对象</returns>
        public static XInputXBox360GameController CreateXInputNextActive()
        {
            var x = new XInputXBox360GameController();
            x.ResetForeachActive();
            return x;
        }

        #endregion

        #region 派生

        public override bool CanGetActivated => true;

        public override bool Activated => Win32XInput.Connected(p_index, out _);

        public override int ButtonCount => 16;

        public override int JoystickCount => 2;

        public override BaseButton GetButton(int index)
        {
            return GetXInputButton(index);
        }

        public override BaseJoystick GetJoystick(int index)
        {
            switch (index)
            {
                case 0:
                    return p_leftJ;
                case 1:
                    return p_rightJ;
                default:
                    throw new NotSupportedException();
            }
        }

        #region

        public override BaseJoystick LeftJoystick => p_leftJ;

        public override BaseJoystick RightJoystick => p_rightJ;

        public override BaseButton ButtonUp => p_buttons[0];

        public override BaseButton ButtonDown => p_buttons[1];

        public override BaseButton ButtonLeft => p_buttons[2];

        public override BaseButton ButtonRight => p_buttons[3];

        public override BaseButton ButtonMenu => p_buttons[4];

        public override BaseButton ButtonBack => p_buttons[5];

        public override BaseButton ButtonLeftThumb => p_buttons[6];

        public override BaseButton ButtonRightThumb => p_buttons[7];

        public override BaseButton ButtonLS => p_buttons[8];

        public override BaseButton ButtonRS => p_buttons[9];

        public override BaseButton ButtonLT => p_buttons[10];

        public override BaseButton ButtonRT => p_buttons[11];

        public override BaseButton ButtonA => p_buttons[12];

        public override BaseButton ButtonB => p_buttons[13];

        public override BaseButton ButtonX => p_buttons[14];

        public override BaseButton ButtonY => p_buttons[15];

        public override bool CanVibration => true;

        public override bool SetXBox360Vibration(ushort leftMotor, ushort rightMotor)
        {
            return Win32XInput.TrySetState(p_index, new XInputVibration(leftMotor, rightMotor)) == 0;
        }

        public override bool GetXBox360Vibration(out ushort leftMotor, out ushort rightMotor)
        {
            var re = Win32XInput.TryGetCapabilities(p_index, out var cap);
            if (re == 0)
            {
                leftMotor = cap.Vibration.leftMotorSpeed;
                rightMotor = cap.Vibration.rightMotorSpeed;
                return true;
            }
            leftMotor = 0; rightMotor = 0;
            return false;
        }

        #endregion

        public override int VibrationCount => 2;

        #endregion


    }

}