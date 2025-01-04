using System;
using System.Collections.Generic;
using System.Collections;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 表示一个只读集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// 集合内的元素数量
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// 表示一个按索引访问的只读集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// 使用索引访问集合
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>访问到的值</returns>
        /// <exception cref="ArgumentException">超出索引范围</exception>
        T this[int index] { get; }
    }

    /// <summary>
    /// 封装一个数组做只读数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayReadOnly<T> : IReadOnlyList<T>
    {

        /// <summary>
        /// 实例化一个空数组
        /// </summary>
        public ArrayReadOnly()
        {
            p_list = emptyArray;
        }

        /// <summary>
        /// 封装指定集合实例化只读数组
        /// </summary>
        /// <param name="list">初始化的集合元素</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public ArrayReadOnly(IList<T> list)
        {
            if(list is null) throw new ArgumentNullException();

            p_list = list;
        }

        private static readonly T[] emptyArray = new T[0];

        private readonly IList<T> p_list;
        /// <summary>
        /// 使用索引访问集合
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>访问到的值</returns>
        /// <exception cref="ArgumentException">超出索引范围</exception>
        public T this[int index]
        {
            get => p_list[index];
        }
        /// <summary>
        /// 判断并获取指定索引的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">要获取的值引用</param>
        /// <returns>是否成功获取，成功后去返回true，否则返回false</returns>
        public bool TryGetValue(int index, out T value)
        {
            value = default;
            if (index < 0 || index >= p_list.Count) return false;
            value = p_list[index];
            return true;
        }
        /// <summary>
        /// 集合元素数
        /// </summary>
        public int Count => p_list.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return p_list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_list.GetEnumerator();
        }
    }

}
