using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;

namespace Cheng.Registrys
{

    /// <summary>
    /// 注册表相关扩展
    /// </summary>
    public static class RegistryExtend
    {

        /// <summary>
        /// 读取系统页面主题
        /// </summary>
        /// <returns>1表示亮色，0表示暗色</returns>
        public static int IsSystemInDarkTheme()
        {
            //系统主题设置
            object systemValue = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "SystemUsesLightTheme",
                1);
            // 1（亮色）
            if (systemValue is int i32)
            {
                return i32;
            }
            if (systemValue is uint ui32)
            {
                return (int)ui32;
            }
            if (systemValue is short i16)
            {
                return i16;
            }
            if (systemValue is ushort ui16)
            {
                return (int)ui16;
            }
            if (systemValue is short i8)
            {
                return i8;
            }
            if (systemValue is ushort ui8)
            {
                return (int)ui8;
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// 读取应用页面主题
        /// </summary>
        /// <returns>1表示亮色，0表示暗色</returns>
        public static int IsAppsInDarkTheme()
        {
            // 读取应用主题设置
            object appsValue = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme",
                1); // 默认值为1（亮色）
            if (appsValue is int i32)
            {
                return i32;
            }
            if (appsValue is uint ui32)
            {
                return (int)ui32;
            }
            if (appsValue is short i16)
            {
                return i16;
            }
            if (appsValue is ushort ui16)
            {
                return ui16;
            }
            if (appsValue is short i8)
            {
                return i8;
            }
            if (appsValue is ushort ui8)
            {
                return ui8;
            }
            if (appsValue is byte b)
            {
                return b;
            }
            if (appsValue is sbyte sb)
            {
                return sb;
            }
            throw new ArgumentException();
        }


    }

}
