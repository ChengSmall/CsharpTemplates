using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace Cheng.Streams
{

    /// <summary>
    /// 可将流数据截断为一部分的只读流
    /// </summary>
    /// <remarks>
    /// <para>封装指定的流对象，将其截断其中一部分数据作为只读流；封装的流必须要有读取和查找功能</para>
    /// <para>对封装的同一个内部<see cref="System.IO.Stream"/>线程安全（类型本身并不是线程安全的）</para>
    /// </remarks>
    public unsafe class TruncateStream : HEStream
    {

        #region 释放

        /// <summary>
        /// 重写该函数可自定义释放代码，需要调用基实现
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected override bool Disposing(bool disposing)
        {
            if (p_isFree && disposing)
            {
                p_stream.Close();
            }
            p_stream = null;
            return true;
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个只读截断流
        /// </summary>
        /// <param name="stream">要截断的流</param>
        /// <param name="startPosition">截断的流起始位置</param>
        /// <param name="length">要截断的字节量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的参数超出范围</exception>
        /// <exception cref="ArgumentException">给定的流不支持查找</exception>
        public TruncateStream(Stream stream, long startPosition, long length) : this(stream, startPosition, length, true, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 实例化一个只读截断流
        /// </summary>
        /// <param name="stream">要截断的流</param>
        /// <param name="startPosition">截断的流起始位置</param>
        /// <param name="length">要截断的字节量</param>
        /// <param name="freeBaseStream">在释放资源时是否释放封装的基础流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的参数超出范围</exception>
        /// <exception cref="ArgumentException">给定的流不支持查找</exception>
        public TruncateStream(Stream stream, long startPosition, long length, bool freeBaseStream) : this(stream, startPosition, length, freeBaseStream, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 实例化一个只读截断流
        /// </summary>
        /// <param name="stream">要截断的流</param>
        /// <param name="startPosition">截断的流起始位置</param>
        /// <param name="length">要截断的字节量</param>
        /// <param name="freeBaseStream">在释放资源时是否释放封装的基础流，默认为true</param>
        /// <param name="bufferSize">指定缓冲区容量，以字节为单位；默认为4096</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的参数超出范围</exception>
        /// <exception cref="ArgumentException">给定的流不支持查找</exception>
        public TruncateStream(Stream stream, long startPosition, long length, bool freeBaseStream, int bufferSize)
        {
            f_initThrow(stream, startPosition, length, bufferSize);
            initStream(stream, startPosition, length, freeBaseStream, bufferSize);
        }

        /// <summary>
        /// 空的构造函数，若要初始化需要调用<see cref="initStream(Stream, long, long, bool, int)"/>
        /// </summary>
        protected TruncateStream()
        {
        }

        private void f_initThrow(Stream stream, long startPosition, long length, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException();

            if ((!stream.CanSeek) || (!stream.CanRead)) throw new NotSupportedException(Cheng.Properties.Resources.StreamParserDef_NotSeekAndRead);

            if (startPosition < 0 || length <= 0) throw new ArgumentOutOfRangeException();

            if(stream.Length < startPosition + length || (bufferSize <= 0)) throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 使用给定参数初始化内部字段和数据
        /// </summary>
        /// <param name="stream">封装的流</param>
        /// <param name="startPosition">截断的起始位置，不要超过封装流的范围</param>
        /// <param name="length">截断的长度，不要超过封装流的范围</param>
        /// <param name="freebaseStream">释放当前实例时是否释放封装的流</param>
        /// <param name="bufferSize">当前流的缓冲区容量，要大于0</param>
        protected void initStream(Stream stream, long startPosition, long length, bool freebaseStream, int bufferSize)
        {
            p_stream = stream;
            p_startPos = startPosition;
            p_endPos = startPosition + length - 1;
            p_isFree = freebaseStream;

            p_buffer = new byte[length < bufferSize ? length : bufferSize];
            p_bufPos = 0;
            p_bufPosEnd = -1;
            p_nowPos = p_startPos;
        }

        private const int cp_defBufferSize = 1024 * 4;
        #endregion

        #region 参数
 
        /// <summary>
        /// 缓冲区当前可用位置
        /// </summary>
        private int p_bufPos;

        /// <summary>
        /// 缓冲区剩余可用的数据末端位置
        /// </summary>
        private int p_bufPosEnd;

        /// <summary>
        /// 截断流的起始位置
        /// </summary>
        private long p_startPos;

        /// <summary>
        /// 截断流的终止位置
        /// </summary>
        private long p_endPos;

        /// <summary>
        /// 记录当前位置所在的基础流的原始指针
        /// </summary>
        private long p_nowPos;

        /// <summary>
        /// 内部封装的流
        /// </summary>
        protected Stream p_stream;

        private byte[] p_buffer;

        private bool p_isFree;

        #endregion

        #region 派生

        #region 封装

        /// <summary>
        /// 按索引区间计算剩余可用缓存量
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private static int f_bufferHaveReadCount(int beginIndex, int endIndex)
        {
            return endIndex - beginIndex + 1;
        }

        /// <summary>
        /// 查看缓存未使用容量
        /// </summary>
        /// <param name="length">缓存长度</param>
        /// <param name="endPos">缓存末端指针</param>
        /// <returns></returns>
        private static int f_bufferNextCount(int length, int endPos)
        {
            return length - (endPos + 1);
        }

        /// <summary>
        /// 将缓冲区指针的指向设为清空数据
        /// </summary>
        /// <param name="beginIndex">起始指针</param>
        /// <param name="endIndex">末端指针</param>
        private static void f_bufferClear(ref int beginIndex, ref int endIndex)
        {
            beginIndex = 0;
            endIndex = -1;
        }

        /// <summary>
        /// 读取基础流数据到缓存（缓存必须为空）
        /// </summary>
        /// <param name="bufCount">缓冲区要读取的量</param>
        /// <returns>此次读取的量</returns>
        private int f_bufferReadbase(out int bufCount)
        {
            var buf = p_buffer;

            //获取缓存容量
            //int count = f_bufferNextCount(buf.Length, p_bufPosEnd);
            //int count = buf.Length - (p_bufPosEnd + 1);
            int count = buf.Length;

            //获取截断流所截断的数据区域 其剩余字节量
            var lastNowCount = ((p_endPos - p_nowPos) + 1);

            //如果截断流剩余数据小于缓存容量
            if (lastNowCount < count)
            {
                //将count设为截断流剩余截断长度
                count = (int)lastNowCount;
            }
            //写入从基础流读取的理想字节数
            bufCount = count;

            if (count == 0)
            {
                return 0; //没有可读取数据
            }

            //获取基础流位置
            int re;

            lock (p_stream)
            {
                long spos = p_stream.Position;
                //判断基础流不在内部指向的位值
                if (spos != p_nowPos)
                {
                    //调整基础流位置
                    p_nowPos = p_stream.Seek(p_nowPos, SeekOrigin.Begin);
                    if (p_nowPos < p_startPos)
                    {
                        p_nowPos = p_startPos;
                        return 0;
                        //p_nowPos = p_startPos;
                    }
                    else if (p_nowPos > p_endPos)
                    {
                        return 0;
                    }
                }
                //从基础流读取
                re = p_stream.Read(buf, 0, count);
            }

            p_bufPosEnd += re;

            return re;
        }

        /// <summary>
        /// 清空缓存数据并重置缓存指针
        /// </summary>
        private void f_bufferClear()
        {
            //如果存在缓存则先将记录的基础流位置变更到理想位置
            if(p_bufPosEnd >= 0) p_nowPos += p_bufPos;
            //清空缓存指针
            p_bufPos = 0;
            p_bufPosEnd = -1;
        }

        /// <summary>
        /// 将缓存读取到指定数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns>读取到的字节</returns>
        private int f_readBuffer(byte[] buffer, int offset, int count)
        {
            //int bufCount = f_bufferHaveReadCount(p_bufPos, p_bufPosEnd);
            //当前缓存的字节量
            int bufCount = p_bufPosEnd - p_bufPos + 1;

            //最多只能读取的字节数
            int rc = (bufCount > count) ? count : bufCount;

            if(rc == 0)
            {
                return 0;
            }

            if (rc == 1)
            {
                buffer[offset] = p_buffer[p_bufPos++];
                return 1;
            }
            
            Buffer.BlockCopy(p_buffer, p_bufPos, buffer, offset, rc);
            //Array.Copy(p_buffer, p_bufPos, buffer, offset, rc);
            //fixed (byte* orcBuf = p_buffer, toBuf = buffer)
            //{
            //    Memorys.MemoryOperation.MemoryCopy(orcBuf + p_bufPos, toBuf + offset, rc);
            //}

            p_bufPos += rc;

            return rc;
        }

        private int f_readPtr(byte* buffer, int count)
        {
            int bufCount = p_bufPosEnd - p_bufPos + 1;

            //最多只能读取的字节数
            int rc = (bufCount > count) ? count : bufCount;

            if (rc == 0)
            {
                return 0;
            }

            if (rc == 1)
            {
                buffer[0] = p_buffer[p_bufPos++];
                return 1;
            }

            fixed (byte* orcBuf = p_buffer)
            {
                Cheng.Memorys.MemoryOperation.MemoryCopy(orcBuf + p_bufPos, buffer, rc);
            }

            p_bufPos += rc;

            return rc;
        }

        private int f_readOnce(byte* buffer, int count)
        {
            int bufCount;
            int re;

            //获取缓存可用量
            bufCount = p_bufPosEnd - p_bufPos + 1;
            //bufCount = f_bufferHaveReadCount(p_bufPos, p_bufPosEnd);

            //判断缓存为空
            if (bufCount == 0)
            {
                //没有缓存
                //重置缓存指针（清空缓存）
                f_bufferClear();

                if (p_nowPos < p_startPos)
                {
                    p_nowPos = p_startPos;
                }
                else if (p_nowPos > p_endPos)
                {
                    return 0;
                }

                //从基础流读取到缓存
                re = f_bufferReadbase(out _);
                if (re == 0)
                {
                    //没有基础数据
                    return 0;
                }
            }

            //到这里一定有缓存数据
            //读取缓存到buffer
            re = f_readPtr(buffer, count);

            return re;
        }

        private int f_readOnce(byte[] buffer, int offset, int count)
        {
            int bufCount;
            int re;

            //获取缓存可用量
            bufCount = p_bufPosEnd - p_bufPos + 1;

            //判断缓存为空并读取基础流
            if (bufCount == 0)
            {
                //没有缓存
                //重置缓存指针（清空缓存）
                f_bufferClear();

                //判断范围
                if (p_nowPos < p_startPos)
                {
                    p_nowPos = p_startPos;
                }
                else if (p_nowPos > p_endPos)
                {
                    return 0;
                }

                //从基础流读取到缓存
                re = f_bufferReadbase(out _);
                if (re == 0)
                {
                    //没有基础数据
                    return 0;
                }
            }

            //读取缓存到buffer
            re = f_readBuffer(buffer, offset, count);

            return re;
        }

        #endregion

        #region 参数

        public override long Position
        {
            get
            {
                ThrowIsDispose(nameof(TruncateStream));
                return (p_nowPos + p_bufPos) - p_startPos;
            }
            set
            {
                ThrowIsDispose(nameof(TruncateStream));
                //if (value < 0 || (value >= Length))
                //{
                //    throw new ArgumentOutOfRangeException("value");
                //}
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIsDispose(nameof(TruncateStream));
                return (p_endPos + 1) - p_startPos;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                ThrowIsDispose(nameof(TruncateStream));
                return p_stream.WriteTimeout;
            }
            set
            {
                ThrowIsDispose(nameof(TruncateStream));
                p_stream.WriteTimeout = value;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return (p_stream?.CanTimeout).GetValueOrDefault();
            }
        }

        public override int ReadTimeout
        {
            get
            {
                ThrowIsDispose(nameof(TruncateStream));
                return p_stream.ReadTimeout;
            }
            set
            {
                ThrowIsDispose(nameof(TruncateStream));
                p_stream.ReadTimeout = value;
            }
        }

        #endregion

        #region 功能

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose(nameof(TruncateStream));
            if (buffer == null) throw new ArgumentNullException();

            int rsize;
            int re = 0;
            int offset = 0;
            while (count != 0)
            {
                rsize = f_readOnce(buffer + offset, count);
                if (rsize == 0) return re;
                offset += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;

        }

        /// <summary>
        /// 在该类型中表示为无效操作
        /// </summary>
        public override void Flush()
        {
            //no code 哈~
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose(nameof(TruncateStream));
            
            long value;

            //f_bufferClear();
            if (p_bufPosEnd >= 0) p_nowPos += p_bufPos;
            p_bufPos = 0;
            p_bufPosEnd = -1;
            var endNext = (p_endPos + 1);
            if (origin == SeekOrigin.Begin)
            {
                value = p_startPos + offset;
            }
            else if (origin == SeekOrigin.End)
            {
                value = endNext + offset;

            }
            else if(origin == SeekOrigin.Current)
            {
                value = p_nowPos + offset;
            }
            else
            {
                throw new ArgumentException();
            }

            if (value < p_startPos)
            {
                value = p_startPos;
            }
            else if (value > endNext)
            {
                value = endNext;
            }
            p_nowPos = value;

            return p_nowPos - p_startPos;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(TruncateStream));
            if (buffer is null) throw new ArgumentNullException();

            var len = buffer.Length;

            if (count < 0 || offset < 0 || offset + count > len) throw new ArgumentOutOfRangeException();

            int rsize;
            int re = 0;
            while (count != 0)
            {
                rsize = f_readOnce(buffer, offset, count);
                if (rsize == 0) return re;
                offset += rsize;
                count -= rsize;
                re += rsize;
            }
            return re;
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(TruncateStream));

            //缓存可用量
            //int count = f_bufferHaveReadCount(p_bufPos, p_bufPosEnd);
            int count = p_bufPosEnd - p_bufPos + 1;

            if (count == 0)
            {
                //无缓存数据
                //清理缓存指针

                f_bufferClear();

                count = f_bufferReadbase(out _);
                //从基础流无法读取则没有数据
                if (count == 0) return -1;
            }

            byte b = p_buffer[p_bufPos++];
            //p_bufPos++;
            return b;
        }
       
        #region 无实现
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        #endregion

        #endregion

        #endregion

        #region 额外功能

        /// <summary>
        /// 获取内部封装的基础流
        /// </summary>
        /// <returns>内部封装的基础流，已释放返回null</returns>
        public Stream BaseStream
        {
            get => p_stream;
        }

        #endregion

    }

    /// <summary>
    /// 可将流数据截断为一部分的只读流，此类没有内置缓冲区
    /// </summary>
    /// <remarks>
    /// 封装指定的流对象，将其截断其中一部分数据作为只读流；封装的流必须要有读取和查找功能
    /// </remarks>
    public unsafe class NotBufferTruncateStream : HEStream
    {

        #region 释放

        /// <summary>
        /// 重写该函数可自定义释放代码，需要调用基实现
        /// </summary>
        /// <param name="disposing"></param>
        /// <returns></returns>
        protected override bool Disposing(bool disposing)
        {
            if (p_isFree && disposing)
            {
                p_stream.Close();
            }
            p_stream = null;
            return true;
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个只读截断流
        /// </summary>
        /// <param name="stream">要截断的流</param>
        /// <param name="startPosition">截断的流起始位置</param>
        /// <param name="length">要截断的字节量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的参数超出范围</exception>
        /// <exception cref="ArgumentException">给定的流不支持查找</exception>
        public NotBufferTruncateStream(Stream stream, long startPosition, long length) : this(stream, startPosition, length, true)
        {
        }

        /// <summary>
        /// 实例化一个只读截断流
        /// </summary>
        /// <param name="stream">要截断的流</param>
        /// <param name="startPosition">截断的流起始位置</param>
        /// <param name="length">要截断的字节量</param>
        /// <param name="freeBaseStream">在释放资源时是否释放封装的基础流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的参数超出范围</exception>
        /// <exception cref="ArgumentException">给定的流不支持查找</exception>
        public NotBufferTruncateStream(Stream stream, long startPosition, long length, bool freeBaseStream)
        {
            f_initThrow(stream, startPosition, length);
            initStream(stream, startPosition, length, freeBaseStream);
        }

        private void f_initThrow(Stream stream, long startPosition, long length)
        {
            if (stream is null) throw new ArgumentNullException();

            if ((!stream.CanSeek) || (!stream.CanRead)) throw new NotSupportedException();

            if (startPosition < 0 || length <= 0) throw new ArgumentOutOfRangeException();

            if (stream.Length < startPosition + length) throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 使用给定参数初始化内部字段和数据
        /// </summary>
        /// <param name="stream">封装的流</param>
        /// <param name="startPosition">截断的起始位置，不要超过封装流的范围</param>
        /// <param name="length">截断的长度，不要超过封装流的范围</param>
        /// <param name="freebaseStream">释放当前实例时是否释放封装的流</param>
        /// <param name="bufferSize">当前流的缓冲区容量，要大于0</param>
        protected void initStream(Stream stream, long startPosition, long length, bool freebaseStream)
        {
            p_stream = stream;
            p_startPos = startPosition;
            p_endPos = startPosition + length - 1;
            p_isFree = freebaseStream;
            p_nowPos = p_startPos;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 截断流的起始位置
        /// </summary>
        private long p_startPos;

        /// <summary>
        /// 截断流的终止位置
        /// </summary>
        private long p_endPos;

        /// <summary>
        /// 记录当前位置所在的基础流的原始指针
        /// </summary>
        private long p_nowPos;

        /// <summary>
        /// 内部封装的流
        /// </summary>
        protected Stream p_stream;

        private bool p_isFree;

        #endregion

        #region 派生

        #region 封装

        #endregion

        #region 参数

        public override long Position
        {
            get
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                return (p_nowPos) - p_startPos;
            }
            set
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                return (p_endPos + 1) - p_startPos;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                return p_stream.WriteTimeout;
            }
            set
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                p_stream.WriteTimeout = value;
            }
        }

        public override bool CanTimeout => (p_stream?.CanTimeout).GetValueOrDefault();

        public override int ReadTimeout
        {
            get
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                return p_stream.ReadTimeout;
            }
            set
            {
                ThrowIsDispose(nameof(NotBufferTruncateStream));
                p_stream.ReadTimeout = value;
            }
        }

        #endregion

        #region 功能

        /// <summary>
        /// 在该类型中表示为无效操作
        /// </summary>
        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));

            long value;

            //f_bufferClear();

            if (origin == SeekOrigin.Begin)
            {
                value = p_startPos + offset;
            }
            else if (origin == SeekOrigin.End)
            {

                value = (p_endPos + 1) + offset;

            }
            else if (origin == SeekOrigin.Current)
            {
                value = p_nowPos + offset;
            }
            else
            {
                throw new ArgumentException();
            }

            if (value < p_startPos)
            {
                value = p_startPos;
            }
            else if (value > p_endPos)
            {
                value = p_endPos + 1;
            }
            p_nowPos = value;

            return p_nowPos - p_startPos;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));
            if (buffer is null) throw new ArgumentNullException();

            var len = buffer.Length;

            if (count < 0 || offset < 0 || offset + count > len) throw new ArgumentOutOfRangeException();

            if (p_nowPos > p_endPos)
            {
                return 0;
            }
            if(p_nowPos < p_startPos)
            {
                p_nowPos = p_startPos;
                //return 0;
            }

            //截断流剩余长度
            var length = (p_endPos + 1) - p_nowPos;

            if(length <= 0)
            {
                return 0;
            }

            if(length > ((p_endPos - p_startPos) + 1))
            {
                //超过范围
                //throwOutRange();
                return 0;
            }

            int reCount;
            if (length <= int.MaxValue) reCount = (int)Math.Min(length, count);
            else reCount = count;

            //判断并设置基位置
            lock (p_stream)
            {
                if (p_nowPos != p_stream.Position)
                {
                    p_stream.Position = p_nowPos;
                }
                reCount = p_stream.Read(buffer, offset, reCount);
            }            

            p_nowPos += reCount;

            return reCount;
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));

            //截断流剩余长度
            var length = (p_endPos + 1) - p_nowPos;

            if (length > 0)
            {
                lock (p_stream)
                {
                    //判断并设置基位置
                    if (p_nowPos != p_stream.Position)
                    {
                        p_stream.Position = p_nowPos;
                    }
                    return p_stream.ReadByte();
                }

            }

            return -1;
        }

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose(nameof(NotBufferTruncateStream));
            if (buffer == null) throw new ArgumentNullException();

            if(p_stream is HEStream hes)
            {
                return hes.ReadToAddress(buffer, count);
            }

            return base.ReadToAddress(buffer, count);
        }

        #region 无实现

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #endregion

        #region 额外功能

        /// <summary>
        /// 获取内部封装的基础流
        /// </summary>
        /// <returns>内部封装的基础流，已释放返回null</returns>
        public Stream BaseStream
        {
            get => p_stream;
        }

        #endregion

    }

}
