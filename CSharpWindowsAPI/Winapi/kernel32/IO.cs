using System;
using System.Runtime.InteropServices;

namespace Cheng.Windows.Win32API
{

    /// <summary>
    /// kernel32.dll 内的其它相关 api
    /// </summary>
    public static unsafe partial class Kernel32
    {

        /// <summary>
        /// 关闭打开的对象句柄
        /// </summary>
        /// <param name="hObject">对象句柄</param>
        /// <returns>(4字节布尔值) 成功关闭返回true，否则返回false</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CloseHandle")]
        public static extern uint CloseHandle(void* hObject);

    }

}
