using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Cheng.Streams
{

    static partial class StreamExtend
    {

        internal static async Task<long> f_async_copyToStream(Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {

            int length = buffer.Length;
            int rsize;
            int reas;
            ulong isReadSize;
            long cre = 0;
            if (maxBytes == 0)
            {
                BeginLoop:
                rsize = await stream.ReadAsync(buffer, 0, length);

                if (rsize == 0) return cre;

                await toStream.WriteAsync(buffer, 0, rsize);
                cre += rsize;
                goto BeginLoop;
            }

            isReadSize = 0;

            nBeginLoop:

            if (isReadSize == maxBytes) return cre;

            if ((isReadSize + (ulong)length) > maxBytes)
            {
                reas = (int)(maxBytes - isReadSize);
            }
            else reas = length;

            rsize = await stream.ReadAsync(buffer, 0, reas);

            if (rsize == 0) return cre;

            isReadSize += (ulong)rsize;
            await toStream.WriteAsync(buffer, 0, rsize);
            cre += rsize;
            goto nBeginLoop;

        }

        /// <summary>
        /// 将流数据异步读取并拷贝到另一个流当中并返回拷贝数据量
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <param name="maxBytes">指定最大拷贝字节量，0表示不指定最大字节量</param>
        /// <returns>异步拷贝操作的任务，任务返回值是实际拷贝的字节数</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static Task<long> CopyToStreamAsync(this Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            if (stream is null || toStream is null || buffer is null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length == 0) throw new ArgumentException();
            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException();

            return f_async_copyToStream(stream, toStream, buffer, maxBytes);
        }

        /// <summary>
        /// 将流数据异步读取并拷贝到另一个流当中并返回拷贝数据量
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <param name="buffer">流数据一次读写的缓冲区</param>
        /// <returns>异步拷贝操作的任务，任务返回值是实际拷贝的字节数</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">缓冲区长度为0</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static Task<long> CopyToStreamAsync(this Stream stream, Stream toStream, byte[] buffer)
        {
            if (stream is null || toStream is null || buffer is null)
            {
                throw new ArgumentNullException();
            }
            if (buffer.Length == 0) throw new ArgumentException();
            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException();

            return f_async_copyToStream(stream, toStream, buffer, 0);
        }

        /// <summary>
        /// 将流数据异步读取并拷贝到另一个流当中并返回拷贝数据量
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <param name="toStream">写入的流</param>
        /// <returns>异步拷贝操作的任务，任务返回值是实际拷贝的字节数</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流数据没有指定权限</exception>
        public static Task<long> CopyToStreamAsync(this Stream stream, Stream toStream)
        {
            if (stream is null || toStream is null)
            {
                throw new ArgumentNullException();
            }
            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException();

            return f_async_copyToStream(stream, toStream, new byte[1024 * 32], 0);
        }

    }

}
