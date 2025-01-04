using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Cheng.Memorys;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cheng.Texts
{

    /// <summary>
    /// 封装文本读取器以进行标准化文本转化
    /// </summary>
    /// <remarks>
    /// <para>将文本形式的字节序列读取并转化为字符串，封装的读取器内的文本是以 XXFXXFXXFXX 的形式存在的，XX表示字节序列的每个字节的16进制数，F表示每两个字节之间的分隔符，该读取器将其读取并转化为二进制数据，再以此数据转化为指定编码格式的字符串</para>
    /// <para>结合<see cref="StandardizedTextWriter"/>使用以进行自动文本转化读写</para>
    /// </remarks>
    public sealed unsafe class StandardizedTextReader : SafeReleaseTextReader
    {

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            Reset();

            if (p_isDisposeReader && disposeing)
            {
                p_reader?.Close();
            }
            p_reader = null;

            return false;
        }
        #endregion

        #region 构造

        /// <summary>
        /// 初始化标准文本转化读取器
        /// </summary>
        /// <param name="reader">封装的读取器</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public StandardizedTextReader(TextReader reader) : this(reader, null, ' ', true, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 初始化标准文本转化读取器
        /// </summary>
        /// <param name="reader">封装的读取器</param>
        /// <param name="mapEncoding">转化标准文本的二进制序列需要的字符编码；输入null则默认为UTF-8编码格式</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public StandardizedTextReader(TextReader reader, Encoding mapEncoding) : this(reader, mapEncoding, ' ', true, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 初始化标准文本转化读取器
        /// </summary>
        /// <param name="reader">封装的读取器</param>
        /// <param name="mapEncoding">转化标准文本的二进制序列需要的字符编码；输入null则默认为UTF-8编码格式</param>
        /// <param name="separator">每两个字节之间的分隔符</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public StandardizedTextReader(TextReader reader, Encoding mapEncoding, char separator) : this(reader, mapEncoding, separator, true, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 初始化标准文本转化读取器
        /// </summary>
        /// <param name="reader">封装的读取器</param>
        /// <param name="mapEncoding">转化标准文本的二进制序列需要的字符编码；输入null则默认为UTF-8编码格式</param>
        /// <param name="separator">每两个字节之间的分隔符</param>
        /// <param name="disposeReader">是否在释放时连同封装的读取器一起释放；默认为true</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public StandardizedTextReader(TextReader reader, Encoding mapEncoding, char separator, bool disposeReader) : this(reader, mapEncoding, separator, disposeReader, cp_defBufferSize)
        {
        }

        /// <summary>
        /// 初始化标准文本转化读取器
        /// </summary>
        /// <param name="reader">封装的读取器</param>
        /// <param name="mapEncoding">转化标准文本的二进制序列需要的字符编码；输入null则默认为UTF-8编码格式</param>
        /// <param name="separator">每两个字节之间的分隔符</param>
        /// <param name="disposeReader">是否在释放时连同封装的读取器一起释放；默认为true</param>
        /// <param name="CharBufferSize">指定字符缓冲区长度；默认为2048</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度小于或等于0</exception>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public StandardizedTextReader(TextReader reader, Encoding mapEncoding, char separator, bool disposeReader, int CharBufferSize)
        {
            if (reader is null) throw new ArgumentNullException();
            if (CharBufferSize <= 0 || CharBufferSize >= int.MaxValue - 1) throw new ArgumentOutOfRangeException();

            if (mapEncoding is null) mapEncoding = Encoding.UTF8;
            f_init(reader, mapEncoding, separator, disposeReader, CharBufferSize);
        }

        private void f_init(TextReader reader, Encoding mapEncoding, char separator, bool disposeReader, int bufferSize)
        {
            p_reader = reader;
            p_encoding = mapEncoding;
            p_spe = separator;
            p_isDisposeReader = disposeReader;

            p_decoder = mapEncoding.GetDecoder();
            //p_encoder = mapEncoding.GetEncoder();

            p_baseBuffer = new char[512 * 3 + 2];
            p_byteBuffer = new byte[4096];

            p_OutBuffer = new char[bufferSize];

            p_readBaseBufPos = 0;
            p_readBaseBufEndPos = -1;
            p_byteBufPos = 0;
            p_byteBufPosEnd = -1;
            p_OutBufferPos = 0;
            p_OutBufferEndPos = -1;

            p_run_OnceCharMaxByte = p_encoding.GetMaxByteCount(1);
        }


        const int cp_defBufferSize = 2048;

        #endregion

        #region 参数

        private TextReader p_reader;

        private Encoding p_encoding;

        /// <summary>
        /// 字节编码转化到字符串工具
        /// </summary>
        private Decoder p_decoder;

        //private Encoder p_encoder;

        #region 缓冲区

        /// <summary>
        /// 字节缓冲区
        /// </summary>
        private byte[] p_byteBuffer;
        /// <summary>
        /// 字节缓冲区已读数据
        /// </summary>
        private int p_byteBufPos;
        /// <summary>
        /// 字节缓冲区可读数据尾端索引
        /// </summary>
        private int p_byteBufPosEnd;

        /// <summary>
        /// 读取基本封装流到此处的缓冲区
        /// </summary>
        private char[] p_baseBuffer;
        /// <summary>
        /// 基本原始字符串数据缓冲区指针，当前需要提取的索引位置
        /// </summary>
        private int p_readBaseBufPos;
        /// <summary>
        /// 基本原始字符串数据缓冲区，已装填的数据结尾索引
        /// </summary>
        private int p_readBaseBufEndPos;


        /// <summary>
        /// 读取到的字符缓冲区
        /// </summary>
        private char[] p_OutBuffer;

        /// <summary>
        /// 读取缓冲区的指针
        /// </summary>
        private int p_OutBufferPos;

        /// <summary>
        /// 读取缓冲区装载量的结尾索引
        /// </summary>
        private int p_OutBufferEndPos;
        #endregion

        #region 运算时常数

        private int p_run_OnceCharMaxByte;

        #endregion

        /// <summary>
        /// 分隔符
        /// </summary>
        private char p_spe;

        /// <summary>
        /// 是否释放基本对象
        /// </summary>
        private bool p_isDisposeReader;

        /// <summary>
        /// 实例是否释放
        /// </summary>
        //private bool p_isDisposed;
        #endregion

        #region 功能

        #region 参数获取

        /// <summary>
        /// 从文本字节序列转换到字符串的字符编码
        /// </summary>
        public Encoding MapEncoding
        {
            get => p_encoding;
        }

        /// <summary>
        /// 字节序列中每字节的中间分隔符
        /// </summary>
        public char Separator => p_spe;

        /// <summary>
        /// 获取封装的基础读取器
        /// </summary>
        /// <value>
        /// 使用Set属性可重新设置新的基础读取器，在设置新的读取器后，请调用<see cref="Reset"/>以重置内部缓冲区；除了一些需要极致优化内存使用的情况外，不建议使用；除非你知道自己在做什么
        /// </value>
        /// <exception cref="ArgumentNullException">set设置为null</exception>
        /// <exception cref="ObjectDisposedException">set设置前当前实例已释放</exception>
        public TextReader BaseReader
        {
            get => p_reader;
            set
            {
                ThrowObjectDisposed();
                if (value is null) throw new ArgumentNullException();
                p_reader = value;
            }
        }

        #endregion

        #region 封装

        #region 判断


        /// <summary>
        /// 判断字符是否属于16进制数
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool f_isX16Char(char c)
        {
            return ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
        }

        #endregion

        #region 缓冲区


        /// <summary>
        /// 左移缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="beginIndex">起始指针</param>
        /// <param name="endIndex">终止指针</param>
        static void f_leftBuffer(byte[] buffer, ref int beginIndex, ref int endIndex)
        {

            if (beginIndex == 0)
            {
                return;
            }

            fixed (byte* buf = buffer)
            {
                int toi;
                for (toi = 0; beginIndex <= endIndex; beginIndex++)
                {
                    buf[toi] = buf[beginIndex];
                }
                endIndex = endIndex - beginIndex;
                beginIndex = 0;
            }

        }

        /// <summary>
        /// 左移缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="beginIndex">起始指针</param>
        /// <param name="endIndex">终止指针</param>
        /// <param name="toEnd">移动后的终止指针</param>
        static void f_leftBuffer(char[] buffer, ref int beginIndex, ref int endIndex)
        {
            if (beginIndex == 0)
            {
                return;
            }

            fixed (char* buf = buffer)
            {
                int toi;
                for (toi = 0; beginIndex <= endIndex; beginIndex++)
                {
                    buf[toi] = buf[beginIndex];
                }
                endIndex = endIndex - beginIndex;
                beginIndex = 0;
            }

        }


        /// <summary>
        /// 按索引区间计算剩余可用缓存量
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        static int f_bufferHaveReadCount(int beginIndex, int endIndex)
        {
            return endIndex - beginIndex + 1;
        }

        /// <summary>
        /// 查看缓冲区未使用的容量
        /// </summary>
        /// <param name="length">本身长度</param>
        /// <param name="index">末端指针</param>
        /// <returns>从index到缓冲区长度尾端的空间</returns>
        static int f_bufferIsFillCount(int length, int index)
        {
            return length - (index + 1);
        }
        
        /// <summary>
        /// 按双索引查看缓冲区已提取的量
        /// </summary>
        /// <param name="beginIndex">起始索引</param>
        /// <returns>起始索引之前的量</returns>
        static int f_bufferIsGetting(int beginIndex)
        {
            //if (endIndex < 0 || beginIndex == 0) return 0;
            return beginIndex;
        }

        /// <summary>
        /// 将缓冲区指针的指向设为清空数据
        /// </summary>
        /// <param name="beginIndex">起始指针</param>
        /// <param name="endIndex">末端指针</param>
        static void f_bufferClear(ref int beginIndex, ref int endIndex)
        {
            beginIndex = 0;
            endIndex = -1;
        }

        /// <summary>
        /// 将指定缓存提取并写入另外缓存
        /// </summary>
        /// <param name="oriBuffer">提取的缓存</param>
        /// <param name="beginIndex">起始索引引用</param>
        /// <param name="endIndex">末尾索引引用</param>
        /// <param name="toBuf">待写入缓存</param>
        /// <param name="index">写入的起始位</param>
        /// <param name="count">写入的量</param>
        /// <returns>此次实际写入的量</returns>
        static int f_bufferWriter(char[] oriBuffer, ref int beginIndex, ref int endIndex, char[] toBuf, int index, int count)
        {
            //剩余缓存量
            int wcount = f_bufferHaveReadCount(beginIndex, endIndex);
            //写入量
            int wrc = System.Math.Min(wcount, count);

            Array.Copy(oriBuffer, beginIndex, toBuf, index, wrc);
            beginIndex += wrc;
            return wrc;
        }

        /// <summary>
        /// 将指定缓存提取并写入另外缓存
        /// </summary>
        /// <param name="oriBuffer">提取的缓存</param>
        /// <param name="beginIndex">起始索引引用</param>
        /// <param name="endIndex">末尾索引引用</param>
        /// <param name="toBuf">待写入缓存</param>
        /// <param name="index">写入的起始位</param>
        /// <param name="count">写入的量</param>
        /// <returns>此次实际写入的量</returns>
        static int f_bufferWriter(byte[] oriBuffer, ref int beginIndex, ref int endIndex, byte[] toBuf, int index, int count)
        {
            //剩余缓存量
            int wcount = f_bufferHaveReadCount(beginIndex, endIndex);
            //写入量
            int wrc = System.Math.Min(wcount, count);

            Array.Copy(oriBuffer, beginIndex, toBuf, index, wrc);
            beginIndex += wrc;
            return wrc;
        }

        /// <summary>
        /// 按双索引格式，尝试从基本读取器中提取文本到缓存直至存满或无法读取
        /// </summary>
        /// <param name="buffer">要读取到的缓冲区</param>
        /// <param name="endIndex">末端索引引用</param>
        /// <returns>读取到的量，0则表示到达结尾</returns>
        private int f_reader(char[] buffer, ref int endIndex)
        {
            int re = p_reader.Read(buffer, endIndex + 1, f_bufferIsFillCount(buffer.Length, endIndex));
            endIndex += re;
            return re;
        }

        #endregion

        #region 原始字符缓存

        /// <summary>
        /// 原始字符数据剩余可提取的缓存数据
        /// </summary>
        /// <returns></returns>
        private int f_baseBufLastCount()
        {
            return p_readBaseBufEndPos - p_readBaseBufPos + 1;
        }

        /// <summary>
        /// 原始字符数据缓冲区当前可向后装填的字符数
        /// </summary>
        /// <returns></returns>
        private int f_baseBufIsFillCount()
        {
            return p_baseBuffer.Length - (p_readBaseBufEndPos + 1);
        }

        /// <summary>
        /// 将原始字符缓冲区重置为空
        /// </summary>
        private void f_baseBufClear()
        {
            p_readBaseBufPos = 0;
            p_readBaseBufEndPos = -1;
        }

        /// <summary>
        /// 将原始数据缓冲区内的已有数据，冲刷到最左端
        /// </summary>
        /// <returns>已到达最左端返回false，成功冲刷返回true</returns>
        private bool f_baseBufferFlushToLeft()
        {
            //从左向右往左挪
            if (p_readBaseBufPos == 0) return false;

            //int length = buf.Length;

            f_leftBuffer(p_baseBuffer, ref p_readBaseBufEndPos, ref p_readBaseBufEndPos);
            //p_readBaseBufEndPos = p_readBaseBufEndPos - p_readBaseBufPos;
            //p_readBaseBufPos = 0;

            return true;
        }

        /// <summary>
        /// 按剩余容量装填待读取的原始字符数据
        /// </summary>
        /// <param name="fillEnd">原始缓冲区可读取数据的指针，是否在读取前已指向末尾</param>
        /// <param name="notRead">基础读取器是否已到达结尾无法读取</param>
        /// <returns>向后装填的字符数</returns>
        private int f_baseBufferFill(out bool fillEnd, out bool notRead)
        {

            var count = f_bufferIsFillCount(p_baseBuffer.Length, p_readBaseBufEndPos);

            if (count == 0)
            {
                fillEnd = true;
                notRead = false;
                return 0;
            }

            count = p_reader.Read(p_baseBuffer, p_readBaseBufEndPos + 1, count);

            if(count == 0)
            {
                notRead = true;
            }
            else notRead = false;
            fillEnd = false;

            return count;
        }

        /// <summary>
        /// 尝试读取新的基础数据到原始字符缓冲区并冲刷到左端
        /// </summary>
        /// <param name="isReadBase">该操作是否读取了基础封装实例</param>
        /// <param name="baseReaderEnd">读取基础读取器时是否到达结尾</param>
        /// <returns>尝试读取到的新的缓存字符数量</returns>
        private int f_baseBufferFlushAndRead(out bool isReadBase, out bool baseReaderEnd)
        {
            //成功左移
            bool isLeftFlush = f_baseBufferFlushToLeft();

            if (isLeftFlush)
            {
                //定有空缺
                isReadBase = true;
                return f_baseBufferFill(out _, out baseReaderEnd);
            }

            int fc = f_baseBufIsFillCount();

            if (fc == 0)
            {
                //缓冲区已满无法读取
                isReadBase = false;
                baseReaderEnd = false;
                return 0;
            }

            //允许读取
            isReadBase = true;
            return f_baseBufferFill(out _, out baseReaderEnd);
        }

        /// <summary>
        /// 判断可用——可提取的的原始缓存数据
        /// </summary>
        /// <returns></returns>
        private bool f_baseBufCanConver()
        {
            int count = f_baseBufLastCount();
            if (count < 2) return false;

            char c = p_baseBuffer[p_readBaseBufPos];
            char c2 = p_baseBuffer[p_readBaseBufPos + 1];

            if (((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
            {
                return ((c2 >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
            }

            if (!((c2 >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
            {
                return false;
            }

            if (count == 2)
            {
                return false;
            }

            c = p_baseBuffer[p_readBaseBufPos + 2];
            return (((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')));
        }

        #endregion

        #region 从原始字符到字节序列

        /// <summary>
        /// 将原始缓冲区提取转化并向后写入到字节缓冲区
        /// </summary>
        /// <param name="isIncomplete">读取原始数据时是否有一个不完整的字节构成在尾端</param>
        /// <param name="isReading">此次是否有成功读取到字节数据</param>
        /// <returns>转化过程是否顺利；顺利转化完毕返回true，出现无法解析的字符或分隔符不是指定内容返回false</returns>
        private bool f_BaseBufConverBytes(out bool isIncomplete, out bool isReading)
        {

            var baseBuf = p_baseBuffer;
            var byteBuf = p_byteBuffer;

            isIncomplete = false;

            int bufLength = byteBuf.Length;
            //是否转化顺利
            //bool toBytesIsTrue = true;

            isReading = false;

            fixed (char* baseBufPtr = baseBuf)
            {

                fixed (byte* bufPtr = byteBuf)
                {
                    byte tb;

                    #region 转化到bytes
                    //int bufEndIndex = bufLength - 1;

                    ref int basePos = ref p_readBaseBufPos;
                    ref int basePosEnd = ref p_readBaseBufEndPos;

                    ref int bytePos = ref p_byteBufPos;
                    ref int bytePosEnd = ref p_byteBufPosEnd;

                    bool b;
                    int re;

                    while (true)
                    {
                        
                        re = f_bufferHaveReadCount(basePos, basePosEnd);
                        if (re < 2)
                        {
                            isIncomplete = re != 0;
                            //基本缓冲区无可用
                            return true;
                        }
                        re = f_bufferIsFillCount(bufLength, bytePosEnd);
                        if (re == 0)
                        {
                            //字节缓冲区没有多余空间                            
                            return true;
                        }
                        //读取到字节

                        if (baseBuf[basePos] == p_spe)
                        {
                            //是分隔符
                            basePos++;
                        }

                        re = f_bufferHaveReadCount(basePos, basePosEnd);
                        if (re < 2)
                        {
                            isIncomplete = re != 0;
                            //基本缓冲区无可用
                            return true;
                        }

                        b = f_readByte(baseBufPtr + basePos, out tb);
                        if (!b)
                        {
                            //无法解析
                            return false;
                        }
                        bytePosEnd++;
                        byteBuf[bytePosEnd] = tb;
                        basePos += 2;
                        isReading = true;

                        re = f_bufferHaveReadCount(basePos, basePosEnd);
                        if (re < 1)
                        {
                            //无分隔符
                            return true;
                        }

                        if (baseBuf[basePos] != p_spe)
                        {
                            //不是分隔符
                            return false;
                        }

                        basePos++;
                    }

                    #endregion

                    //修改指针
                    //转化完成

                }

            }

        }

        /// <summary>
        /// 在字符串中读取并转化一个字节
        /// </summary>
        /// <param name="cs">指向两个字符的左端字符地址</param>
        /// <param name="re">转化到的字节</param>
        /// <returns>是否成功</returns>
        private static bool f_readByte(char* cs, out byte re)
        {
            char c;

            c = cs[1];
            re = 0;

            if (c >= '0' && c <= '9')
            {
                re |= (byte)((c - '0'));
            }
            else if (c >= 'A' && c <= 'F')
            {
                re |= (byte)((c - 'A') + 10);
            }
            else if (c >= 'a' && c <= 'f')
            {
                re |= (byte)((c - 'a') + 10);
            }
            else
            {
                return false;
            }

            c = cs[0];

            if (c >= '0' && c <= '9')
            {
                re |= (byte)((c - '0') << 4);
            }
            else if (c >= 'A' && c <= 'F')
            {
                re |= (byte)(((c - 'A') + 10) << 4);
            }
            else if (c >= 'a' && c <= 'f')
            {
                re |= (byte)(((c - 'a') + 10) << 4);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断内部读取器要读取的字符数，与标准读取数量偏移
        /// </summary>
        /// <param name="count">原始字符数</param>
        /// <returns>
        /// 原始字符数可供转化后的字节数量
        /// </returns>
        private int f_isByteCharsCount(int count)
        {
            int reMod = count % 3;

            if (reMod != 2) return reMod + 1;
            return 0;
        }

        #endregion

        #region 解析后字符缓存

        /// <summary>
        /// 清空输出缓存的可用数据可已读
        /// </summary>
        private void f_outBufClear()
        {
            p_OutBufferPos = 0;
            p_OutBufferEndPos = -1;
        }

        /// <summary>
        /// 将字节缓冲区的可用字节，按字节编码转化并写入到输出字符缓冲区
        /// </summary>
        /// <param name="converByteCount">成功从字节序列转化到字符串的字节数量</param>
        /// <param name="ToCharCount">成功从字节序列转化到字符的字符数量</param>
        /// <returns>此次转化是否将字节缓冲区全部提取完毕，全部提取返回true，没有则返回false</returns>
        private bool f_outBufDecoderOfBytes(out int converByteCount, out int ToCharCount)
        {

            ref var outBuf = ref p_OutBuffer;
            ref var byteBuf = ref p_byteBuffer;

            int byteCount = f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd);
            
            if (byteCount == 0)
            {
                //剩余缓存为0
                converByteCount = 0;
                ToCharCount = 0;
                return false;
            }

            int charCount = f_bufferIsFillCount(outBuf.Length, p_OutBufferEndPos);

            //成功转换到字符的字节数
            int bytesUsed;
            //成功转化到outBuf的字符数
            int charsUsed;

            bool comp;

            p_decoder.Convert(byteBuf,
                p_byteBufPos, byteCount,
                outBuf,
                p_OutBufferEndPos + 1, charCount,
                true, out bytesUsed, out charsUsed, out comp);

            //推进字节缓冲区
            p_byteBufPos += bytesUsed;
            //装载输出字符缓冲区
            p_OutBufferEndPos += charsUsed;

            converByteCount = bytesUsed;
            ToCharCount = charsUsed;

            return comp;
        }

        /// <summary>
        /// 将输出缓存左移——重置已读数据
        /// </summary>
        private void f_outBufLeftData()
        {
            //f_leftBuffer(p_OutBuffer, p_OutBufferPos, p_OutBufferEndPos, out p_OutBufferEndPos);
            //p_OutBufferPos = 0;

            f_leftBuffer(p_OutBuffer, ref p_OutBufferPos, ref p_OutBufferEndPos);
        }

        /// <summary>
        /// 提取输出缓存写入指定数组
        /// </summary>
        /// <param name="buffer">写入的缓冲区</param>
        /// <param name="index">写入的起始位</param>
        /// <param name="count">写入的字符数</param>
        /// <returns>实际写入的量</returns>
        private int f_outWriterBuf(char[] buffer, int index, int count)
        {

            return f_bufferWriter(p_OutBuffer,
                ref p_OutBufferPos, ref p_OutBufferEndPos,
                buffer, index, count);
        }

        /// <summary>
        /// 提取输出缓存写入指定地址
        /// </summary>
        /// <param name="buffer">写入的缓冲区</param>
        /// <param name="count">写入的字符数</param>
        /// <returns>实际写入的量</returns>
        private int f_outWriterBuf(char* buffer, int count)
        {
            //剩余缓存量
            int wcount = f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos);
            //写入量
            int wrc = System.Math.Min(wcount, count);

            if (wrc == 0) return 0;

            fixed (char* op = p_OutBuffer)
            {
                MemoryOperation.MemoryCopy(op + p_OutBufferPos, buffer, wrc * sizeof(char));
            }

            p_OutBufferPos += wrc;
            return wrc / 2;
        }

        #endregion

        /// <summary>
        /// 将字节序列可用数据转化到给定实例
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns>实际写入的字符数</returns>
        private int f_outByteToChars(char[] buffer, int index)
        {
            int re = p_encoding.GetChars(p_byteBuffer, p_byteBufPos, f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd), buffer, index);
            p_byteBufPos = p_byteBufPosEnd + 1;
            return re;
        }

        /// <summary>
        /// 返回当前可用字节序列能够转化的字符数
        /// </summary>
        /// <returns></returns>
        private int f_NowBytesToCharCount()
        {
            int re = f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd);
            return p_encoding.GetCharCount(p_byteBuffer, p_byteBufPos, re);

        }

        /// <summary>
        /// 将读取数据到输出缓存，直至未使用区域填满或无法读取
        /// </summary>
        /// <returns>全部到达末尾无法读取则返回false，否则返回true</returns>
        private bool f_outBufferReadToOn()
        {

            int cs;
            int byteToCharCount;
            
            //条件循环，输出缓冲区未使用字节不等于0则持续循环

            //bool inc;
            bool isReadToByte;

            while (true)
            {
                //输出缓存剩余待存量
                cs = f_bufferIsFillCount(p_OutBuffer.Length, p_OutBufferEndPos);
                if (cs == 0)
                {
                    return true;
                }

                //字节缓存剩余可用量
                cs = f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd);

                if (cs != 0)
                {
                    //字节缓存存在剩余

                    //直接输出

                    //左移数据
                    //f_leftBuffer(p_byteBuffer, p_byteBufPos, p_byteBufPosEnd, out p_byteBufPosEnd);
                    //p_byteBufPos = 0;

                    //f_leftBuffer(p_byteBuffer, ref p_byteBufPos, ref p_byteBufPosEnd);

                    f_leftBuffer(p_OutBuffer, ref p_OutBufferPos, ref p_OutBufferEndPos);
                    //var chs = p_encoding.GetChars(p_byteBuffer, p_byteBufPos, f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd));

                    //从字节到输出字符缓存
                    f_outBufDecoderOfBytes(out _, out byteToCharCount);
                    if(byteToCharCount != 0)
                    {
                        //成功转化到输出缓存，重新循环
                        continue;
                    }
                }


                //字节剩余量不存在可转化内容

                //判断原始缓存是否拥有可用数据

                if (f_baseBufCanConver())
                {
                    //存在原始数据可用量

                    //左移字节缓存
                    f_leftBuffer(p_byteBuffer, ref p_byteBufPos, ref p_byteBufPosEnd);

                    //写入到字节数据
                    var b = f_BaseBufConverBytes(out _, out isReadToByte);

                    if (!b)
                    {
                        throw new InvalidOperationException("无法解析字节序列文本");
                    }

                    if (isReadToByte)
                    {
                        //成功写入到字节缓存
                        continue;
                    }

                }

                //原始缓存无法写入到字节缓存

                //从读取器提取数据到原始缓存

                //左移原始数据

                f_leftBuffer(p_baseBuffer, 
                    ref p_readBaseBufPos, ref p_readBaseBufEndPos);

                //读取器读取到原始数据缓存
                cs = f_reader(p_baseBuffer, ref p_readBaseBufEndPos);

                if (cs == 0)
                {
                    //无法读取，判断递归

                    if (f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos) != 0)
                    {
                        return true;
                    }
                    //if (f_bufferHaveReadCount(p_byteBufPos, p_byteBufPoeEnd) != 0)
                    //{
                    //    return true;
                    //}
                    if (f_baseBufCanConver())
                    {
                        return true;
                    }

                    return false;
                }

            }

            //return true;
        }

        #endregion

        #region 派生

        public override int Read()
        {
            ThrowObjectDisposed();
            var i = f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos);
            char c;
            if (i > 0)
            {
                //剩余字符数
                c = p_OutBuffer[p_OutBufferPos];
                p_OutBufferPos++;
                return c;
            }
            //无用

            f_outBufLeftData();

            if (f_outBufferReadToOn())
            {
                c = p_OutBuffer[p_OutBufferPos];
                p_OutBufferPos++;
                return c;
            }

            return -1;
        }

        public override int Peek()
        {
            ThrowObjectDisposed();
            var c = f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos);
            if (c > 0)
            {
                return p_OutBuffer[p_OutBufferPos];
            }
            f_outBufLeftData();
            if (f_outBufferReadToOn())
            {
                return p_OutBuffer[p_OutBufferPos];
            }

            return -1;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            //int lastCount = f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos);

            //拥有剩余缓存

            //移个位先
            f_outBufLeftData();

            //补个泉先
            if (f_outBufferReadToOn())
            {
                return f_outWriterBuf(buffer, index, count);
            }
            return 0;
            //return f_outWriterBuf(buffer, index, count);
        }

        /// <summary>
        /// 从读取器中读取<paramref name="count"/>数量的字节并写入到<paramref name="bufferAddress"/>处
        /// </summary>
        /// <param name="bufferAddress">要读取到的首地址</param>
        /// <param name="count">要读取的最多字符数</param>
        /// <returns>实际读取的字符数；若等于0则表示没有任何可读取的内容</returns>
        public int Read(char* bufferAddress, int count)
        {

            ThrowObjectDisposed();
            if (bufferAddress == null) throw new ArgumentNullException();

            if (count < 0) throw new ArgumentOutOfRangeException();
            //int lastCount = f_bufferHaveReadCount(p_OutBufferPos, p_OutBufferEndPos);

            //拥有剩余缓存

            //移个位先
            f_outBufLeftData();

            //补个泉先
            f_outBufferReadToOn();
            
            return f_outWriterBuf(bufferAddress, count);
        }
        #endregion

        #region 功能

        /// <summary>
        /// 将内部的解码器以及缓冲区全部重置到初始状态
        /// </summary>
        public void Reset()
        {
            ThrowObjectDisposed();
            lock (this)
            {
                f_bufferClear(ref p_byteBufPos, ref p_byteBufPosEnd);
                f_bufferClear(ref p_OutBufferPos, ref p_OutBufferEndPos);
                f_bufferClear(ref p_readBaseBufPos, ref p_readBaseBufEndPos);
                p_decoder.Reset();
            }
        }

        #endregion

        #endregion

    }

}
