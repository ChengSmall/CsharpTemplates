using System;
using System.Collections.Generic;


namespace Cheng.Controllers
{

    /// <summary>
    /// 手柄拥有的轴状态位或枚举
    /// </summary>
    [Flags]
    public enum HavingJoystick : byte
    {

        Joystick1 = 0b1,

        Joystick2 = 0b10,

        Joystick3 = 0b100,

        Joystick4 = 0b1000,

        Joystick5 = 0b10000,

        Joystick6 = 0b100000,

        Joystick7 = 0b1000000,

        Joystick8 = 0b10000000,

        /// <summary>
        /// 表示拥有编号为1-3的摇杆
        /// </summary>
        Top3 = 0b111,

        /// <summary>
        /// 拥有全部轴
        /// </summary>
        AllJoystick = byte.MaxValue,

    }

    /// <summary>
    /// 手柄拥有的按钮状态位或枚举
    /// </summary>
    [Flags]
    public enum HavingButton : uint
    {

        Button1 = 0b1,
        Button2 = 0b10,
        Button3 = 0b100,
        Button4 = 0b1000,
        Button5 = 0b10000,
        Button6 = 0b100000,
        Button7 = 0b1000000,
        Button8 = 0b10000000,

        Button9 = 0b10000000_0,
        Button10 = 0b10000000_00,
        Button11 = 0b10000000_000,
        Button12 = 0b10000000_0000,
        Button13 = 0b10000000_00000,
        Button14 = 0b10000000_000000,
        Button15 = 0b10000000_0000000,
        Button16 = 0b10000000_00000000,

        Button17 = 0b10000000_00000000_0,
        Button18 = 0b10000000_00000000_00,
        Button19 = 0b10000000_00000000_000,
        Button20 = 0b10000000_00000000_0000,
        Button21 = 0b10000000_00000000_00000,
        Button22 = 0b10000000_00000000_000000,
        Button23 = 0b10000000_00000000_0000000,
        Button24 = 0b10000000_00000000_00000000,

        Button25 = 0b10000000_00000000_00000000_0,
        Button26 = 0b10000000_00000000_00000000_00,
        Button27 = 0b10000000_00000000_00000000_000,
        Button28 = 0b10000000_00000000_00000000_0000,
        Button29 = 0b10000000_00000000_00000000_00000,
        Button30 = 0b10000000_00000000_00000000_000000,
        Button31 = 0b10000000_00000000_00000000_0000000,
        Button32 = 0b10000000_00000000_00000000_00000000,

        /// <summary>
        /// 表示拥有编号为1-4的按钮
        /// </summary>
        Top4 = 0b1111,

        /// <summary>
        /// 表示拥有编号为1-8的按钮
        /// </summary>
        Top8 = byte.MaxValue,

        /// <summary>
        /// 表示拥有编号为1-10的按钮
        /// </summary>
        Top10 = 0b11_11111111,

        /// <summary>
        /// 表示拥有编号为1-12的按钮
        /// </summary>
        Top12 = 0b1111_11111111,

        /// <summary>
        /// 表示拥有编号为1-13的按钮
        /// </summary>
        Top13 = 0b00011111_11111111,

        /// <summary>
        /// 拥有全部32个按钮
        /// </summary>
        All = uint.MaxValue,
    }

}
