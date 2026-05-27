using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Cheng.Memorys;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.Windows.Hooks
{

    /// <summary>
    /// 全局鼠标消息监控挂钩
    /// </summary>
    public unsafe sealed class MouseHook : Hook
    {

        #region 结构

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public void* dwExtraInfo;
        }

        /// <summary>
        /// 鼠标消息类型
        /// </summary>
        [Flags]
        public enum Message : uint
        {
            /// <summary>
            /// 无消息
            /// </summary>
            None = 0,

            /// <summary>
            /// 按下鼠标左键
            /// </summary>
            LeftButtonDown = 0x0201,

            /// <summary>
            /// 释放鼠标左键
            /// </summary>
            LeftButtonUp = 0x0202,

            /// <summary>
            /// 按下鼠标右键
            /// </summary>
            RightButtonDown = 0x0204,

            /// <summary>
            /// 释放鼠标右键
            /// </summary>
            RightButtonUp = 0x0205,

            /// <summary>
            /// 滚动鼠标滚轮
            /// </summary>
            MouseWheel = 0x020A,

            /// <summary>
            /// 移动鼠标
            /// </summary>
            MouseMove = 0x0200,
        }

        /// <summary>
        /// 鼠标消息参数
        /// </summary>
        public readonly struct MouseArgs
        {

            #region 参数和初始化

            internal MouseArgs(void* ms, uint type)
            {
                p_ms = *(MSLLHOOKSTRUCT*)ms;
                this.p_type = type;
            }

            private readonly MSLLHOOKSTRUCT p_ms;

            private readonly uint p_type;

            #endregion

            #region 参数获取

            /// <summary>
            /// 消息类型
            /// </summary>
            public Message MegType
            {
                get => (Message)p_type;
            }

            /// <summary>
            /// 鼠标光标的x和y位置，按DPI屏幕坐标
            /// </summary>
            public PointInt2 Position
            {
                get => new PointInt2(p_ms.pt.x, p_ms.pt.y);
            }

            /// <summary>
            /// 滚轮增量
            /// </summary>
            /// <remarks>
            /// <para>表示滚轮增量，大于0表示向上滚动，小于0向下滚动；滚轮增量一般总是<see cref="WheelDelta"/>的整数倍</para>
            /// <para>当消息<see cref="MegType"/>参数包含<see cref="Message.MouseWheel"/>时，该参数生效，否则是无效参数</para>
            /// </remarks>
            public short MouseWheel
            {
                get
                {
                    return (short)((p_ms.mouseData >> 16) & 0xFFFFU);
                }
            }

            /// <summary>
            /// 一次滚轮滚动的值
            /// </summary>
            public const int WheelDelta = 120;

            /// <summary>
            /// 滚轮增量
            /// </summary>
            /// <value>参数是<see cref="MouseWheel"/>的<see cref="WheelDelta"/>整除数</value>
            public int MouseWheelNormal
            {
                get => (((int)((p_ms.mouseData >> 16) & 0xFFFFU)) / WheelDelta);
            }

            /// <summary>
            /// 鼠标按键序号
            /// </summary>
            /// <remarks>
            /// <para>如果鼠标按下或释放消息，该参数表示触发消息的鼠标按键序号；通常1表示左键，2表示右键</para>
            /// </remarks>
            public short ButtonNumber
            {
                get
                {
                    return (short)((p_ms.mouseData >> 16) & 0xFFFFU);
                }
            }

            /// <summary>
            /// 事件注入标志
            /// </summary>
            public int Flags
            {
                get => (int)p_ms.flags;
            }

            /// <summary>
            /// 此消息的时间戳
            /// </summary>
            public uint Time
            {
                get => (uint)p_ms.time;
            }

            /// <summary>
            /// 判断事件参数内是否包含指定消息
            /// </summary>
            /// <param name="meg">要判断的消息</param>
            /// <returns>包含消息返回true，否则返回false</returns>
            public bool IsMegType(Message meg)
            {
                if (((uint)meg & 0x200) == 0)
                {
                    return false;
                }
                const uint nts = (~0x200U);
                return (p_type & nts) == (((uint)meg) & nts);
            }

            #endregion

        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个全局鼠标消息监控挂钩
        /// </summary>
        public MouseHook() : base(HookID.Mouse_LL)
        {
            p_eventThreadSafe = new object();
            p_event = null;
        }

        #endregion

        #region 参数

        private object p_eventThreadSafe;
        private HookAction<MouseArgs> p_event;

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            var b = base.Disposeing(disposeing);
            if (disposeing)
            {
                p_event = null;
                p_eventThreadSafe = null;
            }
            return b;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 鼠标消息事件
        /// </summary>
        public event HookAction<MouseArgs> MouseHookEvent
        {
            add
            {
                if (IsDispose) return;
                lock (p_eventThreadSafe) p_event += value;
            }
            remove
            {
                if (IsDispose) return;
                lock (p_eventThreadSafe) p_event -= value;
            }
        }

        #region 派生

        protected override void HookCallBack(HookArgs args)
        {
            if (args.code != 0) return;
            p_event?.Invoke(this, new MouseArgs((void*)args.lParam, (uint)args.wParam));
        }

        #endregion

        #endregion

    }

}