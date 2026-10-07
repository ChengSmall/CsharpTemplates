

using System;
using System.Collections.Generic;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 提供特定委托对象的默认实现方法
    /// </summary>
    public static class DeleagateFunctions
    {

        #region 字典创建

        /// <summary>
        /// 委托对象<see cref="Cheng.DataStructure.CreateDictionaryByPairs{K, T}"/>的默认实现
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="pairs"></param>
        /// <param name="keyEqualityComparer"></param>
        /// <returns>类型为<see cref="Dictionary{TKey, TValue}"/>的对象</returns>
        public static IReadOnlyDictionary<K, T> CreateDictionaryByPairs<K, T>(IEnumerable<KeyValuePair<K, T>> pairs, IEqualityComparer<K> keyEqualityComparer)
        {
            
            if (pairs is null) throw new ArgumentNullException();
            var dict = new Dictionary<K, T>(keyEqualityComparer);
            foreach (var pair in pairs)
            {
                dict[pair.Key] = pair.Value;
            }
            return dict;
        }

        /// <summary>
        /// 委托对象<see cref="Cheng.DataStructure.CreateDictionaryByCollection{K, T}"/>的默认实现
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="dictToKeyFunc"></param>
        /// <param name="keyEqualityComparer"></param>
        /// <returns>类型为<see cref="Dictionary{TKey, TValue}"/>的对象</returns>
        public static IReadOnlyDictionary<K, T> CreateDictionaryByCollection<K, T>(IEnumerable<T> collection, Func<T, K> dictToKeyFunc, IEqualityComparer<K> keyEqualityComparer)
        {
            if (collection is null || dictToKeyFunc is null) throw new ArgumentNullException();
            var dict = new Dictionary<K, T>(keyEqualityComparer);
            foreach (var item in collection)
            {
                dict[dictToKeyFunc.Invoke(item)] = item;
            }
            return dict;
        }

        #endregion

    }

}