using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Cheng.Windows.Threads
{

    /// <summary>
    /// win32线程API
    /// </summary>
    public unsafe static class WinThreadAPI
    {

        #region win

#if DEBUG
        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及创建该窗口的进程（可选）的标识符
        /// </summary>
        /// <param name="hwnd">窗口的句柄</param>
        /// <param name="processId">
        /// <para>指向接收进程标识符的变量的指针</para>
        /// <para>如果此参数不为null， 则函数会将进程的标识符复制到变量；如果函数失败，则变量的值不会被修改</para>
        /// </param>
        /// <returns>
        /// <para>如果函数成功，则返回值是创建窗口的线程的标识符；如果窗口句柄无效，则返回0</para>
        /// </returns>
#endif
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowThreadProcessId")]
        private static extern uint win_GetWindowThreadProcessId(IntPtr hwnd, uint* processId);

        #endregion

        #region thread

        /// <summary>
        /// 检索调用线程的线程标识符
        /// </summary>
        /// <returns>返回值是调用该函数所在线程的线程标识符</returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// 检索创建指定窗口的线程的标识符
        /// </summary>
        /// <param name="hwnd">窗口的hwnd句柄</param>
        /// <returns>创建窗口的线程标识符</returns>
        /// <exception cref="Win32Exception">句柄无效</exception>
        public static uint GetWindowThreadProcessId(IntPtr hwnd)
        {
            var re = win_GetWindowThreadProcessId(hwnd, null);

            if(re == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return re;
        }

        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及创建该窗口的进程（可选）的标识符
        /// </summary>
        /// <param name="hwnd">窗口的hwnd句柄</param>
        /// <param name="processID">返回进程的标识符</param>
        /// <returns>创建窗口的线程标识符</returns>
        /// <exception cref="Win32Exception">句柄无效</exception>
        public static uint GetWindowThreadProcessId(IntPtr hwnd, out uint processID)
        {
            //processID = default;
            uint re;

            fixed(uint* up = &processID)
            {
                re = win_GetWindowThreadProcessId(hwnd, up);
                if (re == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            
            return re;
        }

        #endregion

    }
}
#if DEBUG
#endif