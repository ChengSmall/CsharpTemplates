using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 将字符串转化为\u格式字符串的文本写入器
    /// </summary>
    public unsafe sealed class UnicodeEscapeTextWriter : SafeReleaseTextWriter
    {

        #region 构造

        /// <summary>
        /// 实例化一个将字符串转化为\u格式字符串的文本写入器
        /// </summary>
        /// <param name="textWriter">封装的基础写入器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnicodeEscapeTextWriter(TextWriter textWriter) : this(textWriter, true, true)
        {
        }

        /// <summary>
        /// 实例化一个将字符串转化为\u格式字符串的文本写入器
        /// </summary>
        /// <param name="textWriter">封装的基础写入器</param>
        /// <param name="isUpper">转化为\u格式时的16进制数是否使用大写字母；true表示大写，false则使用小写；该参数默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnicodeEscapeTextWriter(TextWriter textWriter, bool isUpper) : this(textWriter, isUpper, true)
        {
        }

        /// <summary>
        /// 实例化一个将字符串转化为\u格式字符串的文本写入器
        /// </summary>
        /// <param name="textWriter">封装的基础写入器</param>
        /// <param name="isUpper">转化为\u格式时的16进制数是否使用大写字母；true表示大写，false则使用小写；该参数默认为true</param>
        /// <param name="isDisposeWriter">在释放对象时是否释放封装的基础写入器实例；默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public UnicodeEscapeTextWriter(TextWriter textWriter, bool isUpper, bool isDisposeWriter)
        {
            if (textWriter is null) throw new ArgumentNullException();
            f_init(textWriter, isUpper, isDisposeWriter);
        }


        private void f_init(TextWriter textWriter, bool isUpper, bool isDisposeWriter)
        {
            p_writer = textWriter;
            p_isUpper = isUpper;
            p_isDisposeWriter = isDisposeWriter;
        }

        #endregion

        #region 参数

        private TextWriter p_writer;

        private bool p_isUpper;

        private bool p_isDisposeWriter;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取基础写入器的字符编码
        /// </summary>
        public override Encoding Encoding => p_writer.Encoding;

        public override IFormatProvider FormatProvider => p_writer.FormatProvider;

        /// <summary>
        /// 转化的\u格式字符串16进制数是否使用大写字母
        /// </summary>
        public bool U16IsUpper
        {
            get => p_isUpper;
        }

        /// <summary>
        /// 获取封装的基础写入器
        /// </summary>
        public TextWriter BaseWriter
        {
            get => p_writer;
        }

        #endregion

        #region 派生

        private void f_writer(char* charPtr, int length)
        {
            for (int i = 0; i < length; i++)
            {
                f_writerChar(charPtr[i], p_isUpper, p_writer);
            }
        }

        public override void Write(char value)
        {
            ThrowObjectDisposed();
            f_writerChar(value, p_isUpper, p_writer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            for (int i = index; i < count; i++)
            {
                f_writerChar(buffer[i], p_isUpper, p_writer);
            }
        }

        public override void Write(char[] buffer)
        {
            ThrowObjectDisposed();
            if (buffer is null) return;

            for (int i = 0; i < buffer.Length; i++)
            {
                f_writerChar(buffer[i], p_isUpper, p_writer);
            }

        }

        public override void WriteLine()
        {
            ThrowObjectDisposed();
            for (int i = 0; i < CoreNewLine.Length; i++)
            {
                f_writerChar(CoreNewLine[i], p_isUpper, p_writer);
            }
        }

        public override void Write(string value)
        {
            ThrowObjectDisposed();
            if (value is null || value.Length == 0) return;
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                f_writerChar(value[i], p_isUpper, p_writer);
            }
        }

        public override void WriteLine(string value)
        {
            ThrowObjectDisposed();
            Write(value);
            for (int i = 0; i < CoreNewLine.Length; i++)
            {
                f_writerChar(CoreNewLine[i], p_isUpper, p_writer);
            }
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            int i;
            for (i = index; i < count; i++)
            {
                f_writerChar(buffer[i], p_isUpper, p_writer);
            }
            for (i = 0; i < CoreNewLine.Length; i++)
            {
                f_writerChar(CoreNewLine[i], p_isUpper, p_writer);
            }
        }

        public override void WriteLine(char[] buffer)
        {
            ThrowObjectDisposed();
            int i;

            if (buffer != null)
            {
                for (i = 0; i < buffer.Length; i++)
                {
                    f_writerChar(buffer[i], p_isUpper, p_writer);
                }
            }

            for (i = 0; i < CoreNewLine.Length; i++)
            {
                f_writerChar(CoreNewLine[i], p_isUpper, p_writer);
            }
        }

        public override void Flush()
        {
            p_writer?.Flush();
        }

        #endregion

        #region 释放

        protected override bool Disposing(bool disposeing)
        {
            p_writer.Flush();
            if (p_isDisposeWriter && disposeing)
            {
                p_writer.Close();
            }
            p_writer = null;
            return false;
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 将str转到\u格式文本
        /// </summary>
        /// <param name="strPtr">字符串首地址</param>
        /// <param name="length">字符串元素数</param>
        /// <param name="isUpper">转是否为大写</param>
        /// <param name="append">转化写入的\u字符串</param>
        internal static void f_strToUnicode(char* strPtr, int length, bool isUpper, StringBuilder append)
        {
            int i;

            for (i = 0; i < length; i++)
            {
                append.Append('\\');
                append.Append('u');
                append.Append(f_toU16Char(strPtr[i], 3, isUpper));
                append.Append(f_toU16Char(strPtr[i], 2, isUpper));
                append.Append(f_toU16Char(strPtr[i], 1, isUpper));
                append.Append(f_toU16Char(strPtr[i], 0, isUpper));
            }

        }

        /// <summary>
        /// 将str转到\u格式文本
        /// </summary>
        /// <param name="strPtr">字符串首地址</param>
        /// <param name="length">字符串元素数</param>
        /// <param name="isUpper">转是否为大写</param>
        /// <param name="append">转化写入的\u字符串</param>
        internal static void f_strToUnicode(char* strPtr, int length, bool isUpper, TextWriter append)
        {
            int i;

            for (i = 0; i < length; i++)
            {
                append.Write('\\');
                append.Write('u');
                append.Write(f_toU16Char(strPtr[i], 3, isUpper));
                append.Write(f_toU16Char(strPtr[i], 2, isUpper));
                append.Write(f_toU16Char(strPtr[i], 1, isUpper));
                append.Write(f_toU16Char(strPtr[i], 0, isUpper));
            }

        }

        /// <summary>
        /// （不安全代码）将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="stringPointer">指向字符串头的字符指针</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <param name="append">转化后添加到的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串长度小于0</exception>
        public static void StringPointerToUnicodeFormat(char* stringPointer, int length, bool isUpper, StringBuilder append)
        {
            if (stringPointer == null || append == null) throw new ArgumentNullException();

            if (length < 0) throw new ArgumentOutOfRangeException();

            if (length == 0) return;

            for (int i = 0; i < length; i++)
            {
                append.Append('\\');
                append.Append('u');
                append.Append(f_toU16Char(stringPointer[i], 3, isUpper));
                append.Append(f_toU16Char(stringPointer[i], 2, isUpper));
                append.Append(f_toU16Char(stringPointer[i], 1, isUpper));
                append.Append(f_toU16Char(stringPointer[i], 0, isUpper));
            }
        }

        /// <summary>
        /// （不安全代码）将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="stringPointer">指向字符串头的字符指针</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <param name="append">转化后添加到的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串长度小于0</exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        public static void StringPointerToUnicodeFormat(char* stringPointer, int length, bool isUpper, TextWriter append)
        {
            if (stringPointer == null || append == null) throw new ArgumentNullException();

            if (length < 0) throw new ArgumentOutOfRangeException();

            if (length == 0) return;

            for (int i = 0; i < length; i++)
            {
                append.Write('\\');
                append.Write('u');
                append.Write(f_toU16Char(stringPointer[i], 3, isUpper));
                append.Write(f_toU16Char(stringPointer[i], 2, isUpper));
                append.Write(f_toU16Char(stringPointer[i], 1, isUpper));
                append.Write(f_toU16Char(stringPointer[i], 0, isUpper));
            }
        }


        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="index">字符串起始索引</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <param name="append">转化后添加到的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串索引超出范围</exception>
        public static void StringToUnicodeFormat(string str, int index, int length, bool isUpper, StringBuilder append)
        {

            if (str is null || append is null) throw new ArgumentNullException();

            int strL = str.Length;

            if (index < 0 || length < 0 || (length + index > strL))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (length == 0) return;

            fixed (char* cp = str)
            {
                f_strToUnicode(cp + index, length, isUpper, append);
            }
        }

        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <param name="append">转化后添加到的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void StringToUnicodeFormat(string str, bool isUpper, StringBuilder append)
        {
            if (str is null || append is null) throw new ArgumentNullException();

            //if (index < 0 || length < 0 || (length + index > strL))
            //{
            //    throw new ArgumentOutOfRangeException();
            //}
            if (str.Length == 0) return;

            fixed (char* cp = str)
            {
                f_strToUnicode(cp, str.Length, isUpper, append);
            }
        }

        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="index">字符串起始索引</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <returns>转化后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串索引超出范围</exception>
        public static string StringToUnicodeFormat(string str, int index, int length, bool isUpper)
        {

            if (str is null) throw new ArgumentNullException();

            int strL = str.Length;

            if (index < 0 || length < 0 || (length + index > strL))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (length == 0) return string.Empty;

            StringBuilder append = new StringBuilder(length * 6);

            fixed (char* cp = str)
            {
                f_strToUnicode(cp + index, length, isUpper, append);
            }

            return append.ToString();
        }

        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <returns>转化后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static string StringToUnicodeFormat(string str, bool isUpper)
        {

            if (str is null) throw new ArgumentNullException();

            int strL = str.Length;

            //if (index < 0 || length < 0 || (length + index > strL))
            //{
            //    throw new ArgumentOutOfRangeException();
            //}

            if (strL == 0) return string.Empty;

            StringBuilder append = new StringBuilder(strL * 6);

            fixed (char* cp = str)
            {
                f_strToUnicode(cp, strL, isUpper, append);
            }

            return append.ToString();
        }

        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="charBuffer">要转化的字符缓冲区</param>
        /// <param name="index">要转化的字符缓冲区起始索引</param>
        /// <param name="count">要转化的字符缓冲区长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <param name="append">转化后添加到的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串索引超出范围</exception>
        public static void StringToUnicodeFormat(char[] charBuffer, int index, int count, bool isUpper, StringBuilder append)
        {

            if (charBuffer is null || append is null) throw new ArgumentNullException();

            int strL = charBuffer.Length;

            if (index < 0 || count < 0 || (count + index > strL))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return;

            fixed (char* cp = charBuffer)
            {
                f_strToUnicode(cp + index, count, isUpper, append);
            }
        }

        /// <summary>
        /// 将字符串转化为\u格式字符编码文本
        /// </summary>
        /// <param name="charBuffer">要转化的字符缓冲区</param>
        /// <param name="index">要转化的字符缓冲区起始索引</param>
        /// <param name="count">要转化的字符缓冲区长度</param>
        /// <param name="isUpper">转化到\u时的16进制的字母是否使用大写；true表示大写，false表示小写</param>
        /// <returns>转化后的\u格式字符编码文本</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符串索引超出范围</exception>
        public static string StringToUnicodeFormat(char[] charBuffer, int index, int count, bool isUpper)
        {

            if (charBuffer is null) throw new ArgumentNullException();

            int strL = charBuffer.Length;

            if (index < 0 || count < 0 || (count + index > strL))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return string.Empty;

            StringBuilder append = new StringBuilder(count * 6);

            fixed (char* cp = charBuffer)
            {
                f_strToUnicode(cp + index, count, isUpper, append);
            }

            return append.ToString();
        }

        #endregion

        #region 封装

        /// <summary>
        /// 访问指定字符值内4个16进制数的位值
        /// </summary>
        /// <param name="value">字符值</param>
        /// <param name="x16Index">16进制数的访问位数[0,3]</param>
        /// <param name="isUpper">转化后的位值的字母是否为大写</param>
        /// <returns>转化后的指定位值</returns>
        static char f_toU16Char(char value, int x16Index, bool isUpper)
        {

            byte bv = (byte)((value >> (x16Index * 4)) & 0xF);

            if(bv <= 0x9)
            {
                return (char)('0' + bv);
            }
            
            return (char)((isUpper ? 'A' : 'a') + (bv - 10));

        }

        /// <summary>
        /// 将指定字符转化为\u并写入writer
        /// </summary>
        /// <param name="c">字符</param>
        /// <param name="isUpper">16进制数大小写开关</param>
        /// <param name="writer">写入的实例</param>
        private static void f_writerChar(char c, bool isUpper, TextWriter writer)
        {
            writer.Write('\\');
            writer.Write('u');
            writer.Write(f_toU16Char(c, 3, isUpper));
            writer.Write(f_toU16Char(c, 2, isUpper));
            writer.Write(f_toU16Char(c, 1, isUpper));
            writer.Write(f_toU16Char(c, 0, isUpper));
        }

        #endregion

        #endregion

    }

}
