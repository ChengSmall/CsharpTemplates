using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cheng.Streams;
using Cheng.Texts;
using Cheng.Algorithm;
using Cheng.DataStructure.Collections;
using System.Threading.Tasks;
using System.Threading;
using Cheng.Memorys;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Encryptions.Char16
{

    /// <summary>
    /// 半字节16字加密数据读取器
    /// </summary>
    public unsafe sealed class Char16EncStreamReader : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16">长度为16的字符数组，每个字符所在的索引代表字符对应的半字节值</param>
        /// <param name="createDictionary">用于创建字典的函数</param>
        /// <param name="reader">char16文本读取器</param>
        /// <exception cref="ArgumentNullException">参数有null</exception>
        /// <exception cref="ArgumentException">字符数组的长度不是16</exception>
        public Char16EncStreamReader(char[] char16, CreateDictionaryByCollection<int, int> createDictionary, TextReader reader) : this(char16, createDictionary, reader, true, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16">长度为16的字符数组，每个字符所在的索引代表字符对应的半字节值</param>
        /// <param name="createDictionary">用于创建字典的函数</param>
        /// <param name="reader">char16文本读取器</param>
        /// <param name="disposeBaseWriter">是否释放内部封装流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数有null</exception>
        /// <exception cref="ArgumentException">字符数组的长度不是16</exception>
        public Char16EncStreamReader(char[] char16, CreateDictionaryByCollection<int, int> createDictionary, TextReader reader, bool disposeBaseWriter) : this(char16, createDictionary, reader, disposeBaseWriter, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16">长度为16的字符数组，每个字符所在的索引代表字符对应的半字节值</param>
        /// <param name="createDictionary">用于创建字典的函数</param>
        /// <param name="reader">char16文本读取器</param>
        /// <param name="disposeBaseWriter">是否释放内部封装流，默认为true</param>
        /// <param name="bufferSize">缓冲区长度，默认是2048</param>
        /// <exception cref="ArgumentNullException">参数有null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度没有大于0</exception>
        /// <exception cref="ArgumentException">字符数组的长度不是16</exception>
        public Char16EncStreamReader(char[] char16, CreateDictionaryByCollection<int, int> createDictionary, TextReader reader, bool disposeBaseWriter, int bufferSize)
        {
            if (char16 is null || reader is null || createDictionary is null) throw new ArgumentNullException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (char16.Length != 16) throw new ArgumentException();

            p_dict = f_init(char16, createDictionary) ?? throw new ArgumentNullException();

            p_reader = reader;
            p_disposeBase = disposeBaseWriter;
            p_buffer = new byte[bufferSize];
            p_charBuf = new char[bufferSize * 2];
            p_bufLength = 0;
            p_bufPos = 0;
        }

        private static System.Collections.Generic.IReadOnlyDictionary<int, int> f_init(char[] char16, CreateDictionaryByCollection<int, int> createDictionaryFunc)
        {
            return createDictionaryFunc.Invoke(Enumerable.Range(0, 16), f_toKey, EqualityComparer<int>.Default);

            // 通过value (index)获取key (char)函数
            int f_toKey(int t_c)
            {
                return char16[t_c];
            }
        }

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16Pairs">
        /// <para>一个映射字符与半字节数据的字典，key表示可转化为字符类型的int32值，value表示字符对应的半字节值</para>
        /// </param>
        /// <param name="reader">char16文本读取器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public Char16EncStreamReader(System.Collections.Generic.IReadOnlyDictionary<int, int> char16Pairs, TextReader reader) : this(char16Pairs, reader, true, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16Pairs">
        /// <para>一个映射字符与半字节数据的字典，key表示可转化为字符类型的int32值，value表示字符对应的半字节值</para>
        /// </param>
        /// <param name="reader">char16文本读取器</param>
        /// <param name="disposeBaseWriter">是否释放内部封装流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public Char16EncStreamReader(System.Collections.Generic.IReadOnlyDictionary<int, int> char16Pairs, TextReader reader, bool disposeBaseWriter) : this(char16Pairs, reader, disposeBaseWriter, 1024 * 2)
        {
        }

        /// <summary>
        /// 实例化半字节16字加密数据读取器
        /// </summary>
        /// <param name="char16Pairs">
        /// <para>一个映射字符与半字节数据的字典，key表示可转化为字符类型的int32值，value表示字符对应的半字节值</para>
        /// </param>
        /// <param name="reader">char16文本读取器</param>
        /// <param name="disposeBaseWriter">是否释放内部封装流，默认为true</param>
        /// <param name="bufferSize">缓冲区长度，默认是2048</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度没有大于0</exception>
        public Char16EncStreamReader(System.Collections.Generic.IReadOnlyDictionary<int, int> char16Pairs, TextReader reader, bool disposeBaseWriter, int bufferSize)
        {
            if (char16Pairs is null || reader is null) throw new ArgumentNullException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));
            p_reader = reader;
            p_dict = char16Pairs;
            p_disposeBase = disposeBaseWriter;
            p_buffer = new byte[bufferSize];
            p_charBuf = new char[bufferSize * 2];
            p_bufLength = 0;
            p_bufPos = 0;
        }

        #endregion

        #region 参数

#if DEBUG
        /// <summary>
        /// 字符键值对
        /// </summary>
        /// <remarks>
        /// <para>key代表要查找的char值，value表示char对应的半字节值</para>
        /// </remarks>
#endif
        private System.Collections.Generic.IReadOnlyDictionary<int, int> p_dict;

        private TextReader p_reader;

        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 相对 p_buffer 的双倍缓冲区
        /// </summary>
#endif
        private char[] p_charBuf;

#if DEBUG
        /// <summary>
        /// buffer 的当前能够读取已有数据长度
        /// </summary>
#endif
        private int p_bufLength;

#if DEBUG
        /// <summary>
        /// buffer 当前读取到的位置
        /// </summary>
#endif
        private int p_bufPos;

        private bool p_disposeBase;

        #endregion

        #region 功能

        #region 封装

        static string f_getErrorMeg()
        {
            return Cheng.Properties.Resources.Exception_Char16NotConvertBytes;
        }

#if DEBUG
        /// <summary>
        /// 从基础文本读取并解码到缓冲区（假设缓冲区已经没有可读数据）
        /// </summary>
        /// <returns>此次读取到缓冲区的字节量</returns>
#endif
        private int f_readToBuf()
        {
            //字符读取数
            var rcc = p_reader.ReadBlock(p_charBuf, 0, p_charBuf.Length);
            if (rcc < 2)
            {
                //小于2
                return 0;
            }

            //可转化的字节数
            var bc = rcc / 2;

            fixed (byte* bufPtr = p_buffer)
            {
                fixed(char* cp = p_charBuf)
                {
                    int ci = 0;
                    for (int i = 0; i < bc; i++)
                    {
                        //获取两位映射字符
                        char c1, c2;
                        c1 = cp[ci++];
                        c2 = cp[ci++];

                        int b1, b2;
                        //从字典获取映射值
                        if(p_dict.TryGetValue((int)c1, out b1) && p_dict.TryGetValue((int)c2, out b2))
                        {
                            //合并
                            bufPtr[i] = (byte)((b1 & 0x0F) | ((b2 << 4) & 0xF0));
                        }
                        else
                        {
                            throw new NotImplementedException(f_getErrorMeg());
                        }
                    }
                }
            }

            p_bufLength = bc;
            p_bufPos = 0;
            return bc;
        }


#if DEBUG
        /// <summary>
        /// 从缓冲区读取一次到内存
        /// </summary>
        /// <param name="buffer">要读取到的位置</param>
        /// <param name="count">要读取的字节量</param>
        /// <returns>本次实际读取的字节数</returns>
#endif
        private int f_readOnce(byte* buffer, int count)
        {
            fixed (byte* bp = p_buffer)
            {
                //计算此次要写入的值
                int cpc = Math.Min(count, p_bufLength - p_bufPos);
                MemoryOperation.MemoryCopy(bp + p_bufPos, buffer, cpc);
                p_bufPos += cpc;
                return cpc;
            }

        }

        private int f_read(byte* buffer, int count)
        {
            int rec;
            int wrc = 0;
            while (count > 0)
            {
                rec = f_readOnce(buffer + wrc, count);
                wrc += rec;
                if (rec == 0)
                {
                    //p_bufPos = 0;
                    var recb = f_readToBuf();
                    if(recb == 0)
                    {
                        return wrc;
                    }
                }
                count -= rec;
            }

            return wrc;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if (disposing && p_disposeBase)
            {
                p_reader.Close();
            }
            p_reader = null;
            p_charBuf = null;
            p_dict = null;
            p_buffer = null;

            return true;
            //return base.Disposing(disposing);
        }

        #endregion

        #region 派生

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            var length = buffer.Length;
            if(offset < 0 || count < 0 || (count + offset) > length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (count == 0) return 0;
            int re;
            fixed (byte* bp = buffer)
            {
                re = f_read(bp + offset, count);
            }
            return re;

            //if(!re.HasValue)
            //{
            //    throw new NotImplementedException();
            //}
            //return re.Value;

        }

        public override int ReadByte()
        {
            int? len = p_bufLength - p_bufPos;
            if(len.Value == 0)
            {
                p_bufPos = 0;
                len = f_readToBuf();
                if (len.HasValue)
                {
                    if (len.Value == 0) return -1;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return p_buffer[p_bufPos++];
        }

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (count == 0) return 0;
            return f_read(buffer, count);
        }

        public override void Flush()
        {
        }

        #region 无实现

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        public override unsafe void WriteToAddress(byte* buffer, int count)
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