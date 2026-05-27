using System;
using System.Collections.Generic;
using System.Collections;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 按索引获取元素的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">要访问的数组对象</param>
    /// <param name="index">数组索引</param>
    /// <returns>索引下的元素</returns>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public delegate T DelegateArrayGetValue<T>(object array, int index);

    /// <summary>
    /// 按索引设置元素的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">要访问的数组对象</param>
    /// <param name="index">数组索引</param>
    /// <param name="value">要设置的元素</param>
    /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
    public delegate void DelegateArraySetValue<T>(object array, int index, T value);

    /// <summary>
    /// 按索引设置元素的委托
    /// </summary>
    /// <param name="array">要访问的数组对象</param>
    /// <returns>数组的元素个数</returns>
    public delegate int DeleaateArrayGetCount(object array);

    /// <summary>
    /// 使用函数委托将对象封装为一个数组
    /// </summary>
    /// <remarks>
    /// 使用三个用于访问和操作数组的委托，来操作某一个数组，以达到将非公共数组派生于<see cref="IList{T}"/>接口的效果
    /// </remarks>
    /// <typeparam name="T">数组元素类型</typeparam>
    public sealed class DelegateArray<T> : IList<T>, IList
    {

        #region 构造

        /// <summary>
        /// 实例化一个委托数组对象
        /// </summary>
        /// <param name="array">要访问的数组对象</param>
        /// <param name="getArrayValue">用于按索引获取元素的委托</param>
        /// <param name="setArrayValue">用于按索引设置元素的委托</param>
        /// <param name="getArrayCount">用于返回数组的元素数量的委托</param>
        public DelegateArray(object array, DelegateArrayGetValue<T> getArrayValue, DelegateArraySetValue<T> setArrayValue, DeleaateArrayGetCount getArrayCount)
        {
            if (array is null || getArrayValue is null || getArrayCount is null || setArrayValue is null)
            {
                throw new ArgumentNullException();
            }
            p_array = array;
            p_getFunc = getArrayValue;
            p_setFunc = setArrayValue;
            p_getCount = getArrayCount;
        }

        #endregion

        #region 参数

        private object p_array;

        private DeleaateArrayGetCount p_getCount;

        private DelegateArrayGetValue<T> p_getFunc;

        private DelegateArraySetValue<T> p_setFunc;

        #endregion

        #region 功能

        public T this[int index]
        {
            get => p_getFunc.Invoke(p_array, index);
            set => p_setFunc.Invoke(p_array, index, value);
        }

        public int Count => p_getCount.Invoke(p_array);

        bool ICollection<T>.IsReadOnly => true;

        bool IList.IsFixedSize => true;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        bool IList.IsReadOnly => true;

        object IList.this[int index]
        {
            get => p_getFunc.Invoke(p_array, index);
            set => p_setFunc.Invoke(p_array, index, (T)value);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        int IList<T>.IndexOf(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        #endregion

    }

}
