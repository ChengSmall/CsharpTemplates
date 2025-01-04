using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi.User32
{

    /// <summary>
    /// windows窗体api
    /// </summary>
    public static class WindowForms
    {

        /// <summary>
        /// 更改指定窗口的位置和尺寸
        /// </summary>
        /// <remarks>
        /// <para>对于顶级窗口，位置和尺寸是相对于屏幕左上角的；对于子窗口，它们相对于父窗口工作区的左上角</para>
        /// <para></para>
        /// </remarks>
        /// <param name="hwnd">窗口的句柄</param>
        /// <param name="X">窗口左侧的新位置</param>
        /// <param name="Y">窗口顶部的新位置</param>
        /// <param name="nWidth">窗口的新宽度</param>
        /// <param name="nHeight">窗口的新高度</param>
        /// <param name="bRepaint">
        /// <para>指示是否重新绘制窗口</para>
        /// <para>如果此参数为true，则窗口将收到消息。 如果参数为 FALSE，则不会进行任何类型的重新绘制</para>
        /// <para>这适用于工作区、非工作区 (包括标题栏和滚动条) ，以及由于移动子窗口而发现父窗口的任何部分</para>
        /// </param>
        /// <returns>如果该函数成功，则返回true，失败返回false；要获得更多的错误信息，请调用 <see cref="Cheng.OtherCode.Winapi.Kernel32_Other.GetLastError"/></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight,[MarshalAs(UnmanagedType.Bool)] bool bRepaint);

        /// <summary>
        /// 更新窗口
        /// </summary>
        /// <remarks>
        /// <para>如果窗口的更新区域不为空，函数通过向窗口发送 WM_PAINT 消息来更新指定窗口的工作区</para>
        /// <para>函数绕过应用程序队列，将 WM_PAINT 消息直接发送到指定窗口的窗口过程</para>
        /// <para>如果更新区域为空，则不发送任何消息</para>
        /// </remarks>
        /// <param name="hwnd">要更新的窗口的句柄</param>
        /// <returns>如果该函数成功，则返回值为true；如果函数失败，则返回值为false；要获得更多的错误信息，请调用 <see cref="Cheng.OtherCode.Winapi.Kernel32_Other.GetLastError"/></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool UpdateWindow(IntPtr hwnd);

    }

}
