﻿using Cheng.Algorithm.HashCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Memorys
{

    /// <summary>
    /// 非托管类型指针对象
    /// </summary>
    /// <typeparam name="T">指针指向的非托管类型</typeparam>
    [Serializable]
    public unsafe readonly struct CPtr<T> : IEquatable<CPtr<T>>, IHashCode64, IComparable<CPtr<T>>, IFormattable where T : unmanaged
    {

        #region 初始化

        /// <summary>
        /// 使用指针初始化
        /// </summary>
        /// <param name="pointer">要初始化的指针</param>
        public CPtr(T* pointer)
        {
            this.p_ptr = pointer;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 表示空值的指针
        /// </summary>
        public static CPtr<T> Null => default;

        internal readonly T* p_ptr;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取内部封装的指针
        /// </summary>
        /// <returns></returns>
        public T* ToPointer() => p_ptr;

        #endregion

        #region 解引用

        /// <summary>
        /// 使用类型偏移量解引用
        /// </summary>
        /// <param name="offset">要添加的类型偏移</param>
        /// <returns>指向的引用对象</returns>
        public ref T this[int offset]
        {
            get => ref p_ptr[offset];
        }

        /// <summary>
        /// 使用类型偏移量解引用
        /// </summary>
        /// <param name="offset">要添加的类型偏移</param>
        /// <returns>指向的引用对象</returns>
        public ref T this[long offset]
        {
            get => ref p_ptr[offset];
        }

        /// <summary>
        /// 使用类型偏移量解引用
        /// </summary>
        /// <param name="offset">要添加的类型偏移</param>
        /// <returns>指向的引用对象</returns>
        public ref T this[uint offset]
        {
            get => ref p_ptr[offset];
        }

        /// <summary>
        /// 使用类型偏移量解引用
        /// </summary>
        /// <param name="offset">要添加的类型偏移</param>
        /// <returns>指向的引用对象</returns>
        public ref T this[ulong offset]
        {
            get => ref p_ptr[offset];
        }

        /// <summary>
        /// 解引用到引用对象
        /// </summary>
        /// <returns>指针指向的对象引用</returns>
        public ref T Ref()
        {
            return ref *p_ptr;
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 添加指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator +(CPtr<T> p1, int offset)
        {
            return new CPtr<T>(p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator -(CPtr<T> p1, int offset)
        {
            return new CPtr<T>(p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator +(CPtr<T> p1, long offset)
        {
            return new CPtr<T>(p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator -(CPtr<T> p1, long offset)
        {
            return new CPtr<T>(p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator +(CPtr<T> p1, uint offset)
        {
            return new CPtr<T>(p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator -(CPtr<T> p1, uint offset)
        {
            return new CPtr<T>(p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要添加的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator +(CPtr<T> p1, ulong offset)
        {
            return new CPtr<T>(p1.p_ptr + offset);
        }

        /// <summary>
        /// 减少指定类型偏移量
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="offset">要减少的类型偏移量</param>
        /// <returns></returns>
        public static CPtr<T> operator -(CPtr<T> p1, ulong offset)
        {
            return new CPtr<T>(p1.p_ptr - offset);
        }

        /// <summary>
        /// 添加一个类型偏移
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static CPtr<T> operator ++(CPtr<T> p)
        {
            return new CPtr<T>(p.p_ptr + 1);
        }

        /// <summary>
        /// 减少一个字节偏移
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static CPtr<T> operator --(CPtr<T> p)
        {
            return new CPtr<T>(p.p_ptr + 1);
        }

        public static bool operator ==(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr == b.p_ptr;
        }


        public static bool operator !=(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr != b.p_ptr;
        }


        public static bool operator <(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr < b.p_ptr;
        }


        public static bool operator >(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr > b.p_ptr;
        }


        public static bool operator <=(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr < b.p_ptr;
        }


        public static bool operator >=(CPtr<T> a, CPtr<T> b)
        {
            return a.p_ptr > b.p_ptr;
        }

        #endregion

        #region 转换

        public static implicit operator T*(CPtr<T> cp)
        {
            return cp.p_ptr;
        }

        public static implicit operator void*(CPtr<T> cp)
        {
            return cp.p_ptr;
        }

        public static implicit operator CPtr<T>(T* ptr)
        {
            return new CPtr<T>(ptr);
        }

        public static explicit operator CPtr<T>(void* ptr)
        {
            return new CPtr<T>((T*)ptr);
        }

        public static implicit operator IntPtr(CPtr<T> cp)
        {
            return new IntPtr(cp.p_ptr);
        }

        public static explicit operator CPtr<T>(IntPtr ptr)
        {
            return new CPtr<T>((T*)ptr.ToPointer());
        }

        public static implicit operator UIntPtr(CPtr<T> cp)
        {
            return new UIntPtr(cp.p_ptr);
        }

        public static explicit operator CPtr<T>(UIntPtr ptr)
        {
            return new CPtr<T>((T*)ptr.ToPointer());
        }

        public static explicit operator Pointer32(CPtr<T> cp)
        {
            return new Pointer32(cp.p_ptr);
        }

        public static explicit operator CPtr<T>(Pointer32 cp)
        {
            return new CPtr<T>((T*)cp.p_ptr);
        }

        public static implicit operator Pointer64(CPtr<T> cp)
        {
            return new Pointer64(cp.p_ptr);
        }

        public static explicit operator CPtr<T>(Pointer64 cp)
        {
            return new CPtr<T>((T*)cp.p_ptr);
        }

        public static explicit operator CPtr<T>(int value)
        {
            return new CPtr<T>((T*)value);
        }

        public static implicit operator CPtr<T>(uint value)
        {
            return new CPtr<T>((T*)value);
        }

        public static explicit operator CPtr<T>(long value)
        {
            return new CPtr<T>((T*)value);
        }

        public static explicit operator CPtr<T>(ulong value)
        {
            return new CPtr<T>((T*)value);
        }

        public static explicit operator int(CPtr<T> ptr)
        {
            return (int)ptr.p_ptr;
        }

        public static explicit operator uint(CPtr<T> ptr)
        {
            return (uint)ptr.p_ptr;
        }

        public static explicit operator long(CPtr<T> ptr)
        {
            return (long)ptr.p_ptr;
        }

        public static implicit operator ulong(CPtr<T> ptr)
        {
            return (ulong)ptr.p_ptr;
        }

        public static explicit operator CPtr<T>(CPtr cp)
        {
            return new CPtr<T>((T*)cp.p_ptr);
        }

        public static implicit operator CPtr(CPtr<T> cp)
        {
            return new CPtr(cp.p_ptr);
        }
        #endregion

        #region 派生

        /// <summary>
        /// 返回指针的字符串格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        public bool Equals(CPtr<T> other)
        {
            return p_ptr == other.p_ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj is CPtr<T> cp) return p_ptr == cp.p_ptr;
            return false;
        }

        public override int GetHashCode()
        {
            if (sizeof(void*) == 4) return (int)p_ptr;
            return ((ulong)p_ptr).GetHashCode();
        }

        public long GetHashCode64()
        {
            return (long)p_ptr;
        }

        public int CompareTo(CPtr<T> other)
        {
            return (this.p_ptr == other.p_ptr) ? 0 : (this.p_ptr < other.p_ptr ? -1 : 1);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (sizeof(void*) == 4)
            {
                return ((uint)p_ptr).ToString(format, formatProvider);
            }
            return ((ulong)p_ptr).ToString(format, formatProvider);
        }

        /// <summary>
        /// 使用指定格式对当前实例的值设置格式
        /// </summary>
        /// <param name="format">要使用的格式； null表示使用默认格式</param>
        /// <returns>采用指定格式的当前实例的值</returns>
        public string ToString(string format) => ToString(format, null);

        /// <summary>
        /// 使用指定格式对当前实例的值设置格式
        /// </summary>
        /// <param name="formatProvider">要用于格式化值的提供程序； null表示使用默认的格式化模式</param>
        /// <returns>采用指定格式的当前实例的值</returns>
        public string ToString(IFormatProvider formatProvider) => ToString(null, formatProvider);

        #endregion

        #endregion

    }

}
