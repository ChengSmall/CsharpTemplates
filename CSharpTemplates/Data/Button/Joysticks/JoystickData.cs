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

    /// <summary>
    /// 摇杆控制器权限枚举
    /// </summary>
    [Flags]
    public enum JoystickAvailablePermissions : uint
    {

        /// <summary>
        /// 空值
        /// </summary>
        None = 0,

        #region 摇杆参数

        #region 摇杆分量

        /// <summary>
        /// 允许获取摇杆水平数据分量
        /// </summary>
        CanGetHorizontalComponent = 0b00000001,

        /// <summary>
        /// 允许获取摇杆垂直数据分量
        /// </summary>
        CanGetVerticalComponent = 0b00000010,

        /// <summary>
        /// 允许设置摇杆水平数据分量
        /// </summary>
        CanSetHorizontalComponent = 0b00000100,

        /// <summary>
        /// 允许设置摇杆垂直数据分量
        /// </summary>
        CanSetVerticalComponent = 0b00001000,

        #endregion

        #region 偏移轴

        /// <summary>
        /// 允许获取摇杆偏移轴数据
        /// </summary>
        CanGetVector = 0b00010000,

        /// <summary>
        /// 允许设置摇杆偏移轴数据
        /// </summary>
        CanSetVector = 0b00100000,

        #endregion

        #region 反转参数

        /// <summary>
        /// 允许访问水平方向摇杆反转参数
        /// </summary>
        CanGetHorizontalReverse = 0b01000000,

        /// <summary>
        /// 允许访问垂直方向摇杆反转参数
        /// </summary>
        CanGetVerticalReverse = 0b10000000,

        /// <summary>
        /// 允许设置水平方向摇杆反转参数
        /// </summary>
        CanSetHorizontalReverse = 0b00000001_00000000,

        /// <summary>
        /// 允许设置垂直方向摇杆反转参数
        /// </summary>
        CanSetVerticalReverse = 0b00000010_00000000,

        /// <summary>
        /// 允许访问和设置两个方向的摇杆反转参数
        /// </summary>
        CanSetAndGetAllReverse = CanGetHorizontalReverse | CanGetVerticalReverse |
            CanSetHorizontalReverse | CanSetVerticalReverse,

        #endregion

        #endregion

        #region 其它

        /// <summary>
        /// 允许使用摇杆数据改变事件
        /// </summary>
        CanChangeEvent = 0b00000100_00000000,

        /// <summary>
        /// 允许将摇杆参数当作四向按钮获取参数
        /// </summary>
        CanGetFourwayButtons = 0b00001000_00000000,

        /// <summary>
        /// 允许将摇杆参数当作四向按钮设置参数
        /// </summary>
        CanSetFourwayButtons = 0b00010000_00000000,

        /// <summary>
        /// 允许获取内部封装的摇杆
        /// </summary>
        CanGetInternalJoystick = 0b00100000_00000000,

        /// <summary>
        /// 允许设置内部封装的摇杆
        /// </summary>
        CanSetInternalJoystick = 0b01000000_00000000,

        /// <summary>
        /// 允许访问和设置内部封装的摇杆
        /// </summary>
        CanGetSetInternalJoystick = CanGetInternalJoystick | CanSetInternalJoystick,

        #endregion

        #region 精度控制

        /// <summary>
        /// 指示实际的可用值属于单精度类型还是双精度类型（参考值）
        /// </summary>
        /// <remarks>
        /// <para>该权限用于指示在成功获取摇杆参数时，究竟是<see cref="BaseJoystick.Horizontal"/>这类参数表示为可用的实际值，还是<see cref="BaseJoystick.HorizontalD"/>这类参数表示为可用的实际值</para>
        /// <para>如果摇杆不存在该值，则实际可用的值表示单精度参数，双精度参数是从单精度参数强制转换而来的参数；如果存在该值，则实际可用的值表示双精度参数，单精度参数是从双精度参数强制转换而来的参数</para>
        /// </remarks>
        IndependentDouble = 0b01_00000000_00000000,

        #endregion

        #region 组合

        /// <summary>
        /// 允许获取垂直方向和竖直方向以及摇杆偏移数据
        /// </summary>
        CanGetAllJoystick = CanGetVector |
            CanGetHorizontalComponent | CanGetVerticalComponent,

        /// <summary>
        /// 允许除了<see cref="CanChangeEvent"/>、<see cref="CanGetFourwayButtons"/>和<see cref="CanSetFourwayButtons"/>之外的所有权限
        /// </summary>
        NotEventAndButtons = ~(CanGetFourwayButtons | CanChangeEvent | CanSetFourwayButtons),

        /// <summary>
        /// 拥有所有权限
        /// </summary>
        AllPermissions = 0xFFFFFFFF,

        #endregion

    }


}
