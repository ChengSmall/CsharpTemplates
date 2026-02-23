using Cheng.Memorys;
using System;


namespace Cheng.Algorithm.HashCodes
{

    /// <summary>
    /// 字符串哈希计算
    /// </summary>
    public static unsafe class StringHashCode
    {

        private static int f_stringHashToBuffer(char* charBuf, int charCount, byte* buffer, int count)
        {
            var n = 2166136261U;
            ushort* pn = (ushort*)&n;
            int bi = 0;
            int ct = 0;
            for (int i = 0; i < charCount; i++, bi++)
            {
                n = (charBuf[i] ^ n) * 16777619U;
                if (bi >= count) bi = 0;
                buffer[bi] ^= (byte)((pn[0] ^ pn[1]) % byte.MaxValue);
                ct++;
            }
            return ct;
        }

        private static int f_stringHashToBuffer(char* charBuf, byte* buffer, int count)
        {
            var n = 2166136261U;
            ushort* pn = (ushort*)&n;
            int bi = 0;
            int ct = 0;
            for (int i = 0; charBuf[i] != '\0'; i++, bi++)
            {
                n = (charBuf[i] ^ n) * 16777619U;
                if (bi >= count) bi = 0;
                buffer[bi] ^= (byte)((pn[0] ^ pn[1]) % byte.MaxValue);
                ct++;
            }
            return ct;
        }

        /// <summary>
        /// 计算字符串哈希并写入字节序列
        /// </summary>
        /// <param name="charBuffer">要计算的字符串首地址</param>
        /// <param name="charCount">字符串的字符数量</param>
        /// <param name="buffer">要将哈希值写入的位置</param>
        /// <param name="count"><paramref name="buffer"/>可用的字节数量</param>
        /// <returns>成功写入到指定位置哈希值的字节数量；因为是循环计算写入，该值可能大于写入区域的字节大小</returns>
        /// <exception cref="ArgumentNullException">指针是null</exception>
        public static int StringHashCodeToBuffer(CPtr<char> charBuffer, int charCount, CPtr<byte> buffer, int count)
        {
            if (charBuffer.IsEmpty || buffer.IsEmpty) throw new ArgumentNullException();
            return f_stringHashToBuffer(charBuffer, charCount, buffer, count);
        }

        /// <summary>
        /// 计算字符串哈希并写入字节序列
        /// </summary>
        /// <param name="charBuffer">要计算的字符串首地址，该字符串是末尾必须存在'\0'的字符串</param>
        /// <param name="buffer">要将哈希值写入的位置</param>
        /// <param name="count"><paramref name="buffer"/>可用的字节数量</param>
        /// <returns>成功写入到指定位置哈希值的字节数量；因为是循环计算写入，该值可能大于写入区域的字节大小</returns>
        /// <exception cref="ArgumentNullException">指针是null</exception>
        public static int CStringHashCodeToBuffer(CPtr<char> charBuffer, CPtr<byte> buffer, int count)
        {
            if (charBuffer.IsEmpty || buffer.IsEmpty) throw new ArgumentNullException();
            return f_stringHashToBuffer(charBuffer, buffer, count);
        }

        /// <summary>
        /// 计算字符串哈希并写入字节数组
        /// </summary>
        /// <param name="str">要计算的字符串</param>
        /// <param name="startIndex">要从中开始计算的字符串索引</param>
        /// <param name="strCount">要计算的字符串的字符数量</param>
        /// <param name="buffer">要将哈希值写入的数组</param>
        /// <param name="offset">字节数组的起始位置</param>
        /// <param name="count">字节数组要用到的字节数量</param>
        /// <returns>成功写入到数组指定位置哈希值的字节数量；因为是循环计算写入，该值可能大于写入区域的字节大小</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定参数超出范围</exception>
        public static int StringHashCodeToBuffer(string str, int startIndex, int strCount, byte[] buffer, int offset, int count)
        {
            if (str is null || buffer is null) throw new ArgumentNullException();
            if (startIndex < 0 || strCount < 0 || offset < 0 || count < 0 || startIndex + strCount > str.Length || offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (strCount == 0 || count == 0) return 0;

            fixed (char* cp = str)
            {
                fixed (byte* buf = buffer)
                {
                    return f_stringHashToBuffer(cp + startIndex, strCount, buf + offset, count);
                }
            }
        }

        /// <summary>
        /// 计算字符串哈希并写入字节数组
        /// </summary>
        /// <param name="str">要计算的字符串</param>
        /// <param name="buffer">要将哈希值写入的数组</param>
        /// <returns>成功写入到数组哈希值的字节数量；因为是循环计算写入，该值可能大于写入区域的字节大小</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int StringHashCodeToBuffer(string str, byte[] buffer)
        {
            if (str is null || buffer is null) throw new ArgumentNullException();
            int strCount = str.Length, count = buffer.Length;
            if (strCount == 0 || count == 0) return 0;

            fixed (char* cp = str)
            {
                fixed (byte* buf = buffer)
                {
                    return f_stringHashToBuffer(cp, strCount, buf, count);
                }
            }
        }

    }

}