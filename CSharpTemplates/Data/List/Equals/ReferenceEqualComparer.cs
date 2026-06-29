using System;
using System.Collections.Generic;


namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 判断相等的引用对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReferenceEqualComparer<T> : EqualityComparer<T>
    {

        #region 实现

        public ReferenceEqualComparer()
        {
            p_isValue = typeof(T).IsValueType;
        }

        private readonly bool p_isValue;

        public override bool Equals(T x, T y)
        {
            if (p_isValue) return false;
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            if (!p_isValue)
            {
                if (obj == null) return 0;
            }
            return obj.GetHashCode();
        }

        #endregion

        #region 单例

        private sealed class LazyObj
        {
            public static ReferenceEqualComparer<T> p_eq = new ReferenceEqualComparer<T>();
        }

        /// <summary>
        /// 默认实现的<typeparamref name="T"/>类型引用对象判断
        /// </summary>
        public static ReferenceEqualComparer<T> DefaultRefEqual
        {
            get => LazyObj.p_eq;
        }

        #endregion

    }

}
