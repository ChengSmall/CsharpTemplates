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

    /// <summary>
    /// 将泛型比较接口转接到基类比较接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GenericComparer<T> : IComparer
    {
        /// <summary>
        /// 实例化封装比较器
        /// </summary>
        /// <param name="comparer">公共比较器接口</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        public GenericComparer(IComparer<T> comparer)
        {
            p_comparer = comparer ?? throw new ArgumentNullException();
        }

        private IComparer<T> p_comparer;

        /// <summary>
        /// 获取内部封装的比较器
        /// </summary>
        public IComparer<T> Comparer => p_comparer;

        public int Compare(object x, object y)
        {
            if(x is T a && y is T b)
            {
                return p_comparer.Compare(a, b);
            }
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// 将一个比较方法转换为另一种类型比较
    /// </summary>
    /// <typeparam name="T">实现比较器的类型</typeparam>
    /// <typeparam name="O">实际使用的比较对象类型</typeparam>
    public sealed class ToOtherComparer<T,O> : Comparer<T>
    {

        /// <summary>
        /// 实例化对象类型转换比较
        /// </summary>
        /// <param name="toOtherComparer">将类类型<typeparamref name="T"/>对象转换为类型<typeparamref name="O"/>对象的转换方法</param>
        /// <param name="comparer">实际使用的对象比较器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public ToOtherComparer(Func<T,O> toOtherComparer, IComparer<O> comparer)
        {
            if (comparer is null || toOtherComparer is null) throw new ArgumentNullException();
            p_to = toOtherComparer;
            p_comparer = comparer;
        }

        private IComparer<O> p_comparer;
        private Func<T, O> p_to;

        public override int Compare(T x, T y)
        {
            return p_comparer.Compare(p_to.Invoke(x), p_to.Invoke(y));
        }

    }

}
