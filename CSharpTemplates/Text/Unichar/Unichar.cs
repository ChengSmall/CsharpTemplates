using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using Cheng.Texts;
using System.Collections;
using System.Collections.Generic;
using Cheng.Memorys;

namespace Cheng.DataStructure.Texts
{

    /// <summary>
    /// 可表示为代理项字符的32位字符值
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct Unichar : IEquatable<Unichar>, IComparable<Unichar>, IComparable
    {

        #region 封装

        private static void toPair(int codePoint, out char low, out char high)
        {
            codePoint = codePoint - 0x10000;
            // 取偏移量的高10位
            high = (char)((((uint)codePoint) >> 10) + 0xD800);
            // 取偏移量的低10位
            low = (char)((((uint)codePoint) & 0x3FF) + 0xDC00);
        }

        private static int toCode(char high, char low)
        {
            // 核心转换公式
            return ((high - 0xD800) * 0x0400) + (low - 0xDC00) + 0x10000;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化一个字符值
        /// </summary>
        /// <param name="value"></param>
        public Unichar(char value)
        {
            low = '\0';
            high = value;
        }

        /// <summary>
        /// 使用高低位代理项码位初始化字符
        /// </summary>
        /// <param name="high">高代理项码位（范围从 U+D800 到 U+DBFF）</param>
        /// <param name="low">低代理项码位（范围从 U+DC00 到 U+DFFF）</param>
        public Unichar(char high, char low)
        {
            this.low = low; this.high = high;
        }

        /// <summary>
        /// 使用unicode码初始化字符
        /// </summary>
        /// <param name="unicode">如果值属于一个代理对字符码位，则初始化为代理对字符；否则将前16位作为high，后16位作为low值初始化</param>
        public Unichar(int unicode)
        {
            if((unicode >= 0x10000 && unicode <= 0x10FFFF))
            {
                toPair(unicode, out this.low, out this.high);
            }
            else
            {
                high = (char)(((uint)unicode) & 0xFFFF);
                low = (char)((((uint)unicode) >> 16) & 0xFFFF);
            }
        }

        /// <summary>
        /// 使用指定字符串初始化代理项字符
        /// </summary>
        /// <param name="ptr">指向字符串的指针</param>
        /// <param name="count">该字符串的<see cref="char"/>字符可用量</param>
        /// <exception cref="ArgumentNullException">指向字符串的指针是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定的字符串长度不够</exception>
        public Unichar(CPtr<char> ptr, int count)
        {
            if(ptr.p_ptr == null) throw new ArgumentNullException();
            if (count <= 0) throw new ArgumentOutOfRangeException();

            var hi = ptr[0];
            if(count > 1)
            {
                var low = ptr[1];
                if (char.IsSurrogatePair(hi, low))
                {
                    //属于代理项
                    this.high = hi;
                    this.low = low;
                    return;
                }
            }
            high = hi;
            this.low = default;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 高代理项字符码位；如果字符不属于代理项，该参数将代表一个常规字符值
        /// </summary>
        public readonly char high;

        /// <summary>
        /// 低代理项字符码位；如果字符不属于代理项，该参数理应是 '\0'
        /// </summary>
        public readonly char low;

        #endregion

        #region 功能

        /// <summary>
        /// 获取代理对Unicode字符码
        /// </summary>
        /// <returns>表示一个UTF32字符码，如果该字符不是代理对字符，返回-1</returns>
        public int ToUTF32()
        {
            if(char.IsSurrogatePair(high, low)) return toCode(high, low);
            return -1;
        }

        /// <summary>
        /// 视为一个普通字符获取字符值
        /// </summary>
        public char Value
        {
            get => high;
        }

        /// <summary>
        /// 判断该字符是否是一个代理对字符
        /// </summary>
        /// <returns>如果是一个代理对字符，返回true；否则返回false</returns>
        public bool IsSurrogatePair()
        {
            return char.IsSurrogatePair(high, low);
        }

        /// <summary>
        /// 获取字符值
        /// </summary>
        /// <returns>如果该字符是一个代理对字符，则返回UTF32字符码点；如果是一个普通字符，则返回<see cref="char"/>字符值</returns>
        public int ToChar()
        {
            if (char.IsSurrogatePair(high, low)) return toCode(high, low);
            return (int)((uint)high | (((uint)low) << 16));
        }

        /// <summary>
        /// 判断一个值是否属于代理对字符码
        /// </summary>
        /// <param name="utf32">要判断的值</param>
        /// <returns>如果<paramref name="utf32"/>属于代理对字符，返回true；否则返回false</returns>
        public static bool IsSurrogatePairByUTF32(int utf32)
        {
            return (utf32 >= 0x10000 && utf32 <= 0x10FFFF);
        }

        #region 写入字符串

        /// <summary>
        /// 将该字符添加到指定的字符串中
        /// </summary>
        /// <param name="str">指向要添加的字符串位置的字符指针</param>
        /// <param name="size">从<paramref name="str"/>开始还有多少可用的字符数</param>
        /// <exception cref="ArgumentOutOfRangeException">可用字符数不够</exception>
        public void AppendString(char* str, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException();
            if(char.IsSurrogatePair(high, low))
            {
                if(size < 2) throw new ArgumentOutOfRangeException();
                str[0] = high;
                str[1] = low;
                return;
            }
            str[0] = high;
        }

        /// <summary>
        /// 将该字符添加到指定的字符串中
        /// </summary>
        /// <param name="str">指向要添加的字符串位置的字符指针</param>
        /// <param name="size">从<paramref name="str"/>开始还有多少可用的字符数</param>
        /// <exception cref="ArgumentOutOfRangeException">可用字符数不够</exception>
        public void AppendString(CPtr<char> str, int size)
        {
            AppendString(str.p_ptr, size);
        }

        /// <summary>
        /// 将该字符添加到指定的字符串缓冲区
        /// </summary>
        /// <param name="append">待添加的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void AppendString(StringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();

            if (char.IsSurrogatePair(high, low))
            {
                append.Append(high);
                append.Append(low);
                return;
            }
            append.Append(high);
        }

        /// <summary>
        /// 将该字符添加到指定的字符串缓冲区
        /// </summary>
        /// <param name="append">待添加的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void AppendString(CMStringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();
            
            if (char.IsSurrogatePair(high, low))
            {
                append.Append(high);
                append.Append(low);
                return;
            }
            append.Append(high);
        }

        /// <summary>
        /// 将该字符添加到指定的字符数组
        /// </summary>
        /// <param name="buffer">待添加的字符数组</param>
        /// <param name="index">要从此索引开始添加</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引小于0或指定参数可用范围不足以写入字符</exception>
        public void Append(char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index >= buffer.Length) throw new ArgumentOutOfRangeException();
            fixed (char* cp = buffer)
            {
                AppendString(cp + index, buffer.Length - index);
            }
        }

        #endregion

        #region 从字符串读取

        /// <summary>
        /// 从指定字符串中读取一个字符
        /// </summary>
        /// <param name="str">要读取的字符串</param>
        /// <param name="startIndex">要从中读取的第一个索引</param>
        /// <returns>读取的字符</returns>
        public static Unichar GetUnichar(string str, int startIndex)
        {
            if (str is null) throw new ArgumentNullException();
            fixed (char* cp = str)
            {
                return new Unichar(new CPtr<char>((cp + startIndex)), str.Length - startIndex);
            }
        }

        /// <summary>
        /// 从指定字符串起始位置中读取一个字符
        /// </summary>
        /// <param name="str">要从中读取的字符</param>
        /// <returns></returns>
        public static Unichar GetUnichar(string str)
        {
            if (str is null) throw new ArgumentNullException();
            fixed (char* cp = str)
            {
                return new Unichar(new CPtr<char>(cp), str.Length);
            }
        }

        #endregion

        #endregion

        #region 运算符

        public static bool operator ==(Unichar x, Unichar y)
        {
            return x.low == y.low && x.high == y.high;
        }

        public static bool operator !=(Unichar x, Unichar y)
        {
            return x.low != y.low || x.high != y.high;
        }

        public static bool operator <(Unichar x, Unichar y)
        {
            return x.ToChar() < y.ToChar();
        }

        public static bool operator >(Unichar x, Unichar y)
        {
            return x.ToChar() > y.ToChar();
        }

        public static bool operator <=(Unichar x, Unichar y)
        {
            return x.ToChar() < y.ToChar();
        }

        public static bool operator >=(Unichar x, Unichar y)
        {
            return x.ToChar() > y.ToChar();
        }

        #endregion

        #region 类型转换

        public static implicit operator Unichar(char c)
        {
            return new Unichar(c);
        }

        /// <summary>
        /// 将32位整型值作为unicode码转化为字符
        /// </summary>
        /// <param name="utf32"></param>
        public static implicit operator Unichar(int utf32)
        {
            return new Unichar(utf32);
        }

        public static implicit operator Unichar(uint utf32)
        {
            return new Unichar((int)utf32);
        }

        /// <summary>
        /// 强制转换为一个普通字符
        /// </summary>
        /// <param name="c"></param>
        public static explicit operator char(Unichar c)
        {
            return c.Value;
        }

        /// <summary>
        /// 转换为一个32为整数值，代表unicode字符码
        /// </summary>
        /// <param name="c"></param>
        public static explicit operator int(Unichar c)
        {
            return c.ToChar();
        }

        /// <summary>
        /// 转换为一个32为整数值，代表unicode字符码
        /// </summary>
        /// <param name="c"></param>
        public static explicit operator uint(Unichar c)
        {
            return (uint)c.ToChar();
        }

        #endregion

        #region 派生

        /// <summary>
        /// 将字符值转化为等效的字符串
        /// </summary>
        /// <returns></returns>
        public override unsafe string ToString()
        {
            if (this.IsSurrogatePair())
            {
                return char.ConvertFromUtf32(toCode(high, low));
            }
            return Value.ToString();
        }

        public bool Equals(Unichar other)
        {
            return this.low == other.low && this.high == other.high;
        }

        public override bool Equals(object obj)
        {
            if (obj is Unichar uc) return this == uc;
            if (obj is char c) return this == new Unichar(c);
            return false;
        }

        public override int GetHashCode()
        {
            return ToChar();
        }

        public int CompareTo(Unichar other)
        {
            return ToChar().CompareTo(other.ToChar());
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Unichar uc) return CompareTo(uc);
            if (obj is char c) return CompareTo(new Unichar(c));
            throw new ArgumentException();
        }

        #endregion

    }

}