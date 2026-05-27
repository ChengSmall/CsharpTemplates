using Cheng.Algorithm.Sorts;
using Cheng.DataStructure.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Collections
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public unsafe static partial class ListExtend
    {

        #region 查

        #region 元素查询

        #region FindIndex

        #region 泛型

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器，null表示使用默认比较器</param>
        /// <param name="index">查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int QueryIndex<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T> equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;

            for (int i = index; i < end; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器，null表示使用默认比较器</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T> equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            //int end = index + count;
            //if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (length == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;

            for (int i = 0; i < length; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex<T>(this IReadOnlyList<T> list, T value)
        {
            return QueryIndex(list, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器，null表示使用默认比较器</param>
        /// <param name="index">查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int QueryIndex<T>(this IList<T> list, T value, IEqualityComparer<T> equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;

            for (int i = index; i < end; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器，null表示使用默认比较器</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex<T>(this IList<T> list, T value, IEqualityComparer<T> equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            //int end = index + count;
            //if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (length == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;

            for (int i = 0; i < length; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex<T>(this IList<T> list, T value)
        {
            return QueryIndex(list, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器，null表示默认比较器</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int QueryLastIndex<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T> equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;
            for (int i = end - 1; i >= index; i--)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器，null表示默认比较器</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T> equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();
            return QueryLastIndex(list, value, equalityComparer, 0, list.Count);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex<T>(this IReadOnlyList<T> list, T value)
        {
            return QueryLastIndex(list, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器，null表示默认比较器</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int QueryLastIndex<T>(this IList<T> list, T value, IEqualityComparer<T> equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;
            for (int i = end - 1; i >= index; i--)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器，null表示默认比较器</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex<T>(this IList<T> list, T value, IEqualityComparer<T> equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();
            return QueryLastIndex(list, value, equalityComparer, 0, list.Count);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex<T>(this IList<T> list, T value)
        {
            return QueryLastIndex(list, value, EqualityComparer<T>.Default);
        }

        #endregion

        #region 装箱

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器</param>
        /// <param name="index">查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int QueryIndex(this IList list, object value, IEqualityComparer equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<object>.Default;

            for (int i = index; i < end; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <param name="equalityComparer">对象比较器，null表示使用默认比较器</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex(this IList list, object value, IEqualityComparer equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            //int end = index + count;
            //if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (length == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<object>.Default;

            for (int i = 0; i < length; i++)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询集合是否拥有匹配项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="value">要查询的匹配项</param>
        /// <returns>匹配的第一个索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryIndex(this IList list, object value)
        {
            return QueryIndex(list, value, null);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        public static int QueryLastIndex(this IList list, object value, IEqualityComparer equalityComparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;
            if (equalityComparer is null) equalityComparer = EqualityComparer<object>.Default;
            for (int i = end - 1; i >= index; i--)
            {
                if (equalityComparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <param name="equalityComparer">比较器，null表示使用默认比较器</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex(this IList list, object value, IEqualityComparer equalityComparer)
        {
            if (list is null) throw new ArgumentNullException();
            return QueryLastIndex(list, value, equalityComparer, 0, list.Count);
        }

        /// <summary>
        /// 从最后一位元素开始查询集合是否拥有匹配项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value">查询的匹配项</param>
        /// <returns>匹配的最后一位元素索引；若没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int QueryLastIndex(this IList list, object value)
        {
            return QueryLastIndex(list, value, EqualityComparer<object>.Default);
        }

        #endregion

        #endregion

        #region Find

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;
            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return -1;

            for (int i = index; i < end; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            for (int i = index; i < end; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="predicate">为此条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindIndex(this IList list, Predicate<object> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            for (int i = index; i < end; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindIndex(list, predicate, 0, list.Count);
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindIndex(list, predicate, 0, list.Count);
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>第一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindIndex(this IList list, Predicate<object> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindIndex(list, predicate, 0, list.Count);
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return -1;

            for (int i = end - 1; i >= index; i--)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindLastIndex<T>(this IList<T> list, Predicate<T> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            for (int i = end - 1; i >= index; i--)
            {

                if (predicate.Invoke(list[i]))
                {
                    return i;
                }

            }

            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="predicate">为此条件</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static int FindLastIndex(this IList list, Predicate<object> predicate, int index, int count)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            for (int i = end - 1; i >= index; i--)
            {

                if (predicate.Invoke(list[i]))
                {
                    return i;
                }

            }

            return -1;
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindLastIndex(list, predicate, 0, list.Count);
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindLastIndex<T>(this IList<T> list, Predicate<T> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindLastIndex(list, predicate, 0, list.Count);
        }

        /// <summary>
        /// 查询元素是否拥匹配谓词条件，并返回索引
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="predicate">谓词条件</param>
        /// <returns>最后一个匹配谓词的元素索引，如果没有匹配元素则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int FindLastIndex(this IList list, Predicate<object> predicate)
        {
            if (list is null) throw new ArgumentNullException();
            return FindLastIndex(list, predicate, 0, list.Count);
        }

        #endregion

        #region FindAll

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="index">要查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll<T>(this IReadOnlyList<T> list, Predicate<T> predicate, int index, int count, ICollection<T> append)
        {
            if (list is null || predicate is null || append is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            for (int i = index; i < end; i++)
            {
                T t = list[i];
                if (predicate.Invoke(t))
                {
                    append.Add(t);
                }
            }
        }

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="index">要查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll<T>(this IList<T> list, Predicate<T> predicate, int index, int count, ICollection<T> append)
        {
            if (list is null || predicate is null || append is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            for (int i = index; i < end; i++)
            {
                T t = list[i];
                if (predicate.Invoke(t))
                {
                    append.Add(t);
                }
            }

        }

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="index">要查询的起始索引</param>
        /// <param name="count">要查询的元素数量</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定索引超出范围</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll(this IList list, Predicate<object> predicate, int index, int count, IList append)
        {
            if (list is null || predicate is null) throw new ArgumentNullException();

            int length = list.Count;
            int end = index + count;

            if (index < 0 || count < 0 || (end > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            for (int i = index; i < end; i++)
            {
                object t = list[i];
                if (predicate.Invoke(t))
                {
                    append.Add(t);
                }
            }
        }

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll<T>(this IReadOnlyList<T> list, Predicate<T> predicate, ICollection<T> append)
        {
            if (list is null) throw new ArgumentNullException();
            FindAll(list, predicate, 0, list.Count, append);
        }

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll<T>(this IList<T> list, Predicate<T> predicate, ICollection<T> append)
        {
            if (list is null) throw new ArgumentNullException();
            FindAll(list, predicate, 0, list.Count, append);
        }

        /// <summary>
        /// 将所有匹配谓词的元素添加到指定集合
        /// </summary>
        /// <param name="list">要查询的集合</param>
        /// <param name="predicate">谓词</param>
        /// <param name="append">将匹配谓词的元素添加到的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">添加的集合为只读集合</exception>
        public static void FindAll(this IList list, Predicate<object> predicate, IList append)
        {
            if (list is null) throw new ArgumentNullException();
            FindAll(list, predicate, 0, list.Count, append);
        }

        #endregion

        #endregion

        #region 检查是否有序

        public static bool IsOrdered<T>(this IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;

            using (var enr = collection.GetEnumerator())
            {
                T last;

                if (!enr.MoveNext())
                {
                    return true;
                }
                last = enr.Current;

                while (enr.MoveNext())
                {
                    var cur = enr.Current;
                    if (comparer.Compare(last, cur) >= 0)
                    {
                        return false;
                    }
                    last = cur;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证集合是否有序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要验证的集合</param>
        /// <param name="comparer">进行验证时的比较方法实现，参数为null时使用默认实现的<see cref="Comparer{T}.Default"/></param>
        /// <returns>有序集合返回true，无序集合返回false</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static bool IsOrdered<T>(this IReadOnlyList<T> list, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;

            int count = list.Count;
            if (count <= 1) return true;
            int i;
            int length = count - 1;

            for (i = 0; i < length; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证集合是否有序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要验证的集合</param>
        /// <returns>有序集合返回true，无序集合返回false</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static bool IsOrdered<T>(this IReadOnlyList<T> list)
        {
            return IsOrdered(list, null);
        }

        /// <summary>
        /// 验证集合是否有序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要验证的集合</param>
        /// <param name="comparer">进行验证时的比较方法实现，参数为null时使用默认实现的<see cref="Comparer{T}.Default"/></param>
        /// <returns>有序集合返回true，无序集合返回false</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static bool IsOrdered<T>(this IList<T> list, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;

            int count = list.Count;
            if (count <= 1) return true;
            int i;
            int length = count - 1;

            for (i = 0; i < length; i++)
            {
                if(comparer.Compare(list[i], list[i + 1]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证集合是否有序，使用默认比较器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要验证的集合</param>
        /// <returns>有序集合返回true，无序集合返回false</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static bool IsOrdered<T>(this IList<T> list)
        {
            int count = list.Count;
            if (count <= 1) return true;

            var comparer = Comparer<T>.Default;

            int i;
            int length = count - 1;

            for (i = 0; i < length; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证集合是否有序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要验证的集合</param>
        /// <param name="comparer">进行验证时的比较方法实现</param>
        /// <returns>有序集合返回true，无序集合返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">排序方法异常</exception>
        public static bool IsOrdered(this IList list, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            int count = list.Count;
            if (count <= 1) return true;
            int i;
            int length = count - 1;

            for (i = 0; i < length; i++)
            {
                if (comparer.Compare(list[i], list[i + 1]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 二分查找

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法接口；该参数为null则表示使用默认的<see cref="Comparer{T}.Default"/></param>
        /// <param name="index">查询范围的起始索引</param>
        /// <param name="count">要查询的集合数量范围</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或者无法进行查询</exception>
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T obj, IComparer<T> comparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();
            int ir = index + count;
            if ((ir > list.Count) || index < 0 || count < 0) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;
            return f_binSearch(list, index, ir - 1, obj, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合，请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法接口；该参数为null则表示使用默认的<see cref="Comparer{T}.Default"/></param>
        /// <returns></returns>
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T obj, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            return BinarySearch(list, obj, comparer, 0, list.Count);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合，请保证集合必须是能够被默认比较器有序比较的数组</param>
        /// <param name="obj">要查找的元素</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或者无法进行查询</exception>
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T obj)
        {
            if (list is null) throw new ArgumentNullException();
            if (list.Count == 0) return -1;
            return f_binSearch(list, 0, list.Count - 1, obj, Comparer<T>.Default);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法接口；该参数为null则表示使用默认的<see cref="Comparer{T}.Default"/></param>
        /// <param name="index">查询范围的起始索引</param>
        /// <param name="count">要查询的集合数量范围</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或者无法进行查询</exception>
        public static int BinarySearch<T>(this IList<T> list, T obj, IComparer<T> comparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();

            int ir = index + count;
            if ((ir > list.Count) || index < 0 || count < 0) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            return f_binSearch(list, index, ir - 1, obj, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合，请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法接口；该参数为null则表示使用默认的<see cref="Comparer{T}.Default"/></param>
        /// <returns></returns>
        public static int BinarySearch<T>(this IList<T> list, T obj, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            return BinarySearch(list, obj, comparer ?? Comparer<T>.Default, 0, list.Count);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被有序比较的数组
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要查找的集合，请保证集合必须是能够被默认比较器有序比较的数组</param>
        /// <param name="obj">要查找的元素</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或者无法进行查询</exception>
        public static int BinarySearch<T>(this IList<T> list, T obj)
        {
            if (list is null) throw new ArgumentNullException();
            if (list.Count == 0) return -1;
            return f_binSearch(list, 0, list.Count - 1, obj, Comparer<T>.Default);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被参数<paramref name="comparer"/>有序比较的数组
        /// </remarks>
        /// <param name="list">要查找的集合</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法</param>
        /// <param name="index">查询范围的起始索引</param>
        /// <param name="count">要查询的集合数量范围</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或无法进行查询或排序方法出错</exception>
        public static int BinarySearch(this IList list, object obj, IComparer comparer, int index, int count)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            int ir = index + count;
            if ((ir > list.Count) || index < 0 || count < 0) throw new ArgumentOutOfRangeException();

            if (count == 0) return -1;

            return fng_binSearch(list, index, ir - 1, obj, comparer);
        }

        /// <summary>
        /// 使用二分查找返回指定元素的索引
        /// </summary>
        /// <remarks>
        /// 如果集合内有相等的元素，则返回值是其中一个元素索引；请保证集合必须是能够被有序比较的数组
        /// </remarks>
        /// <param name="list">要查找的集合，请保证集合必须是能够被默认比较器有序比较的数组</param>
        /// <param name="obj">要查找的元素</param>
        /// <param name="comparer">进行二分查找时的比较方法</param>
        /// <returns>元素<paramref name="obj"/>的索引，若找不到则返回-1</returns>
        /// <exception cref="ArgumentNullException">list或comparer参数为null</exception>
        /// <exception cref="ArgumentException">集合不是有序的或无法进行查询或排序方法出错</exception>
        public static int BinarySearch(this IList list, object obj, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();
            int count = list.Count;
            if (count == 0) return -1;
            return fng_binSearch(list, 0, count - 1, obj, comparer);
        }

        static int f_binSearch<T>(IReadOnlyList<T> list, int left, int right, T obj, IComparer<T> comparer)
        {
            int n;
            int temp;

            while (left <= right)
            {
                n = left + ((right - left) / 2);

                temp = comparer.Compare(list[n], obj);
                if (temp == 0)
                {
                    return n;
                }
                else if (temp < 0)
                {
                    //在对象左边
                    //移动左指针到索引
                    left = n + 1;
                }
                else if (temp > 0)
                {
                    //在对象右边
                    //移动右指针到索引
                    right = n - 1;
                }
            }
            return -1;
        }

        static int f_binSearch<T>(IList<T> list, int left, int right, T obj, IComparer<T> comparer)
        {
            int n;
            int temp;

            while (left <= right)
            {

                n = left + ((right - left) / 2);

                temp = comparer.Compare(list[n], obj);
                if (temp == 0)
                {
                    return n;
                }
                else if (temp < 0)
                {
                    //在对象左边
                    //移动左指针到索引
                    left = n + 1;
                }
                else if(temp > 0)
                {
                    //在对象右边
                    //移动右指针到索引
                    right = n - 1;
                }

            }
            return -1;

        }

        static int fng_binSearch(IList list, int left, int right, object obj, IComparer comparer)
        {
            int n;
            int temp;

            while (left <= right)
            {

                n = left + ((right - left) / 2);

                temp = comparer.Compare(list[n], obj);
                if (temp == 0)
                {
                    return n;
                }
                else if (temp < 0)
                {
                    //在对象左边
                    //移动左指针到索引
                    left = n + 1;
                }
                else if (temp > 0)
                {
                    //在对象右边
                    //移动右指针到索引
                    right = n - 1;
                }

            }
            return -1;
        }

        #endregion

        #endregion

        #region 排序

        /// <summary>
        /// 将集合内的两个元素交换
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index1">元素1索引</param>
        /// <param name="index2">元素2索引</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引越界</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public static void Swap(this IList list, int index1, int index2)
        {
            if (list is null) throw new ArgumentNullException();
            f_Swap(list, index1, index2);
        }

        /// <summary>
        /// 将集合内的两个元素交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index1">元素1索引</param>
        /// <param name="index2">元素2索引</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引越界</exception>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            if (list is null) throw new ArgumentNullException();
            f_Swap(list, index1, index2);
        }

        internal static void f_Swap(this IList list, int index1, int index2)
        {
            object temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        internal static void f_Swap<T>(this IList<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">定义的排序方法，若为null则使用默认实现的排序方法</param>
        /// <param name="index">要排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ArgumentNullException">集合参数为null</exception>
        public static void Sort<T>(this IList<T> list, IComparer<T> comparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();
            int length = list.Count;
            if (index < 0 || count < 0 || (index + count > length)) throw new ArgumentOutOfRangeException();

            if (count <= 1) return;

            if (comparer is null) comparer = Comparer<T>.Default;

            if (count == 2)
            {
                if (comparer.Compare(list[0], list[1]) > 0) list.f_Swap(0, 1);
                return;
            }

            if (list is T[])
            {
                Array.Sort((T[])list, index, count, comparer);
            }
            else if (list is List<T>)
            {
                ((List<T>)list).Sort(index, count, comparer);
            }
            else
            {
                f_qukeAndInsertSort(list, index, index + count - 1, comparer, 64);
            }

        }

        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法，null表示使用默认实现</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void Sort<T>(this IList<T> list, IComparer<T> comparer)
        {
            Sort(list ?? throw new ArgumentNullException(), comparer, 0, list.Count);
        }

        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        public static void Sort<T>(this IList<T> list)
        {
            Sort(list ?? throw new ArgumentNullException(), null, 0, list.Count);
        }

        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">定义的排序方法</param>
        /// <param name="index">要排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void Sort(this IList list, IComparer comparer, int index, int count)
        {
            if (list is null) throw new ArgumentNullException();
            int length = list.Count;
            if (index < 0 || count < 0 || (index + count > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            if (comparer is null) throw new ArgumentNullException();

            if (count == 2)
            {
                if (comparer.Compare(list[0], list[1]) > 0) list.f_Swap(0, 1);
                return;
            }

            if (list is Array)
            {
                Array.Sort((Array)list, index, count, comparer);
            }
            else if (list is System.Collections.ArrayList)
            {
                ((ArrayList)list).Sort(index, count, comparer);
            }
            else
            {
                ng_QukeAndInsertSort(list, index, index + count - 1, comparer, 64);
            }
        }

        /// <summary>
        /// 对集合进行排序
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void Sort(this IList list, IComparer comparer)
        {
            Sort(list ?? throw new ArgumentNullException(), comparer, 0, list.Count);
        }

        #region 综排

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和开销</para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">默认排序器没有默认排序实现</exception>
        public static void QukeAndInsertSort<T>(this IList<T> list)
        {
            QukeAndInsertSort(list ?? throw new ArgumentNullException(), null, 0, list.Count);
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和开销</para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法，null表示使用<typeparamref name="T"/>类型的默认排序方法</param>
        /// <param name="index">排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">排序器是null并且没有默认排序实现，或其他参数错误</exception>
        public static void QukeAndInsertSort<T>(this IList<T> list, IComparer<T> comparer)
        {
            QukeAndInsertSort(list ?? throw new ArgumentNullException(), comparer, 0, list.Count);
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和消耗</para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法，null表示使用<typeparamref name="T"/>类型的默认排序方法</param>
        /// <param name="index">排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ArgumentException">排序器是null并且没有默认排序实现，或其他参数错误</exception>
        public static void QukeAndInsertSort<T>(this IList<T> list, IComparer<T> comparer, int index, int count)
        {
            QukeAndInsertSort(list, comparer, index, count, 64);
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和消耗</para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法，null表示使用<typeparamref name="T"/>类型的默认排序方法</param>
        /// <param name="index">排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <param name="stackDepth">进行快速排序时的递归深度，值越大深度越深，等于或小于0则不使用快速排序，默认值为64</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ArgumentException">排序器是null并且没有默认排序实现，或其他参数错误</exception>
        public static void QukeAndInsertSort<T>(this IList<T> list, IComparer<T> comparer, int index, int count, int stackDepth)
        {
            if (list is null) throw new ArgumentNullException();
            int length = list.Count;
            if (index < 0 || count < 0 || (index + count > length)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            f_qukeAndInsertSort(list, index, index + count - 1, comparer ?? Comparer<T>.Default, stackDepth);
        }

        static void f_qukeAndInsertSort<T>(IList<T> list, int low, int high, IComparer<T> comparer, int count)
        {
            if (low < high)
            {
                if (count <= 0 /*|| ((high - low) <= 8)*/)
                {
                    //按条件插排
                    InsertionSort.f_InsertSort(list, comparer, low, high - low + 1);
                }
                else
                {
                    //将数组分割为两部分，并返回分割点的索引
                    int pivotIndex = QukeSort.partition(list, low, high, comparer);

                    //递归对分割后的两部分进行排序
                    f_qukeAndInsertSort(list, low, pivotIndex - 1, comparer, count - 1);
                    f_qukeAndInsertSort(list, pivotIndex + 1, high, comparer, count - 1);
                }

            }
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和开销</para>
        /// </remarks>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法</param>
        /// <param name="index">排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <param name="stackDepth">进行快速排序时的入栈深度，值越大深度越深，等于或小于0则不使用快速排序，默认值为32</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static void QukeAndInsertSort(this IList list, IComparer comparer, int index, int count, int stackDepth)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();
            int length = list.Count;
            if (index < 0 || count < 0 || (index + count > length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return;
            ng_QukeAndInsertSort(list, index, index + count - 1, comparer, stackDepth);
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <remarks>
        /// <para>使用快速和选择排序优化排序速度和开销</para>
        /// </remarks>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法</param>
        /// <param name="index">排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static void QukeAndInsertSort(this IList list, IComparer comparer, int index, int count)
        {
            QukeAndInsertSort(list, comparer, index, count, 32);
        }

        /// <summary>
        /// 对集合排序
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">排序方法</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static void QukeAndInsertSort(this IList list, IComparer comparer)
        {
            QukeAndInsertSort(list ?? throw new ArgumentNullException(), comparer, 0, list.Count, 32);
        }

        static void ng_QukeAndInsertSort(IList list, int low, int high, IComparer comparer, int count)
        {
            if (low < high)
            {
                if (count <= 0 /*|| (high - low) <= 16*/)
                {
                    //按条件插排
                    InsertionSort.InsertSort(list, comparer, low, high - low + 1);
                }
                else
                {
                    //将数组分割为两部分，并返回分割点的索引
                    int pivotIndex = QukeSort.ng_partition(list, low, high, comparer);

                    //递归对分割后的两部分进行排序
                    ng_QukeAndInsertSort(list, low, pivotIndex - 1, comparer, count - 1);
                    ng_QukeAndInsertSort(list, pivotIndex + 1, high, comparer, count - 1);
                }

            }
        }

        #endregion
       
        #endregion

        #region 二维数组

        #region 块移动

        internal static void f_moveToRect<T>(this T[,] array, int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {

            if (toBeginX < beginX)
            {

                if (toBeginY < beginY)
                {
                    //左下
                    f_leftDownMove(array, beginX, endX, beginY, endY, toBeginX, toBeginY);
                }
                else
                {
                    //左上或左
                    f_leftUpMove(array, beginX, endX, beginY, endY, toBeginX, toBeginY);
                }
            }
            else
            {

                if (toBeginY < beginY)
                {
                    //右下
                    f_rightDownMove(array, beginX, endX, beginY, endY, toBeginX, toBeginY);
                }
                else
                {
                    //右上
                    f_rightUpMove(array, beginX, endX, beginY, endY, toBeginX, toBeginY);
                }

            }

        }

        private static void f_leftDownMove<T>(T[,] array, int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {
            int x, y, nx, ny;


            for (x = beginX, nx = toBeginX; x <= endX; x++, nx++)
            {
                //横轴从左到右
                for (y = beginY, ny = toBeginY; y <= endY; y++, ny++)
                {
                    //纵轴从下到上
                    array[nx, ny] = array[x, y];
                }
            }


        }

        private static void f_leftUpMove<T>(T[,] array, int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {
            int x, y, nx, ny;
            int toby, tobx;

            tobx = toBeginX;
            toby = toBeginY + (endY - beginY);

            for (x = beginX, nx = tobx; x <= endX; x++, nx++)
            {
                //横轴从左到右
                for (y = endY, ny = toby; y >= beginX; y--, ny--)
                {
                    //纵轴从上到下
                    array[nx, ny] = array[x, y];
                }
            }
        }

        private static void f_rightDownMove<T>(T[,] array, int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {
            int x, y, nx, ny;
            int toby, tobx;

            tobx = toBeginX + (endX - beginX);
            toby = toBeginY;

            for (x = endX, nx = tobx; x >= beginX; x--, nx--)
            {
                //横轴从右到左
                for (y = beginY, ny = toby; y <= endY; y++, ny++)
                {
                    //纵轴从下到上
                    array[nx, ny] = array[x, y];
                }
            }
        }

        private static void f_rightUpMove<T>(T[,] array, int beginX, int endX, int beginY, int endY, int toBeginX, int toBeginY)
        {
            int x, y, nx, ny;
            int toby, tobx;

            tobx = toBeginX + (endX - beginX);
            toby = toBeginY + (endY - beginY);

            for (x = endX, nx = tobx; x >= beginX; x--, nx--)
            {
                //横轴从右到左
                for (y = endX, ny = toby; y >= beginY; y--, ny--)
                {
                    //纵轴从上到下
                    array[nx, ny] = array[x, y];
                }
            }
        }

        /// <summary>
        /// 将二维数组中的某一块数据内容拷贝移动到另一块数据中
        /// </summary>
        /// <param name="beginX">原数据块的长度起始索引</param>
        /// <param name="countX">原数据块的长度</param>
        /// <param name="beginY">原数据块的高度起始索引</param>
        /// <param name="countY">原数据块的高度</param>
        /// <param name="toBeginX">目标数据块的起始长度索引</param>
        /// <param name="toBeginY">目标数据块的起始高度索引</param>
        /// <exception cref="ArgumentException">指定的参数超出范围</exception>
        public static void MoveToRect<T>(this T[,] array, int beginX, int countX, int beginY, int countY, int toBeginX, int toBeginY)
        {
            if (array is null) throw new ArgumentNullException();

            int endX = beginX + countX - 1;
            int endY = beginY + countY - 1;

            if (beginX < 0 || beginY < 0 || toBeginX < 0 || toBeginY < 0) throw new ArgumentOutOfRangeException();
            if (endX < beginX || endY < beginY) throw new ArgumentOutOfRangeException();
            if (endX >= array.GetLength(0) || endY >= array.GetLength(1)) throw new ArgumentException();

            f_moveToRect(array, beginX, endX, beginY, endY, toBeginX, toBeginY);
        }

        #endregion

        /// <summary>
        /// 将二维数组拷贝到另一个数组中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">要拷贝的原数组</param>
        /// <param name="beginX">拷贝的起始长度索引</param>
        /// <param name="countX">拷贝的长度</param>
        /// <param name="beginY">拷贝的起始宽度索引</param>
        /// <param name="countY">拷贝的宽度</param>
        /// <param name="toArray">目标数组</param>
        /// <param name="toBeginX">目标数组的起始长度索引</param>
        /// <param name="toBeginY">目标数组的起始宽度索引</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public static void CopyTo<T>(this T[,] array, int beginX, int countX, int beginY, int countY, T[,] toArray, int toBeginX, int toBeginY)
        {
            if (array is null || toArray is null) throw new ArgumentNullException();

            if (beginX < 0 || countX < 0 || beginY < 0 || countY < 0 || toBeginX < 0 || toBeginY < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            int endX, endY;
            endX = beginX + countX;
            endY = beginY + countY;

            if(array.GetLength(0) < endX || array.GetLength(1) < endY || toArray.GetLength(0) < endX || toArray.GetLength(1) < endY)
            {
                throw new ArgumentException();
            }

            f_copyTo(array, beginX, endX - 1, beginY, endY - 1, toArray, toBeginX, toBeginY);
        }

        internal static void f_copyTo<T>(this T[,] array, int beginX, int endX, int beginY, int endY, T[,] toArray, int toBeginX, int toBeginY)
        {
            int x, y, nx, ny;
            if (endX - beginX == 0 || endY - beginY == 0) return;

            for (x = beginX, nx = toBeginX; x <= endX; x++)
            {
                for (y = beginY, ny = toBeginY; y < endY; x++)
                {
                    toArray[nx, ny] = array[x, y];
                }
            }

        }

        #endregion

        #region 多叉树

        #endregion

    }

}
