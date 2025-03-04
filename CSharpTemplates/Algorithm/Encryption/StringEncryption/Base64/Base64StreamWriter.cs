using System;
using System.IO;
using Cheng.Memorys;
using Cheng.Streams;

namespace Cheng.Algorithm.Encryptions.Base64Encryption
{

    /// <summary>
    /// Base64流编码器
    /// </summary>
    /// <remarks>
    /// 封装一个文本写入器到只写流对象中，写入的字节数据会编码为Base64文本并写入到文本写入器中
    /// </remarks>
    public unsafe class Base64StreamWriter : HEStream
    {

        #region 构造

        const int defBufLen = 512;

        /// <summary>
        /// 实例化Base64流编码器
        /// </summary>
        /// <param name="textWriter">要编码到的Base64文本写入器</param>
        /// <param name="disposeBaseWriter">在释放对象时是否释放内部的文本写入器，该参数默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public Base64StreamWriter(TextWriter textWriter) : this(textWriter, true, defBufLen)
        {
        }

        /// <summary>
        /// 实例化Base64流编码器
        /// </summary>
        /// <param name="textWriter">要编码到的Base64文本写入器</param>
        /// <param name="disposeBaseWriter">在释放对象时是否释放内部的文本写入器，该参数默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public Base64StreamWriter(TextWriter textWriter, bool disposeBaseWriter) : this(textWriter, disposeBaseWriter, defBufLen)
        {
        }

        /// <summary>
        /// 实例化Base64流编码器
        /// </summary>
        /// <param name="textWriter">要编码到的Base64文本写入器</param>
        /// <param name="disposeBaseWriter">在释放对象时是否释放内部的文本写入器，该参数默认为true</param>
        /// <param name="bufferSizeMagnification">
        /// <para>指定缓冲区长度倍率</para>
        /// <para>该参数表示Base64缓冲区长度的最小公倍数，内部使用该参数以3和4倍的长度分别初始化字节缓冲区和字符缓冲区</para>
        /// <para>该参数默认为512</para>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度没有大于0</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public Base64StreamWriter(TextWriter textWriter, bool disposeBaseWriter, int bufferSizeMagnification)
        {
            if (textWriter is null) throw new ArgumentNullException();
            if (bufferSizeMagnification <= 0) throw new ArgumentOutOfRangeException();

            f_init(textWriter, disposeBaseWriter, bufferSizeMagnification);
        }

        private void f_init(TextWriter writer, bool disposeBase, int bufferSizeMagnification)
        {
            p_writer = writer;
            p_disposeBaseWriter = disposeBase;
            //p_base64List = Base64EncodeReader.Base64EncodeList.ToCharArray();
            p_charBuf = new char[4 * bufferSizeMagnification];
            p_buffer = new byte[3 * bufferSizeMagnification];
            //p_bufferPos = 0;
            p_bufferLen = 0;
        }

        #endregion

        #region 参数

        private TextWriter p_writer;

        private char[] p_charBuf;

        private byte[] p_buffer;

        /// <summary>
        /// 字节缓冲区数据长度
        /// </summary>
        private int p_bufferLen;
        
        private bool p_disposeBaseWriter;

        #endregion

        #region 功能

        #region 封装

        /// <summary>
        /// 将buffer转化写入文本（缓冲区填满时）
        /// </summary>
        private void f_toBase64TextWriter()
        {
            var charCount = Convert.ToBase64CharArray(p_buffer, 0, p_bufferLen, p_charBuf, 0, Base64FormattingOptions.None);
            p_writer.Write(p_charBuf, 0, charCount);
            p_bufferLen = 0;
        }

        private void f_writer(byte* buffer, int count)
        {

            int bufLen = p_buffer.Length;

            byte* reBuf = buffer;
            fixed (byte* bufPtr = p_buffer)
            {
                //写入字节量

                while (count != 0)
                {
                    if (p_bufferLen == bufLen)
                    {
                        f_toBase64TextWriter();
                    }

                    var wrc = Math.Min(count, bufLen - p_bufferLen);

                    //将buffer写入缓冲区
                    MemoryOperation.MemoryCopy(reBuf, bufPtr + p_bufferLen, wrc);

                    //剩余字节量
                    count -= wrc;
                    reBuf += wrc;
                    p_bufferLen += wrc;

                }

            }

        }

        private void f_flushOver()
        {
            var charCount = Convert.ToBase64CharArray(p_buffer, 0, p_bufferLen, p_charBuf, 0, Base64FormattingOptions.None);
            p_writer.Write(p_charBuf, 0, charCount);
            p_bufferLen = 0;

        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            f_flushOver();
            if (disposing && p_disposeBaseWriter)
            {
                p_writer?.Flush();
            }
            p_writer = null;
            return true;
        }

        #endregion

        #region 派生

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(Base64StreamWriter));
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (offset + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            fixed (byte* bp = buffer)
            {
                f_writer(bp + offset, count);
            }
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose(nameof(Base64StreamWriter));

            if (p_bufferLen == p_buffer.Length)
            {
                f_toBase64TextWriter();
            }

            p_buffer[p_bufferLen++] = value;
        }

        /// <summary>
        /// 将缓冲区的剩余字节数据作为结尾解码到写入器并清空缓冲区
        /// </summary>
        public override void Flush()
        {
            Flush(true);
        }

        /// <summary>
        /// 将缓冲区的剩余字节数据作为结尾解码到写入器
        /// </summary>
        /// <param name="flushBaseWriter">写入后是否调用基础文本写入器的<see cref="TextWriter.Flush"/></param>
        public void Flush(bool flushBaseWriter)
        {
            if (IsDispose) return;

            f_flushOver();

            if (flushBaseWriter)
            {
                p_writer?.Flush();
            }
        }

        #endregion

        #region 禁用

        public override long Seek(long offset, SeekOrigin origin)
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

        public override long Position 
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Length => throw new NotSupportedException();

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

    }


}
