using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure.Texts
{

    /// <summary>
    /// 可循环访问<see cref="Unichar"/>的枚举器
    /// </summary>
    public sealed class UnicharEnumerator : ICollection<Unichar>
    {

        /// <summary>
        /// 实例化一个枚举器，使用指定字符串
        /// </summary>
        /// <param name="value">要循环访问的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public UnicharEnumerator(string value)
        {
            p_str = value ?? throw new ArgumentNullException();
            p_index = 0;
            p_count = value.Length;

            p_uniCount = f_getUnicharCount(value, 0, p_count);
        }

        /// <summary>
        /// 实例化一个枚举器，使用指定字符串范围
        /// </summary>
        /// <param name="value">要循环访问的字符串</param>
        /// <param name="index">指定初始索引</param>
        /// <param name="count">指定<see cref="char"/>字符数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出字符串范围</exception>
        public UnicharEnumerator(string value, int index, int count)
        {
            p_str = value ?? throw new ArgumentNullException();
            if (index < 0 || index + count > value.Length) throw new ArgumentOutOfRangeException();
            p_index = index;
            p_count = count;

            p_uniCount = f_getUnicharCount(value, index, count);
        }

        private readonly string p_str;
        private readonly int p_index;
        private readonly int p_count;
        private readonly int p_uniCount;

        /// <summary>
        /// 当前循环枚举器可访问多少字符
        /// </summary>
        public int Count => p_count - p_uniCount;

        /// <summary>
        /// 返回一个循环访问字符的枚举器
        /// </summary>
        /// <returns></returns>
        public UnicharEnumator GetEnumerator()
        {
            return new UnicharEnumator(p_str, p_index, p_count);
        }

        IEnumerator<Unichar> IEnumerable<Unichar>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region 结构

        /// <summary>
        /// 循环访问字符的枚举器
        /// </summary>
        public struct UnicharEnumator : IEnumerator<Unichar>
        {
            internal UnicharEnumator(string value, int index, int count)
            {
                p_str = value ?? throw new ArgumentNullException();
                if (index < 0 || index + count > value.Length) throw new ArgumentOutOfRangeException();
                p_start = index;
                p_index = index - 1;
                p_end = count + index;
                p_char = default;
            }

            private string p_str;
            private int p_start;
            private int p_index;
            private int p_end;
            private Unichar p_char;

            public Unichar Current => p_char;

            public bool MoveNext()
            {
                var i = p_index + 1;

                if (i >= p_end)
                {
                    return false;
                }

                var c = p_str[i];
                p_char = c;
                if (i + 1 < p_end)
                {
                    var nc = p_str[i + 1];
                    if (char.IsSurrogatePair(c, nc))
                    {
                        //属于代理对字符
                        p_char = new Unichar(c, nc);
                        i = i + 1;
                    }
                }

                p_index = i;
                return true;
            }

            public void Reset()
            {
                p_index = p_start - 1;
                p_char = default;
            }

            object IEnumerator.Current => Current;

            void IDisposable.Dispose()
            {
                p_char = default;
                p_index = p_end + 1;
            }
        }

        #endregion

        #region 功能

        internal static int f_getUnicharCount(string value, int index, int count)
        {
            if (count <= 1) return 0;
            int cr = 0;

            int length = index + count;
            int i = index;
            char last, next;

            last = value[i];
            i++;
            next = value[i];
            
            Loop:
            {
                if(char.IsSurrogatePair(last, next))
                {
                    //属于代理对字符
                    cr++;
                    i++;
                    if (i < length) return cr;
                    last = value[i];
                    i++;
                    if (i < length) return cr;
                    next = value[i];
                    goto Loop;
                }

                last = next;
                i++;
                if(i < length) return cr;

                next = value[i];
                goto Loop;
            }

        }

        #endregion

        #region

        bool ICollection<Unichar>.IsReadOnly => true;

        bool ICollection<Unichar>.Contains(Unichar item)
        {
            var e = GetEnumerator();
            while (e.MoveNext())
            {
                if(e.Current == item)
                {
                    return true;
                }
            }
            return false;
        }

        void ICollection<Unichar>.CopyTo(Unichar[] array, int arrayIndex)
        {
            if (array is null) throw new ArgumentNullException();
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException();

            var e = GetEnumerator();
            while (e.MoveNext())
            {
                if (arrayIndex >= array.Length) return;
                array[arrayIndex++] = e.Current;
            }
        }

        void ICollection<Unichar>.Add(Unichar item)
        {
            throw new NotSupportedException();
        }

        void ICollection<Unichar>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<Unichar>.Remove(Unichar item)
        {
            throw new NotSupportedException();
        }

        #endregion

    }

    /// <summary>
    /// 快速可循环访问<see cref="Unichar"/>的枚举器
    /// </summary>
    public readonly ref struct UnicharEnumeratorSpan
    {

        /// <summary>
        /// 初始化一个枚举器，使用指定字符串
        /// </summary>
        /// <param name="value">要循环访问的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public UnicharEnumeratorSpan(string value)
        {
            p_str = value ?? throw new ArgumentNullException();
            p_index = 0;
            p_count = value.Length;

            p_uniCount = UnicharEnumerator.f_getUnicharCount(value, 0, p_count);
        }

        /// <summary>
        /// 初始化一个枚举器，使用指定字符串范围
        /// </summary>
        /// <param name="value">要循环访问的字符串</param>
        /// <param name="index">指定初始索引</param>
        /// <param name="count">指定<see cref="char"/>字符数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出字符串范围</exception>
        public UnicharEnumeratorSpan(string value, int index, int count)
        {
            p_str = value ?? throw new ArgumentNullException();
            if (index < 0 || index + count > value.Length) throw new ArgumentOutOfRangeException();
            p_index = index;
            p_count = count;

            p_uniCount = UnicharEnumerator.f_getUnicharCount(value, index, count);
        }

        private readonly string p_str;
        private readonly int p_index;
        private readonly int p_count;
        private readonly int p_uniCount;

        /// <summary>
        /// 当前循环枚举器可访问多少字符
        /// </summary>
        public int Count => p_count - p_uniCount;

        /// <summary>
        /// 返回一个循环访问字符的枚举器
        /// </summary>
        /// <returns></returns>
        public UnicharEnumerator.UnicharEnumator GetEnumerator()
        {
            return new UnicharEnumerator.UnicharEnumator(p_str, p_index, p_count);
        }
    }

    /// <summary>
    /// 代理项字符扩展方法
    /// </summary>
    public static unsafe class UnicharExtend
    {

        #region

        /// <summary>
        /// 获取字符串内包含的代理项字符的数量
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>代理项字符的数量，没有搜索到返回0</returns>
        /// <exception cref="ArgumentNullException">字符串是null</exception>
        public static int GetSurrogatePairCount(this string value)
        {
            if (value is null) throw new ArgumentNullException();
            return UnicharEnumerator.f_getUnicharCount(value, 0, value.Length);
        }

        /// <summary>
        /// 获取字符串内包含的代理项字符的数量
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">要搜索的字符数量</param>
        /// <returns>代理项字符的数量，没有搜索到返回0</returns>
        /// <exception cref="ArgumentNullException">字符串是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int GetSurrogatePairCount(this string value, int index, int count)
        {
            if(value is null) throw new ArgumentNullException();
            if (index < 0 || index + count > value.Length) throw new ArgumentOutOfRangeException();
            
            return UnicharEnumerator.f_getUnicharCount(value, index, count);
        }

        #endregion

    }

}