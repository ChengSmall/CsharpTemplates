using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Cheng.Texts;
using Cheng.Memorys;

namespace Cheng.Algorithm.Encryptions.Base64Encryption
{

    /// <summary>
    /// 对流数据进行Base64编码并使用文本读取器读取Base64文本
    /// </summary>
    public sealed unsafe class Base64EncodeReader : SafeReleaseTextReader
    {

        #region 构造

        const int defBufSize = 512;

        /// <summary>
        /// 实例化一个Base64编码器
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public Base64EncodeReader(Stream stream) : this(stream, true, defBufSize)
        {
        }

        /// <summary>
        /// 实例化一个Base64编码器
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="disposeBaseStream">在释放对象时是否释放基本封装的流对象，默认值为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public Base64EncodeReader(Stream stream, bool disposeBaseStream) : this(stream, disposeBaseStream, defBufSize)
        {
        }

        /// <summary>
        /// 实例化一个Base64编码器
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="disposeBaseStream">在释放对象时是否释放基本封装的流对象，默认值为true</param>
        /// <param name="bufferSizeMagnification">
        /// <para>指定缓冲区长度倍率</para>
        /// <para>该参数表示Base64缓冲区长度的最小公倍数，内部使用该参数以3和4倍的长度分别初始化字节缓冲区和字符缓冲区</para>
        /// <para>该参数默认为512</para>
        /// </param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度不大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public Base64EncodeReader(Stream stream, bool disposeBaseStream, int bufferSizeMagnification)
        {
            if (stream is null) throw new ArgumentNullException();

            if (bufferSizeMagnification <= 0) throw new ArgumentOutOfRangeException();

            if ((!stream.CanRead))
            {
                throw new NotSupportedException();
            }

            f_init(stream, disposeBaseStream, bufferSizeMagnification);
        }

        private void f_init(Stream stream, bool disposeBaseStream, int bufferSize)
        {
            //p_base64Str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            p_stream = stream;
            p_disposeBaseStream = disposeBaseStream;
            p_charBuffer = new char[bufferSize * 4];
            p_buffer = new byte[bufferSize * 3];
            p_base64List = Base64EncodeList.ToCharArray();
            p_charBufferLen = 0;
            p_charBufferPos = 0;
        }

        #endregion

        #region 参数

        /// <summary>
        /// Base64编码列表字符串
        /// </summary>
        public const string Base64EncodeList = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private Stream p_stream;

        private char[] p_base64List;

        private char[] p_charBuffer;

        private byte[] p_buffer;

        /// <summary>
        /// 字符缓冲区当前填充的数量
        /// </summary>
        private int p_charBufferLen;

        /// <summary>
        /// 字符缓冲区当前被外部读取到的索引
        /// </summary>
        private int p_charBufferPos;

        private bool p_disposeBaseStream;

        #endregion

        #region 功能

        #region 封装

        /// <summary>
        /// 一组byte转化4个字符
        /// </summary>
        /// <param name="b0">第1个byte</param>
        /// <param name="b1">第2个byte，null表示没有剩余</param>
        /// <param name="b2">第3个byte，null表示没有剩余</param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        /// <returns>表示3个一组的byte缺失了多少byte</returns>
        private int f_transToBase64(byte b0, byte? b1, byte? b2, out char c1, out char c2, out char c3, out char c4)
        {
            bool b1h = b1.HasValue;
            bool b2h = b2.HasValue;
            uint bit24;
            const char bw = '=';

            const byte b6and = 0b00111111;

            if (b1h)
            {
                if (b2h)
                {
                    //没有缺失
                    bit24 = b2.Value | (((uint)b1.Value) << 8) | (((uint)b0) << 16);
                    //bp[0] = b0;
                    //bp[1] = b1.Value;
                    //bp[2] = b2.Value;
                    c1 = p_base64List[(((bit24) >> (6 * 3)) & b6and)];
                    c2 = p_base64List[((bit24 >> (6 * 2)) & b6and)];
                    c3 = p_base64List[((bit24 >> (6 * 1)) & b6and)];
                    c4 = p_base64List[(((bit24)) & b6and)];
                    return 0;
                }
                else
                {
                    //缺失一位

                    //组16位 后补2bit组成18位
                    bit24 = (((uint)b0) << 16) | (((uint)b1.Value) << 8);
                    //分离出三个字符
                    c1 = p_base64List[(((bit24) >> (6 * 3)) & 0b111111)];
                    c2 = p_base64List[((bit24 >> (6 * 2)) & 0b111111)];
                    c3 = p_base64List[((bit24 >> (6 * 1)) & 0b111111)];
                    //添加一个补位
                    c4 = bw;

                    return 1;
                }
            }
            else
            {
                //缺失两位
                //填充1byte后补4bit组成12位
                bit24 = (((uint)b0) << 4);
                //bp[0] = b0;
                //分离出两个字符
                c1 = p_base64List[((bit24 >> (6 * 1)) & 0b111111)];
                c2 = p_base64List[((bit24) & 0b111111)];
                c3 = bw;
                c4 = bw;
                return 2;
            }
        }

        /// <summary>
        /// 从基础流读取到缓冲区（假设缓冲区用完）
        /// </summary>
        /// <returns>是否到结尾</returns>
        private bool f_readBaseStream()
        {
            p_charBufferPos = 0;
            p_charBufferLen = 0;
            var buffer = p_buffer;
            var charBuffer = p_charBuffer;
            var count = p_stream.ReadBlock(buffer, 0, buffer.Length);

            if (count == 0) return true;
            char c1, c2, c3, c4;
            //组数
            var mc = count / 3;

            //剩余最后一组数
            var mcM = count % 3;

            int i;
            
            for (i = 0; i < mc; i++)
            {
                f_transToBase64(buffer[(i * 3)], buffer[(i * 3) + 1], buffer[(i * 3) + 2], out c1, out c2, out c3, out c4);

                charBuffer[p_charBufferLen] = c1;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c2;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c3;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c4;
                p_charBufferLen++;
            }
            

            if (mcM != 0)
            {
                var index = i * 3;
                if(mcM == 1)
                {
                    f_transToBase64(buffer[index], null, null, out c1, out c2, out c3, out c4);
                }
                else
                {
                    f_transToBase64(buffer[index], buffer[index + 1], null, out c1, out c2, out c3, out c4);
                }

                charBuffer[p_charBufferLen] = c1;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c2;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c3;
                p_charBufferLen++;
                charBuffer[p_charBufferLen] = c4;
                p_charBufferLen++;

            }

            return false;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            if (disposeing || p_disposeBaseStream)
            {
                p_stream?.Close();
            }
            p_stream = null;

            return true;
        }

        #endregion

        #region 派生

        public override int Read()
        {
            ThrowObjectDisposed(nameof(Base64EncodeReader));

            if (p_charBufferPos >= p_charBufferLen)
            {
                //缓冲区用完
                if (f_readBaseStream())
                {
                    return -1; //到达结尾
                }
            }

            return p_charBuffer[p_charBufferPos++];
        }

        public override int Peek()
        {
            ThrowObjectDisposed(nameof(Base64EncodeReader));

            if (p_charBufferPos >= p_charBufferLen)
            {
                //缓冲区用完
                if (f_readBaseStream())
                {
                    return -1; //到达结尾
                }
            }

            return p_charBuffer[p_charBufferPos];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed(nameof(Base64EncodeReader));
            if (buffer is null) throw new ArgumentNullException();
            if(index < 0 || count < 0 || (index + count > buffer.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return 0;

            if (p_charBufferPos >= p_charBufferLen)
            {
                //缓冲区用完
                if (f_readBaseStream())
                {
                    return 0; //到达结尾
                }
            }

            //p_charBufferPos;
            //p_charBufferLen;

            //剩余字符数
            int lastCount = p_charBufferLen - p_charBufferPos;

            int rc = Math.Min(lastCount, count);

            Array.Copy(p_charBuffer, p_charBufferPos, buffer, index, rc);

            p_charBufferPos += rc;

            return rc;
        }

        #endregion

        #endregion

    }

}
