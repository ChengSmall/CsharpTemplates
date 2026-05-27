#if UNITY_STANDALONE_WIN

using System;
using UnityEngine;
using Cheng.Algorithm;
using Cheng.Unitys.Windows.XInput;
using Cheng.Unitys.Windows.XInput.Win32API;

using XIn = Cheng.Unitys.Windows.XInput.Win32API.WindowsXInput;

namespace Cheng.ButtonTemplates.Joysticks.Unitys
{

    /// <summary>
    /// XInput控制器摇杆类型
    /// </summary>
    public enum XInputJoystickType
    {

        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 左摇杆（主摇杆）
        /// </summary>
        LeftJoystick = 1,

        /// <summary>
        /// 右摇杆（副摇杆）
        /// </summary>
        RightJoystick = 2

    }

    /// <summary>
    /// XInput控制器摇杆基类
    /// </summary>
    public abstract class XInputBaseUnityJoystick : BaseJoystick
    {

        /// <summary>
        /// 摇杆绑定的控制器索引
        /// </summary>
        public abstract int PadIndex { get; set; }

        /// <summary>
        /// 摇杆类型
        /// </summary>
        public abstract XInputJoystickType XInputType { get; set; }

        public override bool CanGetHorizontalComponent => true;

        public override bool CanGetVerticalComponent => true;

        public override bool CanGetVector => true;

    }

}

#endif