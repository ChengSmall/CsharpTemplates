using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// 表示一个键值对集合类型的json对象
    /// </summary>
    public sealed class JsonDictionary : JsonVariable, IDictionary<string, JsonVariable>
    {

        #region 构造

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        public JsonDictionary()
        {
            p_dict = new Dictionary<string, JsonVariable>();
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="capacity">指定初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>小于0</exception>
        public JsonDictionary(int capacity)
        {
            p_dict = new Dictionary<string, JsonVariable>(capacity);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="comparer">指定key的比较器和哈希算法</param>
        public JsonDictionary(IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, JsonVariable>(comparer);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="capacity">指定初始容量</param>
        /// <param name="comparer">指定key的比较器和哈希算法</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="comparer"/> 小于 0</exception>
        public JsonDictionary(int capacity, IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, JsonVariable>(capacity, comparer);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="json">指定的拷贝对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public JsonDictionary(JsonDictionary json)
        {
            p_dict = new Dictionary<string, JsonVariable>(json?.p_dict, json?.p_dict.Comparer);
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
            p_dict.Add(key, new JsonBoolean(value));
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

        public IEnumerator<KeyValuePair<string, JsonVariable>> GetEnumerator()
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
        /// 以格式返回所有元素
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            int count = 0;
            int length = Count;
            StringBuilder sb = new StringBuilder(length * 2);
            sb.Append("{");
            foreach (var item in this)
            {
                count++;
                sb.Append("\"" + item.Key + "\"");
                sb.Append(':');
                sb.Append(item.Value.ToString());
                if(count != length) sb.Append(',');
            }
            sb.Append('}');
            return sb.ToString();

        }

        public override string ToString(IFormatProvider formatProvider)
        {
            int count = 0;
            int length = Count;
            StringBuilder sb = new StringBuilder(length * 2);
            sb.Append("{");
            foreach (var item in this)
            {
                count++;
                sb.Append("\"" + item.Key.ToString(formatProvider) + "\"");
                sb.Append(':');
                sb.Append(item.Value.ToString(formatProvider));
                if (count != length) sb.Append(',');
            }
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// 返回一个新的相同键值对实例
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, JsonVariable> ToDictionary()
        {
            return new Dictionary<string, JsonVariable>(p_dict);
        }

        #endregion

    }


}
