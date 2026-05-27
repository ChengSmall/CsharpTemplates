using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 标准\u格式字符串解析读取器
    /// </summary>
    /// <remarks>
    /// 将封装的读取器内的\u格式字符串解析并转化为字符
    /// </remarks>
    public unsafe sealed class UnicodeEscapeTextReader : SafeReleaseTextReader
    {

        #region 构造

        /// <summary>
        /// 初始化标准\u格式字符串解析读取器
        /// </summary>
        /// <param name="textReader">要封装的读取\u格式字符串文本的基础读取器</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public UnicodeEscapeTextReader(TextReader textReader) : this(textReader, true)
        {
        }

        /// <summary>
        /// 初始化标准\u格式字符串解析读取器
        /// </summary>
        /// <param name="textReader">要封装的读取\u格式字符串文本的基础读取器</param>
        /// <param name="isDisposeReader">释放时是否释放基本读取器；默认为true</param>
        /// <exception cref="ArgumentNullException">读取器为null</exception>
        public UnicodeEscapeTextReader(TextReader textReader, bool isDisposeReader)
        {
            if (textReader is null) throw new ArgumentNullException();
            f_init(textReader, isDisposeReader);
        }

        private void f_init(TextReader textReader, bool isDisposeReader)
        {
            p_reader = textReader;
            p_isDisposeReader = isDisposeReader;
            p_charBuffer = new char[6];
            //p_charBufferPos = 0;
            //p_charBufferLength = 0;
            p_isEnd = false;
            //p_charBufferHaveBlock = false;
        }

        #endregion

        #region 参数

        private TextReader p_reader;

        /// <summary>
        /// 字符缓存
        /// </summary>
        private char[] p_charBuffer;
       
        //private int p_charBufferLength;

        /// <summary>
        /// peek缓存
        /// </summary>
        private int p_peekBuffer;
        //private int p_charBufferPos;

        private bool p_isEnd;

        private bool p_isDisposeReader;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取封装的基础读取器
        /// </summary>
        public TextReader BaseTextReader
        {
            get => p_reader;
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            if (disposeing)
            {
                if (p_isDisposeReader && p_reader != null)
                {
                    p_reader.Close();
                }
                p_reader = null;

                p_charBuffer = null;
            }

            return false;
        }

        #endregion

        #region 封装

        /// <summary>
        /// 将4个代表16位的16进制字符转化为一个16位值
        /// </summary>
        /// <param name="c4">第4位16进制数</param>
        /// <param name="c3">第3位16进制数</param>
        /// <param name="c2">第2位16进制数</param>
        /// <param name="c1">第1位16进制数</param>
        /// <param name="reValue">转化后的值</param>
        /// <returns>是否符合格式</returns>
        static bool f_charToX16(char c4, char c3, char c2, char c1, out ushort reValue)
        {
            //const ushort ToLopper = 0b00000000_00000000;

            const ushort ToUpper = 0b11111111_11011111;
            //ushort uv;

            char ct;

            int de;


            ct = c1;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                reValue = 0;
                return false;
            }

            reValue = (ushort)(de);

            ct = c2;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }


            reValue |= (ushort)(de << 4);


            ct = c3;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }

            reValue |= (ushort)(de << 8);

            ct = c4;

            if (ct >= '0' && ct <= '9')
            {
                de = (ct - '0');
            }
            else if ((ct >= 'A' && ct <= 'F') || (ct >= 'a' && ct <= 'f'))
            {
                de = (((ushort)ct & ToUpper) - 'A') + 10;
            }
            else
            {
                //不是指定格式
                return false;
            }

            reValue |= (ushort)(de << 12);

            return true;
        }

        /// <summary>
        /// 将一个\u转义化为char
        /// </summary>
        /// <param name="strp">一个字符串地址，从字符'\'开始向后6位格式检测 \uXXXX；必须满足向后6位字符内存</param>
        /// <param name="c">转化后的字符</param>
        /// <param name="nextPtr">转化后地址的下一位可能的'\'字符处</param>
        /// <returns>是否成功</returns>
        static bool f_UEtoChar(char* strp, out char c, out char* nextPtr)
        {
            c = default;
            nextPtr = strp + 6;

            if (strp[0] != '\\' || strp[1] != 'u') return false;

            if (!f_charToX16(strp[2], strp[3], strp[4], strp[5], out var uv)) return false;

            c = (char)uv;
            return true;
        }

        /// <summary>
        /// 从基础流中读取一块\u格式字符串字符到缓存
        /// </summary>
        /// <returns>是否读取为完整一块</returns>
        private bool f_readNextBlock()
        {
            var re = p_reader.ReadBlock(p_charBuffer, 0, 6);
            //p_charBufferLength = re;
            return re == 6;
        }

        /// <summary>
        /// 将当前缓存\u数组转化为字符
        /// </summary>
        /// <param name="c">转化到的字符</param>
        /// <returns>是否成功</returns>
        private bool f_peekBuffer(out char c)
        {
            c = default;
            //nextPtr = strp + 6;

            if (p_charBuffer[0] != '\\' || p_charBuffer[1] != 'u') return false;

            if (!f_charToX16(p_charBuffer[2], p_charBuffer[3], p_charBuffer[4], p_charBuffer[5], out var uv)) return false;

            c = (char)uv;
            return true;
        }

        /// <summary>
        /// 将当前缓存\u数组转化为字符并写入peek缓存变量
        /// </summary>
        /// <returns>是否转化成功</returns>
        private bool f_peekBuffer()
        {
            var b = f_peekBuffer(out char c);
            p_peekBuffer = (b ? c : -1);
            return b;
        }
        #endregion

        #region 派生

        public override int Peek()
        {
            ThrowObjectDisposed();
            if (p_peekBuffer == -1 && (!p_isEnd))
            {
                //没有块
                if (!f_readNextBlock())
                {
                    //p_peekBuffer = -1;
                    p_isEnd = true;
                    return -1; //没有读取到完整块
                }

                if(f_peekBuffer(out char re))
                {
                    p_peekBuffer = re;
                }
              
            }

            //peek

            return p_peekBuffer;

        }

        public override int Read()
        {
            ThrowObjectDisposed();
            if (p_isEnd)
            {
                //到达结尾
                return -1;
            }
            
            //没有块
            if (!f_readNextBlock())
            {
                //p_peekBuffer = -1;
                p_isEnd = true;
                return -1; //没有读取到完整块
            }

            if (!f_peekBuffer())
            {
                throw new ArgumentException();
            }

            //没有到达结尾


            return p_peekBuffer;

        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            int length = buffer.Length;
            if (index < 0 || count < 0 || (index + count > length))
            {
                throw new ArgumentOutOfRangeException();
            }

            int rec = 0;
            while (count != 0)
            {
                int re = Read();
                if (re == -1) break;
                buffer[index] = (char)re;
                index++;
                count--;
                rec++;
            }

            return rec;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Read(buffer, index, count);
        }

        public override string ReadToEnd()
        {
            ThrowObjectDisposed();
            if (p_isEnd) return string.Empty;
            
            StringBuilder sb = new StringBuilder(1024 * 2);

            while (true)
            {
                int re = Read();
                if (re == -1) break;
                sb.Append(((char)re));
            }

            return sb.ToString();
        }

        #endregion

        #region 扩展功能

        /// <summary>
        /// 从\u编码格式字符串转化到一般C#字符串
        /// </summary>
        /// <param name="unicodeStrPtr">\u格式字符串</param>
        /// <param name="length">长度</param>
        /// <param name="append">要转化到的缓冲区，向后添加</param>
        /// <returns>是否成功</returns>
        internal static bool f_unicodeEscToStr(char* unicodeStrPtr, int length, StringBuilder append)
        {

            if (length == 0) return true;

            int i;

            if (length % 6 != 0) return false;

            int blockLength = length / 6;

            char* handPtr = unicodeStrPtr;

            char rec;

            for (i = 0; i < blockLength; i++)
            {

                if(!f_UEtoChar(handPtr, out rec, out handPtr))
                {
                    return false;
                }

                append.Append(rec);
            }

            return true;
        }

        /// <summary>
        /// 从\u编码格式字符串转化到一般C#字符串
        /// </summary>
        /// <param name="unicodeStrPtr">\u格式字符串</param>
        /// <param name="length">长度</param>
        /// <param name="append">要转化到的缓冲区，向后添加</param>
        /// <returns>是否成功</returns>
        internal static bool f_unicodeEscToStr(char* unicodeStrPtr, int length, TextWriter append)
        {

            if (length == 0) return true;

            int i;

            if (length % 6 != 0) return false;

            int blockLength = length / 6;

            char* handPtr = unicodeStrPtr;

            char rec;

            for (i = 0; i < blockLength; i++)
            {

                if (!f_UEtoChar(handPtr, out rec, out handPtr))
                {
                    return false;
                }

                append.Write(rec);
            }

            return true;
        }

        /// <summary>
        /// （不安全）将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="unicodeStrPtr">指向\u编码格式字符串头的字符指针</param>
        /// <param name="length">\u编码格式字符串的字符长度</param>
        /// <param name="append">要添加到的字符串缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">长度小于0</exception>
        public static bool UnicodeEscapePointerToString(char* unicodeStrPtr, int length, StringBuilder append)
        {
            if (unicodeStrPtr == null || append == null) throw new ArgumentNullException();
            if (length < 0) throw new ArgumentOutOfRangeException();
            if (length == 0) return true;

            return f_unicodeEscToStr(unicodeStrPtr, length, append);
        }

        /// <summary>
        /// （不安全）将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="unicodeStrPtr">指向\u编码格式字符串头的字符指针</param>
        /// <param name="length">\u编码格式字符串的字符长度</param>
        /// <param name="append">要添加到的字符串缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">长度小于0</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        public static bool UnicodeEscapePointerToString(char* unicodeStrPtr, int length, TextWriter append)
        {
            if (unicodeStrPtr == null || append == null) throw new ArgumentNullException();
            if (length < 0) throw new ArgumentOutOfRangeException();
            if (length == 0) return true;

            return f_unicodeEscToStr(unicodeStrPtr, length, append);
        }


        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <param name="startIndex">\u编码格式字符串的起始索引</param>
        /// <param name="length">\u编码格式字符串的字符长度</param>
        /// <param name="append">要添加到的字符串缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        public static bool UnicodeEscapeToString(string unicodeStr, int startIndex, int length, StringBuilder append)
        {

            if (unicodeStr is null || append is null) throw new ArgumentNullException();

            if (startIndex < 0 || length < 0 || (startIndex + length > unicodeStr.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* strPtr = unicodeStr)
            {
                return f_unicodeEscToStr(strPtr + startIndex, length, append);
            }

        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <param name="startIndex">\u编码格式字符串的起始索引</param>
        /// <param name="append">要添加到的字符串缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        public static bool UnicodeEscapeToString(string unicodeStr, int startIndex, StringBuilder append)
        {

            if (unicodeStr is null || append is null) throw new ArgumentNullException();

            if (startIndex < 0 || (startIndex >= unicodeStr.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* strPtr = unicodeStr)
            {
                return f_unicodeEscToStr(strPtr + startIndex, unicodeStr.Length - startIndex, append);
            }
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <param name="append">要添加到的字符串缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool UnicodeEscapeToString(string unicodeStr, StringBuilder append)
        {

            if (unicodeStr is null || append is null) throw new ArgumentNullException();

            fixed (char* strPtr = unicodeStr)
            {
                return f_unicodeEscToStr(strPtr, unicodeStr.Length, append);
            }
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <param name="startIndex">\u编码格式字符串的起始索引</param>
        /// <param name="length">\u编码格式字符串的字符长度</param>
        /// <returns>转化后的原始字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        /// <exception cref="ArgumentException">转化的字符串格式不正确</exception>
        public static string UnicodeEscapeToString(string unicodeStr, int startIndex, int length)
        {

            if (unicodeStr is null) throw new ArgumentNullException();

            if (startIndex < 0 || length < 0 || (startIndex + length > unicodeStr.Length)) throw new ArgumentOutOfRangeException();

            if (length == 0) return string.Empty;

            StringBuilder append = new StringBuilder((length / 6));

            fixed (char* strPtr = unicodeStr)
            {
                if(f_unicodeEscToStr(strPtr + startIndex, length, append))
                {
                    return append.ToString();
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <param name="startIndex">\u编码格式字符串的起始索引</param>
        /// <returns>转化后的原始字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        /// <exception cref="ArgumentException">转化的字符串格式不正确</exception>
        public static string UnicodeEscapeToString(string unicodeStr, int startIndex)
        {

            if (unicodeStr is null) throw new ArgumentNullException();

            if (startIndex < 0 || (startIndex >= unicodeStr.Length)) throw new ArgumentOutOfRangeException();

            int length = unicodeStr.Length - startIndex;
            if (length == 0) return string.Empty;

            StringBuilder append = new StringBuilder((length / 6));

            fixed (char* strPtr = unicodeStr)
            {
                if (f_unicodeEscToStr(strPtr + startIndex, length, append))
                {
                    return append.ToString();
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串
        /// </summary>
        /// <param name="unicodeStr">\u编码格式字符串</param>
        /// <returns>转化后的原始字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">转化的字符串格式不正确</exception>
        public static string UnicodeEscapeToString(string unicodeStr)
        {

            if (unicodeStr is null) throw new ArgumentNullException();

            //if (startIndex < 0 || (startIndex >= unicodeStr.Length)) throw new ArgumentOutOfRangeException();

            int length = unicodeStr.Length;
            if (length == 0) return string.Empty;

            StringBuilder append = new StringBuilder((length / 6));

            fixed (char* strPtr = unicodeStr)
            {
                if (f_unicodeEscToStr(strPtr, length, append))
                {
                    return append.ToString();
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="buffer">\u编码格式的字符缓冲区</param>
        /// <param name="index">\u编码格式字符缓冲区起始索引</param>
        /// <param name="count">\u编码格式的字符缓冲区转化字符数量</param>
        /// <param name="append">转化后添加到的缓冲区；向后添加</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        public static bool UnicodeEscapeToString(char[] buffer, int index, int count, StringBuilder append)
        {

            if (buffer is null || append is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* strPtr = buffer)
            {
                return f_unicodeEscToStr(strPtr + index, count, append);
            }

        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="buffer">\u编码格式的字符缓冲区</param>
        /// <param name="index">\u编码格式字符缓冲区起始索引</param>
        /// <param name="count">\u编码格式的字符缓冲区转化字符数量</param>
        /// <returns>转化后的原始字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引长度超出范围</exception>
        public static string UnicodeEscapeToString(char[] buffer, int index, int count)
        {

            if (buffer is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return string.Empty;

            StringBuilder append = new StringBuilder(count / 6);

            fixed (char* strPtr = buffer)
            {
                if(f_unicodeEscToStr(strPtr + index, count, append))
                {
                    return append.ToString();
                }
            }

            throw new ArgumentException();
        }


        #endregion

        #endregion

    }

}
