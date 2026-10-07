using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;
using Cheng.DataStructure.Windows;
using Cheng.Windows.Processes;
using Cheng.Windows.Win32API;

using EnvDict = System.Collections.Generic.IReadOnlyDictionary<string, string>;
using EnvDictCrFunc = Cheng.DataStructure.CreateDictionaryByPairs<string, string>;
using ProWinAPI = Cheng.Windows.Processes.ProcessAPI;
using Cheng.Windows.EnvironmentVariables;

namespace Cheng.Systems
{

    /// <summary>
    /// win32系统功能
    /// </summary>
    public static unsafe partial class SystemEnvironmentWindows
    {

        #region 权限

        internal static class ProcessAPI
        {
            // ========== 常量 ==========
            private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
            private const uint TOKEN_QUERY = 0x0008;
            private const uint TokenElevation = 20;
            private const uint TokenIntegrityLevel = 25;

            // 完整性级别 RID 阈值
            private const int SECURITY_MANDATORY_LOW_RID = 0x1000;
            private const int SECURITY_MANDATORY_MEDIUM_RID = 0x2000;
            private const int SECURITY_MANDATORY_HIGH_RID = 0x3000;
            private const int SECURITY_MANDATORY_SYSTEM_RID = 0x4000;
            private const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = 0x5000;

            [StructLayout(LayoutKind.Sequential)]
            private struct TOKEN_ELEVATION
            {
                public int TokenIsElevated;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SID_AND_ATTRIBUTES
            {
                public IntPtr Sid;
                public uint Attributes;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct TOKEN_MANDATORY_LABEL
            {
                public SID_AND_ATTRIBUTES Label;
            }

            [DllImport("advapi32.dll", SetLastError = true)]
            private static extern int GetTokenInformation(
                IntPtr TokenHandle, uint TokenInformationClass, void* TokenInformation,
                int TokenInformationLength, void* ReturnLength);

            [DllImport("advapi32.dll", SetLastError = true)]
            private static extern IntPtr GetSidSubAuthorityCount(IntPtr pSid);

            [DllImport("advapi32.dll", SetLastError = true)]
            private static extern IntPtr GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

#if DEBUG
            /// <summary>
            /// 根据进程 ID 获取权限等级
            /// </summary>
            /// <param name="processId">目标进程 ID</param>
            /// <returns>进程权限等级</returns>
#endif
            internal static unsafe ProcessPrivilegeLevel GetPrivilegeLevel(int processId)
            {
                var hPro = ProWinAPI.OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, 0, processId);
                if (hPro == null) return ProcessPrivilegeLevel.Unknown;
                IntPtr hProcess = new IntPtr(hPro);

                // 打开进程令牌
                IntPtr hToken = IntPtr.Zero;
                
                if (ProWinAPI.OpenProcessToken(hProcess.ToPointer(), TOKEN_QUERY, (&hToken)) == 0)
                {
                    Cheng.Windows.Win32API.Kernel32.CloseHandle(hProcess.ToPointer());
                    return ProcessPrivilegeLevel.Unknown;
                }

                try
                {
                    // 查询提升状态
                    ProcessPrivilegeLevel level = GetElevationLevel(hToken);
                    if (level != ProcessPrivilegeLevel.Unknown) return level;

                    // 如果提升状态查询失败，回退到完整性级别
                    return GetIntegrityLevel(hToken);
                }
                finally
                {
                    Kernel32.CloseHandle(hToken.ToPointer());
                    Kernel32.CloseHandle(hProcess.ToPointer());
                }
            }

#if DEBUG
            /// <summary>
            /// 通过 TokenElevation 查询提升状态
            /// </summary>
#endif
            private static ProcessPrivilegeLevel GetElevationLevel(IntPtr hToken)
            {
                TOKEN_ELEVATION elevation;
                int size = sizeof(TOKEN_ELEVATION);

                if (GetTokenInformation(hToken, TokenElevation, &elevation, size, null) == 0)
                {
                    return ProcessPrivilegeLevel.Unknown;
                }
                    

                if (elevation.TokenIsElevated != 0)
                    return ProcessPrivilegeLevel.High;
                // 未提升，继续用完整性级别判断
                return ProcessPrivilegeLevel.Unknown;
            }

#if DEBUG
            /// <summary>
            /// 通过 TokenIntegrityLevel 查询完整性级别
            /// </summary>
#endif
            private static unsafe ProcessPrivilegeLevel GetIntegrityLevel(IntPtr hToken)
            {
                // 第一次调用获取所需缓冲区大小
                int bufferSize = 256;
                GetTokenInformation(hToken, TokenIntegrityLevel, null, 0, &bufferSize);
                int lastError = Marshal.GetLastWin32Error();

                if (lastError != 122) return ProcessPrivilegeLevel.Unknown;

                byte[] buffer = new byte[bufferSize];
                fixed (byte* bufp = buffer)
                {
                    if (GetTokenInformation(hToken, TokenIntegrityLevel, bufp, bufferSize, null) == 0)
                    {
                        return ProcessPrivilegeLevel.Unknown;
                    }

                    TOKEN_MANDATORY_LABEL* pLabel = (TOKEN_MANDATORY_LABEL*)bufp;
                    IntPtr pSid = pLabel->Label.Sid;
                    if (pSid == IntPtr.Zero)
                        return ProcessPrivilegeLevel.Unknown;

                    // 获取 SID 的 SubAuthority 数量
                    IntPtr pCount = GetSidSubAuthorityCount(pSid);
                    if (pCount == IntPtr.Zero)
                        return ProcessPrivilegeLevel.Unknown;

                    byte subAuthorityCount = Marshal.ReadByte(pCount);
                    if (subAuthorityCount == 0)
                        return ProcessPrivilegeLevel.Unknown;

                    // 获取最后一个 SubAuthority（即 RID）
                    IntPtr pRid = GetSidSubAuthority(pSid, (uint)(subAuthorityCount - 1));
                    if (pRid == IntPtr.Zero)
                        return ProcessPrivilegeLevel.Unknown;

                    int rid = Marshal.ReadInt32(pRid);

                    // 根据 RID 判断等级
                    if (rid < SECURITY_MANDATORY_LOW_RID)
                        return ProcessPrivilegeLevel.Untrusted;
                    if (rid < SECURITY_MANDATORY_MEDIUM_RID)
                        return ProcessPrivilegeLevel.Low;
                    if (rid < SECURITY_MANDATORY_HIGH_RID)
                        return ProcessPrivilegeLevel.Medium;
                    if (rid < SECURITY_MANDATORY_SYSTEM_RID)
                        return ProcessPrivilegeLevel.High;
                    if (rid < SECURITY_MANDATORY_PROTECTED_PROCESS_RID)
                        return ProcessPrivilegeLevel.System;

                    return ProcessPrivilegeLevel.Protected;
                }
            }

        }

        /// <summary>
        /// 判断此进程是否为管理员权限
        /// </summary>
        /// <returns>
        /// 是管理员权限返回true，否则返回false
        /// </returns>
        public static bool IsAdministrator
        {
            get
            {
                var re = ProcessAPI.GetPrivilegeLevel(ProWinAPI.GetCurrentProcessId());
                return re >= ProcessPrivilegeLevel.High;
            }
        }

        /// <summary>
        /// 获取当前进程的权限等级
        /// </summary>
        /// <returns>当前进程的权限等级</returns>
        public static ProcessPrivilegeLevel GetCurrentProcessLevel()
        {
            return ProcessAPI.GetPrivilegeLevel(ProWinAPI.GetCurrentProcessId());
        }

        /// <summary>
        /// 获取指定进程的权限等级
        /// </summary>
        /// <param name="pid">进程ID</param>
        /// <returns>进程的权限等级</returns>
        public static ProcessPrivilegeLevel GetProcessLevelByID(int pid)
        {
            return ProcessAPI.GetPrivilegeLevel(pid);
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

        /// <summary>
        /// 获取当前进程的所有环境变量
        /// </summary>
        /// <param name="envVariableComparer">环境变量字符串key的比较器，null使用默认的比较器</param>
        /// <param name="createDictionaryFunc">用于创建字典的委托，null使用默认的字典创建方法</param>
        /// <returns>存储当前进程所有环境变量的只读字典</returns>
        /// <exception cref="NotImplementedException">无法获取环境变量</exception>
        public static EnvDict GetEnvironmentVariables(IEqualityComparer<string> envVariableComparer, EnvDictCrFunc createDictionaryFunc)
        {
            if (createDictionaryFunc is null)
            {
                createDictionaryFunc = DeleagateFunctions.CreateDictionaryByPairs;
            }

            char* envstrptr = EnvVariableAPI.api_GetEnvironmentStrings();
            if (envstrptr == null)
            {
                throw new NotImplementedException();
            }

            try
            {
                return createDictionaryFunc.Invoke(EnvVariableAPI.f_getEnvs(new CPtr<char>(envstrptr)), envVariableComparer ?? StringComparer.OrdinalIgnoreCase);
            }
            finally
            {
                EnvVariableAPI.api_FreeEnvironmentStrings(envstrptr);
            }

        }

        /// <summary>
        /// 获取当前进程的所有环境变量
        /// </summary>
        /// <returns>存储当前进程所有环境变量的只读字典</returns>
        /// <exception cref="NotImplementedException">无法获取环境变量</exception>
        public static EnvDict GetEnvironmentVariables()
        {
            return GetEnvironmentVariables(null, null);
        }

        #endregion

    }

}
#if DEBUG
#endif