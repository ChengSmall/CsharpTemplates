using System.Collections.Generic;
using System.Collections;
using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.Memorys;
using System;

namespace Cheng.Controllers
{

    /// <summary>
    /// 一个游戏手柄基类
    /// </summary>
    /// <remarks>派生此类实现游戏手柄</remarks>
    public abstract class GameController : BaseController
    {

        #region 功能

        /// <summary>
        /// 表示游戏手柄拥有的摇杆
        /// </summary>
        /// <remarks>
        /// 默认返回编号为1，2，3的摇杆枚举
        /// </remarks>
        /// <returns>
        /// 拥有的游戏摇杆编号枚举
        /// </returns>
        public override HavingJoystick HavingJoysticks
        {
            get
            {
                return HavingJoystick.Joystick1 | HavingJoystick.Joystick2 | HavingJoystick.Joystick3;
            }
        }

        /// <summary>
        /// 表示游戏手柄拥有的按钮
        /// </summary>
        /// <remarks>
        /// 默认返回编号为1-12的按钮枚举
        /// </remarks>
        /// <returns>
        /// 拥有的游戏按钮编号枚举
        /// </returns>
        public override HavingButton HavingButtons
        {
            get
            {
                return HavingButton.Button1 | HavingButton.Button2 | HavingButton.Button3 | HavingButton.Button4 | HavingButton.Button5 | HavingButton.Button6 | HavingButton.Button7 | HavingButton.Button8 | HavingButton.Button9 | HavingButton.Button10 | HavingButton.Button11 | HavingButton.Button12;                 
            }
        }

        /// <summary>
        /// 表示游戏手柄的主摇杆
        /// </summary>
        /// <remarks>
        /// 摇杆通常存在于手柄的左侧
        /// </remarks>
        public override BaseJoystick Joystick1
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 表示游戏手柄的副摇杆
        /// </summary>
        /// <remarks>
        /// 摇杆通常存在于手柄的右下
        /// </remarks>
        public override BaseJoystick Joystick2
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 表示游戏手柄的第三个摇杆
        /// </summary>
        /// <remarks>
        /// 摇杆通常存在于手柄的左下
        /// </remarks>
        public override BaseJoystick Joystick3
        {
            get => throw new NotSupportedException();
        }


        /// <summary>
        /// 右侧操作按键 1
        /// </summary>
        public override BaseButton Button1 => base.Button1;

        /// <summary>
        /// 右侧操作按键 2
        /// </summary>
        public override BaseButton Button2 => base.Button2;

        /// <summary>
        /// 右侧操作按键 3
        /// </summary>
        public override BaseButton Button3 => base.Button3;

        /// <summary>
        /// 右侧操作按键 4
        /// </summary>
        public override BaseButton Button4 => base.Button4;

        /// <summary>
        /// 表示左侧肩键 1
        /// </summary>
        public override BaseButton Button5 => base.Button5;

        /// <summary>
        /// 表示左侧肩键 2
        /// </summary>
        public override BaseButton Button6 => base.Button6;

        /// <summary>
        /// 表示右侧肩键 1
        /// </summary>
        public override BaseButton Button7 => base.Button7;

        /// <summary>
        /// 表示右侧肩键 2
        /// </summary>
        public override BaseButton Button8 => base.Button8;

        /// <summary>
        /// 表示菜单键 1
        /// </summary>
        public override BaseButton Button9 => base.Button9;

        /// <summary>
        /// 表示菜单键 2
        /// </summary>
        public override BaseButton Button10 => base.Button10;

        /// <summary>
        /// 主摇杆下压按键
        /// </summary>
        public override BaseButton Button11 => base.Button11;

        /// <summary>
        /// 副摇杆下压按键
        /// </summary>
        public override BaseButton Button12 => base.Button11;

        #endregion

    }

}
