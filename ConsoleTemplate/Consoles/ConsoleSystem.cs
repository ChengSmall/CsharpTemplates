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
using Cheng.Texts;

namespace Cheng.Consoles
{

    /// <summary>
    /// Windows 控制台系统
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

#if DEBUG
        /// <summary>
        /// 设置指定窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nCmdShow"></param>
        /// <returns>以前可见返回true，以前隐藏返回false</returns>
#endif
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

#if DEBUG
        /// <summary>
        /// 设置控制台输入缓冲区的输入模式或控制台屏幕缓冲区的输出模式
        /// </summary>
        /// <param name="hConsoleHandle">控制台输入缓冲区或控制台屏幕缓冲区的句柄</param>
        /// <param name="dwMode">要设置的输入或输出模式</param>
        /// <returns></returns>
#endif
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        #endregion

        #endregion

        #region 宽高

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

        #region 标准输入

        /// <summary>
        /// 进入控制台输入模式
        /// </summary>
        /// <remarks>
        /// <para>进入输入模式后可自由编写文本，并允许使用退格键，换行键操作输入的内容</para>
        /// <para>按下回车键换行；按Shift + 回车键退出输入模式，或按下<paramref name="cancelKey"/>退出输入模式并将返回值设为false</para>
        /// </remarks>
        /// <param name="intercept">输入时是否禁止显示字符</param>
        /// <param name="cancelKey">按下该按键后将立即结束，并返回false；不想设退出键可将参数设为0</param>
        /// <param name="inputAction">每次检测到一次输入都会执行回调，null表示不使用回调</param>
        /// <param name="inputText">所有输入的内容，List每一个元素代表一行内容</param>
        /// <returns>按下回车结束输入并返回true，输入结果保存到<paramref name="inputText"/>；如果按下<paramref name="cancelKey"/>返回false，也会将结果保存到<paramref name="inputText"/></returns>
        public static bool ReadInput(bool intercept, ConsoleKey cancelKey, Action<ConsoleKeyInfo> inputAction, out List<CMStringBuilder> inputText)
        {

            bool isAction = inputAction != null;
            inputText = new List<CMStringBuilder>();
            inputText.Add(new CMStringBuilder());
            Loop:

            var rk = SCol.ReadKey(true);

            var key = rk.Key;
            if ((cancelKey != 0) && (key == cancelKey)) return false;

            var mod = rk.Modifiers;

            if (isAction)
            {
                inputAction.Invoke(rk);
            }

            if (key == ConsoleKey.Enter)
            {
                //回车
                if ((mod & ConsoleModifiers.Shift) == ConsoleModifiers.Shift)
                {
                    //Shift + 回车
                    return true;
                }
                else
                {
                    //单回车
                    //换行
                    inputText.Add(new CMStringBuilder());
                    if (!intercept) SCol.WriteLine();
                    goto Loop;
                }
            }

            if (key == ConsoleKey.Backspace)
            {
                //退格
                if (inputText.Count > 0)
                {
                    //当前行缓存
                    var sb = inputText[inputText.Count - 1];
                    if (sb.Length > 0)
                    {
                        //当前行退格
                        //最后一个字符
                        var endc = sb[sb.Length - 1];
                        sb.RemoveEnd(1);
                        //覆盖当前行
                        if (!intercept)
                        {
                            SCol.CursorLeft = 0;
                            int i;
                            for (i = 0; i < sb.Length; i++)
                            {
                                var sbc = sb[i];
                                SCol.Write(sbc);
                            }
                            //最后一个打印
                            if (endc.IsFullWidth())
                            {
                                //全角字符用全角空格
                                SCol.Write('\u3000');
                                SCol.CursorLeft = SCol.CursorLeft - 2;
                            }
                            else
                            {
                                SCol.Write(' ');
                                SCol.CursorLeft = SCol.CursorLeft - 1;
                            }
                        }

                    }
                    else
                    {
                        //当前行为空
                        if (inputText.Count > 1)
                        {
                            //最后一行缓存
                            sb = inputText[inputText.Count - 1];

                            //删除最后一行
                            inputText.RemoveAt(inputText.Count - 1);
                            if (!intercept)
                            {
                                Console.CursorLeft = 0;
                                int i;
                                for (i = 0; i < sb.Length; i++)
                                {
                                    var sbc = sb[i];
                                    if (sbc.IsFullWidth())
                                    {
                                        //全角字符用全角空格
                                        Console.Write('\u3000');
                                    }
                                    else
                                    {
                                        Console.Write(' ');
                                    }
                                }

                                //提升光标并左移到顶
                                Console.SetCursorPosition(0, Console.CursorTop - 1);
                                //获取上一行缓存
                                sb = inputText[inputText.Count - 1];
                                //打印
                                var cbuf = sb.GetCharBuffer();
                                Console.Write(cbuf.Array, cbuf.Offset, cbuf.Count);
                            }
                        }
                        else
                        {
                            //退回到顶
                        }
                    }
                }

                goto Loop;
            }

            var kc = rk.KeyChar;

            if (!intercept) Console.Write(kc);

            inputText[inputText.Count - 1].Append(kc);

            goto Loop;
        }

        /// <summary>
        /// 进入控制台行输入模式
        /// </summary>
        /// <remarks>
        /// <para>用于获取一行输入的模式</para>
        /// </remarks>
        /// <param name="inputText">要使用并用于接受输入的缓冲区</param>
        /// <param name="intercept">输入时是否禁止在控制台显示字符</param>
        /// <param name="maxCount">限制最大字符输入数量，0表示无限制</param>
        /// <param name="cancelKey">按下该按键后将立即结束，并返回false；不想设退出键可将参数设为0</param>
        /// <returns>成功获取一行true，如果按下了<paramref name="cancelKey"/>按键则返回false</returns>
        /// <exception cref="ArgumentNullException">缓冲区参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCount"/>小于0</exception>
        public static bool ReadInputLine(CMStringBuilder inputText, bool intercept, int maxCount, ConsoleKey cancelKey)
        {
            if (inputText is null) throw new ArgumentNullException();
            if (maxCount < 0) throw new ArgumentOutOfRangeException();
            inputText.Clear();

            Loop:
            var rk = SCol.ReadKey(true);

            var key = rk.Key;
            if ((cancelKey != 0) && (key == cancelKey)) return false;

            if (key == ConsoleKey.Enter)
            {
                return true;
            }

            if (key == ConsoleKey.Backspace)
            {
                var sb = inputText;
                if (sb.Length > 0)
                {
                    //当前行退格
                    //最后一个字符
                    var endc = sb[sb.Length - 1];
                    sb.RemoveEnd(1);
                    //覆盖当前行
                    if (!intercept)
                    {
                        SCol.CursorLeft = 0;
                        int i;
                        for (i = 0; i < sb.Length; i++)
                        {
                            var sbc = sb[i];
                            SCol.Write(sbc);
                        }
                        //最后一个覆盖
                        if (endc.IsFullWidth())
                        {
                            //全角字符用全角空格
                            SCol.Write('\u3000');
                            SCol.CursorLeft = SCol.CursorLeft - 2;
                        }
                        else
                        {
                            SCol.Write(' ');
                            SCol.CursorLeft = SCol.CursorLeft - 1;
                        }
                    }
                }
                goto Loop;
            }
            if ((maxCount != 0) && inputText.Length >= maxCount)
            {

                goto Loop;
            }
            var kc = rk.KeyChar;
            inputText.Append(kc);
            if (!intercept)
            {
                SCol.Write(kc);
            }
            goto Loop;
        }

        /// <summary>
        /// 进入控制台行输入模式
        /// </summary>
        /// <param name="inputText">要使用并用于接受输入的缓冲区</param>
        /// <param name="intercept">输入时是否禁止在控制台显示字符</param>
        /// <returns>成功获取一行true，如果按下了<see cref="ConsoleKey.Escape"/>则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool ReadInputLine(CMStringBuilder inputText, bool intercept)
        {
            return ReadInputLine(inputText, intercept, 0, ConsoleKey.Escape);
        }

        /// <summary>
        /// 进入控制台行输入模式
        /// </summary>
        /// <param name="inputText">要使用并用于接受输入的缓冲区</param>
        /// <returns>成功获取一行true，如果按下了<see cref="ConsoleKey.Escape"/>则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool ReadInputLine(CMStringBuilder inputText)
        {
            return ReadInputLine(inputText, false, 0, ConsoleKey.Escape);
        }

        #endregion

    }

}
#if DEBUG
#endif