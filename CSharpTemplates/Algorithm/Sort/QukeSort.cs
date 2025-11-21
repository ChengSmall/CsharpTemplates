using Cheng.Algorithm.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts
{

    /// <summary>
    /// 快速排序实现
    /// </summary>
    public sealed class QukeSort : BaseSort
    {

        public override bool CanWithinRange => true;

        public override void Sort<T>(IList<T> list)
        {
            Sort(list, Comparer<T>.Default);
        }

        public override void Sort<T>(IList<T> list, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            QukeliySort(list ?? throw new ArgumentNullException(), 0, list.Count - 1, comparer);
        }

        public override void Sort<T>(IList<T> list, int beginIndex, int count, IComparer<T> comparer)
        {
            QukeliySort(list ?? throw new ArgumentNullException(), beginIndex, list.Count - 1, comparer);
        }

        public override void Sort(IList list, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();
            QukeliySort(list ?? throw new ArgumentNullException(), 0, list.Count - 1, comparer);
        }

        public override void Sort(IList list, int beginIndex, int count, IComparer comparer)
        {

            QukeliySort(list ?? throw new ArgumentNullException(), beginIndex, list.Count - 1, comparer);

        }

        /// <summary>
        /// 使用快速排序排序集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排集合</param>
        /// <param name="low">待排的最小索引</param>
        /// <param name="high">待排的最小索引</param>
        /// <param name="comparer">排序的比较方法实现，该参数为null时使用默认的<see cref="Comparer{T}.Default"/></param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">使用默认的排序方法时没有实现接口</exception>
        public static void QukeliySort<T>(IList<T> list, int low, int high, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;

            if (low < high)
            {
                //将数组分割为两部分，并返回分割点的索引
                int pivotIndex = partition(list, low, high, comparer);

                //递归对分割后的两部分进行排序
                QukeliySort(list, low, pivotIndex - 1, comparer);
                QukeliySort(list, pivotIndex + 1, high, comparer);
            }
        }

        /// <summary>
        /// 使用快速排序排序集合
        /// </summary>
        /// <param name="list">待排集合</param>
        /// <param name="low">待排的最小索引</param>
        /// <param name="high">待排的最小索引</param>
        /// <param name="comparer">排序的比较方法实现</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">排序方法出错</exception>
        public static void QukeliySort(IList list, int low, int high, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            if (low < high)
            {
                //将数组分割为两部分，并返回分割点的索引
                int pivotIndex = ng_partition(list, low, high, comparer);

                //递归对分割后的两部分进行排序
                QukeliySort(list, low, pivotIndex - 1, comparer);
                QukeliySort(list, pivotIndex + 1, high, comparer);
            }
        }

        /// <summary>
        /// 进行一次快速筛选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        internal static int partition<T>(IList<T> list, int low, int high, IComparer<T> comparer)
        {
            //选择最后一个元素作为基准元素
            T pivot = list[high];
            int i = low - 1;

            for (int j = low; j <= high - 1; j++)
            {
                //如果当前元素小于等于基准元素，则将它与i+1位置的元素交换
                if (comparer.Compare(list[j], pivot) <= 0)
                {

                    i++;
                    list.f_Swap(i, j);
                    //Swap(array, i, j);
                }
            }

            //将基准元素放置到正确的位置上
            list.f_Swap(i + 1, high);

            return i + 1; //返回基准元素的索引
        }

        internal static int ng_partition(IList list, int low, int high, IComparer comparer)
        {
            //选择最后一个元素作为基准元素
            object pivot = list[high];
            int i = low - 1;

            for (int j = low; j <= high - 1; j++)
            {
                //如果当前元素小于等于基准元素，则将它与i+1位置的元素交换
                if (comparer.Compare(list[j], pivot) <= 0)
                {

                    i++;
                    list.f_Swap(i, j);
                    //Swap(array, i, j);
                }
            }

            //将基准元素放置到正确的位置上
            list.f_Swap(i + 1, high);

            return i + 1; //返回基准元素的索引
        }

        /// <summary>
        /// 进行一次快速排序分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">待排数组</param>
        /// <param name="low">要快排的最小索引</param>
        /// <param name="high">要快排的最大索引</param>
        /// <param name="comparer">用于排序的比较器，null则使用默认排序器</param>
        /// <returns>分组完毕后的中间索引</returns>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引格式错误</exception>
        public static int Partition<T>(IList<T> list, int low, int high, IComparer<T> comparer)
        {
            if (list is null) throw new ArgumentNullException();
            
            if (low < 0 || high < 0 || (high >= list.Count) || (low > high)) throw new ArgumentOutOfRangeException();

            
            return partition(list, low, high, comparer ?? Comparer<T>.Default);
        }

        /// <summary>
        /// 进行一次快速排序分组
        /// </summary>
        /// <param name="list">待排数组</param>
        /// <param name="low">要快排的最小索引</param>
        /// <param name="high">要快排的最大索引</param>
        /// <param name="comparer">用于排序的比较器，null则使用默认排序器</param>
        /// <returns>分组完毕后的中间索引</returns>
        /// <exception cref="ArgumentNullException">集合或排序方法为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引格式错误</exception>
        public static int Partition(IList list, int low, int high, IComparer comparer)
        {
            if (list is null || comparer is null) throw new ArgumentNullException();

            if (low < 0 || high < 0 || (high >= list.Count) || (low > high)) throw new ArgumentOutOfRangeException();


            return ng_partition(list, low, high, comparer);
        }

       
    }

}
