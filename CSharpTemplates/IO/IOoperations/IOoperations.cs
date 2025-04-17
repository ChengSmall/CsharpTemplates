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
            OrderToBytes(*(uint*)&value, buffer);
        }

        /// <summary>
        /// 将值顺序转化到指定内存序列
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="buffer">要转化到的内存序列，必须保证该地址的可用内存等于或大于<paramref name="value"/>的字节大小</param>
        public static void OrderToBytes(this double value, byte* buffer)
        {
            OrderToBytes(*(ulong*)&value, buffer);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
                throw new ArgumentOutOfRangeException(Cheng.Properties.Resources.Exception_FuncArgOutOfRange);
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
            const int size = sizeof(uint);
            byte* bptr = (byte*)buffer;
            uint t = default;

            for (int i = 0; i < size; i++)
            {
                t |= (((uint)bptr[i]) << (i * 8));
            }
            return t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要8字节</param>
        /// <returns>转化后的值</returns>
        public static ulong OrderToUInt64(this IntPtr buffer)
        {
            const int size = sizeof(ulong);
            byte* bptr = (byte*)buffer;
            ulong t = default;

            for (int i = 0; i < size; i++)
            {
                t |= (((ulong)bptr[i]) << (i * 8));
            }
            return t;
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
            ushort t = default;

            t |= bptr[0];
            t |= (ushort)(((uint)bptr[1]) << (1 * 8));

            return t;
        }

        /// <summary>
        /// 将内存顺序序列转化为值
        /// </summary>
        /// <param name="buffer">表示一个可用内存地址，内存可用长度至少要2字节</param>
        /// <returns>转化后的值</returns>
        public static short OrderToInt16(this IntPtr buffer)
        {
            byte* bptr = (byte*)buffer;
            short t = default;

            t |= (short)bptr[0];
            t |= (short)(((uint)bptr[1]) << (1 * 8));

            return t;
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

        #endregion

        #endregion

        #endregion

        #region 文件

        /// <summary>
        /// 计算指定流默认的Hash256值
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="buffer32">大小至少是32字节的字节缓冲区</param>
        /// <param name="readBuffer">大小至少是32字节的第二个字节缓冲区</param>
        /// <returns>对于<paramref name="stream"/>Hash256值的默认计算结果</returns>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小不够</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">两个缓冲区是同一个实例的引用</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        public static Hash256 ToHash256(this Stream stream, byte[] buffer32, byte[] readBuffer)
        {
            if (stream is null || buffer32 is null || readBuffer is null) throw new ArgumentNullException();
            if (buffer32.Length < Hash256.Size || readBuffer.Length < Hash256.Size) throw new ArgumentException();
            if (buffer32 == readBuffer) throw new ArgumentException();

            var re = stream.ReadBlock(buffer32, 0, Hash256.Size);
            f_encBuf32(buffer32);
            if (re != Hash256.Size)
            {
                return f_bytesToHash256(buffer32);
            }

            Loop:
            re = stream.ReadBlock(readBuffer, 0, Hash256.Size);
            for (int i = 0; i < re; i++)
            {
                buffer32[i] ^= readBuffer[i];
            }
            f_encBuf32(buffer32);

            if (re == Hash256.Size) goto Loop;

            return f_bytesToHash256(buffer32);
        }

        static void f_encBuf32(byte[] buf32)
        {
            const ulong a = 0xFEFAB0B1_1B0BAFEF, b = 0x87654321_12345678, c = 0x000FF000_EEE00EEE;
            const ulong x = (a ^ c);

            var u1 = OrderToUInt64(buf32, 0);
            var u2 = OrderToUInt64(buf32, 8);
            var u3 = OrderToUInt64(buf32, 16);
            var u4 = OrderToUInt64(buf32, 24);

            (u1 ^ a).OrderToByteArray(buf32, 24);
            (u2 ^ b).OrderToByteArray(buf32, 16);
            (u3 ^ c).OrderToByteArray(buf32, 8);
            (u4 ^ x).OrderToByteArray(buf32, 0);
        }

        static Hash256 f_bytesToHash256(byte[] buf32)
        {
            return new Hash256(OrderToUInt64(buf32, 0), OrderToUInt64(buf32, 8), OrderToUInt64(buf32, 16), OrderToUInt64(buf32, 24));
        }

        /// <summary>
        /// 计算指定流默认的Hash256值
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns>对于<paramref name="stream"/>Hash256值的默认计算结果</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        public static Hash256 ToHash256(this Stream stream)
        {
            return ToHash256(stream, new byte[Hash256.Size], new byte[Hash256.Size]);
        }

        #endregion

    }

}
