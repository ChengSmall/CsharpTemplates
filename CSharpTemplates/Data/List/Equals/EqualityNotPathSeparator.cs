using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 忽略路径分隔符的字符比较器
    /// </summary>
    /// <remarks>
    /// <para>在比较两个字符时，如果两个字符都是'/'或'\'其中一个字符，则判定为相等</para>
    /// </remarks>
    public sealed class EqualityNotPathSeparator : EqualityComparerHash64<char>
    {

        /// <summary>
        /// 忽略路径分隔符的字符比较器
        /// </summary>
        public EqualityNotPathSeparator()
        {
        }

        public override bool Equals(char x, char y)
        {
            if (x == y) return true;
            return ((x == '/' || x == '\\') && (y == '/' || y == '\\'));
            //if ((x == '/' || x == '\\') && (y == '/' || y == '\\'))
            //{
            //    return true;
            //}
            //return false;
        }

        public override int GetHashCode(char obj)
        {
            return obj == '/' ? '\\' : obj;
        }

        /// <summary>
        /// 判断两个字符是否相等并且不区分路径分隔符
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// <para>当<paramref name="x"/>和<paramref name="y"/>相等时返回true</para>
        /// <para>当<paramref name="x"/>和<paramref name="y"/>不相等时，如果两方均为<![CDATA[/ 或 \]]>其中之一，则返回true，否则返回false</para>
        /// </returns>
        public static bool EqualChar(char x, char y)
        {
            if (x == y) return true;
            return ((x == '/' || x == '\\') && (y == '/' || y == '\\'));
        }

        /// <summary>
        /// 判断两个字符是否相等并且不区分路径分隔符和忽略字母大小写
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool EqualCaseInsensitiveChar(char x, char y)
        {
            x = x.ToLower();
            y = y.ToLower();
            if (x == y) return true;
            return ((x == '/' || x == '\\') && (y == '/' || y == '\\'));
        }

        public override long GetHashCode64(char value)
        {
            return value == '/' ? '\\' : value;
        }
    }


    /// <summary>
    /// 忽略路径分隔符的字符串比较器
    /// </summary>
    /// <remarks>
    /// <para>在比较字符串时，如果其中两个字符都是'/'或'\'其中一个字符，则判定为相等字符</para>
    /// </remarks>
    public sealed unsafe class EqualityStrNotPathSeparator : EqualityComparerHash64<string>
    {

        /// <summary>
        /// 忽略路径分隔符的字符串比较器
        /// </summary>
        public EqualityStrNotPathSeparator()
        {
            p_ifEndSeparators = false;
            p_case_insensitive = false;
        }

        /// <summary>
        /// 忽略路径分隔符的字符串比较器
        /// </summary>
        /// <param name="endSeparators">
        /// <para>是否将字符串末尾可能存在的分隔符忽略</para>
        /// <para>如果是true，则当检测到字符串末尾有<![CDATA[/ 或 \]]>字符时将其忽略；false则不做忽略</para>
        /// <para>该参数默认为false</para>
        /// </param>
        public EqualityStrNotPathSeparator(bool endSeparators)
        {
            p_ifEndSeparators = endSeparators;
            p_case_insensitive = false;
        }

        /// <summary>
        /// 忽略路径分隔符的字符串比较器
        /// </summary>
        /// <param name="endSeparators">
        /// <para>是否将字符串末尾可能存在的分隔符忽略</para>
        /// <para>如果是true，则当检测到字符串末尾有<![CDATA[/ 或 \]]>字符时将其忽略；false则不做忽略</para>
        /// <para>该参数默认为false</para>
        /// </param>
        /// <param name="caseInsensitive">是否开启大小写不敏感比较；true表示进行字母对比时不区分大小写，false区分大小写；默认为false</param>
        public EqualityStrNotPathSeparator(bool endSeparators, bool caseInsensitive)
        {
            p_ifEndSeparators = endSeparators;
            p_case_insensitive = caseInsensitive;
        }

        private bool p_ifEndSeparators;

        private bool p_case_insensitive;

        /// <summary>
        /// 是否将字符串末尾可能存在的分隔符忽略
        /// </summary>
        /// <value>
        /// <para>如果是true，则当检测到字符串末尾有<![CDATA[/ 或 \]]>字符时将其忽略；false则不做忽略</para>
        /// <para>该参数默认为false</para>
        /// </value>
        public bool EndSeparators
        {
            get => p_ifEndSeparators;
            set => p_ifEndSeparators = value;
        }

        /// <summary>
        /// 是否开启大小写不敏感比较
        /// </summary>
        /// <value>
        /// true表示进行字母对比时不区分大小写，false区分大小写；参数默认为false
        /// </value>
        public bool CaseInsensitive
        {
            get => p_case_insensitive;
            set => p_case_insensitive = value;
        }

        public override bool Equals(string x, string y)
        {
            return EqualPath(x, y, p_ifEndSeparators, p_case_insensitive);
        }

        public override int GetHashCode(string str)
        {
            return GetHashCodeUInt64(str, p_ifEndSeparators, p_case_insensitive).GetHashCode();
        }

        public override long GetHashCode64(string str)
        {
            return (long)GetHashCodeUInt64(str, p_ifEndSeparators, p_case_insensitive);
        }

        public static ulong GetHashCodeUInt64(string str, bool ifEndSeparators, bool caseInsensitive)
        {
            if (str is null) return 0;
            int length = str.Length;
            if (length == 0) return 1;
            char c;
            if (ifEndSeparators)
            {
                c = str[length - 1];
                if (c == '/' || c == '\\') length--;
            }

            const ulong FNV_OFFSET_BASIS = 14695981039346656037UL;
            const ulong FNV_PRIME = 1099511628211UL;
            int i;
            ulong hash = FNV_OFFSET_BASIS;
            if (caseInsensitive)
            {
                for (i = 0; i < length; i++)
                {
                    c = str[i];
                    if (c == '/') c = '\\';
                    c = c.ToLower();
                    hash = (hash ^ c) * FNV_PRIME;
                }
            }
            else
            {
                for (i = 0; i < length; i++)
                {
                    c = str[i];
                    if (c == '/') c = '\\';
                    hash = (hash ^ c) * FNV_PRIME;
                }
            }

            return hash;
        }

        /// <summary>
        /// 忽略路径分隔符的字符串比较函数
        /// </summary>
        /// <param name="x">要进行比较的字符串</param>
        /// <param name="y">要进行比较的字符串</param>
        /// <param name="ifEndSeparators">
        /// <para>是否将字符串末尾可能存在的分隔符忽略</para>
        /// <para>如果是true，则当检测到字符串末尾有<![CDATA[/ 或 \]]>字符时将其忽略；false则不做忽略</para>
        /// </param>
        /// <param name="caseInsensitive">是否开启大小写不敏感比较；true表示进行字母对比时不区分大小写，false区分大小写</param>
        /// <returns>字符串相同返回true，字符串不同返回false；当<paramref name="x"/>和<paramref name="y"/>都属于null时，返回true，而只有一个参数为null时，返回false</returns>
        public static bool EqualPath(string x, string y, bool ifEndSeparators, bool caseInsensitive)
        {
            if ((object)x == (object)y)
            {
                return true;
            }
            if (x is null || y is null)
            {
                return false;
            }

            int xlen = x.Length;
            int ylen = y.Length;
            int length = xlen;
            if (xlen != ylen)
            {
                if (ifEndSeparators)
                {
                    if (xlen + 1 == ylen)
                    {
                        //y 比 x 多 1
                        if (y[xlen] == '\\' || y[xlen] == '/')
                        {
                            //省略最后一个分隔符
                            length = xlen;
                        }
                        else return false; //不属于分隔符
                    }
                    else if (xlen == ylen + 1)
                    {
                        //x 比 y 多 1
                        if (x[ylen] == '\\' || x[ylen] == '/')
                        {
                            //省略最后一个分隔符
                            length = ylen;
                        }
                        else return false; //不属于分隔符
                    }
                }

                //长度不等
                return false;
            }
            int i;
            fixed (char* xp = x, yp = y)
            {
                if (caseInsensitive)
                {
                    for (i = 0; i < length; i++)
                    {
                        if (!EqualityNotPathSeparator.EqualCaseInsensitiveChar(xp[i], yp[i])) return false;
                    }
                }
                else
                {
                    for (i = 0; i < length; i++)
                    {
                        if (!EqualityNotPathSeparator.EqualChar(xp[i], yp[i])) return false;
                    }
                }

            }
            return true;
        }

    }

}
