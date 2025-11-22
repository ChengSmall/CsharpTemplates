using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;

namespace Cheng.Systems
{

    /// <summary>
    /// win32上的系统功能
    /// </summary>
    public static unsafe class SystemEnvironmentWindows
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
                return ProcessUser(WindowsBuiltInRole.Administrator);
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
                    flag = new WindowsPrincipal(identity).IsInRole(role);
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
        private static extern uint f_win_getLogicalDrives();

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
                while (p_index < 26)
                {
                    p_index++;
                    if (((p_value >> p_index) & 0b1) == 1)
                    {
                        cp[0] = (char)('A' + p_index);
                        cp[1] = ':';
                        cp[2] = '\\';
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
                var value = f_win_getLogicalDrives();
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
                var value = f_win_getLogicalDrives();
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
            var value = f_win_getLogicalDrives();
            if(value == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return new Enumerator_getLogiacls(value);
        }

        /// <summary>
        /// 返回一个逻辑驱动器卷标的枚举器
        /// </summary>
        /// <returns>逻辑驱动器卷标枚举器</returns>
        public static IEnumerable<char> GetLogicalDrives()
        {
            return new UpdateGetLogicalsEnumerable();
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
            var value = f_win_getLogicalDrives();
            if (value == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return new Enumerator_getLogiaclsStr(value);
        }

        /// <summary>
        /// 返回一个逻辑驱动器卷标名称枚举器
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLogicalDriveNames()
        {
            return new UpdateGetLogicalNamesEnumerable();
        }

        /// <summary>
        /// 获取当前系统逻辑驱动器的数量
        /// </summary>
        /// <returns>当前系统逻辑驱动器的数量，0表示无法获取逻辑驱动器信息</returns>
        public static int GetLogicalDiriveCount()
        {
            var value = f_win_getLogicalDrives();
            if (value == 0) return 0;

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

        /// <summary>
        /// 枚举当前系统的逻辑驱动器标志
        /// </summary>
        /// <param name="action">枚举的逻辑驱动器卷标字符要执行的函数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static void ForeachLogicalDrives(Action<char> action)
        {
            if (action is null) throw new ArgumentNullException();
            var value = f_win_getLogicalDrives();
            if (value == 0)
            {
                return;
            }

            for (int i = 0; i < 26; i++)
            {
                if (((value >> i) & 1) == 1)
                {
                    action.Invoke((char)('A' + i));
                }
            }

        }

        #endregion

        #region TickCount

        [DllImport("kernel32.dll", EntryPoint = "GetTickCount64")]
        private static extern ulong f_win_GetTickCount64();

        /// <summary>
        /// 获取一个64位整数，表示自操作系统启动后经过的毫秒数
        /// </summary>
        public static long TickCount64
        {
            get
            {
                return (long)f_win_GetTickCount64();
            }
        }

        #endregion

        #region 环境变量

        private static unsafe class EnvAPI
        {

            #region winapi

#if DEBUG
            /// <summary>
            /// 检索当前进程的环境变量
            /// </summary>
            /// <remarks>
            /// <para>函数返回指向内存块的指针，该内存块包含调用进程的环境变量 (系统和用户环境变量)</para>
            /// <para>
            /// 每个环境块包含以下格式的环境变量<br/>
            /// <code>
            /// name1=value1\0
            /// name2=value2\0
            /// name3=value3\0
            /// ...
            /// nameN=valueN\0\0
            /// </code>
            /// 每个name=value字符串都使用一个\0作为结尾，而在整个环境快字符串末尾，还有一个\0；<br/>
            /// 环境变量的名称不能包含等号
            /// </para>
            /// </remarks>
            /// <returns>
            /// <para>如果函数成功，则返回值是指向当前进程的环境块的指针；失败则为null</para>
            /// <para>不再使用后，需要调用<see cref="fc_FreeEnvironmentStrings(char*)"/>释放内存</para>
            /// </returns>
#endif
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetEnvironmentStrings")]
            public unsafe static extern char* fc_GetEnvironmentStrings();

#if DEBUG
            /// <summary>
            /// 释放环境字符串块
            /// </summary>
            /// <param name="pStrings">指向环境字符串块的指针</param>
            /// <returns>如果函数成功，则返回值为非零，失败返回0</returns>
#endif
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "FreeEnvironmentStrings")]
            public unsafe static extern uint fc_FreeEnvironmentStrings(char* pStrings);

            #endregion

            #region

            static KeyValuePair<string, string>? f_createEnvPair(CPtr<char> str, int index, int count)
            {
                char* cp = str;

                int eqi;
                int end = index + count;
                for (eqi = index; eqi < end; eqi++)
                {
                    if (cp[eqi] == '=')
                    {
                        goto checkEQ;
                    }
                }
                //未检测到等号
                return null;

                checkEQ:

                string key = new string(cp, index, (eqi - index));

                string val = new string(cp, eqi + 1, ((end - 1) - (eqi)));

                return new KeyValuePair<string, string>(key, val);
            }

            public static IEnumerable<KeyValuePair<string, string>> f_getEnvs(CPtr<char> envstrptr)
            {
                int startIdnex = 0;
                int strIndex = 0;

                int envLen;

                Loop:
                //计数
                envLen = 0;
                while (envstrptr[strIndex] != '\0')
                {
                    strIndex++;
                    envLen++;
                }

                //查找到\0
                //获取环境变量块并转换
                var pair = f_createEnvPair(envstrptr, startIdnex, envLen);
                if (pair.HasValue)
                {
                    if(!string.IsNullOrEmpty(pair.Value.Key)) yield return pair.Value;
                }
                else
                {
                    throw new NotImplementedException();
                }

                //推进索引
                strIndex++;

                if (envstrptr[strIndex] == '\0')
                {
                    //结尾
                    yield break;
                }

                //未达到结尾
                //推进前索引
                startIdnex = strIndex;
                goto Loop;

            }

            #endregion

        }

        /// <summary>
        /// 获取当前进程的所有环境变量
        /// </summary>
        /// <param name="createDictionaryFunc">用于创建字典的委托</param>
        /// <returns>存储当前进程所有环境变量的只读字典</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotImplementedException">无法获取环境变量</exception>
        public static System.Collections.Generic.IReadOnlyDictionary<string, string> GetEnvironmentVariables(Cheng.DataStructure.CreateDictionaryByPairs<string,string> createDictionaryFunc)
        {
            if (createDictionaryFunc is null) throw new ArgumentNullException();

            char* envstrptr = EnvAPI.fc_GetEnvironmentStrings();
            if (envstrptr == null)
            {
                throw new NotImplementedException();
            }

            try
            {
                return createDictionaryFunc.Invoke(EnvAPI.f_getEnvs(new CPtr<char>(envstrptr)), StringComparer.InvariantCultureIgnoreCase);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EnvAPI.fc_FreeEnvironmentStrings(envstrptr);
            }

        }

        /// <summary>
        /// 获取当前进程的所有环境变量
        /// </summary>
        /// <returns>存储当前进程所有环境变量的只读字典</returns>
        /// <exception cref="NotImplementedException">无法获取环境变量</exception>
        public static System.Collections.Generic.IReadOnlyDictionary<string, string> GetEnvironmentVariables()
        {
            char* envstrptr = EnvAPI.fc_GetEnvironmentStrings();
            if (envstrptr == null)
            {
                throw new NotImplementedException();
            }

            try
            {
                var ens = EnvAPI.f_getEnvs(new CPtr<char>(envstrptr));

                Dictionary<string, string> dict = new Dictionary<string, string>(16, StringComparer.InvariantCultureIgnoreCase);
                foreach (var item in ens)
                {
                    dict[item.Key] = item.Value;
                }
                return dict;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                EnvAPI.fc_FreeEnvironmentStrings(envstrptr);
            }
        }

        #endregion

    }

}
#if DEBUG

#endif