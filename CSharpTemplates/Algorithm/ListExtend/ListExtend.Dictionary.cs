using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Collections
{

    unsafe partial class ListExtend
    {

        /// <summary>
        /// 默认创建的字典的函数
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="pairs">包含要创建的键值对集合</param>
        /// <param name="keyEqualityComparer">字典的key比较器，null使用默认比较器</param>
        /// <returns>创建的字典</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pairs"/>是null</exception>
        public static Dictionary<TK, TV> CreateSystemDictionaryByPairs<TK, TV>(IEnumerable<KeyValuePair<TK, TV>> pairs, IEqualityComparer<TK> keyEqualityComparer)
        {
            if (pairs is null) throw new ArgumentNullException();
            if(pairs is IDictionary<TK, TV> idict)
            {
                return new Dictionary<TK, TV>(idict, keyEqualityComparer);
            }

            int count;
            if(pairs is ICollection<KeyValuePair<TK, TV>>)
            {
                count = ((ICollection<KeyValuePair<TK, TV>>)pairs).Count;
            }
            else if (pairs is ICollection)
            {
                count = ((ICollection)pairs).Count;
            }
            else
            {
                count = 0;
            }
            Dictionary<TK, TV> dict;
            if (count == 0) dict = new Dictionary<TK, TV>(keyEqualityComparer);
            else dict = new Dictionary<TK, TV>(count, keyEqualityComparer);
            foreach (var item in pairs)
            {
                dict[item.Key] = item.Value;
            }
            return dict;
        }

        /// <summary>
        /// 默认创建的字典的函数
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="collection">包含要创建的值集合</param>
        /// <param name="dictToKeyFunc">使用值获取键的函数委托</param>
        /// <param name="keyEqualityComparer">字典的key比较器，null使用默认比较器</param>
        /// <returns>创建的字典</returns>
        /// <exception cref="ArgumentNullException">集合或委托是null</exception>
        public static Dictionary<TK, TV> CreateDictionaryByCollection<TK, TV>(IEnumerable<TV> collection, Func<TV, TK> dictToKeyFunc, IEqualityComparer<TK> keyEqualityComparer)
        {
            if (collection is null || dictToKeyFunc is null) throw new ArgumentNullException();

            int count;
            if (collection is ICollection<TV>)
            {
                count = ((ICollection<KeyValuePair<TK, TV>>)collection).Count;
            }
            else if (collection is ICollection)
            {
                count = ((ICollection)collection).Count;
            }
            else
            {
                count = 0;
            }

            Dictionary<TK, TV> dict;
            if (count == 0) dict = new Dictionary<TK, TV>(keyEqualityComparer);
            else dict = new Dictionary<TK, TV>(count, keyEqualityComparer);
            foreach (var item in collection)
            {
                dict[dictToKeyFunc.Invoke(item)] = item;
            }
            return dict;
        }

    }

}