using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// Kernel32库的其它api
    /// </summary>
    public unsafe static class Kernel32_Other
    {

        /// <summary>
        /// 检索调用线程的最后错误代码值
        /// </summary>
        /// <remarks>
        ///  最后一个错误代码按线程进行维护，多个线程不会覆盖彼此的最后一个错误代码
        /// </remarks>
        /// <returns>
        /// <para>返回值是调用线程的最后错误代码</para>
        /// <para>
        /// 设置最后错误代码的每个函数的文档的返回值部分记录了函数设置最后错误代码的条件；<br/>
        /// 设置线程最后错误代码的大多数函数在失败时设置它；但是，某些函数还会在成功时设置最后一个错误代码<br/>
        /// 如果未记录函数以设置最后一个错误代码，则此函数返回的值只是要设置的最新最后一个错误代码；某些函数在成功时将最后一个错误代码设置为0，而其他函数则不这样做
        /// </para>
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        /// <summary>
        /// 表示当前可用磁盘驱动器的位掩码
        /// </summary>
        /// <returns>
        /// <para>如果函数成功，则返回值为表示当前可用磁盘驱动器的位掩码</para>
        /// <para>从第0位开始表示驱动器A，1表示驱动器B，2表示驱动器C；1表示存在，0表示不存在</para>
        /// <para>返回0表示函数失败</para>
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetLogicalDrives();

        /// <summary>
        /// <see cref="SetThreadExecutionState(EsFlags)"/>函数的参数
        /// </summary>
        public enum EsFlags : uint
        {

            /// <summary>
            /// 错误
            /// </summary>
            Error = 0,

            /// <summary>
            /// 通过重置系统空闲计时器强制系统处于工作状态
            /// </summary>
            System_Required = 0x00000001,

            /// <summary>
            /// 通过重置显示空闲计时器强制显示处于打开状态
            /// </summary>
            Display_Required = 0x00000002,

            /// <summary>
            /// 离开模式只能由媒体录制和媒体分发应用程序使用，这些应用程序必须在计算机似乎处于睡眠状态时在台式计算机上执行关键后台处理
            /// </summary>
            AwayMode_Required = 0x00000040,

            /// <summary>
            /// 通知系统正在设置的状态应保持有效，直到使用 ES_CONTINUOUS 的下一次调用和清除其他状态标志之一。
            /// </summary>
            CONTINUOUS = 0x80000000

        }

        /// <summary>
        /// 使应用程序能够通知系统它正在使用，从而防止系统在应用程序运行时进入睡眠状态或关闭显示器
        /// </summary>
        /// <param name="esFlags">线程的执行要求</param>
        /// <returns>如果函数成功，则返回值为上一个线程执行状态；如果函数失败，则返回值<see cref="EsFlags.Error"/></returns>
        [DllImport("kernel32.dll")]
        public static extern EsFlags SetThreadExecutionState(EsFlags esFlags);

        /// <summary>
        /// 检索自系统启动以来经过的毫秒数
        /// </summary>
        /// <returns>毫秒数</returns>
        [DllImport("kernel32.dll")]
        public static extern ulong GetTickCount64();

    }
}
