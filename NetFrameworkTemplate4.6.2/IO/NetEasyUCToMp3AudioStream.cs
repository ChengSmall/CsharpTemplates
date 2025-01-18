using Cheng.Streams;
using Cheng.Memorys;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cheng.IO.NetEasys
{

    /// <summary>
    /// 网易云音乐uc缓存文件转mp3文件读写流
    /// </summary>
    public sealed class NetEasyUCToMp3AudioStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化一个转化流
        /// </summary>
        /// <param name="baseStream">要封装的基础uc文件流</param>
        public NetEasyUCToMp3AudioStream(Stream baseStream) : this(baseStream, true)
        {
        }

        /// <summary>
        /// 实例化一个转化流
        /// </summary>
        /// <param name="baseStream">要封装的基础uc文件流</param>
        /// <param name="disposeBase">释放时是否释放<paramref name="baseStream"/>，释放传入true，不释放传入false，默认值是true</param>
        public NetEasyUCToMp3AudioStream(Stream baseStream, bool disposeBase)
        {
            if (baseStream is null) throw new ArgumentNullException();
            p_stream = baseStream;
            p_disposeBase = disposeBase;
        }

        #endregion

        #region 参数

        private Stream p_stream;

        private readonly bool p_disposeBase;

        #endregion

        #region 功能

        #region 释放

        protected override bool Disposing(bool disposing)
        {

            if(disposing && p_disposeBase)
            {
                p_stream.Close();
            }

            return true;
        }

        #endregion

        #region 重写

        public override bool CanRead => p_stream.CanRead;

        public override bool CanSeek => p_stream.CanSeek;

        public override bool CanWrite => p_stream.CanWrite;

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

        public override void Flush()
        {
            p_stream.Flush();
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();

            var re = p_stream.Read(buffer, offset, count);

            for (int i = offset; i < re; i++)
            {
                buffer[i] ^= 163;
            }

            return re;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();

            if (buffer is null) throw new ArgumentNullException();

            int end = offset + count;
            if (end >= buffer.Length || offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = offset; i < end; i++)
            {
                buffer[i] ^= 163;
            }

            p_stream.Write(buffer, offset, count);
        }

        public override int ReadByte()
        {
            var re = p_stream.ReadByte();
            if (re < 0) return -1;
            return (byte)(re ^ 163);
        }

        public override void WriteByte(byte value)
        {
            p_stream.WriteByte((byte)(value ^ 163));
        }

        #endregion

        #endregion

    }

}
