using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Cheng.Memorys;

namespace Cheng.Streams
{

    /// <summary>
    /// 封装一个流，每次读取数据都会强制完整读取
    /// </summary>
    public class ReadBlockStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化强制完整读取流
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadBlockStream(Stream stream) : this(stream, true)
        {
        }

        /// <summary>
        /// 实例化强制完整读取流
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="disposeBase">释放时是否释放封装的流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ReadBlockStream(Stream stream, bool disposeBase)
        {
            p_stream = stream ?? throw new ArgumentNullException();
            p_disposeBase = disposeBase;
        }

        #endregion

        #region 参数

        private Stream p_stream;

        private bool p_disposeBase;

        #endregion

        #region 功能

        public override bool CanRead => (p_stream?.CanRead).GetValueOrDefault();

        public override bool CanSeek => (p_stream?.CanSeek).GetValueOrDefault();

        public override bool CanTimeout => (p_stream?.CanTimeout).GetValueOrDefault();

        public override bool CanWrite => (p_stream?.CanWrite).GetValueOrDefault();

        public override int ReadTimeout 
        {
            get
            {
                ThrowIsDispose();
                return p_stream.ReadTimeout;
            }
            set
            {
                ThrowIsDispose();
                p_stream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout 
        { 
            get => p_stream.WriteTimeout; 
            set => p_stream.WriteTimeout = value; 
        }

        public override long Length
        {
            get
            {
                ThrowIsDispose();
                return p_stream.Length;
            }
        }

        public override long Position 
        {
            get
            {
                ThrowIsDispose();
                return p_stream.Position;
            }
            set
            {
                ThrowIsDispose();
                p_stream.Position = value;
            }
        }

        public override bool CanInternalStream => true;

        public override Stream InternalBaseStream => p_stream;

        public override void Flush()
        {
            p_stream?.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return p_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose();
            p_stream.SetLength(value);
        }

        /// <summary>
        /// 从当前流读取字节序列，并将此流中的位置提升读取的字节数
        /// </summary>
        /// <remarks>
        /// 此函数与<see cref="Cheng.Memorys.MemoryOperation.ReadBlock(Stream, byte[], int, int)"/>功能一致
        /// </remarks>
        /// <param name="buffer">要将数据读取到的字节数组</param>
        /// <param name="offset">字节数组的数据从指定偏移开始写入</param>
        /// <param name="count">要读取的字节数</param>
        /// <returns>
        /// <para>实际读取到的字节数</para>
        /// <para>若该值小于<paramref name="count"/>，则表示此次读取后流已到达末尾；若返回为0，表示已经到达末尾</para>
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            return p_stream.ReadBlock(buffer, offset, count);
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            return p_stream.ReadByte();
        }

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose();
            int c = 0;

            while (c < count)
            {
                var re = p_stream.ReadByte();

                if (re == -1) break;

                buffer[c++] = (byte)re;
            }

            return c;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            p_stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            p_stream.WriteByte(value);
        }

        protected override bool Disposing(bool disposing)
        {
            if (disposing && p_disposeBase)
            {
                p_stream.Close();
            }
            p_stream = null;
            return true;
        }

        #endregion

    }

}
