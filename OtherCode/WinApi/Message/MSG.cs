using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

}
