using Cheng.Streams;
using Cheng.Memorys;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cheng.IO.NetEasys
{

    /// <summary>
    /// 网易云音乐uc缓存文件转音频文件读写流
    /// </summary>
    /// <remarks>
    /// <para>封装网易云音乐的uc缓存文件，将其解密或加密成可用音频流的读写封装流</para>
    /// </remarks>
    public sealed class NetEasyUCToAudioStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化一个转化流
        /// </summary>
        /// <param name="baseStream">要封装的基础uc文件流</param>
        public NetEasyUCToAudioStream(Stream baseStream) : this(baseStream, true)
        {
        }

        /// <summary>
        /// 实例化一个转化流
        /// </summary>
        /// <param name="baseStream">要封装的基础uc文件流</param>
        /// <param name="disposeBase">释放时是否释放<paramref name="baseStream"/>，释放传入true，不释放传入false，默认值是true</param>
        public NetEasyUCToAudioStream(Stream baseStream, bool disposeBase)
        {
            if (baseStream is null) throw new ArgumentNullException();
            p_stream = baseStream;
            p_disposeBase = disposeBase;
        }

        #endregion

        #region 参数

        private Stream p_stream;

        private readonly bool p_disposeBase;

        public const ulong NetEasyUCxorInt64 = 0xA3A3A3A3A3A3A3A3;
        public const byte NetEasyUCxorValue = 163;

        #endregion

        #region 功能

        #region 释放

        protected override bool Disposing(bool disposing)
        {

            try
            {
                p_stream.Flush();
            }
            catch (Exception)
            {
            }
            if (disposing && p_disposeBase)
            {
                p_stream.Close();
            }

            return true;
        }

        #endregion

        #region 重写

        public override bool CanRead => p_stream.CanRead;

        public override bool CanSeek => p_stream.CanSeek;

        public override bool CanWrite => false;

        public override bool CanTimeout => p_stream.CanTimeout;

        public override int ReadTimeout 
        { 
            get => p_stream.ReadTimeout; 
            set => p_stream.ReadTimeout = value;
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

        public override void Flush()
        {
            //p_stream?.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return p_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose();
            throw new NotSupportedException();
        }

        public unsafe override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            var re = p_stream.Read(buffer, offset, count);

            var mb = re / sizeof(ulong);

            fixed (byte* bufPtr = buffer)
            {
                int i;
                
                ulong* ptrInt64 = (ulong*)(bufPtr + offset);
                for (i = 0; i < mb; i++, ptrInt64++)
                {
                    *(ptrInt64) ^= NetEasyUCxorInt64;
                }
                //剩余
                var sy = re % 8;
                byte* lastPtr = (byte*)(ptrInt64);
                for (i = 0; i < sy; i++)
                {
                    lastPtr[i] ^= NetEasyUCxorValue;
                }
            }

            return re;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            var re = p_stream.ReadByte();
            if (re < 0) return -1;
            return (byte)(re ^ NetEasyUCxorValue);
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            throw new NotSupportedException();
        }

        #endregion

        #endregion

    }

}
