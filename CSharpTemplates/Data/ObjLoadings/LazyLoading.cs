using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure
{

    /// <summary>
    /// 可使用<paramref name="key"/>重复加载对象的函数
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TObj"></typeparam>
    /// <param name="key">用于加载对象的唯一id</param>
    /// <param name="obj">加载的对象</param>
    /// <returns>成功加载返回true，如果标识符<paramref name="key"/>有错误或加载失败则返回false</returns>
    public delegate bool LoadToObject<TKey, TObj>(TKey key, out TObj obj);

    /// <summary>
    /// 懒加载池
    /// </summary>
    /// <remarks>
    /// <para>封装一个加载机制和对象缓存池，用于在第一次获取对象时加载，并保存在对象池中以便以后快速查找相同项</para>
    /// </remarks>
    /// <typeparam name="TKey">对象的唯一id类型，每一个加载的对象对应一个唯一值</typeparam>
    /// <typeparam name="TObj">被加载的对象类型</typeparam>
    public sealed class LazyLoadPool<TKey, TObj> : IEnumerable<KeyValuePair<TKey, TObj>>
    {

        #region 构造

        /// <summary>
        /// 实例化一个懒加载池
        /// </summary>
        /// <param name="loadToObject">提供加载对象的函数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public LazyLoadPool(LoadToObject<TKey, TObj> loadToObject)
        {
            p_load = loadToObject ?? throw new ArgumentNullException();
            p_dict = new Dictionary<TKey, TObj>();
        }

        /// <summary>
        /// 实例化一个懒加载池
        /// </summary>
        /// <param name="loadToObject">提供加载对象的函数</param>
        /// <param name="keyComparer">提供对象唯一id的比较器，null则使用默认实现</param>
        /// <exception cref="ArgumentNullException">加载函数是null</exception>
        public LazyLoadPool(LoadToObject<TKey, TObj> loadToObject, IEqualityComparer<TKey> keyComparer)
        {
            p_load = loadToObject ?? throw new ArgumentNullException();
            p_dict = new Dictionary<TKey, TObj>(keyComparer);
        }

        /// <summary>
        /// 实例化一个懒加载池
        /// </summary>
        /// <param name="loadToObject">提供加载对象的函数</param>
        /// <param name="keyComparer">提供对象唯一id的比较器，null则使用默认实现</param>
        /// <param name="capacity">指定缓存池字典的初始容量</param>
        /// <exception cref="ArgumentNullException">加载函数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">容量小于0</exception>
        public LazyLoadPool(LoadToObject<TKey, TObj> loadToObject, IEqualityComparer<TKey> keyComparer, int capacity)
        {
            p_load = loadToObject ?? throw new ArgumentNullException();
            p_dict = new Dictionary<TKey, TObj>(capacity, keyComparer);
        }

        #endregion

        #region 参数

        private readonly LoadToObject<TKey, TObj> p_load;

        private Dictionary<TKey, TObj> p_dict;

        #endregion

        #region 功能

        /// <summary>
        /// 使用<paramref name="key"/>加载对应的对象
        /// </summary>
        /// <param name="key">要使用的对象唯一id</param>
        /// <returns>加载的对象</returns>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="NotImplementedException">无法加载</exception>
        public TObj Load(TKey key)
        {
            TObj obj;
            if(!p_dict.TryGetValue(key, out obj))
            {
                //未加载
                if(!p_load.Invoke(key, out obj))
                {
                    throw new NotImplementedException();
                }
                p_dict[key] = obj;
            }
            return obj;
        }

        /// <summary>
        /// 使用<paramref name="key"/>加载对应的对象
        /// </summary>
        /// <param name="key">要使用的对象唯一id</param>
        /// <param name="obj">加载的对象</param>
        /// <returns>如果成功加载返回true；如果标识符<paramref name="key"/>有错误或加载失败则返回false</returns>
        /// <exception cref="ArgumentNullException">key是null</exception>
        public bool TryLoad(TKey key, out TObj obj)
        {
            if (!p_dict.TryGetValue(key, out obj))
            {
                //未加载
                if (!p_load.Invoke(key, out obj))
                {
                    return false;
                }
                p_dict[key] = obj;
            }
            return true;
        }

        /// <summary>
        /// 使用<paramref name="key"/>加载对应的对象，提供线程安全保护
        /// </summary>
        /// <param name="key">要使用的对象唯一id</param>
        /// <returns>加载的对象</returns>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="NotImplementedException">无法加载</exception>
        public TObj SafeLoad(TKey key)
        {
            TObj obj;
            if (!p_dict.TryGetValue(key, out obj))
            {
                //未加载
                lock (p_dict)
                {
                    if (!p_load.Invoke(key, out obj))
                    {
                        throw new NotImplementedException();
                    }
                    p_dict[key] = obj;
                }
            }
            return obj;
        }

        /// <summary>
        /// 使用<paramref name="key"/>加载对应的对象，提供线程安全保护
        /// </summary>
        /// <param name="key">要使用的对象唯一id</param>
        /// <param name="obj">加载的对象</param>
        /// <returns>如果成功加载返回true；如果标识符<paramref name="key"/>有错误或加载失败则返回false</returns>
        /// <exception cref="ArgumentNullException">key是null</exception>
        public bool SafeTryLoad(TKey key, out TObj obj)
        {
            if (!p_dict.TryGetValue(key, out obj))
            {
                lock (p_load)
                {
                    if (!p_load.Invoke(key, out obj))
                    {
                        return false;
                    }
                    p_dict[key] = obj;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取一个循环访问已加载项的枚举器
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, TObj>.Enumerator GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        /// <summary>
        /// 保存所有已加载项的唯一id集合
        /// </summary>
        public Dictionary<TKey, TObj>.KeyCollection Keys
        {
            get => p_dict.Keys;
        }

        /// <summary>
        /// 保存所有已加载项的集合
        /// </summary>
        public Dictionary<TKey, TObj>.ValueCollection Objs
        {
            get => p_dict.Values;
        }

        /// <summary>
        /// 获取当前加载器中已加载的对象数量
        /// </summary>
        public int LoadCount
        {
            get => p_dict.Count;
        }

        /// <summary>
        /// 从已加载缓存清空所有已加载项
        /// </summary>
        public void ClearAllObj()
        {
            p_dict.Clear();
        }

        /// <summary>
        /// 遍历当前所有已加载的项
        /// </summary>
        /// <param name="action">对每个键值对调用的函数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ForeachObj(Action<KeyValuePair<TKey, TObj>> action)
        {
            if (action is null) throw new ArgumentNullException();
            foreach (var pair in p_dict)
            {
                action.Invoke(pair);
            }
        }

        IEnumerator<KeyValuePair<TKey, TObj>> IEnumerable<KeyValuePair<TKey, TObj>>.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        #endregion

    }

}