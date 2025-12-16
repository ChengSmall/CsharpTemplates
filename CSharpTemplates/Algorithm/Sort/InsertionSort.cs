using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts
{
    /// <summary>
    /// 实现插入排序
    /// </summary>
    public sealed class InsertionSort : BaseSort
    {

        public override bool CanWithinRange => true;

        public override void Sort<T>(IList<T> list, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            Sort(list, comparer, 0, list.Count);
        }

        public override void Sort<T>(IList<T> list, IComparer<T> comparer, int beginIndex, int count)
        {
            InsertSort(list, comparer, beginIndex, count);
        }

        public override void Sort(IList list, IComparer comparer)
        {
            if (list is null) throw new ArgumentNullException();
            Sort(list, comparer, 0, list.Count);
        }

        public override void Sort(IList list, IComparer comparer, int beginIndex, int count)
        {
            InsertSort(list, comparer, beginIndex, count);
        }

        /// <summary>
        /// 排序集合，使用插入排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">定义排序规则，null则使用默认实现排序规则</param>
        /// <param name="beginIndex">起始索引</param>
        /// <param name="count">排序的元素数量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public static void InsertSort<T>(IList<T> list, IComparer<T> comparer, int beginIndex, int count)
        {
            if (list is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;

            if (beginIndex < 0 || count < 0 || beginIndex + count > list.Count) throw new ArgumentOutOfRangeException();

            f_InsertSort(list, comparer, beginIndex, count);
        }

        internal static void f_InsertSort<T>(IList<T> list, IComparer<T> comparer, int beginIndex, int count)
        {
            int length = beginIndex + count;
            T tmp;
            for (int index = beginIndex; index < length; index++)
            {
                //得到未排序区第一位元素
                tmp = list[index];

                //得到排序区最后一位的下标j-1

                for (int j = index; j > 0; j--)
                {
                    if (comparer.Compare(tmp, list[j - 1]) < 0)
                    {
                        //如果未排序区第一位小于排序区最后一位
                        list[j] = list[j - 1];
                        //两者位置互换
                        list[j - 1] = tmp;
                    }
                    //j--之后，tmp与排序区倒数第二位比较

                }
                //index+1
            }
        }

        /// <summary>
        /// 排序集合，使用插入排序
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="comparer">定义排序规则，null则使用默认实现排序规则</param>
        /// <param name="beginIndex">起始索引</param>
        /// <param name="count">排序的元素数量</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public static void InsertSort(IList list, IComparer comparer, int beginIndex, int count)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            int length = beginIndex + count;
            if (beginIndex < 0 || length < 0 || length > list.Count) throw new ArgumentOutOfRangeException();

            object tmp;
            for (int index = beginIndex; index < length; index++)
            {
                //得到未排序区第一位元素
                tmp = list[index];

                //得到排序区最后一位的下标j-1

                for (int j = index; j > 0; j--)
                {
                    if (comparer.Compare(tmp, list[j - 1]) < 0)
                    {
                        //如果未排序区第一位小于排序区最后一位
                        list[j] = list[j - 1];
                        //两者位置互换
                        list[j - 1] = tmp;
                    }
                    //j--之后，tmp与排序区倒数第二位比较

                }
                //index+1 ,继续比较直至比较到最后一个元素
            }
        }

    }

}
