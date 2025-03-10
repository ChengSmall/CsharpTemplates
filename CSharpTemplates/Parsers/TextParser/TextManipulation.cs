using Cheng.Memorys;
using System;
using System.IO;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 文本和字符串扩展功能
    /// </summary>
    public unsafe static class TextManipulation
    {

        #region 读写器

        /// <summary>
        /// 将读取器的文本读取并写入到另一个文本写入器中
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <param name="writer">待写入文本的写入器</param>
        /// <param name="buffer">文本读取缓冲区，容量必须大于0</param>
        /// <exception cref="ArgumentNullException">参数有null对象</exception>
        /// <exception cref="ArgumentException">缓冲区大小为空</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void CopyToText(this TextReader reader, TextWriter writer, char[] buffer)
        {
            if(reader is null || writer is null || buffer is null)
            {
                throw new ArgumentNullException();
            }

            int length = buffer.Length;

            if (length == 0) throw new ArgumentException();

            lock (buffer)
            {
                Loop:
                var re = reader.Read(buffer, 0, length);
                if (re == 0) return;
                writer.Write(buffer, 0, re);
                goto Loop;
            }
        }

        /// <summary>
        /// 将读取器的文本读取并写入到另一个文本写入器中
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <param name="writer">待写入文本的写入器</param>
        /// <exception cref="ArgumentNullException">参数有null对象</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void CopyToText(this TextReader reader, TextWriter writer)
        {
            CopyToText(reader, writer, new char[1024 * 4]);
        }

        #endregion

        #region 字符串\u格式转化

        /// <summary>
        /// （不安全）将字符串转化为\u格式文本字符串
        /// </summary>
        /// <param name="charPointer">指向字符串首地址的指针</param>
        /// <param name="charCount">待转化字符串的字符数量</param>
        /// <param name="isUpper">转化到\u格式的16进制字母是否为大写；true大写，false小写</param>
        /// <param name="append">要转化并写入到的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">指针或参数为null</exception>
        public static void CharsToUnicodeX16Chars(IntPtr charPointer, int charCount, bool isUpper, StringBuilder append)
        {
            if (append is null || charPointer == IntPtr.Zero) throw new ArgumentNullException();

            if (charCount == 0) return;

            UnicodeEscapeTextWriter.f_strToUnicode((char*)charPointer, charCount, isUpper, append);
        }

        /// <summary>
        /// 将字符串转化为\u格式文本字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">转化的字符串起始索引</param>
        /// <param name="count">转化的字符串字符数量</param>
        /// <param name="isUpper">转化到\u格式的16进制字母是否为大写；true大写，false小写</param>
        /// <param name="append">要转化并写入到的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void StringToUnicodeX16Text(this string str, int startIndex, int count, bool isUpper, StringBuilder append)
        {

            if (append is null || str is null) throw new ArgumentNullException();
            
            if (startIndex < 0 || count < 0 || (startIndex + count < str.Length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            fixed (char* strPtr = str)
            {
                UnicodeEscapeTextWriter.f_strToUnicode(strPtr + startIndex, count, isUpper, append);
            }
        }

        /// <summary>
        /// 将字符串转化为\u格式文本字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="startIndex">转化的字符串起始索引</param>
        /// <param name="count">转化的字符串字符数量</param>
        /// <param name="isUpper">转化到\u格式的16进制字母是否为大写；true大写，false小写</param>
        /// <returns>转化后的\u格式文本字符串</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string StringToUnicodeX16Text(this string str, int startIndex, int count, bool isUpper)
        {

            if (str is null) throw new ArgumentNullException();

            if (startIndex < 0 || count < 0 || (startIndex + count < str.Length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return string.Empty;

            StringBuilder append = new StringBuilder(count * 6);

            fixed (char* strPtr = str)
            {
                UnicodeEscapeTextWriter.f_strToUnicode(strPtr + startIndex, count, isUpper, append);
            }

            return append.ToString();
        }

        /// <summary>
        /// 将字符串转化为\u格式文本字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="isUpper">转化到\u格式的16进制字母是否为大写；true大写，false小写</param>
        /// <returns>转化后的\u格式文本字符串</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string StringToUnicodeX16Text(this string str, bool isUpper)
        {

            if (str is null) throw new ArgumentNullException();
            int count = str.Length;
            if (count == 0) return string.Empty;

            StringBuilder append = new StringBuilder(count * 6);

            fixed (char* strPtr = str)
            {
                UnicodeEscapeTextWriter.f_strToUnicode(strPtr, count, isUpper, append);
            }

            return append.ToString();
        }

        /// <summary>
        /// （不安全）将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="ustrPointer">表示\u格式文本的字符串首地址</param>
        /// <param name="length">要转化的\u格式文本所在字符串的字符数量</param>
        /// <param name="append">转化后要写入的字符串缓冲区</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool UnicodeX16CharsToString(this IntPtr ustrPointer, int length, StringBuilder append)
        {
            if (append is null || ustrPointer == IntPtr.Zero) throw new ArgumentNullException();

            return UnicodeEscapeTextReader.f_unicodeEscToStr((char*)ustrPointer, length, append);
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="ustr">\u文本字符串</param>
        /// <param name="startIndex">要转化的字符串起始位置</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <param name="append">转化后要写入的字符串缓冲区</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引范围超出</exception>
        public static bool UnicodeX16TextToString(this string ustr, int startIndex, int length, StringBuilder append)
        {

            if (ustr is null || append is null) throw new ArgumentNullException();

            if (startIndex < 0 || length < 0 || (startIndex + length > ustr.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* strPtr = ustr)
            {
                return UnicodeEscapeTextReader.f_unicodeEscToStr(strPtr + startIndex, length, append);
            }
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串写入缓冲区
        /// </summary>
        /// <param name="ustr">\u文本字符串</param>
        /// <param name="append">转化后要写入的字符串缓冲区</param>
        /// <returns>是否转化成功</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool UnicodeX16TextToString(this string ustr, StringBuilder append)
        {

            if (ustr is null || append is null) throw new ArgumentNullException();

            if (ustr.Length == 0) return true;

            fixed (char* strPtr = ustr)
            {
                return UnicodeEscapeTextReader.f_unicodeEscToStr(strPtr, ustr.Length, append);
            }
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串
        /// </summary>
        /// <param name="ustr">\u文本字符串</param>
        /// <param name="startIndex">要转化的字符串起始位置</param>
        /// <param name="length">要转化的字符串长度</param>
        /// <returns>转化后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引范围超出</exception>
        /// <exception cref="NotImplementedException">无法转化</exception>
        public static string UnicodeX16TextToString(this string ustr, int startIndex, int length)
        {

            if (ustr is null) throw new ArgumentNullException();

            if (startIndex < 0 || length < 0 || (startIndex + length > ustr.Length)) throw new ArgumentOutOfRangeException();

            if (ustr.Length == 0) return string.Empty;

            StringBuilder append = new StringBuilder(ustr.Length / 6);

            fixed (char* strPtr = ustr)
            {
                if (UnicodeEscapeTextReader.f_unicodeEscToStr(strPtr + startIndex, length, append)) return append.ToString();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 将指定的\u编码格式字符串转化为字符串
        /// </summary>
        /// <param name="ustr">\u文本字符串</param>
        /// <returns>转化后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">无法转化</exception>
        public static string UnicodeX16TextToString(this string ustr)
        {

            if (ustr is null) throw new ArgumentNullException();

            if (ustr.Length == 0) return string.Empty;

            StringBuilder append = new StringBuilder(ustr.Length / 6);

            fixed (char* strPtr = ustr)
            {
                if (UnicodeEscapeTextReader.f_unicodeEscToStr(strPtr, ustr.Length, append)) return append.ToString();
            }

            throw new NotImplementedException();
        }

        #endregion

        #region 字符串增删查改

        /// <summary>
        /// 清空字符串缓冲区内的所有字符
        /// </summary>
        /// <param name="sb"></param>
        /// <returns>此实例</returns>
        public static StringBuilder Clear(this StringBuilder sb)
        {
            sb.Length = 0;
            return sb;
        }

        /// <summary>
        /// 从字符串中删除指定数组中的字符并返回
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="chars">要移除的字符合集</param>
        /// <returns>移除指定字符合集后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static string RemoveChars(this string str, char[] chars)
        {
            if (str is null || chars is null) throw new ArgumentNullException();

            int length = str.Length;

            if (length == 0) return str;

            StringBuilder sb = new StringBuilder(length);

            bool b;
            char c;
            for (int i = 0; i < length; i++)
            {
                c = str[i];
                b = true;
                for (int j = 0; j < chars.Length; j++)
                {
                    if(c == chars[j])
                    {
                        b = false;
                        break;
                    }
                }

                if (b) sb.Append(c);
            }

            return sb.ToString();
        }

        #endregion

        #region 字符大小写转化

        /// <summary>
        /// 将读取器中的字符序列的字母转化为大写并写入到写入器
        /// </summary>
        /// <param name="reader">要读取的字符序列</param>
        /// <param name="toUpper">转化后要写入的字符序列</param>
        /// <param name="buffer">转化时读取字符的缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度为0</exception>
        public static void ToUpper(this TextReader reader, TextWriter toUpper, char[] buffer)
        {

            if (reader is null || toUpper is null || buffer is null) throw new ArgumentNullException();
            
            int length = buffer.Length;

            if (length == 0) throw new ArgumentOutOfRangeException();

            fixed (char* bp = buffer)
            {

                int re = reader.Read(buffer, 0, length);

                if (re == 0) return;

                Memorys.MemoryOperation.ToUpper(bp, bp, re);

                toUpper.Write(buffer, 0, re);

            }

        }

        /// <summary>
        /// 将读取器中的字符序列的字母转化为小写并写入到写入器
        /// </summary>
        /// <param name="reader">要读取的字符序列</param>
        /// <param name="toLopper">转化后要写入的字符序列</param>
        /// <param name="buffer">转化时读取字符的缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度为0</exception>
        public static void ToLopper(this TextReader reader, TextWriter toLopper, char[] buffer)
        {

            if (reader is null || toLopper is null || buffer is null) throw new ArgumentNullException();

            int length = buffer.Length;

            if (length == 0) throw new ArgumentOutOfRangeException();

            fixed (char* bp = buffer)
            {

                int re = reader.Read(buffer, 0, length);

                if (re == 0) return;

                Memorys.MemoryOperation.ToLopper(bp, bp, re);

                toLopper.Write(buffer, 0, re);

            }

        }

        /// <summary>
        /// 将指定字符数组的字符转化为大写并写入另一个字符数组
        /// </summary>
        /// <param name="buffer">要转化的原字符数组</param>
        /// <param name="index">原字符数组的起始位置</param>
        /// <param name="count">要转化的字符数量</param>
        /// <param name="toUpper">转化到的字符数组</param>
        /// <param name="toIndex">转化后写入的起始位置</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public static void ToUpper(this char[] buffer, int index, int count, char[] toUpper, int toIndex)
        {
            if (buffer is null || toUpper is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || toIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (index + count > buffer.Length || toIndex >= toUpper.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count > toUpper.Length - toIndex)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return;

            fixed (char* op = buffer, tp = toUpper)
            {

                MemoryOperation.ToUpper(op + index, tp + toIndex, count);

            }

        }

        /// <summary>
        /// 将指定字符数组的字符转化为小写并写入另一个字符数组
        /// </summary>
        /// <param name="buffer">要转化的原字符数组</param>
        /// <param name="index">原字符数组的起始位置</param>
        /// <param name="count">要转化的字符数量</param>
        /// <param name="toLopper">转化到的字符数组</param>
        /// <param name="toIndex">转化后写入的起始位置</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public static void ToLopper(this char[] buffer, int index, int count, char[] toLopper, int toIndex)
        {
            if (buffer is null || toLopper is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || toIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var toLength = toLopper.Length;

            if (index + count > buffer.Length || toIndex >= toLength)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count > toLength - toIndex)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return;

            fixed (char* op = buffer, tp = toLopper)
            {

                MemoryOperation.ToLopper(op + index, tp + toIndex, count);

            }

        }


        #endregion

        #region 字符串和值

        /// <summary>
        /// 将一个字节值转化为表示一个十六进制数的两个字符文本
        /// </summary>
        /// <param name="value">字节值</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="left">十六进制数的左侧字符</param>
        /// <param name="right">十六进制数的右侧字符</param>
        public static void ValueToX16Text(this byte value, bool toUpper, out char left, out char right)
        {
            byte b = (byte)(value >> 4);

            char firstA = toUpper ? 'A' : 'a';
            
            if (b < 10)
            {
                left = (char)('0' + b);
            }
            else
            {
                left = (char)(firstA + (b - 10));
            }

            b = (byte)(value & 0b1111);

            if (b < 10)
            {
                right = (char)('0' + b);
            }
            else
            {
                right = (char)(firstA + (b - 10));
            }
        }

        /// <summary>
        /// 将一个32位整数转化为十六进制文本
        /// </summary>
        /// <param name="value">32位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="charBuffer">转化到的字符缓冲区，需要保证该指针指向的位置有至少8个字符大小的可用空间</param>
        public static void ValueToX16Text(this uint value, bool toUpper, char* charBuffer)
        {
            const int length = sizeof(uint);

            for (int i = 0; i < length; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 8)) & 0xFF);

                ValueToX16Text(b, toUpper, out *(charBuffer + (i * 2)), out *(charBuffer + (i * 2 + 1)));
            }

        }

        /// <summary>
        /// 将一个64位整数转化为十六进制文本
        /// </summary>
        /// <param name="value">64位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="charBuffer">转化到的字符缓冲区，需要保证该指针指向的位置有至少16个字符大小的可用空间</param>
        public static void ValueToX16Text(this ulong value, bool toUpper, char* charBuffer)
        {
            const int length = sizeof(ulong);

            for (int i = 0; i < length; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 8)) & 0xFF);

                ValueToX16Text(b, toUpper, out *(charBuffer + (i * 2)), out *(charBuffer + (i * 2 + 1)));
            }

        }

        /// <summary>
        /// 将一个32位整数转化为十六进制文本
        /// </summary>
        /// <param name="value">32位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="buffer">转化到的字符缓冲区</param>
        /// <param name="index">从指定位置开始写入转化的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定的范围不足以承载转化文本</exception>
        public static void ValueToX16Text(this uint value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 8 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char * p = buffer)
            {
                ValueToX16Text(value, toUpper, p + index);
            }
        }

        /// <summary>
        /// 将一个64位整数转化为十六进制文本
        /// </summary>
        /// <param name="value">64位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="buffer">转化到的字符缓冲区</param>
        /// <param name="index">从指定位置开始写入转化的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定的范围不足以承载转化文本</exception>
        public static void ValueToX16Text(this ulong value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 8 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                ValueToX16Text(value, toUpper, p + index);
            }
        }

        /// <summary>
        /// 将两个字符组成的十六进制字节文本转化为字节值
        /// </summary>
        /// <param name="left">第一位，左侧的十六进制数</param>
        /// <param name="right">第二位，右侧的十六进制数</param>
        /// <param name="value">返回的字节值</param>
        /// <returns>如果成功转化返回true，如果无法转化为十六进制字节值返回false</returns>
        public static bool X16TextToByte(char left, char right, out byte value)
        {
            value = 0;

            #region right

            if (right >= '0' && right <= '9')
            {
                value = (byte)((right) - '0');
            }
            else if (right >= 'A' && right <= 'F')
            {
                value = (byte)(((right) - 'A') + 10);
            }
            else if (right >= 'a' && right <= 'f')
            {
                value = (byte)(((right) - 'a') + 10);
            }
            else
            {
                return false;
            }

            #endregion

            #region left

            if (left >= '0' && left <= '9')
            {
                value |= (byte)(((left) - '0') << 4);
            }
            else if (left >= 'A' && left <= 'F')
            {
                value |= (byte)((((left) - 'A') + 10) << 4);
            }
            else if (left >= 'a' && left <= 'f')
            {
                value |= (byte)((((left) - 'a') + 10) << 4);
            }
            else
            {
                return false;
            }

            #endregion

            return true;
        }

        static bool f_X16TextToBit4(char c, out byte value)
        {
            if (c >= '0' && c <= '9')
            {
                value = (byte)((c) - '0');
            }
            else if (c >= 'A' && c <= 'F')
            {
                value = (byte)(((c) - 'A') + 10);
            }
            else if (c >= 'a' && c <= 'f')
            {
                value = (byte)(((c) - 'a') + 10);
            }
            else
            {
                value = 0;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="charBuffer">指向表示16进制文本地址的指针</param>
        /// <param name="count">指示指针指向位置可用的字符长度</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        public static bool X16ToValue(char* charBuffer, int count, out ushort value)
        {
            const int size = sizeof(ushort) * 2;
            if (count > size) count = size;
            value = 0;
            byte b;
            int i;
            for (i = 0; i < count; i++)
            {
                if(!f_X16TextToBit4(charBuffer[i], out b))
                {
                    value = 0;
                    return false;
                }

                value |= (ushort)(b << (4 * (count - i - 1)));

            }

            return true;
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="buffer">表示16进制文本的字符缓冲区</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this char[] buffer, int index, int count, out ushort value)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                return X16ToValue(p + index, count, out value);
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="strValue">表示16进制文本的字符串</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this string strValue, int index, int count, out ushort value)
        {
            if (strValue is null) throw new ArgumentNullException();

            if (index + count > strValue.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = strValue)
            {
                return X16ToValue(p + index, count, out value);
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="charBuffer">指向表示16进制文本地址的指针</param>
        /// <param name="count">指示指针指向位置可用的字符长度</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        public static bool X16ToValue(char* charBuffer, int count, out uint value)
        {
            const int size = sizeof(uint) * 2;
            if (count > size) count = size;
            value = 0;
            byte b;
            int i;
            for (i = 0; i < count; i++)
            {
                if (!f_X16TextToBit4(charBuffer[i], out b))
                {
                    value = 0;
                    return false;
                }

                value |= ((uint)b << (4 * (count - i - 1)));

            }

            return true;
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="buffer">表示16进制文本的字符缓冲区</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this char[] buffer, int index, int count, out uint value)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                return X16ToValue(p + index, count, out value);
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="strValue">表示16进制文本的字符串</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this string strValue, int index, int count, out uint value)
        {
            if (strValue is null) throw new ArgumentNullException();

            if (index + count > strValue.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = strValue)
            {
                return X16ToValue(p + index, count, out value);
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="buffer">表示16进制文本的字符缓冲区</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this char[] buffer, int index, int count, out int value)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                fixed (int* vp = &value)
                {
                    return X16ToValue(p + index, count, out *(uint*)vp);
                }
                
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="buffer">表示16进制文本的字符串</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this char[] buffer, int index, int count, out long value)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                fixed (long* vp = &value)
                {
                    return X16ToValue(p + index, count, out *(ulong*)vp);
                }
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="strValue">表示16进制文本的字符缓冲区</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this string strValue, int index, int count, out int value)
        {
            if (strValue is null) throw new ArgumentNullException();

            if (index + count > strValue.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = strValue)
            {
                fixed (int* vp = &value)
                {
                    return X16ToValue(p + index, count, out *(uint*)vp);
                }

            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="strValue">表示16进制文本的字符串</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this string strValue, int index, int count, out long value)
        {
            if (strValue is null) throw new ArgumentNullException();

            if (index + count > strValue.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = strValue)
            {
                fixed (long* vp = &value)
                {
                    return X16ToValue(p + index, count, out *(ulong*)vp);
                }
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="charBuffer">指向表示16进制文本地址的指针</param>
        /// <param name="count">指示指针指向位置可用的字符长度</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        public static bool X16ToValue(char* charBuffer, int count, out ulong value)
        {
            const int size = sizeof(ulong) * 2;
            if (count > size) count = size;
            value = 0;
            byte b;
            int i;
            for (i = 0; i < count; i++)
            {
                if (!f_X16TextToBit4(charBuffer[i], out b))
                {
                    value = 0;
                    return false;
                }

                value |= ((ulong)b << (4 * (count - i - 1)));

            }

            return true;
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="buffer">表示16进制文本的字符缓冲区</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this char[] buffer, int index, int count, out ulong value)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index + count > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                return X16ToValue(p + index, count, out value);
            }
        }

        /// <summary>
        /// 将16进制文本转化为值
        /// </summary>
        /// <param name="strValue">表示16进制文本的字符串</param>
        /// <param name="index">文本的起始位置</param>
        /// <param name="count">指定要解析文本的字符数量</param>
        /// <param name="value">转化后的值</param>
        /// <returns>是否成功转化</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定长度错误</exception>
        public static bool X16ToValue(this string strValue, int index, int count, out ulong value)
        {
            if (strValue is null) throw new ArgumentNullException();

            if (index + count > strValue.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = strValue)
            {
                return X16ToValue(p + index, count, out value);
            }
        }


        #endregion

    }

}
