using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Cheng.Algorithm.Sorts
{

    /// <summary>
    /// 实现排序算法的基类
    /// </summary>
    public abstract class BaseSort
    {
        /// <summary>
        /// 是否允许使用范围内排序
        /// </summary>
        public abstract bool CanWithinRange { get; }

        /// <summary>
        /// 使用默认排序集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual void Sort<T>(IList<T> list)
        {
            Sort(list, Comparer<T>.Default);
        }
        /// <summary>
        /// 排序集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要排序的集合</param>
        /// <param name="comparer">定义的排序规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public abstract void Sort<T>(IList<T> list, IComparer<T> comparer);

        /// <summary>
        ///  排序指定范围的集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要排序的集合</param>
        /// <param name="beginIndex">排序的元素起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <param name="comparer">定义的排序规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">无法使用范围内排序</exception>
        public virtual void Sort<T>(IList<T> list, int beginIndex, int count, IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 排序集合内元素
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="comparer">排序方法</param>
        /// <exception cref="ArgumentNullException">集合为null</exception>
        /// <exception cref="ArgumentException">参数不合规</exception>
        public abstract void Sort(IList list, IComparer comparer);

        /// <summary>
        /// 排序集合内指定范围的元素
        /// </summary>
        /// <param name="list">要排序的集合</param>
        /// <param name="beginIndex">排序的元素起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <param name="comparer">定义的排序规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不合规</exception>
        /// <exception cref="NotSupportedException">无法使用范围内排序</exception>
        public virtual void Sort(IList list, int beginIndex, int count, IComparer comparer)
        {
            throw new NotSupportedException();
        }

    }



}
