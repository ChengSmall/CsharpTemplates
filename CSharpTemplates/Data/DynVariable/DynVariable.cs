using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.DynamicVariables
{

    /// <summary>
    /// 动态数据类型
    /// </summary>
    public enum DynVariableType : byte
    {

        /// <summary>
        /// 空值
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 32位整数
        /// </summary>
        Int32 =     0b0001, // 1

        /// <summary>
        /// 64位整数
        /// </summary>
        Int64 =     0b0010, // 2

        /// <summary>
        /// 32位浮点数
        /// </summary>
        Float = 0b0101, // 5

        /// <summary>
        /// 64位双精度浮点数
        /// </summary>
        Double =    0b0110, // 6

        /// <summary>
        /// 布尔类型
        /// </summary>
        Boolean =   0b0100, // 4

        /// <summary>
        /// 字符串类型
        /// </summary>
        String =    0b0111, // 7


        /// <summary>
        /// 集合类型
        /// </summary>
        /// <remarks>可储存动态类型数据的集合</remarks>
        List =          0b1001, // 9

        /// <summary>
        /// 字典类型
        /// </summary>
        /// <remarks>key是字符串，value是动态类型数据的字典；key的字符数量不超过65535</remarks>
        Dictionary =    0b1010, // 10

    }

    /// <summary>
    /// 通用的动态数据类型基类
    /// </summary>
    public abstract class DynVariable : IEquatable<DynVariable>, IComparable<DynVariable>, IHashCode64, ICloneable
    {

        #region 参数

        /// <summary>
        /// 数据的类型
        /// </summary>
        public abstract DynVariableType DynType { get; }

        /// <summary>
        /// 获取32位整数值
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual int Int32Value
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取64位整数值
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual long Int64Value
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual float FloatValue
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取双浮点值
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual double DoubleValue
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual bool BooleanValue
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual string StringValue
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 是否为空值
        /// </summary>
        public virtual bool IsEmpty => false;

        /// <summary>
        /// 获取集合类型
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual DynList DynamicList
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取字典类型
        /// </summary>
        /// <exception cref="NotSupportedException">不是该类型</exception>
        public virtual DynDictionary DynamicDictionary
        {
            get => throw new NotSupportedException();
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(DynVariable a, DynVariable b)
        {
            if ((object)a == (object)b) return true;
            if (a is null)
            {
                return b.Equals(a);
            }
            return a.Equals(b);
        }

        /// <summary>
        /// 判断是否不相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(DynVariable a, DynVariable b)
        {
            if ((object)a == (object)b) return false;
            if (a is null)
            {
                return !b.Equals(a);
            }
            return !a.Equals(b);
        }

        #endregion

        #region 派生

        public virtual bool Equals(DynVariable other)
        {
            return object.ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (obj is DynVariable dyn) return Equals(dyn);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return DynType.ToString();
        }

        public virtual int CompareTo(DynVariable other)
        {
            if(other is null)
            {
                return IsEmpty ? 0 : 1;
            }
            return this.DynType - other.DynType;
        }

        public virtual long GetHashCode64()
        {
            return GetHashCode();
        }

        /// <summary>
        /// 返回一个新克隆的此实例的数据
        /// </summary>
        /// <returns>新的拷贝实例</returns>
        public virtual DynVariable Clone()
        {
            return this;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region 创建

        /// <summary>
        /// 创建一个32位整数
        /// </summary>
        /// <param name="value">要创建的值</param>
        /// <returns>类型是32位整数的动态对象</returns>
        public static DynVariable CreateInt32(int value)
        {
            return new Dyn32bit(value);
        }

        /// <summary>
        /// 创建一个64位整数
        /// </summary>
        /// <param name="value">要创建的值</param>
        /// <returns>类型是32位整数的动态对象</returns>
        public static DynVariable CreateInt64(long value)
        {
            return new Dyn64bit(value);
        }

        /// <summary>
        /// 创建一个32位单精度浮点数
        /// </summary>
        /// <param name="value">要创建的值</param>
        /// <returns>类型是单浮点数的动态对象</returns>
        public static DynVariable CreateFloat(float value)
        {
            return new Dyn32bit(value);
        }

        /// <summary>
        /// 创建一个64位双精度浮点数
        /// </summary>
        /// <param name="value">要创建的值</param>
        /// <returns>类型是双浮点数的动态对象</returns>
        public static DynVariable CreateDouble(double value)
        {
            return new Dyn64bit(value);
        }

        /// <summary>
        /// 创建一个字符串
        /// </summary>
        /// <param name="value">要创建的字符串</param>
        /// <returns>类型是字符串的动态对象</returns>
        public static DynVariable CreateString(string value)
        {
            return new DynString(value);
        }

        /// <summary>
        /// 创建一个布尔值
        /// </summary>
        /// <param name="value">要创建的值</param>
        /// <returns>表示布尔类型的新对象</returns>
        public static DynVariable CreateBoolean(bool value)
        {
            return new DynBoolean(value);
        }

        /// <summary>
        /// 创建一个空值
        /// </summary>
        /// <returns>创建的空值对象</returns>
        public static DynVariable CreateEmpty()
        {
            return new DynEmpty();
        }

        #endregion

        #region 常量

        /// <summary>
        /// 表示空值的对象
        /// </summary>
        public static DynVariable EmptyValue => sp_constVar.p_empty;

        /// <summary>
        /// 值为true的布尔值
        /// </summary>
        public static DynVariable BooleanTrue => sp_constVar.p_true;

        /// <summary>
        /// 值为false的布尔值
        /// </summary>
        public static DynVariable BooleanFalse => sp_constVar.p_false;

        #region

        private static readonly c_constVar sp_constVar = f_createConstVar();

        static c_constVar f_createConstVar()
        {
            c_constVar var;
            var.p_empty = CreateEmpty();
            var.p_true = CreateBoolean(true);
            var.p_false = CreateBoolean(false);
            return var;
        }

        private struct c_constVar
        {
            public DynVariable p_true;
            public DynVariable p_false;
            public DynVariable p_empty;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 可变对象的公共基类
    /// </summary>
    public abstract class DynamicObject : DynVariable
    {

        /// <summary>
        /// 当前对象是否已被锁定
        /// </summary>
        public abstract bool Locked { get; }

        /// <summary>
        /// 将对象锁定
        /// </summary>
        /// <remarks>
        /// <para>锁定后的对象将变成只读，无法修改内部参数</para>
        /// </remarks>
        public abstract void OnLock();

        public override bool Equals(DynVariable other)
        {
            return ReferenceEquals(this, other);
        }

    }

}
