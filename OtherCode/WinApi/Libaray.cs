using System;
using System.Runtime.InteropServices;

namespace Cheng.OtherCode.Winapi
{


    public static unsafe class Libaray
    {

        /// <summary>
        /// 将指定的模块加载到调用进程的地址空间中
        /// </summary>
        /// <remarks>
        /// <para>指定的模块可能会导致加载其他模块</para>
        /// </remarks>
        /// <param name="lpFileName">
        /// <para>模块的名称</para>
        /// <para>这可以是库模块（.dll 文件）或可执行模块（.exe 文件）； 如果指定的模块是可执行模块，则不会加载静态导入，而是使用 DONT_RESOLVE_DLL_REFERENCES 标志 LoadLibraryEx 加载模块</para>
        /// <para>指定的名称是模块的文件名，与库模块本身中存储的名称无关，由模块定义 （.def） 文件中的 LIBRARY 关键字指定</para>
        /// <para>如果字符串指定完整路径，则仅搜索模块的该路径；如果字符串指定相对路径或没有路径的模块名称，则函数使用标准搜索策略查找模块</para>
        /// <para>如果函数找不到模块，该函数将失败；指定路径时，请务必使用反斜杠'\'，而不是正斜杠'/'</para>
        /// <para>如果字符串指定了没有路径的模块名称，并且省略文件扩展名，该函数会将默认库扩展名“.DLL”追加到模块名称；若要防止函数将“.DLL”追加到模块名称，请在模块名称字符串中包含尾随点字符'.'</para>
        /// </param>
        /// <returns>如果函数成功，则返回值是模块的基址；如果函数失败，则返回为 null</returns>
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "LoadLibrary")]
        public static extern void* LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "LoadLibrary")]
        public static extern void* LoadLibrary(char* lpFileName);

        /// <summary>
        /// 指定的dll动态链接库检索导出函数或变量的地址
        /// </summary>
        /// <param name="hModule">DLL 模块的基址</param>
        /// <param name="lpProcName">函数或变量名称，或函数的序号值；如果此参数是序号值，则它必须在低序位字中，高序位字必须为零</param>
        /// <returns>如果函数成功，则返回导出的函数或变量的地址；如果函数失败，则返回为 NULL</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void* GetProcAddress(void* hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        /// <summary>
        /// 释放加载的动态链接库模块，并在必要时递减其引用计数
        /// </summary>
        /// <remarks>当引用计数达到零时，模块将从调用进程的地址空间中卸载，句柄不再有效</remarks>
        /// <param name="hModule">已加载的库模块的基址</param>
        /// <returns>函数是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(void* hModule);

    }

}