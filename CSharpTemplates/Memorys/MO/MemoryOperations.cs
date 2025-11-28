using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Cheng.Streams;

namespace Cheng.Memorys
{

    /// <summary>
    /// 内存和流的扩展功能
    /// </summary>
    public unsafe static partial class MemoryOperation
    {

        #region 内存方法

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
            if (setValue) value |= 1UL << offset;
            else value &= ~(1UL << offset);
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
            if (setValue) value |= 1U << offset;
            else value &= ~(1U << offset);
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
            return HasFlag(value, bitOf);
        }

        /// <summary>
        /// 判断枚举值的位域信息是否包含<paramref name="bitOf"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">判断的值</param>
        /// <param name="bitOf">要判断的位域信息</param>
        /// <returns><paramref name="value"/>中包含<paramref name="bitOf"/>内的所有位值返回true，否则返回false</returns>
        public static bool HasFlag<T>(this T value, T bitOf) where T : unmanaged, global::System.Enum
        {
            switch (sizeof(T))
            {
                case 8:
                    //return IsAndBit(*((ulong*)&value), *((ulong*)&bitOf));
                    return ((*((ulong*)&value)) & (*((ulong*)&bitOf))) == (*((ulong*)&bitOf));
                case 4:
                    //return IsAndBit(*((uint*)&value), *((uint*)&bitOf));
                    return ((*((uint*)&value)) & (*((uint*)&bitOf))) == (*((uint*)&bitOf));
                case 2:
                    //return IsAndBit(*((ushort*)&value), *((ushort*)&bitOf));
                    return ((*((ushort*)&value)) & (*((ushort*)&bitOf))) == (*((ushort*)&bitOf));
                default:
                    //return IsAndBit(*((byte*)&value), *((byte*)&bitOf));
                    return ((*((byte*)&value)) & (*((byte*)&bitOf))) == (*((byte*)&bitOf));
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
            
            return EqualsMemory(ptr1.ToPointer(), ptr2.ToPointer(), length);
        }

        /// <summary>
        /// 比较两块内存中的数据是否相同
        /// </summary>
        /// <param name="ptr1">地址1</param>
        /// <param name="ptr2">地址2</param>
        /// <param name="length">比较的长度</param>
        /// <returns></returns>
        public static bool EqualsMemory(void* ptr1, void* ptr2, int length)
        {
            if (length == 0) return true;
            var lenM4 = length / 4;

            int* ia = (int*)ptr1;
            int* ib = (int*)ptr2;
            int i;
            for (i = 0; i < lenM4; i++)
            {
                if (ia[i] != ib[i]) return false;
            }

            byte* ba = (byte*)(ia + i);
            byte* bb = (byte*)(ib + i);

            var ms = length % 4;
            if (ms != 0)
            {
                //至少是1
                if (ba[0] != bb[0]) return false;
                if (ms > 1)
                {
                    //2 或 3
                    if (ba[1] != bb[1]) return false;
                    //是3
                    if (ms == 3) if (ba[2] != bb[2]) return false;
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
                return EqualsMemory(bp, bp2, length);
            }
        }

        /// <summary>
        /// 比较两个字节数组的指定范围内存数据是否相等
        /// </summary>
        /// <param name="buffer1">要比较的字节数组1</param>
        /// <param name="offset"><paramref name="buffer1"/>从0开始的内存偏移</param>
        /// <param name="buffer2">要比较的字节数组1</param>
        /// <param name="offset2"><paramref name="buffer2"/>从0开始的内存偏移</param>
        /// <param name="count">要比较的数量</param>
        /// <returns>如果给定范围是数据完全一致，返回true；不一致返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定内存范围超出数组范围</exception>
        public static bool EqualsBytes(this byte[] buffer1, int offset, byte[] buffer2, int offset2, int count)
        {
            if (buffer1 is null || buffer2 is null) throw new ArgumentNullException();

            if ((offset < 0 || offset2 < 0) || (offset + count > buffer1.Length || offset2 + count > buffer2.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bp = buffer1, bp2 = buffer2)
            {
                return EqualsMemory(bp + offset, bp2 + offset2, count);
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

    }

}
