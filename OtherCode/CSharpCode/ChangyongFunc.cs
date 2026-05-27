using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.OtherCode
{

    /// <summary>
    /// 常用方法
    /// </summary>
    internal static unsafe class ChangyongFunc
    {

        /// <summary>
        /// 完整读取流数据的字节序列
        /// </summary>
        /// <remarks>
        /// <para>此函数会不断读取数据，直至读取的字节数等于参数<paramref name="count"/>或流内无法读取</para>
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
        public static int ReadBlock(Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();
            //if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException(getArgOutOfRangeReadBlock());
            //int index = offset;
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.Read(buffer, offset, count);
                if (rsize == 0) return re;
                offset += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        /// <summary>
        /// 完整读取流数据的字节序列
        /// </summary>
        /// <remarks>
        /// <para>此函数会不断读取数据，直至读取的字节数等于参数<paramref name="count"/>或流内无法读取</para>
        /// </remarks>
        /// <param name="stream">读取的流</param>
        /// <param name="buffer">读取到的缓冲区首地址</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>实际读取的字节数量；若返回的值小于<paramref name="count"/>表示剩余字节数小于要读取的字节数，返回0表示流已到达结尾</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">不支持方法</exception>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public static int ReadBlock(Stream stream, byte* buffer, int count)
        {
            if (stream is null) throw new ArgumentNullException();
            if (count < 0) throw new ArgumentOutOfRangeException();
            //int index = offset;
            //int rsize;
            int re = 0;
            int index = 0;
            while (count != 0)
            {
                int reb = stream.ReadByte();
                if (reb < 0) break;
                buffer[index] = (byte)reb;
                re++;
                count--;
                index++;
            }
            return re;
        }

        /// <summary>
        /// 将流数据读取并拷贝到另一个流当中
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        public static void CopyToStream(Stream stream, Stream toStream, byte[] buffer)
        {
            int length = buffer.Length;
            int rsize;

            BeginLoop:
            rsize = stream.Read(buffer, 0, length);

            if (rsize == 0) return;

            toStream.Write(buffer, 0, rsize);
            goto BeginLoop;

        }


    }

}
