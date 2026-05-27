using System;
using System.Collections.Generic;
using System.IO;
using Cheng.Streams;
using System.Text;
using System.Reflection;
using Cheng.Texts;
using Cheng.Memorys;

namespace Cheng.Algorithm.Encryptions.Base64Encryption
{

    /// <summary>
    /// 对Base64文本解码并以流数据读取的Base64解码器
    /// </summary>
    public sealed unsafe class Base64DecodeStream : HEStream
    {

        #region 构造

        const int defBufferSize = 512;

        /// <summary>
        /// 实例化Base64解码器
        /// </summary>
        /// <param name="reader">要读取的Base64文本读取器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public Base64DecodeStream(TextReader reader) : this(reader, true, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化Base64解码器
        /// </summary>
        /// <param name="reader">要读取的Base64文本读取器</param>
        /// <param name="disposeBaseReader">在释放时是否释放内部封装的文本读取器对象，默认参数为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public Base64DecodeStream(TextReader reader, bool disposeBaseReader) : this(reader, disposeBaseReader, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化Base64解码器
        /// </summary>
        /// <param name="reader">要读取的Base64文本读取器</param>
        /// <param name="disposeBaseReader">在释放时是否释放内部封装的文本读取器对象，默认参数为true</param>
        /// <param name="bufferSizeMagnification">
        /// <para>指定缓冲区长度倍率</para>
        /// <para>该参数表示Base64缓冲区长度的最小公倍数，内部使用该参数以3和4倍的长度分别初始化字节缓冲区和字符缓冲区</para>
        /// <para>该参数默认为512</para>
        /// </param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度没有大于0</exception>
        public Base64DecodeStream(TextReader reader, bool disposeBaseReader, int bufferSizeMagnification)
        {
            if (reader is null) throw new ArgumentNullException();
            if (bufferSizeMagnification <= 0) throw new ArgumentOutOfRangeException();

            p_disposeBaseReader = disposeBaseReader;
            p_base64Reader = reader;
            p_charBuffer = new char[bufferSizeMagnification * 4];
            p_buffer = new byte[bufferSizeMagnification * 3];
            f_init();
        }

        private void f_init()
        {
            p_bufferLen = 0;
            p_bufferPos = 0;
        }

        #endregion

        #region 参数

        private TextReader p_base64Reader;

        private char[] p_charBuffer;

        private byte[] p_buffer;

        /// <summary>
        /// 数据缓冲区当前填充的数量
        /// </summary>
        private int p_bufferLen;

        /// <summary>
        /// 数据缓冲区当前被外部读取到的索引
        /// </summary>
        private int p_bufferPos;

        private bool p_disposeBaseReader;

        #endregion

        #region 功能

        #region 封装

        static bool f_base64DictGetValue(char c, out byte re)
        {
            /*
            ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz 0123456789 + / "
                    0-25                   26-51                     52-61   62 63
            */
            if (c >= 'A' && c <= 'Z')
            {
                re = (byte)(c - 'A');
                return true;
            }

            if (c >= 'a' && c <= 'z')
            {
                re = (byte)((c - 'a') + 26);
                return true;
            }

            if(c >= '0' && c <= '9')
            {
                re = (byte)((c - '0') + 52);
                return true;
            }
            if(c == '+')
            {
                re = 62;
                return true;
            }
            if(c == '/')
            {
                re = 63;
                return true;
            }
            re = 0;
            return false;
        }

        /// <summary>
        /// 解码一组4个字符到字节
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        /// <param name="b1">解码后第1个字节</param>
        /// <param name="b2">解码后第2个字节，没有则null</param>
        /// <param name="b3">解码后第3个字节，没有则null</param>
        /// <returns>此次解码后空余的字节数，0表示没有空余，-1表示错误</returns>
        private int f_base64ToBytes(char c1, char c2, char c3, char c4, out byte b1, out byte? b2, out byte? b3)
        {
            const char bw = '=';
            //const byte band6 = 0b00111111
            byte d1, d2, d3, d4;
            uint bit24;

            b1 = default;
            b2 = null;
            b3 = null;

            if (c4 == bw)
            {

                if(c3 == bw)
                {
                    //两个占位符，空2byte
                    if (f_base64DictGetValue(c1, out d1) && f_base64DictGetValue(c2, out d2))
                    {
                        //加上补位填充4位共12位 4 + 8
                        //取消补位移位右移4位
                        bit24 = (((uint)d1) << ((6 - 4))) | ((((uint)d2) >> 4));
                        //bit24 = (bit24 & byte.MaxValue);

                        //还原数据
                        b1 = (byte)((bit24));

                        return 2;
                    }

                   
                    return -1;
                }
                else
                {
                    //一个占位符，空1byte
                    //获取前三个索引
                    if (f_base64DictGetValue(c1, out d1) && f_base64DictGetValue(c2, out d2) && f_base64DictGetValue(c3, out d3))
                    {
                        //加上补位填充18位 2 + 16
                        bit24 = (((uint)d1) << ((6 * 2))) | (((uint)d2) << ((6))) | (((uint)d3));
                        //取消补位
                        bit24 = (bit24 >> 2) & 0b00_11111111_11111111;

                        b1 = (byte)((bit24 >> (8)));
                        b2 = (byte)((bit24));

                        return 1;
                    }
                    return -1;
                }

            }
            else
            {
                //没有占位符

                if(f_base64DictGetValue(c1, out d1) && f_base64DictGetValue(c2, out d2) && f_base64DictGetValue(c3, out d3) && f_base64DictGetValue(c4, out d4))
                {
                    //全符合索引
                    bit24 = (((uint)d1) << (6 * 3)) | (((uint)d2) << (6 * 2)) | (((uint)d3) << (6 * 1)) | (d4);

                    b1 = (byte)((bit24 >> (8 * 2)));
                    b2 = (byte)((bit24 >> (8 * 1)));
                    b3 = (byte)((bit24));
                    return 0;
                }

                return -1;
            }
        }

        /// <summary>
        /// 从文本读取数据并解码到缓冲区（假设无缓冲区）
        /// </summary>
        /// <param name="over">此次读取已经无法读取新数据</param>
        /// <returns>是否成功正常解码</returns>
        private bool f_readToBuffer(out bool over)
        {
            over = false;
            p_bufferPos = 0;
            p_bufferLen = 0;
            var buffer = p_buffer;
            var charBuffer = p_charBuffer;

            var count = p_base64Reader.ReadBlock(charBuffer, 0, charBuffer.Length);

            if (count == 0)
            {
                over = true;
                return true;
            }

            if (count % 4 != 0)
            {
                return false;
            }

            int mc = count / 4;
            
            int i;
            byte b1;
            byte? b2, b3;
            
            for (i = 0; i < mc; i++)
            {
                var re = f_base64ToBytes(charBuffer[(i * 4)], charBuffer[(i * 4) + 1], charBuffer[(i * 4) + 2], charBuffer[(i * 4) + 3], out b1, out b2, out b3);

                if(re < 0)
                {
                    return false;
                }
                buffer[p_bufferLen++] = b1;
                if(b2.HasValue) buffer[p_bufferLen++] = b2.Value;
                if (b3.HasValue) buffer[p_bufferLen++] = b3.Value;
            }

            return true;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if(disposing && p_disposeBaseReader)
            {
                p_base64Reader?.Close();
            }
            p_base64Reader = null;
            return true;
        }

        #endregion

        #region 派生

        #region 参数访问

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        #endregion

        #region 读取功能

        /// <summary>
        /// 从文本读取器中解码Base64字符并读取解码后的字节到指定位置
        /// </summary>
        /// <param name="buffer">要读取到的字节数组</param>
        /// <param name="offset">读取到数组的位置偏移</param>
        /// <param name="count">此次需要读取的字节数量</param>
        /// <returns>此次实际读取到的字节数，返回0表示已读取到结尾；</returns>
        /// <exception cref="Base64EncoderException">读取的文本不是Base64文本</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定参数超出范围</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(Base64DecodeStream));
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (offset + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return 0;

            if (p_bufferPos >= p_bufferLen)
            {
                //缓冲区用完
                if (f_readToBuffer(out bool over))
                {
                    if (over) return 0; //到达结尾
                }
                else
                {
                    throw new Base64EncoderException();
                }
            }


            //剩余
            int lastCount = p_bufferLen - p_bufferPos;

            int rc = Math.Min(lastCount, count);

            Array.Copy(p_buffer, p_bufferPos, buffer, offset, rc);

            p_bufferPos += rc;

            return rc;
        }

        /// <summary>
        /// 从文本读取器中解码Base64字符并读取一个解码后的字节
        /// </summary>
        /// <returns>从中读取的一个字节数据，如果已到达结尾返回-1</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="Base64EncoderException">读取的文本不是Base64文本</exception>
        /// <exception cref="IOException">IO错误</exception>
        public override int ReadByte()
        {
            ThrowIsDispose(nameof(Base64DecodeStream));

            if (p_bufferPos >= p_bufferLen)
            {
                //缓冲区用完
                if (f_readToBuffer(out bool over))
                {
                    if(over) return -1; //到达结尾
                }
                else
                {
                    throw new Base64EncoderException();
                }
            }

            return p_buffer[p_bufferPos++];
        }

        #endregion

        #region 禁用功能

        static void f_NotSupportedError()
        {
            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        public override void WriteByte(byte value)
        {
            f_NotSupportedError();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            f_NotSupportedError();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            f_NotSupportedError();
            return 0;
        }

        public override void SetLength(long value)
        {
            f_NotSupportedError();
        }
        
        /// <summary>
        /// 该函数无实际意义
        /// </summary>
        public override void Flush()
        {
        }

        public override long Length
        {
            get
            {
                f_NotSupportedError();
                return 0;
            }
        }

        public override long Position 
        { 
            get
            {
                f_NotSupportedError();
                return 0;
            }
            set => f_NotSupportedError();
        }

        #endregion

        #endregion

        #endregion

    }


}
