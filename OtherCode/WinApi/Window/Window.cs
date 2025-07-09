using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// 窗体相关api
    /// </summary>
    public static class WindowAPI
    {

        /// <summary>
        /// 函数 <see cref="ShowWindow(IntPtr, CmdShowValue)"/> 的传递参数
        /// </summary>
        public enum CmdShowValue : int
        {

            /// <summary>
            /// 隐藏窗口并激活另一个窗口
            /// </summary>
            Hide = 0,

            /// <summary>
            /// 激活并显示窗口
            /// </summary>
            /// <remarks>
            /// <para>如果窗口最小化、最大化或排列，系统会将其还原到其原始大小和位置</para>
            /// <para>应用程序应在首次显示窗口时指定此标志</para>
            /// </remarks>
            ShowNormal = 1,

            /// <summary>
            /// 激活窗口并将其显示为最小化窗口 ShowMinImized
            /// </summary>
            ShowMinImized = 2,

            /// <summary>
            /// 激活窗口并显示最大化的窗口
            /// </summary>
            ShowMaxImized = 3,

            /// <summary>
            /// 以最近的大小和位置显示窗口
            /// </summary>
            /// <remarks>
            /// <para>类似于<see cref="ShowNormal"/>，只是窗口未激活</para>
            /// </remarks>
            ShowNoActivate = 4,

            /// <summary>
            /// 激活窗口并以当前大小和位置显示窗口
            /// </summary>
            Show = 5,

            /// <summary>
            /// 最小化指定的窗口并按 Z 顺序激活下一个顶级窗口
            /// </summary>
            MinImize = 6,

            /// <summary>
            /// 将窗口显示为最小化窗口
            /// </summary>
            /// <remarks>
            /// <para>此值类似于<see cref="ShowMinImized"/>，但窗口未激活</para>
            /// </remarks>
            ShowMinNoActive = 7,

            /// <summary>
            /// 以当前大小和位置显示窗口
            /// </summary>
            /// <remarks>
            /// <para>此值类似于<see cref="Show"/>，但窗口未激活</para>
            /// </remarks>
            ShowNoActive = 8,

            /// <summary>
            /// 激活并显示窗口
            /// </summary>
            /// <remarks>
            /// <para>如果窗口最小化、最大化或排列，系统会将其还原到其原始大小和位置</para>
            /// <para>还原最小化窗口时，应用程序应指定此标志</para>
            /// </remarks>
            Restore = 9,

            /// <summary>
            /// 进程默认参数传递
            /// </summary>
            /// <remarks>
            /// <para>根据启动应用程序的程序传递给进程的 STARTUPINFO 启动参数结构中指定的值设置显示状态</para>
            /// </remarks>
            ShowDefault = 10,

            /// <summary>
            /// 最小化窗口，即使拥有窗口的线程没有响应
            /// </summary>
            /// <remarks>
            /// <para>仅当最小化不同线程的窗口时，才应使用此标志</para>
            /// </remarks>
            ForceMinImize = 11
        }

        /// <summary>
        /// 设置指定窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nCmdShow">控制窗口的显示方式</param>
        /// <returns>以前可见返回true，以前隐藏返回false</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, CmdShowValue nCmdShow);

        /// <summary>
        /// 检索与调用进程相关联的控制台使用的窗口句柄
        /// </summary>
        /// <returns>与调用进程相关联的控制台所使用的窗口句柄；如果没有关联控制台，则返回空句柄</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();


    }

}
