using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Streams
{

    /// <summary>
    /// 流对象扩展
    /// </summary>
    public static unsafe partial class StreamExtend
    {

        #region 读取和拷贝

        static string getArgOutOfRangeReadBlock()
        {
            return Cheng.Properties.Resources.Exception_FuncArgOutOfRange;
        }

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
        public static int ReadBlock(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();

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
        public static int ReadBlock(this Stream stream, byte* buffer, int count)
        {
            if (stream is null) throw new ArgumentNullException();
            if (count < 0) throw new ArgumentOutOfRangeException(getArgOutOfRangeReadBlock());

            if(stream is HEStream hs)
            {
                return f_readB(hs, buffer, count);
            }

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
            if (stream is null || buffer is null)
            {
                if(stream is null) throw new ArgumentNullException(nameof(stream));
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException(getArgOutOfRangeReadBlock());

            return f_ReadBlockEnumator(stream, buffer, offset, count);
        }

        internal static IEnumerable<int> f_ReadBlockEnumator(Stream stream, byte[] buffer, int offset, int count)
        {
            //int index = offset;
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.Read(buffer, offset, count);
                if (rsize == 0) yield break;
                offset += rsize;
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

            if (buffer.Length == 0) throw new ArgumentException();

            if ((!stream.CanRead) || (!toStream.CanWrite)) throw new NotSupportedException();

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

        internal static void copyToStream(Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
        {
            int length = buffer.Length;
            int rsize;
            int reas;
            ulong isReadSize;

            if (maxBytes == 0)
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

        internal static void copyToStream(Stream stream, Stream toStream, byte[] buffer)
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

        internal static IEnumerable<int> copyToStreamEnr(Stream stream, Stream toStream, byte[] buffer, ulong maxBytes)
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
        /// 读取流数据的所有数据到字节数组
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <returns>从<paramref name="stream"/>读取的所有数据</returns>
        public static byte[] ReadAll(this Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();

            if (stream.CanRead)
            {
                if (stream.CanSeek && stream.Length < int.MaxValue)
                {
                    var buf = new byte[stream.Length];
                    stream.ReadBlock(buf, 0, buf.Length);
                    return buf;
                }
                else
                {
                    MemoryStream ms = new MemoryStream(1024);
                    stream.CopyToStream(ms, new byte[1024]);
                    if (ms.TryGetBuffer(out var arr))
                    {
                        if (arr.Count == ms.Length && arr.Offset == 0)
                        {
                            return arr.Array;
                        }
                    }
                    return ms.ToArray();
                }
            }

            throw new NotSupportedException(Cheng.Properties.Resources.Exception_StreamNotRead);

        }

        public static int ReadToAddress(this Stream stream, byte* buffer, int count)
        {
            if(stream is HEStream hs)
            {
                return hs.ReadToAddress(buffer, count);
            }

            int c = 0;

            while (c < count)
            {
                var re = stream.ReadByte();
                if (re == -1) break;
                buffer[c++] = (byte)re;
            }

            return c;
        }

        #endregion

        #region 读写对象

        /// <summary>
        /// 从流数据中读取指定类型的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="stream">流</param>
        /// <param name="buffer">读取时需要的缓冲区，长度必须大于类型<typeparamref name="T"/></param>
        /// <param name="value">读取到的变量</param>
        /// <returns>是否成功读取到或读取完整</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static bool ReadValue<T>(this Stream stream, byte[] buffer, out T value) where T : unmanaged
        {
            if (stream is null || buffer is null ) throw new ArgumentNullException();
            if (buffer.Length < sizeof(T)) throw new ArgumentException();
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
                return true;
            }

            ri = stream.ReadBlock(buffer, 0, sizeof(T));

            if (ri < sizeof(T))
            {
                value = default;
                return false;
            }

            value = buffer.ToStructure<T>();
            return true;
        }

        static int f_readB(HEStream stream, byte* ptr, int count)
        {
            int rsize;
            int offset = 0;
            int re = 0;
            while (count != 0)
            {
                rsize = stream.ReadToAddress(ptr + offset, count);
                if (rsize == 0) return re;
                offset += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        /// <summary>
        /// 从流数据中读取指定类型的对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="stream">流</param>
        /// <param name="value">读取到的变量</param>
        /// <returns>是否成功读取到或读取完整</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static bool ReadValue<T>(this Stream stream, out T value) where T : unmanaged
        {
            if (stream is null) throw new ArgumentNullException();
            int re;
            fixed (T* tp = &value)
            {
                if (stream is HEStream hs)
                {
                    re = f_readB(hs, (byte*)tp, sizeof(T));
                    return re == sizeof(T);
                }

                re = ReadBlock(stream, (byte*)tp, sizeof(T));
            }
            return re == sizeof(T);
        }

        /// <summary>
        /// 将指定对象存储到流数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">流</param>
        /// <param name="buffer">写入流时的缓冲区，该缓冲区长度不得小于<typeparamref name="T"/>类型的大小</param>
        /// <param name="value">要存储的对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无写入权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static void WriteValue<T>(this Stream stream, byte[] buffer, T value) where T : unmanaged
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();
            if (buffer.Length < sizeof(T)) throw new ArgumentException();

            fixed (byte* bp = buffer)
            {
                *((T*)bp) = value;

                stream.Write(buffer, 0, sizeof(T));
            }
        }

        /// <summary>
        /// 将指定对象存储到流数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">流</param>
        /// <param name="value">要存储的对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无写入权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static void WriteValue<T>(this Stream stream, T value) where T : unmanaged
        {
            if (stream is null) throw new ArgumentNullException();

            byte* bp = (byte*)&value;

            if(stream is HEStream hs)
            {
                hs.WriteToAddress(bp, sizeof(T));
                return;
            }

            for (int i = 0; i < sizeof(T); i++)
            {
                stream.WriteByte(bp[i]);
            }
        }

        /// <summary>
        /// 读取一个字节并判断是否能否读取
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <param name="value">成功读取到的字节值；如果返回值是false该参数无效</param>
        /// <returns>如果成功读取一个字节返回true，如果读取到末尾返回false</returns>
        public static bool ReadByte(this Stream stream, out byte value)
        {
            if (stream is null) throw new ArgumentNullException();
            var re = stream.ReadByte();
            value = (byte)re;
            return re != -1;
        }

        #endregion

    }

}
