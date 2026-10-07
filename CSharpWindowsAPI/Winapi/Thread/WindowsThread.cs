using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

using Cheng.Windows.Win32API;

namespace Cheng.Windows.Threads
{

    /// <summary>
    /// win32线程API
    /// </summary>
    public unsafe static class WindowsThread
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

        /// <summary>
        /// 检索调用线程的线程本地存储槽中的值
        /// </summary>
        /// <remarks>进程中的每个线程都具有自己的针对每个 TLS 索引的槽</remarks>
        /// <param name="index">TlsAlloc 函数分配的 TLS 索引</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern void* TlsGetValue(uint index);

        /// <summary>
        /// 将值存储在调用线程的线程本地存储 (指定 TLS 索引的 TLS) 槽中
        /// </summary>
        /// <param name="index">TLS 索引</param>
        /// <param name="lpTlsValue">要存储在索引的调用线程的 TLS 槽中的值</param>
        /// <returns>成功返回true，否则为false；调用GetLastError获取错误信息</returns>
        [DllImport("kernel32.dll")]
        public static extern uint TlsSetValue(uint index, void* lpTlsValue);

        /// <summary>
        /// 分配线程本地存储 (TLS) 索引
        /// </summary>
        /// <remarks>
        /// 进程的任何线程随后都可以使用此索引来存储和检索线程本地的值，因为每个线程都会收到自己的索引槽
        /// </remarks>
        /// <returns>
        /// 如果函数成功，则返回值为 TLS 索引；索引的槽初始化为零
        /// <para>如果函数失败，则返回<see cref="uint.MaxValue"/>；要获得更多的错误信息，请调用GetLastError</para>
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint TlsAlloc();

        /// <summary>
        /// 释放线程本地存储 (TLS) 索引
        /// </summary>
        /// <param name="lpTlsIndex">TLS索引</param>
        /// <returns>是否成功；失败请调用<see cref="Cheng.Windows.Win32API.ErrorCodeAPI.GetLastError"/>获取错误信息</returns>
        [DllImport("kernel32.dll")]
        public static extern uint TlsFree(uint lpTlsIndex);

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
            uint re;
            processID = 0;
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