using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Cheng.Streams;

namespace Cheng.Memorys
{

    static unsafe partial class MemoryOperation
    {

        #region 内存拷贝

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的原字节数组</param>
        /// <param name="copyMemoryOffset">原字节数组的起始偏移</param>
        /// <param name="toMemory">要拷贝到的目标字节数组</param>
        /// <param name="toMempryOffset">写入到目标数组的起始偏移</param>
        /// <param name="size">要拷贝的字节数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        public static void MemoryCopy(this byte[] copyMemory, int copyMemoryOffset, byte[] toMemory, int toMempryOffset, int size)
        {
            if (copyMemory is null || toMemory is null) throw new ArgumentNullException();
            if(copyMemoryOffset < 0 || toMempryOffset < 0 || size < 0) throw new ArgumentOutOfRangeException();
            if ((copyMemoryOffset + (long)size > copyMemory.LongLength) || (toMempryOffset + (long)size > toMemory.LongLength))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (size == 0) return;
            fixed (byte* copyPtr = copyMemory, toPtr = toMemory)
            {
                MemoryCopy(copyPtr + copyMemoryOffset, toPtr + toMempryOffset, size);
            }
        }

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块</param>
        /// <param name="toMemory">要拷贝到的内存位置</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        /// <exception cref="ArgumentOutOfRangeException">拷贝的字节小于0</exception>
        public static void MemoryCopy(this IntPtr copyMemory, IntPtr toMemory, int size)
        {
            if (size == 0) return;
            Buffer.MemoryCopy(copyMemory.ToPointer(), toMemory.ToPointer(), size, size);
        }

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块</param>
        /// <param name="toMemory">要拷贝到的内存位置</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        public static void MemoryCopy(void* copyMemory, void* toMemory, int size)
        {
            Buffer.MemoryCopy(copyMemory, toMemory, size, size);
        }

        /// <summary>
        /// 将两个内存块中的数据交换
        /// </summary>
        /// <param name="memory1">内存块1</param>
        /// <param name="memory2">内存块2</param>
        /// <param name="size">内存块字节大小</param>
        public static void MemorySwap(this IntPtr memory1, IntPtr memory2, int size)
        {
            if (size == 0) return;
            IntPtr temptr;
            if (size <= 128)
            {
                byte* temp = stackalloc byte[size];
                temptr = new IntPtr(temp);

                MemoryCopy(memory1, temptr, size);
                MemoryCopy(memory2, memory1, size);
                MemoryCopy(temptr, memory2, size);
                return;
            }

            byte[] buf = new byte[size];
            fixed (byte* bp = buf)
            {
                temptr = new IntPtr(bp);
                MemoryCopy(memory1, temptr, size);
                MemoryCopy(memory2, memory1, size);
                MemoryCopy(temptr, memory2, size);
            }

        }

        /// <summary>
        /// 将两个内存块中的数据交换
        /// </summary>
        /// <param name="memory1">内存块1</param>
        /// <param name="memory2">内存块2</param>
        /// <param name="size">两个内存块要交换的字节大小</param>
        /// <param name="tempBuffer">交换时使用的临时内存块，长度不得小于<paramref name="size"/></param>
        /// <exception cref="ArgumentOutOfRangeException">给定的内存块长度小于临时内存块</exception>
        /// <exception cref="ArgumentNullException">临时内存块为null</exception>
        public static void MemorySwap(this IntPtr memory1, IntPtr memory2, int size, byte[] tempBuffer)
        {
            if (tempBuffer is null) throw new ArgumentNullException();
            if (size > tempBuffer.Length) throw new ArgumentOutOfRangeException();

            if (size == 0) return;

            fixed (byte* bp = tempBuffer)
            {
                void* m1 = memory1.ToPointer();
                void* m2 = memory2.ToPointer();
                MemoryCopy(m1, bp, size);
                MemoryCopy(m2, m1, size);
                MemoryCopy(bp, m2, size);
            }

        }

        /// <summary>
        /// 将两个内存块中的数据交换
        /// </summary>
        /// <param name="memory1">内存块1</param>
        /// <param name="memory2">内存块2</param>
        /// <param name="size">两个内存块要交换的字节大小</param>
        /// <param name="tempBuffer">交换时使用的临时内存块，长度不得小于<paramref name="size"/></param>
        public static void MemorySwap(this IntPtr memory1, IntPtr memory2, int size, void* tempBuffer)
        {
            if (size == 0) return;

            void* m1 = memory1.ToPointer();
            void* m2 = memory2.ToPointer();
            MemoryCopy(m1, tempBuffer, size);
            MemoryCopy(m2, m1, size);
            MemoryCopy(tempBuffer, m2, size);
        }

        /// <summary>
        /// 将内存块的数据拷贝到另一个内存块中，并保证完整拷贝
        /// </summary>
        /// <param name="copyMemory">要待拷贝的内存</param>
        /// <param name="toMemory">将要拷贝到的目标内存首地址</param>
        /// <param name="size">要拷贝的字节大小</param>
        /// <exception cref="ArgumentNullException">内存块指针是null</exception>
        public static void MemoryCopyWhole(void* copyMemory, void* toMemory, int size)
        {
            if (copyMemory == null || toMemory == null) throw new ArgumentNullException();

            if (copyMemory == toMemory) return;
            Buffer.MemoryCopy(copyMemory, toMemory, size, size);
        }

        /// <summary>
        /// 将内存块的数据拷贝到另一个内存块中，并保证完整拷贝
        /// </summary>
        /// <param name="copyMemory">要待拷贝的内存</param>
        /// <param name="toMemory">将要拷贝到的目标内存首地址</param>
        /// <param name="size">要拷贝的字节大小</param>
        /// <exception cref="ArgumentNullException">内存块指针是null</exception>
        public static void MemoryCopyWhole(this IntPtr copyMemory, IntPtr toMemory, int size)
        {
            MemoryCopyWhole(copyMemory.ToPointer(), toMemory.ToPointer(), size);
        }

        #endregion

        #region 块

        /// <summary>
        /// 将指定内存区域的值全部清零
        /// </summary>
        /// <param name="buffer">要清空内存的首地址</param>
        /// <param name="size">要设置的长度，必须大于0</param>
        public static void ClearBuffer(void* buffer, int size)
        {
            if (size == 0) return;
            int i;
            byte* ptr = (byte*)buffer;

            const int bsizelen = 64;

            if(size < bsizelen)
            {
                for (i = 0; i < size; i++)
                {
                    ptr[i] = 0;
                }
                return;
            }

            var bsize = size / bsizelen;
            
            byte* buf64ptr = stackalloc byte[bsizelen];
            for (i = 0; i < bsizelen; i++)
            {
                buf64ptr[i] = 0;
            }

            for (i = 0; i < bsize; i++)
            {
                Buffer.MemoryCopy(buf64ptr, ptr + (i * bsizelen), size - (i * bsizelen), bsizelen);
            }

            bsize = size % bsizelen;
            if(bsize != 0)
            {
                ptr = ptr + (i * bsizelen);
                for (i = 0; i < bsize; i++)
                {
                    ptr[i] = 0;
                }
            }
        }

        /// <summary>
        /// 将指定内存区域的值全部清零
        /// </summary>
        /// <param name="buffer">要清空内存的首地址</param>
        /// <param name="size">要设置的长度</param>
        /// <exception cref="ArgumentOutOfRangeException">长度小于0</exception>
        /// <exception cref="ArgumentNullException">指针是null</exception>
        public static void ClearBuffer(IntPtr buffer, int size)
        {
            if (buffer == IntPtr.Zero) throw new ArgumentNullException(nameof(buffer));
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (size == 0) return;
            ClearBuffer(buffer.ToPointer(), size);
        }

        #endregion

    }

}
