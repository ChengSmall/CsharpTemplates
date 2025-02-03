using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Memorys;

//using HookID = Cheng.Windows.Hooks.HookID;

namespace Cheng.Windows.Hooks
{

    /// <summary>
    /// 键码消息捕获挂钩
    /// </summary>
    public sealed class KeyHook : Hook
    {

        #region 结构

        /// <summary>
        /// 键码事件参数
        /// </summary>
        public struct KeyHookArgs : IEquatable<KeyHookArgs>
        {
            /// <summary>
            /// 初始化键码事件参数
            /// </summary>
            /// <param name="key">键码</param>
            /// <param name="state">按下true或松开false</param>
            public KeyHookArgs(Keys key, bool state)
            {
                this.key = (int)key;
                this.state = state;
            }

            /// <summary>
            /// 初始化键码事件参数
            /// </summary>
            /// <param name="key">键码</param>
            /// <param name="state">按下true或松开false</param>
            public KeyHookArgs(int key, bool state)
            {
                this.key = key;
                this.state = state;
            }

            /// <summary>
            /// 虚拟键码
            /// </summary>
            public readonly int key;

            /// <summary>
            /// 键码状态，true表示按下，false表示松开
            /// </summary>
            public readonly bool state;

            /// <summary>
            /// 获取将<see cref="KeyHookArgs.key"/>转化为<see cref="System.Windows.Forms.Keys"/>的参数
            /// </summary>
            public Keys Keys
            {
                get => (Keys)key;
            }

            public bool Equals(KeyHookArgs other)
            {
                return key == other.key && state == other.state;
            }

            public override bool Equals(object obj)
            {
                if (obj is KeyHookArgs k) return this == k;
                return false;
            }

            public override int GetHashCode()
            {
                return key ^ (int)((uint)state.GetHashCode() << 31);
            }

            public static bool operator ==(KeyHookArgs k1, KeyHookArgs k2)
            {
                return k1.key == k2.key && k1.state == k2.state;
            }

            public static bool operator !=(KeyHookArgs k1, KeyHookArgs k2)
            {
                return k1.key != k2.key || k1.state != k2.state;
            }

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
        /// 实例化一个键码消息捕获挂钩
        /// </summary>
        public KeyHook() : base(HookID.KeyBoard_LL)
        {
            p_eventThreadSafe = new object();
        }

        /// <summary>
        /// 实例化一个键码消息捕获挂钩
        /// </summary>
        /// <param name="threadID">挂钩过程所在线程id</param>
        public KeyHook(int threadID) : base(HookID.KeyBoard_LL, threadID)
        {
            p_eventThreadSafe = new object();
        }

        /// <summary>
        /// 实例化一个键码消息捕获挂钩
        /// </summary>
        /// <param name="threadID">挂钩过程所在线程id</param>
        /// <param name="handleMod">DLL的句柄，包含事件委托指向的挂钩过程</param>
        public KeyHook(int threadID, IntPtr handleMod) : base(HookID.KeyBoard_LL, threadID, handleMod)
        {
            p_eventThreadSafe = new object();
        }

        #endregion

        #region 参数
        private HookAction<KeyHookArgs> p_keyEvent;
        private object p_eventThreadSafe;
        #endregion

        #region 派生

        protected override void HookCallBack(HookArgs args)
        {
            if (args.code >= 0)
            {
                KeyHookArgs kh = new KeyHookArgs(args.lParam.PtrDef<Keys>(), args.wParam == new IntPtr(0x100));
                p_keyEvent?.Invoke(this, kh);
            }
        }

        /// <summary>
        /// 键盘挂钩引发的事件
        /// </summary>
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
