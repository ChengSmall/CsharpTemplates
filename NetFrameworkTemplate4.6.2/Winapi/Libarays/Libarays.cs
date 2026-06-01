using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.DesignerServices;
using System.IO;
using System.ComponentModel;
using System.Text;

using Cheng.Memorys;

namespace Cheng.Windows
{

    /// <summary>
    /// Windows动态链接的代码模块
    /// </summary>
    public unsafe sealed class WinLibaray : ReleaseDestructor
    {

        #region api

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode)]
        private static extern void* winapi_LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        private static extern void* winapi_GetProcAddress(void* hModule, [MarshalAs(UnmanagedType.AnsiBStr)] string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool winapi_FreeLibrary(void* hModule);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress")]
        private static extern void* winapi_GetProcAddress_ptr(void* hModule, void* lpProcName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)]
        private static extern void winapi_GetModuleHandle(char* dllName);

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
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数是空字符串</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static WinLibaray LoadLibaray(string dllFileName)
        {
            return new WinLibaray(dllFileName);
        }

        /// <summary>
        /// 加载指定的模块到调用进程的地址空间中，并返回代码库对象
        /// </summary>
        /// <param name="dllFileName">
        /// <para>dll代码模块的名称</para>
        /// <para>
        /// 指定的名称是模块的文件名，与库模块本身中存储的名称无关<br/>
        /// 如果字符串指定完整路径，则仅搜索模块的该路径；如果字符串指定相对路径或没有路径的模块名称，则使用标准搜索策略查找模块<br/>
        /// 如果字符串指定了没有路径的模块名称，并且省略文件扩展名，该函数会将默认库扩展名“.DLL”追加到模块名称；若要防止函数将“.DLL”追加到模块名称，请在模块名称字符串中包含尾随点字符'.'
        /// </para>
        /// </param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数是空字符串</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public WinLibaray(string dllFileName)
        {
            if (dllFileName is null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(dllFileName)) throw new ArgumentException();

            dllFileName = dllFileName.Replace('/', '\\');
            var ptr = winapi_LoadLibrary(dllFileName);
            if (ptr == null)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            p_hmod = ptr;
            p_encoding = Encoding.ASCII;
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            //p_hmod?.Dispose();
            if(p_hmod != null)
            {
                if (winapi_FreeLibrary(p_hmod))
                {
                    p_hmod = null;
                }
            }
            p_encoding = null;
            return false;
        }

        #endregion

        #region 参数

        private void* p_hmod;
        private Encoding p_encoding;

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
            return new IntPtr(p_hmod);
        }

        /// <summary>
        /// 获取模块基址
        /// </summary>
        /// <returns>模块基址</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public CPtr GetModuleBaseCPtr()
        {
            ThrowObjectDisposeException();
            return new CPtr(p_hmod);
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

            var asciCount = p_encoding.GetByteCount(procName);
            if(asciCount > 4096)
            {
                throw new ArgumentException();
            }

            byte* bptr = stackalloc byte[asciCount + 1];
            fixed (char* nameCPtr = procName)
            {
                var wrCC = p_encoding.GetBytes(nameCPtr, procName.Length, bptr, asciCount);
                if(wrCC < asciCount)
                {
                    throw new ArgumentException();
                }
            }
            bptr[asciCount] = 0;

            void* ptr = winapi_GetProcAddress_ptr(p_hmod, bptr);
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