using Cheng.ButtonTemplates;
using Cheng.ButtonTemplates.Joysticks;
using Cheng.Memorys;
using System;

namespace Cheng.Controllers
{

    /// <summary>
    /// 表示一个操作手柄基类
    /// </summary>
    /// <remarks>派生此类实现不同类型的操作手柄</remarks>
    public abstract class BaseController : SafreleaseUnmanagedResources
    {

        #region 释放

        #endregion

        #region 访问权限

        /// <summary>
        /// 一个按位判断的枚举，表示当前手柄拥有哪些编号的摇杆
        /// </summary>
        public virtual HavingJoystick HavingJoysticks => 0;

        /// <summary>
        /// 一个按位判断的枚举，表示当前手柄拥有哪些编号的按钮
        /// </summary>
        public virtual HavingButton HavingButtons => 0;

        /// <summary>
        /// 该手柄是否允许有震动功能
        /// </summary>
        public virtual bool CanVibration => false;

        /// <summary>
        /// 判断该手柄是否拥有指定编号的摇杆
        /// </summary>
        /// <param name="index">手柄编号索引，范围在0-7</param>
        /// <returns>拥有返回true，没有返回false</returns>
        public virtual bool HavingJoystickByIndex(int index)
        {
            return (((uint)HavingJoysticks >> (index)) & 1) == 1;
        }

        /// <summary>
        /// 判断该手柄是否拥有指定编号的按钮
        /// </summary>
        /// <param name="index">按钮编号索引，范围在0-31</param>
        /// <returns>拥有返回true，没有返回false</returns>
        public virtual bool HavingButtonByIndex(int index)
        {
            return (((byte)HavingJoysticks >> (index)) & 1) == 1;
        }

        #endregion

        #region 参数访问

        #region 摇杆

        /// <summary>
        /// 获取指定编号索引的摇杆
        /// </summary>
        /// <param name="index">摇杆编号索引，范围在0-7，分别对应1-8</param>
        /// <returns>指定编号索引的摇杆</returns>
        /// <exception cref="NotSupportedException">无法获取指定编号的摇杆</exception>
        public virtual BaseJoystick GetJoystick(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取指定编号索引的摇杆
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <param name="index">摇杆编号索引，范围在0-7，分别对应1-8</param>
        /// <returns>指定编号索引的摇杆</returns>
        /// <exception cref="NotSupportedException">无法获取指定编号的摇杆</exception>
        /// <exception cref="InvalidCastException">无法将按钮转化成类型<typeparamref name="T"/></exception>
        public virtual T GetJoystick<T>(int index) where T : BaseJoystick
        {
            return (T)GetJoystick(index);
        }

        /// <summary>
        /// 获取手柄1号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick1 => GetJoystick(0);
        /// <summary>
        /// 获取手柄2号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick2 => GetJoystick(1);
        /// <summary>
        /// 获取手柄3号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick3 => GetJoystick(2);
        /// <summary>
        /// 获取手柄4号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick4 => GetJoystick(3);
        /// <summary>
        /// 获取手柄5号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick5 => GetJoystick(4);
        /// <summary>
        /// 获取手柄6号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick6 => GetJoystick(5);
        /// <summary>
        /// 获取手柄7号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick7 => GetJoystick(6);
        /// <summary>
        /// 获取手柄8号摇杆
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此摇杆</exception>
        public virtual BaseJoystick Joystick8 => GetJoystick(7);
        #endregion

        #region 按钮

        /// <summary>
        /// 获取指定编号索引的按钮
        /// </summary>
        /// <param name="index">按钮编号索引，范围在0-31，分别对应1-32</param>
        /// <returns>指定编号索引的按钮</returns>
        /// <exception cref="NotSupportedException">无法获取指定编号的按钮</exception>
        public virtual BaseButton GetButton(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取指定编号索引的按钮
        /// </summary>
        /// <typeparam name="T">获取的按钮类型</typeparam>
        /// <param name="index">按钮编号索引，范围在0-31，分别对应1-32</param>
        /// <returns>指定编号索引的按钮</returns>
        /// <exception cref="NotSupportedException">无法获取指定编号的按钮</exception>
        /// <exception cref="InvalidCastException">无法转换到类型<typeparamref name="T"/></exception>
        public virtual T GetButton<T>(int index) where T : BaseButton
        {
            return (T)GetButton(index);
        }

        /// <summary>
        /// 手柄按钮1
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button1 => GetButton(0);

        /// <summary>
        /// 手柄按钮2
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button2 => GetButton(1);

        /// <summary>
        /// 手柄按钮3
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button3 => GetButton(2);

        /// <summary>
        /// 手柄按钮4
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button4 => GetButton(3);

        /// <summary>
        /// 手柄按钮5
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button5 => GetButton(4);

        /// <summary>
        /// 手柄按钮6
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button6 => GetButton(5);

        /// <summary>
        /// 手柄按钮7
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button7 => GetButton(6);

        /// <summary>
        /// 手柄按钮8
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button8 => GetButton(7);

        /// <summary>
        /// 手柄按钮9
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button9 => GetButton(8);

        /// <summary>
        /// 手柄按钮10
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button10 => GetButton(9);

        /// <summary>
        /// 手柄按钮11
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button11 => GetButton(10);

        /// <summary>
        /// 手柄按钮12
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button12 => GetButton(11);

        /// <summary>
        /// 手柄按钮13
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button13 => GetButton(12);

        /// <summary>
        /// 手柄按钮14
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button14 => GetButton(13);

        /// <summary>
        /// 手柄按钮15
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button15 => GetButton(14);

        /// <summary>
        /// 手柄按钮16
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button16 => GetButton(15);

        /// <summary>
        /// 手柄按钮17
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button17 => GetButton(16);

        /// <summary>
        /// 手柄按钮18
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button18 => GetButton(17);

        /// <summary>
        /// 手柄按钮19
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button19 => GetButton(18);

        /// <summary>
        /// 手柄按钮20
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button20 => GetButton(19);

        /// <summary>
        /// 手柄按钮21
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button21 => GetButton(20);

        /// <summary>
        /// 手柄按钮22
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button22 => GetButton(21);

        /// <summary>
        /// 手柄按钮23
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button23 => GetButton(22);

        /// <summary>
        /// 手柄按钮24
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button24 => GetButton(23);

        /// <summary>
        /// 手柄按钮25
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button25 => GetButton(24);

        /// <summary>
        /// 手柄按钮26
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button26 => GetButton(25);

        /// <summary>
        /// 手柄按钮27
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button27 => GetButton(26);

        /// <summary>
        /// 手柄按钮28
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button28 => GetButton(27);

        /// <summary>
        /// 手柄按钮29
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button29 => GetButton(28);

        /// <summary>
        /// 手柄按钮30
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button30 => GetButton(29);

        /// <summary>
        /// 手柄按钮31
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button31 => GetButton(30);

        /// <summary>
        /// 手柄按钮32
        /// </summary>
        /// <exception cref="NotSupportedException">无法访问或没有此按钮</exception>
        public virtual BaseButton Button32 => GetButton(31);
        #endregion

        #region 其它

        /// <summary>
        /// 该手柄是否有需要释放的非托管资源或实例
        /// </summary>
        public virtual bool IsNeedDisposed
        {
            get => false;
        }

        /// <summary>
        /// 该操作手柄是否可访问<see cref="Activated"/>参数
        /// </summary>
        public virtual bool CanGetActivated => false;

        /// <summary>
        /// 该手柄是否已激活
        /// </summary>
        /// <returns>true表示可以使用该操作手柄，false表示当前不可访问该操作手柄的任何摇杆和按钮</returns>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public virtual bool Activated
        {
            get => throw new NotSupportedException();
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 该手柄拥有的按钮数量
        /// </summary>
        public virtual int ButtonCount
        {
            get
            {
                int count = 0;

                uint v = (uint)HavingButtons;

                while (v != 0)
                {
                    if ((v & 1) == 1) count++;
                    v >>= 1;
                }

                return count;
            }
        }

        /// <summary>
        /// 该手柄拥有的摇杆数量
        /// </summary>
        public virtual int JoystickCount
        {
            get
            {
                int count = 0;

                byte v = (byte)HavingJoysticks;

                while (v != 0)
                {
                    if ((v & 1) == 1) count++;
                    v >>= 1;
                }

                return count;
            }
        }

        #endregion

        #region 封装

        #endregion

    }

}
