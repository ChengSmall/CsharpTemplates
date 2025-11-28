using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Security;
using System.IO;

using SCol = global::System.Console;

namespace Cheng.Consoles
{

    /// <summary>
    /// Windows .Net 控制台系统
    /// </summary>
    public static class ConsoleSystem
    {

        #region 虚拟终端

        /// <summary>
        /// 启用虚拟终端
        /// </summary>
        /// <returns>0表示成功启动，否则返回错误参数</returns>
        public static uint TryEnableVirtualTerminalProcessingOnWindows()
        {
            var iStdOut = GetStdHandle(STD_Output_HandleID);

            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                return GetLastError();
            }

            outConsoleMode |= (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);

            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                return GetLastError();
            }
            return 0;
        }

        /// <summary>
        /// 启用虚拟终端
        /// </summary>
        /// <exception cref="Win32Exception">出现win32错误</exception>
        public static void EnableVirtualTerminalProcessingOnWindows()
        {
            var re = TryEnableVirtualTerminalProcessingOnWindows();
            if(re != 0)
            {
                throw new Win32Exception((int)re);
            }
        }

        /// <summary>
        /// 关闭虚拟终端
        /// </summary>
        /// <returns>0表示成功关闭，否则返回错误参数</returns>
        public static uint TryDisableVirtualTerminalProcessingOnWindows()
        {
            var iStdOut = GetStdHandle(STD_Output_HandleID);

            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                return GetLastError();
            }

            //outConsoleMode |= (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);

            if (!SetConsoleMode(iStdOut, (outConsoleMode & ~(ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN))))
            {
                return GetLastError();
            }
            return 0;
        }

        /// <summary>
        /// 关闭虚拟终端
        /// </summary>
        /// <exception cref="Win32Exception">出现win32错误</exception>
        public static void DisableVirtualTerminalProcessingOnWindows()
        {
            uint re = TryDisableVirtualTerminalProcessingOnWindows();
            if (re != 0)
            {
                throw new Win32Exception((int)re);
            }
        }

        #endregion

        #region winapi

        #region 封装

        /// <summary>
        /// 获取标准输入设备的句柄
        /// </summary>
        /// <returns>
        /// <para>返回值为指定设备的句柄</para>
        /// <para>如果应用程序没有关联的标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDInputHandle()
        {
            var ptr = GetStdHandle(STD_Input_HandleID);

            if (ptr == new IntPtr(-1))
            {
                throw new Win32Exception((int)GetLastError());
            }

            return ptr;
        }

        /// <summary>
        /// 获取标准输出设备的句柄
        /// </summary>
        /// <returns>
        /// <para>返回值为指定设备的句柄</para>
        /// <para>如果应用程序没有关联的标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDOutputHandle()
        {
            var ptr = GetStdHandle(STD_Output_HandleID);

            if (ptr == new IntPtr(-1))
            {
                throw new Win32Exception((int)GetLastError());
            }

            return ptr;
        }

        /// <summary>
        /// 获取标准错误设备的句柄
        /// </summary>
        /// <returns>
        /// <para>返回值为指定设备的句柄</para>
        /// <para>如果应用程序没有关联的标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDErrorHandle()
        {
            var ptr = GetStdHandle(STD_Error_HandleID);

            if (ptr == new IntPtr(-1))
            {
                throw new Win32Exception((int)GetLastError());
            }

            return ptr;
        }

        /// <summary>
        /// 将控制台窗口显示或隐藏
        /// </summary>
        /// <param name="enable">true显示控制台窗口，false隐藏控制台窗口</param>
        /// <returns>如果调用该函数之前控制台处于显示状态，返回true；处于隐藏状态返回false</returns>
        /// <exception cref="Win32Exception">无法获取控制台句柄或其他win32错误</exception>
        public static bool WindowShow(bool enable)
        {
            const int onEnable = 5;
            const int onDisable = 0;

            var handle = GetConsoleWindow();
            if(handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return ShowWindow(handle, enable ? onEnable : onDisable);
        }

        #endregion

        #region 原始

        /// <summary>
        /// 设置指定窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nCmdShow"></param>
        /// <returns>以前可见返回true，以前隐藏返回false</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 检索与调用进程相关联的控制台使用的窗口句柄
        /// </summary>
        /// <returns>与调用进程相关联的控制台所使用的窗口句柄；如果没有关联控制台，则返回空句柄</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        private const int STD_Input_HandleID = -10;
        private const int STD_Output_HandleID = -11;
        private const int STD_Error_HandleID = -12;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        /// <summary>
        /// 设置控制台输入缓冲区的输入模式或控制台屏幕缓冲区的输出模式
        /// </summary>
        /// <param name="hConsoleHandle">控制台输入缓冲区或控制台屏幕缓冲区的句柄</param>
        /// <param name="dwMode">要设置的输入或输出模式</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        #endregion

        #endregion

        #region 扩展

        /// <summary>
        /// 将控制台的缓冲区大小设置为当前控制台窗口区域大小
        /// </summary>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">权限错误</exception>
        public static void SetBufferToWindows()
        {
            SCol.SetBufferSize(SCol.WindowLeft + SCol.WindowWidth,
                SCol.WindowTop + SCol.WindowHeight);
        }

        /// <summary>
        /// 设置控制台的缓冲区大小并保证缓冲区不会小于控制台窗口区域
        /// </summary>
        /// <param name="width">长度</param>
        /// <param name="height">行高</param>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">权限错误</exception>
        public static void SetBufferGreaterThanWindows(int width, int height)
        {
            SCol.SetBufferSize(Math.Max(SCol.WindowLeft + SCol.WindowWidth, width),
                Math.Max(SCol.WindowTop + SCol.WindowHeight, height));
        }

        /// <summary>
        /// 设置控制台缓冲区行高并保证高度不会小于控制台窗口区域
        /// </summary>
        /// <param name="height">行高</param>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">权限错误</exception>
        public static void SetBufferHeightGreaterThanWindows(int height)
        {
            SCol.BufferHeight = Math.Max(height, SCol.WindowTop + SCol.WindowHeight);
        }

        /// <summary>
        /// 设置控制台缓冲区行高并保证长度不会小于控制台窗口区域
        /// </summary>
        /// <param name="width">长度</param>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="SecurityException">权限错误</exception>
        public static void SetBufferWidthGreaterThanWindows(int width)
        {
            SCol.BufferHeight = Math.Max(width, SCol.WindowLeft + SCol.WindowWidth);
        }

        #endregion

    }

}
