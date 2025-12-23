using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 封装一个键值对为只读
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ReadOnlyDictionary<TKey, TValue> : System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>
    {

        #region 结构

        /// <summary>
        /// 表示一个只读集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class ReadOnlyCollection<T> : System.Collections.Generic.IReadOnlyCollection<T>
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

        /// <summary>
        /// 实例化一个只读键值对
        /// </summary>
        /// <param name="dict">要封装的键值对</param>
        public ReadOnlyDictionary(IDictionary<TKey,TValue> dict)
        {
            p_dict = dict ?? throw new ArgumentNullException();
        }

        #endregion

        #region 参数

        private readonly IDictionary<TKey, TValue> p_dict;

        #endregion

        #region 功能

        /// <summary>
        /// 只读字典封装的内部对象
        /// </summary>
        public IDictionary<TKey, TValue> BaseDictionary
        {
            get => p_dict;
        }

        #region 派生

        /// <summary>
        /// 字典中的元素数
        /// </summary>
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

        IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys => this.Keys;

        IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values => this.Values;

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
