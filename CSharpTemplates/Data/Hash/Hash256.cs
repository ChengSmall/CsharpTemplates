using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Texts;
using Cheng.Algorithm.HashCodes;
using Cheng.IO;

namespace Cheng.DataStructure.Hashs
{

    /// <summary>
    /// 一个256位的哈希结构
    /// </summary>
    [Serializable]
    public unsafe struct Hash256 : IEquatable<Hash256>, IComparable<Hash256>, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 初始化Hash256
        /// </summary>
        /// <param name="s1">第一个64位值</param>
        /// <param name="s2">第二个64位值</param>
        /// <param name="s3">第三个64位值</param>
        /// <param name="s4">第四个64位值</param>
        public Hash256(ulong s1, ulong s2, ulong s3, ulong s4)
        {
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 第一个64位值
        /// </summary>
        public ulong s1;

        /// <summary>
        /// 第二个64位值
        /// </summary>
        public ulong s2;

        /// <summary>
        /// 第三个64位值
        /// </summary>
        public ulong s3;

        /// <summary>
        /// 第四个64位值
        /// </summary>
        public ulong s4;

        /// <summary>
        /// 该结构的字节大小
        /// </summary>
        public const int Size = sizeof(ulong) * 4;

        #endregion

        #region 功能

        #region 字符串转化

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <param name="link">每个64位值之间的分隔符，默认分隔符是 - </param>
        /// <param name="strBuffer">要转化到的字符串缓冲区，该指针指向的位置至少有68个字符大小的可用空间</param>
        public void ValueToString(bool upper, char link, char* strBuffer)
        {
            int index = 0;
            s1.ValueToFixedX16Text(upper, strBuffer);
            index += 16;
            strBuffer[index] = link;
            index++;
            s2.ValueToFixedX16Text(upper, strBuffer + index);
            index += 16;
            strBuffer[index] = link;
            index++;
            s3.ValueToFixedX16Text(upper, strBuffer + index);
            index += 16;
            strBuffer[index] = link;
            index++;
            s4.ValueToFixedX16Text(upper, strBuffer + index);
        }

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <param name="strBuffer">要转化到的字符串缓冲区，该指针指向的位置至少有68个字符大小的可用空间</param>
        public void ValueToString(bool upper, char* strBuffer)
        {
            ValueToString(upper, '-', strBuffer);
        }

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <param name="link">每个64位值之间的分隔符，默认分隔符是 - </param>
        /// <param name="buffer">要将字符串写入到的字符缓冲区</param>
        /// <param name="index">字符缓冲区的起始位置</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定范围不足以容纳Hash256文本</exception>
        public void ValueToString(bool upper, char link, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 68 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* cp = buffer)
            {
                ValueToString(upper, link, cp + index);
            }
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="buffer68">指向表示Hash256文本的指针，至少要有68个可用字符</param>
        /// <param name="hash">转化后的实例</param>
        /// <returns>是否成功转化</returns>
        public static bool ToHash256(char* buffer68, out Hash256 hash)
        {
            hash = default;

            if (!TextManipulation.X16ToValue(buffer68, 16, out hash.s1)) return false;

            if (!TextManipulation.X16ToValue(buffer68 + (16 + 1), 16, out hash.s2)) return false;

            if (!TextManipulation.X16ToValue(buffer68 + (16 * 2 + 2), 16, out hash.s3)) return false;

            if (!TextManipulation.X16ToValue(buffer68 + (16 * 3 + 3), 16, out hash.s4)) return false;

            return true;
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="buffer68">指向表示Hash256文本的指针，至少要有68个可用字符</param>
        /// <returns>转化后的实例</returns>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        public static Hash256 ToHash256(char* buffer68)
        {
            Hash256 h;
            if(!TextManipulation.X16ToValue(buffer68, 16, out h.s1)) throw new NotImplementedException();

            if (!TextManipulation.X16ToValue(buffer68 + (16 + 1), 16, out h.s2)) throw new NotImplementedException();

            if(!TextManipulation.X16ToValue(buffer68 + (16 * 2 + 2), 16, out h.s3)) throw new NotImplementedException();

            if(!TextManipulation.X16ToValue(buffer68 + (16 * 3 + 3), 16, out h.s4)) throw new NotImplementedException();
            return h;
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="buffer">表示Hash256文本的字符缓冲区</param>
        /// <param name="index">从字符缓冲区读取文本的起始位置</param>
        /// <returns>转化后的Hash256实例</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度不足</exception>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        public static Hash256 ToHash256(char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index + 68 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = buffer)
            {
                return ToHash256(p + index);
            }
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="text">表示Hash256文本</param>
        /// <param name="index">从字符文本读取的起始位置</param>
        /// <returns>转化后的Hash256实例</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">文本长度不足</exception>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        public static Hash256 ToHash256(string text, int index)
        {
            if (text is null) throw new ArgumentNullException();
            if (index + 68 > text.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = text)
            {
                return ToHash256(p + index);
            }
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="buffer">表示Hash256文本的字符缓冲区</param>
        /// <param name="index">从字符缓冲区读取文本的起始位置</param>
        /// <param name="hash">转化后的值</param>
        /// <returns>是否成功转化</returns>
        public static bool ToHash256(char[] buffer, int index, out Hash256 hash)
        {
            hash = default;
            if (buffer is null) return false;
            if (index + 68 > buffer.Length) return false;

            fixed (char* p = buffer)
            {
                return ToHash256(p + index, out hash);
            }
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="text">表示Hash256文本</param>
        /// <param name="index">从字符文本读取的起始位置</param>
        /// <param name="hash">转化后的值</param>
        /// <returns>是否成功转化</returns>
        public static bool ToHash256(string text, int index, out Hash256 hash)
        {
            hash = default;
            if (text is null) return false;
            if (index + 68 > text.Length) return false;

            fixed (char* p = text)
            {
                return ToHash256(p + index, out hash);
            }
        }

        /// <summary>
        /// 将表示Hash256的文本转化为Hash256
        /// </summary>
        /// <param name="text">表示Hash256文本</param>
        /// <returns>转化后的Hash256实例</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">文本长度不足</exception>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        public static Hash256 ToHash256(string text)
        {
            if (text is null) throw new ArgumentNullException();
            if (68 > text.Length) throw new ArgumentOutOfRangeException();

            fixed (char* p = text)
            {
                return ToHash256(p);
            }
        }

        #endregion

        #region 字节数组转换

        /// <summary>
        /// 将字节数组转化为<see cref="Hash256"/>
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">字节数组的起始位置</param>
        /// <returns>转化后的实例</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定范围的长度不足32字节</exception>
        public static Hash256 BytesToHash256(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 32 > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            fixed (byte* bp = buffer)
            {
                return BytesToHash256(bp + offset);
            }
        }

        /// <summary>
        /// 将字节序列转化为<see cref="Hash256"/>
        /// </summary>
        /// <param name="buffer">字节序列首地址，请保证地址指向的内存拥有至少32个字节</param>
        /// <returns>转化后的实例</returns>
        public static Hash256 BytesToHash256(byte* buffer)
        {
            return new Hash256(IOoperations.OrderToUInt64(new IntPtr(buffer)),
                IOoperations.OrderToUInt64(new IntPtr(buffer + 8)),
                IOoperations.OrderToUInt64(new IntPtr(buffer + 16)),
                IOoperations.OrderToUInt64(new IntPtr(buffer + 24)));
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static bool operator ==(Hash256 h1, Hash256 h2)
        {
            return h1.s1 == h2.s1 && h1.s2 == h2.s2 && h1.s3 == h2.s3 && h1.s4 == h2.s4;
        }

        /// <summary>
        /// 判断不相等
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static bool operator !=(Hash256 h1, Hash256 h2)
        {
            return h1.s1 != h2.s1 || h1.s2 != h2.s2 || h1.s3 != h2.s3 || h1.s4 != h2.s4;
        }

        #endregion

        #region 派生接口

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char* cp = stackalloc char[68];
            ValueToString(false, cp);
            return new string(cp, 0, 68);
        }

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <returns></returns>
        public string ToString(bool upper)
        {
            char* cp = stackalloc char[68];
            ValueToString(upper, cp);
            return new string(cp, 0, 68);
        }

        /// <summary>
        /// 将Hash256转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <param name="link">每个64整数值之间的连接字符</param>
        /// <returns></returns>
        public string ToString(bool upper, char link)
        {
            char* cp = stackalloc char[68];
            ValueToString(upper, link, cp);
            return new string(cp, 0, 68);
        }

        public override int GetHashCode()
        {
            return (s1 ^ s2 ^ s3 ^ s4).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is Hash256)
            {
                return this == (Hash256)obj;
            }
            return false;
        }

        /// <summary>
        /// 与另一个对象比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Hash256 other)
        {
            return this.s1 == other.s1 && this.s2 == other.s2 && this.s3 == other.s3 && this.s4 == other.s4;
        }

        public long GetHashCode64()
        {
            return (long)(s1 ^ s2 ^ s3 ^ s4);
        }

        /// <summary>
        /// 默认的比较方法，从s1到s4依次比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Hash256 other)
        {
            var re = s1.CompareTo(other.s1);
            if (re != 0) return re;
            re = s2.CompareTo(other.s2);
            if (re != 0) return re;
            re = s3.CompareTo(other.s3);
            if (re != 0) return re;
            return s4.CompareTo(other.s4);
        }

        #endregion

        #endregion

    }

}
