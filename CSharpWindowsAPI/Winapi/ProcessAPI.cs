using System;
using System.Runtime.InteropServices;


namespace Cheng.Windows.Processes
{

    /// <summary>
    /// 进程相关 api
    /// </summary>
    public static unsafe class ProcessAPI
    {

        /// <summary>
        /// 将数据写入到指定进程中的内存区域
        /// </summary>
        /// <remarks>要写入的整个区域必须可访问，否则操作将失败</remarks>
        /// <param name="hProcess">要修改的进程内存的句柄</param>
        /// <param name="writeAddress">
        /// 要写入到的指定进程<paramref name="hProcess"/>中地址的指针
        /// <para>在进行数据传输之前，系统会验证指定大小的基址和内存中的所有数据是否可供写入访问，如果无法访问，则函数将失败</para>
        /// </param>
        /// <param name="writeBuffer">要写入的数据，包含执行堆栈所在的进程内存块</param>
        /// <param name="size">要写入的数据大小</param>
        /// <param name="realWriteSize">
        /// 实际写入的内存大小；
        /// <para>该参数表示指向当前执行堆栈所在进程地址的指针，指向类型在32位运行环境下为32位整形，64位运行环境下为64位整形</para>
        /// <para>该值为null时，忽略此参数</para>
        /// </param>
        /// <returns>(4字节布尔值) 成功写入返回true，否则返回false</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WriteProcessMemory(void* hProcess, void* writeAddress, void* writeBuffer, uint size, void* realWriteSize);

        /// <summary>
        /// 读取指定进程中的内存
        /// </summary>
        /// <param name="hProcess">要读取的进程句柄</param>
        /// <param name="readAddress">指向进程<paramref name="hProcess"/>中要读取的首地址</param>
        /// <param name="readBuffer">要将数据读取到的内存地址</param>
        /// <param name="size">要读取的字节大小</param>
        /// <param name="realReadSize">
        /// 实际读取到的字节大小
        /// <para>该参数表示指向当前执行堆栈所在进程地址的指针，指向类型在32位运行环境下为32位整形，64位运行环境下为64位整形</para>
        /// <para>该值为null时，忽略此参数</para>
        /// </param>
        /// <returns>(4字节布尔值) 成功读取返回true，否则返回false</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ReadProcessMemory(void* hProcess, void* readAddress, void* readBuffer, uint size, void* realReadSize);

        /// <summary>
        /// 打开现有的本地进程对象
        /// </summary>
        /// <param name="dwDesiredAccess">对进程对象的访问位或枚举，使用<see cref="ProcessAccessFlags"/>转化</param>
        /// <param name="bInheritHandle">(4字节布尔值) 如果此值为true，则此进程创建的进程将继承句柄</param>
        /// <param name="processId">要打开的本地进程的标识符</param>
        /// <returns>返回打开的进程句柄；若失败则返回null</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void* OpenProcess(uint dwDesiredAccess, uint bInheritHandle, int processId);

        /// <summary>
        /// 当前调用堆栈所在进程的唯一标识符
        /// </summary>
        /// <returns>进程终止之前的唯一标识符</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentProcessId();

        /// <summary>
        /// 返回指定句柄的进程ID
        /// </summary>
        /// <param name="proHandle">进程句柄</param>
        /// <returns>(4字节布尔值) 成功返回进程句柄，否则返回0</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetProcessId(void* proHandle);

        /// <summary>
        /// 确定指定的进程是在 WOW64 还是 x64 处理器的 Intel64 下运行
        /// </summary>
        /// <param name="proHandle">进程的句柄</param>
        /// <param name="wow64Process">指向判断值的指针，0表示false；非0表示true</param>
        /// <returns>(4字节布尔值) 是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint IsWow64Process(void* proHandle, int* wow64Process);


        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int OpenProcessToken(
            void* processHandle, uint desiredAccess, void* tokenHandle);


    }

}
