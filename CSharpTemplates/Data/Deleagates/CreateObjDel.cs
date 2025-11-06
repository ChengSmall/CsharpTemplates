using System;
using System.Collections;
using System.Collections.Generic;




namespace Cheng.DataStructure
{

    /// <summary>
    /// 用于创建只读字典的委托
    /// </summary>
    /// <param name="collection">要创建字典的数据集合</param>
    /// <param name="dictToKeyFunc">通过集合元素创建对应字典的key</param>
    /// <param name="keyEqualityComparer">指定字典的key比较器和哈希获取值函数</param>
    /// <returns>从<paramref name="collection"/>创建的只读字典</returns>
    public delegate System.Collections.Generic.IReadOnlyDictionary<K, T> CreateDictionaryByCollection<K, T>(IEnumerable<T> collection, Func<T, K> dictToKeyFunc, IEqualityComparer<K> keyEqualityComparer);

    /// <summary>
    /// 用于创建只读字典的委托
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="pairs">要创建字典的键值对数据集合</param>
    /// <param name="keyEqualityComparer">指定字典的key比较器和哈希获取值函数</param>
    /// <returns>从<paramref name="pairs"/>创建的只读字典</returns>
    public delegate System.Collections.Generic.IReadOnlyDictionary<K, T> CreateDictionaryByPairs<K, T>(IEnumerable<KeyValuePair<K, T>> pairs, IEqualityComparer<K> keyEqualityComparer);

}
