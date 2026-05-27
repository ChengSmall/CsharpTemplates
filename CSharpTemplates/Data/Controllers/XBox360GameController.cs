using System.Collections.Generic;
using System.Collections;
using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks;
using System;

namespace Cheng.Controllers
{

    /// <summary>
    /// XBox360手柄模板
    /// </summary>
    public abstract class XBox360GameController : GameController
    {

        #region 功能

        /// <summary>
        /// 拥有16个按钮，14个按键和2个扳机键
        /// </summary>
        public override HavingButton HavingButtons
        {
            get => HavingButton.Top16;
        }

        /// <summary>
        /// 拥有一左一右两个摇杆，编号分别是1，2
        /// </summary>
        public override HavingJoystick HavingJoysticks
        {
            get => HavingJoystick.Top2;
        }

        /// <summary>
        /// 拥有2个电机，1号是低频电机，2号是高频电机
        /// </summary>
        public override HavingVibration HavingVibrations
        {
            get => HavingVibration.TwoVibration;
        }

        #region 参数

        /// <summary>
        /// 主摇杆（左侧摇杆）
        /// </summary>
        public virtual BaseJoystick LeftJoystick => throw new NotSupportedException();

        /// <summary>
        /// 副摇杆（右侧摇杆）
        /// </summary>
        public virtual BaseJoystick RightJoystick => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——方向键 上
        /// </summary>
        public virtual BaseButton ButtonUp => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——方向键 下
        /// </summary>
        public virtual BaseButton ButtonDown => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——方向键 左
        /// </summary>
        public virtual BaseButton ButtonLeft => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——方向键 右
        /// </summary>
        public virtual BaseButton ButtonRight => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——菜单键
        /// </summary>
        public virtual BaseButton ButtonMenu => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——返回键（Back）
        /// </summary>
        public virtual BaseButton ButtonBack => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——左摇杆按压键
        /// </summary>
        public virtual BaseButton ButtonLeftThumb => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——右摇杆按压键
        /// </summary>
        public virtual BaseButton ButtonRightThumb => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——左肩键
        /// </summary>
        public virtual BaseButton ButtonLS => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——右肩键
        /// </summary>
        public virtual BaseButton ButtonRS => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——左扳机
        /// </summary>
        public virtual BaseButton ButtonLT => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——右扳机
        /// </summary>
        public virtual BaseButton ButtonRT => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——A
        /// </summary>
        public virtual BaseButton ButtonA => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——B
        /// </summary>
        public virtual BaseButton ButtonB => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——X
        /// </summary>
        public virtual BaseButton ButtonX => throw new NotSupportedException();

        /// <summary>
        /// XBox手柄按键——Y
        /// </summary>
        public virtual BaseButton ButtonY => throw new NotSupportedException();

        /// <summary>
        /// 设置xbox360手柄的振动电机
        /// </summary>
        /// <param name="leftMotor">低频电机</param>
        /// <param name="rightMotor">高频电机</param>
        /// <returns>是否成功设置</returns>
        public virtual bool SetXBox360Vibration(ushort leftMotor, ushort rightMotor)
        {
            return false;
        }

        /// <summary>
        /// 获取xbox360手柄的振动电机
        /// </summary>
        /// <param name="leftMotor">低频电机</param>
        /// <param name="rightMotor">高频电机</param>
        /// <returns>是否成功获取</returns>
        public virtual bool GetXBox360Vibration(out ushort leftMotor, out ushort rightMotor)
        {
            leftMotor = 0; rightMotor = 0;
            return false;
        }

        #endregion

        #region 派生

        /// <summary>
        /// 主摇杆（左侧摇杆）
        /// </summary>
        public override BaseJoystick Joystick1 => LeftJoystick;

        /// <summary>
        /// 副摇杆（右侧摇杆）
        /// </summary>
        public override BaseJoystick Joystick2 => RightJoystick;

        /// <summary>
        /// XBox手柄按键——方向键 上
        /// </summary>
        public override BaseButton Button1 => ButtonUp;

        /// <summary>
        /// XBox手柄按键——方向键 下
        /// </summary>
        public override BaseButton Button2 => ButtonDown;

        /// <summary>
        /// XBox手柄按键——方向键 左
        /// </summary>
        public override BaseButton Button3 => ButtonLeft;

        /// <summary>
        /// XBox手柄按键——方向键 右
        /// </summary>
        public override BaseButton Button4 => ButtonRight;

        /// <summary>
        /// XBox手柄按键——菜单键
        /// </summary>
        public override BaseButton Button5 => ButtonMenu;

        /// <summary>
        /// XBox手柄按键——返回键（Back）
        /// </summary>
        public override BaseButton Button6 => ButtonBack;

        /// <summary>
        /// XBox手柄按键——左摇杆按压键
        /// </summary>
        public override BaseButton Button7 => ButtonLeftThumb;

        /// <summary>
        /// XBox手柄按键——右摇杆按压键
        /// </summary>
        public override BaseButton Button8 => ButtonRightThumb;

        /// <summary>
        /// XBox手柄按键——左肩键
        /// </summary>
        public override BaseButton Button9 => ButtonLS;

        /// <summary>
        /// XBox手柄按键——右肩键
        /// </summary>
        public override BaseButton Button10 => ButtonRS;

        /// <summary>
        /// XBox手柄按键——左扳机
        /// </summary>
        public override BaseButton Button11 => ButtonLT;

        /// <summary>
        /// XBox手柄按键——右扳机
        /// </summary>
        public override BaseButton Button12 => ButtonRT;

        /// <summary>
        /// XBox手柄按键——A
        /// </summary>
        public override BaseButton Button13 => ButtonA;

        /// <summary>
        /// XBox手柄按键——B
        /// </summary>
        public override BaseButton Button14 => ButtonB;

        /// <summary>
        /// XBox手柄按键——X
        /// </summary>
        public override BaseButton Button15 => ButtonX;

        /// <summary>
        /// XBox手柄按键——Y
        /// </summary>
        public override BaseButton Button16 => ButtonY;

        public override BaseButton GetButton(int index)
        {
            switch (index)
            {
                case 0:
                    return Button1;
                case 1:
                    return Button2;
                case 2:
                    return Button3;
                case 3:
                    return Button4;
                case 4:
                    return Button5;
                case 5:
                    return Button6;
                case 6:
                    return Button7;
                case 7:
                    return Button8;
                case 8:
                    return Button9;
                case 9:
                    return Button10;
                case 10:
                    return Button11;
                case 11:
                    return Button12;
                case 12:
                    return Button13;
                case 13:
                    return Button14;
                case 14:
                    return Button15;
                case 15:
                    return Button16;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void StopAllVibration()
        {
            SetXBox360Vibration(0, 0);
        }

        public override float GetVibration(int index)
        {
            GetXBox360Vibration(out var left, out var right);
            switch (index)
            {
                case 0:
                    return left;
                case 1:
                    return right;
                default:
                    return 0;
            }
        }

        public override bool SetVibration(int index, float power)
        {
            if (!GetXBox360Vibration(out var left, out var right)) return false;
            switch (index)
            {
                case 0:
                    SetXBox360Vibration((byte)(power * 255), right);
                    return true;
                case 1:
                    SetXBox360Vibration(left, (byte)(power * 255));
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #endregion

    }

}