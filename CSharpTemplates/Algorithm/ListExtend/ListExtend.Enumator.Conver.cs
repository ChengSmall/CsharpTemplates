using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cheng.DataStructure;

namespace Cheng.Algorithm.Collections
{

    public static partial class ListExtend
    {

        #region 转换另一个类型

        #region 结构

        private class Enumerable_ToOtherItems<T, O> : IEnumerable<O>
        {

            public Enumerable_ToOtherItems(IEnumerable<T> collections, Converter<T, O> convertFunc, ConvertByCondition<T,O> convertIfunc, bool funcIf)
            {
                p_collections = collections;
                p_func = convertFunc;
                p_if = funcIf;
                p_convertIfunc = convertIfunc;
            }
            private IEnumerable<T> p_collections;
            private Converter<T, O> p_func;
            private ConvertByCondition<T, O> p_convertIfunc;
            private bool p_if;

            public IEnumerator<O> GetEnumerator()
            {
                if (p_if)
                {
                    return new Enumator_ToOtherIfItems<T, O>(p_collections, p_convertIfunc);
                }
                else
                {
                    return new Enumator_ToOtherItems<T, O>(p_collections, p_func);
                }
                
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class Enumator_ToOtherItems<T, O> : IEnumerator<O>
        {
            public Enumator_ToOtherItems(IEnumerable<T> collections, Converter<T, O> convertFunc)
            {
                p_enumator = collections.GetEnumerator() ?? throw new ArgumentNullException();
                p_current = default;
                p_convertFunc = convertFunc;
            }

            private IEnumerator<T> p_enumator;
            private Converter<T, O> p_convertFunc;
            private O p_current;

            public O Current
            {
                get => p_current;
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (p_enumator is null) throw new NotImplementedException();
                var next = p_enumator.MoveNext();

                if (!next)
                {
                    p_current = default;
                    return false;
                }

                p_current = p_convertFunc.Invoke(p_enumator.Current);
                return true;
            }

            public void Reset()
            {
                if (p_enumator is null) throw new NotImplementedException();
                p_current = default;
                p_enumator.Reset();
            }

            public void Dispose()
            {
                if (p_enumator is null) return;
                p_enumator.Dispose();
                p_enumator = null;
                p_current = default;
                p_convertFunc = null;
            }
        }

        private class Enumator_ToOtherIfItems<T, O> : IEnumerator<O>
        {
            public Enumator_ToOtherIfItems(IEnumerable<T> collections, ConvertByCondition<T, O> convertFunc)
            {
                p_enumator = collections.GetEnumerator() ?? throw new ArgumentNullException();
                p_current = default;
                p_convertFunc = convertFunc;
            }

            private IEnumerator<T> p_enumator;
            private ConvertByCondition<T, O> p_convertFunc;
            private O p_current;

            public O Current
            {
                get => p_current;
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (p_enumator is null) throw new NotImplementedException();

                Loop:
                var next = p_enumator.MoveNext();

                if (!next)
                {
                    p_current = default;
                    return false;
                }

                if (p_convertFunc.Invoke(p_enumator.Current, out O conver))
                {
                    p_current = conver;
                    return true;
                }
                goto Loop;

            }

            public void Reset()
            {
                if (p_enumator is null) throw new NotImplementedException();
                p_current = default;
                p_enumator.Reset();
            }

            public void Dispose()
            {
                if (p_enumator is null) return;
                p_enumator.Dispose();
                p_enumator = null;
                p_convertFunc = null;
                p_current = default;
            }
        }

        #endregion

        /// <summary>
        /// 将集合元素转化为另一种类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="collections">原集合</param>
        /// <param name="convertFunc">转化方法</param>
        /// <returns>另一种类型的集合</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static IEnumerable<O> ToOtherItems<T,O>(this IEnumerable<T> collections, Converter<T, O> convertFunc)
        {
            if(collections is null || convertFunc is null)
            {
                throw new ArgumentNullException(collections is null ? nameof(collections) : nameof(convertFunc));
            }

            return new Enumerable_ToOtherItems<T,O>(collections, convertFunc, null, false);
        }

        /// <summary>
        /// 将集合元素按条件筛选转化为另一种类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="collections">原集合</param>
        /// <param name="convertFunc">条件选择转化器</param>
        /// <returns>另一种类型的集合</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static IEnumerable<O> ToOtherItemsByCondition<T, O>(this IEnumerable<T> collections, ConvertByCondition<T,O> convertFunc)
        {
            if (collections is null || convertFunc is null)
            {
                throw new ArgumentNullException(collections is null ? nameof(collections) : nameof(convertFunc));
            }

            return new Enumerable_ToOtherItems<T, O>(collections, null, convertFunc, true);
        }

        #endregion

    }

}
