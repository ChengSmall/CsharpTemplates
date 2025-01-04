using System;
using System.Collections.Generic;
using System.Collections;
using Cheng.Algorithm.Collections;

namespace Cheng.Algorithm.Sorts
{

    /// <summary>
    /// 冒泡排序算法
    /// </summary>
    public sealed class BubbleSort : BaseSort
    {

        /// <summary>
        /// 使用冒泡排序算法进行集合排序
        /// </summary>
        /// <typeparam name="T">排序元素类型</typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">元素排序数量</param>
        /// <param name="comparer">排序比较器，若为null则使用默认类型排序器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出集合范围</exception>
        public static void BubbleSorting<T>(IList<T> list, int index, int count, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();

            if (index < 0 || (index + count > list.Count)) throw new ArgumentOutOfRangeException();

            if (comparer is null) comparer = Comparer<T>.Default;

            f_bsort(list, index, count, comparer);
        }

        /// <summary>
        /// 使用冒泡排序算法进行集合排序
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">元素排序数量</param>
        /// <param name="comparer">排序比较器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出集合范围</exception>
        public static void BubbleSorting(IList list, int index, int count, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            if (index < 0 || (index + count > list.Count)) throw new ArgumentOutOfRangeException();

            f_bsort(list, index, count, comparer);
        }

        #region 封装

        public BubbleSort() { }

        internal static void f_bsort<T>(IList<T> list, int index, int count, IComparer<T> comparer)
        {

            int last;

            int end = index + count - 1;

            for (int i = index; i < end; i++)
            {
                last = end - i;
                for (int j = index; j < last; j++)
                {
                    if (comparer.Compare(list[j], list[j + 1]) > 0)
                    {
                        list.f_Swap(j, j + 1);
                    }
                }
            }

        }

        internal static void f_bsort(IList list, int index, int count, IComparer comparer)
        {

            int last;

            int end = index + count - 1;

            for (int i = index; i < end; i++)
            {
                // times
                last = end - i;
                for (int j = index; j < last; j++)
                {
                    if (comparer.Compare(list[j], list[j + 1]) > 0)
                    {
                        list.f_Swap(j, j + 1);
                    }
                }
            }

        }

        #endregion

        #region 派生

        public override bool CanWithinRange => true;

        public override void Sort<T>(IList<T> list, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();

            if (comparer is null) comparer = Comparer<T>.Default;

            f_bsort<T>(list, 0, list.Count, comparer);
        }

        public override void Sort(IList list, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            f_bsort(list, 0, list.Count, comparer);
        }

        public override void Sort<T>(IList<T> list)
        {
            f_bsort<T>(list, 0, list.Count, Comparer<T>.Default);
        }

        public override void Sort<T>(IList<T> list, int beginIndex, int count, IComparer<T> comparer)
        {
            BubbleSorting<T>(list, beginIndex, count, comparer);
        }

        public override void Sort(IList list, int beginIndex, int count, IComparer comparer)
        {
            BubbleSorting(list, beginIndex, count, comparer);
        }

        #endregion

    }

}
