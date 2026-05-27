using Cheng.Algorithm;
using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 换行符标准化文本读取器
    /// </summary>
    /// <remarks>
    /// <para>当在文本读取器读取到换行符时，无论是'\n'、'\r\n'、'\r'，全部转换为指定的换行符</para>
    /// </remarks>
    public unsafe sealed class NewLineStdTextReader : SafeReleaseTextReader
    {

        #region 构造

        /// <summary>
        /// 实例化换行符标准化文本读取器，使用系统换行符作为标准换行符
        /// </summary>
        /// <param name="reader">要封装的文本读取器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NewLineStdTextReader(TextReader reader) : this(reader, true, Environment.NewLine, 2048)
        {}

        /// <summary>
        /// 实例化换行符标准化文本读取器，使用系统换行符作为标准换行符
        /// </summary>
        /// <param name="reader">要封装的文本读取器</param>
        /// <param name="disposeBase">是否释放内部封装的实例，true释放，false不释放；该参数默认是true</param>
        public NewLineStdTextReader(TextReader reader, bool disposeBase) : this(reader, disposeBase, Environment.NewLine, 2048)
        {}

        /// <summary>
        /// 实例化换行符标准化文本读取器
        /// </summary>
        /// <param name="reader">要封装的文本读取器</param>
        /// <param name="disposeBase">是否释放内部封装的实例，true释放，false不释放；该参数默认是true</param>
        /// <param name="newLine">要将换行符转换的标准换行符文本</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">换行符字符串是空字符串</exception>
        public NewLineStdTextReader(TextReader reader, bool disposeBase, string newLine) : this(reader, disposeBase, newLine, 2048)
        {}

        /// <summary>
        /// 实例化换行符标准化文本读取器
        /// </summary>
        /// <param name="reader">要封装的文本读取器</param>
        /// <param name="disposeBase">是否释放内部封装的实例，true释放，false不释放；该参数默认是true</param>
        /// <param name="newLine">要将换行符转换的标准换行符文本</param>
        /// <param name="bufferSize">数据缓冲区大小，默认为2048</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小超出范围</exception>
        /// <exception cref="ArgumentException">换行符字符串是空字符串</exception>
        public NewLineStdTextReader(TextReader reader, bool disposeBase, string newLine, int bufferSize)
        {
            if (reader is null || newLine is null) throw new ArgumentNullException();
            if (newLine.Length == 0) throw new ArgumentException();
            p_reader = reader;
            if (bufferSize <= 0 || (bufferSize > (int.MaxValue - 3))) throw new ArgumentOutOfRangeException();
            p_disposeBase = disposeBase;
            p_buffer = new char[bufferSize + 3];
            p_bufPos = 0;
            p_bufLen = 0;
            p_newLineBuf = newLine.ToCharArray();
        }

        /// <summary>
        /// 实例化换行符标准化文本读取器
        /// </summary>
        /// <param name="reader">要封装的文本读取器</param>
        /// <param name="disposeBase">是否释放内部封装的实例，true释放，false不释放；该参数默认是true</param>
        /// <param name="newLine">要将换行符转换的文本</param>
        /// <param name="bufferSize">数据缓冲区大小，默认为2048</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小超出范围</exception>
        /// <exception cref="ArgumentException">给定的标准换行符是空数组</exception>
        public NewLineStdTextReader(TextReader reader, bool disposeBase, char[] newLine, int bufferSize)
        {
            if (reader is null || newLine is null) throw new ArgumentNullException();
            if (newLine.Length == 0) throw new ArgumentException();
            p_reader = reader;
            if (bufferSize <= 0 || (bufferSize > (int.MaxValue - 3))) throw new ArgumentOutOfRangeException();
            p_disposeBase = disposeBase;
            p_buffer = new char[bufferSize + 3];
            p_bufPos = 0;
            p_bufLen = 0;
            p_newLineBuf = (char[])newLine.Clone();
        }

        #endregion

        #region 参数

        private TextReader p_reader;

        private char[] p_buffer;
        private int p_bufPos;
        private int p_bufLen;

        private char[] p_newLineBuf;

        private bool p_disposeBase;

        #endregion

        #region 功能

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            if (disposeing && p_disposeBase)
            {
                p_reader.Close();
            }
            p_reader = null;
            p_buffer = null;
            return true;
        }

        #endregion

        #region 封装

#if DEBUG
        /// <summary>
        /// 从缓冲区读取数据并解析换行符转换为系统换行符（默认无缓冲区数据）
        /// </summary>
        /// <returns>读取的字符数</returns>
#endif
        private int f_readByBuf()
        {
            p_bufPos = 0;
            int rei;
            int index = 0;
            int endLen = p_buffer.Length - 3;

            while (index < endLen)
            {

                rei = p_reader.Read();
                if(rei == -1)
                {
                    p_bufLen = index;
                    return index;
                }

                char c = (char)rei;

                if (c == '\r')
                {
                    //换行符 \r
                    rei = p_reader.Read();
                    if(rei == -1)
                    {
                        //只有一个\r
                        Array.Copy(p_newLineBuf, 0, p_buffer, index, p_newLineBuf.Length);
                        p_bufLen = index + p_newLineBuf.Length;
                        return index;
                    }

                    char nc = (char)rei;
                    if (nc == '\n')
                    {
                        //属于一般换行符
                        Array.Copy(p_newLineBuf, 0, p_buffer, index, p_newLineBuf.Length);
                        index += p_newLineBuf.Length;
                        continue;
                    }
                    else
                    {
                        //不属于\n
                        //写入换行符
                        Array.Copy(p_newLineBuf, 0, p_buffer, index, p_newLineBuf.Length);
                        index += p_newLineBuf.Length;
                        p_buffer[index++] = nc;
                        continue;
                    }

                }
                else if(c == '\n')
                {
                    //换行符 \n
                    //写入系统换行符
                    Array.Copy(p_newLineBuf, 0, p_buffer, index, p_newLineBuf.Length);
                    index += p_newLineBuf.Length;
                    continue;
                }

                p_buffer[index++] = c;

            }

            return index;
        }

#if DEBUG
        /// <summary>
        /// 将当前缓冲区数据写入缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <returns>实际读取到的字符数量</returns>
#endif
        private int f_readTo(char* buffer, int count)
        {
            const int sizeBlock = (int.MaxValue - 1) / 2;
            count = Maths.Min(count, sizeBlock, p_bufLen);
            if (count == 0) return 0;
            fixed (char* cbuf = p_buffer)
            {
                MemoryOperation.MemoryCopy((cbuf + p_bufPos), buffer, count * sizeof(char));
            }
            p_bufPos += count;
            return count;
        }

        #endregion

        public override int Peek()
        {
            ThrowObjectDisposed(nameof(NewLineStdTextReader));
            if(p_bufPos >= p_bufLen)
            {
                if (f_readByBuf() == 0) return -1;
            }

            return p_buffer[p_bufPos];
        }

        public override int Read()
        {
            ThrowObjectDisposed(nameof(NewLineStdTextReader));
            if (p_bufPos >= p_bufLen)
            {
                if (f_readByBuf() == 0) return -1;
            }

            return p_buffer[p_bufPos++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed(nameof(NewLineStdTextReader));
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* buf = buffer)
            {
                if (p_bufPos >= p_bufLen)
                {
                    if (f_readByBuf() == 0) return 0;
                }
                return f_readTo(buf + index, count);
            }
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed(nameof(NewLineStdTextReader));
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            int rec = 0;
            fixed (char* buf = buffer)
            {
                while (count > 0)
                {
                    if (p_bufPos >= p_bufLen)
                    {
                        if (f_readByBuf() == 0) return rec;
                    }
                    var re = f_readTo(buf + index, count);
                    count -= re;
                    index += re;
                    rec += re;
                }
            }
            return rec;
        }

        #endregion

    }

}
#if DEBUG
#endif