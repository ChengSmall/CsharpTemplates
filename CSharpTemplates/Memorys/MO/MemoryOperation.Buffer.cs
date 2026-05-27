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

        #region 结构
        [StructLayout(LayoutKind.Sequential, Size = 32)]
        private struct byte32
        {
        }
        [StructLayout(LayoutKind.Sequential, Size = 128)]
        private struct byte128
        {
        }
        [StructLayout(LayoutKind.Sequential, Size = 256)]
        private struct byte256
        {
        }
        [StructLayout(LayoutKind.Sequential, Size = 1024)]
        private struct byte1024
        {
        }
        [StructLayout(LayoutKind.Sequential, Size = 8192)]
        private struct byte8192
        {
        }
        [StructLayout(LayoutKind.Sequential, Size = 1024 * 1024 * 4)]
        private struct mb4
        {
        }
        #endregion

        #region

        static void memoryCopyMB4(void* copy, void* to, int sizeMB4)
        {
            mb4* cp = (mb4*)copy;
            mb4* top = (mb4*)to;
            for (int i = 0; i < sizeMB4; i++)
            {
                top[i] = cp[i];
            }
        }
        static void memoryCopy8192(void* copy, void* to, int size8192)
        {
            byte8192* cp = (byte8192*)copy;
            byte8192* top = (byte8192*)to;
            for (int i = 0; i < size8192; i++)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy1024(void* copy, void* to, int size1024)
        {
            byte1024* cp = (byte1024*)copy;
            byte1024* top = (byte1024*)to;
            for (int i = 0; i < size1024; i++)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy128(void* copy, void* to, int size128)
        {
            byte128* cp = (byte128*)copy;
            byte128* top = (byte128*)to;
            for (int i = 0; i < size128; i++)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy32(void* copy, void* to, int size32)
        {
            byte32* cp = (byte32*)copy;
            byte32* top = (byte32*)to;
            for (int i = 0; i < size32; i++)
            {
                top[i] = cp[i];
            }
        }

        static void memoryCopyMB4last(void* copy, void* to, int sizeb)
        {
            mb4* cp = (mb4*)copy;
            mb4* top = (mb4*)to;

            int end = -sizeb;
            for (int i = -1; i >= end; i--)
            {
                top[i] = cp[i];
            }
        }
        static void memoryCopy8192last(void* copy, void* to, int sizeb)
        {
            byte8192* cp = (byte8192*)copy;
            byte8192* top = (byte8192*)to;
            int end = -sizeb;
            for (int i = -1; i >= end; i--)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy1024last(void* copy, void* to, int sizeb)
        {
            byte1024* cp = (byte1024*)copy;
            byte1024* top = (byte1024*)to;
            int end = -sizeb;
            for (int i = -1; i >= end; i--)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy128last(void* copy, void* to, int sizeb)
        {
            byte128* cp = (byte128*)copy;
            byte128* top = (byte128*)to;
            int end = -sizeb;
            for (int i = -1; i >= end; i--)
            {
                top[i] = cp[i];
            }
        }
        static void memroyCopy32last(void* copy, void* to, int sizeb)
        {
            byte32* cp = (byte32*)copy;
            byte32* top = (byte32*)to;
            int end = -sizeb;
            for (int i = -1; i >= end; i--)
            {
                top[i] = cp[i];
            }
        }

        #endregion

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块</param>
        /// <param name="toMemory">要拷贝到的内存位置</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        /// <exception cref="ArgumentOutOfRangeException">拷贝的字节小于0</exception>
        public static void MemoryCopy(this IntPtr copyMemory, IntPtr toMemory, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException();
            if (size == 0) return;
            MemoryCopy((void*)copyMemory, (void*)toMemory, size);
        }

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块</param>
        /// <param name="toMemory">要拷贝到的内存位置</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        public static void MemoryCopy(void* copyMemory, void* toMemory, int size)
        {
            const int mb4 = 1024 * 1024 * 4;
            //const int mb256 = 1024 * 1024 * 256;
            byte* copy = (byte*)copyMemory, to = (byte*)toMemory;

            //拷贝索引
            int copyByteIndex = 0;
            //倍率
            int sizeMagnitude;

            //大于32byte

            if (size > 128)
            {
                //大于128byte

                if (size > 1024)
                {

                    if (size > 8192)
                    {
                        //大于4MB

                        if (size > mb4)
                        {

                            sizeMagnitude = size / mb4;

                            memoryCopyMB4(copy + copyByteIndex, to + copyByteIndex, sizeMagnitude);

                            copyByteIndex += sizeMagnitude * mb4;
                            size = size % mb4;
                        }

                        //大于8192byte
                        sizeMagnitude = size / 8192;

                        memoryCopy8192(copy + copyByteIndex, to + copyByteIndex, sizeMagnitude);

                        copyByteIndex += sizeMagnitude * 8192;
                        size = size % 8192;
                    }


                    //大于1024byte，小于等于8192
                    sizeMagnitude = size / 1024;
                    memroyCopy1024(copy + copyByteIndex, to + copyByteIndex, sizeMagnitude);

                    copyByteIndex += sizeMagnitude * 1024;
                    size = size % 1024;
                }

                //大于128byte，小于1024
                sizeMagnitude = size / 128;
                memroyCopy128(copy + copyByteIndex, to + copyByteIndex, sizeMagnitude);

                copyByteIndex += sizeMagnitude * 128;
                size = size % 128;
            }


            //剩余或小于128
            copy += copyByteIndex;
            to += copyByteIndex;

            int i;

            int size4 = size / 4;
            for (i = 0; i < size4; i++)
            {
                *(((int*)to) + i) = *(((int*)copy) + i);
            }

            size = size % 4;

            if (size != 0)
            {
                int offset4 = size4 * 4;
                copy += offset4;
                to += offset4;

                for (i = 0; i < size; i++)
                {
                    to[i] = copy[i];
                }
            }

        }

        /// <summary>
        /// 将内存块拷贝到另一块内存当中，采用从后向前拷贝
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块起始位</param>
        /// <param name="toMemory">要拷贝到的内存起始位</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        /// <exception cref="ArgumentOutOfRangeException">拷贝的字节小于0</exception>
        public static void MemoryLastCopy(this IntPtr copyMemory, IntPtr toMemory, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException();
            if (size == 0) return;
            MemoryLastCopy((void*)copyMemory, (void*)toMemory, size);
        }

        /// <summary>
        /// 将内存块拷贝到另一块内存当中，采用从后向前拷贝
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块起始位</param>
        /// <param name="toMemory">要拷贝到的目标内存起始位</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        public static void MemoryLastCopy(void* copyMemory, void* toMemory, int size)
        {
            //const int mb4 = 1024 * 1024 * 4;
            byte* copy = (byte*)copyMemory, to = (byte*)toMemory;

            //倍率
            int sizeMagnitude;

            byte* endCopy, endTo;
            endCopy = copy + size;
            endTo = to + size;
            //大于32byte

            if (size > 128)
            {
                //大于128byte

                if (size > 1024)
                {

                    if (size > 8192)
                    {
                        //大于8192byte
                        sizeMagnitude = size / 8192;

                        memoryCopy8192last(endCopy, endTo, sizeMagnitude);

                        endCopy -= sizeMagnitude * 8192;
                        endTo -= sizeMagnitude * 8192;
                        size %= 8192;
                    }

                    //大于1024byte，小于等于8192
                    sizeMagnitude = size / 1024;
                    memroyCopy1024last(endCopy, endTo, sizeMagnitude);

                    endCopy -= sizeMagnitude * 1024;
                    endTo -= sizeMagnitude * 1024;
                    size %= 1024;
                }

                //大于128byte，小于1024
                sizeMagnitude = size / 128;
                memroyCopy128last(endCopy, endTo, sizeMagnitude);

                endCopy -= sizeMagnitude * 128;
                endTo -= sizeMagnitude * 128;
                size %= 128;
            }


            sizeMagnitude = size / 4;
            int lastb = -sizeMagnitude;
            int i;
            int* endToi = (int*)endTo;
            int* endCopyi = (int*)endCopy;
            for (i = -1; i >= lastb; i--)
            {
                endToi--;
                endCopyi--;
                *endToi = *endCopyi;
            }

            size %= 4;

            if (size != 0)
            {
                endCopy -= sizeMagnitude * 4;
                endTo -= sizeMagnitude * 4;
                if (size == 1)
                {
                    endTo[-1] = endCopy[-1];
                }
                else if (size == 2)
                {
                    endTo[-1] = endCopy[-1];
                    endTo[-2] = endCopy[-2];
                }
                else
                {
                    endTo[-1] = endCopy[-1];
                    endTo[-2] = endCopy[-2];
                    endTo[-3] = endCopy[-3];
                }
            }

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
            bool left;
            if(sizeof(void*) == 8)
            {
                left = (ulong)copyMemory < (ulong)toMemory;
            }
            else
            {
                left = (uint)copyMemory < (uint)toMemory;
            }
            if (left)
            {
                //原在左侧，目标在右侧，从右烤左
                MemoryLastCopy(copyMemory, toMemory, size);
            }
            else
            {
                //原在右侧，目标在左侧，从左烤右
                MemoryCopy(copyMemory, toMemory, size);
            }

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
        /// <param name="size">要设置的长度</param>
        /// <exception cref="ArgumentOutOfRangeException">长度小于0</exception>
        public static void ClearBuffer(void* buffer, int size)
        {
            if (size == 0) return;
            if (size < 0) throw new ArgumentOutOfRangeException();
            int blockIndex;
            int i;
            byte* ptr = (byte*)buffer;
            if (size > 256)
            {
                blockIndex = size / 256;
                byte256* p256 = (byte256*)ptr;
                for (i = 0; i < blockIndex; i++)
                {
                    *p256 = default(byte256);
                    p256++;
                }
                ptr = (byte*)p256;
            }

            var lastCount = size % 256;
            blockIndex = lastCount / 4;
            int* p4 = (int*)ptr;
            for (i = 0; i < blockIndex; i++)
            {
                *p4 = 0;
                p4++;
            }
            ptr = (byte*)p4;

            lastCount = lastCount % 4;

            if (lastCount > 0)
            {
                ptr[0] = 0;
                if(lastCount > 1)
                {
                    ptr[1] = 0;
                    if(lastCount > 2)
                    {
                        ptr[2] = 0;
                    }
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
            ClearBuffer(buffer.ToPointer(), size);
        }

        #endregion

    }


}
