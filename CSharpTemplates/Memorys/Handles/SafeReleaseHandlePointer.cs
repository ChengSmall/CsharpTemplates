

using System;

namespace Cheng.IO
{

    /// <summary>
    /// 使用析构函数安全释放的句柄或指针
    /// </summary>
    public abstract class SafeReleaseHandlePointer : SafeHandlePointer
    {

        /// <summary>
        /// 使用对象终结器保证在托管实例释放前安全释放非托管资源
        /// </summary>
        ~SafeReleaseHandlePointer()
        {
            Dispose(false);
        }


        protected SafeReleaseHandlePointer() : base()
        {
        }

        /// <summary>
        /// 初始化句柄
        /// </summary>
        /// <param name="handle"></param>
        protected SafeReleaseHandlePointer(IntPtr handle) : base(handle)
        {
        }
    }

}
