using Cheng.Memorys;
using System;
using System.IO;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 文本和字符串扩展功能
    /// </summary>
    public unsafe static partial class TextManipulation
    {

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

        /// <summary>
        /// 查找字符串内匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>第一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int FindIndex(this string value, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找字符串指定范围内匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="index">要查找的起始索引位置</param>
        /// <param name="count">要查找的字符数量</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>第一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IndexOutOfRangeException">索引超出范围</exception>
        public static int FindIndex(this string value, int index, int count, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            int length = index + count;
            for (int i = index; i < length; i++)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找字符串从指定索引开始到最后之间匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="index">要查找的起始索引位置</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>第一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IndexOutOfRangeException">索引超出范围</exception>
        public static int FindIndex(this string value, int index, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            int length = value.Length;
            for (int i = index; i < length; i++)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// 反向查找字符串内匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>最后一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int LastFindIndex(this string value, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            int length = value.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// 反向查找字符串指定范围内匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="index">要查找的起始索引位置</param>
        /// <param name="count">要查找的字符数量</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>最后一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IndexOutOfRangeException">索引超出范围</exception>
        public static int LastFindIndex(this string value, int index, int count, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            for (int i = index + count - 1; i >= index; i--)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// 反向查找字符串从指定索引开始到最后之间匹配的字符并返回索引
        /// </summary>
        /// <param name="value">要查找的字符串</param>
        /// <param name="index">要查找的起始索引位置</param>
        /// <param name="predicate">进行匹配的谓词</param>
        /// <returns>最后一个匹配的索引；如果没有匹配字符则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IndexOutOfRangeException">索引超出范围</exception>
        public static int LastFindIndex(this string value, int index, Predicate<char> predicate)
        {
            if (value is null || predicate is null) throw new ArgumentNullException();
            for (int i = value.Length - 1; i >= index; i--)
            {
                if (predicate.Invoke(value[i])) return i;
            }
            return -1;
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

        #region 全角和半角

        /// <summary>
        /// 快速判断字符是否属于全角字符（占2个字母宽度）
        /// </summary>
        /// <param name="c">要判断的字符</param>
        /// <returns>返回true表示字符是全角字符，false表示半角字符；过于生僻的符号可能不会准确，该函数不会判断字符能否正常显示</returns>
        public static bool IsFullWidth(this char c)
        {
            //if (c <= 0x7E) return false;

            //// 常见全角字符快速检查
            //if (c >= 0xFF01 && c <= 0xFF60) return true;  // 全角ASCII变体
            //if (c >= 0xFFE0 && c <= 0xFFE6) return true;  // 全角符号

            //// 主要CJK字符范围检查
            //if (c >= 0x3000 && c <= 0x303F) return true;  // CJK符号和标点
            //if (c >= 0x3040 && c <= 0x309F) return true;  // 平假名
            //if (c >= 0x30A0 && c <= 0x30FF) return true;  // 片假名
            //if (c >= 0x4E00 && c <= 0x9FFF) return true;  // CJK统一表意文字
            //if (c >= 0xAC00 && c <= 0xD7AF) return true;  // 韩文字母
            //if (c >= 0xF900 && c <= 0xFAFF) return true;  // CJK兼容象形文字

            //return false;

            return (c > 0x7E) && ((c >= 0xFF01 && c <= 0xFF60) || (c >= 0xFFE0 && c <= 0xFFE6) || (c >= 0x3000 && c <= 0x303F) || (c >= 0x3040 && c <= 0x309F) || (c >= 0x30A0 && c <= 0x30FF) || (c >= 0x4E00 && c <= 0x9FFF) || (c >= 0x4E00 && c <= 0x9FFF) || (c >= 0xAC00 && c <= 0xD7AF) || (c >= 0xF900 && c <= 0xFAFF));
        }

        #endregion


    }

}
