using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.Winapi
{
    public unsafe static partial class Memory
    {

        #region 磁盘

        /// <summary>
        /// 检索有关磁盘卷上可用空间量的信息
        /// </summary>
        /// <remarks>即空间总量、可用空间总量以及与调用线程关联的用户可用空间总量。</remarks>
        /// <param name="lpDirectoryName">
        /// 磁盘上的目录，如果此参数为null，则该函数使用当前磁盘的根<br/>
        /// 如果使用每用户配额，此值可能小于磁盘上的可用字节总数
        /// </param>
        /// <param name="lpFreeBytesAvailable">
        /// 接收指定目录硬盘的可用字节量；<br/>
        /// 该变量接收磁盘上可供与调用线程关联的用户使用的可用字节总数
        /// </param>
        /// <param name="lpTotalNumberOfBytes">
        /// 接收指定目录硬盘的总字节量；<br/>
        /// 该变量接收磁盘上可供与调用线程关联的用户使用的总字节数
        /// </param>
        /// <param name="lpTotalNumberOfFreeBytes">指向接收磁盘上可用字节总数的变量的指针</param>
        /// <returns>是否调用成功</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        ulong* lpFreeBytesAvailable, ulong* lpTotalNumberOfBytes, ulong* lpTotalNumberOfFreeBytes);

        #endregion

        #region 内存

        /// <summary>
        /// 包含有关物理内存和虚拟内存（包括扩展内存）的当前状态的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public MEMORYSTATUSEX(uint length)
            {
                this = default;
                dwLength = length;
            }

            /// <summary>
            /// 结构大小
            /// </summary>
            /// <remarks>在调用之前，必须设置此成员</remarks>
            public uint dwLength;
            /// <summary>
            /// 一个介于 0 和 100 之间的数字
            /// </summary>
            /// <remarks>指定正在使用的物理内存的近似百分比(0 表示不使用内存，100 表示已满内存使用)</remarks>
            public uint dwMemoryLoad;
            /// <summary>
            /// 实际物理内存量（以字节为单位）
            /// </summary>
            public ulong ullTotalPhys;
            /// <summary>
            /// 当前可用的物理内存量（以字节为单位）
            /// </summary>
            /// <remarks>这是可以立即重复使用的物理内存量，而无需先将其内容写入磁盘。它是备用列表、可用列表和零列表的大小之和</remarks>
            public ulong ullAvailPhys;
            /// <summary>
            /// 系统或当前进程的当前已提交内存限制，以字节为单位，以较小者为准
            /// </summary>
            /// <remarks>若要获取系统范围的已提交内存限制，请调用 GetPerformanceInfo。</remarks>
            public ulong ullTotalPageFile;
            /// <summary>
            /// 当前进程可以提交的最大内存量（以字节为单位）
            /// </summary>
            /// <remarks>
            /// 此值等于或小于系统范围的可用提交值。 若要计算系统范围的可用提交值，请调用 GetPerformanceInfo，并从 CommitLimit 的值中减去 CommitTotal 的值。
            /// </remarks>
            public ulong ullAvailPageFile;
            /// <summary>
            /// 调用进程的虚拟地址空间的用户模式部分的大小（以字节为单位）
            /// </summary>
            /// <remarks>
            /// 此值取决于进程类型、处理器类型和操作系统的配置。 例如，对于 x86 处理器上的大多数 32 位进程，此值约为 2 GB，对于在启用了 4 GB 优化 的系统上运行的大地址感知的 32 位进程，此值约为 3 GB。
            /// </remarks>
            public ulong ullTotalVirtual;
            /// <summary>
            /// 未保留和未提交的内存量
            /// </summary>
            /// <remarks>当前位于调用进程的虚拟地址空间的用户模式部分中的未保留和未提交的内存量（以字节为单位）</remarks>
            public ulong ullAvailVirtual;
            /// <summary>
            /// 保留
            /// </summary>
            /// <remarks>此值始终为0</remarks>
            public ulong ullAvailExtendedVirtual;
        }

        /// <summary>
        /// 检索有关系统当前物理内存和虚拟内存使用情况的信息
        /// </summary>
        /// <param name="stat">指向<see cref="MEMORYSTATUSEX"/>结构的指针，该结构接收有关当前内存可用性的信息</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        public static extern bool GlobalMemoryStatusEx(MEMORYSTATUSEX* stat);

        #endregion


    }

}
