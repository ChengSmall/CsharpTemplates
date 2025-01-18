using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// json对象的数据类型
    /// </summary>
    public enum JsonType : byte
    {

        /// <summary>
        /// 整数
        /// </summary>
        Integer = 1,

        /// <summary>
        /// 浮点型小数
        /// </summary>
        RealNum,

        /// <summary>
        /// 布尔值
        /// </summary>
        Boolean,

        /// <summary>
        /// 字符串
        /// </summary>
        String,

        /// <summary>
        /// 空对象null
        /// </summary>
        Null,

        /// <summary>
        /// 数据集合 [value1, value2, value3]
        /// </summary>
        List,

        /// <summary>
        /// 键值对集合 {"key":value}
        /// </summary>
        Dictionary
    }

    /// <summary>
    /// json数据对象的基类
    /// </summary>
    public abstract class JsonVariable : IEquatable<JsonVariable>
    {

        #region 参数
        /// <summary>
        /// 获取Json对象的数据类型
        /// </summary>
        public abstract JsonType DataType { get; }

        const string NotDataTypeException = "不是此数据类型";

        /// <summary>
        /// 访问或设置整数
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual long Integer
        {
            get => throw new NotImplementedException(NotDataTypeException);
            set => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问或设置实数
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual double RealNum
        {
            get => throw new NotImplementedException(NotDataTypeException);
            set => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问或设置布尔值
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual bool Boolean
        {
            get => throw new NotImplementedException(NotDataTypeException);
            set => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问或设置字符串
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual string String
        {
            get => throw new NotImplementedException(NotDataTypeException);
            set => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问json键值对对象
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual JsonDictionary JsonObject
        {
            get => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问json集合对象
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual JsonList Array
        {
            get => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 此实例数据是否为空
        /// </summary>
        public virtual bool IsNull
        {
            get => DataType == JsonType.Null;
        }

        /// <summary>
        /// 访问此实例的数据
        /// </summary>
        /// <returns>
        /// <para>该内容取决于Json对象的<see cref="DataType"/>值；</para>
        /// <para>
        /// 类型为<see cref="JsonType.Integer"/>时该值为<see cref="long"/>；<see cref="JsonType.RealNum"/>是<see cref="double"/>；<see cref="JsonType.Boolean"/>是<see cref="bool"/>；<see cref="JsonType.String"/>是<see cref="string"/>；<see cref="JsonType.Null"/>表示空引用null；<see cref="JsonType.List"/>是<see cref="JsonList"/>；<see cref="JsonType.Dictionary"/>是<see cref="JsonDictionary"/>
        /// </para>
        /// </returns>
        public virtual object Data
        {
            get
            {
                JsonType type = DataType;
                switch (type)
                {
                    case JsonType.Integer:
                        return Integer;
                    case JsonType.RealNum:
                        return RealNum;
                    case JsonType.Boolean:
                        return Boolean;
                    case JsonType.String:
                        return String;
                    case JsonType.Null:
                        return null;
                    default:
                        return this;
                }
            }
        }
        #endregion

        #region 功能

        #region 派生

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is JsonVariable)
            {
                return Equals((JsonVariable)obj);
            }
            if(obj == null)
            {
                return DataType == JsonType.Null;
            }
            return false;
        }

        /// <summary>
        /// 返回此json数据的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            switch (DataType)
            {
                case JsonType.Integer:
                    return Integer.GetHashCode();
                case JsonType.RealNum:
                    return RealNum.GetHashCode();
                case JsonType.Boolean:
                    return Boolean.GetHashCode();
                case JsonType.String:
                    return String.GetHashCode();
                case JsonType.Null:
                    return 0;
                default:
                    return base.GetHashCode();
            }
        }

        /// <summary>
        /// 返回json对象的数据字符串表现形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (DataType)
            {
                case JsonType.Integer:
                    return Integer.ToString();
                case JsonType.RealNum:
                    return RealNum.ToString();
                case JsonType.Boolean:
                    return Boolean.ToString();
                case JsonType.String:
                    return String;
                case JsonType.Null:
                    return string.Empty;
                default:
                    return base.ToString();
            }
        }

        /// <summary>
        /// 使用指定区域性格式返回json对象的字符串
        /// </summary>
        /// <param name="formatProvider">提供区域性特定的格式设置信息</param>
        /// <returns></returns>
        public virtual string ToString(IFormatProvider formatProvider)
        {
            switch (DataType)
            {
                case JsonType.Integer:
                    return Integer.ToString(formatProvider);
                case JsonType.RealNum:
                    return RealNum.ToString(formatProvider);
                case JsonType.Boolean:
                    return Boolean.ToString(formatProvider);
                case JsonType.String:
                    return String.ToString(formatProvider);
                case JsonType.Null:
                    return string.Empty;
                default:
                    return base.ToString();
            }
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(JsonVariable other)
        {
            if (other is null) return DataType == JsonType.Null;
            JsonType type = other.DataType;
            if (type != DataType) return false;

            switch (type)
            {
                case JsonType.Integer:
                    return Integer == other.Integer;
                case JsonType.RealNum:
                    return RealNum == other.RealNum;
                case JsonType.Boolean:
                    return Boolean == other.Boolean;
                case JsonType.String:
                    return String == other.String;
                case JsonType.Null:
                    return true;
                default:
                    return (object)this == (object)other;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// 原始默认哈希代码
        /// </summary>
        /// <returns></returns>
        protected int BaseHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <returns></returns>
        public static bool operator ==(JsonVariable j1, JsonVariable j2)
        {
            if ((object)j1 == (object)j2) return true;
            if (j1 is null || j2 is null)
            {
                if(j1 is null)
                {
                    return j2.DataType == JsonType.Null;
                }
                else
                {
                    return j1.DataType == JsonType.Null;
                }
            }
            return j1.Equals(j2);
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="j1"></param>
        /// <param name="j2"></param>
        /// <returns></returns>
        public static bool operator !=(JsonVariable j1, JsonVariable j2)
        {
            if ((object)j1 == (object)j2) return false;
            if (j1 is null || j2 is null)
            {
                if (j1 is null)
                {
                    return j2.DataType != JsonType.Null;
                }
                else
                {
                    return j1.DataType != JsonType.Null;
                }
            }
            return !j1.Equals(j2);
        }

        #endregion

        #endregion

    }

    #region json对象

    /// <summary>
    /// 表示一个null类型的json对象
    /// </summary>
    /// <remarks>
    /// 使用此类型实例化的所有对象都表示同一个值，建议使用<see cref="JsonNull.Nullable"/>只读对象做为json数据结构的所有null对象
    /// </remarks>
    public sealed class JsonNull : JsonVariable
    {

        /// <summary>
        /// 表示null的json对象
        /// </summary>
        public readonly static JsonNull Nullable = new JsonNull();

        public JsonNull()
        {
        }

        /// <summary>
        /// Json的null对象文本
        /// </summary>
        public const string JsonText = "null";

        public override JsonType DataType => JsonType.Null;
        public override bool IsNull => true;
        public override object Data => null;

        public override string ToString()
        {
            return JsonText;
        }

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns>当对象类型为<see cref="JsonType.Null"/>时返回true，若对象实例为null，同样返回true</returns>
        public override bool Equals(JsonVariable other)
        {
            return (other is null) || other.IsNull;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return JsonText.ToString(formatProvider);
        }
    }

    /// <summary>
    /// 表示整数类型的json对象
    /// </summary>
    public sealed class JsonInteger : JsonVariable
    {
        #region 构造
        /// <summary>
        /// 实例化一个整数json对象
        /// </summary>
        public JsonInteger()
        {
            value = 0;
        }
        /// <summary>
        /// 实例化一个整数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonInteger(long value)
        {
            this.value = value;
        }
        #endregion

        #region 参数
        /// <summary>
        /// 数据
        /// </summary>
        public long value;
        #endregion

        #region 派生

        public override JsonType DataType => JsonType.Integer;

        public override long Integer
        {
            get => value;
            set
            {
                this.value = value;
            }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;
            if (other.DataType != JsonType.Integer) return false;
            return value == other.Integer; 
        }

        public override bool IsNull => false;

        public override string ToString()
        {
            return value.ToString();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value.ToString(formatProvider);
        }

        #endregion

    }

    /// <summary>
    /// 表示小数类型的json对象
    /// </summary>
    public sealed class JsonRealNumber : JsonVariable
    {

        #region 构造

        /// <summary>
        /// 实例化一个小数json对象
        /// </summary>
        public JsonRealNumber()
        {
            value = 0d;
        }

        /// <summary>
        /// 实例化一个小数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonRealNumber(double value)
        {
            this.value = value;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 数据
        /// </summary>
        public double value;

        #endregion

        #region 派生

        public override JsonType DataType => JsonType.RealNum;

        public override double RealNum
        {
            get => value; set => this.value = value;
        }

        public override bool IsNull => false;

        public override object Data => value;

        public override string ToString()
        {
            return value.ToString();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value.ToString(formatProvider);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;

            if (other.DataType != JsonType.RealNum) return false;

            return value == other.RealNum;
        }

        #endregion

    }

    /// <summary>
    /// 表示布尔类型的json对象
    /// </summary>
    public sealed class JsonBoolean : JsonVariable
    {

        #region 构造
        /// <summary>
        /// 实例化一个布尔类型json对象
        /// </summary>
        public JsonBoolean() { }
        /// <summary>
        /// 实例化一个布尔类型json对象
        /// </summary>
        /// <param name="value">指定值</param>
        public JsonBoolean(bool value)
        {
            this.value = value;
        }
        #endregion

        #region 参数
        /// <summary>
        /// 数据
        /// </summary>
        public bool value;
        /// <summary>
        /// 值为true时的json文本
        /// </summary>
        public const string TrueJsonText = "true";
        /// <summary>
        /// 值为false时的json文本
        /// </summary>
        public const string FalseJsonText = "false";
        #endregion

        public override JsonType DataType => JsonType.Boolean;
        public override bool Boolean
        {
            get => value;
            set => this.value = value;
        }
        public override object Data => value;
        public override bool IsNull => false;
        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;

            if (other.DataType != JsonType.Boolean) return false;

            return value == other.Boolean;
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return value ? TrueJsonText : FalseJsonText;
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value ? TrueJsonText.ToString(formatProvider) : FalseJsonText.ToString(formatProvider);            
        }

    }

    /// <summary>
    /// 表示字符串类型的json对象
    /// </summary>
    public sealed class JsonString : JsonVariable
    {
        #region 构造
        /// <summary>
        /// 实例化字符串类型的json对象
        /// </summary>
        public JsonString()
        {
            value = string.Empty;
        }
        /// <summary>
        /// 实例化字符串类型的json对象
        /// </summary>
        /// <param name="value">指定的字符串</param>
        public JsonString(string value)
        {
            this.value = (value is null) ? string.Empty : value;
        }
        #endregion

        #region 参数

        private string value;
        /// <summary>
        /// 访问或设置字符串数据
        /// </summary>
        public string Value
        {
            get => value;
            set
            {
                this.value = (value is null) ? string.Empty : value;
            }
        }
        #endregion

        #region 派生
        public override JsonType DataType => JsonType.String;
        /// <summary>
        /// 访问或设置字符串数据
        /// </summary>
        public override string String
        {
            get => value;
            set => this.value = (value is null) ? string.Empty : value;
        }
        public override object Data => value;
        public override bool IsNull => false;
        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;

            if (other.DataType != JsonType.String) return false;

            return value == other.String;
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return "\"" + value + "\"";
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return "\"" + value.ToString(formatProvider) + "\"";            
        }
        #endregion
    }

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
            p_list = new List<JsonVariable>(list);
        }
        #endregion

        #region 参数
        private List<JsonVariable> p_list;
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
            if (json is null) p_list.Add(JsonNull.Nullable);
            else p_list.Add(json);
        }

        private void insert(int index, JsonVariable json)
        {
            if (json is null) p_list.Insert(index, JsonNull.Nullable);
            else p_list.Insert(index, json);
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
            p_list.Add(new JsonString(value));
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
        public JsonDictionary(int capacity, IEqualityComparer<string> comparer)
        {
            p_dict = new Dictionary<string, JsonVariable>(capacity, comparer);
        }

        /// <summary>
        /// 实例化一个键值对类型的json对象
        /// </summary>
        /// <param name="json">指定的拷贝对象</param>
        public JsonDictionary(JsonDictionary json)
        {
            p_dict = new Dictionary<string, JsonVariable>(json);
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
        /// <param name="json">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, long value)
        {
            p_dict.Add(key, new JsonInteger(value));
        }
        /// <summary>
        /// 添加一对键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="json">值</param>
        /// <exception cref="ArgumentNullException">key为null</exception>
        public void Add(string key, double value)
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

        public ICollection<string> Keys => p_dict.Keys;

        public ICollection<JsonVariable> Values => p_dict.Values;

        bool ICollection<KeyValuePair<string, JsonVariable>>.IsReadOnly => ((ICollection<KeyValuePair<string, JsonVariable>>)p_dict).IsReadOnly;

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

    #endregion

}
