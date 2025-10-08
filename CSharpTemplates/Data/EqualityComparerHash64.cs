using Cheng.Algorithm.HashCodes;
using System.Collections.Generic;
using System;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 提供<see cref="IEqualityComparer{T}"/>和<see cref="IBaseHashCode64{T}"/>的共同接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEqualityComparerHash64<T> : IEqualityComparer<T>, IBaseHashCode64<T>
    {
    }


    /// <summary>
    /// 提供<see cref="IEqualityComparer{T}"/>接口的实现基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EqualityComparerHash64<T> : IEqualityComparerHash64<T>
    {

        protected EqualityComparerHash64()
        {
        }

        public abstract bool Equals(T x, T y);

        public virtual int GetHashCode(T obj)
        {
            return GetHashCode64(obj).GetHashCode();
        }

        /// <summary>
        /// 获取指定对象的64位哈希值
        /// </summary>
        /// <param name="value">要获取的对象</param>
        /// <returns>对象的64位哈希值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public abstract long GetHashCode64(T value);

        static defEq p_def = new defEq();

        /// <summary>
        /// 获取一个默认实现的<typeparamref name="T"/>类型对象
        /// </summary>
        /// <returns>
        /// <para>默认实现的对象，从<see cref="EqualityComparer{T}.Default"/>获取<see cref="IEqualityComparer{T}"/>来实现<see cref="Equals(T, T)"/>和<see cref="GetHashCode(T)"/>；从<see cref="BaseHashCode64{T}.Default"/>获取<see cref="IBaseHashCode64{T}"/>来实现<see cref="GetHashCode64(T)"/></para>
        /// </returns>
        public static EqualityComparerHash64<T> Default
        {
            get => p_def;
        }

        #region

        private sealed class defEq : EqualityComparerHash64<T>
        {
            public defEq()
            {
                p_defeq = EqualityComparer<T>.Default;
                p_defHash64 = BaseHashCode64<T>.Default;
            }

            private IEqualityComparer<T> p_defeq;

            private IBaseHashCode64<T> p_defHash64;

            public override bool Equals(T x, T y)
            {
                return p_defeq.Equals(x, y);
            }

            public override int GetHashCode(T obj)
            {
                return p_defeq.GetHashCode(obj);
            }

            public override long GetHashCode64(T value)
            {
                return p_defHash64.GetHashCode64(value);
            }
        }

        #endregion

    }


}
