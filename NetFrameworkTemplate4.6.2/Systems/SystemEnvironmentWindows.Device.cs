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

        #region 设备

        /// <summary>
        /// 获取设备的系统Guid
        /// </summary>
        /// <returns>表示系统Guid的文本，null表示找不到或无法获取</returns>
        /// <exception cref="UnauthorizedAccessException">用户没有注册表权限</exception>
        /// <exception cref="SecurityException">用户没有执行此操作所需的权限</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static string GetMachineGuidText()
        {
            const string regPath = @"SOFTWARE\Microsoft\Cryptography";

            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = hklm.OpenSubKey(regPath))
                {
                    if (key is object)
                    {
                        var value = key.GetValue("MachineGuid");
                        if (value != null) return value.ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取设备的系统<see cref="Guid"/>
        /// </summary>
        /// <returns>设备的<see cref="Guid"/></returns>
        /// <exception cref="UnauthorizedAccessException">用户没有注册表权限</exception>
        /// <exception cref="SecurityException">用户没有执行此操作所需的权限</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="FormatException">无法转化到<see cref="Guid"/>对象</exception>
        public static Guid GetMachineGuid()
        {
            return Guid.Parse(GetMachineGuidText());
        }

        #endregion

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