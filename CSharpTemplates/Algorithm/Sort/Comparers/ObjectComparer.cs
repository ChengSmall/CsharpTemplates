using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 将基类型接口<see cref="IComparer"/>封装到<see cref="IComparer{T}"/>泛型接口内
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ObjectComparer<T> : Comparer<T>
    {

        /// <summary>
        /// 实例化封装比较器
        /// </summary>
        /// <param name="comparer">公共比较器接口</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        public ObjectComparer(IComparer comparer)
        {
            p_comparer = comparer ?? throw new ArgumentNullException();
        }

        private IComparer p_comparer;

        /// <summary>
        /// 获取内部封装的比较器
        /// </summary>
        public IComparer Comparer => p_comparer;

        /// <summary>
        /// 使用内部比较器比较的两个泛型对象
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(T x, T y)
        {
            return p_comparer.Compare(x, y);
        }


    }

}
