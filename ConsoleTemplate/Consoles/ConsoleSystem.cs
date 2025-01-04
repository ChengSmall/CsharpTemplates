using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

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
            var iStdOut = GetStdHandle(STD_Output_HandleID);
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                throw new Win32Exception((int)GetLastError());
                //return (uint)Marshal.GetLastWin32Error();
            }
            outConsoleMode |= (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);

            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                throw new Win32Exception((int)GetLastError());
            }
            //return 0;
        }

        /// <summary>
        /// 关闭虚拟终端
        /// </summary>
        /// <returns>0表示成功启动，否则返回错误参数</returns>
        public static uint TryDisableVirtualTerminalProcessingOnWindows()
        {
            var iStdOut = GetStdHandle(STD_Output_HandleID);

            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                return GetLastError();
            }

            //outConsoleMode |= (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);

            if (!SetConsoleMode(iStdOut, outConsoleMode & ENABLE_VIRTUAL_TERMINAL_PROCESSING))
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

        #endregion

        #region 原始

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

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        #endregion

        #endregion

    }

}
