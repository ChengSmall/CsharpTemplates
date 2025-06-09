using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Systems
{

    /// <summary>
    /// win32上的系统功能
    /// </summary>
    public unsafe static class SystemEnvironmentWindows
    {

        #region 权限

        /// <summary>
        /// 判断此进程是否为管理员权限
        /// </summary>
        /// <returns>
        /// 是管理员权限返回true，否则返回false
        /// <para>获取此属性值会在内部申请非托管对象并销毁，因此最好不要频繁调用</para>
        /// </returns>
        public static bool IsAdministrator
        {
            get
            {
                bool flag;
                try
                {
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                    {
                        var ws = new WindowsPrincipal(identity);

                        flag = ws.IsInRole(WindowsBuiltInRole.Administrator);
                    }
                }
                catch (Exception)
                {
                    flag = false;
                }
                return flag;
            }
        }

        /// <summary>
        /// 判断此进程是否为指定的用户权限
        /// </summary>
        /// <param name="role">用户权限</param>
        /// <returns>
        /// 是指定的用户权限返回true，否则返回false
        /// <para>获取此属性值会在内部申请非托管对象并销毁，因此最好不要频繁调用</para>
        /// </returns>
        public static bool ProcessUser(WindowsBuiltInRole role)
        {
            bool flag;
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    // 使用身份对象进行操作
                    var ws = new WindowsPrincipal(identity);

                    flag = ws.IsInRole(role);
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #region 逻辑驱动器

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetLogicalDrives")]
        private static extern uint win_getLogicalDrives();

        private class Enumerator_getLogiaclsStr : IEnumerator<string>
        {

            #region

            public Enumerator_getLogiaclsStr(uint value)
            {
                p_value = value;
                p_index = -1;
                p_cut = default;
            }

            private uint p_value;

            private int p_index;

            private string p_cut;

            #endregion

            #region

            public string Current => p_cut;

            object IEnumerator.Current => p_cut;

            public unsafe bool MoveNext()
            {
                if (p_index > 25)
                {
                    return false;
                }
                char* cp = stackalloc char[3];
                cp[1] = ':';
                cp[2] = '\\';
                while (p_index < 26)
                {
                    p_index++;
                    if (((p_value >> p_index) & 0b1) == 1)
                    {
                        cp[0] = (char)('A' + p_index);
                        p_cut = new string(cp, 0, 3);
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                p_index = -1;
                p_cut = null;
            }

            public void Dispose()
            {
                p_index = 26;
            }

            #endregion

        }

        private class Enumerator_getLogiacls : IEnumerator<char>
        {

            #region

            public Enumerator_getLogiacls(uint value)
            {
                p_value = value;
                p_index = -1;
                p_cut = default;
            }

            private uint p_value;

            private int p_index;

            private char p_cut;

            #endregion

            #region

            public char Current => p_cut;

            object IEnumerator.Current => p_cut;

            public bool MoveNext()
            {
                if(p_index > 25)
                {
                    return false;
                }

                while (p_index < 26)
                {
                    p_index++;

                    if(((p_value >> p_index) & 0b1) == 1)
                    {
                        p_cut = (char)('A' + p_index);
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                p_index = -1;
                p_cut = default;
            }

            public void Dispose()
            {
                p_index = 26;
            }

            #endregion

        }

        /// <summary>
        /// 一个逻辑驱动器卷标枚举器
        /// </summary>
        public sealed class UpdateGetLogicalsEnumerable : IEnumerable<char>
        {
            /// <summary>
            /// 实例化一个逻辑驱动器卷标枚举器
            /// </summary>
            public UpdateGetLogicalsEnumerable()
            {
            }

            /// <summary>
            /// 每次调用该函数都会从系统返回最新的逻辑驱动器信息并枚举可用的驱动器卷标
            /// </summary>
            /// <returns>
            /// <para>一个可循环访问的逻辑驱动器卷标集合，每次访问获取一个大写字母，每个字母对应一个逻辑驱动器卷标；例如返回C表示存在<![CDATA[C:\]]>，返回D表示存在<![CDATA[D:\]]></para>
            /// </returns>
            /// <exception cref="Win32Exception">无法获取逻辑驱动器卷标集合</exception>
            public IEnumerator<char> GetEnumerator()
            {
                var value = win_getLogicalDrives();
                if (value == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return new Enumerator_getLogiacls(value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// 一个逻辑驱动器卷标名称枚举器
        /// </summary>
        public sealed class UpdateGetLogicalNamesEnumerable : IEnumerable<string>
        {
            /// <summary>
            /// 实例化一个逻辑驱动器卷标枚举器
            /// </summary>
            public UpdateGetLogicalNamesEnumerable()
            {
            }

            /// <summary>
            /// 每次调用该函数都会从系统返回最新的逻辑驱动器信息并枚举可用的驱动器卷标名称
            /// </summary>
            /// <returns>
            /// <para>一个可循环访问的逻辑驱动器卷标名称集合，每次访问获取一个逻辑驱动器卷标名称，例如<![CDATA[C:\]]></para>
            /// </returns>
            /// <exception cref="Win32Exception">无法获取逻辑驱动器信息</exception>
            public IEnumerator<string> GetEnumerator()
            {
                var value = win_getLogicalDrives();
                if (value == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return new Enumerator_getLogiaclsStr(value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// 获取一个逻辑驱动器卷标枚举器
        /// </summary>
        /// <returns>
        /// <para>一个可循环访问的逻辑驱动器卷标集合，每次访问获取一个大写字母，每个字母对应一个逻辑驱动器卷标；例如返回C表示存在<![CDATA[C:\]]>，返回D表示存在<![CDATA[D:\]]></para>
        /// <para>当调用此函数获取枚举器时，系统已经将逻辑驱动器信息发送到枚举器中，此时如果操作系统的逻辑驱动器数量发生变化，已经获取的枚举器将不会实时更新信息；想要重新获取系统的逻辑驱动器分卷请重新调用此函数，或使用<see cref="UpdateGetLogicalsEnumerable"/>对象</para>
        /// </returns>
        /// <exception cref="Win32Exception">无法获取逻辑驱动器信息</exception>
        public static IEnumerator<char> EnumableGetLogicalDrives()
        {
            var value = win_getLogicalDrives();
            if(value == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return new Enumerator_getLogiacls(value);
        }

        /// <summary>
        /// 获取一个逻辑驱动器卷标名称枚举器
        /// </summary>
        /// <returns>
        /// <para>一个可循环访问的逻辑驱动器卷标名称集合，每次访问获取一个逻辑驱动器卷标名称，例如<![CDATA[C:\]]></para>
        /// </returns>
        /// <exception cref="Win32Exception">无法获取逻辑驱动器信息</exception>
        public static IEnumerator<string> EnumableGetLogicalDriveNames()
        {
            var value = win_getLogicalDrives();
            if (value == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return new Enumerator_getLogiaclsStr(value);
        }

        /// <summary>
        /// 获取当前系统逻辑驱动器的数量
        /// </summary>
        /// <returns>当前系统逻辑驱动器的数量</returns>
        /// <exception cref="Win32Exception">无法获取逻辑驱动器信息</exception>
        public static int GetLogicalDiriveCount()
        {
            var value = win_getLogicalDrives();
            if (value == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            int count = 0;
            for (int i = 0; i < 26; i++)
            {
                if(((value >> i) & 1) == 1)
                {
                    count++;
                }
            }
            return count;
        }

        #endregion

    }

}
