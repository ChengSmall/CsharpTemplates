using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;

namespace Cheng.Texts
{

    /// <summary>
    /// 对另一个文本读取器添加一个缓冲层
    /// </summary>
    public sealed unsafe class BufferedTextReader : SafeReleaseTextReader
    {

        #region 构造

        /// <summary>
        /// 实例化一个文本读取器缓冲层
        /// </summary>
        /// <param name="reader">封装的文本读取器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BufferedTextReader(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            f_init(reader, true, 1024 * 2);
        }

        /// <summary>
        /// 实例化一个文本读取器缓冲层
        /// </summary>
        /// <param name="reader">封装的文本读取器</param>
        /// <param name="isDispose">在释放实例时是否将封装的读取器一并释放；默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BufferedTextReader(TextReader reader, bool isDispose)
        {
            if (reader is null) throw new ArgumentNullException();
            f_init(reader, isDispose, 1024 * 2);
        }

        /// <summary>
        /// 实例化一个文本读取器缓冲层
        /// </summary>
        /// <param name="reader">封装的文本读取器</param>
        /// <param name="isDispose">在释放实例时是否将封装的读取器一并释放；默认为true</param>
        /// <param name="charBufferSize">指定字符缓冲区可缓存的字符数量；默认为2048</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度小于或等于0</exception>
        public BufferedTextReader(TextReader reader, bool isDispose, int charBufferSize)
        {
            if (reader is null) throw new ArgumentNullException();
            if (charBufferSize <= 0) throw new ArgumentOutOfRangeException();
            f_init(reader, isDispose, charBufferSize);
        }

        private void f_init(TextReader reader, bool isDispose, int bufferSize)
        {
            p_reader = reader;
            p_isDisposeBaseReader = isDispose;
            p_charBuffer = new char[bufferSize];
            p_bufferLength = 0;
            p_bufferPos = 0;
        }

        #endregion

        #region 参数

        private TextReader p_reader;

        private char[] p_charBuffer;

        /// <summary>
        /// 缓冲区当前推进索引
        /// </summary>
        private int p_bufferPos;

        /// <summary>
        /// 缓冲区可用长度
        /// </summary>
        private int p_bufferLength;

        private bool p_isDisposeBaseReader;

        #endregion

        #region 功能

        #region 封装

        /// <summary>
        /// 清空缓冲区并从基础读取器读取数据
        /// </summary>
        private int f_clearAndreadToBuffer()
        {
            p_bufferPos = 0;
            p_bufferLength = p_reader.ReadBlock(p_charBuffer, 0, p_charBuffer.Length);
            return p_bufferLength;
        }

        /// <summary>
        /// 剩余缓冲区可用字符量
        /// </summary>
        /// <returns></returns>
        private int f_resBufSize()
        {
            return p_bufferLength - p_bufferPos;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            
            if (disposeing && p_isDisposeBaseReader && p_reader != null)
            {
                p_reader.Close();
            }
            p_reader = null;

            return true;
        }

        #endregion

        #region 参数访问

        #endregion

        #region 派生

        public override int Peek()
        {

            ThrowObjectDisposed();

            //获取剩余缓存
            int size = f_resBufSize();

            if(size > 0)
            {
                //有缓存
                return p_charBuffer[p_bufferPos];
            }

            //没有缓存
            
            //刷新缓存
            size = f_clearAndreadToBuffer();
            if (size == 0) return -1; //无数据

            return p_charBuffer[p_bufferPos];

        }

        public override int Read()
        {
            ThrowObjectDisposed();

            //获取剩余缓存
            int size = f_resBufSize();

            if (size > 0)
            {
                //有缓存
                return p_charBuffer[p_bufferPos++];
            }

            //没有缓存

            //刷新缓存
            size = f_clearAndreadToBuffer();
            if (size == 0) return -1; //无数据

            return p_charBuffer[p_bufferPos++];

        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();

            int bufLen = buffer.Length;

            if (index < 0 || count < 0 || (index + count > bufLen)) throw new ArgumentOutOfRangeException();

            if (count == 0) return 0;
            
            int cpCountre = 0;

            int canCopyCount = count;

            int copyCount;

            int size;

            Loop:
            //读取剩余缓存量
            size = f_resBufSize();

            if(size > 0)
            {
                //存在剩余缓存
                //获取最小值
                copyCount = System.Math.Min(canCopyCount, size);

                //拷贝指定字符到实例
                Array.Copy(p_charBuffer, p_bufferPos, buffer, index, copyCount);
                //推进参数
                p_bufferPos += copyCount;
                cpCountre += copyCount;
                canCopyCount -= copyCount;
            }

            //拷贝量到达指标
            if (cpCountre == count)
            {
                //拷贝完毕
                return cpCountre;
            }

            //拷贝数小于指定数

            //刷新缓冲区
            size = f_clearAndreadToBuffer();
            if(size == 0)
            {
                //没有了
                return cpCountre;
            }

            //成功刷新缓冲区
            goto Loop;

        }

        public override string ReadToEnd()
        {
            ThrowObjectDisposed();
            return base.ReadToEnd();
        }

        #endregion

        #endregion

    }

}
