using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cheng.Streams;
using Cheng.Texts;
using Cheng.Algorithm;
using System.Threading.Tasks;
using System.Threading;
using Cheng.Memorys;

namespace Cheng.Algorithm.Encryptions.Char16
{

    /// <summary>
    /// 半字节16字数据加密写入器
    /// </summary>
    public unsafe sealed class Char16EecStreamWriter : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化半字节16字数据加密写入器
        /// </summary>
        /// <param name="char16">有16个不同字符的字符串</param>
        /// <param name="writer">要封装的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        public Char16EecStreamWriter(string char16, TextWriter writer) : this(char16, writer, true, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字数据加密写入器
        /// </summary>
        /// <param name="char16">有16个不同字符的字符串</param>
        /// <param name="writer">要封装的文本写入器</param>
        /// <param name="disposeBaseWriter">释放时是否释放封装的对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        public Char16EecStreamWriter(string char16, TextWriter writer, bool disposeBaseWriter) : this(char16, writer, disposeBaseWriter, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字数据加密写入器
        /// </summary>
        /// <param name="char16">有16个不同字符的字符数组</param>
        /// <param name="writer">要封装的文本写入器</param>
        /// <param name="disposeBaseWriter">释放时是否释放封装的对象</param>
        /// <param name="bufferSize">缓冲区大小，默认为2048</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区没有大于0</exception>
        public Char16EecStreamWriter(char[] char16, TextWriter writer, bool disposeBaseWriter, int bufferSize) : this(new string(char16), writer, disposeBaseWriter, bufferSize)
        {
        }

        /// <summary>
        /// 实例化半字节16字数据加密写入器
        /// </summary>
        /// <param name="char16">有16个不同字符的字符串</param>
        /// <param name="writer">要封装的文本写入器</param>
        /// <param name="disposeBaseWriter">释放时是否释放封装的对象</param>
        /// <param name="bufferSize">缓冲区大小，默认为2048</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区没有大于0</exception>
        public Char16EecStreamWriter(string char16, TextWriter writer, bool disposeBaseWriter, int bufferSize)
        {
            if (writer is null || char16 is null) throw new ArgumentNullException();
            if (char16.Length != 16) throw new ArgumentException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            p_char16 = char16;
            p_writer = writer;
            p_disposeBase = disposeBaseWriter;
            p_buffer = new byte[bufferSize];
            p_charBuf = new char[bufferSize * 2];
        }

        #endregion

        #region 参数

        private TextWriter p_writer;

        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 相对 p_buffer 的双倍缓冲区
        /// </summary>
#endif
        private char[] p_charBuf;

        private string p_char16;

#if DEBUG
        /// <summary>
        /// p_buffer 的当前写入长度
        /// </summary>
#endif
        private int p_bufLength;

        private bool p_disposeBase;

        #endregion

        #region 功能

        #region 封装

#if DEBUG
        /// <summary>
        /// 将数据写入缓冲区
        /// </summary>
        /// <param name="buffer">写入的数据</param>
        /// <param name="count">要写入的字节数</param>
        /// <returns>实际写入的字节数</returns>
#endif
        private int f_writeToBuf(byte* buffer, int count)
        {
            //剩余可写长度
            var lastLen = p_buffer.Length - p_bufLength;
            var wrSize = Math.Min(lastLen, count);
            fixed (byte* bp = p_buffer)
            {
                MemoryOperation.MemoryCopy(buffer, bp + p_bufLength, wrSize);
                p_bufLength += wrSize;
            }
            return wrSize;
        }

#if DEBUG
        /// <summary>
        /// 将缓冲区数据转化并写入基础流
        /// </summary>
#endif
        private void f_bufEncWrToBaseText()
        {
            int i;
            int ci;

            for (i = 0, ci = 0; i < p_bufLength; i++)
            {
                var bv = p_buffer[i];
                p_charBuf[ci++] = p_char16[(bv & 0xF)];
                p_charBuf[ci++] = p_char16[((bv >> 4) & 0xF)];
            }
            var charLen = p_bufLength * 2;

            p_bufLength = 0;

            //写入writer
            p_writer.Write(p_charBuf, 0, charLen);
        }

        private void f_writer(byte* buffer, int count)
        {
            int wrBufSize;
            int wrc = 0;
            while (count > 0)
            {
                wrBufSize = f_writeToBuf(buffer + wrc, count);
                if (wrBufSize == 0)
                {
                    lock (p_buffer) f_bufEncWrToBaseText();
                }
                wrc += wrBufSize;
                count -= wrBufSize;
            }
        }

        private void f_flush(bool writerBase)
        {
            f_bufEncWrToBaseText();
            if (writerBase)
            {
                p_writer.Flush();
            }
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if (disposing)
            {
                f_flush(false);
                if (p_disposeBase) p_writer.Close();
            }

            p_writer = null;
            p_buffer = null;
            p_charBuf = null;
            p_char16 = null;

            return true;
        }

        #endregion

        #region 派生

        public override bool CanWrite => true;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override void Flush()
        {
            if (IsDispose) return;
            f_flush(true);
        }

        /// <summary>
        /// 清除当前缓冲区并将数据写入封装的写入器
        /// </summary>
        /// <param name="flushBase">是否在写入封装的写入器后调用<see cref="TextWriter.Flush"/></param>
        public void Flush(bool flushBase)
        {
            if (IsDispose) return;
            f_flush(flushBase);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            int length = buffer.Length;
            if (offset < 0 || count < 0 || (offset + count) > length) new ArgumentOutOfRangeException();

            fixed (byte* bp = buffer)
            {
                f_writer(bp + offset, count);
            }
        }

        public override void WriteByte(byte value)
        {
            //剩余可写长度
            var lastLen = p_buffer.Length - p_bufLength;
            if(lastLen == 0)
            {
                lock (p_buffer) f_bufEncWrToBaseText();
            }
            f_writeToBuf(&value, 1);
        }

        public override unsafe void WriteToAddress(byte* buffer, int count)
        {
            if (buffer == null) throw new ArgumentNullException();
            f_writer(buffer, count);
        }

        #region 无实现

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new InvalidOperationException();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #endregion

    }

}

#if DEBUG

#endif