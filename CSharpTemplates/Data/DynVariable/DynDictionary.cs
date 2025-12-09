using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Collections;
using Cheng.Algorithm;
using Cheng.Algorithm.Sorts;

namespace Cheng.DataStructure.DynamicVariables
{

    /// <summary>
    /// 字典对象
    /// </summary>
    public sealed class DynDictionary : DynamicObject, IDictionary<string, DynVariable>, System.Collections.Generic.IReadOnlyDictionary<string, DynVariable>
    {

        #region 初始化

        /// <summary>
        /// 实例化字典对象
        /// </summary>
        public DynDictionary()
        {
            p_dict = new Dictionary<string, DynVariable>(BinaryStringEqualComparer.Default);
            p_open = true;
        }

        /// <summary>
        /// 实例化字典对象，指定初始容量
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException">容量参数小于0</exception>
        public DynDictionary(int capacity) : this(capacity, null)
        {
        }

        /// <summary>
        /// 实例化字典对象
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <param name="comparer">字典的key比较器，null表示一个默认的字符串值比较器</param>
        /// <exception cref="ArgumentOutOfRangeException">容量参数小于0</exception>
        public DynDictionary(int capacity, IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, DynVariable>(capacity, comparer ?? BinaryStringEqualComparer.Default);
            p_open = true;
        }

        /// <summary>
        /// 实例化字典对象
        /// </summary>
        /// <param name="pairs">初始化时要拷贝其中的键值对</param>
        /// <param name="comparer">字典的key比较器，null表示一个默认的字符串值比较器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public DynDictionary(IEnumerable<KeyValuePair<string, DynVariable>> pairs, IEqualityComparer<string> comparer)
        {
            if (pairs is null) throw new ArgumentNullException();
            p_open = true;

            int count;
            if (pairs is ICollection<KeyValuePair<string, DynVariable>>)
            {
                count = ((ICollection<KeyValuePair<string, DynVariable>>)pairs).Count;
            }
            else
            {
                count = 4;
            }

            p_dict = new Dictionary<string, DynVariable>(count, comparer ?? BinaryStringEqualComparer.Default);

            foreach (var item in pairs)
            {
                p_dict[item.Key] = item.Value ?? EmptyValue;
            }
        }

        #endregion

        #region 参数

        internal Dictionary<string, DynVariable> p_dict;
        private bool p_open;

        #endregion

        #region 字典

        #region

        private void f_add(string key, DynVariable value)
        {
            if (!p_open) throw new NotSupportedException();
            if (key is null) throw new ArgumentNullException();
            if (key.Length > ushort.MaxValue) throw new ArgumentException();
            p_dict.Add(key, value ?? EmptyValue);
        }

        private void f_set(string key, DynVariable value)
        {
            if (!p_open) throw new NotSupportedException();
            if (key is null) throw new ArgumentNullException();
            if (key.Length > ushort.MaxValue) throw new ArgumentException();
            p_dict[key] = value ?? EmptyValue;
        }

        #endregion

        /// <summary>
        /// 该字典是否已被锁定
        /// </summary>
        public override bool Locked => !p_open;

        /// <summary>
        /// 将字典锁定
        /// </summary>
        /// <remarks>
        /// <para>锁定后的字典将变成只读，无法修改字典内容</para>
        /// </remarks>
        public override void OnLock()
        {
            p_open = false;
            p_dict = new Dictionary<string, DynVariable>(p_dict, p_dict.Comparer);
        }

        public ICollection<string> Keys => p_dict.Keys;

        public ICollection<DynVariable> Values => p_dict.Values;

        /// <summary>
        /// 字典内用于查询字符串的比较器
        /// </summary>
        public IEqualityComparer<string> Comparer
        {
            get => p_dict.Comparer;
        }

        public DynVariable this[string key]
        {
            get
            {
                return p_dict[key];
            }
            set
            {
                f_set(key, value);
            }
        }

        public int Count => p_dict.Count;

        public void Add(string key, DynVariable value)
        {
            f_add(key, value);
        }

        /// <summary>
        /// 添加一个32位整数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, int value)
        {
            f_add(key, CreateInt32(value));
        }

        /// <summary>
        /// 添加一个64位整数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, long value)
        {
            f_add(key, CreateInt64(value));
        }

        /// <summary>
        /// 添加一个单浮点数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, float value)
        {
            f_add(key, CreateFloat(value));
        }

        /// <summary>
        /// 添加一个双浮点数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, double value)
        {
            f_add(key, CreateDouble(value));
        }

        /// <summary>
        /// 添加一个字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, string value)
        {
            f_add(key, value is null ? EmptyValue : CreateString(value));
        }

        /// <summary>
        /// 添加一个布尔值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void Add(string key, bool value)
        {
            f_add(key, value ? BooleanTrue : BooleanFalse);
        }

        /// <summary>
        /// 添加一个空值
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="NotSupportedException">对象已锁定</exception>
        /// <exception cref="ArgumentNullException">key是null</exception>
        /// <exception cref="ArgumentException">key的长度不属于范围内</exception>
        public void AddEmpty(string key)
        {
            f_add(key, EmptyValue);
        }

        public bool ContainsKey(string key)
        {
            return p_dict.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (!p_open) throw new NotSupportedException();
            return p_dict.Remove(key);
        }

        public bool TryGetValue(string key, out DynVariable value)
        {
            return p_dict.TryGetValue(key, out value);
        }

        public void Clear()
        {
            if (!p_open) throw new NotSupportedException();
            p_dict.Clear();
        }

        /// <summary>
        /// 返回一个循环访问字典元素的枚举器
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DynVariable>.Enumerator GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, DynVariable>> IEnumerable<KeyValuePair<string, DynVariable>>.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_dict.GetEnumerator();
        }

        #region

        IEnumerable<string> System.Collections.Generic.IReadOnlyDictionary<string, DynVariable>.Keys => p_dict.Keys;

        IEnumerable<DynVariable> System.Collections.Generic.IReadOnlyDictionary<string, DynVariable>.Values => p_dict.Values;

        void ICollection<KeyValuePair<string, DynVariable>>.Add(KeyValuePair<string, DynVariable> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, DynVariable>>.Contains(KeyValuePair<string, DynVariable> item)
        {
            return ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<string, DynVariable>>.CopyTo(KeyValuePair<string, DynVariable>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, DynVariable>>)p_dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, DynVariable>>.Remove(KeyValuePair<string, DynVariable> item)
        {
            return Remove(item.Key);
        }

        bool ICollection<KeyValuePair<string, DynVariable>>.IsReadOnly => !p_open;

        #endregion

        #endregion

        #region 派生

        public override DynVariableType DynType => DynVariableType.Dictionary;

        public override DynDictionary DynamicDictionary => this;

        public override string ToString()
        {
            return nameof(DynDictionary);
        }

        public override DynVariable Clone()
        {
            DynDictionary d = new DynDictionary(p_dict.Count, p_dict.Comparer);
            foreach (var pair in p_dict)
            {
                d[pair.Key] = pair.Value.Clone();
            }
            return d;
        }

        #endregion

    }

}