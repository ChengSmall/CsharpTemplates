using System;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 表示按钮事件的委托
    /// </summary>
    /// <typeparam name="B">引发事件的按钮类型</typeparam>
    /// <param name="button">引发事件的按钮</param>
    public delegate void ButtonEvent<in B>(B button) where B : BaseButton;

    /// <summary>
    /// 表示按钮事件的参数委托
    /// </summary>
    /// <typeparam name="B">引发事件的按钮类型</typeparam>
    /// <typeparam name="Arg">事件的参数类型</typeparam>
    /// <param name="button">引发事件的按钮</param>
    /// <param name="arg">事件的参数</param>
    public delegate void ButtonEvent<in B, Arg>(B button, Arg arg) where B : BaseButton;

    /// <summary>
    /// 按钮可用权限枚举
    /// </summary>
    [Flags]
    public enum ButtonAvailablePermissions : uint
    {

        /// <summary>
        /// 没有权限
        /// </summary>
        None = 0,

        #region state

        /// <summary>
        /// 允许获取按钮状态
        /// </summary>
        CanGetState = 0b00000001,

        /// <summary>
        /// 允许设置按钮状态
        /// </summary>
        CanSetState = 0b00000010,

        /// <summary>
        /// 允许访问或设置按钮状态
        /// </summary>
        AllStatePermissions = CanGetState | CanSetState,

        #endregion

        #region power

        /// <summary>
        /// 允许获取按钮力度值
        /// </summary>
        CanGetPower = 0b00000100,

        /// <summary>
        /// 允许设置按钮力度值
        /// </summary>
        CanSetPower = 0b00001000,

        /// <summary>
        /// 允许获取最大力度值
        /// </summary>
        CanGetMaxPower = 0b00010000,

        /// <summary>
        /// 允许获取最小力度值
        /// </summary>
        CanGetMinPower = 0b00100000,

        /// <summary>
        /// 允许设置最大力度值
        /// </summary>
        CanSetMaxPower = 0b01000000,

        /// <summary>
        /// 允许设置最小力度值
        /// </summary>
        CanSetMinPower = 0b10000000,

        /// <summary>
        /// 允许访问或设置力度值
        /// </summary>
        CanSetGetPower = CanGetPower | CanSetPower,

        /// <summary>
        /// 允许Power相关的所有访问权限
        /// </summary>
        AllGetPowerPermissions = CanGetPower | CanGetMaxPower | CanGetMinPower,

        /// <summary>
        /// 允许Power相关的全部权限
        /// </summary>
        AllPowerPermissions = CanGetPower | CanSetPower | 
            CanGetMaxPower | CanGetMinPower | CanSetMaxPower | CanSetMinPower,

        #endregion

        #region event

        /// <summary>
        /// 允许按钮注册按钮按下事件
        /// </summary>
        CanButtonDownEvent = 0b00000001_00000000,

        /// <summary>
        /// 允许按钮注册按钮抬起事件
        /// </summary>
        CanButtonUpEvent = 0b00000010_00000000,

        /// <summary>
        /// 允许按钮注册点击事件
        /// </summary>
        CanButtonClick = 0b00000100_00000000,

        #endregion

        #region frame

        /// <summary>
        /// 允许按钮访问当前帧是否按下
        /// </summary>
        CanGetChangeFrameButtonDown = 0b00001000_00000000,

        /// <summary>
        /// 允许按钮访问当前帧是否抬起
        /// </summary>
        CanGetChangeFrameButtonUp = 0b00010000_00000000,

        /// <summary>
        /// 允许按钮设置当前帧是否按下
        /// </summary>
        CanSetChangeFrameButtonDown = 0b00100000_00000000,

        /// <summary>
        /// 允许按钮设置当前帧抬起
        /// </summary>
        CanSetChangeFrameButtonUp = 0b01000000_00000000,

        /// <summary>
        /// 允许按钮访问当前帧数
        /// </summary>
        CanGetFrameValue = 0b10000000_00000000,

        /// <summary>
        /// 允许所有帧访问相关的权限
        /// </summary>
        AllFrameGetPermissions = CanGetFrameValue | CanGetFrameValue |
            CanGetChangeFrameButtonUp | CanGetChangeFrameButtonDown,

        /// <summary>
        /// 允许所有帧相关权限
        /// </summary>
        AllFramePermissions = CanGetFrameValue | CanGetFrameValue |
            CanSetChangeFrameButtonUp | CanSetChangeFrameButtonDown |
            CanGetChangeFrameButtonUp | CanGetChangeFrameButtonDown,

        #endregion

        #region 封装button

        /// <summary>
        /// 允许按钮获取内部封装的按钮
        /// </summary>
        CanGetInternalButton = 0b01_00000000_00000000,

        /// <summary>
        /// 允许按钮设置内部封装的按钮
        /// </summary>
        CanSetInternalButton = 0b10_00000000_00000000,

        /// <summary>
        /// 允许按钮访问或设置内部封装的按钮
        /// </summary>
        CanSetAndGetInternalButton = CanGetInternalButton | CanSetInternalButton,

        #endregion

        #region 组合

        /// <summary>
        /// 允许状态和力度的访问权限
        /// </summary>
        AllGetStateAndPower = CanGetState | CanGetPower,

        /// <summary>
        /// 允许获取按钮状态和按钮力度以及按钮大小最值
        /// </summary>
        CanGetAllStatePower = CanGetState | AllGetPowerPermissions,

        /// <summary>
        /// 允许所有权限
        /// </summary>
        AllPermissions = 0xFFFFFFFF

        #endregion

    }

    /// <summary>
    /// 表示一个按钮基类
    /// </summary>
    /// <remarks>
    /// 这是一个集合了所有按钮功能的基类，可以派生此类型实现各种按钮功能
    /// </remarks>
    public abstract class BaseButton
    {

        #region 派生

        #region 控制权限

        /// <summary>
        /// 访问该按钮可用权限枚举
        /// </summary>
        public virtual ButtonAvailablePermissions AvailablePermissions
        {
            get => ButtonAvailablePermissions.None;
        }

        /// <summary>
        /// 该按钮是否允许访问按钮状态
        /// </summary>
        public bool CanGetState => (AvailablePermissions & ButtonAvailablePermissions.CanGetState) == ButtonAvailablePermissions.CanGetState;

        /// <summary>
        /// 该按钮是否允许设置按钮状态
        /// </summary>
        public bool CanSetState => (AvailablePermissions & ButtonAvailablePermissions.CanSetState) == ButtonAvailablePermissions.CanSetState;

        /// <summary>
        /// 该按钮是否允许访问按钮力度
        /// </summary>
        /// 
        public bool CanGetPower => (AvailablePermissions & ButtonAvailablePermissions.CanGetPower) == ButtonAvailablePermissions.CanGetPower;

        /// <summary>
        /// 该按钮是否允许设置按钮力度
        /// </summary>
        public bool CanSetPower => (AvailablePermissions & ButtonAvailablePermissions.CanSetPower) == ButtonAvailablePermissions.CanSetPower;

        /// <summary>
        /// 是否允许获取力度按钮最大值参数
        /// </summary>
        public bool CanGetMaxPower => (AvailablePermissions & ButtonAvailablePermissions.CanGetMaxPower) == ButtonAvailablePermissions.CanGetMaxPower;

        /// <summary>
        /// 是否允许获取力度按钮最小值参数
        /// </summary>
        public bool CanGetMinPower => (AvailablePermissions & ButtonAvailablePermissions.CanGetMinPower) == ButtonAvailablePermissions.CanGetMinPower;

        /// <summary>
        /// 是否允许设置力度按钮最大值参数
        /// </summary>
        public bool CanSetMaxPower => (AvailablePermissions & ButtonAvailablePermissions.CanSetMaxPower) == ButtonAvailablePermissions.CanSetMaxPower;

        /// <summary>
        /// 是否允许设置力度按钮最小值参数
        /// </summary>
        public bool CanSetMinPower => (AvailablePermissions & ButtonAvailablePermissions.CanSetMinPower) == ButtonAvailablePermissions.CanSetMinPower;

        /// <summary>
        /// 该按钮的按下事件是否可用
        /// </summary>
        public bool CanButtonDownEvent => (AvailablePermissions & ButtonAvailablePermissions.CanButtonDownEvent) == ButtonAvailablePermissions.CanButtonDownEvent;

        /// <summary>
        /// 该按钮的松开事件是否可用
        /// </summary>
        public bool CanButtonUpEvent => (AvailablePermissions & ButtonAvailablePermissions.CanButtonUpEvent) == ButtonAvailablePermissions.CanButtonUpEvent;

        /// <summary>
        /// 按钮被点击事件是否可用
        /// </summary>
        public bool CanButtonClick => (AvailablePermissions & ButtonAvailablePermissions.CanButtonClick) == ButtonAvailablePermissions.CanButtonClick;

        /// <summary>
        /// 是否允许访问按钮按下的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否按下状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public bool CanGetChangeFrameButtonDown => (AvailablePermissions & ButtonAvailablePermissions.CanGetChangeFrameButtonDown) == ButtonAvailablePermissions.CanGetChangeFrameButtonDown;

        /// <summary>
        /// 是否允许访问按钮抬起的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否抬起状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public bool CanGetChangeFrameButtonUp => (AvailablePermissions & ButtonAvailablePermissions.CanGetChangeFrameButtonUp) == ButtonAvailablePermissions.CanGetChangeFrameButtonUp;

        /// <summary>
        /// 是否允许设置按钮按下的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否按下状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public bool CanSetChangeFrameButtonDown => (AvailablePermissions & ButtonAvailablePermissions.CanSetChangeFrameButtonDown) == ButtonAvailablePermissions.CanSetChangeFrameButtonDown;

        /// <summary>
        /// 是否允许设置按钮抬起的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否抬起状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public bool CanSetChangeFrameButtonUp => (AvailablePermissions & ButtonAvailablePermissions.CanSetChangeFrameButtonUp) == ButtonAvailablePermissions.CanSetChangeFrameButtonUp;

        /// <summary>
        /// 是否允许获取当前帧数
        /// </summary>
        public bool CanGetFrameValue => (AvailablePermissions & ButtonAvailablePermissions.CanGetFrameValue) == ButtonAvailablePermissions.CanGetFrameValue;

        /// <summary>
        /// 该按钮是否拥有内部封装的按钮并允许获取
        /// </summary>
        public bool CanGetInternalButton => (AvailablePermissions & ButtonAvailablePermissions.CanGetInternalButton) == ButtonAvailablePermissions.CanGetInternalButton;

        /// <summary>
        /// 该按钮是否拥有内部封装的按钮并允许设置
        /// </summary>
        public bool CanSetInternalButton => (AvailablePermissions & ButtonAvailablePermissions.CanSetInternalButton) == ButtonAvailablePermissions.CanSetInternalButton;

        #endregion

        #region 参数

        /// <summary>
        /// 获取或设置内部封装的按钮
        /// </summary>
        /// <exception cref="NotSupportedException">参数<see cref="CanGetInternalButton"/>或<see cref="CanSetInternalButton"/>为false</exception>
        public virtual BaseButton InternalButton
        {
            get => ThrowSupportedException<BaseButton>();
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 访问或设置按钮状态
        /// </summary>
        /// <returns>true表示按钮被按下，false表示按钮被抬起</returns>
        /// <exception cref="NotSupportedException">此按钮不允许访问或设置</exception>
        public virtual bool ButtonState
        {
            get => ThrowSupportedException<bool>();
            set
            {
                ThrowSupportedException();
            }
        }

        /// <summary>
        /// 访问或设置按钮力度值
        /// </summary>
        /// <remarks>该属性通常用于平滑轴实现，例如油门，操控杆，手柄扳机，压力板，力度轴等</remarks>
        /// <value>设置按钮力度，值的范围取决于派生类的实现方式</value>
        /// <exception cref="NotSupportedException">此按钮不允许访问或设置</exception>
        public virtual float Power
        {
            get => ThrowSupportedException<float>();
            set
            {
                ThrowSupportedException();
            }
        }

        /// <summary>
        /// 访问或设置表示按钮力度值的最大值
        /// </summary>
        /// <exception cref="NotSupportedException">力度最值参数不允许访问或设置</exception>
        public virtual float MaxPower
        {
            get => ThrowSupportedException<float>();
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 访问或设置表示按钮力度值的最小值
        /// </summary>
        /// <exception cref="NotSupportedException">力度最值参数不允许访问或设置</exception>
        public virtual float MinPower
        {
            get => ThrowSupportedException<float>();
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 是否在当前帧按下
        /// </summary>
        /// <returns>返回一个布尔值，判断此按钮是否在当前帧被按下</returns>
        /// <exception cref="NotSupportedException"><see cref="CanGetChangeFrameButtonDown"/>或<see cref="CanSetChangeFrameButtonDown"/>为false</exception>
        public virtual bool ButtonDown
        {
            get => ThrowSupportedException<bool>();
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 是否在当前帧抬起
        /// </summary>
        /// <returns>返回一个布尔值，判断此按钮是否在当前帧被松开</returns>
        /// <exception cref="NotSupportedException"><see cref="CanGetChangeFrameButtonUp"/>或<see cref="CanSetChangeFrameButtonUp"/>为false</exception>
        public virtual bool ButtonUp
        {
            get => ThrowSupportedException<bool>();
            set => ThrowSupportedException();
        }

        /// <summary>
        /// 获取当前帧数
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="CanGetFrameValue"/>为false</exception>
        public virtual long NowFrame
        {
            get => ThrowSupportedException<long>();
        }

        #endregion

        #region 按钮事件

        /// <summary>
        /// 按下按钮时引发的事件
        /// </summary>
        /// <exception cref="NotSupportedException">此按钮不支持按下事件</exception>
        public virtual event ButtonEvent<BaseButton> ButtonDownEvent
        {
            add => ThrowSupportedException();
            remove => ThrowSupportedException();
        }

        /// <summary>
        /// 松开按钮时引发的事件
        /// </summary>
        /// <exception cref="NotSupportedException">此按钮不支持按钮抬起事件</exception>
        public virtual event ButtonEvent<BaseButton> ButtonUpEvent
        {
            add => ThrowSupportedException();
            remove => ThrowSupportedException();
        }

        /// <summary>
        /// 按钮被点击时引发的事件
        /// </summary>
        /// <exception cref="NotSupportedException">此按钮不支持按钮抬起事件</exception>
        public virtual event ButtonEvent<BaseButton> ButtonClickEvent
        {
            add => ThrowSupportedException();
            remove => ThrowSupportedException();
        }

        #endregion

        #region 异常

        /// <summary>
        /// 调用此函数引发按钮不支持此功能时引发的异常
        /// </summary>
        /// <returns>返回预定默认值；因为此函数会引发异常，因此不会真的返回</returns>
        protected static T ThrowSupportedException<T>()
        {
            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        protected static void ThrowSupportedException()
        {
            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        #endregion

        #endregion

    }

}
