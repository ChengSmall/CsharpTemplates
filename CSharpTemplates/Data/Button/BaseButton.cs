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
        /// 该按钮是否允许访问按钮状态
        /// </summary>
        public virtual bool CanGetState => false;

        /// <summary>
        /// 该按钮是否允许设置按钮状态
        /// </summary>
        public virtual bool CanSetState => false;

        /// <summary>
        /// 该按钮是否允许访问按钮力度
        /// </summary>
        /// 
        public virtual bool CanGetPower => false;

        /// <summary>
        /// 该按钮是否允许设置按钮力度
        /// </summary>
        public virtual bool CanSetPower => false;

        /// <summary>
        /// 是否允许获取力度按钮最大值参数
        /// </summary>
        public virtual bool CanGetMaxPower => false;

        /// <summary>
        /// 是否允许获取力度按钮最小值参数
        /// </summary>
        public virtual bool CanGetMinPower => false;

        /// <summary>
        /// 是否允许设置力度按钮最大值参数
        /// </summary>
        public virtual bool CanSetMaxPower => false;

        /// <summary>
        /// 是否允许设置力度按钮最小值参数
        /// </summary>
        public virtual bool CanSetMinPower => false;

        /// <summary>
        /// 该按钮的按下事件是否可用
        /// </summary>
        public virtual bool CanButtonDownEvent => false;

        /// <summary>
        /// 该按钮的松开事件是否可用
        /// </summary>
        public virtual bool CanButtonUpEvent => false;

        /// <summary>
        /// 按钮被点击事件是否可用
        /// </summary>
        public virtual bool CanButtonClick => false;

        /// <summary>
        /// 是否允许访问按钮按下的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否按下状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public virtual bool CanGetChangeFrameButtonDown => false;

        /// <summary>
        /// 是否允许访问按钮抬起的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否抬起状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public virtual bool CanGetChangeFrameButtonUp => false;

        /// <summary>
        /// 是否允许设置按钮按下的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否按下状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public virtual bool CanSetChangeFrameButtonDown => false;

        /// <summary>
        /// 是否允许设置按钮抬起的帧变化状态
        /// </summary>
        /// <returns>该属性返回为true时，表示可以使用帧变化是否抬起状态参数，可以使用<see cref="NowFrame"/>属性获取当前帧数，该参数通常用于游戏引擎</returns>
        public virtual bool CanSetChangeFrameButtonUp => false;

        /// <summary>
        /// 是否允许获取当前帧数
        /// </summary>
        public virtual bool CanGetFrameValue => false;

        /// <summary>
        /// 该按钮是否拥有内部封装的按钮并允许获取
        /// </summary>
        public virtual bool CanGetInternalButton => false;

        /// <summary>
        /// 该按钮是否拥有内部封装的按钮并允许设置
        /// </summary>
        public virtual bool CanSetInternalButton => false;

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
        /// 获取内部封装的按钮
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <returns>内部封装的按钮，若类型不匹配则为null</returns>
        /// <exception cref="NotSupportedException">参数<see cref="CanGetInternalButton"/>为false</exception>
        public virtual T GetInternalButton<T>() where T : BaseButton
        {
            return InternalButton as T;
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
