using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Cheng.Memorys;
using Cheng.IO;
using Cheng.Algorithm;
using System.Runtime.CompilerServices;

namespace Cheng.Streams
{

    /// <summary>
    /// 将<see cref="Stream"/>对象的某一段固定长度的数据进行单独操作
    /// </summary>
    /// <remarks>
    /// <para>将<see cref="Stream"/>对象的某一段数据进行单独操作，要求封装的<see cref="Stream"/>对象必须有读取和查找功能，如果封装的<see cref="Stream"/>拥有写入功能，则可以在数据段内的任意位置进行覆写，但是无法写入超出数据段范围的数据</para>
    /// <para>注意，即便对象本身支持使用<see cref="Seek(long, SeekOrigin)"/>等方法进行随机读写，但不能使用<see cref="SetLength(long)"/>修改长度</para>
    /// </remarks>
    public unsafe sealed class PartialStream : HEStream
    {

        #region 结构

        /// <summary>
        /// 在进行写入数据时无法完整写入引发的异常
        /// </summary>
        public class WriterOffsetOutException : Exception
        {
            internal WriterOffsetOutException() : base()
            {
            }
            internal WriterOffsetOutException(string message, int notWriteCount) : base(message)
            {
                p_notWriteCount = notWriteCount;
            }

            private int p_notWriteCount;

            /// <summary>
            /// 引发异常前还需要写入的剩余字节大小
            /// </summary>
            public int NotWrittenSize
            {
                get => p_notWriteCount;
            }

        }
        
        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if (disposing)
            {
                f_flush(false);
                if(p_disposeBase) p_stream.Close();
            }
            p_stream = null;
            return true;
        }

        #endregion

        #region 初始化

        const int cp_defInitBufSize = 1024 * 8;

        /// <summary>
        /// 实例化部分流
        /// </summary>
        /// <param name="stream">要封装的基础流对象</param>
        /// <param name="offset">流数据的起始位置偏移</param>
        /// <param name="length">从起始偏移开始的字节长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象不能查找和读取</exception>
        /// <exception cref="ArgumentException">偏移参数超出范围或缓冲区参数不大于0</exception>
        public PartialStream(Stream stream, long offset, long length) : this(stream, offset, length, true, cp_defInitBufSize)
        {
        }

        /// <summary>
        /// 实例化部分流
        /// </summary>
        /// <param name="stream">要封装的基础流对象</param>
        /// <param name="offset">流数据的起始位置偏移</param>
        /// <param name="length">从起始偏移开始的字节长度</param>
        /// <param name="disposeBaseStream">在释放时是否调用基础对象的释放函数，默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象不能查找和读取</exception>
        /// <exception cref="ArgumentException">偏移参数超出范围或缓冲区参数不大于0</exception>
        public PartialStream(Stream stream, long offset, long length, bool disposeBaseStream) :this(stream, offset, length, disposeBaseStream, cp_defInitBufSize)
        {
        }

        /// <summary>
        /// 实例化部分流
        /// </summary>
        /// <param name="stream">要封装的基础流对象</param>
        /// <param name="offset">流数据的起始位置偏移</param>
        /// <param name="length">从起始偏移开始的字节长度</param>
        /// <param name="disposeBaseStream">在释放时是否调用基础对象的释放函数，默认为true</param>
        /// <param name="bufferSize">指定缓冲区字节长度，默认为8192</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象不能查找和读取</exception>
        /// <exception cref="ArgumentException">偏移参数超出范围或缓冲区参数不大于0</exception>
        public PartialStream(Stream stream, long offset, long length, bool disposeBaseStream, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            if (!(stream.CanSeek && stream.CanRead)) throw new NotSupportedException();

            if (offset + length > stream.Length)
            {
                throw new ArgumentException();
            }

            p_buffer = new byte[bufferSize];
            p_stream = stream;
            p_offset = offset;
            p_length = length;
            p_disposeBase = disposeBaseStream;
            p_canTimeout = stream.CanTimeout;
            p_mode = 0;
            p_ptr_pos = 0;
            p_ptr_bufPos = 0;
            p_ptr_bufPosEnd = -1;
            p_canWrite = p_stream.CanWrite;
        }

        #endregion

        #region 参数

#if DEBUG
        /// <summary>
        /// 截断的起始偏移
        /// </summary>
#endif
        private readonly long p_offset;

#if DEBUG
        /// <summary>
        /// 截断的长度
        /// </summary>
#endif
        private readonly long p_length;

        private Stream p_stream;

#if DEBUG
        /// <summary>
        /// 公用数据缓冲区
        /// </summary>
#endif
        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 当前内部截断的流所在的模拟位置
        /// </summary>
        /// <remarks>
        /// <para>读取模式：读取缓冲区前的流所指向的模拟位置</para>
        /// <para>写入模式：在缓冲区写入流之前，流当前要写入的模拟位置</para>
        /// </remarks>
#endif
        private long p_ptr_pos;


#if DEBUG
        /// <summary>
        /// 缓冲区当前的位置
        /// </summary>
        /// <remarks>
        /// <para>读取模式：缓冲区数据当前的可用位置</para>
        /// <para>写入模式：缓冲区当前写入到的位置</para>
        /// </remarks>
#endif
        private int p_ptr_bufPos;

#if DEBUG
        /// <summary>
        /// 缓冲区剩余可用的数据末端位置
        /// </summary>
        /// <remarks>
        /// <para>读取模式：此次缓冲区的末端位置</para>
        /// </remarks>
#endif
        private int p_ptr_bufPosEnd;

#if DEBUG
        /// <summary>
        /// 模式 空:0 读:1 写:2
        /// </summary>
#endif
        private byte p_mode;

        private bool p_disposeBase;

        private bool p_canTimeout;

        private bool p_canWrite;

        #endregion

        #region 功能

        #region 实现

        #region 读取模式

#if DEBUG
        /// <summary>
        /// 读取模式：缓冲区可用长度
        /// </summary>
        /// <returns></returns>
#endif
        private int f_readMode_bufLen()
        {
            return p_ptr_bufPosEnd - p_ptr_bufPos + 1;
        }

#if DEBUG
        /// <summary>
        /// 尝试切换到读取模式
        /// </summary>
#endif
        private void f_checkReadMode()
        {
            if (p_mode == 1) return; //读模式不切

            //if (p_mode == 0)
            //{
            //    //清空缓冲区
            //    //p_ptr_bufPos = 0;
            //    //p_ptr_bufPosEnd = -1;
            //}
            if(p_mode == 2)
            {
                f_wrMode_flushBuf();
            }

            p_mode = 1;
        }

#if DEBUG
        /// <summary>
        /// 读取模式：仅清空读取数据的缓冲区
        /// </summary>
#endif
        private void f_readMode_clearBuf()
        {
            p_ptr_bufPos = 0;
            p_ptr_bufPosEnd = -1;
        }

#if DEBUG
        /// <summary>
        /// 读取模式：获取当前流指针位置
        /// </summary>
        /// <returns></returns>
#endif
        private long f_readMode_nowPos()
        {
            return p_ptr_pos + (long)p_offset + (long)p_ptr_bufPos;
        }

#if DEBUG
        /// <summary>
        /// 从基础流中读取数据到缓冲区（保证缓冲区为空）
        /// </summary>
        /// <returns>此次读取到的字节数</returns>
#endif
        private int f_readMode_readBaseToBuf()
        {
            //要切换到的真实起始位置
            var ptr_realPos = p_ptr_pos + p_offset;
            int re;

            //截断流最后的位置后一位
            //var endAddoffset = p_length + p_offset;

            //此次读取，从当前位置到最后的截断的剩余长度
            var maxLen = (p_length) - (p_ptr_pos);

            lock (p_stream)
            {
                if (p_stream.Position != ptr_realPos)
                {
                    p_stream.Seek(ptr_realPos, SeekOrigin.Begin);
                }
                re = p_stream.Read(p_buffer, 0, (int)Math.Min(p_buffer.Length, maxLen));
            }

            p_ptr_bufPosEnd = re - 1;
            return re;
        }

#if DEBUG
        /// <summary>
        /// 清除缓冲区并校准流位置
        /// </summary>
#endif
        private void f_readMode_clearBufNext()
        {
            p_ptr_pos = p_ptr_pos + p_ptr_bufPos;

            p_ptr_bufPos = 0;
            p_ptr_bufPosEnd = -1;
        }

        #if DEBUG
        /// <summary>
        /// 从当前位置读取数据
        /// </summary>
        /// <param name="buffer">要读取到的缓冲区</param>
        /// <param name="count">要读取的字节数量</param>
        /// <returns>此次实际读取的数量</returns>
        #endif
        private int f_readData(byte* buffer, int count)
        {
            int readCount = 0;
            //切换
            f_checkReadMode();
            int bufLen;

            Loop:
            //当前缓冲区长度
            if (count == 0) return readCount;
            bufLen = f_readMode_bufLen();
            if (bufLen == 0)
            {
                //缓冲区无剩余
                f_readMode_clearBufNext();
                //从基础流读取
                bufLen = f_readMode_readBaseToBuf();
                if(bufLen == 0)
                {
                    return readCount; //完全到达结尾
                }
            }
            int cpCount;
            fixed (byte* bufPtr = p_buffer)
            {
                //要拷贝的字节数
                cpCount = Math.Min(bufLen, count);

                Buffer.MemoryCopy(bufPtr + p_ptr_bufPos, buffer + readCount, count, cpCount);
                //推进缓冲区
                p_ptr_bufPos += cpCount;
                //减少拷贝数
                count -= cpCount;
                //添加读取字节数返回值
                readCount += cpCount;
            }

            goto Loop;
        }

        #endregion

        #region 写入模式

        #if DEBUG
        /// <summary>
        /// 写入模式：获取当前写入包括缓冲区内长度的实际位置
        /// </summary>
        /// <returns></returns>
        #endif
        private long f_wrMode_nowPos()
        {
            return p_ptr_pos + (long)p_offset + (long)p_ptr_bufPos;
        }

        #if DEBUG
        /// <summary>
        /// 写入模式：当前缓冲区已写入的长度
        /// </summary>
        /// <returns></returns>
        #endif
        private int f_wrMode_bufLen()
        {
            return p_ptr_bufPosEnd - p_ptr_bufPos + 1;
        }

        #if DEBUG
        /// <summary>
        /// 尝试切换到写入模式
        /// </summary>
        #endif
        private void f_checkWriteMode()
        {
            if (p_mode == 2) return;
            if(p_mode == 1) f_readMode_clearBufNext();
            //p_ptr_bufPos = 0;
            //p_ptr_bufPosEnd = -1;
            p_mode = 2;
        }

        #if DEBUG
        /// <summary>
        /// 将当前缓冲区数据全部写入基础流并推进模拟位置（无视边界检查）
        /// </summary>
        #endif
        private void f_wrMode_flushBuf()
        {
            int len;
            //写入流的真实首偏移
            var wrPosOffset = p_ptr_pos + (long)p_offset;

            len = f_wrMode_bufLen();
            if(len == 0)
            {
                goto OnLockOver;
            }
            lock (p_stream)
            {
                if (p_stream.Position != wrPosOffset)
                {
                    p_stream.Seek(wrPosOffset, SeekOrigin.Begin);
                }
                p_stream.Write(p_buffer, p_ptr_bufPos, len);
            }
            OnLockOver:
            //推进指针
            p_ptr_pos += len;
            //清空缓冲区
            p_ptr_bufPos = 0;
            p_ptr_bufPosEnd = -1;
        }

        #if DEBUG
        /// <summary>
        /// 获取当前写入模式下当前位置的剩余可写字节数
        /// </summary>
        /// <returns></returns>
        #endif
        private long f_wrMode_nowPosLcaNextEnd()
        {
            return p_length - p_ptr_pos;
        }

        #if DEBUG
        /// <summary>
        /// 将指定数据写入一次缓冲区
        /// </summary>
        /// <param name="buffer">要写入的数据位置</param>
        /// <param name="count">写入的字节数</param>
        /// <returns>
        /// <para>参数1: 实际写入的字节量</para>
        /// <para>参数2: 写入后缓冲区剩余可写的字节数</para>
        /// </returns>
        #endif
        private (int, int) f_writeOnce(byte* buffer, long count)
        {
            var blen = p_buffer.Length;

            //缓冲区剩余可写长度
            var oLen = blen - (p_ptr_bufPosEnd + 1);

            //写入长度
            int wrLen = (int)Math.Min(oLen, count);
            if(wrLen != 0)
            {
                fixed (byte* bufPtr = p_buffer)
                {
                    Buffer.MemoryCopy(buffer, bufPtr + (p_ptr_bufPosEnd + 1), oLen, wrLen);
                }
            }

            p_ptr_bufPosEnd += wrLen;

            return (wrLen, blen - (p_ptr_bufPosEnd + 1));
        }

        #if DEBUG
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer">要写入的数据</param>
        /// <param name="count">要写入的字节数</param>
        /// <returns>实际写入的字节量，0表示到达末尾</returns>
        #endif
        private long f_writeData(byte* buffer, long count)
        {
            f_checkWriteMode();

            int isWrC = 0;

            Loop:
            var wrLen = f_wrMode_bufLen();
            if(wrLen == p_buffer.Length)
            {
                f_wrMode_flushBuf();
            }

            //剩余可写字节数
            var lastWrCount = f_wrMode_nowPosLcaNextEnd();
            //安全写入字节数
            long minWrC = Math.Min(lastWrCount, count);

            if (minWrC == 0) return isWrC;

            var ore = f_writeOnce(buffer + isWrC, minWrC);
            count -= ore.Item1;
            //记录写入数
            isWrC += ore.Item1;
            goto Loop;

        }

        #endregion

        #region seek

        private void f_setPos(long offset)
        {
            if(p_mode == 2)
            {
                //写入缓冲区
                f_wrMode_flushBuf();
                p_mode = 0;
            }
            else if(p_mode == 1)
            {
                f_readMode_clearBufNext();
                p_mode = 0;
            }

            p_ptr_pos = offset;
        }

        private long f_getNowPos()
        {
            return p_ptr_pos + p_ptr_bufPos;
        }

        #endregion

        #region flush

        private void f_flush(bool wrBase)
        {
            if(p_mode == 2)
            {
                f_wrMode_flushBuf();
                if (wrBase)
                {
                    p_stream.Flush();
                }
            }
        }

        #endregion

        #endregion

        #region 异常消息字符串

#if DEBUG
        /// <summary>
        /// 写入时无法完整写入数据的异常消息
        /// </summary>
        /// <returns></returns>
#endif
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static string f_getWriterOffsetExcMeg()
        {
            return Cheng.Properties.Resources.Exception_PartialStreamNotWriterAllData;
        }

        #endregion

        #region  API

        #region 权限

        public override bool CanRead => true;

        public override bool CanWrite => p_canWrite;

        public override bool CanSeek => true;

        #endregion

        #region 参数访问

        /// <summary>
        /// 获取封装的内部对象，如果已释放返回null
        /// </summary>
        public Stream BaseStream
        {
            get => p_stream;
        }

        #endregion

        #region 派生

        public override long Length
        {
            get
            {
                ThrowIsDispose(nameof(PartialStream));
                return p_length;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIsDispose(nameof(PartialStream));
                return f_getNowPos();
            }
            set
            {
                ThrowIsDispose(nameof(PartialStream));
                value = value.Clamp(0, p_length);
                if (value < 0 || value > p_length) throw new IOException();
                f_setPos(value);
            }
        }

        public override void Flush()
        {
            if (IsDispose) return;
            f_flush(true);
        }

        /// <summary>
        /// 清除当前对象的缓冲区，并指定是否将基础封装对象一并清理
        /// </summary>
        /// <param name="writeBaseStream">如果参数是true，则在清理缓冲区后调用内部封装流的<see cref="Stream.Flush"/>；false则不会调用</param>
        public void Flush(bool writeBaseStream)
        {
            if (IsDispose) return;
            f_flush(writeBaseStream);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose(nameof(PartialStream));

            long value;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    value = offset;
                    break;
                case SeekOrigin.Current:
                    value = p_ptr_pos + p_ptr_bufPos + offset;
                    break;
                case SeekOrigin.End:
                    value = p_length + offset;
                    break;
                default:
                    throw new ArgumentException();
            }

            value = value.Clamp(0, p_length);
            f_setPos(value);
            return p_ptr_pos + p_ptr_bufPos;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(PartialStream));

            if (buffer is null) throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || count < 0 || (offset + count) > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* buf = buffer)
            {
                return f_readData(buf, count);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(PartialStream));
            if (!p_canWrite) throw new NotSupportedException();

            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || (offset + count) > buffer.Length) throw new ArgumentOutOfRangeException();

            long re;
            fixed (byte* buf = buffer)
            {
                re = f_writeData(buf, count);
            }
            if(re != count)
            {
                throw new WriterOffsetOutException(f_getWriterOffsetExcMeg(), (int)(count - re));
            }
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(PartialStream));
            byte re;
            var rec = f_readData(&re, 1);
            if (rec == 0) return -1;
            return re;
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose(nameof(PartialStream));
            if (!p_canWrite) throw new NotSupportedException();
            var re = f_writeData(&value, 1);
            if (re < 1) throw new WriterOffsetOutException(f_getWriterOffsetExcMeg(), 1);
        }

        #endregion

        #region 不实现

        public override void SetLength(long value)
        {
            ThrowIsDispose(nameof(PartialStream));
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #endregion

    }

}
#if DEBUG
#endif