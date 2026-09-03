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
using Cheng.DataStructure.Windows;

using EnvDict = System.Collections.Generic.IReadOnlyDictionary<string, string>;
using EnvDictCrFunc = Cheng.DataStructure.CreateDictionaryByPairs<string, string>;

namespace Cheng.Systems
{

    /// <summary>
    /// win32上的系统功能
    /// </summary>
    public static unsafe partial class SystemEnvironmentWindows
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

        #region 时间

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

        /// <summary>
        /// 获取一个64位整数，表示自操作系统启动后经过的毫秒数
        /// </summary>
        public static ulong TickCountU64
        {
            get => f_win_GetTickCount64();
        }

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetSystemTimeAdjustment")]
        private static extern uint f_win32api_GetSystemTimeAdjustment(
        uint* lpTimeAdjustment, uint* lpTimeIncrement,
        uint* lpTimeAdjustmentDisabled);

        /// <summary>
        /// 确定系统是否对其时间时钟应用定期时间调整，并获取任何此类调整的值和周期
        /// </summary>
        /// <param name="timeAdjustment">函数将该变量设置为添加到时间时钟的<paramref name="timeIncrement"/> 100 纳秒单位数，该时间段实际通过系统计数；仅当函数返回false时，该参数才有意义</param>
        /// <param name="timeIncrement">函数将该变量设置为间隔（以 100 纳秒为单位），系统将在其中将 <paramref name="timeAdjustment"/>添加到时间时钟；仅当函数返回false时，该参数才有意义</param>
        /// <returns>
        /// <para>
        /// 值为 true 表示禁用定期时间调整，并且系统时间时钟按正常速率前进<br/>
        /// 在此模式下，系统可以使用自己的内部时间同步机制调整一天中的时间；这些内部时间同步机制可能导致在系统操作的正常过程中更改时间时钟，这可能包括系统认为必要的明显时间跳跃
        /// </para>
        /// <para>
        /// 值为 false 表示正在使用定期时间调整来调整一天中的时间时钟<br/>
        /// 对于实际经过的每个<paramref name="timeIncrement"/>时间段，<paramref name="timeAdjustment"/>将添加到一天中的时间<br/>
        /// 如果 <paramref name="timeAdjustment"/> 值小于 <paramref name="timeIncrement"/>，则系统时间时钟将以比平常慢的速度前进； 如果 <paramref name="timeAdjustment"/> 值大于 <paramref name="timeIncrement"/>，则一天中的时钟将以比平常快的速度前进。 如果 <paramref name="timeAdjustment"/> 等于 <paramref name="timeIncrement"/>，则时间时钟将按其正常速度前进
        /// </para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static bool GetSystemTimeAdjustment(out uint timeAdjustment, out uint timeIncrement)
        {
            if(!TryGetSystemTimeAdjustment(out timeAdjustment, out timeIncrement, out bool reb))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return reb;
        }

        /// <summary>
        /// 确定系统是否对其时间时钟应用定期时间调整，并获取任何此类调整的值和周期
        /// </summary>
        /// <param name="timeAdjustment">
        /// 函数将该变量设置为添加到时间时钟的 <paramref name="timeIncrement"/> 100 纳秒单位数，该时间段实际通过系统计数；仅当函数返回false时，该参数才有意义
        /// </param>
        /// <param name="timeIncrement">
        /// 函数将该变量设置为间隔（以 100 纳秒为单位），系统将在其中将 <paramref name="timeAdjustment"/> 添加到时间时钟；仅当函数返回false时，该参数才有意义
        /// </param>
        /// <param name="timeAdjustmentDisabled">
        /// <para>
        /// 值为 true 表示禁用定期时间调整，并且系统时间时钟按正常速率前进<br/>
        /// 在此模式下，系统可以使用自己的内部时间同步机制调整一天中的时间；这些内部时间同步机制可能导致在系统操作的正常过程中更改时间时钟，这可能包括系统认为必要的明显时间跳跃
        /// </para>
        /// <para>
        /// 值为 false 表示正在使用定期时间调整来调整一天中的时间时钟<br/>
        /// 对于实际经过的每个<paramref name="timeIncrement"/>时间段，<paramref name="timeAdjustment"/>将添加到一天中的时间<br/>
        /// 如果 <paramref name="timeAdjustment"/> 值小于 <paramref name="timeIncrement"/>，则系统时间时钟将以比平常慢的速度前进； 如果 <paramref name="timeAdjustment"/> 值大于 <paramref name="timeIncrement"/>，则一天中的时钟将以比平常快的速度前进。 如果 <paramref name="timeAdjustment"/> 等于 <paramref name="timeIncrement"/>，则时间时钟将按其正常速度前进
        /// </para>
        /// </param>
        /// <returns>返回true表示函数成功，false表示失败；如果失败请用<see cref="Marshal.GetLastWin32Error"/>获取错误码</returns>
        public static bool TryGetSystemTimeAdjustment(out uint timeAdjustment, out uint timeIncrement, out bool timeAdjustmentDisabled)
        {
            uint reb;
            uint re;
            timeAdjustment = 0; timeIncrement = 0;
            timeAdjustmentDisabled = false;
            reb = 0;
            fixed (uint* tap = &timeAdjustment, tip = &timeIncrement)
            {
                re = f_win32api_GetSystemTimeAdjustment(tap, tip, &reb);
            }
            timeAdjustmentDisabled = reb != 0;
            return re != 0;
        }

        #region win32api
#if DEBUG
        /// <summary>
        /// 设置当前系统时间和日期，系统时间以协调世界时 (UTC) 表示
        /// </summary>
        /// <param name="lpSystemTime">
        /// 指向包含新日期时间的<see cref="Win32SystemTime"/>结构的指针<br/>
        /// 将忽略<see cref="Win32SystemTime.dayOfWeek"/>成员
        /// </param>
        /// <returns>如果该函数成功，则返回值为非0值；返回0用GetLastError获取错误信息</returns>
#endif
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetSystemTime")]
        private static extern unsafe uint win32_SetSystemTime(void* lpSystemTime);

#if DEBUG
        /// <summary>
        /// 检索协调世界时 (UTC) 格式的当前系统日期和时间
        /// </summary>
        /// <param name="lpSystemTime">指向<see cref="Win32SystemTime"/>结构的指针，用于接收日期和时间</param>
#endif
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetSystemTime")]
        private static extern unsafe void win32_GetSystemTime(void* lpSystemTime);

#if DEBUG
        /// <summary>
        /// 检索当前本地日期和时间
        /// </summary>
        /// <param name="lpSystemTime">指向<see cref="Win32SystemTime"/>结构的指针，用于接收日期和时间</param>
#endif
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetLocalTime")]
        private static extern void win32_GetLocalTime(void* lpSystemTime);

        #endregion

        /// <summary>
        /// 设置当前系统时间和日期，以UTC协调世界时设置系统时间
        /// </summary>
        /// <param name="time">要设置的时间；会忽略<see cref="Win32SystemTime.dayOfWeek"/>参数</param>
        /// <returns>是否成功设置；如果失败则返回false，从<see cref="Marshal.GetLastWin32Error"/>获取错误码</returns>
        public static bool TrySetSystemTime(in Win32SystemTime time)
        {
            bool b;
            fixed (Win32SystemTime* ptr = &time)
            {
                b = win32_SetSystemTime(ptr) != 0;
            }
            return b;
        }

        /// <summary>
        /// 设置当前系统时间和日期，以UTC协调世界时设置系统时间
        /// </summary>
        /// <param name="time">要设置的时间；会忽略<see cref="Win32SystemTime.dayOfWeek"/>参数</param>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static void SetSystemTime(in Win32SystemTime time)
        {
            if(!TrySetSystemTime(in time))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// 检索当前系统的UTC协调世界时格式的时间
        /// </summary>
        /// <returns>系统UTC时间</returns>
        public static Win32SystemTime GetSystemTime()
        {
            Win32SystemTime time;
            win32_GetSystemTime(&time);
            return time;
        }

        /// <summary>
        /// 检索当前系统本地时区的时间
        /// </summary>
        /// <returns>系统本地时区的时间</returns>
        public static Win32SystemTime GetSystemLocalTime()
        {
            Win32SystemTime time;
            win32_GetLocalTime(&time);
            return time;
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
        public static EnvDict GetEnvironmentVariables(EnvDictCrFunc createDictionaryFunc)
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
        public static EnvDict GetEnvironmentVariables()
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

        #region 系统

        /// <summary>
        /// 检索调用线程的最后错误代码值
        /// </summary>
        /// <remarks>
        ///  <para>最后一个错误代码按线程进行维护，多个线程不会覆盖彼此的最后一个错误代码</para>
        ///  <para>在.NET运行时通常使用<see cref="Marshal.GetLastWin32Error"/>获取错误码，除非是在一些P/Invoke调用约定中不支持<see cref="DllImportAttribute.SetLastError"/>参数的情况下</para>
        /// </remarks>
        /// <returns>
        /// <para>返回值是调用线程的最后错误代码</para>
        /// <para>
        /// 设置最后错误代码的每个函数的文档的返回值部分记录了函数设置最后错误代码的条件；<br/>
        /// 设置线程最后错误代码的大多数函数在失败时设置它；但是，某些函数还会在成功时设置最后一个错误代码<br/>
        /// 如果未记录函数以设置最后一个错误代码，则此函数返回的值只是要设置的最新最后一个错误代码；某些函数在成功时将最后一个错误代码设置为0，而其他函数则不这样做
        /// </para>
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        public static extern uint GetLastError();

        #endregion

    }

}
#if DEBUG

#endif