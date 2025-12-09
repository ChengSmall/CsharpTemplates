using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

using Cheng.Memorys;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.UnmanagedMemoryagements
{

    /// <summary>
    /// 非托管字符串对象
    /// </summary>
    public unsafe sealed class UnString : UnObject, IEquatable<UnString>, IComparable<UnString>, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 实例化非托管字符串对象
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="length">指定非托管字符串能容纳的字符数量</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/>小于0</exception>
        public UnString(UnmanagedMemoryagement umemg, int length) : base(umemg)
        {
            if (length < 0) throw new ArgumentOutOfRangeException();
            f_init(null, length);
        }

        /// <summary>
        /// 实例化非托管字符串对象
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="value">要初始化到对象的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public UnString(UnmanagedMemoryagement umemg, string value) : base(umemg)
        {
            if (value is null) throw new ArgumentNullException();

            fixed (char* buf = value)
            {
                f_init(value.Length == 0 ? null : buf, value.Length);
            }
        }

        /// <summary>
        /// 实例化非托管字符串对象
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="value">要初始化到对象的字符串</param>
        /// <param name="index">字符串的起始索引</param>
        /// <param name="count">要拷贝到对象的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public UnString(UnmanagedMemoryagement umemg, string value, int index, int count) : base(umemg)
        {
            if (value is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count > value.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* buf = value)
            {
                f_init(buf + index, count);
            }
        }

        /// <summary>
        /// 实例化非托管字符串对象
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="buffer">要初始化到对象的字符串数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public UnString(UnmanagedMemoryagement umemg, char[] buffer) : base(umemg)
        {
            if (buffer is null) throw new ArgumentNullException();

            fixed (char* buf = buffer)
            {
                f_init(buf, buffer.Length);
            }
        }

        /// <summary>
        /// 实例化非托管字符串对象
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="buffer">要初始化到对象的字符串数组</param>
        /// <param name="index">数组的起始索引</param>
        /// <param name="count">要从数组拷贝到对象的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public UnString(UnmanagedMemoryagement umemg, char[] buffer, int index, int count) : base(umemg)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();

            fixed (char* buf = buffer)
            {
                f_init(buf + index, count);
            }
        }

        /// <summary>
        /// 实例化非托管字符串对象，使用字符串首地址
        /// </summary>
        /// <param name="umemg">内存管理器</param>
        /// <param name="buffer">要初始化到对象的字符串首地址</param>
        /// <param name="count">字符串的字符数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">字符数量小于0</exception>
        public UnString(UnmanagedMemoryagement umemg, char* buffer, int count) : base(umemg)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (count < 0) throw new ArgumentOutOfRangeException();
            f_init(buffer, count);
        }

        private void f_init(char* buffer, int count)
        {
            if (count == 0)
            {
                this.p_buffer = null;
            }
            else
            {
                this.p_buffer = (char*)p_umemg.AllocMemory(count * sizeof(char));
            }

            p_length = count;
            if (count != 0 && buffer != null)
            {
                MemoryOperation.MemoryCopy(buffer, this.p_buffer, count * sizeof(char));
            }
            p_hash = 0;
        }

        #endregion

        #region 参数

        private ulong p_hash;

        private int p_length;

        private char* p_buffer;

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing && (p_length != 0) && p_umemg.IsNotDispose)
            {
                p_umemg.FreeMemory(new IntPtr(p_buffer));
            }
            p_buffer = null;
            p_length = -1;
            return base.Disposeing(disposeing);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 字符串的字符数量，如果对象已释放返回-1
        /// </summary>
        public int Length
        {
            get
            {
                return p_length;
            }
        }

        /// <summary>
        /// 字符串的内存地址，如果对象已释放返回null，长度为0也是null
        /// </summary>
        public char* Pointer
        {
            get => p_buffer;
        }

        /// <summary>
        /// 字符串的内存地址，如果对象已释放返回null，长度为0也是null
        /// </summary>
        public CPtr<char> CPointer
        {
            get => new CPtr<char>(p_buffer);
        }

        /// <summary>
        /// 访问指定索引下的字符引用对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>指定索引下的字符</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public ref char this[int index]
        {
            get
            {
                ThrowObjectDisposeException();
                if (index < 0 || index >= p_length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return ref p_buffer[index];
            }
        }

        /// <summary>
        /// 将非托管字符串创建为CLR字符串对象
        /// </summary>
        /// <returns>新的字符串对象</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public string GetString()
        {
            ThrowObjectDisposeException(nameof(UnString));
            if (p_length == 0) return string.Empty;
            return new string(p_buffer, 0, p_length);
        }

        /// <summary>
        /// 将非托管字符串创建为CLR字符串对象并返回
        /// </summary>
        /// <returns>新的字符串对象</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public override string ToString()
        {
            ThrowObjectDisposeException(nameof(UnString));
            return GetString();
        }

        /// <summary>
        /// 比较字符串值是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UnString other)
        {
            ThrowObjectDisposeException(nameof(UnString));
            if (other is null) return false;
            if (p_length != other.p_length) return false;
            return MemoryOperation.EqualsMemory(p_buffer, other.p_buffer, p_length * sizeof(char));
        }

        /// <summary>
        /// 比较字符串值是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(UnString a, UnString b)
        {
            if ((object)a == b) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        /// <summary>
        /// 比较字符串值是否不相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(UnString a, UnString b)
        {
            if ((object)a == b) return false;
            if (a is null || b is null) return true;
            return !a.Equals(b);
        }

        /// <summary>
        /// 使用字符值依次进行大小比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public int CompareTo(UnString other)
        {
            ThrowObjectDisposeException(nameof(UnString));
            if (other is null) return 1;
            return BinaryStringEqualComparer.CompareFixedBuffer(p_buffer, other.p_buffer, p_length);
        }

        #endregion

        #region 派生

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (IsDispose) return false;
            if (obj is UnString un) return Equals(un);
            return false;
        }

        public long GetHashCode64()
        {
            if (IsDispose) return 0;
            if((p_hash >> 63) == 0)
            {
                p_hash = (ulong)HashCode64.GetHashCode64ByPointer(p_buffer, p_length);
                p_hash = p_hash | ((1UL << 63));
            }
            return (long)(p_hash & (~(1UL << 63)));
        }

        #endregion

    }

}