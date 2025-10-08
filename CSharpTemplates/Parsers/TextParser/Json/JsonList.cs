using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cheng.Json
{

    #region

    /// <summary>
    /// 表示一个集合类型的json对象
    /// </summary>
    public sealed class JsonList : JsonVariable, IList<JsonVariable>
    {

        #region 构造

        /// <summary>
        /// 实例化一个集合json对象
        /// </summary>
        public JsonList()
        {
            p_list = new List<JsonVariable>();
        }

        /// <summary>
        /// 实例化一个集合json对象
        /// </summary>
        /// <param name="capacity">指定初始容量</param>
        public JsonList(int capacity)
        {
            p_list = new List<JsonVariable>(capacity);
        }

        /// <summary>
        /// 实例化一个集合json对象
        /// </summary>
        /// <param name="list">指定的拷贝对象</param>
        public JsonList(JsonList list)
        {
            if (list is null) throw new ArgumentNullException();
            int length = list.Count;
            p_list = new List<JsonVariable>(length);
            for (int i = 0; i < length; i++)
            {
                p_list.Add(list[i].Clone());
            }
        }

        #endregion

        #region 参数
        internal List<JsonVariable> p_list;
        #endregion

        #region 派生
        
        public override JsonType DataType => JsonType.List;

        public override bool IsNull => false;

        public override object Data => this;

        public override JsonList Array => this;

        public override bool Equals(JsonVariable other)
        {
            return (object)this == (object)other;
        }

        public override int GetHashCode()
        {
            return BaseHashCode();
        }

        public override long GetHashCode64()
        {
            return BaseHashCode();
        }

        /// <summary>
        /// 使用json格式文本返回所有元素
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Count);

            sb.Append('[');

            for (int i = 0; i < Count; i++)
            {
                sb.Append(p_list[i].ToString());
                if (i != Count - 1) sb.Append(',');
            }

            sb.Append(']');

            return sb.ToString();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder(Count);

            sb.Append('[');

            for (int i = 0; i < Count; i++)
            {
                sb.Append(p_list[i].ToString(formatProvider));
                if (i != Count - 1) sb.Append(',');
            }

            sb.Append(']');

            return sb.ToString();
        }

        public override JsonVariable Clone()
        {
            return new JsonList(this);
        }

        #endregion

        #region 集合功能

        /// <summary>
        /// 获取元素数量
        /// </summary>
        public int Count => p_list.Count;

        /// <summary>
        /// 获取或设置集合的实际容量
        /// </summary>
        /// <value>设置此值不会改变元素数量，会改变实际容量；实际容量 = 元素数量 + 剩余空间</value>
        /// <exception cref="ArgumentOutOfRangeException">值小于元素数量</exception>
        /// <exception cref="OutOfMemoryException">没有足够的内存</exception>
        public int Capacity
        {
            get => p_list.Capacity;
            set
            {
                p_list.Capacity = value;
            }
        }

        bool ICollection<JsonVariable>.IsReadOnly => false;

        /// <summary>
        /// 使用索引访问或设置元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public JsonVariable this[int index]
        {
            get
            {
                return p_list[index];
            }
            set
            {
                p_list[index] = (value is null) ? JsonNull.Nullable : value;
            }
        }

        private void add(JsonVariable json)
        {
            p_list.Add(json ?? JsonNull.Nullable);
        }

        private void insert(int index, JsonVariable json)
        {
            p_list.Insert(index, json ?? JsonNull.Nullable);
            //if (json is null) p_list.Insert(index, JsonNull.Nullable);
            //else p_list.Insert(index, json ?? JsonNull.Nullable);
        }

        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="json">要添加的元素</param>
        public void Add(JsonVariable json)
        {
            add(json);
        }

        /// <summary>
        /// 添加一个整形元素
        /// </summary>
        /// <param name="value">要添加的元素</param>
        public void Add(long value)
        {
            p_list.Add(new JsonInteger(value));
        }

        /// <summary>
        /// 添加一个浮点型元素
        /// </summary>
        /// <param name="value"></param>
        public void Add(double value)
        {
            p_list.Add(new JsonRealNumber(value));
        }

        /// <summary>
        /// 添加一个布尔类型元素
        /// </summary>
        /// <param name="value"></param>
        public void Add(bool value)
        {
            p_list.Add(new JsonBoolean(value));
        }

        /// <summary>
        /// 添加一个字符串
        /// </summary>
        /// <param name="value"></param>
        public void Add(string value)
        {
            if (value is null) AddNull();
            else p_list.Add(new JsonString(value));
        }

        /// <summary>
        /// 添加一个空元素
        /// </summary>
        public void AddNull()
        {
            p_list.Add(JsonNull.Nullable);
        }

        /// <summary>
        /// 在指定位置插入元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="json">元素</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void Insert(int index, JsonVariable json)
        {
            insert(index, json);
        }

        /// <summary>
        /// 清空所有元素
        /// </summary>
        public void Clear()
        {
            p_list.Clear();
        }

        /// <summary>
        /// 删除指定索引的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void RemoveAt(int index)
        {
            p_list.RemoveAt(index);
        }

        /// <summary>
        /// 删除指定索引的元素序列
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">要删除的元素个数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveAt(int index, int count)
        {
            p_list.RemoveRange(index, count);
        }

        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="json">要删除的元素</param>
        /// <returns>成功删除返回true，删除失败返回false</returns>
        public bool Remove(JsonVariable json)
        {
            if (json is null) return false;
            return p_list.Remove(json);
        }

        public int IndexOf(JsonVariable json)
        {
            return p_list.IndexOf(json);
        }

        /// <summary>
        /// 按谓词搜索匹配项并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>第一个匹配项的索引，否则为-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public int FindIndex(Predicate<JsonVariable> match)
        {
            return p_list.FindIndex(match);
        }

        public bool Contains(JsonVariable json)
        {
            return p_list.Contains(json);
        }

        public void CopyTo(JsonVariable[] array, int arrayIndex)
        {
            p_list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 将该数组的所有元素返回到新数组中
        /// </summary>
        /// <returns></returns>
        public JsonVariable[] ToArray()
        {
            return p_list.ToArray();
        }

        public IEnumerator<JsonVariable> GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        #endregion

    }

    #endregion

}
