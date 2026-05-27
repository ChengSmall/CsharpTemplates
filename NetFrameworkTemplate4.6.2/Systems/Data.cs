using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Cheng.DataStructure;

namespace Cheng.DataStructure.Windows
{

    /// <summary>
    /// 系统内存的状态信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryStatus
    {

        #region 参数

        internal uint dwLength;
        internal uint dwMemoryLoad;
        internal ulong ullTotalPhys;
        internal ulong ullAvailPhys;
        internal ulong ullTotalPageFile;
        internal ulong ullAvailPageFile;
        internal ulong ullTotalVirtual;
        internal ulong ullAvailVirtual;
        internal ulong ullAvailExtendedVirtual;

        #endregion

        #region 参数

        /// <summary>
        /// 实际物理内存量（以字节为单位）
        /// </summary>
        public ulong TotalPhysicsMemorySize
        {
            get => ullTotalPhys;
        }

        /// <summary>
        /// 当前可用的物理内存量（以字节为单位）
        /// </summary>
        public ulong AvailPhysicsMemorySize
        {
            get => ullAvailPhys;
        }

        /// <summary>
        /// 调用所在进程的虚拟地址空间，用户模式部分的大小（以字节为单位）
        /// </summary>
        public ulong TotalVirtual
        {
            get => ullTotalVirtual;
        }

        /// <summary>
        /// 调用所在进程的虚拟地址空间，用户模式部分中的未保留和未提交的内存量（以字节为单位）
        /// </summary>
        public ulong AvailVirtual
        {
            get => ullAvailVirtual;
        }

        /// <summary>
        /// 虚拟提交限制总大小
        /// </summary>
        /// <value>
        /// <para>
        /// 等于物理内存 + 页面文件(s) 的当前总大小<br/>
        /// 这是系统可以向所有进程承诺的最大虚拟内存总量，不是页面文件本身的大小
        /// </para>
        /// </value>
        public ulong TotalPageFile
        {
            get => ullTotalPageFile;
        }

        /// <summary>
        /// 当前可以提交的虚拟内存量
        /// </summary>
        /// <value>
        /// <para>代表在当前状态下，还能额外申请内存而不会出现“内存不足”错误的虚拟内存大小</para>
        /// </value>
        public ulong AvailPageFile
        {
            get => ullAvailPageFile;
        }

        /// <summary>
        /// 内存利用率
        /// </summary>
        /// <value>
        /// <para>返回一个内存利用率的近似值，范围在[0,100]的整数代表百分比</para>
        /// </value>
        public int UtilizationRate
        {
            get => (int)dwMemoryLoad;
        }

        #endregion

    }

}