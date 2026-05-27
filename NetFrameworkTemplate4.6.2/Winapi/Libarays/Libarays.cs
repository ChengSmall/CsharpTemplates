using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.DesignerServices;
using System.IO;
using System.ComponentModel;
using Cheng.Memorys;

namespace Cheng.Windows
{

    /// <summary>
    /// Windows动态链接的代码模块
    /// </summary>
    public unsafe sealed class WinLibaray : SafreleaseUnmanagedResources
    {

        #region api

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "LoadLibrary")]
        private static extern void* winapi_LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        private static extern void* winapi_GetProcAddress(void* hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool winapi_FreeLibrary(void* hModule);

        private sealed class SafeDLLHandle : System.Runtime.InteropServices.SafeHandle
        {
            public SafeDLLHandle(void* hmod) : base(IntPtr.Zero, true)
            {
                this.SetHandle(new IntPtr(hmod));
            }

            public override bool IsInvalid => DangerousGetHandle() == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                var p = DangerousGetHandle();
                return winapi_FreeLibrary(p.ToPointer());
            }
        }

        #endregion

        #region 初始化

        private WinLibaray()
        {
        }

        /// <summary>
        /// 加载指定的模块到调用进程的地址空间中，并返回代码库对象
        /// </summary>
        /// <param name="dllFileName">
        /// <para>模块的名称</para>
        /// <para>
        /// 指定的名称是模块的文件名，与库模块本身中存储的名称无关<br/>
        /// 如果字符串指定完整路径，则仅搜索模块的该路径；如果字符串指定相对路径或没有路径的模块名称，则函数使用标准搜索策略查找模块<br/>
        /// 如果字符串指定了没有路径的模块名称，并且省略文件扩展名，该函数会将默认库扩展名“.DLL”追加到模块名称；若要防止函数将“.DLL”追加到模块名称，请在模块名称字符串中包含尾随点字符'.'
        /// </para>
        /// </param>
        /// <returns>代码库对象，不再使用后请手动释放对象</returns>
        public static WinLibaray LoadLibaray(string dllFileName)
        {
            if (dllFileName is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dllFileName)) throw new ArgumentException();

            dllFileName = dllFileName.Replace('/', '\\');
            var ptr = winapi_LoadLibrary(dllFileName);
            if(ptr == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            WinLibaray lib = new WinLibaray();
            lib.p_hmod = new SafeDLLHandle(ptr);
            return lib;
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_hmod.Dispose();
            }
            p_hmod = null;
            return false;
        }

        #endregion

        #region 参数

        private SafeDLLHandle p_hmod;

        #endregion

        #region 功能

        /// <summary>
        /// 获取模块基址
        /// </summary>
        /// <returns>模块基址</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public IntPtr GetModuleBaseHandle()
        {
            ThrowObjectDisposeException();
            return p_hmod.DangerousGetHandle();
        }

        /// <summary>
        /// 获取模块基址
        /// </summary>
        /// <returns>模块基址</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public CPtr GetModuleBaseCPtr()
        {
            ThrowObjectDisposeException();
            return p_hmod.DangerousGetHandle();
        }

        /// <summary>
        /// 检索导出函数或变量的地址
        /// </summary>
        /// <param name="procName">函数或变量名称</param>
        /// <returns>导出的函数或变量所在的地址</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentException">参数是null或空字符串</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public IntPtr GetProcAddress(string procName)
        {
            ThrowObjectDisposeException(nameof(WinLibaray));
            if (procName is null) throw new ArgumentNullException();
            if (procName.Length == 0) throw new ArgumentException();

            var ptr = winapi_GetProcAddress(p_hmod.DangerousGetHandle().ToPointer(), procName);
            if(ptr == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return new IntPtr(ptr);
        }

        /// <summary>
        /// 检索导出函数并转换成对应委托对象
        /// </summary>
        /// <typeparam name="TDel"></typeparam>
        /// <param name="funcName">函数的名称</param>
        /// <returns>导出的函数对应的委托对象</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentException">参数是null或空字符串</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public TDel GetDelegateForProcAddress<TDel>(string funcName) where TDel : System.Delegate
        {
            var ptr = GetProcAddress(funcName);
            return Marshal.GetDelegateForFunctionPointer<TDel>(ptr);
        }

        #endregion

    }

}