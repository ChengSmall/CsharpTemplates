using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Streams
{

    /// <summary>
    /// 拥有固定字节量为0值的只读流
    /// </summary>
    /// <remarks>
    /// <para>内部使用参数模拟流数据，不会申请大块内存</para>
    /// </remarks>
    public sealed class FixedZeroByteStream : HEStream
    {

        #region

        /// <summary>
        /// 实例化一个固定字节量为0值的只读流
        /// </summary>
        /// <param name="max">要指定字节量的数量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public FixedZeroByteStream(int max) : this((long)max)
        {
        }

        /// <summary>
        /// 实例化一个固定字节量为0值的只读流
        /// </summary>
        /// <param name="max">要指定字节量的数量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public FixedZeroByteStream(long max)
        {
            if (max < 0) throw new ArgumentOutOfRangeException();
            p_count = 0;
            p_max = max;
        }

        #endregion

        #region

        private long p_count;

        private readonly long p_max;

        #endregion

        #region 流功能

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                return p_max;
            }
        }

        public override long Position 
        {
            get
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                return p_count;
            }
            set
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                if (value < 0) p_count = 0;
                else p_count = value;
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            switch (origin)
            {
                case SeekOrigin.Begin:
                    p_count = offset;
                    break;
                case SeekOrigin.Current:
                    p_count = p_count + offset;
                    break;
                case SeekOrigin.End:
                    p_count = (p_max - offset);
                    break;
                default:
                    throw new ArgumentException();
            }
            if (p_count < 0) p_count = 0;
            return p_count;
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            if (p_count < p_max)
            {
                p_count++;
                return 0;
            }
            return -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (count + offset > buffer.Length)) throw new ArgumentOutOfRangeException();

            long lastCount;
            //剩余字节量
            //var lastCount = p_max - p_count;
            lastCount = Math.Min(p_max - p_count, count);

            if (lastCount == 0) return 0;

            Array.Clear(buffer, offset, count);
            p_count += lastCount;
            return (int)lastCount;
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            throw new NotSupportedException();
        }

        #endregion

    }

    /// <summary>
    /// 拥有固定字节量的指定值的只读流
    /// </summary>
    /// <remarks>
    /// <para>内部使用参数模拟流数据，不会申请大块内存</para>
    /// </remarks>
    public sealed class FixedByteStream : HEStream
    {

        #region

        /// <summary>
        /// 实例化一个固定字节量为指定值的只读流
        /// </summary>
        /// <param name="value">每个字节的值</param>
        /// <param name="count">要指定字节量的数量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public FixedByteStream(byte value, long count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException();
            p_count = 0;
            p_max = count;
            p_value = value;
        }

        #endregion

        #region

        private readonly long p_max;
        private long p_count;
        private readonly byte p_value;

        #endregion

        #region 流功能

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                return p_max;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                return p_count;
            }
            set
            {
                ThrowIsDispose(nameof(FixedZeroByteStream));
                if (value < 0) p_count = 0;
                else p_count = value;
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            switch (origin)
            {
                case SeekOrigin.Begin:
                    p_count = offset;
                    break;
                case SeekOrigin.Current:
                    p_count = p_count + offset;
                    break;
                case SeekOrigin.End:
                    p_count = (p_max - offset);
                    break;
                default:
                    throw new ArgumentException();
            }
            if (p_count < 0) p_count = 0;
            return p_count;
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            if (p_count < p_max)
            {
                p_count++;
                return p_value;
            }
            return -1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (count + offset > buffer.Length)) throw new ArgumentOutOfRangeException();

            long lastCount;
            //剩余字节量
            //var lastCount = p_max - p_count;
            lastCount = Math.Min(p_max - p_count, count);

            if (lastCount == 0) return 0;
            if(p_value == 0) Array.Clear(buffer, offset, count);
            else
            {
                for (int i = offset; i < count; i++)
                {
                    buffer[i] = p_value;
                }
            }

            p_count += lastCount;
            return (int)lastCount;
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(FixedZeroByteStream));
            throw new NotSupportedException();
        }

        #endregion

    }

}
