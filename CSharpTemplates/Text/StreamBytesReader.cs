using Cheng.Texts;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Cheng.Memorys;

namespace Cheng.IO
{

    /// <summary>
    /// 字节文本读取器
    /// </summary>
    /// <remarks>
    /// <para>封装一个<see cref="Stream"/>，从中读取字节数据并把每个字节值转化为文本</para>
    /// <para>中间可添加分隔符，比如FF-FF-FF-FF；没有分隔符则每个字节数据之间没有空隙</para>
    /// </remarks>
    public sealed class StreamBytesReader : SafeReleaseTextReader
    {

        #region 构造

        /// <summary>
        /// 实例化一个字节文本读取器
        /// </summary>
        /// <param name="stream">封装的可读流</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public StreamBytesReader(Stream stream) : this(stream, false, string.Empty, true, cp_defBuffSize)
        {
        }

        /// <summary>
        /// 实例化一个字节文本读取器
        /// </summary>
        /// <param name="stream">封装的可读流</param>
        /// <param name="toUpper">字节文本的字母是否采用大写字母，默认是false</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public StreamBytesReader(Stream stream, bool toUpper) : this(stream, toUpper, string.Empty, true, cp_defBuffSize)
        {
        }

        /// <summary>
        /// 实例化一个字节文本读取器
        /// </summary>
        /// <param name="stream">封装的可读流</param>
        /// <param name="toUpper">字节文本的字母是否采用大写字母，默认是false</param>
        /// <param name="separator">每个字节之间的分隔符，如果是null或空字符串则表示无分隔符</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public StreamBytesReader(Stream stream, bool toUpper, string separator) : this(stream, toUpper, separator, true, cp_defBuffSize)
        {
        }

        /// <summary>
        /// 实例化一个字节文本读取器
        /// </summary>
        /// <param name="stream">封装的可读流</param>
        /// <param name="toUpper">字节文本的字母是否采用大写字母，默认是false</param>
        /// <param name="separator">每个字节之间的分隔符，如果是null或空字符串则表示无分隔符</param>
        /// <param name="onDispose">在释放时是否释放封装流；默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public StreamBytesReader(Stream stream, bool toUpper, string separator, bool onDispose) : this(stream, toUpper, separator, onDispose, cp_defBuffSize)
        {
        }

        /// <summary>
        /// 实例化一个字节文本读取器
        /// </summary>
        /// <param name="stream">封装的可读流</param>
        /// <param name="toUpper">字节文本的字母是否采用大写字母，默认是false</param>
        /// <param name="separator">每个字节之间的分隔符，如果是null或空字符串则表示无分隔符</param>
        /// <param name="onDispose">在释放时是否释放封装流；默认为true</param>
        /// <param name="bufferSize">字符缓冲区大小；默认是1024</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public StreamBytesReader(Stream stream, bool toUpper, string separator, bool onDispose, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            if (!stream.CanRead) throw new NotSupportedException();
            if (separator is null) separator = string.Empty;

            p_stream = stream;
            p_separator = separator;
            p_charBuf = new char[bufferSize * (2 + separator.Length)];
            p_onDispose = onDispose;
            p_bufLen = 0;
            p_bufPos = 0;
            //p_buffer = new byte[bufferSize];
            p_toUpper = toUpper;
        }

        #endregion

        #region 参数
        private const int cp_defBuffSize = 1024;

        private Stream p_stream;
        private char[] p_charBuf;
        private string p_separator;
        private int p_bufPos;
        private int p_bufLen;
        private bool p_toUpper;
        private bool p_onDispose;

        #endregion

        #region 功能

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            if (p_onDispose && disposeing)
            {
                p_stream?.Close();
            }
            p_stream = null;

            return true;
        }

        #endregion

        #region 封装

        /// <summary>
        /// 从基础流读取数据到缓冲区（保证缓冲区空）
        /// </summary>
        /// <returns>读取的字节转化后的字符数</returns>
        private int f_readToBuf()
        {
            p_bufLen = 0;
            p_bufPos = 0;
            //读取到字节缓冲区
            char left, right;

            while (p_bufLen < p_charBuf.Length)
            {

                int reV = p_stream.ReadByte();

                if (reV < 0) return p_bufLen - p_bufPos;

                //写入到字符缓冲区
                TextManipulation.ValueToX16Char((byte)reV, p_toUpper, out left, out right);

                p_charBuf[p_bufLen] = left;
                p_charBuf[p_bufLen + 1] = right;
                p_separator.CopyTo(0, p_charBuf, p_bufLen + 2, p_separator.Length);

                p_bufLen += (p_separator.Length + 2);
            }

            return p_bufLen - p_bufPos;
        }

        private int f_read(char[] buffer, int index, int count)
        {
            int lastCount = p_bufLen - p_bufPos;
            if (lastCount == 0)
            {
                lastCount = f_readToBuf();
                if (lastCount == 0) return 0;
            }

            int copyCount = Math.Min(lastCount, count);
            Array.Copy(p_charBuf, p_bufPos, buffer, index, copyCount);
            p_bufPos += copyCount;
            return copyCount;
        }

        #endregion

        #region 派生

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return 0;
            return f_read(buffer, index, count);
        }

        public override int Read()
        {
            if (p_bufPos == p_bufLen)
            {
                if (f_readToBuf() == 0) return -1;
            }

            return p_charBuf[p_bufPos++];
        }

        public override int Peek()
        {
            if (p_bufPos == p_bufLen)
            {
                if (f_readToBuf() == 0) return -1;
            }

            return p_charBuf[p_bufPos];
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return 0;
            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = f_read(buffer, index, count);
                if (rsize == 0) return re;
                index += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        #endregion

        #endregion

    }

}
