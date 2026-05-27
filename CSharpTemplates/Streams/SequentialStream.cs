using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Cheng.Memorys;

namespace Cheng.Streams
{

    /// <summary>
    /// 截断指定流对象并只能以顺序读取
    /// </summary>
    /// <remarks>
    /// <para>该截断流能够封装一个无法跳转位置的流对象读取小于实际长度的字节数量</para>
    /// </remarks>
    public sealed unsafe class SequentialStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化顺序截断流
        /// </summary>
        /// <param name="stream">要封装的基础流数据</param>
        /// <param name="length">最大可读取的字节长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">读取长度小于0</exception>
        /// <exception cref="NotSupportedException">流不支持读取</exception>
        public SequentialStream(Stream stream, long length) : this(stream, length, true, 1024 * 4)
        {}

        /// <summary>
        /// 实例化顺序截断流
        /// </summary>
        /// <param name="stream">要封装的基础流数据</param>
        /// <param name="length">最大可读取的字节长度</param>
        /// <param name="disposeBaseStream">释放时是否释放基础流；默认是true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">读取长度小于0</exception>
        /// <exception cref="NotSupportedException">流不支持读取</exception>
        public SequentialStream(Stream stream, long length, bool disposeBaseStream) : this(stream, length, disposeBaseStream, 1024 * 4)
        {}

        /// <summary>
        /// 实例化顺序截断流
        /// </summary>
        /// <param name="stream">要封装的基础流数据</param>
        /// <param name="length">最大可读取的字节长度</param>
        /// <param name="disposeBaseStream">释放时是否释放基础流；默认是true</param>
        /// <param name="bufferSize">内部缓冲区字节大小，默认是4096</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">读取长度小于0或缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流不支持读取</exception>
        public SequentialStream(Stream stream, long length, bool disposeBaseStream, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException();
            if (length < 0 || bufferSize <= 0) throw new ArgumentOutOfRangeException();
            if (!stream.CanRead)
            {
                throw new NotSupportedException();
            }
            
            p_s = stream;
            p_nowReadCount = 0;
            p_length = length;
            p_disposeBase = disposeBaseStream;
            p_buffer = new byte[bufferSize];
        }

        #endregion

        #region 参数

        private Stream p_s;

        private byte[] p_buffer;

        private long p_nowReadCount;

        private long p_length;

        private bool p_disposeBase;

        #endregion

        #region 参数

        /// <summary>
        /// 已读取的字节数
        /// </summary>
        public long ReadByteCount
        {
            get => p_nowReadCount;
        }

        #endregion

        #region 派生

        protected override bool Disposing(bool disposing)
        {
            if (disposing && p_disposeBase)
            {
                p_s.Close();
            }
            p_s = null;
            return true;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));
            if (buffer == null) throw new ArgumentNullException();
            // 剩余读取数
            var lastC = (p_length - p_nowReadCount);
            if (lastC == 0) return 0;
            int re;
            // 此次要读取数
            var rec = Math.Min(p_buffer.Length, count);
            if(rec <= lastC)
            {
                re = p_s.ReadBlock(p_buffer, 0, rec);
                p_nowReadCount += re;
                fixed (byte* bufPtr = p_buffer)
                {
                    MemoryOperation.MemoryCopy(bufPtr, buffer, re);
                }
                return re;
            }
            else
            {
                // 剩余数小于读取数
                re = p_s.ReadBlock(p_buffer, 0, (int)lastC);
                p_nowReadCount += re;
                fixed (byte* bufPtr = p_buffer)
                {
                    MemoryOperation.MemoryCopy(bufPtr, buffer, re);
                }
                return re;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            var len = buffer.Length;

            if (count < 0 || offset < 0 || offset + count > len) throw new ArgumentOutOfRangeException();

            fixed (byte* bptr = buffer)
            {
                return ReadToAddress(bptr + offset, count);
            }
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));
            if ((p_length - p_nowReadCount) > 0)
            {
                int re;
                re = p_s.ReadByte();
                if(re < 0)
                {
                    return -1;
                }
                p_nowReadCount++;
                return re;
            }
            return -1;
        }

        /// <summary>
        /// 共可读取的字节数
        /// </summary>
        public override long Length => p_length;

        /// <summary>
        /// 已读取的字节数量
        /// </summary>
        public override long Position
        {
            get => p_nowReadCount;
        }

        #endregion

    }

}