using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi
{

    public static unsafe class WinCrypt
    {

        #region

        public const string dllName = "advapi32.dll";

        /// <summary>
        /// 创建和销毁密钥容器
        /// </summary>
        /// <remarks>
        /// <para>此函数首先尝试查找具有 <paramref name="dwProvType"/> 和 <paramref name="szProvider"/> 参数中描述的特征的 CSP。如果找到 CSP，该函数将尝试在 CSP 中查找与 <paramref name="szContainer"/> 参数指定的名称匹配的密钥容器</para>
        /// <para>用 <paramref name="dwFlags"/> 的适当设置，此函数还可以创建和销毁密钥容器，并且可以在不需要访问私钥时提供对具有临时密钥容器的 CSP 的访问权限</para>
        /// <para></para>
        /// </remarks>
        /// <param name="hProv">指向 CSP 句柄的指针；使用完 CSP 后，通过调用<see cref="CryptReleaseContext(IntPtr, uint)"/>函数释放句柄</param>
        /// <param name="szContainer">
        /// <para>密钥容器名称，用于向加密服务提供程序（CSP）标识密钥容器</para>
        /// <para>该名称与存储密钥的方法无关</para>
        /// <para>不同CSP的密钥容器存储方式可能不同：有些CSP将密钥容器存储在内部（如硬件中），有些使用系统注册表，还有些使用文件系统</para>
        /// <para>在大多数情况下，当dwFlags参数设置为 CRYPT_VERIFYCONTEXT 时，<paramref name="szContainer"/>参数必须设置为null。但对于基于硬件的CSP（如智能卡CSP），则可以访问指定容器中的公开可用信息。</para>
        /// </param>
        /// <param name="szProvider">
        /// <para>包含要使用的加密服务提供程序（CSP）的名称</para>
        /// <para>如果此参数设置为null，则使用用户默认提供程序</para>
        /// </param>
        /// <param name="dwProvType">指定要获取的提供程序的类型；加密提供程序类型中讨论了定义的提供程序类型</param>
        /// <param name="dwFlags">标志位；此参数通常设置为零，但某些应用程序会设置以下一个或多个标志</param>
        /// <returns>是否成功</returns>
        [DllImport(dllName, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptAcquireContext(IntPtr* hProv, 
            string szContainer, string szProvider, uint dwProvType, uint dwFlags);

        /// <summary>
        /// 使用加密随机字节填充缓冲区
        /// </summary>
        /// <remarks>
        /// <para>此函数生成的数据在加密上是随机的；它比一般随机算法生成的数据要更加随机</para>
        /// <para>此函数通常用于生成随机 初始化向量 和 盐值</para>
        /// <para>软件随机数生成器的工作方式基本相同；它们从随机种子开始，然后使用算法基于它生成伪随机位序列。此过程最困难的部分是获取一个真正随机的种子。这通常基于用户输入延迟或来自一个或多个硬件组件的抖动</para>
        /// </remarks>
        /// <param name="hProv">加密服务提供程序 (CSP) 调用<see cref="CryptAcquireContext(IntPtr*, string, string, uint, uint)"/>创建的句柄</param>
        /// <param name="dwLen">要生成的随机数据的字节数</param>
        /// <param name="pbBuffer">用于接收返回数据的缓冲区；此缓冲区的长度必须至少 为<paramref name="dwLen"/>字节</param>
        /// <returns>是否成功</returns>
        [DllImport(dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptGenRandom(IntPtr hProv, uint dwLen, byte* pbBuffer);

        /// <summary>
        /// 于释放加密服务提供程序（CSP）的句柄和密钥容器
        /// </summary>
        /// <remarks>
        /// <para>每次调用该函数时，CSP 的引用计数会减1；当引用计数归零时，该上下文将被完全释放，应用程序中的任何函数都将无法再使用它</para>
        /// </remarks>
        /// <param name="hProv">加密服务提供程序 (CSP) 调用<see cref="CryptAcquireContext(IntPtr*, string, string, uint, uint)"/>创建的句柄</param>
        /// <param name="dwFlags">保留以供将来使用，必须为零</param>
        /// <returns>函数是否成功</returns>
        [DllImport(dllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

        #endregion

    }

}
