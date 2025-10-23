using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Memorys
{
    

    static unsafe partial class MemoryOperation
    {

        #region 字符操作

        /// <summary>
        /// 将字符串的字母转化到小写
        /// </summary>
        /// <param name="originCharptr">原字符串地址</param>
        /// <param name="toCharptr">转化后写入的位置</param>
        /// <param name="length">字符串转化的字符数</param>
        public static void ToLopper(char* originCharptr, char* toCharptr, int length)
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
                    //toCharptr[i] = c;
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

        #endregion

    }


}
