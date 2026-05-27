using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Memorys
{


    static unsafe partial class MemoryOperation
    {

        #region 字符操作

        [Obsolete("", true)]
        public static void ToLopper(char* originCharptr, char* toCharptr, int length)
        {
            ToLower(originCharptr, toCharptr, length);
        }

        /// <summary>
        /// 将字符串的字母转化到小写
        /// </summary>
        /// <param name="originCharptr">原字符串地址</param>
        /// <param name="toCharptr">转化后写入的位置</param>
        /// <param name="length">字符串转化的字符数</param>
        public static void ToLower(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const byte cbit = 0b00100000;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];
                if (c >= 'A' && c <= 'Z')
                {
                    c |= (char)cbit;
                }
                toCharptr[i] = c;
            }
        }

        /// <summary>
        /// 将字符串的字母转化到大写
        /// </summary>
        /// <param name="originCharptr">原字符串地址</param>
        /// <param name="toCharptr">转化后写入的位置</param>
        /// <param name="length">字符串转化的字符数</param>
        public static void ToUpper(char* originCharptr, char* toCharptr, int length)
        {
            //小写1大写0
            const ushort cbit = 0b11111111_11011111;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = originCharptr[i];
                if (c >= 'a' && c <= 'z')
                {
                    c &= (char)cbit;
                }
                toCharptr[i] = c;
            }
        }

        /// <summary>
        /// 将字符转换为小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ToLower(this char value)
        {
            const char cb = (char)0b00000000_00100000;
            if (value >= 'A' && value <= 'Z')
            {
                value |= cb;
            }
            return value;
        }

        /// <summary>
        /// 将字符转换为大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char ToUpper(this char value)
        {
            const char cb = (char)0b11111111_11011111;
            if (value >= 'a' && value <= 'z')
            {
                return (char)(value & cb);
            }
            return value;
        }

        /// <summary>
        /// 将字符串转换为小写
        /// </summary>
        /// <remarks>如果发现存在字符是大小字母，则转换为小写字母，否则不进行操作</remarks>
        /// <param name="buffer">要转换的字符串首地址</param>
        /// <param name="length">字符串长度</param>
        public static void ToLower(char* buffer, int length)
        {
            //小写1大写0
            const ushort cbit = 0b00000000_00100000;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = buffer[i];

                if (c >= 'A' && c <= 'Z')
                {
                    buffer[i] = (char)(c | (char)cbit);
                }
            }
        }

        /// <summary>
        /// 将字符串转换为大写
        /// </summary>
        /// <remarks>如果发现存在字符是大小字母，则转换为小写字母，否则不进行操作</remarks>
        /// <param name="buffer">要转换的字符串首地址</param>
        /// <param name="length">字符串长度</param>
        public static void ToUpper(char* buffer, int length)
        {
            const ushort cbit = 0b11111111_11011111;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = buffer[i];

                if (c >= 'a' && c <= 'z')
                {
                    buffer[i] = (char)(c & cbit);
                }
            }
        }

        /// <summary>
        /// 将字符串转换为小写
        /// </summary>
        /// <remarks>如果发现存在字符是大小字母，则转换为小写字母，否则不进行操作</remarks>
        /// <param name="buffer">要转换的字符串首地址</param>
        /// <param name="length">字符串长度</param>
        public static void ToLower(this CPtr<char> buffer, int length)
        {
            ToLower(buffer.p_ptr, length);
        }

        /// <summary>
        /// 将字符串转换为大写
        /// </summary>
        /// <remarks>如果发现存在字符是大小字母，则转换为小写字母，否则不进行操作</remarks>
        /// <param name="buffer">要转换的字符串首地址</param>
        /// <param name="length">字符串长度</param>
        public static void ToUpper(this CPtr<char> buffer, int length)
        {
            ToUpper(buffer.p_ptr, length);
        }

        #endregion

        #region 二进制文本

        /// <summary>
        /// 将值转化为字符串形式的二进制
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <param name="buffer71">要写入的字符串首地址，需要至少71个字符（142字节）大小的可用空间</param>
        public static void ToBinStr(ulong value, char fen, char* buffer71)
        {
            const char c0 = '0';
            const char c1 = '1';

            //char* cp = buffer;
            //fixed (char* cp = carr)
            //{

            int i = 0;
            for (int boi = 0; boi < sizeof(ulong); boi++, i++)
            {
                for (int bi = 0; bi < 8; bi++, i++)
                {
                    int rimov = ((sizeof(ulong) * 8) - 1) - ((boi * 8) + bi);

                    char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
                    buffer71[i] = sc;
                }
                if (sizeof(ulong) - 1 != boi)
                {
                    buffer71[i] = fen;
                }
            }
            //return new string(cp, 0, 8 * sizeof(ulong) + sizeof(ulong));
        }

        /// <summary>
        /// 将值转化为字符串形式的二进制
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <param name="buffer35">要写入的字符串首地址，需要至少35个字符（70字节）大小的可用空间</param>
        public static void ToBinStr(uint value, char fen, char* buffer35)
        {
            const char c0 = '0';
            const char c1 = '1';

            int i = 0;
            for (int boi = 0; boi < sizeof(uint); boi++, i++)
            {
                for (int bi = 0; bi < 8; bi++, i++)
                {
                    int rimov = ((sizeof(uint) * 8) - 1) - ((boi * 8) + bi);

                    char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
                    buffer35[i] = sc;
                }
                if (sizeof(uint) - 1 != boi)
                {
                    buffer35[i] = fen;
                }
            }
        }

        /// <summary>
        /// 将值转化为字符串形式的二进制
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="fen">每个字节之间的分隔符</param>
        /// <param name="buffer17">要写入的字符串首地址，需要至少17个字符（34字节）大小的可用空间</param>
        public static void ToBinStr(ushort value, char fen, char* buffer17)
        {
            const char c0 = '0';
            const char c1 = '1';

            int i = 0;
            for (int boi = 0; boi < sizeof(ushort); boi++, i++)
            {
                for (int bi = 0; bi < 8; bi++, i++)
                {
                    int rimov = ((sizeof(ushort) * 8) - 1) - ((boi * 8) + bi);

                    char sc = ((value >> rimov) & 0b1) == 0b1 ? c1 : c0;
                    buffer17[i] = sc;
                }
                if (sizeof(ushort) - 1 != boi)
                {
                    buffer17[i] = fen;
                }
            }
        }

        /// <summary>
        /// 将字节值转化为字符串形式的二进制
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="buffer8">要写入的字符串首地址，需要至少8个字符（16字节）大小的可用空间</param>
        public static void ToBinStr(byte value, char* buffer8)
        {
            const char c0 = '0';
            const char c1 = '1';
            for (int i = 0; i < 8; i++)
            {
                buffer8[i] = ((value >> (7 - i)) & 0b1) == 0b1 ? c1 : c0;
            }
        }

        #endregion

    }


}
