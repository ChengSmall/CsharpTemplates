using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using Cheng.Memorys;

namespace Cheng.Windows.Hooks
{

    /// <summary>
    /// 挂钩事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hook">引发事件的实例</param>
    /// <param name="arg">事件参数</param>
    public delegate void HookAction<T>(Hook hook, T arg);

    /// <summary>
    /// 挂钩事件委托
    /// </summary>
    /// <param name="hook">引发事件的实例</param>
    public delegate void HookAction(Hook hook);

    /// <summary>
    /// 挂钩链回调参数
    /// </summary>
    public struct HookArgs
    {
        public HookArgs(int code, IntPtr wParam, IntPtr lParam)
        {
            this.code = code;
            this.wParam = wParam;
            this.lParam = lParam;
        }

        public readonly int code;

        public readonly IntPtr wParam;

        public readonly IntPtr lParam;
    }

    /// <summary>
    /// 提供使用挂钩对应用程序截获消息、鼠标操作和击键等事件进行处理的基类
    /// </summary>
    public abstract unsafe class Hook : ReleaseDestructor
    {

        #region 释放

        /// <summary>
        /// 在派生类重写此方法，需调用基实现
        /// </summary>
        protected override void UnmanagedReleasources()
        {
            if (p_hookID != null)
            {
                if (WinHooks.UnhookWindowsHookEx(p_hookID)) p_hookID = null;
            }

            p_callback = null;
        }

        /// <summary>
        /// 调用该方法以释放挂钩链
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个挂钩并关联全部线程
        /// </summary>
        /// <param name="id">挂钩过程类型id</param>
        /// <exception cref="Win32Exception">引发win32错误</exception>
        public Hook(HookID id) : this(id, 0)
        {
        }

        /// <summary>
        /// 实例化一个挂钩
        /// </summary>
        /// <param name="id">挂钩过程类型id</param>
        /// <param name="threadID">与之关联的挂钩过程的所在线程ID</param>
        /// <exception cref="Win32Exception">引发win32错误</exception>
        public Hook(HookID id, int threadID)
        {
            p_callback = f_callBack;
            p_hookID = WinHooks.SetWindowsHookEx(id, p_callback, null, (uint)threadID);
            if (p_hookID == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            //p_threadSafe = new object();
            p_active = true;
        }

        /// <summary>
        /// 实例化一个挂钩
        /// </summary>
        /// <param name="id">挂钩过程类型id</param>
        /// <param name="threadID">与之关联的挂钩过程的所在线程ID</param>
        /// <param name="handleMod">DLL的句柄，包含事件委托指向的挂钩过程</param>
        /// <exception cref="Win32Exception">引发win32错误</exception>
        public Hook(HookID id, int threadID, IntPtr handleMod)
        {
            p_callback = f_callBack;
            p_hookID = WinHooks.SetWindowsHookEx(id, p_callback, handleMod.ToPointer(), (uint)threadID);
            if (p_hookID == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            //p_threadSafe = new object();
            p_active = true;
        }

        /// <summary>
        /// 空构造
        /// </summary>
        protected Hook()
        {
            p_callback = f_callBack;
        }

        /// <summary>
        /// 使用参数初始化挂钩
        /// </summary>
        /// <param name="id">钩子类型ID</param>
        /// <param name="threadID">与之关联的挂钩过程的所在线程ID</param>
        /// <param name="handleMod">DLL的句柄，包含事件委托指向的挂钩过程，可为null</param>
        /// <exception cref="Win32Exception">引发win32错误</exception>
        protected void initCtor(HookID id, int threadID, IntPtr handleMod)
        {
            p_callback = f_callBack;
            p_hookID = WinHooks.SetWindowsHookEx(id, p_callback, handleMod.ToPointer(), (uint)threadID);
            if (p_hookID == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            p_active = true;
        }

        #endregion

        #region 参数

        protected ProcCallBack p_callback;

        /// <summary>
        /// 挂钩句柄
        /// </summary>
        protected void* p_hookID;

        /// <summary>
        /// 开启或关闭挂钩事件触发后在该对象的回调
        /// </summary>
        protected bool p_active;

        #endregion

        #region 封装

        private void* f_callBack(int nCode, void* wParam, void* lParam)
        {
            //IntPtr w = new IntPtr(wParam), l = new IntPtr(lParam);
            if(p_active) HookCallBack(new HookArgs(nCode, new IntPtr(wParam), new IntPtr(lParam)));
            return WinHooks.CallNextHookEx(p_hookID, nCode, wParam, lParam);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 引发挂钩事件的函数，在派生类实现
        /// </summary>
        /// <remarks>在派生类重写以重新实现挂钩事件响应函数</remarks>
        /// <param name="args">call back回调函数中的参数</param>
        protected abstract void HookCallBack(HookArgs args);

        #region 静态函数

        #endregion

        #endregion

    }

}
