using Cheng.Algorithm.HashCodes;
using Cheng.Memorys;
using System;

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
    public abstract class JsonVariable : IEquatable<JsonVariable>, IHashCode64, ICloneable
    {

        #region 参数

        /// <summary>
        /// 获取Json对象的数据类型
        /// </summary>
        public abstract JsonType DataType { get; }

        const string NotDataTypeException = "不是此数据类型";

        /// <summary>
        /// 访问整数
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual long Integer
        {
            get => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问实数
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual double RealNum
        {
            get => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问布尔值
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual bool Boolean
        {
            get => throw new NotImplementedException(NotDataTypeException);
        }

        /// <summary>
        /// 访问或设置字符串
        /// </summary>
        /// <exception cref="NotImplementedException">不是此类型数据</exception>
        public virtual string String
        {
            get => throw new NotImplementedException(NotDataTypeException);
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

        /// <summary>
        /// 当前实例是否为数值类型
        /// </summary>
        /// <returns>
        /// <para>如果<see cref="DataType"/>属于<see cref="JsonType.RealNum"/>或<see cref="JsonType.Integer"/>则返回true，否则返回false</para>
        /// </returns>
        public virtual bool IsNumberType
        {
            get => false;
        }

        /// <summary>
        /// 返回此实例的数值
        /// </summary>
        /// <returns>如果该实例是<see cref="JsonType.RealNum"/>则返回值，如果是<see cref="JsonType.Integer"/>则转化并返回双浮点值</returns>
        /// <exception cref="NotImplementedException">不是数值类型</exception>
        public virtual double Number
        {
            get
            {
                throw new NotImplementedException(NotDataTypeException);
            }
        }

        /// <summary>
        /// 获取此实例的数值
        /// </summary>
        /// <param name="number">获取</param>
        /// <returns>如果<see cref="DataType"/>属于<see cref="JsonType.RealNum"/>或<see cref="JsonType.Integer"/>则返回true并将值写入<paramref name="number"/>；否则返回false，并且<paramref name="number"/>参数无效</returns>
        public virtual bool TryGetNumber(out double number)
        {
            number = default;
            return false;
        }

        #endregion

        #region 功能

        #region 创建

        /// <summary>
        /// 创建一个浮点值json对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonVariable CreateNumber(double value)
        {
            return new JsonRealNumber(value);
        }

        /// <summary>
        /// 创建一个整数值json对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonVariable CreateInteger(long value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// 创建一个布尔类型json对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonVariable CreateBoolean(bool value)
        {
            return new JsonBoolean(value);
        }

        /// <summary>
        /// 创建一个字符串类型json对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonVariable CreateString(string value)
        {
            return new JsonString(value);
        }

        #endregion

        #region 派生

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as JsonVariable);
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
            return this.GetType().Name;
        }

        /// <summary>
        /// 使用指定区域性格式返回json对象的字符串
        /// </summary>
        /// <param name="formatProvider">提供区域性特定的格式设置信息</param>
        /// <returns></returns>
        public virtual string ToString(IFormatProvider formatProvider)
        {
            JsonParserDefault jpd = new JsonParserDefault();
            using (var swr = new System.IO.StringWriter(formatProvider))
            {
                jpd.ParsingJson(this, swr);
                return swr.ToString();
            }
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals(JsonVariable other);

        /// <summary>
        /// 原始默认哈希代码
        /// </summary>
        /// <returns></returns>
        protected int BaseHashCode()
        {
            return base.GetHashCode();
        }

        public virtual long GetHashCode64()
        {
            switch (DataType)
            {
                case JsonType.Integer:
                    return Integer.GetHashCode64();
                case JsonType.RealNum:
                    return RealNum.GetHashCode64();
                case JsonType.Boolean:
                    return Boolean.GetHashCode64();
                case JsonType.String:
                    return String.GetHashCode64();
                case JsonType.Null:
                    return 0;
                default:
                    return base.GetHashCode();
            }
        }

        /// <summary>
        /// 创建一个与当前对象内容相同的新对象
        /// </summary>
        /// <returns>与当前对象内容相同的新对象</returns>
        public virtual JsonVariable Clone()
        {
            return this;
        }

        #region 运算符重载

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

        /// <summary>
        /// 强转为整数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator long(JsonVariable jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            var t = jobj.DataType;
            if (t == JsonType.Integer)
            {
                return jobj.Integer;
            }
            if(t == JsonType.RealNum)
            {
                return (long)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为浮点数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator double(JsonVariable jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            var t = jobj.DataType;
            if (t == JsonType.RealNum)
            {
                return jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为整数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator int(JsonVariable jobj)
        {
            if(jobj is null) throw new InvalidCastException();
            var t = jobj.DataType;
            if (t == JsonType.Integer)
            {
                return (int)jobj.Integer;
            }
            if (t == JsonType.RealNum)
            {
                return (int)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为浮点数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator float(JsonVariable jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            var t = jobj.DataType;
            if (t == JsonType.RealNum)
            {
                return (float)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为字符串
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator string(JsonVariable jobj)
        {
            if (jobj is null) return null;
            var t = jobj.DataType;
            if (t == JsonType.String)
            {
                return jobj.String;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为布尔值
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">不是能转换的类型</exception>
        public static explicit operator bool(JsonVariable jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            var t = jobj.DataType;
            if (t == JsonType.Boolean)
            {
                return jobj.Boolean;
            }
            throw new InvalidCastException();
        }


        /// <summary>
        /// 强转为整数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">无效转化</exception>
        public static explicit operator long?(JsonVariable jobj)
        {
            if (jobj is null || jobj.IsNull) return null;
            var t = jobj.DataType;
            if (t == JsonType.Integer)
            {
                return jobj.Integer;
            }
            if (t == JsonType.RealNum)
            {
                return (long)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为浮点数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">无效转化</exception>
        public static explicit operator double?(JsonVariable jobj)
        {
            if (jobj is null || jobj.IsNull) return null;
            var t = jobj.DataType;
            if (t == JsonType.RealNum)
            {
                return jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为整数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">无效转化</exception>
        public static explicit operator int?(JsonVariable jobj)
        {
            if (jobj is null || jobj.IsNull) return null;
            var t = jobj.DataType;
            if (t == JsonType.Integer)
            {
                return (int)jobj.Integer;
            }
            if (t == JsonType.RealNum)
            {
                return (int)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为浮点数
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">无效转化</exception>
        public static explicit operator float?(JsonVariable jobj)
        {
            if (jobj is null || jobj.IsNull) return null;
            var t = jobj.DataType;
            if (t == JsonType.RealNum)
            {
                return (float)jobj.RealNum;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为布尔值
        /// </summary>
        /// <param name="jobj"></param>
        /// <exception cref="InvalidCastException">无效转化</exception>
        public static explicit operator bool?(JsonVariable jobj)
        {
            if (jobj is null || jobj.IsNull) return null;
            var t = jobj.DataType;
            if (t == JsonType.Boolean)
            {
                return jobj.Boolean;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(long value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(double value)
        {
            return new JsonRealNumber(value);
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(int value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(float value)
        {
            return new JsonRealNumber(value);
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(bool value)
        {
            return new JsonBoolean(value);
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(long? value)
        {
            if (value.HasValue) return new JsonInteger(value.Value);
            return JsonNull.Nullable;
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(double? value)
        {
            if (value.HasValue) return new JsonRealNumber(value.Value);
            return JsonNull.Nullable;
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(int? value)
        {
            if (value.HasValue) return new JsonInteger(value.Value);
            return JsonNull.Nullable;
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(float? value)
        {
            if (value.HasValue) return new JsonRealNumber(value.Value);
            return JsonNull.Nullable;
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(bool? value)
        {
            if (value.HasValue) return new JsonBoolean(value.Value);
            return JsonNull.Nullable;
        }

        /// <summary>
        /// 强转为Json对象
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator JsonVariable(string value)
        {
            return value is null ? (JsonVariable)JsonNull.Nullable : new JsonString(value);
        }

        #endregion

        #region 显式实现

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #endregion

        #region 常量

        private sealed class c_ConstJson
        {
            public c_ConstJson()
            {
                btrue = new JsonBoolean(true);
                bfalse = new JsonBoolean(false);
            }
            public JsonBoolean btrue;
            public JsonBoolean bfalse;

            public readonly static c_ConstJson cp_obj = new c_ConstJson();
        }

        /// <summary>
        /// 表示null的json对象
        /// </summary>
        public static JsonVariable JsonNullValue
        {
            get => JsonNull.Nullable;
        }

        /// <summary>
        /// 表示布尔值true的json对象
        /// </summary>
        public static JsonVariable JsonTrueValue
        {
            get => c_ConstJson.cp_obj.btrue;
        }

        /// <summary>
        /// 表示布尔值false的json对象
        /// </summary>
        public static JsonVariable JsonFalseValue
        {
            get => c_ConstJson.cp_obj.bfalse;
        }

        /// <summary>
        /// 根据布尔参数获取json布尔值常量
        /// </summary>
        /// <param name="value">判断参数</param>
        /// <returns>布尔类型的json对象</returns>
        public static JsonVariable GetBooleanValue(bool value)
        {
            return value ? JsonTrueValue : JsonFalseValue;
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

        public override long GetHashCode64()
        {
            return 0;
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return JsonText;
        }

        public override JsonVariable Clone()
        {
            return Nullable;
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

        /// <summary>
        /// 实例化一个整数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonInteger(int value)
        {
            this.value = value;
        }

        #endregion

        #region 参数
        /// <summary>
        /// 数据
        /// </summary>
        public readonly long value;
        #endregion

        #region 派生

        public override JsonType DataType => JsonType.Integer;

        public override long Integer
        {
            get => value;
        }

        public override bool IsNumberType => true;

        public override double Number => value;

        public override bool TryGetNumber(out double number)
        {
            number = value;
            return true;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override long GetHashCode64()
        {
            return value;
        }

        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;
            if (other.DataType != JsonType.Integer) return false;
            return value == other.Integer; 
        }

        public override bool IsNull => false;

        public override JsonVariable Clone() => this;

        public override string ToString()
        {
            return value.ToString();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value.ToString(formatProvider);
        }

        /// <summary>
        /// 使用指定的格式，将此实例的数值转换为它的等效字符串表示形式
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns>此实例的值的字符串表示形式，由<paramref name="format"/>指定</returns>
        /// <exception cref="System.FormatException"><paramref name="format"/>无效</exception>
        public string ToString(string format)
        {
            return value.ToString(format);
        }

        public static implicit operator long(JsonInteger jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return jobj.value;
        }

        public static implicit operator long?(JsonInteger jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return jobj?.value;
        }

        public static explicit operator int(JsonInteger jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return (int)jobj.value;
        }

        public static explicit operator int?(JsonInteger jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return (int?)(jobj?.value);
        }

        public static implicit operator JsonInteger(long value)
        {
            return new JsonInteger(value);
        }

        public static implicit operator JsonInteger(int value)
        {
            return new JsonInteger(value);
        }

        public static explicit operator JsonInteger(double value)
        {
            return new JsonInteger((long)value);
        }

        public static explicit operator JsonInteger(float value)
        {
            return new JsonInteger((long)value);
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

        /// <summary>
        /// 实例化一个小数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonRealNumber(float value)
        {
            this.value = value;
        }

        /// <summary>
        /// 实例化一个小数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonRealNumber(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// 实例化一个小数json对象
        /// </summary>
        /// <param name="value">指定的值</param>
        public JsonRealNumber(int value)
        {
            this.value = value;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 数据
        /// </summary>
        public readonly double value;

        #endregion

        #region 派生

        public override JsonType DataType => JsonType.RealNum;

        public override double RealNum
        {
            get => value;
        }

        public override bool IsNull => false;

        public override object Data => value;

        public override bool IsNumberType => true;

        public override double Number => value;

        public override bool TryGetNumber(out double number)
        {
            number = value;
            return true;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        /// <summary>
        /// 使用指定的格式，将此实例的数值转换为它的等效字符串表示形式
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns>此实例的值的字符串表示形式，由<paramref name="format"/>指定</returns>
        /// <exception cref="System.FormatException"><paramref name="format"/>无效</exception>
        public string ToString(string format)
        {
            return value.ToString(format);
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value.ToString(formatProvider);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override long GetHashCode64()
        {
            return value.GetHashCode64();
        }

        public override JsonVariable Clone() => this;

        public override bool Equals(JsonVariable other)
        {
            if (other is null) return false;

            if (other.DataType != JsonType.RealNum) return false;

            return value == other.RealNum;
        }

        public static implicit operator double(JsonRealNumber jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return jobj.value;
        }

        public static explicit operator float(JsonRealNumber jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return (float)jobj.value;
        }

        public static explicit operator long(JsonRealNumber jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return (long)jobj.value;
        }

        public static explicit operator int(JsonRealNumber jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return (int)jobj.value;
        }

        public static implicit operator JsonRealNumber(double value)
        {
            return new JsonRealNumber(value);
        }

        public static implicit operator JsonRealNumber(float value)
        {
            return new JsonRealNumber(value);
        }

        public static implicit operator JsonRealNumber(int value)
        {
            return new JsonRealNumber(value);
        }

        public static implicit operator JsonRealNumber(long value)
        {
            return new JsonRealNumber(value);
        }


        public static implicit operator double?(JsonRealNumber jobj)
        {
            return jobj?.value;
        }

        public static explicit operator float?(JsonRealNumber jobj)
        {
            return (float?)(jobj?.value);
        }

        public static explicit operator long?(JsonRealNumber jobj)
        {
            return (long?)jobj?.value;
        }

        public static explicit operator int?(JsonRealNumber jobj)
        {
            return (int?)jobj?.value;
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
        public JsonBoolean()
        {
            value = default;
        }

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
        public readonly bool value;

        /// <summary>
        /// 值为true时的json文本
        /// </summary>
        public const string TrueJsonText = "true";

        /// <summary>
        /// 值为false时的json文本
        /// </summary>
        public const string FalseJsonText = "false";

        #endregion

        #region 派生

        public override JsonType DataType => JsonType.Boolean;

        public override bool Boolean
        {
            get => value;
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

        public override long GetHashCode64()
        {
            return value.GetHashCode64();
        }

        public override JsonVariable Clone() => this;

        public override string ToString()
        {
            return value ? TrueJsonText : FalseJsonText;
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return value ? TrueJsonText : FalseJsonText;
        }

        public static implicit operator bool?(JsonBoolean jobj)
        {
            return jobj?.value;
        }

        public static implicit operator bool(JsonBoolean jobj)
        {
            if (jobj is null) throw new InvalidCastException();
            return jobj.value;
        }

        public static explicit operator JsonBoolean(bool value)
        {
            return new JsonBoolean(value);
        }

        public static explicit operator JsonBoolean(bool? value)
        {
            return value.HasValue ? new JsonBoolean(value.Value) : null;
        }

        #endregion

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
        /// <param name="value">指定的字符串，如果是null则表示为空字符串</param>
        public JsonString(string value)
        {
            this.value = (value is null) ? string.Empty : value;
        }

        #endregion

        #region 参数

        private readonly string value;

        /// <summary>
        /// 访问字符串数据
        /// </summary>
        public string Value
        {
            get => value;
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

        public override long GetHashCode64()
        {
            return value.GetHashCode64();
        }

        public override JsonVariable Clone() => this;

        public static implicit operator JsonString(string value)
        {
            if (value is null) return null;
            return new JsonString(value);
        }

        public static implicit operator string(JsonString json)
        {
            return json?.value;
        }

        public override unsafe string ToString()
        {
            var len = value.Length;
            int leng = len + 2;
            if (len < 128)
            {
                char* cp = stackalloc char[leng];
                cp[0] = '"';
                cp[leng - 1] = '"';
                fixed (char* np = value)
                {
                    MemoryOperation.MemoryCopy(np, cp + 1, sizeof(char) * len);
                }
                return new string(cp, 0, leng);
            }
            char[] carr = new char[leng];
            carr[0] = '"';
            carr[leng - 1] = '"';
            value.CopyTo(0, carr, 1, len);
            return new string(carr);
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return "\"" + value + "\"";
        }

        #endregion

    }

    #endregion

}
