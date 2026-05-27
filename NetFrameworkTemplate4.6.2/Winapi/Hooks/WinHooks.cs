using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Windows.Hooks
{

    /// <summary>
    /// 挂钩类型ID
    /// </summary>
    public enum HookID : int
    {

        /// <summary>
        /// 安装监视击键消息的挂钩过程
        /// </summary>
        KeyBoard = 2,

        /// <summary>
        /// 用于监视发布到消息队列的消息的挂钩过程
        /// </summary>
        GetMessage = 3,

        /// <summary>
        /// 挂钩过程在系统将消息发送到目标窗口过程之前对其进行监视
        /// </summary>
        CallWndProc = 4,

        /// <summary>
        /// 挂钩过程用于接收对 CBT 应用程序有用的通知
        /// </summary>
        CBT = 5,

        /// <summary>
        /// 用于监视由于对话框、消息框、菜单或滚动条中的输入事件而生成的消息
        /// </summary>
        /// <remarks>
        /// 挂钩过程监视与调用线程相同的桌面中的所有应用程序的消息
        /// </remarks>
        SysMegFilter = 6,

        /// <summary>
        /// 安装监视鼠标消息的挂钩过程
        /// </summary>
        Mouse = 7,

        /// <summary>
        /// 安装一个挂钩过程，用于调试其他挂钩过程
        /// </summary>
        Debug = 9,

        /// <summary>
        /// 用于接收对 shell 应用程序有用的通知
        /// </summary>
        Shell = 10,

        /// <summary>
        /// 当应用程序的前台线程变为空闲状态前将调用该挂钩过程
        /// </summary>
        /// <remarks>此挂钩可用于在空闲时间执行低优先级任务</remarks>
        ForeGroundIdle = 11,

        /// <summary>
        /// 该挂钩过程在目标窗口过程处理消息后对其进行监视
        /// </summary>
        CallWndProcret = 12,

        /// <summary>
        /// 安装监视低级别键盘输入事件的挂钩过程
        /// </summary>
        KeyBoard_LL = 13,

        /// <summary>
        /// 安装用于监视低级别鼠标输入事件的挂钩过程
        /// </summary>
        Mouse_LL = 14,

        /// <summary>
        /// 用于监视由于对话框、消息框、菜单或滚动条中的输入事件而生成的消息
        /// </summary>
        MsgFilter = -1,
    }

    /// <summary>
    /// 与 <see cref="WinHooks.SetWindowsHookEx(HookID, ProcCallBack, void*, uint)"/> 一起使用的回调函数
    /// </summary>
    /// <param name="nCode">指定钩子过程是否必须处理消息</param>
    /// <param name="wParam">指定是否已从队列中删除消息</param>
    /// <param name="lParam">指向包含消息详细信息的 MSG 结构的指针</param>
    /// <returns>
    /// 调用<see cref="WinHooks.CallNextHookEx(void*, int, void*, void*)"/>返回的挂钩链句柄
    /// <para>
    /// 如果<paramref name="nCode"/>小于零，则钩子过程必须返回 <see cref="WinHooks.CallNextHookEx(void*, int, void*, void*)"/> 返回的值；<br/>
    /// 如果<paramref name="nCode"/>大于或等于零，则强烈建议调用 <see cref="WinHooks.CallNextHookEx(void*, int, void*, void*)"/> 并返回它返回的值；否则，安装有 <see cref="HookID.GetMessage"/> 钩子的其他应用程序不会收到钩子通知，可能会因此出现错误行为
    /// </para>
    /// </returns>
    public unsafe delegate void* ProcCallBack(int nCode, void* wParam, void* lParam);

    /// <summary>
    /// （不安全代码）windows挂钩api
    /// </summary>
    public unsafe static class WinHooks
    {

        /// <summary>
        /// winapi所在的dll库
        /// </summary>
        public const string dllName = "user32.dll";

        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中
        /// </summary>
        /// <remarks>
        /// 需要安装挂钩过程来监视系统的某些类型的事件；<br/>
        /// 这些事件与特定线程或与调用线程位于同一桌面中的所有线程相关联
        /// </remarks>
        /// <param name="idHook">要安装的挂钩过程的类型</param>
        /// <param name="lpfn">
        /// <para>指向挂钩过程的指针</para>
        /// <para>如果 <paramref name="dwThreadId"/> 参数为零或指定由其他进程创建的线程的标识符， 则 <paramref name="lpfn"/> 参数必须指向 DLL 中的挂钩过程；否则，<paramref name="lpfn"/> 可以指向与当前进程关联的代码中的挂钩过程</para>
        /// </param>
        /// <param name="hMod">
        /// <para>DLL 的句柄，包含 <paramref name="lpfn"/> 参数指向的挂钩过程</para>
        /// <para>如果 <paramref name="dwThreadId"/> 参数指定当前进程创建的线程，并且挂钩过程位于与当前进程关联的代码中，则必须将该 <paramref name="hMod"/> 设置为 null</para>
        /// </param>
        /// <param name="dwThreadId">
        /// <para>要与之关联的挂钩过程所在线程的标识符</para>
        /// <para>如果此参数为0，则对于桌面应用，挂钩过程会关联当前线程所在桌面运行的所有现有线程</para>
        /// </param>
        /// <returns>
        /// 如果函数成功，则返回值是挂钩过程的句柄；如果函数失败，则返回值为null；要获得更多的错误信息，请调用<see cref="Marshal.GetLastWin32Error"/>
        /// </returns>
        [DllImport(dllName, SetLastError = true, EntryPoint = "SetWindowsHookEx")]
        public static extern void* SetWindowsHookEx(HookID idHook, [MarshalAs(UnmanagedType.FunctionPtr)] ProcCallBack lpfn, void* hMod, uint dwThreadId);

        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中
        /// </summary>
        /// <remarks>
        /// 需要安装挂钩过程来监视系统的某些类型的事件；<br/>
        /// 这些事件与特定线程或与调用线程位于同一桌面中的所有线程相关联
        /// </remarks>
        /// <param name="idHook">要安装的挂钩过程的类型，可等价转换为<see cref="HookID"/></param>
        /// <param name="lpfn">
        /// <para>指向挂钩过程的指针</para>
        /// <para>如果 <paramref name="dwThreadId"/> 参数为零或指定由其他进程创建的线程的标识符， 则 <paramref name="lpfn"/> 参数必须指向 DLL 中的挂钩过程；否则，<paramref name="lpfn"/> 可以指向与当前进程关联的代码中的挂钩过程</para>
        /// </param>
        /// <param name="hMod">
        /// <para>DLL 的句柄，包含 <paramref name="lpfn"/> 参数指向的挂钩过程</para>
        /// <para>如果 <paramref name="dwThreadId"/> 参数指定当前进程创建的线程，并且挂钩过程位于与当前进程关联的代码中，则必须将该 <paramref name="hMod"/> 设置为 null</para>
        /// </param>
        /// <param name="dwThreadId">
        /// <para>要与之关联的挂钩过程所在线程的标识符</para>
        /// <para>如果此参数为0，则对于桌面应用，挂钩过程会关联当前线程所在桌面运行的所有现有线程</para>
        /// </param>
        /// <returns>
        /// 如果函数成功，则返回值是挂钩过程的句柄；如果函数失败，则返回值为null；要获得更多的错误信息，请调用<see cref="Marshal.GetLastWin32Error"/>
        /// </returns>
        [DllImport(dllName, SetLastError = true, EntryPoint = "SetWindowsHookEx")]
        public static extern void* SetWindowsHookEx_win(int idHook, [MarshalAs(UnmanagedType.FunctionPtr)] ProcCallBack lpfn, void* hMod, uint dwThreadId);


        /// <summary>
        /// 删除<see cref="SetWindowsHookEx(HookID, ProcCallBack, void*, uint)"/>函数安装在挂钩链中的挂钩过程
        /// </summary>
        /// <param name="hhk">要移除的挂钩的句柄
        /// <para>此参数是由先前调用 <see cref="SetWindowsHookEx(HookID, ProcCallBack, void*, uint)"/> 获取的返回值</para>
        /// </param>
        /// <returns>
        /// 是否成功删除；成功返回true，否则返回false；
        /// <para>要获得更多的错误信息，请调用<see cref="Marshal.GetLastWin32Error"/></para>
        /// </returns>
        [DllImport(dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(void* hhk);

        /// <summary>
        /// 将挂钩信息传递给当前挂钩链中的下一个挂钩过程
        /// </summary>
        /// <remarks>
        /// 挂钩过程可以在处理挂钩信息之前或之后调用此函数
        /// </remarks>
        /// <param name="hhk">
        /// 挂钩链所在的挂钩句柄
        /// </param>
        /// <param name="nCode">
        /// 传递给当前挂钩过程的挂钩代码；下一个挂钩过程使用此代码来确定如何处理挂钩信息
        /// </param>
        /// <param name="wParam">传递给当前挂钩过程的 wParam 值；此参数的含义取决于与当前挂钩链关联的挂钩类型</param>
        /// <param name="lParam">传递给当前挂钩过程的 lParam 值；此参数的含义取决于与当前挂钩链关联的挂钩类型</param>
        /// <returns>
        /// 此值由链中的下一个挂钩过程返回
        /// <para>当前挂钩过程必须返回此值；返回值的含义取决于挂钩类型</para>
        /// </returns>
        [DllImport(dllName, SetLastError = true)]
        public static extern void* CallNextHookEx(void* hhk, int nCode, void* wParam, void* lParam);

        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及创建该窗口的进程（可选）的标识符
        /// </summary>
        /// <param name="hwnd">窗口的句柄</param>
        /// <param name="processId">
        /// <para>指向接收进程标识符的变量的指针</para>
        /// <para>如果此参数不为null， 则函数会将进程的标识符复制到变量；如果函数失败，则变量的值不会被修改</para>
        /// </param>
        /// <returns>
        /// <para>如果函数成功，则返回值是创建窗口的线程的标识符；如果窗口句柄无效，则返回0</para>
        /// </returns>
        [DllImport(dllName, SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, uint* processId);

        /// <summary>
        /// 检索调用线程的线程标识符
        /// </summary>
        /// <returns>返回值是调用该函数所在线程的线程标识符</returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

    }

}
