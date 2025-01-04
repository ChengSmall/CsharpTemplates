using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// 根据键名访问值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>访问到的值</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        /// <exception cref="KeyNotFoundException">键不存在</exception>
        TValue this[TKey key] { get; }
        /// <summary>
        /// 根据键名访问值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">访问到的值，无法获取则表示一个默认值</param>
        /// <returns>若成功获取值返回true，若无法成功获取返回false</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        bool TryGetValue(TKey key, out TValue value);
        /// <summary>
        /// 获取当前键值对集合内拥有的键值对数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 访问所有的键
        /// </summary>
        IReadOnlyCollection<TKey> Keys { get; }
        /// <summary>
        /// 访问所有的值
        /// </summary>
        IReadOnlyCollection<TValue> Values { get; }
        /// <summary>
        /// 确认指定键是否存在于该键值对集合中
        /// </summary>
        /// <param name="key">要确认的键</param>
        /// <returns>存在于集合中返回true，不存在集合中返回false</returns>
        /// <exception cref="ArgumentNullException">键为null</exception>
        bool ContainsKey(TKey key);
    }
    /// <summary>
    /// 封装一个键值对为只读
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {

        #region 结构
        /// <summary>
        /// 表示一个只读集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ReadOnlyCollection<T> : IReadOnlyCollection<T>
        {
            /// <summary>
            /// 实例化一个只读集合
            /// </summary>
            /// <param name="collection">要封装的集合</param>
            public ReadOnlyCollection(ICollection<T> collection)
            {
                this.collection = collection ?? throw new ArgumentNullException();
            }

            private ICollection<T> collection;

            public int Count => collection.Count;

            public IEnumerator<T> GetEnumerator()
            {
                return collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return collection.GetEnumerator();
            }
        }

        #endregion

        #region 构造
        public ReadOnlyDictionary(IDictionary<TKey,TValue> dict)
        {
            p_dict = dict ?? throw new ArgumentNullException();
        }
        #endregion

        #region 参数

        private readonly IDictionary<TKey, TValue> p_dict;

        #endregion

        #region 功能

        #region 派生

        public int Count => p_dict.Count;
        /// <summary>
        /// 获取所有的键
        /// </summary>
        public ReadOnlyCollection<TKey> Keys
        {
            get => new ReadOnlyCollection<TKey>(p_dict.Keys);
        }
        /// <summary>
        /// 获取所有的值
        /// </summary>
        public ReadOnlyCollection<TValue> Values
        {
            get => new ReadOnlyCollection<TValue>(p_dict.Values);
        }

        IReadOnlyCollection<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get => this.Keys;
        }
        IReadOnlyCollection<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get => this.Values;
        }

        public TValue this[TKey key] => p_dict[key];

        public bool TryGetValue(TKey key, out TValue value)
        {
            return p_dict.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return p_dict.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        #endregion

        #endregion

    }

}
