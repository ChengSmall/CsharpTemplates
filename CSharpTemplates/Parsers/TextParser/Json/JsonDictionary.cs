using Cheng.DataStructure.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// 表示一个键值对集合类型的json对象
    /// </summary>
    public sealed class JsonDictionary : JsonVariable, IDictionary<string, JsonVariable>, IReadOnlyDictionary<string, JsonVariable>
    {

        #region 构造

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        public JsonDictionary()
        {
            p_dict = new Dictionary<string, JsonVariable>(Cheng.DataStructure.Collections.BinaryStringEqualComparer.Default);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="capacity">指定初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>小于0</exception>
        public JsonDictionary(int capacity)
        {
            p_dict = new Dictionary<string, JsonVariable>(capacity, Cheng.DataStructure.Collections.BinaryStringEqualComparer.Default);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="comparer">指定key的比较器和哈希算法，null使用默认的值比较器</param>
        public JsonDictionary(IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, JsonVariable>(comparer ?? BinaryStringEqualComparer.Default);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="capacity">指定初始容量</param>
        /// <param name="comparer">指定key的比较器和哈希算法，null使用默认的值比较器</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparer"/> 小于 0</exception>
        public JsonDictionary(int capacity, IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, JsonVariable>(capacity, comparer ?? BinaryStringEqualComparer.Default);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="json">指定的拷贝对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public JsonDictionary(JsonDictionary json)
        {
            if (json is null) throw new ArgumentNullException();
            p_dict = new Dictionary<string, JsonVariable>(json, json.p_dict.Comparer);

            foreach (var pair in json.p_dict)
            {
                add(pair.Key, pair.Value.Clone());
            }
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="pairs">指定要拷贝的键值对集合对象</param>
        /// <param name="comparer">指定key的比较器和哈希算法；null使用默认的值比较器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public JsonDictionary(IEnumerable<KeyValuePair<string, JsonVariable>> pairs, IEqualityComparer<string> comparer)
        {
            if (pairs is null) throw new ArgumentNullException();
            int count;
            if(pairs is ICollection<KeyValuePair<string, JsonVariable>>)
            {
                count = ((ICollection<KeyValuePair<string, JsonVariable>>)pairs).Count;
            }
            else if (pairs is ICollection)
            {
                count = ((ICollection)pairs).Count;
            }
            else
            {
                count = 0;
            }

            p_dict = new Dictionary<string, JsonVariable>(count, comparer ?? BinaryStringEqualComparer.Default);

            foreach (var pair in pairs)
            {
                add(pair.Key, pair.Value?.Clone());
            }
        }

        #endregion

        #region 参数
        internal Dictionary<string, JsonVariable> p_dict;
        #endregion

        #region 键值对功能

        /// <summary>
        /// 获取键值对的元素数量
        /// </summary>
        public int Count => p_dict.Count;

        private void add(string key, JsonVariable json)
        {
            p_dict.Add(key, json ?? JsonNull.Nullable);
        }

        private void set(string key, JsonVariable json)
        {
            p_dict[key] = json ?? JsonNull.Nullable;
        }

        /// <summary>
        /// 访问或设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <value>若键值对内没有指定的键，则添加一个键值对，若存在键则覆盖旧值</value>
        /// <returns>返回对应键所在的值</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        /// <exception cref="KeyNotFoundException">获取的键不存在</exception>
        public JsonVariable this[string key]
        {
            get => p_dict[key];
            
            set => set(key, value);
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="json">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        /// <exception cref="ArgumentException">已存在相同的键</exception>
        public void Add(string key, JsonVariable json)
        {
            add(key, json);
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, long value)
        {
            p_dict.Add(key, new JsonInteger(value));
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, double value)
        {
            p_dict.Add(key, new JsonRealNumber(value));
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, int value)
        {
            p_dict.Add(key, new JsonInteger(value));
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, float value)
        {
            p_dict.Add(key, new JsonRealNumber(value));
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="json">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, bool value)
        {
            p_dict.Add(key, JsonVariable.GetBooleanValue(value));
        }

        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="json">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, string value)
        {
            p_dict.Add(key, new JsonString(value));
        }

        /// <summary>
        /// 添加一个值为空的键值对
        /// </summary>
        /// <param name="key">添加的键</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void AddNull(string key)
        {
            p_dict.Add(key, JsonNull.Nullable);
        }

        /// <summary>
        /// 删除一堆指定的键值
        /// </summary>
        /// <param name="key">要删除的键</param>
        /// <returns>是否成功删除</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public bool Remove(string key)
        {
            return p_dict.Remove(key);
        }

        /// <summary>
        /// 清空所有键值对元素
        /// </summary>
        public void Clear()
        {
            p_dict.Clear();
        }

        public ICollection<string> Keys => p_dict.Keys;

        public ICollection<JsonVariable> Values => p_dict.Values;

        /// <summary>
        /// 获取用于确定字典中的键是否相等的比较器
        /// </summary>
        public IEqualityComparer<string> Comparer
        {
            get => p_dict.Comparer;
        }

        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="json">获取的值</param>
        /// <returns>获取成功返回true，无法找到返回false</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public bool TryGetValue(string key, out JsonVariable json)
        {
            return p_dict.TryGetValue(key, out json);
        }

        /// <summary>
        /// 判断指定键是否存在
        /// </summary>
        /// <param name="key">判断的键</param>
        /// <returns>存在返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public bool ContainsKey(string key)
        {
            return p_dict.ContainsKey(key);
        }

        /// <summary>
        /// 舍去多余的存储空间
        /// </summary>
        public void TrimExcess()
        {
            p_dict = new Dictionary<string, JsonVariable>(p_dict, p_dict.Comparer);
        }

        /// <summary>
        /// 返回一个可循环访问<see cref="JsonDictionary"/>的枚举器
        /// </summary>
        /// <returns>可循环访问当前对象的枚举器</returns>
        public Dictionary<string, JsonVariable>.Enumerator GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        #region

        void ICollection<KeyValuePair<string, JsonVariable>>.Add(KeyValuePair<string, JsonVariable> item)
        {
            add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, JsonVariable>>.Contains(KeyValuePair<string, JsonVariable> item)
        {
            return p_dict.ContainsKey(item.Key);
        }
        void ICollection<KeyValuePair<string, JsonVariable>>.CopyTo(KeyValuePair<string, JsonVariable>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, JsonVariable>>)p_dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, JsonVariable>>.Remove(KeyValuePair<string, JsonVariable> item)
        {
            return p_dict.Remove(item.Key);
        }

        IEnumerator<KeyValuePair<string, JsonVariable>> IEnumerable<KeyValuePair<string, JsonVariable>>.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, JsonVariable>>.IsReadOnly => ((ICollection<KeyValuePair<string, JsonVariable>>)p_dict).IsReadOnly;

        #endregion

        #endregion

        #region 派生

        public override JsonType DataType => JsonType.Dictionary;

        public override object Data => this;

        public override bool IsNull => false;

        public override JsonDictionary JsonObject => this;

        IEnumerable<string> IReadOnlyDictionary<string, JsonVariable>.Keys => p_dict.Keys;

        IEnumerable<JsonVariable> IReadOnlyDictionary<string, JsonVariable>.Values => p_dict.Values;

        public override int GetHashCode()
        {
            return BaseHashCode();
        }

        public override long GetHashCode64()
        {
            return BaseHashCode();
        }

        public override bool Equals(JsonVariable other)
        {
            return (object)this == (object)other;
        }

        /// <summary>
        /// 返回一个新的相同键值对实例
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, JsonVariable> ToDictionary()
        {
            return new Dictionary<string, JsonVariable>(p_dict);
        }

        public override JsonVariable Clone()
        {
            return new JsonDictionary(this);
        }

        public override string ToString()
        {
            StringBuilder sb;
            int len = Count;
            if (len == 0) return "{}";
            sb = new StringBuilder(len * 4);
            int cd = 0;
            sb.Append('{');
            foreach (var item in p_dict)
            {
                sb.Append('"')
                    .Append(item.Key)
                    .Append('"')
                    .Append(':')
                    .Append(item.Value.ToString())
                    ;
                cd++;
                if (cd != len) sb.Append(',');
            }
            sb.Append('}');
            return sb.ToString();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return ToString();
        }

        #endregion

    }

}
