using Cheng.Memorys;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 对另一个文本写入器添加一个缓冲层
    /// </summary>
    public sealed unsafe class BufferedTextWriter : SafeReleaseTextWriter
    {

        #region 构造

        /// <summary>
        /// 实例化一个文本写入器缓冲层
        /// </summary>
        /// <param name="textWriter">封装的基础文本写入器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BufferedTextWriter(TextWriter textWriter)
        {
            if (textWriter is null) throw new ArgumentNullException();

            f_init(textWriter, 1024 * 2, true);
        }

        /// <summary>
        /// 实例化一个文本写入器缓冲层
        /// </summary>
        /// <param name="textWriter">封装的基础文本写入器</param>
        /// <param name="isDisposeBaseWriter">在释放实例时是否将封装的写入器一并释放；默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BufferedTextWriter(TextWriter textWriter, bool isDisposeBaseWriter)
        {
            if (textWriter is null) throw new ArgumentNullException();

            f_init(textWriter, 1024 * 2, isDisposeBaseWriter);
        }

        /// <summary>
        /// 实例化一个文本写入器缓冲层
        /// </summary>
        /// <param name="textWriter">封装的基础文本写入器</param>
        /// <param name="isDisposeBaseWriter">在释放实例时是否将封装的写入器一并释放；默认为true</param>
        /// <param name="charBufferSize">指定缓冲层的缓存字符数量；默认为2048</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区缓存字符数量小于或等于0</exception>
        public BufferedTextWriter(TextWriter textWriter, bool isDisposeBaseWriter, int charBufferSize)
        {
            if (textWriter is null) throw new ArgumentNullException();
            if (charBufferSize <= 0) throw new ArgumentOutOfRangeException();

            f_init(textWriter, charBufferSize, isDisposeBaseWriter);
        }

        private void f_init(TextWriter writer, int bufferSize, bool isDispose)
        {
            p_writer = writer;
            p_charBuffer = new char[bufferSize];
            p_bufferLen = 0;
            p_isDosposeBaseWriter = isDispose;
        }

        #endregion

        #region 参数

        private TextWriter p_writer;

        private char[] p_charBuffer;

        /// <summary>
        /// 缓冲区写入字符数量
        /// </summary>
        private int p_bufferLen;

        private bool p_isDosposeBaseWriter;

        #endregion

        #region 功能

        #region 封装

        /// <summary>
        /// 剩余可写入量
        /// </summary>
        /// <returns></returns>
        private int f_resWriterSize()
        {
            return p_charBuffer.Length - p_bufferLen;
        }

        /// <summary>
        /// 写入到缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns>写入到缓存的字符数</returns>
        private int f_writerToBuffer(char[] buffer, int index, int count)
        {
            int wrc = System.Math.Min(p_charBuffer.Length - p_bufferLen, count);
            Array.Copy(buffer, index, p_charBuffer, p_bufferLen, wrc);
            p_bufferLen += wrc;
            return wrc;
        }

        /// <summary>
        /// 写入到缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns>写入到缓存的字符数</returns>
        private int f_writerToBuffer(string buffer, int index, int count)
        {
            int wrc = System.Math.Min(p_charBuffer.Length - p_bufferLen, count);

            buffer.CopyTo(index, p_charBuffer, p_bufferLen, wrc);
            p_bufferLen += wrc;
            return wrc;
        }

        /// <summary>
        /// 写入到缓冲区
        /// </summary>
        /// <param name="buffer">要写入的首地址</param>
        /// <param name="count"></param>
        /// <returns>写入到缓存的字符数</returns>
        private int f_writerToBuffer(char* buffer, int count)
        {
            int wrc = System.Math.Min(p_charBuffer.Length - p_bufferLen, count);

            fixed (char* thisBufPtr = p_charBuffer)
            {
                MemoryOperation.MemoryCopy(buffer, thisBufPtr + p_bufferLen, sizeof(char) * wrc);
            }
            p_bufferLen += wrc;
            return wrc;

        }

        /// <summary>
        /// 如果缓存已满则将缓存写入并清空缓存
        /// </summary>
        private void f_isFullToflushAndClear()
        {
            int size = p_charBuffer.Length - p_bufferLen;
            if(size == 0)
            {
                //满了
                p_writer.Write(p_charBuffer, 0, p_bufferLen);
                p_bufferLen = 0;
            }
        }

        /// <summary>
        /// 将缓存写入到基础并清空缓存
        /// </summary>
        private void f_flushBuffer()
        {
            p_writer.Write(p_charBuffer, 0, p_bufferLen);
            p_bufferLen = 0;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            f_flushBuffer();
            if(p_writer != null)
            {
                if (p_isDosposeBaseWriter && disposeing)
                {
                    p_writer.Close();
                }
                else
                {
                    p_writer.Flush();
                }
            }          
            p_writer = null;

            return true;
        }

        #endregion

        #region 参数访问


        #endregion

        #region 派生

        public override Encoding Encoding => p_writer.Encoding;

        public override IFormatProvider FormatProvider => p_writer.FormatProvider;

        public override void Write(char value)
        {
            ThrowObjectDisposed();
            f_isFullToflushAndClear();
            p_charBuffer[p_bufferLen] = value;
            p_bufferLen++;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();

            int bufLen = buffer.Length;
            if (index < 0 || count < 0 || (index + count > bufLen)) throw new ArgumentOutOfRangeException();

            int wrc;

            while (count != 0)
            {
                f_isFullToflushAndClear();
                wrc = f_writerToBuffer(buffer, index, count);
                count -= wrc;
                index += wrc;
            }

        }

        public override void Write(char[] buffer)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            
            int count = buffer.Length;
            int index = 0;
            int wrc;
            while (count != 0)
            {
                f_isFullToflushAndClear();
                wrc = f_writerToBuffer(buffer, index, count);
                count -= wrc;
                index += wrc;
            }
        }

        public override void Write(string value)
        {
            ThrowObjectDisposed();
            if (value is null) return;

            int wrc;
            int count = value.Length;
            int index = 0;
            while (count != 0)
            {
                f_isFullToflushAndClear();
                wrc = f_writerToBuffer(value, index, count);
                count -= wrc;
                index += wrc;
            }

        }

        public override void WriteLine()
        {
            ThrowObjectDisposed();

            int count = CoreNewLine.Length;
            int index = 0;
            int wrc;
            while (count != 0)
            {
                if (p_charBuffer.Length - p_bufferLen < CoreNewLine.Length)
                {
                    //满了
                    p_writer.Write(p_charBuffer, 0, p_bufferLen);
                    p_bufferLen = 0;
                }
                wrc = f_writerToBuffer(CoreNewLine, index, count);
                count -= wrc;
                index += wrc;
            }
        }

        /// <summary>
        /// （不安全）将指定地址当中的数据写入到文本写入器
        /// </summary>
        /// <param name="charPointer">包含一个可用字符串内存的首地址</param>
        /// <param name="count">要写入的字符数量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="AccessViolationException"></exception>
        public void Write(char* charPointer, int count)
        {
            if (charPointer == null) throw new ArgumentNullException();
            
            int wrc;
            int index = 0;
            while (count != 0)
            {
                f_isFullToflushAndClear();
                wrc = f_writerToBuffer(charPointer + index, count);
                count -= wrc;
                index += wrc;
            }
        }

        public override void Flush()
        {
            if (IsDispose) return;
            f_flushBuffer();
            p_writer.Flush();
        }

        /// <summary>
        /// 将缓冲区写入到基础封装器并清空缓冲区数据
        /// </summary>
        /// <param name="flushBase">清理缓冲区后是否调用基础封装器的<see cref="TextWriter.Flush"/></param>
        public void Flush(bool flushBase)
        {
            if (IsDispose) return;
            f_flushBuffer();
            if(flushBase) p_writer.Flush();
        }

        #endregion

        #endregion

    }

}
