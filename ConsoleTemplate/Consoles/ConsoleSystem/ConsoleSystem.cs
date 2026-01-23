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
using Cheng.Memorys;

namespace Cheng.Consoles
{

    /// <summary>
    /// Windows 控制台系统
    /// </summary>
    public static unsafe partial class ConsoleSystem
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
        /// <para>如果应用程序没有关联标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDInputHandle()
        {
            var ptr = GetStdHandle(STD_Input_HandleID);

            if ((ptr == new IntPtr(-1)))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return ptr;
        }

        /// <summary>
        /// 获取标准输出设备的句柄
        /// </summary>
        /// <returns>
        /// <para>返回值为指定设备的句柄</para>
        /// <para>如果应用程序没有关联标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDOutputHandle()
        {
            var ptr = GetStdHandle(STD_Output_HandleID);

            if (ptr == new IntPtr(-1))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return ptr;
        }

        /// <summary>
        /// 获取标准错误设备的句柄
        /// </summary>
        /// <returns>
        /// <para>返回值为指定设备的句柄</para>
        /// <para>如果应用程序没有关联标准句柄（例如在交互式桌面上运行的服务），并且尚未重定向这些句柄，则返回空值</para>
        /// </returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static IntPtr GetSTDErrorHandle()
        {
            var ptr = GetStdHandle(STD_Error_HandleID);

            if (ptr == new IntPtr(-1))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
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

        /// <summary>
        /// 从控制台输入缓冲区读取数据，并将其读取到的数据从缓冲区清除
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="record">要从中读取的数据</param>
        /// <returns>是否成功读取</returns>
        public static bool ReadConsoleInput(IntPtr handle, out InputRecord record)
        {
            record = default;
            if (handle == IntPtr.Zero) return false;

            uint uc;
            uint re;
            fixed (InputRecord* ptr = &record)
            {
                re = winapi_ReadConsoleInput(handle, ptr, 1, &uc);
            }
            return (re != 0) && (uc > 0);
        }

        /// <summary>
        /// 从控制台输入缓冲区读取数据，并将其读取到的数据从缓冲区清除
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="lpBuffer">指向连续的<see cref="InputRecord"/>数组地址，表示要将数据读取到的位置</param>
        /// <param name="length"><paramref name="lpBuffer"/>指向的数组的元素数量</param>
        /// <param name="readCount">成功读取后，实际读取并填充到<paramref name="lpBuffer"/>指向区域的数量</param>
        /// <returns>是否成功读取</returns>
        public static bool ReadConsoleInput(IntPtr handle, CPtr<InputRecord> lpBuffer, int length, out int readCount)
        {
            readCount = 0;
            if (lpBuffer.IsEmpty || handle == IntPtr.Zero) return false;
            if (length < 0) return false;
            if (handle == IntPtr.Zero) return false;

            uint rec;
            var rb = winapi_ReadConsoleInput(handle, (void*)lpBuffer, (uint)length, &rec);

            if (rb == 0) return false;

            readCount = (int)rec;
            return true;
        }

        /// <summary>
        /// 从控制台输入缓冲区读取数据，并将其读取到的数据从缓冲区清除
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="buffer">要读取到的目标数组</param>
        /// <param name="index">目标从指定索引开始填充</param>
        /// <param name="count">要读取的数量</param>
        /// <returns>实际成功读取的数量，如果无法成功读取返回0</returns>
        /// <exception cref="ArgumentNullException">缓冲区数组或<paramref name="handle"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int ReadConsoleInput(IntPtr handle, InputRecord[] buffer, int index, int count)
        {
            if (buffer is null || handle == IntPtr.Zero) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > buffer.Length) throw new ArgumentOutOfRangeException();
            uint re;
            fixed (InputRecord* bufptr = buffer)
            {
                if(winapi_ReadConsoleInput(handle, bufptr + index, (uint)count, &re) == 0)
                {
                    return 0;
                }
            }
            return (int)re;
        }

        /// <summary>
        /// 从控制台输入缓冲区读取数据，但不会清除缓冲区数据
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="record">要从中读取的数据</param>
        /// <returns>是否成功读取</returns>
        public static bool PeekConsoleInput(IntPtr handle, out InputRecord record)
        {
            record = default;
            if (handle == IntPtr.Zero) return false;
            uint uc;
            uint re;
            fixed (InputRecord* ptr = &record)
            {
                re = winapi_PeekConsoleInput(handle, ptr, 1, &uc);
            }
            return (re != 0) && (uc > 0);
        }

        /// <summary>
        /// 从控制台输入缓冲区读取数据，但不会清除缓冲区数据
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="lpBuffer">指向连续的<see cref="InputRecord"/>数组地址，表示要将数据读取到的位置</param>
        /// <param name="length"><paramref name="lpBuffer"/>指向的数组的元素数量</param>
        /// <param name="readCount">成功读取后，实际读取并填充到<paramref name="lpBuffer"/>指向区域的数量</param>
        /// <returns>是否成功读取</returns>
        public static bool PeekConsoleInput(IntPtr handle, CPtr<InputRecord> lpBuffer, int length, out int readCount)
        {
            readCount = 0;
            if (lpBuffer.IsEmpty || handle == IntPtr.Zero) return false;
            if (length < 0) return false;
            if (handle == IntPtr.Zero) return false;

            uint rec;
            var rb = winapi_PeekConsoleInput(handle, lpBuffer, (uint)length, &rec);

            if (rb == 0) return false;

            readCount = (int)rec;
            return true;
        }

        /// <summary>
        /// 从控制台输入缓冲区读取数据，但不会清除缓冲区数据
        /// </summary>
        /// <param name="handle">控制台输入缓冲区操作句柄</param>
        /// <param name="buffer">要读取到的目标数组</param>
        /// <param name="index">目标从指定索引开始填充</param>
        /// <param name="count">要读取的数量</param>
        /// <returns>实际成功读取的数量，如果无法成功读取返回0</returns>
        /// <exception cref="ArgumentNullException">缓冲区数组或<paramref name="handle"/>是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int PeekConsoleInput(IntPtr handle, InputRecord[] buffer, int index, int count)
        {
            if (buffer is null || handle == IntPtr.Zero) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count) > buffer.Length) throw new ArgumentOutOfRangeException();
            uint re;
            fixed (InputRecord* bufptr = buffer)
            {
                if (winapi_ReadConsoleInput(handle, bufptr + index, (uint)count, &re) == 0)
                {
                    return 0;
                }
            }
            return (int)re;
        }

        /// <summary>
        /// 检索控制台输入缓冲区中未读输入记录的数量
        /// </summary>
        /// <param name="handle">控制台输入缓冲区的句柄</param>
        /// <returns>控制台输入缓冲区中未读输入记录的数量</returns>
        /// <exception cref="Win32Exception">win32错误</exception>
        /// <exception cref="ArgumentNullException">句柄是null</exception>
        public static int GetConsoleInputEventCount(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentNullException();
            uint rec = 0;
            if(winapi_GetNumberOfConsoleInputEvents(handle, &rec) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return (int)rec;
        }

        /// <summary>
        /// 检索控制台输入缓冲区中未读输入记录的数量
        /// </summary>
        /// <param name="handle">控制台输入缓冲区的句柄</param>
        /// <returns>控制台输入缓冲区中未读输入记录的数量，-1表示出现错误，可使用<see cref="Marshal.GetLastWin32Error"/>获取错误代码</returns>
        public static int TryGetConsoleInputEventCount(IntPtr handle)
        {
            uint rec = 0;
            if (winapi_GetNumberOfConsoleInputEvents(handle, &rec) == 0)
            {
                return -1;
            }

            return (int)rec;
        }

        /// <summary>
        /// 刷新控制台输入缓冲区；当前在输入缓冲区中的所有输入记录都将被丢弃
        /// </summary>
        /// <param name="handle">控制台输入缓冲区的句柄</param>
        /// <returns>是否成功</returns>
        public static bool FlushConsoleInputBuffer(IntPtr handle)
        {
            return winapi_FlushConsoleInputBuffer(handle) != 0;
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

        #region
#if DEBUG
        /// <summary>
        /// 从控制台输入缓冲区读取数据，并将其从缓冲区删除
        /// </summary>
        /// <remarks>
        /// <see href="https://learn.microsoft.com/zh-cn/windows/console/readconsoleinput"/>
        /// </remarks>
        /// <param name="hConsoleInput">控制台输入缓冲区的句柄；该句柄必须具有 GENERIC_READ 访问权限</param>
        /// <param name="lpBuffer">指向接收输入缓冲区数据的 <see cref="InputRecord"/> 结构数组</param>
        /// <param name="nLength"><paramref name="lpBuffer"/>指向的数组大小</param>
        /// <param name="lpNumberOfEventsRead">指向接收所读取输入记录数量的变量</param>
        /// <returns>该函数是否成功</returns>
#endif
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "ReadConsoleInput")]
        private static extern uint winapi_ReadConsoleInput(IntPtr hConsoleInput, void* lpBuffer,
        uint nLength, uint* lpNumberOfEventsRead);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "PeekConsoleInput")]
        private static extern uint winapi_PeekConsoleInput(IntPtr hConsoleInput, void* lpBuffer,
        uint nLength, uint* lpNumberOfEventsRead);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetNumberOfConsoleInputEvents")]
        extern static uint winapi_GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, uint* lpcNumberOfEvents);


        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FlushConsoleInputBuffer")]
        extern static uint winapi_FlushConsoleInputBuffer(IntPtr hConsoleInput);

        #endregion

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