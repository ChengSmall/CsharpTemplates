using Cheng.Algorithm.HashCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 引用值类型
    /// </summary>
    /// <remarks>
    /// <para>将值类型封装到引用类型对象，可以避免频繁装箱导致的额外开销</para>
    /// </remarks>
    /// <typeparam name="T">值类型</typeparam>
    public sealed class RefStructure<T> : IEquatable<RefStructure<T>>, IEquatable<T>, IEquatable<T?>, IComparable<RefStructure<T>>, IComparable<T>, IComparable<T?>, IHashCode64 where T : struct
    {

        #region

        /// <summary>
        /// 实例化一个引用值类型
        /// </summary>
        public RefStructure()
        {
            this.value = default;
        }

        /// <summary>
        /// 实例化一个引用值类型
        /// </summary>
        /// <param name="value">要封装的值</param>
        public RefStructure(T value)
        {
            this.value = value;
        }

        #endregion

        #region

        private T value;

        #endregion

        #region

        /// <summary>
        /// 访问或设置值
        /// </summary>
        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override bool Equals(object obj)
        {
            if(obj is RefStructure<T> refv)
            {
                return this.value.Equals(refv.value);
            }
            return value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public bool Equals(RefStructure<T> other)
        {
            if (other is null) return false;
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public long GetHashCode64()
        {
            return BaseHashCode64<T>.Default.GetHashCode64(value);
        }

        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(value, other);
        }

        public int CompareTo(RefStructure<T> other)
        {
            return Comparer<T>.Default.Compare(value, other.value);
        }

        public int CompareTo(T other)
        {
            return Comparer<T>.Default.Compare(value, other);
        }

        public bool Equals(T? other)
        {
            return EqualityComparer<T?>.Default.Equals(value, other);
        }

        public int CompareTo(T? other)
        {
            return Comparer<T?>.Default.Compare(value, other);
        }

        /// <summary>
        /// 转换为值类型
        /// </summary>
        /// <param name="rv"></param>
        /// <exception cref="ArgumentNullException">null引用无法转换为值类型</exception>
        public static explicit operator T(RefStructure<T> rv)
        {
            if (rv is null) throw new ArgumentNullException();
            return rv.value;
        }

        /// <summary>
        /// 隐式转换为引用封装
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator RefStructure<T>(T v)
        {
            return new RefStructure<T>(v);
        }

        /// <summary>
        /// 转换为可空值类型
        /// </summary>
        /// <param name="rv"></param>
        public static implicit operator T?(RefStructure<T> rv)
        {
            return rv?.value;
        }

        /// <summary>
        /// 隐式转换为引用封装
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator RefStructure<T>(T? v)
        {
            return v.HasValue ? new RefStructure<T>(v.Value) : null;
        }

        #endregion

    }

}
