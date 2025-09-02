using System;
using System.Collections.Generic;
using System.Linq;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 使用比较器列表按顺序比较对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ListComparer<T> : Comparer<T>
    {

        #region 构造

        /// <summary>
        /// 实例化列表比较器
        /// </summary>
        /// <param name="collection">要使用的比较器列表，比较时会按顺序依次使用比较器</param>
        public ListComparer(IEnumerable<IComparer<T>> collection)
        {
            p_arr = collection.Where(f_toNotNull).ToArray();
            if (p_arr.Length == 0) throw new ArgumentException();
        }

        static bool f_toNotNull(IComparer<T> comparer)
        {
            return comparer != null;
        }

        #endregion

        #region 参数

        private readonly IComparer<T>[] p_arr;

        #endregion

        #region 派生

        /// <summary>
        /// 使用比较器列表比较两个对象
        /// </summary>
        /// <param name="x">前一个对象</param>
        /// <param name="y">后一个对象</param>
        /// <returns>
        /// <para>按顺序依次调用列表内的比较器，如果返回非0值则返回当前比较器的结果；如果等于0使用下一个比较器继续比较，如果所有比较器返回值都是0则该函数返回0</para>
        /// </returns>
        public override int Compare(T x, T y)
        {
            foreach (var comp in p_arr)
            {
                var c = comp.Compare(x, y);
                if (c != 0) return c;
            }
            return 0;
        }

        #endregion

    }

}
