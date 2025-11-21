using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Memorys;
using System.IO;

namespace Cheng.Texts
{
    
    public unsafe static partial class TextManipulation
    {

        #region 字符串和值

        #region 16进制

        #region 16进制

        /// <summary>
        /// 将一个字节值的前4bit返回为一个表示十六进制的个位数文本
        /// </summary>
        /// <param name="value">字节值，该函数会忽略后4bit位</param>
        /// <param name="toUpper">字母位是否启用大写</param>
        /// <returns>范围在[0,F]的16进制数</returns>
        public static char ValueToX16Last4BitText(this byte value, bool toUpper)
        {
            value &= 0x0F;
            if (value < 10)
            {
                return (char)('0' + value);
            }
            else
            {
                return (char)((toUpper ? 'A' : 'a') + (value - 10));
            }
        }

        /// <summary>
        /// 将一个字节值转化为表示一个十六进制数的两个字符文本
        /// </summary>
        /// <param name="value">字节值</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="left">十六进制数的左侧字符</param>
        /// <param name="right">十六进制数的右侧字符</param>
        public static void ValueToX16Char(this byte value, bool toUpper, out char left, out char right)
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
        public static void ValueToFixedX16Text(this uint value, bool toUpper, char* charBuffer)
        {
            const int length = sizeof(uint);

            for (int i = 0; i < length; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 8)) & 0xFF);

                ValueToX16Char(b, toUpper, out *(charBuffer + (i * 2)), out *(charBuffer + (i * 2 + 1)));
            }

        }

        /// <summary>
        /// 将整数值转化为16进制文本，仅转化有值范围
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toUpper">字母位否大写</param>
        /// <param name="charBuffer">转化到的字符缓冲区起始位地址</param>
        /// <param name="bufferCount">字符缓冲区的可用字符数</param>
        public static void ValueToX16Text(this uint value, bool toUpper, char* charBuffer, int bufferCount)
        {
            const int length = sizeof(uint) * 2;

            char* firstC = charBuffer;
            char* endC = charBuffer + bufferCount;
            bool hasValue = false;
            for (int i = 0; i < length && firstC < endC; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 4)) & 0xF);

                if (b != 0)
                {
                    //有值
                    hasValue = true;
                }

                if (hasValue)
                {
                    *firstC = ValueToX16Last4BitText(b, toUpper);
                    firstC++;
                }
            }
        }

        /// <summary>
        /// 将一个64位整数转化为十六进制文本
        /// </summary>
        /// <param name="value">64位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="charBuffer">转化到的字符缓冲区，需要保证该指针指向的位置有至少16个字符大小的可用空间</param>
        public static void ValueToFixedX16Text(this ulong value, bool toUpper, char* charBuffer)
        {
            const int length = sizeof(ulong);

            for (int i = 0; i < length; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 8)) & 0xFF);

                ValueToX16Char(b, toUpper, out *(charBuffer + (i * 2)), out *(charBuffer + (i * 2 + 1)));
            }
        }

        /// <summary>
        /// 将整数值转化为16进制文本，仅转化有值范围
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toUpper">字母位否大写</param>
        /// <param name="charBuffer">转化到的字符缓冲区起始位地址</param>
        /// <param name="bufferCount">字符缓冲区的可用字符数</param>
        public static void ValueToX16Text(this ulong value, bool toUpper, char* charBuffer, int bufferCount)
        {
            const int length = sizeof(ulong) * 2;

            char* firstC = charBuffer;
            char* endC = charBuffer + bufferCount;
            bool hasValue = false;
            for (int i = 0; i < length && firstC < endC; i++)
            {
                byte b = (byte)((value >> (((length - 1) - i) * 4)) & 0xF);

                if (b != 0)
                {
                    //有值
                    hasValue = true;
                }

                if (hasValue)
                {
                    *firstC = ValueToX16Last4BitText(b, toUpper);
                    firstC++;
                }
            }
        }

        /// <summary>
        /// 将值转化为十六进制文本，仅转化有值范围
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="buffer">转化到的字符缓冲区</param>
        /// <param name="index">从指定位置开始写入转化的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定的索引超出范围</exception>
        public static void ValueToX16Text(this uint value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index >= buffer.Length) throw new ArgumentOutOfRangeException();
            if (index == buffer.Length) return;

            fixed (char* p = buffer)
            {
                ValueToX16Text(value, toUpper, p + index, buffer.Length - index);
            }
        }

        /// <summary>
        /// 将一个64位整数转化为十六进制文本，仅转化有值范围
        /// </summary>
        /// <param name="value">64位整数</param>
        /// <param name="toUpper">字母是否大写</param>
        /// <param name="buffer">转化到的字符缓冲区</param>
        /// <param name="index">从指定位置开始写入转化的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定的索引超出范围</exception>
        public static void ValueToX16Text(this ulong value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index >= buffer.Length) throw new ArgumentOutOfRangeException();
            if (index == buffer.Length) return;

            fixed (char* p = buffer)
            {
                ValueToX16Text(value, toUpper, p + index, buffer.Length - index);
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
        public static void ValueToFixedX16Text(this uint value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 8 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                ValueToFixedX16Text(value, toUpper, p + index);
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
        public static void ValueToFixedX16Text(this ulong value, bool toUpper, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 8 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                ValueToFixedX16Text(value, toUpper, p + index);
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

        #endregion

        #region 值

        static bool X16TextToBit4(char c, out byte value)
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
                if (!X16TextToBit4(charBuffer[i], out b))
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
                if (!X16TextToBit4(charBuffer[i], out b))
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
                if (!X16TextToBit4(charBuffer[i], out b))
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

        #endregion

        #region 二进制

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this ulong value, char fen)
        {
            char* cp = stackalloc char[8 * sizeof(ulong) + (sizeof(ulong) - 1)];
            MemoryOperation.ToBinStr(value, fen, cp);
            return new string(cp, 0, 8 * sizeof(ulong) + (sizeof(ulong) - 1));
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this long value, char fen)
        {
            return ToBin((ulong)value, fen);
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this uint value, char fen)
        {
            char* cp = stackalloc char[8 * sizeof(uint) + (sizeof(uint) - 1)];
            MemoryOperation.ToBinStr(value, fen, cp);
            return new string(cp, 0, 8 * sizeof(uint) + (sizeof(uint) - 1));
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this int value, char fen)
        {
            return ToBin((uint)value, fen);
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this ushort value, char fen)
        {
            char* cp = stackalloc char[8 * sizeof(ushort) + (sizeof(ushort) - 1)];
            MemoryOperation.ToBinStr(value, fen, cp);
            return new string(cp, 0, 8 * sizeof(ushort) + (sizeof(ushort) - 1));
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this short value, char fen)
        {
            char* cp = stackalloc char[8 * sizeof(short) + (sizeof(short) - 1)];
            MemoryOperation.ToBinStr((ushort)value, fen, cp);
            return new string(cp, 0, 8 * sizeof(short) + (sizeof(short) - 1));
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <returns></returns>
        public static string ToBin(this byte value)
        {
            const char c0 = '0';
            const char c1 = '1';
            char* cs = stackalloc char[8];
            for (int i = 0; i < 8; i++)
            {
                cs[7 - i] = ((value >> i) & 0b1) == 1 ? c1 : c0;
            }
            return new string(cs, 0, 8);
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this float value, char fen)
        {
            return ToBin(*(uint*)&value, fen);
        }

        /// <summary>
        /// 将值转化为二进制文本
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <returns>表示二进制文本的字符串</returns>
        public static string ToBin(this double value, char fen)
        {
            return ToBin(*(ulong*)&value, fen);
        }

        #endregion

        #endregion

    }

}
