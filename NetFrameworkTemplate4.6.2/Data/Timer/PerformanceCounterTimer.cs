using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Cheng.Timers
{

    /// <summary>
    /// 微秒级别的高精度低损耗计时器；可用于测量性能，计时等功能
    /// </summary>
    /// <remarks>
    /// <para>在原理上和<see cref="Stopwatch"/>一致</para>
    /// </remarks>
    public sealed unsafe class PerformanceCounterTimer : TickTimeTimer
    {

        #region api

        [DllImport("kernel32.dll", SetLastError = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        internal extern static bool QueryPerformanceCounter(long* lpPerformanceCount);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool QueryPerformanceFrequency(long* frequency);

        /// <summary>
        /// 获取高分辨率计数器的当前值
        /// </summary>
        /// <returns>当前性能计数器值（以计数为单位）</returns>
        /// <exception cref="Win32Exception">win32api错误</exception>
        public static long GetPerformanceCounterTick()
        {
            long t;
            if (QueryPerformanceCounter(&t))
            {
                return t;
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// 检索性能计数器的频率
        /// </summary>
        /// <returns>
        /// <para>性能计数器的频率</para>
        /// <para>性能计数器的频率在系统启动后就会固定，并且在所有处理器中保持一致；因此，只需在应用程序初始化时查询一次参数便可缓存结果</para>
        /// <para>如果系统无法使用性能计数器则返回-1；可以使用<see cref="Marshal.GetHRForLastWin32Error"/>获取错误代码</para>
        /// </returns>
        public static long GetPerformanceFrequency()
        {
            long t;
            if (QueryPerformanceFrequency(&t)) return t;
            return -1;
        }

        /// <summary>
        /// 当前性能计数器的每秒频率，如果系统无法使用性能计数器则该参数为-1
        /// </summary>
        public static readonly long Frequency = GetPerformanceFrequency();

        #endregion

        #region 构造

        /// <summary>
        /// 实例化计时器
        /// </summary>
        /// <exception cref="Win32Exception">系统无法使用高分辨率计时器api</exception>
        public PerformanceCounterTimer()
        {
            GetPerformanceCounterTick();
        }

        /// <summary>
        /// 实例化计时器
        /// </summary>
        /// <param name="span">计时器当前已计时的时间</param>
        /// <exception cref="Win32Exception">系统无法使用高分辨率计时器api</exception>
        public PerformanceCounterTimer(TimeSpan span)
        {
            GetPerformanceCounterTick();
            p_elapsed = (ulong)span.Ticks;
        }

        #endregion

        #region 派生

        protected override ulong NowTimeTick
        {
            get
            {
                long t = 0;
                QueryPerformanceCounter(&t);
                return (ulong)t;
            }
        }

        protected override DateTime NowTime
        {
            get
            {
                long t = 0;
                QueryPerformanceCounter(&t);
                return new DateTime(t);
            }
        }

        #endregion

    }

}
