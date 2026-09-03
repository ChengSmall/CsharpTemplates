using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;
using Cheng.DataStructure.Windows;

namespace Cheng.Systems
{

    unsafe partial class SystemEnvironmentWindows
    {

        #region 内存

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GlobalMemoryStatusEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool win32_GlobalMemoryStatusEx(void* lpBuffer);

        /// <summary>
        /// 检索有关系统当前物理内存和虚拟内存信息
        /// </summary>
        /// <param name="ms">获取的内存信息对象</param>
        /// <returns>成功获取返回true，获取失败返回false；失败后可用<see cref="Marshal.GetLastWin32Error"/>获取错误信息</returns>
        public static bool TryGetGlobalMemoryStatus(out MemoryStatus ms)
        {
            ms.dwLength = (uint)sizeof(MemoryStatus);
            fixed (MemoryStatus* p = &ms)
            {
                return win32_GlobalMemoryStatusEx(p);
            }
        }

        /// <summary>
        /// 检索有关系统当前物理内存和虚拟内存信息
        /// </summary>
        /// <returns>内存信息对象</returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static MemoryStatus GetGlobalMemoryStatus()
        {
            MemoryStatus ms;
            ms.dwLength = (uint)sizeof(MemoryStatus);
            if (win32_GlobalMemoryStatusEx(&ms)) return ms;
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        #endregion

    }

}