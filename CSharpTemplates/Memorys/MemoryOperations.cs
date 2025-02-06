using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Cheng.Memorys
{

    /// <summary>
    /// 内存和流操作方法，此扩展方法内若没有注释说明，则默认未做安全检查
    /// </summary>
    public unsafe static class MemoryOperation
    {

        #region 内存方法

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
        [StructLayout(LayoutKind.Sequential, Size = 1024 * 1024 * 256)]
        private struct mb256
        {
        }
        #endregion

        static void memoryCopyMB256(void* copy, void* to, int sizeMB256)
        {
            mb256* cp = (mb256*)copy;
            mb256* top = (mb256*)to;
            for (int i = 0; i < sizeMB256; i++)
            {
                top[i] = cp[i];
            }
        }
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

        /// <summary>
        /// 将内存块拷贝到另一块内存当中
        /// </summary>
        /// <param name="copyMemory">要拷贝的内存块</param>
        /// <param name="toMemory">要拷贝到的内存位置</param>
        /// <param name="size">要拷贝的内存字节大小</param>
        /// <exception cref="ArgumentOutOfRangeException">拷贝的字节小于0</exception>
        public static void MemoryCopy(this IntPtr copyMemory, IntPtr toMemory, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size");
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
            //Buffer.MemoryCopy(copyMemory, toMemory, size, size);

            const int mb4 = 1024 * 1024 * 4;
            const int mb256 = 1024 * 1024 * 256;
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

                            if(size > mb256)
                            {

                                sizeMagnitude = size / mb256;

                                memoryCopyMB256(copy + copyByteIndex, to + copyByteIndex, sizeMagnitude);

                                copyByteIndex += sizeMagnitude * mb256;
                                size = size % mb256;

                            }

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
            int offset4 = size4 * 4;
            copy += offset4;
            to += offset4;

            if (size != 0)
            {
                for (i = 0; i < size; i++)
                {
                    to[i] = copy[i];
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
            if (size <= 64)
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

        #endregion

        #region 字节数据

        /// <summary>
        /// 将指定变量的地址以另一个类型变量访问
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="value">变量</param>
        /// <returns>
        /// 变量<paramref name="value"/>所在内存的新访问类型
        /// <para>等价于 *(<typeparamref name="R"/>*)<![CDATA[&]]><paramref name="value"/></para>
        /// </returns>
        public static R ToValue<T, R>(this T value) where T : unmanaged where R : unmanaged
        {
            if (sizeof(T) < sizeof(R))
            {
                R r = default;
                MemoryCopy(&value, &r, sizeof(T));
                return r;
            }

            return *(R*)&value;
        }

        /// <summary>
        /// 将非托管数据写入到字节数组
        /// </summary>
        /// <remarks>
        /// 此函数将非托管变量的内存转化到给定的字节数组当中，转化方式以此程序内的内存为基准
        /// </remarks>
        /// <typeparam name="T">要转化的类型</typeparam>
        /// <param name="value">要转化的数据</param>
        /// <param name="buffer">要转化到的字节数组，必须保证给定数组的写入长度大于或等于类型大小</param>
        /// <param name="index">要转化到的字节数组的起始位置</param>
        public static void ToByteArray<T>(this T value, byte[] buffer, int index) where T : unmanaged
        {
            fixed (byte* bp = buffer)
            {
                *((T*)(bp + index)) = value;
            }
        }

        /// <summary>
        /// 将非托管数据写入到字节数组
        /// </summary>
        /// <remarks>
        /// 此函数将非托管变量的内存转化到给定的字节数组当中，转化方式以此程序内的内存为基准
        /// </remarks>
        /// <typeparam name="T">要转化的类型</typeparam>
        /// <param name="value">要转化的数据</param>
        /// <param name="buffer">要转化到的字节数组，必须保证给定数组的写入长度大于或等于类型大小</param>
        public static void ToByteArray<T>(this T value, byte[] buffer) where T : unmanaged
        {
            fixed (byte* bp = buffer)
            {
                *((T*)bp) = value;
            }
        }

        /// <summary>
        /// 从字节数组的内存中获取指定类型的非托管数据
        /// </summary>
        /// <remarks>
        /// 从给定数组的位置，获取指定类型<typeparamref name="T"/>长度大小的字节，并以该类型返回数据
        /// </remarks>
        /// <typeparam name="T">转化的类型</typeparam>
        /// <param name="buffer">要转化的数组，必须保证给定数组的长度大于或等于类型大小</param>
        /// <param name="index">要转化的字节数组的起始位置</param>
        /// <returns>转化后的数据</returns>
        public static T ToStructure<T>(this byte[] buffer, int index) where T : unmanaged
        {
            fixed (byte* bp = buffer)
            {
                return (*((T*)(bp + index)));
            }
        }

        /// <summary>
        /// 从字节数组的内存中获取指定类型的非托管数据
        /// </summary>
        /// <remarks>
        /// 从给定数组的位置，获取指定类型<typeparamref name="T"/>长度大小的字节，并以该类型返回数据
        /// </remarks>
        /// <typeparam name="T">转化的类型</typeparam>
        /// <param name="buffer">要转化的数组，必须保证给定数组的长度大于或等于类型大小</param>
        /// <returns>转化后的数据</returns>
        public static T ToStructure<T>(this byte[] buffer) where T : unmanaged
        {
            fixed (byte* bp = buffer)
            {
                return (*((T*)bp));
            }
        }

        /// <summary>
        /// 将字符串内存数据直接作为字节数组返回
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="index">字符串要转化的的起始位置</param>
        /// <returns>给定字符串的内存字节；若字符串为null则直接返回null</returns>
        public static byte[] ToByteArray(this string str, int index)
        {
            if (str is null) return null;
            int size = str.Length * sizeof(char);
            byte[] buf = new byte[size];

            fixed (void* bp = buf, strp = str)
            {
                MemoryCopy(new IntPtr((((char*)strp) + index)), new IntPtr(bp), size);
            }
            return buf;
        }

        /// <summary>
        /// 将字符串内存数据直接作为字节数组返回
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>给定字符串的内存字节；若字符串为null则直接返回null</returns>
        public static byte[] ToByteArray(this string str)
        {
            if (str is null) return null;
            int size = str.Length * sizeof(char);
            byte[] buf = new byte[size];

            fixed (void* bp = buf, strp = str)
            {
                MemoryCopy(((char*)strp), bp, size);
            }
            return buf;
        }

        /// <summary>
        /// 将字符串内存数据写入为字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index">字符串要转化的的起始位置</param>
        /// <param name="buffer">写入到的字节数组</param>
        /// <param name="offset">字节数组的起始位置</param>
        public static void ToByteArray(this string str, int index, byte[] buffer, int offset)
        {
            if (str is null) throw new ArgumentNullException();
            int size = str.Length * sizeof(char);

            fixed (void* bp = buffer, strp = str)
            {
                MemoryCopy(new IntPtr((((char*)strp) + index)), new IntPtr(((byte*)bp) + offset), size);
            }
        }

        /// <summary>
        /// 将字节数组中的内存直接转化为字符串
        /// </summary>
        /// <param name="buffer">要转化的字节数组</param>
        /// <param name="index">要转化的字节数组起始位置</param>
        /// <param name="count">要转化到字符串的字符数</param>
        /// <returns>转化的字符串</returns>
        public static string ToStringBuffer(this byte[] buffer, int index, int count)
        {
            int length = buffer.Length;

            fixed(byte* p = buffer)
            {
                return new string((char*)(p + index), 0, count);
            }
        }

        /// <summary>
        /// 将字节数组中的内存直接转化为字符串
        /// </summary>
        /// <param name="buffer">要转化的字节数组</param>
        /// <param name="index">要转化的字节数组起始位置</param>
        /// <returns>转化的字符串</returns>
        public static string ToStringBuffer(this byte[] buffer, int index)
        {
            int length = buffer.Length;

            fixed (byte* p = buffer)
            {
                int count = (buffer.Length - index) / 2;
                return new string((char*)(p + index), 0, count);
            }
        }

        /// <summary>
        /// 将指定的非托管内存缓冲区写入到另一个非托管内存缓冲区当中
        /// </summary>
        /// <typeparam name="T">原缓冲区数组类型</typeparam>
        /// <typeparam name="TO">目标缓冲区数组类型</typeparam>
        /// <param name="buffer">原缓冲区</param>
        /// <param name="offset">原缓冲区要拷贝的字节偏移</param>
        /// <param name="toBuffer">要拷贝到的目标缓冲区</param>
        /// <param name="toOffset">要拷贝到的缓冲区的起始偏移</param>
        /// <param name="copyByteSize">要拷贝的字节数量</param>
        public static void CopyBufferArray<T, TO>(this T[] buffer, int offset, TO[] toBuffer, int toOffset, int copyByteSize) where T : unmanaged where TO : unmanaged
        {
            
            fixed (void* fi_buf = buffer, fi_toBuf = toBuffer)
            {

                MemoryCopy(((byte*)fi_buf) + offset, ((byte*)fi_toBuf) + toOffset, copyByteSize);

            }

        }

        /// <summary>
        /// 将非托管数据写入到字节数组
        /// </summary>
        /// <remarks>
        /// 此函数将非托管变量的内存转化到给定的字节数组当中，转化方式以此程序内的内存为基准
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要转化的数据</param>
        /// <param name="buffer">要转化到的字节数组，必须保证给定数组的写入长度大于或等于类型大小</param>
        /// <param name="index">要转化到的字节数组的起始位置</param>
        /// <param name="endIndex">转化完毕后根据起始索引向后推进到新位置的第一位索引，流数据位置模拟</param>
        public static void ToByteArray<T>(this T value, byte[] buffer, int index, out int endIndex) where T : unmanaged
        {
            endIndex = sizeof(T) + index;
            ToByteArray(value, buffer, index);
        }

        /// <summary>
        /// 从字节数组的内存中获取指定类型的非托管数据
        /// </summary>
        /// <remarks>
        /// 从给定数组的位置，获取指定类型<typeparamref name="T"/>长度大小的字节，并以该类型返回数据
        /// </remarks>
        /// <typeparam name="T">转化的类型</typeparam>
        /// <param name="buffer">要转化的数组，必须保证给定数组的长度大于或等于类型大小</param>
        /// <param name="index">要转化的字节数组的起始位置</param>
        /// <param name="endIndex">转化完毕后的索引位置，流数据位置模拟</param>
        /// <returns>转化后的数据</returns>
        public static T ToStructure<T>(this byte[] buffer, int index, out int endIndex) where T : unmanaged
        {
            endIndex = sizeof(T) + index;
            return ToStructure<T>(buffer, index);
        }
        

        #endregion

        #region 位域

        /// <summary>
        /// 获取单字节指定位域的值
        /// </summary>
        /// <param name="b">字节</param>
        /// <param name="index">位域索引，范围[0,7]</param>
        /// <returns>指定位域的值，1返回true，0返回false</returns>
        public static bool ByteBit(this byte b, int index)
        {
            return ((b >> index) & 1) == 1;
        }
        /// <summary>
        /// 设置单字节指定位域的值
        /// </summary>
        /// <param name="b">字节引用</param>
        /// <param name="index">位域索引，范围[0,7]</param>
        /// <param name="value">设置到指定位域的值，true表示设置为1，false表示设置为0</param>
        public static void ByteBit(this ref byte b, int index, bool value)
        {
            if(value) b |= (byte)(1 << index);
            else b &= (byte)(~((byte)(1 << index)));
        }
        /// <summary>
        /// 获取指定非托管内存的某一字节引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">值</param>
        /// <param name="index">字节偏移</param>
        /// <returns>指定偏移的字节引用</returns>
        public static ref byte RefByte<T>(this ref T value, int index) where T : unmanaged
        {
            fixed (T* p = &value)
            {
                return ref *(((byte*)p) + index);
            }
        }
        /// <summary>
        /// 获取指定内存的某一字节引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ptrAddress">内存所在地址</param>
        /// <param name="index">字节偏移</param>
        /// <returns>指定偏移的字节引用</returns>
        public static ref byte RefPtrByte(this IntPtr ptrAddress, int index)
        {
            return ref *(((byte*)ptrAddress) + index);
        }
        /// <summary>
        /// 按位访问bit状态
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="offset">要访问的偏移，从0到127表示从低到高的比特位</param>
        /// <returns>比特位状态，true表示1，false表示0</returns>
        public static bool BitOffset(this ulong value, int offset)
        {
            return ((value >> offset) & 1) == 1;
        }

        /// <summary>
        /// 按位设置bit状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">要访问的偏移，从0到127表示从低到高的比特位</param>
        /// <param name="setValue">要设置的值，true表示1，false表示0</param>
        public static void BitOffset(this ref ulong value, int offset, bool setValue)
        {
            if (setValue) value |= (ulong)1 << offset;
            else value &= ~((ulong)1 << offset);
        }

        /// <summary>
        /// 按位访问bit状态
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="offset">要访问的偏移，从0到127表示从低到高的比特位</param>
        /// <returns>比特位状态，true表示1，false表示0</returns>
        public static bool BitOffset(this uint value, int offset)
        {
            return ((value >> offset) & 1) == 1;
        }

        /// <summary>
        /// 按位设置bit状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">要访问的偏移，从0到127表示从低到高的比特位</param>
        /// <param name="setValue">要设置的值，true表示1，false表示0</param>
        public static void BitOffset(this ref uint value, int offset, bool setValue)
        {
            if (setValue) value |= (uint)1 << offset;
            else value &= ~((uint)1 << offset);
        }

        /// <summary>
        /// 判断值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool IsAndBit(this uint value, uint bitOf)
        {
            return (value & bitOf) == bitOf;
        }

        /// <summary>
        /// 判断值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool IsAndBit(this byte value, byte bitOf)
        {
            return (value & bitOf) == bitOf;
        }

        /// <summary>
        /// 判断值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool IsAndBit(this ushort value, ushort bitOf)
        {
            return (value & bitOf) == bitOf;
        }

        /// <summary>
        /// 判断值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool IsAndBit(this ulong value, ulong bitOf)
        {
            return (value & bitOf) == bitOf;
        }

        /// <summary>
        /// 判断枚举值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool IsAndEnumBit<T>(this T value, T bitOf) where T : unmanaged, global::System.Enum
        {

            switch (sizeof(T))
            {
                case 8:
                    return IsAndBit(*((ulong*)&value), *((ulong*)&bitOf));
                case 4:
                    return IsAndBit(*((uint*)&value), *((uint*)&bitOf));
                case 2:
                    return IsAndBit(*((ushort*)&value), *((ushort*)&bitOf));
                default:
                    return IsAndBit(*((byte*)&value), *((byte*)&bitOf));
            }
           
        }

        #endregion

        #region 大小端

        /// <summary>
        /// 判断当前程序运行环境是否为大端存储
        /// </summary>
        /// <returns>返回true表示大端存储，false表示小端存储</returns>
        public static bool IsBigEndian
        {
            get
            {
                ushort i = 1;
                return (*(byte*)&i) == 0;
            }
        }

        /// <summary>
        /// 进行大小端转化
        /// </summary>
        /// <param name="bytes">要转化的数据</param>
        /// <param name="re">转化到的数据</param>
        public static void StorageConversionByte(this byte[] bytes, byte[] re)
        {
            int length = bytes.Length;
            for (int i = 0, j = length - 1; i < length; i++, j--)
            {
                re[i] = bytes[j];
            }
        }

        /// <summary>
        /// 进行位域反转
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ConversionStorageBit(this byte value)
        {
            byte re = 0;
            const int length = 8;

            for (int i = 0; i < length; i++)
            {
                re.ByteBit(7 - i, (((value >> i) & 1) == 1));
            }
            //(((value >> i) & 1) == 1)

            return re;
        }

        /// <summary>
        /// 进行大小端和位域转化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="re"></param>
        public static void StorageConversionAll(this byte[] bytes, byte[] re)
        {
            int length = bytes.Length;
            for (int i = 0, j = length - 1; i < length; i++, j--)
            {
                re[i] = ConversionStorageBit(bytes[i]);
            }
        }

        /// <summary>
        /// 进行大小端转化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要转化的数据</param>
        /// <returns>转化后的数据</returns>
        public static T StorageConversionByte<T>(this T value) where T : unmanaged
        {
            T temp;
            int length = sizeof(T);
            int end = sizeof(T) - 1;
            byte* firstp = (byte*)&value;
            byte* endp = (((byte*)&temp) + end);
            int i;

            for (i = 0; i < length; i++)
            {
                endp[end - i] = firstp[i];
            }

            return temp;
        }

        /// <summary>
        /// 进行大小端和位域转化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要转化的数据</param>
        /// <returns>转化后的数据</returns>
        public static T StorageConversionAll<T>(this T value) where T : unmanaged
        {

            T temp;
            int length = sizeof(T);
            int end = sizeof(T) - 1;
            byte* firstp = (byte*)&value;
            byte* endp = (((byte*)&temp) + end);
            int i;
            for (i = 0; i < length; i++)
            {
                endp[end - i] = ConversionStorageBit(firstp[i]);
            }

            return temp;

        }

        #endregion

        #region 流数据

        /// <summary>
        /// 完整读取流数据的字节序列
        /// </summary>
        /// <remarks>
        /// <para>此函数会不断调用<see cref="Stream.Read(byte[], int, int)"/>读取数据，直至读取的字节数等于参数<paramref name="count"/>或流内无法读取</para>
        /// </remarks>
        /// <param name="stream">读取的流</param>
        /// <param name="buffer">读取到的缓冲区</param>
        /// <param name="offset">缓冲区存放的起始索引</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>实际读取的字节数量；若返回的值小于<paramref name="count"/>表示剩余字节数小于要读取的字节数，返回0表示流已到达结尾</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">给定参数超出范围</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定参数超出范围</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">不支持方法</exception>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public static int ReadBlock(this Stream stream, byte[] buffer, int offset, int count)
        {
            if(stream is null) throw new ArgumentNullException(nameof(stream));
            int index = offset;
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.Read(buffer, index, count);
                if (rsize == 0) return re;
                index += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        /// <summary>
        /// 使用函数枚举器完整读取流数据的字节序列
        /// </summary>
        /// <remarks>
        /// <para>此函数每次推进会调用一次<see cref="Stream.Read(byte[], int, int)"/>读取数据，直至读取的字节数等于参数<paramref name="count"/>或流内无法读取</para>
        /// </remarks>
        /// <param name="stream">读取的流</param>
        /// <param name="buffer">读取到的缓冲区</param>
        /// <param name="offset">缓冲区存放的起始索引</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>返回一个函数枚举器，每次推进后返回此次读取的字节数量</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">给定参数超出范围</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定参数超出范围</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">不支持方法</exception>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public static IEnumerable<int> ReadBlockEnumator(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);

            return f_ReadBlockEnumator(stream, buffer, offset, count);
        }

        internal static IEnumerable<int> f_ReadBlockEnumator(Stream stream, byte[] buffer, int offset, int count)
        {
            int index = offset;
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.Read(buffer, index, count);
                if (rsize == 0) yield break;
                index += rsize;
                count -= rsize;
                re += rsize;
                yield return rsize;
            }
        }

        /// <summary>
        /// 将流数据读取并拷贝到另一个流当中
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static void CopyToStream(this Stream stream, Stream toStream, byte[] buffer)
        {
            if (stream is null || toStream is null || buffer is null) throw new ArgumentNullException();

            if (buffer.Length == 0) throw new ArgumentException("给定缓冲区长度为0");

            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException("流不支持读取或写入");

            copyToStream(stream, toStream, buffer);
        }

        /// <summary>
        /// 将流数据读取并拷贝到另一个流当中
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <returns>一个枚举器，每次推进都会读写指定字节的数据，并把此次拷贝的字节量返回到枚举值当中</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static IEnumerable<int> CopyToStreamEnumator(this Stream stream, Stream toStream, byte[] buffer)
        {
            if (stream is null || toStream is null || buffer is null) throw new ArgumentNullException();

            if (buffer.Length == 0) throw new ArgumentException("给定缓冲区长度为0");

            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException("流不支持读取或写入");

            return copyToStreamEnr(stream, toStream, buffer, 0);
        }

        /// <summary>
        /// 将流数据读取并拷贝到另一个流当中
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <param name="maxBytes">指定最大拷贝字节量，0表示不指定最大字节量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static void CopyToStream(this Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            if (stream is null || toStream is null || buffer is null) throw new ArgumentNullException();

            if (buffer.Length == 0) throw new ArgumentException("给定缓冲区长度为0");

            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException("流不支持读取或写入");

            copyToStream(stream, toStream, buffer, maxBytes);
        }

        /// <summary>
        /// 将流数据读取并拷贝到另一个流当中
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <param name="maxBytes">指定最大拷贝字节量，0表示不指定最大字节量</param>
        /// <returns>一个枚举器，每次推进都会读写指定字节的数据，并把此次拷贝的字节量返回到枚举值当中</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static IEnumerable<int> CopyToStreamEnumator(this Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            if (stream is null || toStream is null || buffer is null) throw new ArgumentNullException();

            if (buffer.Length == 0) throw new ArgumentException("给定缓冲区长度为0");

            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException("流不支持读取或写入");

            return copyToStreamEnr(stream, toStream, buffer, maxBytes);
        }

        static void copyToStream(Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            int length = buffer.Length;
            int rsize;
            int reas;
            ulong isReadSize;

            if(maxBytes == 0)
            {
                BeginLoop:
                rsize = stream.Read(buffer, 0, length);

                if (rsize == 0) return;

                toStream.Write(buffer, 0, rsize);

                goto BeginLoop;
            }

            isReadSize = 0;

            nBeginLoop:

            if (isReadSize == maxBytes) return;

            if ((isReadSize + (ulong)length) > maxBytes)
            {
                reas = (int)(maxBytes - isReadSize);
            }
            else reas = length;

            rsize = stream.Read(buffer, 0, reas);

            if (rsize == 0) return;

            isReadSize += (ulong)rsize;
            toStream.Write(buffer, 0, rsize);

            goto nBeginLoop;

        }


        static void copyToStream(Stream stream, Stream toStream, byte[] buffer)
        {
            int length = buffer.Length;
            int rsize;
            //int reas;
            //ulong isReadSize;
            
            BeginLoop:
            rsize = stream.Read(buffer, 0, length);

            if (rsize == 0) return;

            toStream.Write(buffer, 0, rsize);

            goto BeginLoop;
            
        }

        static IEnumerable<int> copyToStreamEnr(Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            int length = buffer.Length;
            int rsize;
            int reas;
            ulong isReadSize;

            if (maxBytes == 0)
            {
                BeginLoop:
                rsize = stream.Read(buffer, 0, length);

                if (rsize == 0) yield break;

                toStream.Write(buffer, 0, rsize);
                yield return rsize;
                goto BeginLoop;
            }

            isReadSize = 0;

            nBeginLoop:

            if (isReadSize == maxBytes) yield break;

            if ((isReadSize + (ulong)length) > maxBytes)
            {
                reas = (int)(maxBytes - isReadSize);
            }
            else reas = length;

            rsize = stream.Read(buffer, 0, reas);

            if (rsize == 0) yield break;

            isReadSize += (ulong)rsize;
            toStream.Write(buffer, 0, rsize);
            yield return rsize;
            goto nBeginLoop;

        }

        /// <summary>
        /// 将流数据转化到字节数组
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="buffer">指定读取时的缓冲区</param>
        /// <returns>转化为字节数组的流数据</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static byte[] ToByteArray(this Stream stream, byte[] buffer)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();

            MemoryStream ms = new MemoryStream(buffer.Length);

            copyToStream(stream, ms, buffer, 0);
            var bs = ms.GetBuffer();
            if (bs.Length == ms.Length) return bs;
            return ms.ToArray();

        }

        /// <summary>
        /// 从流数据中读取指定类型的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="stream">流</param>
        /// <param name="buffer">读取时需要的缓冲区，长度必须大于类型<typeparamref name="T"/></param>
        /// <param name="value">读取到的变量</param>
        /// <returns>是否成功读取到或读取完整</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        public static bool ReadValue<T>(this Stream stream, byte[] buffer, out T value) where T : unmanaged
        {
            if (stream is null || buffer is null || buffer.Length < sizeof(T)) throw new ArgumentException();
            int ri;

            if (sizeof(T) == 1)
            {
                ri = stream.ReadByte();
                if (ri == -1)
                {
                    value = default;
                    return false;
                }
                value = *(T*)&ri;
            }

            ri = stream.ReadBlock(buffer, 0, sizeof(T));

            if(ri < sizeof(T))
            {
                value = default;
                return false;
            }

            value = buffer.ToStructure<T>();
            return true;
        }

        /// <summary>
        /// 将指定对象存储到流数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">流</param>
        /// <param name="buffer">写入流时的缓冲区，该缓冲区长度不得小于类型<typeparamref name="T"/>，后果自负</param>
        /// <param name="value">要存储的对象</param>
        public static void WriteValue<T>(this Stream stream, byte[] buffer, T value) where T : unmanaged
        {

            fixed (byte* bp = buffer)
            {
                *((T*)bp) = value;

                stream.Write(buffer, 0, sizeof(T));
            }
        }

        /// <summary>
        /// 将托管内存流的数据返回到字节数组
        /// </summary>
        /// <param name="memoryStream">内存流</param>
        /// <param name="buffer">储存原始字节数组的数据</param>
        /// <exception cref="ArgumentNullException">流是null</exception>
        public static bool TryGetBuffer(this MemoryStream memoryStream, out ArraySegment<byte> buffer)
        {
            if (memoryStream == null) throw new ArgumentNullException();
            try
            {
                var buf = memoryStream.GetBuffer();
                buffer = new ArraySegment<byte>(buf, 0, (int)memoryStream.Length);
                return true;
            }
            catch (Exception)
            {
                buffer = default;
                return false;
            }
            
        }

        #endregion

        #region 比较

        /// <summary>
        /// 比较两块内存中的数据是否相同
        /// </summary>
        /// <param name="ptr1">地址1</param>
        /// <param name="ptr2">地址2</param>
        /// <param name="length">比较的长度</param>
        /// <returns></returns>
        public static bool EqualsMemory(this IntPtr ptr1, IntPtr ptr2, int length)
        {
            long* xlp = (long*)ptr1, ylp = (long*)ptr2;
            if (xlp == ylp) return true;

            int i;
            int size8 = length / 8;
            int sf8 = length % 8;

            for (i = 0; i < size8; i++)
            {
                if ((*(xlp + i)) != (*(ylp + i))) return false;
            }

            if (sf8 != 0)
            {
                xlp += i;
                ylp += i;

                for (i = 0; i < sf8; i++)
                {
                    if ((*(((byte*)xlp) + i)) != (*(((byte*)ylp) + i))) return false;
                }

            }

            return true;
        }

        /// <summary>
        /// 比较两个字节数组的内存数据是否相等
        /// </summary>
        /// <param name="buffer1"></param>
        /// <param name="buffer2"></param>
        /// <returns>当两个字节数组的元素内容全部相同时，返回true，否则返回false；若两个参数全部是null，则返回true</returns>
        public static bool EqualsBytes(this byte[] buffer1, byte[] buffer2)
        {
            if (buffer1 == buffer2) return true;
            if (buffer1 is null || buffer2 is null) return false;

            int length = buffer1.Length;
            if (length != buffer2.Length) return false;

            fixed(byte* bp = buffer1, bp2 = buffer2)
            {
                return EqualsMemory(new IntPtr(bp), new IntPtr(bp2), length);
            }
        }

        #endregion

        #region 指针

        /// <summary>
        /// 将指针解引用
        /// </summary>
        /// <typeparam name="T">表示解引用的类型</typeparam>
        /// <param name="ptrAddress">表示一个存储地址的指针变量</param>
        /// <returns>指针指向的值</returns>
        public static T PtrDef<T>(this IntPtr ptrAddress) where T : unmanaged
        {
            return (*(T*)ptrAddress);
        }

        /// <summary>
        /// 将指针解引用
        /// </summary>
        /// <typeparam name="T">表示解引用的类型</typeparam>
        /// <param name="ptrAddress">表示一个存储地址的指针变量</param>
        /// <param name="value">为指针指向的地址写入的新值</param>
        public static void PtrDef<T>(this IntPtr ptrAddress, T value) where T : unmanaged
        {
            *((T*)ptrAddress) = value;
        }

        /// <summary>
        /// 将指针解引用
        /// </summary>
        /// <typeparam name="T">表示解引用的类型</typeparam>
        /// <param name="ptrAddress">表示一个存储地址的指针变量</param>
        /// <returns>使用指针指向的地址访问内存并以<typeparamref name="T"/>类型变量返回值</returns>
        public static ref T PtrDefRef<T>(this IntPtr ptrAddress) where T : unmanaged
        {
            return ref (*(T*)ptrAddress);
        }

        /// <summary>
        /// 将指针解引用
        /// </summary>
        /// <typeparam name="R">表示一个指针的类型</typeparam>
        /// <typeparam name="T">表示解引用的类型</typeparam>
        /// <param name="ptrAddress">表示一个存储地址的指针变量</param>
        /// <returns>使用指针指向的地址访问内存并以<typeparamref name="T"/>类型变量返回值</returns>
        public static ref T PtrDef<R, T>(this R ptrAddress) where R : unmanaged where T : unmanaged
        {
            return ref *(*(T**)&ptrAddress);
        }

        /// <summary>
        /// 将指针解引用
        /// </summary>
        /// <typeparam name="R">表示一个指针的类型</typeparam>
        /// <typeparam name="T">表示解引用的类型</typeparam>
        /// <param name="ptrAddress">表示一个存储地址的指针变量</param>
        /// <param name="value">使用指针指向的地址访问内存并以<typeparamref name="T"/>类型变量赋值</param>
        public static void PtrDef<R, T>(this R ptrAddress, T value) where R : unmanaged where T : unmanaged
        {
            *(*(T**)&ptrAddress) = value;
        }

        /// <summary>
        /// 返回添加指定偏移的新指针
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="offset">字节偏移</param>
        /// <returns>新指针</returns>
        public static IntPtr AddOffset(this IntPtr ptr, int offset)
        {
            return new IntPtr(((byte*)ptr) + offset);
        }

        /// <summary>
        /// 返回添加指定偏移的新指针
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="offset">字节偏移</param>
        /// <returns>新指针</returns>
        public static UIntPtr AddOffset(this UIntPtr ptr, int offset)
        {
            return new UIntPtr(((byte*)ptr) + offset);
        }

        /// <summary>
        /// 返回添加指定偏移的新指针
        /// </summary>
        /// <typeparam name="TP">指针类型变量</typeparam>
        /// <param name="ptr">指针</param>
        /// <param name="offset">字节偏移</param>
        /// <returns>新指针</returns>
        public static TP AddOffset<TP>(this TP ptr, int offset) where TP : unmanaged
        {
            byte* bp = (*(byte**)&ptr) + offset;
            return *((TP*)&bp);
        }

        /// <summary>
        /// 将单浮点按内存返回为32位整数
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <returns>返回的整数</returns>
        public static uint ToInt(this float value)
        {
            return *(uint*)&value;
        }

        /// <summary>
        /// 将整数按内存返回为浮点数
        /// </summary>
        /// <param name="value">整数</param>
        /// <returns>浮点数</returns>
        public static float ToFloat(this uint value)
        {
            return *(float*)&value;
        }

        /// <summary>
        /// 将双浮点按内存返回为32位整数
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <returns>返回的整数</returns>
        public static ulong ToInt(this double value)
        {
            return *(ulong*)&value;
        }

        /// <summary>
        /// 将整数按内存返回为浮点数
        /// </summary>
        /// <param name="value">整数</param>
        /// <returns>浮点数</returns>
        public static double ToDouble(this ulong value)
        {
            return *(double*)&value;
        }

        #endregion

        #endregion

        #region 字符操作

        /// <summary>
        /// 将字符串的字母转化到小写
        /// </summary>
        /// <param name="originCharptr">原字符串地址</param>
        /// <param name="toCharptr">转化后写入的位置</param>
        /// <param name="length">字符串转化的字符数</param>
        public static void ToLopper(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const byte cbit = 0b00100000;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];

                if (c >= 'A' && c <= 'Z')
                {
                    c |= (char)cbit;
                }

                toCharptr[i] = c;
            }
        }

        /// <summary>
        /// 将字符串的字母转化到大写
        /// </summary>
        /// <param name="originCharptr">原字符串地址</param>
        /// <param name="toCharptr">转化后写入的位置</param>
        /// <param name="length">字符串转化的字符数</param>
        public static void ToUpper(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const ushort cbit = 0b11111111_11011111;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];

                if (c >= 'a' && c <= 'z')
                {
                    c &= (char)cbit;
                    //toCharptr[i] = c;
                }

                toCharptr[i] = c;
            }
        }


        #endregion

    }

}
