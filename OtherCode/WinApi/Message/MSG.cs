using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HWND = System.IntPtr;

namespace Cheng.OtherCode.Winapi.Messages
{

    /// <summary>
    /// 包含来自线程的消息队列的消息信息结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;

        public int message;

        public IntPtr wParam;

        public IntPtr lParam;

        public int time;

        public int pt_x;

        public int pt_y;
    }

    /// <summary>
    /// windows消息函数
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


    public static unsafe class WinMessage
    {

        /// <summary>
        /// 将指定的消息发送到一个或多个窗口
        /// </summary>
        /// <remarks>
        /// <para>函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回</para>
        /// </remarks>
        /// <param name="hWnd">
        /// <para>窗口的句柄，其窗口过程将接收消息</para>
        /// <para>如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则消息将发送到系统中的所有顶级窗口，包括禁用或不可见的无所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口</para>
        /// <para>消息发送受 UIPI 约束；进程线程只能将消息发送到完整性级别较低或相等进程的线程的消息队列</para>
        /// </param>
        /// <param name="Msg">要发送的消息</param>
        /// <param name="wParam">其他的消息特定信息</param>
        /// <param name="lParam">其他的消息特定信息</param>
        /// <returns>指定消息处理的结果；这取决于发送的消息</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(HWND hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


    }

}
