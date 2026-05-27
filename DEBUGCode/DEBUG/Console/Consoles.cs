using Cheng.Consoles;
using System;
using System.Collections;
using System.ComponentModel;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 仅限win32的控制台测试类
    /// </summary>
    public static class DEBUGConsole
    {

        /// <summary>
        /// 读取可能存在的控制台消息并判断是键盘按下指定按键的消息
        /// </summary>
        /// <param name="handle">输入缓冲区句柄</param>
        /// <param name="consoleKey">判断的控制台按键</param>
        /// <returns>成功返回true；如果按键不匹配，或消息不是按键消息，无法读取，则返回false；如果没有任何消息可读取，返回null</returns>
        /// <exception cref="ArgumentNullException">参数错误</exception>
        /// <exception cref="Win32Exception">win32错误</exception>
        public static bool? IsInputConsoleKey(IntPtr handle, ConsoleKey consoleKey)
        {
            var c = ConsoleSystem.GetConsoleInputEventCount(handle);
            if(c > 0 && (ConsoleSystem.ReadConsoleInput(handle, out var rec)))
            {
                if (rec.IsEventType(ConsoleSystem.EventType.Key))
                {
                    return rec.Key_ConsoleKey == consoleKey;
                }
            }
            return null;
        }

    }

}