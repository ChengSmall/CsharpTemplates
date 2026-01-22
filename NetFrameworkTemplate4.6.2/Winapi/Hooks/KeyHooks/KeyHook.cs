using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Memorys;

//using HookID = Cheng.Windows.Hooks.HookID;

namespace Cheng.Windows.Hooks
{

    /// <summary>
    /// 键盘消息捕获挂钩
    /// </summary>
    public unsafe sealed class KeyHook : Hook
    {

        #region 结构

        /// <summary>
        /// 键盘消息标识符
        /// </summary>
        [Flags]
        public enum KeyState : byte
        {
            /// <summary>
            /// 按下按键
            /// </summary>
            KeyDown = 0,

            /// <summary>
            /// 释放按键
            /// </summary>
            KeyUp = 1,

            /// <summary>
            /// 按下的是系统按键
            /// </summary>
            IsSystemKey = 0b10
        }

        /// <summary>
        /// 按键事件参数
        /// </summary>
        public unsafe readonly struct KeyHookArgs : IEquatable<KeyHookArgs>
        {

            internal KeyHookArgs(ref KBDLLHOOKSTRUCT kb, KeyState state)
            {
                p_kb = kb;
                this.keyState = state;
            }

            private readonly KBDLLHOOKSTRUCT p_kb;

            /// <summary>
            /// 键盘消息标识
            /// </summary>
            public readonly KeyState keyState;

            /// <summary>
            /// 虚拟键码
            /// </summary>
            public int VkCode
            {
                get
                {
                    return p_kb.vkCode;
                }
            }

            /// <summary>
            /// 硬件扫描码
            /// </summary>
            public int ScanCode
            {
                get => p_kb.scanCode;
            }

            /// <summary>
            /// 扩展键、事件注入、上下文代码和转换状态标志
            /// </summary>
            public uint Flags
            {
                get => p_kb.flags;
            }

            /// <summary>
            /// 此消息的时间戳
            /// </summary>
            public uint Time
            {
                get => p_kb.time;
            }

            /// <summary>
            /// 将<see cref="KeyHookArgs.VkCode"/>转化为<see cref="System.Windows.Forms.Keys"/>并返回
            /// </summary>
            public Keys Keys
            {
                get => (Keys)VkCode;
            }

            /// <summary>
            /// 按键是按下还是释放
            /// </summary>
            /// <remarks>该参数仅判断按键是否为按下还是释放，不判断按下的按键是否为系统按键</remarks>
            /// <returns>true表示按下按键，false表示释放按键</returns>
            public bool State
            {
                get => ((byte)keyState & 0b1) == 0;
            }

            /// <summary>
            /// 按键是否为系统按键
            /// </summary>
            public bool IsSystemKey
            {
                get => ((byte)keyState & 0b10) == 0b10;
            }

            /// <summary>
            /// 与消息关联的其他信息和参数
            /// </summary>
            public IntPtr DwExtraInfo
            {
                get => new IntPtr(p_kb.dwExtraInfo);
            }

            public static bool operator ==(KeyHookArgs k1, KeyHookArgs k2)
            {
                return k1.p_kb.vkCode == k2.p_kb.vkCode && k1.keyState == k2.keyState && k1.p_kb.flags == k2.p_kb.flags && k1.p_kb.time == k2.p_kb.time && k1.p_kb.dwExtraInfo == k2.p_kb.dwExtraInfo;
            }

            public static bool operator !=(KeyHookArgs k1, KeyHookArgs k2)
            {
                return k1.p_kb.vkCode != k2.p_kb.vkCode || k1.keyState != k2.keyState || k1.p_kb.flags != k2.p_kb.flags || k1.p_kb.time != k2.p_kb.time || k1.p_kb.dwExtraInfo != k2.p_kb.dwExtraInfo;
            }

            public bool Equals(KeyHookArgs other)
            {
                return this == other;
            }

            public override bool Equals(object obj)
            {
                if (obj is KeyHookArgs k) return this == k;
                return false;
            }

            public override int GetHashCode()
            {
                
                return (int)(((uint)p_kb.vkCode | ((uint)p_kb.scanCode << 15)) ^ ((p_kb.flags ^ p_kb.time) | (((uint)keyState) << 30)));

            }

        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public uint flags;
            public uint time;
            public void* dwExtraInfo;
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_keyEvent = null;
            }

            return base.Disposeing(disposeing);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个全局键盘消息捕获挂钩
        /// </summary>
        public KeyHook() : base(HookID.KeyBoard_LL)
        {
            p_eventThreadSafe = new object();
        }

        /// <summary>
        /// 实例化一个全局键盘消息捕获挂钩
        /// </summary>
        /// <param name="handleMod">DLL的句柄，包含事件委托指向的挂钩过程</param>
        public KeyHook(IntPtr handleMod) : base(HookID.KeyBoard_LL, 0, handleMod)
        {
            p_eventThreadSafe = new object();
        }

        #endregion

        #region 参数

        private HookAction<KeyHookArgs> p_keyEvent;
        private object p_eventThreadSafe;

        #endregion

        #region 功能

        protected override void HookCallBack(HookArgs args)
        {
            if (args.code == 0)
            {
                KeyState ks;

                switch ((int)args.wParam)
                {
                    case 0x100:
                        ks = KeyState.KeyDown;
                        break;
                    case 0x101:
                        ks = KeyState.KeyUp;
                        break;
                    case 0x104:
                        ks = KeyState.KeyDown | KeyState.IsSystemKey;
                        break;
                    default:
                        ks = KeyState.KeyUp | KeyState.IsSystemKey;
                        break;
                }

                //KeyHookArgs kh = new KeyHookArgs(ref *(KBDLLHOOKSTRUCT*)args.lParam, ks);
                p_keyEvent?.Invoke(this, new KeyHookArgs(ref *(KBDLLHOOKSTRUCT*)args.lParam, ks));
            }
        }

        /// <summary>
        /// 键盘挂钩引发的事件
        /// </summary>
        /// <remarks>
        /// <para>将键盘事件挂钩安装到挂钩链，监控windows键盘事件</para>
        /// <para>每当按下键盘按键时，无论进程是否处于前台还是后台，窗口是否拥有焦点，都会引发事件</para>
        /// </remarks>
        public event HookAction<KeyHookArgs> KeyHookEvent
        {
            add
            {
                ThrowObjectDisposeException();
                lock (p_eventThreadSafe)
                {
                    p_keyEvent += value;
                }
            }
            remove
            {
                ThrowObjectDisposeException();
                lock (p_eventThreadSafe)
                {
                    p_keyEvent -= value;
                }
            }
        }

        #endregion

    }

}
