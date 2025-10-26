using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Memorys
{

    /// <summary>
    /// 栈内存块指针
    /// </summary>
    /// <typeparam name="T">非托管类型</typeparam>
    public unsafe readonly ref struct Span<T> where T : unmanaged
    {

        #region 初始化

        public Span(void* value, int length)
        {
            pointer = (T*)value;
            this.length = length;
        }

        public Span(T* value, int length)
        {
            pointer = value;
            this.length = length;
        }

        /// <summary>
        /// 使用数组初始化内存块指针
        /// </summary>
        /// <param name="array">要访问的数组</param>
        /// <param name="start">起始索引</param>
        /// <param name="length">内存长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">内存块超出范围</exception>
        public Span(T[] array, int start, int length)
        {
            if (array is null) throw new ArgumentNullException();
            if (start < 0 || length < 0 || start + length > array.Length) throw new ArgumentOutOfRangeException();
            if(length == 0)
            {
                pointer = null;
                this.length = 0;
                return;
            }
            fixed (T* ptr = array)
            {
                pointer = (ptr + start);
                this.length = length;
            }
        }

        /// <summary>
        /// 使用数组初始化内存块指针
        /// </summary>
        /// <param name="array">要访问的数组</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public Span(T[] array)
        {
            if (array is null) throw new ArgumentNullException();
            this.length = array.Length;
            if (length == 0)
            {
                pointer = null;
            }
            else
            {
                fixed (T* ptr = array)
                {
                    pointer = (ptr);
                }
            }
        }

        #endregion

        #region

        private readonly T* pointer;
        private readonly int length;

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 指针字节大小
        /// </summary>
        public int Size => sizeof(void*);

        /// <summary>
        /// 返回一个空指针
        /// </summary>
        public Span<T> Empty => new Span<T>(null, 0);

        /// <summary>
        /// 内存长度
        /// </summary>
        public int Length => length;

        /// <summary>
        /// 是否为空值
        /// </summary>
        public bool IsEmpty => length == 0;

        #endregion

        #region 解引用

        /// <summary>
        /// 使用类型偏移数解引用
        /// </summary>
        /// <param name="index">要添加的<typeparamref name="T"/>个类型的偏移量</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= length) throw new ArgumentOutOfRangeException();
                return ref pointer[index];
            }
        }

        #endregion

        #region 运算符重载

        public static bool operator ==(Span<T> a, Span<T> b)
        {
            return a.pointer == b.pointer;
        }

        public static bool operator !=(Span<T> a, Span<T> b)
        {
            return a.pointer != b.pointer;
        }

        #endregion

        #region 类型转化

        public static implicit operator Span<T>(T[] array)
        {
            fixed (T* ptr = array) return new Span<T>(ptr, array.Length);
        }

        public static implicit operator Span<T>(ArraySegment<T> segment)
        {
            fixed (T* ptr = segment.Array) return new Span<T>(ptr + segment.Offset, segment.Count);
        }

        #endregion

        #region 派生

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            if (sizeof(void*) == 4) return (int)pointer;
            return ((long)pointer).GetHashCode();
        }

        #endregion

        #region Enumator

        /// <summary>
        /// 获取一个值枚举器
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// 访问枚举器
        /// </summary>
        public ref struct Enumerator
        {
            internal Enumerator(Span<T> span)
            {
                this.span = span;
                index = -1;
            }

            private readonly Span<T> span;
            private int index;

            /// <summary>
            /// 枚举器当前访问到的值
            /// </summary>
            public ref T Current
            {
                get => ref span.pointer[index];
            }

            /// <summary>
            /// 推进到下一个值
            /// </summary>
            /// <returns>成功推进到下一个值返回true，已到达结尾返回false</returns>
            public bool MoveNext()
            {
                int i = index + 1;
                if (i == span.length) return false;
                index = i;
                return true;
            }
        }

        #endregion

        #endregion


    }

}
