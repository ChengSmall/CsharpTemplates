using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;
using System.Runtime;
using Cheng.DataStructure.Hashs;
using Cheng.Memorys;

namespace Cheng.IO
{

    /// <summary>
    /// IO扩展操作
    /// </summary>
    public unsafe static partial class IOoperations
    {

        #region 顺序字节转化

        #region 转为流

        #region 指针

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this uint value, byte* buffer)
        {
            const int size = sizeof(uint);

            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)((value >> (i * 8)) & byte.MaxValue);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this int value, byte* buffer)
        {
            OrderToBytes((uint)value, buffer);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this long value, byte* buffer)
        {
            OrderToBytes((ulong)value, buffer);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this ulong value, byte* buffer)
        {
            const int size = sizeof(ulong);

            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)((value >> (i * 8)) & byte.MaxValue);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this ushort value, byte* buffer)
        {            
            buffer[0] = (byte)((value) & byte.MaxValue);
            buffer[1] = (byte)((value >> (1 * 8)) & byte.MaxValue);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this char value, byte* buffer)
        {
            buffer[0] = (byte)((value) & byte.MaxValue);
            buffer[1] = (byte)((value >> (1 * 8)) & byte.MaxValue);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this short value, byte* buffer)
        {
            buffer[0] = (byte)((value) & byte.MaxValue);
            buffer[1] = (byte)((((ushort)value) >> (8)) & byte.MaxValue);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this float value, byte* buffer)
        {
            var t = value;
            OrderToBytes(*(uint*)&t, buffer);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this double value, byte* buffer)
        {
            var t = value;
            OrderToBytes(*(ulong*)&t, buffer);
        }


        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this uint value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this int value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this long value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this ulong value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this ushort value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this char value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this short value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this float value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this double value, CPtr<byte> buffer)
        {
            OrderToBytes(value, buffer.p_ptr);
        }

        #endregion

        #region 字节数组

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this uint value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(uint) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this int value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(int) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this long value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(long) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this ulong value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(ulong) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this short value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(short) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this ushort value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(ushort) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this char value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(char) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this float value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(float) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        /// <summary>
        /// 将值顺序转化到指定的字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的字节数组</param>
        /// <param name="index">要转化到的数组的起始位置索引</param>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组的内存不足以存放值</exception>
        public static void OrderToByteArray(this double value, byte[] buffer, int index)
        {

            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (index + sizeof(double) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                OrderToBytes(value, bptr + index);
            }
        }

        #endregion

        #endregion

        #region 转回值

        #region 指针

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static uint OrderToUInt32(this IntPtr buffer)
        {
            byte* bptr = (byte*)buffer;
            return ((uint)bptr[0] | ((uint)bptr[1] << 8) | (((uint)bptr[2]) << (16)) | (((uint)bptr[3]) << (24)));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static ulong OrderToUInt64(this IntPtr buffer)
        {
            byte* bptr = (byte*)buffer;

            return (((ulong)bptr[0]) | ((ulong)bptr[1] << 8) | (((ulong)bptr[2]) << (8 * 2)) | (((ulong)bptr[3]) << (8 * 3)) | (((ulong)bptr[4]) << (8 * 4)) | ((ulong)bptr[5] << (8 * 5)) | (((ulong)bptr[6]) << (8 * 6)) | (((ulong)bptr[7]) << (8 * 7)));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static int OrderToInt32(this IntPtr buffer)
        {
            return (int)OrderToUInt32(buffer);
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static long OrderToInt64(this IntPtr buffer)
        {
            return (long)OrderToUInt64(buffer);
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static float OrderToFloat(this IntPtr buffer)
        {
            var t = OrderToUInt32(buffer);
            return *(float*)&t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static double OrderToDouble(this IntPtr buffer)
        {
            var t = OrderToUInt64(buffer);
            return *(double*)&t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要2字节</param>
        /// <returns>转化后的值</returns>
        public static ushort OrderToUInt16(this IntPtr buffer)
        {
            byte* bptr = (byte*)buffer;
            return (ushort)(bptr[0] | ((bptr[1]) << 8));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要2字节</param>
        /// <returns>转化后的值</returns>
        public static short OrderToInt16(this IntPtr buffer)
        {
            byte* bptr = (byte*)buffer;
            return (short)(bptr[0] | ((bptr[1]) << 8));
        }


        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static uint OrderToUInt32(this CPtr<byte> buffer)
        {
            byte* bptr = (byte*)buffer;
            return ((uint)bptr[0] | ((uint)bptr[1] << 8) | (((uint)bptr[2]) << (16)) | (((uint)bptr[3]) << (24)));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static ulong OrderToUInt64(this CPtr<byte> buffer)
        {
            byte* bptr = buffer.p_ptr;

            return (((ulong)bptr[0]) | ((ulong)bptr[1] << 8) | (((ulong)bptr[2]) << (8 * 2)) | (((ulong)bptr[3]) << (8 * 3)) | (((ulong)bptr[4]) << (8 * 4)) | ((ulong)bptr[5] << (8 * 5)) | (((ulong)bptr[6]) << (8 * 6)) | (((ulong)bptr[7]) << (8 * 7)));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static int OrderToInt32(this CPtr<byte> buffer)
        {
            return (int)OrderToUInt32(buffer);
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static long OrderToInt64(this CPtr<byte> buffer)
        {
            return (long)OrderToUInt64(buffer);
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要4字节</param>
        /// <returns>转化后的值</returns>
        public static float OrderToFloat(this CPtr<byte> buffer)
        {
            var t = OrderToUInt32(buffer);
            return *(float*)&t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static double OrderToDouble(this CPtr<byte> buffer)
        {
            var t = OrderToUInt64(buffer);
            return *(double*)&t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要2字节</param>
        /// <returns>转化后的值</returns>
        public static ushort OrderToUInt16(this CPtr<byte> buffer)
        {
            byte* bptr = buffer.p_ptr;
            return (ushort)(bptr[0] | ((bptr[1]) << 8));
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要2字节</param>
        /// <returns>转化后的值</returns>
        public static short OrderToInt16(this CPtr<byte> buffer)
        {
            byte* bptr = buffer.p_ptr;
            return (short)(bptr[0] | ((bptr[1]) << 8));
        }

        #endregion

        #region 字节数组转

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static uint OrderToUInt32(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if(offset + sizeof(uint) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt32(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static int OrderToInt32(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(uint) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt32(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static ulong OrderToUInt64(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(ulong) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt64(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static long OrderToInt64(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(long) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt64(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static ushort OrderToUInt16(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(ushort) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt16(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static short OrderToInt16(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(short) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt16(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static float OrderToFloat(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(float) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToFloat(new IntPtr(bptr + offset));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">从字节数组读取的起始位置</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static double OrderToDouble(this byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset + sizeof(double) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToDouble(new IntPtr(bptr + offset));
            }

        }


        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static uint OrderToUInt32(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if ( sizeof(uint) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt32(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static int OrderToInt32(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(uint) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt32(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static ulong OrderToUInt64(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(ulong) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt64(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static long OrderToInt64(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(long) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt64(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static ushort OrderToUInt16(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(ushort) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToUInt16(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static short OrderToInt16(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(short) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToInt16(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static float OrderToFloat(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(float) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToFloat(new IntPtr(bptr));
            }

        }

        /// <summary>
        /// 将字节数组的内存按顺序序列转化为值
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns>要转化的值</returns>
        /// <exception cref="ArgumentNullException">字节数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定字节数组范围小于要转化的值大小</exception>
        public static double OrderToDouble(this byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (sizeof(double) > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            fixed (byte* bptr = buffer)
            {
                return OrderToDouble(new IntPtr(bptr));
            }

        }

        #endregion

        #endregion

        #endregion

    }

}
