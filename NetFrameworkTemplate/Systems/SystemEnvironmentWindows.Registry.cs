using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Security;
using System.IO;
using Microsoft.Win32;

using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;

using WinReg = Microsoft.Win32.Registry;

namespace Cheng.Systems
{

    unsafe partial class SystemEnvironmentWindows
    {

        /// <summary>
        /// 注册表读写封装
        /// </summary>
        public static partial class Registrys
        {

            #region 色调主题

            /// <summary>
            /// 从注册表读取系统页面主题色调
            /// </summary>
            /// <returns>1表示亮色系，0表示暗色系，-1表示找不到注册表参数</returns>
            /// <exception cref="ArgumentException">参数异常</exception>
            /// <exception cref="SecurityException">权限异常</exception>
            /// <exception cref="IOException">注册表错误</exception>
            public static int IsSystemInDarkTheme()
            {
                //系统主题设置
                object value = WinReg.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "SystemUsesLightTheme",
                    1);
                if (value == null) return -1;
                // 1（亮色）
                if (value is int i32)
                {
                    return i32;
                }
                if (value is uint ui32)
                {
                    return (int)ui32;
                }
                if (value is long i64)
                {
                    return (int)i64;
                }
                if (value is ulong ui64)
                {
                    return (int)ui64;
                }
                return -1;
            }

            /// <summary>
            /// 从注册表读取应用页面主题色调
            /// </summary>
            /// <returns>1表示亮色，0表示暗色；-1表示找不到注册表参数</returns>
            /// <exception cref="ArgumentException">参数异常</exception>
            /// <exception cref="SecurityException">权限异常</exception>
            /// <exception cref="IOException">注册表错误</exception>
            public static int IsAppsInDarkTheme()
            {
                // 读取应用主题设置
                object value = WinReg.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme",
                    ((int)1)); // 默认值为1（亮色）
                if (value == null) return -1;
                // 1（亮色）
                if (value is int i32)
                {
                    return i32;
                }
                if (value is uint ui32)
                {
                    return (int)ui32;
                }
                if (value is long i64)
                {
                    return (int)i64;
                }
                if (value is ulong ui64)
                {
                    return (int)ui64;
                }
                throw new ArgumentException();
            }

            /// <summary>
            /// 从注册表读取页面主题色调
            /// </summary>
            /// <remarks>
            /// <para>按顺序读取主题色调，如果不存在用户主题色调参数，则从系统参数读取</para>
            /// </remarks>
            /// <returns>1表示亮色，0表示暗色；-1表示找不到注册表参数</returns>
            /// <exception cref="ArgumentException">参数异常</exception>
            /// <exception cref="SecurityException">权限异常</exception>
            /// <exception cref="IOException">注册表错误</exception>
            public static int GetInDarkTheme()
            {
                // 读取应用主题设置
                object value = WinReg.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme",
                    ((int)1));
                if (value == null)
                {
                    value = WinReg.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "SystemUsesLightTheme",
                    ((int)1));
                }
                if (value == null) return -1;
                // 1（亮色）
                if (value is int i32)
                {
                    return i32;
                }
                if (value is uint ui32)
                {
                    return (int)ui32;
                }
                if (value is long i64)
                {
                    return (int)i64;
                }
                if (value is ulong ui64)
                {
                    return (int)ui64;
                }
                throw new ArgumentException();

            }

            #endregion

            #region 设备ID

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

        }

    }

}
