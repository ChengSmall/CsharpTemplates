using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// heapapi.h 标头
    /// </summary>
    public static unsafe class Heap
    {

        /// <summary>
        /// 检索调用进程的默认堆的句柄
        /// </summary>
        /// <returns>调用进程的堆的句柄，失败返回null，用 GetLastError 获取错误码</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static void* GetProcessHeap();

        /// <summary>
        /// 从堆中分配内存块
        /// </summary>
        /// <param name="handleHeap">要从中分配内存的堆的句柄</param>
        /// <param name="dwFlags">
        /// <para>堆分配选项，没有则传入0</para>
        /// <para>添加 0x8 位值可将新申请到的内存初始化为0</para>
        /// </param>
        /// <param name="size">要分配的字节数</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static void* HeapAlloc(void* handleHeap, uint dwFlags, void* size);

        /// <summary>
        /// 释放从堆中分配的内存
        /// </summary>
        /// <param name="handleHeap">进程堆句柄</param>
        /// <param name="dwFlags">堆位值选项，默认为0</param>
        /// <param name="lpMem">指向要释放的内存的指针</param>
        /// <returns>win32专用4字节布尔值，是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static uint HeapFree(void* handleHeap, uint dwFlags, void* lpMem);

        /// <summary>
        /// 创建可由调用进程使用的专用堆对象
        /// </summary>
        /// <remarks>函数在进程的虚拟地址空间中保留空间，并为此块的指定初始部分分配物理存储</remarks>
        /// <param name="flOptions">堆分配选项</param>
        /// <param name="dwInitialSize">
        /// <para>
        /// 堆的初始大小（以字节为单位）<br/>
        /// 此值确定为堆提交的初始内存量。 该值向上舍入为系统页面大小的倍数
        /// </para>
        /// <para>如果此参数为 0，则函数将提交一页</para>
        /// </param>
        /// <param name="dwMaximumSize">
        /// <para>
        /// 堆的最大大小（以字节为单位）<br/>
        /// 函数将<paramref name="dwMaximumSize"/>舍入到系统页大小的倍数，然后在堆的进程虚拟地址空间中保留该大小的块
        /// </para>
        /// <para>如果<paramref name="dwMaximumSize"/>不为零，则堆大小是固定的，并且不能增长到超过最大大小；此外，对于 32 位进程，可从堆中分配的最大内存块略小于 512 KB，而对于 64 位进程，则略低于 1,024 KB。即使堆的最大大小足以包含块，分配较大块的请求也会失败</para>
        /// <para>如果 dwMaximumSize 为 0，堆大小可能会增大。堆的大小仅受可用内存的限制</para>
        /// </param>
        /// <returns>成功返回新创建的句柄，失败返回null，调用 GetLastError 获取错误信息</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static void* HeapCreate(uint flOptions, void* dwInitialSize, void* dwMaximumSize);

        /// <summary>
        /// 取消提交并释放私有堆对象的所有页面内存，并使堆的句柄失效
        /// </summary>
        /// <param name="handleHeap">要销毁的堆的句柄</param>
        /// <returns>win32专用4字节布尔值，是否成功销毁</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static uint HeapDestroy(void* handleHeap);

    }

}