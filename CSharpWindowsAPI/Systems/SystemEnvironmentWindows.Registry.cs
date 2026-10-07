using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Security;
using System.IO;

using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;
using Cheng.Windows.Registrys;

namespace Cheng.Systems
{

    unsafe partial class SystemEnvironmentWindows
    {

        /// <summary>
        /// 访问注册表内容
        /// </summary>
        public static partial class Registrys
        {

            #region 色调主题

            /// <summary>
            /// 从注册表读取系统页面主题色调
            /// </summary>
            /// <returns>1表示亮色系，0表示暗色系</returns>
            /// <exception cref="ArgumentException">参数异常</exception>
            /// <exception cref="Win32Exception">win32错误</exception>
            public static int IsSystemInDarkTheme()
            {
                const string subPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string regvname = "SystemUsesLightTheme";

                var reg = RegistryValue.GetRegistryValue(RegHandleKey.CurrentUser, subPath, regvname, RegValueType.DWORD, true);
                return (int)reg.Intger;
            }

            /// <summary>
            /// 从注册表读取应用页面主题色调
            /// </summary>
            /// <returns>1表示亮色，0表示暗色</returns>
            /// <exception cref="ArgumentException">参数异常</exception>
            /// <exception cref="Win32Exception">win32错误</exception>
            public static int IsAppsInDarkTheme()
            {
                const string subPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string regvname = "AppsUseLightTheme";

                var reg = RegistryValue.GetRegistryValue(RegHandleKey.CurrentUser, subPath, regvname, RegValueType.DWORD, true);
                return (int)reg.Intger;
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
                Exception lastex;
                try
                {
                    return IsAppsInDarkTheme();
                }
                catch (Exception ex)
                {
                    lastex = ex;
                }
                try
                {
                    return IsSystemInDarkTheme();
                }
                catch (Exception)
                {
                    if (lastex != null) throw lastex;
                    throw;
                }
            }

            #endregion

            #region 设备ID

            /// <summary>
            /// 获取设备的系统Guid
            /// </summary>
            /// <returns>表示系统Guid的文本</returns>
            /// <exception cref="ArgumentException">注册表参数错误</exception>
            /// <exception cref="Win32Exception">win32错误</exception>
            public static string GetMachineGuidText()
            {
                const RegHandleKey hk = RegHandleKey.LocalMachine;
                const string regPath = @"SOFTWARE\Microsoft\Cryptography";
                RegSam sam = 0;
                if (!Environment.Is64BitProcess)
                {
                    sam = RegSam.WOW64Key64;
                }
                var rv = RegistryValue.GetRegistryValue(hk, regPath, "MachineGuid", RegValueType.String, true, sam);
                return rv.StringValue;
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
