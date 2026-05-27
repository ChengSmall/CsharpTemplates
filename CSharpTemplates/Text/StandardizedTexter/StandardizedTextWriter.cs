using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Texts
{

    /// <summary>
    /// 封装文本写入器以进行标准化文本转化
    /// </summary>
    /// <remarks>
    /// <para>
    /// 在写入字符串后，将其转化为字符串的字符编码内存格式的二进制序列，再将其以字符串的方式写入到封装的文本写入器；
    /// </para>
    /// <para>
    /// 举个例子，假如写入了"abc"，则会根据字符编码转化成二进制序列 61 62 63 ，并再将其转化为文本"61 62 63"写入到内部封装的写入器；
    /// </para>
    /// <para>
    /// 结合<see cref="StandardizedTextReader"/>使用以进行自动文本转化读写
    /// </para>
    /// </remarks>
    public sealed unsafe class StandardizedTextWriter : SafeReleaseTextWriter
    {

        #region 构造

        /// <summary>
        /// 初始化转化标准文本写入器，使用UTF-8字符编码
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        public StandardizedTextWriter(TextWriter writer) : this(writer, null, ' ', true, true, cp_defBufferCharSize)
        {
        }

        /// <summary>
        /// 初始化转化标准文本写入器
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <param name="mapEncoding">转化到二进制序列时的字符编码格式，若为null则默认为UTF-8</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        public StandardizedTextWriter(TextWriter writer, Encoding mapEncoding) : this(writer, mapEncoding, ' ', true, true, cp_defBufferCharSize)
        {
        }

        /// <summary>
        /// 初始化转化标准文本写入器
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <param name="mapEncoding">转化到二进制序列时的字符编码格式，若为null则默认为UTF-8</param>
        /// <param name="separator">指定每个字节间的分隔符，默认为空格</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        public StandardizedTextWriter(TextWriter writer, Encoding mapEncoding, char separator) : this(writer, mapEncoding, separator, true, true, cp_defBufferCharSize)
        {
        }

        /// <summary>
        /// 初始化转化标准文本写入器
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <param name="mapEncoding">转化到二进制序列时的字符编码格式，若为null则默认为UTF-8</param>
        /// <param name="separator">指定每个字节间的分隔符，默认为空格</param>
        /// <param name="isDisposing">在释放时是否释放封装的基础写入器，默认为true</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        public StandardizedTextWriter(TextWriter writer, Encoding mapEncoding, char separator, bool isDisposing) : this(writer, mapEncoding, separator, isDisposing, true, cp_defBufferCharSize)
        {
        }

        /// <summary>
        /// 初始化转化标准文本写入器
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <param name="mapEncoding">转化到二进制序列时的字符编码格式，若为null则默认为UTF-8</param>
        /// <param name="separator">指定每个字节间的分隔符，默认为空格</param>
        /// <param name="isDisposing">在释放时是否释放封装的基础写入器，默认为true</param>
        /// <param name="isUpper">在转化二进制序列文本时字母为大写还是小写；true表示大写，false表示小写；默认为true</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度小于或等于0，或者循环写入字符数量大于 <![CDATA[((int.MaxValue + 1) / 2]]></exception>
        public StandardizedTextWriter(TextWriter writer, Encoding mapEncoding, char separator, bool isDisposing, bool isUpper) : this(writer, mapEncoding, separator, isDisposing, isUpper, cp_defBufferCharSize)
        {
        }

        /// <summary>
        /// 初始化转化标准文本写入器
        /// </summary>
        /// <param name="writer">要封装的写入器</param>
        /// <param name="mapEncoding">转化到二进制序列时的字符编码格式，若为null则默认为UTF-8</param>
        /// <param name="separator">指定每个字节间的分隔符，默认为空格</param>
        /// <param name="isDisposing">在释放时是否释放封装的基础写入器，默认为true</param>
        /// <param name="isUpper">在转化二进制序列文本时字母为大写还是小写；true表示大写，false表示小写；默认为true</param>
        /// <param name="charBufferSize">指定字符缓冲区长度，默认为1024</param>
        /// <exception cref="ArgumentNullException">封装的写入器为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度小于或等于0</exception>
        public StandardizedTextWriter(TextWriter writer, Encoding mapEncoding, char separator, bool isDisposing, bool isUpper, int charBufferSize)
        {
            if (writer is null) throw new ArgumentNullException();

            if (charBufferSize <= 0) throw new ArgumentOutOfRangeException();

            //if (writerLoopSize <= 0 || (long)writerLoopSize >= ((long)int.MaxValue + 1) / 2) throw new ArgumentException();

            if (mapEncoding is null) mapEncoding = Encoding.UTF8;

            f_init(writer, mapEncoding, separator, isDisposing, charBufferSize, isUpper);
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="writer">封装</param>
        /// <param name="mapEncoding">字节编码转化器</param>
        /// <param name="separator">分隔符</param>
        /// <param name="isDisposing">是否释放封装器</param>
        /// <param name="charBufferSize">字节缓冲区大小</param>
        private void f_init(TextWriter writer, Encoding mapEncoding, char separator, bool isDisposing, int charBufferSize, bool upper)
        {
            p_writer = writer;
            p_encoding = mapEncoding;
           
            p_spe = separator;
            p_isDisposeWriter = isDisposing;
            
            p_upper = upper;

            //p_isDisposed = false;

            //cp_ToBufferByteMaxSize = p_encoding.GetMaxByteCount(OnceTransMaxCharCount);
            //p_decoder = mapEncoding.GetDecoder();
            p_encoder = mapEncoding.GetEncoder();

            int byteBufferSize = charBufferSize * 2;

            byteBufferSize = System.Math.Min(byteBufferSize, mapEncoding.GetMaxByteCount(charBufferSize));

            p_writerBuffer = new char[charBufferSize];
            p_byteBuffer = new byte[byteBufferSize];

            //int mbs = byteBufferSize % 3;

            p_inCharBuffer = new char[byteBufferSize * 3];

            p_inCharBufPos = 0;
            p_inCharBufPosEnd = -1;
            p_byteBufPos = 0;
            p_byteBufPosEnd = -1;
            p_writerBufPos = 0;
            p_writerBufPosEnd = -1;
        }

        #endregion

        #region 常量

        const int cp_defBufferCharSize = 1024;

        #endregion

        #region 参数

        private TextWriter p_writer;

        /// <summary>
        /// 字符串转化映射的二进制序列字符编码
        /// </summary>
        private Encoding p_encoding;

        //private Decoder p_decoder;
        private Encoder p_encoder;

        #region 缓存

        /// <summary>
        /// 转化前字符缓冲区
        /// </summary>
        private char[] p_writerBuffer;

        /// <summary>
        /// 字节序列缓冲区
        /// </summary>
        private byte[] p_byteBuffer;

        /// <summary>
        /// 转化后字符缓存
        /// </summary>
        private char[] p_inCharBuffer;

        /// <summary>
        /// 转化后字符缓存首指针
        /// </summary>
        private int p_inCharBufPos;
        /// <summary>
        /// 转化后字符缓存尾指针
        /// </summary>
        private int p_inCharBufPosEnd;

        /// <summary>
        /// 字节缓冲区首指针
        /// </summary>
        private int p_byteBufPos;
        /// <summary>
        /// 字节缓冲区尾指针
        /// </summary>
        private int p_byteBufPosEnd;

        /// <summary>
        /// 字符缓冲区首指针
        /// </summary>
        private int p_writerBufPos;
        /// <summary>
        /// 字符缓冲区尾指针
        /// </summary>
        private int p_writerBufPosEnd;

        #endregion
        
        #region 条件参数
        /// <summary>
        /// 分隔符
        /// </summary>
        private char p_spe;

        /// <summary>
        /// 是否大写字母
        /// </summary>
        private bool p_upper;

        /// <summary>
        /// 是否释放基本对象
        /// </summary>
        private bool p_isDisposeWriter;

        #endregion

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 封装的字符序列写入器
        /// </summary>
        public TextWriter MapTextWriter
        {
            get => p_writer;
        }

        /// <summary>
        /// 写入后转化为二进制序列的字符编码格式
        /// </summary>
        public override Encoding Encoding
        {
            get => p_encoding;
        }

        /// <summary>
        /// 在转化为二进制编码序列到字符串时，16进制数的字母是否为大写
        /// </summary>
        public bool IsUpper
        {
            get => p_upper;
        }

        /// <summary>
        /// 在转化为二进制编码序列到字符串时，两个字节之间的分隔符
        /// </summary>
        public char Separator
        {
            get => p_spe;
        }

        #endregion

        #region 封装

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
        static int f_bufferIsNextCount(int length, int index)
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
        static int f_bufferWriter(Array oriBuffer, ref int beginIndex, ref int endIndex, Array toBuf, int index, int count)
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
        /// 将指定缓存提取并写入另外缓存未使用量
        /// </summary>
        /// <param name="oriBuffer">提取的缓存</param>
        /// <param name="beginIndex">起始索引引用</param>
        /// <param name="endIndex">末尾索引引用</param>
        /// <param name="toBuf">待写入缓存</param>
        /// <param name="toBeginIndex">写入的起始索引</param>
        /// <param name="toEndIndex">写入的末端索引</param>
        /// <returns>写入到的元素量</returns>
        static int f_bufferWriterPos(Array oriBuffer, ref int beginIndex, ref int endIndex, Array toBuf, ref int toBeginIndex, ref int toEndIndex)
        {
            int wcount = f_bufferHaveReadCount(beginIndex, endIndex);
           
            int toBufLength = toBuf.Length;

            int toCount = f_bufferIsNextCount(toBufLength, toEndIndex);

            Array.Copy(oriBuffer, beginIndex, toBuf, toEndIndex + 1, toCount);

            toEndIndex = toBufLength - 1;
            beginIndex = endIndex + 1;

            return toCount;
        }

        #endregion

        #region 转化

        /// <summary>
        /// 将字节值转化为2个字符
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="twoSizePtr">转化到的位置</param>
        /// <param name="upper">字母大写</param>
        private static void f_byteToChars(byte value, char* twoSizePtr, bool upper)
        {
            const byte x4 = 0xF;

            byte re = (byte)(value & x4);

            char firstX = upper ? 'A' : 'a';

            if (re < 10)
            {
                twoSizePtr[1] = (char)('0' + re);
            }
            else
            {
                twoSizePtr[1] = (char)(firstX + (re - 10));
            }

            re = (byte)(value >> 4);
            if (re < 10)
            {
                twoSizePtr[0] = (char)('0' + re);
            }
            else
            {
                twoSizePtr[0] = (char)(firstX + (re - 10));
            }
        }

        #endregion

        #region flush

        /// <summary>
        /// 从字节缓存到输出缓存
        /// </summary>
        /// <remarks>将字节序列缓存中可用数据全部转化并写入到待输出缓存，直至转化完毕或输出缓存已满</remarks>
        /// <returns>转化成字符的字节序列数量</returns>
        private int f_flush_byteBufWriter()
        {

            byte[] bytebuf = p_byteBuffer;

            char[] inBuf = p_inCharBuffer;

            int toByteCount;

            fixed (byte* byteBufPtr = p_byteBuffer)
            {
                fixed (char* inBufPtr = p_inCharBuffer)
                {
                    //从字节序列挨个转化

                    //字节序列索引
                    int byteI;
                    //待输出缓存索引
                    int inbufI;

                    //int inLength = inBuf.Length;
                    int inEndIndex = inBuf.Length - 1;

                    //提取索引
                    byteI = p_byteBufPos;

                    //写入索引
                    inbufI = p_inCharBufPosEnd;
                    toByteCount = 0;
                    while (true)
                    {
                        //判断索引超标
                        if (inbufI == inEndIndex || byteI > p_byteBufPosEnd)
                        {
                            //没有剩余
                            p_inCharBufPosEnd = inbufI;
                            p_byteBufPos = byteI;
                            break;
                        }
                        inbufI++;

                        //转化byteI到inBufI
                        f_byteToChars(byteBufPtr[byteI], inBufPtr + inbufI, p_upper);
                        toByteCount++;

                        //推进索引
                        inbufI += 1;
                        byteI++;

                        //判断超标
                        if (inbufI == inEndIndex || byteI > p_byteBufPosEnd)
                        {
                            //超出输出缓存范围
                            p_inCharBufPosEnd = inbufI;
                            p_byteBufPos = byteI;
                            break;
                        }

                        //if (byteI > p_byteBufPosEnd)
                        //{
                        //    //没有可用字节
                        //    p_inCharBufPosEnd = inbufI;
                        //    p_byteBufPos = byteI;
                        //    break;
                        //}

                        //写入分隔符
                        inbufI++;

                        inBuf[inbufI] = p_spe;
                        

                    }

                    //转化完毕

                }

            }

            return toByteCount;

        }

        /// <summary>
        /// 从写入缓存到字节缓存
        /// </summary>
        /// <remarks>将写入的缓存解码并转化到字节序列</remarks>
        /// <param name="flush">转码参数</param>
        /// <param name="getCharCount">此次操作提取的写入缓存量</param>
        /// <param name="writerByteCount">此次操作后转换过去的字节量</param>
        /// <param name="allChars">此次的操作是否全部将剩余写入缓存转化</param>
        private void f_flush_WriterToBytes(bool flush, out int getCharCount, out int writerByteCount, out bool allChars)
        {
            //写入缓存

            ref int wrBufPos = ref p_writerBufPos;
            ref int wrBufPend = ref p_writerBufPosEnd;

            //ref int byfPos = ref p_byteBufPos;
            ref int byfEnd = ref p_byteBufPosEnd;

            int byteCount = f_bufferIsNextCount(p_byteBuffer.Length, byfEnd);
            int charCount = f_bufferHaveReadCount(wrBufPos, wrBufPend);

            if (byteCount == 0 || charCount == 0)
            {
                getCharCount = 0;
                writerByteCount = 0;
                allChars = false;
                return;
            }

            fixed (char* cptr = p_writerBuffer)
            {

                fixed (byte* bp = p_byteBuffer)
                {

                    p_encoder.Convert(cptr, charCount,
                        bp, byteCount, flush,
                        out charCount, out byteCount, out allChars);

                    wrBufPos += charCount;
                    byfEnd += byteCount;
                }

            }

            getCharCount = charCount;
            writerByteCount = byteCount;

        }

        /// <summary>
        /// 左移字节缓存
        /// </summary>
        private void f_flush_leftBytes()
        {
            f_leftBuffer(p_byteBuffer, ref p_byteBufPos, ref p_byteBufPosEnd);
        }

        /// <summary>
        /// 左移输出缓存
        /// </summary>
        private void f_flush_leftInChars()
        {
            f_leftBuffer(p_inCharBuffer, ref p_inCharBufPos, ref p_inCharBufPosEnd);
        }

        /// <summary>
        /// 左移写入缓存
        /// </summary>
        private void f_flush_leftWriterBuf()
        {
            f_leftBuffer(p_writerBuffer, ref p_writerBufPos, ref p_writerBufPosEnd);
        }

        /// <summary>
        /// 将输出缓存写入封装对象
        /// </summary>
        /// <returns>写入到基础对象的字符数</returns>
        private int f_flush_writerToObj()
        {

            char[] inBuf = p_inCharBuffer;

            ref int pos = ref p_inCharBufPos;
            ref int posEnd = ref p_inCharBufPosEnd;

            int toCount = f_bufferHaveReadCount(pos, posEnd);

            if (toCount == 0) return 0;

            p_writer.Write(inBuf, pos, toCount);

            pos = posEnd + 1;

            return toCount;
        }

        /// <summary>
        /// 从写入缓存到写入设备一条龙服务
        /// </summary>
        /// <param name="baseObj">是否flush基础对象</param>
        private void f_flushAll(bool baseObj)
        {
            f_flush_leftBytes();

            f_flush_WriterToBytes(true, out _, out _, out _);

            f_flush_leftInChars();

            f_flush_byteBufWriter();

            f_flush_writerToObj();

            f_flush_leftWriterBuf();
            if(baseObj) p_writer.Flush();
        }

        #endregion

        #region 写入

        /// <summary>
        /// 计算写入缓存未使用的量
        /// </summary>
        /// <returns></returns>
        private int f_writerBufNextCount()
        {
            return f_bufferIsNextCount(p_writerBuffer.Length, p_writerBufPosEnd);
        }
        
        /// <summary>
        /// 从指定内存到写入缓存
        /// </summary>
        /// <param name="cs">从此处开始读取</param>
        /// <param name="count">写入的理想字符量</param>
        /// <returns>实际写入到的量</returns>
        private int f_objWriteToBuf(char* cs, int count)
        {
            //从外部缓冲区写入到写入缓存

            char[] wrBuf = p_writerBuffer;

            ref int pos = ref p_writerBufPos;
            ref int posEnd = ref p_writerBufPosEnd;
            int lastCount = f_writerBufNextCount();

            lastCount = (lastCount < count) ? lastCount : count;

            if (lastCount == 0)
            {
                return 0;
            }

            fixed (char* csp = wrBuf)
            {
                Memorys.MemoryOperation.MemoryCopy(cs, ((byte*)csp) + (posEnd + 1), lastCount * sizeof(char));
            }

            posEnd += lastCount;
            return lastCount;
        }

        /// <summary>
        /// 从指定内存到写入缓存
        /// </summary>
        /// <param name="cs">从此读取</param>
        /// <param name="index">读取索引</param>
        /// <param name="count">写入数量</param>
        /// <returns>实际写入到的量</returns>
        private int f_objWriteToBuf(char[] cs, int index, int count)
        {

            char[] wrBuf = p_writerBuffer;

            ref int pos = ref p_writerBufPos;
            ref int posEnd = ref p_writerBufPosEnd;
            int lastCount = f_writerBufNextCount();

            lastCount = (lastCount < count) ? lastCount : count;

            if (lastCount == 0)
            {
                return 0;
            }

            Array.Copy(cs, index, wrBuf, posEnd + 1, lastCount);

            posEnd += lastCount;
            return lastCount;
        }

        /// <summary>
        /// 将数据全部写完
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        private void f_writerEnd(char[] cs, int index, int count)
        {

            while (count != 0)
            {
                f_flush_leftWriterBuf();

                int wrc = f_objWriteToBuf(cs, index, count);

                count -= wrc;
                index += wrc;

                f_flushAll(false);
            }

        }

        /// <summary>
        /// 将数据全部写完
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="count"></param>
        private void f_writerEnd(char* cs, int count)
        {
            while (count != 0)
            {
                f_flush_leftWriterBuf();

                int wrc = f_objWriteToBuf(cs, count);

                count -= wrc;
                cs += wrc;

                f_flushAll(false);
            }
        }

        #endregion

        #region 判断

        /// <summary>
        /// 检查内部是否拥有缓存数据
        /// </summary>
        /// <returns></returns>
        private bool f_ifHaveBuffer()
        {
            int re = f_bufferHaveReadCount(p_writerBufPos, p_writerBufPosEnd);
            if (re != 0) return true;
            re = f_bufferHaveReadCount(p_byteBufPos, p_byteBufPosEnd);
            if (re != 0) return true;
            re = f_bufferHaveReadCount(p_inCharBufPos, p_inCharBufPosEnd);
            return re != 0;
        }

        #endregion

        #region 旧

        //private void f_write(char* charPtr, int count, int maxByteCount)
        //{
        //    var enc = p_encoding;

        //    //int maxByteCount = enc.GetMaxByteCount(count);

        //    //字节缓冲区长度
        //    var bufSize = p_byteBuffer.Length;

        //    if (p_byteBuffer.Length < maxByteCount)
        //    {
        //        p_byteBuffer = new byte[maxByteCount];
        //        bufSize = maxByteCount;
        //    }

        //    //字节缓冲区
        //    var buffer = p_byteBuffer;

        //    //实际写入字节数
        //    int wrSize;

        //    //获取字节缓冲区指针
        //    fixed (byte* bufPtr = buffer)
        //    {

        //        //写
        //        wrSize = enc.GetBytes(charPtr, count, bufPtr, bufSize);


        //        #region 将字节序列转化为字符串

        //        //bufPtr => [0,wrSize)

        //        //分隔符数量
        //        //int speCount = wrSize - 1;
        //        //字节字符数量
        //        //int byteCharCount = wrSize * 2;

        //        var wr = p_writer;

        //        fixed (char* charBufPtr = p_writerBuffer)
        //        {
        //            int charBufSize = p_writerBuffer.Length;
        //            int i;
        //            int charIndex = 0;

        //            for (i = 0; i < wrSize; i++)
        //            {

        //                byte tbv = bufPtr[i];

        //                f_byteToChars(bufPtr[i], charBufPtr + charIndex, p_upper);
        //                charBufPtr[charIndex + 2] = p_spe;

        //                charIndex += 3;

        //                if (charIndex + 3 >= charBufSize)
        //                {
        //                    //将charBuf写入基对象
        //                    wr.Write(p_writerBuffer, 0, charIndex + 1);
        //                    charIndex = 0;
        //                }

        //            }

        //            if(charIndex != 0)
        //            {
        //                //写入剩余缓存

        //                wr.Write(p_writerBuffer, 0, charIndex + 1);

        //            }

        //        }

        //        #endregion

        //    }

        //}

        //private void f_writer(char* charPtr, int count)
        //{
        //    var enc = p_encoding;

        //    var maxSize = cp_ToBufferByteMaxSize;

        //    int c = 0;
        //    if (count >= maxSize) c = enc.GetMaxByteCount(maxSize);

        //    while (true)
        //    {
        //        if (count >= maxSize)
        //        {

        //            f_write(charPtr, maxSize, c);
        //            count -= maxSize;
        //        }
        //        else
        //        {
        //            f_write(charPtr, count, enc.GetMaxByteCount(count));
        //        }

        //    }

        //}

        #endregion

        #endregion

        #region 派生

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            //p_isDisposed;
            f_flushAll(true);
            if (p_isDisposeWriter && disposeing)
            {
                p_writer.Close();
            }
            p_writer = null;
            return false;
        }

        #endregion

        public override void Flush()
        {
            if (IsDispose) return;
            f_flushAll(true);
        }

        /// <summary>
        /// 清理并将缓冲区数据写入基础封装器，可指定是否清理内部实例缓冲区
        /// </summary>
        /// <param name="flushBase">如果参数为true，则在清理完该实例缓冲区后，会调用内部封装对象的<see cref="TextWriter.Flush"/></param>
        public void Flush(bool flushBase)
        {
            if (IsDispose) return;
            f_flushAll(flushBase);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            f_writerEnd(buffer, index, count);

            //fixed (char* cp = buffer)
            //{
            //    if (count < cp_ToBufferByteMaxSize)
            //    {
            //        f_write(cp + index, count, p_encoding.GetMaxByteCount(count));
            //    }
            //    else
            //    {
            //        f_writer(cp + index, count);
            //    }
            //}
        }

        public override void Write(char[] buffer)
        {

            if (buffer is null) throw new ArgumentNullException();

            ThrowObjectDisposed();

            f_writerEnd(buffer, 0, buffer.Length);
        }

        public override void Write(char value)
        {
            //f_write(&value, 1, p_encoding.GetMaxByteCount(1));
            ThrowObjectDisposed();

            char[] wrBuf = p_writerBuffer;

            ref int pos = ref p_writerBufPos;
            ref int posEnd = ref p_writerBufPosEnd;
            int lastCount = f_writerBufNextCount();

            //lastCount = (lastCount < count) ? lastCount : count;
            if (lastCount == 0)
            {
                //剩余缓存为0
                f_flushAll(false);
                f_flush_leftWriterBuf();
            }
            posEnd++;
            wrBuf[posEnd] = value;
            
            //fixed (char* csp = wrBuf)
            //{
            //    Memorys.MemoryOperation.MemoryCopy(cs, csp + posEnd + 1, lastCount);
            //}

            //posEnd += lastCount;
        }

        /// <summary>
        /// 将字符串写入到该写入器
        /// </summary>
        /// <param name="value">要写入的字符串，null则不会写入</param>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public override void Write(string value)
        {
            ThrowObjectDisposed();
            if (value is null) return;

            int count = value.Length;

            if (count == 0) return;

            if(count == 1)
            {
                Write(value[0]);
                return;
            }

            fixed (char* cp = value)
            {
                f_writerEnd(cp, count);
            }

            //fixed (char* cp = value)
            //{
            //    if (count < cp_ToBufferByteMaxSize)
            //    {
            //        f_write(cp, count, p_encoding.GetMaxByteCount(count));
            //    }
            //    else
            //    {
            //        f_writer(cp, count);
            //    }
            //}
        }

        public override void WriteLine()
        {
            ThrowObjectDisposed();
            f_writerEnd(CoreNewLine, 0, CoreNewLine.Length);
        }

        /// <summary>
        /// 将指定位置中的字符序列写入并转化到该文本编写器中
        /// </summary>
        /// <param name="bufferAddress">要写入的字符序列的首地址</param>
        /// <param name="count">要写入的字符数量</param>
        public void Write(char* bufferAddress, int count)
        {
            ThrowObjectDisposed();
            if (bufferAddress == null) throw new ArgumentNullException();
            if (count < 0) throw new ArgumentOutOfRangeException();

            f_writerEnd(bufferAddress, count);
        }

        #endregion

        #endregion

    }

}
