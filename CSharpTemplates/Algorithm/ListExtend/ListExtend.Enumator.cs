using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Collections
{

    unsafe partial class ListExtend
    {

        #region 限制枚举器数量

        #region 枚举器

        internal sealed class Enumerable_enumator_GetEnumatorCount<T> : IEnumerable<T>
        {
            public Enumerable_enumator_GetEnumatorCount(IEnumerable<T> es, long maxCount)
            {
                p_enum = es;
                this.p_maxCount = maxCount;
            }
            private readonly IEnumerable<T> p_enum;
            private readonly long p_maxCount;

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumator_GetEnumatorCount<T>(p_enum.GetEnumerator(), p_maxCount);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class Enumator_GetEnumatorCount<T> : IEnumerator<T>
        {
            public Enumator_GetEnumatorCount(IEnumerator<T> ator, long maxCount)
            {
                this.p_enum = ator ?? throw new ArgumentNullException();
                p_maxCount = maxCount;
                p_index = -1;
                p_cur = default;
            }
            private readonly long p_maxCount;
            private long p_index;
            private IEnumerator<T> p_enum;
            private T p_cur;

            public T Current => p_cur;

            public bool MoveNext()
            {
                if (p_enum is null) throw new NotImplementedException();
                var i = p_index + 1;
                if(i >= p_maxCount)
                {
                    return false;
                }
                if (p_enum.MoveNext())
                {
                    p_cur = p_enum.Current;
                    p_index = i;
                    return true;
                }
                return false;
            }

            public void Dispose()
            {
                p_enum?.Dispose();
                p_index = p_maxCount + 1;
                p_cur = default;
            }

            public void Reset()
            {
                if (p_enum is null) throw new NotImplementedException();
                p_enum.Reset();
                p_index = p_maxCount + 1;
                p_cur = default;
            }

            object IEnumerator.Current => Current;
        }

        #endregion

        /// <summary>
        /// 限制序列的最大枚举元素数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">要封装的序列枚举器</param>
        /// <param name="maxCount">要限制的最大元素数</param>
        /// <returns>指定最大元素数的枚举器，枚举推进数量最大枚举次数等于<paramref name="maxCount"/>，枚举器的推进次数小于<paramref name="maxCount"/>则不会影响</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public static IEnumerable<T> GetEnumatorToMaxCount<T>(this IEnumerable<T> collection, long maxCount)
        {
            if (collection is null) throw new ArgumentNullException();
            if (maxCount < 0) throw new ArgumentOutOfRangeException();
            return new Enumerable_enumator_GetEnumatorCount<T>(collection, maxCount);
        }

        /// <summary>
        /// 限制序列的最大枚举元素数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">要封装的序列枚举器</param>
        /// <param name="maxCount">要限制的最大元素数</param>
        /// <returns>指定最大元素数的枚举器，枚举推进数量最大枚举次数等于<paramref name="maxCount"/>，枚举器的推进次数小于<paramref name="maxCount"/>则不会影响</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public static IEnumerable<T> GetEnumatorToMaxCount<T>(this IEnumerable<T> collection, int maxCount)
        {
            if (collection is null) throw new ArgumentNullException();
            if (maxCount < 0) throw new ArgumentOutOfRangeException();
            return new Enumerable_enumator_GetEnumatorCount<T>(collection, maxCount);
        }

        #endregion

    }

}