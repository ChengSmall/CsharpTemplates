using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

using Cheng.Memorys;

namespace Cheng.IO
{

    /// <summary>
    /// 一个安全封装的句柄或指针
    /// </summary>
    public unsafe abstract class SafeHandlePointer : SafreleaseUnmanagedResources
    {

        #region 构造

        /// <summary>
        /// 实例化句柄
        /// </summary>
        protected SafeHandlePointer()
        {
            handlePtr = default;
        }

        /// <summary>
        /// 使用指针初始化句柄
        /// </summary>
        /// <param name="ptr"></param>
        protected SafeHandlePointer(IntPtr ptr)
        {
            handlePtr = ptr;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 封装的句柄指针
        /// </summary>
        protected IntPtr handlePtr;

        #endregion

        #region 功能

        /// <summary>
        /// 调用该方法清理和释放句柄
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// 判断当前句柄是否已被关闭或释放
        /// </summary>
        public bool IsClosed
        {
            get => this.IsDispose;
        }

        /// <summary>
        /// 获取该安全句柄内的值
        /// </summary>
        /// <returns>
        /// 一个封装的内部值，该值获取后有可能失效，判断<see cref="IsClosed"/>参数以获得句柄是否可用
        /// </returns>
        /// <exception cref="ObjectDisposedException">句柄已关闭或释放</exception>
        public IntPtr HandlePointer
        {
            get
            {
                ThrowObjectDisposeException();
                return handlePtr;
            }
        }

        /// <summary>
        /// 在派生类重写需要调用基实现
        /// </summary>
        protected override void UnmanagedReleasources()
        {
            if (DisposeHandle(handlePtr))
            {
                handlePtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 重写该函数用于释放指定句柄
        /// </summary>
        /// <param name="handle">要释放的句柄</param>
        /// <returns>是否成功释放</returns>
        protected abstract bool DisposeHandle(IntPtr handle);

        #endregion

    }

}
