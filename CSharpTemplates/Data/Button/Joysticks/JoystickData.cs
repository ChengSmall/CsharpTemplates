using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Algorithm;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.ButtonTemplates.Joysticks
{

    /// <summary>
    /// 摇杆事件委托
    /// </summary>
    /// <param name="joystick">引发事件的摇杆</param>
    public delegate void JoystickEvent(BaseJoystick joystick);

    /// <summary>
    /// 摇杆事件委托
    /// </summary>
    /// <typeparam name="Arg"></typeparam>
    /// <param name="joystick">引发事件的摇杆</param>
    /// <param name="arg">事件参数</param>
    public delegate void JoystickEvent<Arg>(BaseJoystick joystick, Arg arg);

}
