using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;

using Cheng.Texts;
using Cheng.Algorithm.HashCodes;
using Cheng.IO;
using Cheng.Memorys;

namespace Cheng.DataStructure.Hashs
{

    /// <summary>
    /// 128位哈希值
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Hash128 : IEquatable<Hash128>, IComparable<Hash128>, IHashCode64
    {

        #region 初始化

        /// <summary>
        /// 初始化哈希值
        /// </summary>
        /// <param name="p1">第一个32位参数</param>
        /// <param name="p2">第二个32位参数</param>
        /// <param name="p3">第三个32位参数</param>
        /// <param name="p4">第四个32位参数</param>
        public Hash128(uint p1, uint p2, uint p3, uint p4)
        {
            this.p1 = p1; this.p2 = p2; this.p3 = p3; this.p4 = p4;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 第一个32位参数
        /// </summary>
        public uint p1;

        /// <summary>
        /// 第二个32位参数
        /// </summary>
        public uint p2;

        /// <summary>
        /// 第三个32位参数
        /// </summary>
        public uint p3;

        /// <summary>
        /// 第四个32位参数
        /// </summary>
        public uint p4;

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 该结构的字节大小
        /// </summary>
        public const int Size = sizeof(uint) * 4;

        #endregion

        #region 数组转化

        /// <summary>
        /// 将<see cref="Hash128"/>转化到字节序列
        /// </summary>
        /// <param name="buffer">要转化到的字节序列首地址，需保证该指针指向的位置至少存在16字节可用内存</param>
        public void ToBytes(byte* buffer)
        {
            p1.OrderToBytes(buffer);
            p2.OrderToBytes(buffer + (4));
            p3.OrderToBytes(buffer + (4 * 2));
            p4.OrderToBytes(buffer + (4 * 3));
        }

        /// <summary>
        /// 将<see cref="Hash128"/>转化到字节序列
        /// </summary>
        /// <param name="buffer">要转化到的字节序列首地址，需保证该指针指向的位置至少存在16字节可用内存</param>
        public void ToBytes(CPtr<byte> buffer)
        {
            ToBytes(buffer.p_ptr);
        }

        /// <summary>
        /// 将<see cref="Hash128"/>转化到字节数组
        /// </summary>
        /// <param name="buffer">要写入的数组</param>
        /// <param name="offset">从指定偏移索引开始写入</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public void ToBytes(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 16 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* bptr = buffer)
            {
                ToBytes(bptr + offset);
            }
        }

        /// <summary>
        /// 将<see cref="Hash128"/>转化到字节数组
        /// </summary>
        /// <returns>转化后的字节数组</returns>
        public byte[] ToBytes()
        {
            byte[] buf = new byte[16];
            ToBytes(buf, 0);
            return buf;
        }

        /// <summary>
        /// 将字节序列转化为<see cref="Hash128"/>
        /// </summary>
        /// <param name="buffer">要读取的字节序列内容，需保证该指针指向的位置至少存在16字节可用内存</param>
        /// <returns>转化后的hash值</returns>
        public static Hash128 BytesToHash(CPtr<byte> buffer)
        {
            Hash128 re;
            re.p1 = IOoperations.OrderToUInt32(buffer);
            re.p2 = IOoperations.OrderToUInt32(buffer + (4));
            re.p3 = IOoperations.OrderToUInt32(buffer + (4 * 2));
            re.p4 = IOoperations.OrderToUInt32(buffer + (4 * 3));
            return re;
        }

        /// <summary>
        /// 将字节数组转化为<see cref="Hash128"/>
        /// </summary>
        /// <param name="buffer">要从中读取的字节数组</param>
        /// <param name="offset">指定要读取的起始偏移</param>
        /// <returns>转化后的hash值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static Hash128 BytesToHash(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 16 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* bptr = buffer)
            {
                return BytesToHash(bptr);
            }
        }

        /// <summary>
        /// 将字节数组转化为<see cref="Hash128"/>
        /// </summary>
        /// <param name="buffer">要从中读取的字节数组</param>
        /// <returns>转化后的hash值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static Hash128 BytesToHash(byte[] buffer)
        {
            return BytesToHash(buffer, 0);
        }

        #endregion

        #region 字符串转化

        /// <summary>
        /// Hash值默认的连接字符
        /// </summary>
        public const char DefaultLinkChar = '-';

        /// <summary>
        /// 将当前Hash128值转化为字符串
        /// </summary>
        /// <param name="upper">十六进制值的字母是否大写</param>
        /// <param name="link">32位值之间的连接符</param>
        /// <param name="strBuffer">指向要写入到的内存的指针，内存需要至少有35个字符大小的字符可用空间</param>
        public void ValueToString(bool upper, char link, CPtr<char> strBuffer)
        {
            int index = 0;
            p1.ValueToFixedX16Text(upper, strBuffer.p_ptr);
            index += 8;
            strBuffer[index] = link;
            index++;
            p2.ValueToFixedX16Text(upper, strBuffer.p_ptr + index);
            index += 8;
            strBuffer[index] = link;
            index++;
            p3.ValueToFixedX16Text(upper, strBuffer.p_ptr + index);
            index += 8;
            strBuffer[index] = link;
            index++;
            p4.ValueToFixedX16Text(upper, strBuffer.p_ptr + index);
        }

        /// <summary>
        /// 将当前Hash128值转化为字符串
        /// </summary>
        /// <param name="upper">十六进制值的字母是否大写</param>
        /// <param name="strBuffer">指向要写入到的内存的指针，内存需要至少有35个字符大小的字符可用空间</param>
        public void ValueToString(bool upper, CPtr<char> strBuffer)
        {
            ValueToString(upper, DefaultLinkChar, strBuffer);
        }

        /// <summary>
        /// 将当前Hash128值转化为字符串
        /// </summary>
        /// <param name="upper">十六进制值的字母是否大写</param>
        /// <param name="link">32位值之间的连接符</param>
        /// <param name="buffer">要写入的字符数组</param>
        /// <param name="index">字符数组要写入的起始位置</param>
        public void ValueToString(bool upper, char link, char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + (32 + 4) > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* cp = buffer)
            {
                ValueToString(upper, link, cp + index);
            }
        }

        /// <summary>
        /// 将表示Hash128的文本转化为Hash128
        /// </summary>
        /// <param name="buffer35">指向表示Hash128文本的指针，至少要有35个可用字符</param>
        /// <param name="hash">转化后的Hash值</param>
        /// <returns>是否成功转化</returns>
        public static bool ToHash128(CPtr<char> buffer35, out Hash128 hash)
        {
            hash = default;

            if (!TextManipulation.X16ToValue(buffer35, 8, out hash.p1)) return false;

            if (!TextManipulation.X16ToValue(buffer35 + (8 + 1), 8, out hash.p2)) return false;

            if (!TextManipulation.X16ToValue(buffer35 + (8 * 2 + 2), 8, out hash.p3)) return false;

            if (!TextManipulation.X16ToValue(buffer35 + (8 * 3 + 3), 8, out hash.p4)) return false;

            return true;
        }

        /// <summary>
        /// 将表示Hash128的文本转化为Hash128
        /// </summary>
        /// <param name="buffer35">指向表示Hash128文本的指针，至少要有35个可用字符</param>
        /// <returns>转化后的Hash值</returns>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        public static Hash128 ToHash128(CPtr<char> buffer35)
        {
            Hash128 re;
            if(ToHash128(buffer35, out re))
            {
                return re;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 将表示Hash128的文本转化为Hash128
        /// </summary>
        /// <param name="buffer">表示Hash128文本的字符数组</param>
        /// <param name="index">从字符数组读取的起始位置</param>
        /// <returns>转化后的Hash值</returns>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static Hash128 ToHash128(char[] buffer, int index)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || index + 35 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (char* cp = buffer)
            {
                return ToHash128(cp);
            }
        }

        /// <summary>
        /// 将表示Hash128的文本转化为Hash128
        /// </summary>
        /// <param name="str">表示Hash128文本的字符串</param>
        /// <param name="startIndex">从字符串读取的起始位置</param>
        /// <returns>转化后的Hash值</returns>
        /// <exception cref="NotImplementedException">文本不是Hash256格式</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static Hash128 ToHash128(string str, int startIndex)
        {
            if (str is null) throw new ArgumentNullException();
            if (startIndex < 0 || startIndex + 35 > str.Length) throw new ArgumentOutOfRangeException();

            fixed (char* cp = str)
            {
                return ToHash128(cp);
            }
        }

        #endregion

        #region 类型转换

        #endregion

        #region 运算符

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Hash128 left, Hash128 right)
        {
            return  left.p1 == right.p1 && left.p2 == right.p2 &&
                    left.p3 == right.p3 && left.p4 == right.p4;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Hash128 left, Hash128 right)
        {
            return left.p1 != right.p1 || left.p2 != right.p2 ||
                    left.p3 != right.p3 || left.p4 != right.p4;
        }

        #endregion

        #region 派生

        public bool Equals(Hash128 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        public long GetHashCode64()
        {
            return (long)((((ulong)p1) | (((ulong)p2) << 32)) ^ (((ulong)p3) | (((ulong)p4) << 32)));
        }

        public override bool Equals(object obj)
        {
            if (obj is Hash128 other) return this == other; return false;
        }

        /// <summary>
        /// 返回Hash128格式的字符串
        /// </summary>
        /// <returns>Hash128格式的字符串</returns>
        public override string ToString()
        {
            return ToString(true, DefaultLinkChar);
        }

        /// <summary>
        /// 将Hash128转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <returns></returns>
        public string ToString(bool upper)
        {
            return ToString(upper, DefaultLinkChar);
        }

        /// <summary>
        /// 将Hash128转化为字符串
        /// </summary>
        /// <param name="upper">字母是否大写</param>
        /// <param name="link">每个32整数值之间的连接字符</param>
        /// <returns></returns>
        public string ToString(bool upper, char link)
        {
            char* cp = stackalloc char[35];
            ValueToString(upper, link, cp);
            return new string(cp, 0, 35);
        }

        public int CompareTo(Hash128 other)
        {
            var c = p1.CompareTo(other.p1);
            if (c != 0) return c;
            c = p2.CompareTo(other.p2);
            if (c != 0) return c;
            c = p3.CompareTo(other.p3);
            if (c != 0) return c;
            return p4.CompareTo(other.p4);
        }

        #endregion

        #endregion

    }

}