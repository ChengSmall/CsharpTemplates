using System;
using System.Collections.Generic;
using System.Collections;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 可随意遍历的先进后出可变栈数据结构
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public class TraverStack<T> : IReadOnlyList<T>
    {

        #region 结构

        /// <summary>
        /// 用于循环访问元素的枚举器
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            internal Enumerator(TraverStack<T> stack)
            {
                p_stack = stack;
                p_version = p_stack.p_version;
                p_index = -2;
                p_current = default(T);
            }

            private TraverStack<T> p_stack;

            private int p_index;

            private int p_version;

            private T p_current;

            /// <summary>
            /// 获取集合中位于枚举数当前位置的元素
            /// </summary>
            public T Current
            {
                get
                {
                    if (p_index == -2)
                    {
                        throw new InvalidOperationException();
                    }

                    if (p_index == -1)
                    {
                        throw new InvalidOperationException();
                    }

                    return p_current;
                }
            }


            /// <summary>
            /// 将枚举数推进到集合的下一个元素
            /// </summary>
            /// <returns>返回true表示成功地推进到下一个元素；返回false则表示已经遍历到末尾</returns>
            /// <exception cref="InvalidOperationException">在遍历时集合发生变动</exception>
            public bool MoveNext()
            {
                if (p_version != p_stack.p_version)
                {
                    throw new InvalidOperationException();
                }

                bool b;
                if (p_index == -2)
                {
                    p_index = p_stack.p_size - 1;
                    b = (p_index >= 0);
                    if (b)
                    {
                        p_current = p_stack.p_arr[p_index];
                    }

                    return b;
                }

                if (p_index == -1)
                {
                    return false;
                }

                b = (--p_index >= 0);
                if (b)
                {
                    p_current = p_stack.p_arr[p_index];
                }
                else
                {
                    p_current = default(T);
                }

                return b;
            }

            /// <summary>
            /// 将枚举数重置为初始位置，该位置位于第一个元素之前
            /// </summary>
            public void Reset()
            {
                if (p_version != p_stack.p_version)
                {
                    throw new InvalidOperationException();
                }

                p_index = -2;
                p_current = default(T);
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            void IDisposable.Dispose()
            {
                p_index = -1;
            }

        }

        #endregion

        #region 构造

        /// <summary>
        /// 初始化可遍历栈
        /// </summary>
        public TraverStack()
        {
            p_arr = cp_emptyArray;
            p_size = 0;
            p_version = 0;
        }

        /// <summary>
        /// 初始化可遍历栈，指定栈初始容量
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public TraverStack(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            p_arr = new T[capacity];
            p_size = 0;
            p_version = 0;
        }

        /// <summary>
        /// 初始化可遍历栈，将指定集合元素压入栈
        /// </summary>
        /// <param name="enumator">要压栈的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public TraverStack(IEnumerable<T> enumator)
        {
            if (enumator == null)
            {
                throw new ArgumentNullException();
            }

            if (enumator is ICollection<T> carr)
            {
                int count = carr.Count;
                p_arr = new T[count];
                carr.CopyTo(p_arr, 0);
                p_size = count;
                return;
            }

            p_size = 0;
            p_arr = new T[cp_defaultCapacity];
            foreach (T item in enumator)
            {
                Push(item);
            }
        }

        #endregion

        #region 参数

        private const int cp_defaultCapacity = 4;

        private static T[] cp_emptyArray = new T[0];

        private T[] p_arr;

        private int p_size;

        private int p_version;

        #endregion

        #region 栈功能

        /// <summary>
        /// 当前栈的元素数量
        /// </summary>
        public int Count
        {
            get
            {
                return p_size;
            }
        }

        /// <summary>
        /// 按索引顺序从栈顶访问栈元素
        /// </summary>
        /// <param name="index">从栈顶开始的索引，范围在[0,<see cref="Count"/>)</param>
        /// <returns>指定栈位的元素</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出元素范围</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= p_size) throw new ArgumentNullException();
                return p_arr[(p_size - 1) - index];
            }
        }

        /// <summary>
        /// 按索引顺序从栈底访问栈元素
        /// </summary>
        /// <param name="index">从栈底开始的索引，范围在[0,<see cref="Count"/>)</param>
        /// <returns>指定栈位的元素</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出元素范围</exception>
        public T GetStackLast(int index)
        {
            if (index < 0 || index >= p_size) throw new ArgumentNullException();
            return p_arr[index];
        }

        /// <summary>
        /// 清空栈的所有元素数量
        /// </summary>
        public void Clear()
        {
            Array.Clear(p_arr, 0, p_size);
            p_size = 0;
            p_version++;
        }

        /// <summary>
        /// 查询栈内是否存在指定元素
        /// </summary>
        /// <param name="item">要查询的元素</param>
        /// <param name="equalityComparer">查询时的比较方法，null表示使用默认实现</param>
        /// <returns>若存在返回true，不存在返回false</returns>
        public bool Contains(T item, IEqualityComparer<T> equalityComparer)
        {
            int size = p_size;
            if(equalityComparer is null) equalityComparer = EqualityComparer<T>.Default;
            while (size-- > 0)
            {
                if (p_arr[size] != null && equalityComparer.Equals(p_arr[size], item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 查询栈内是否存在指定元素
        /// </summary>
        /// <param name="item">要查询的元素</param>
        /// <returns>若存在返回true，不存在返回false</returns>
        public bool Contains(T item)
        {
            return this.Contains(item, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 讲元素拷贝到指定数组
        /// </summary>
        /// <param name="array">要拷贝到的目标数组</param>
        /// <param name="arrayIndex">拷贝到目标数组的起始位置</param>
        /// <exception cref="ArgumentOutOfRangeException">给定范围容量不够</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            if (arrayIndex < 0 || arrayIndex > array.Length || (array.Length - arrayIndex < p_size))
            {
                throw new ArgumentOutOfRangeException();
            }

            Array.Copy(p_arr, 0, array, arrayIndex, p_size);
            Array.Reverse(array, arrayIndex, p_size);
        }

        /// <summary>
        /// 如果缓冲区容量有空余，则将缓冲区容量收缩到与元素数匹配的容量
        /// </summary>
        /// <returns>如果容量成功收缩容量返回true，容量没有空余无法收缩则返回false</returns>
        public bool TrimExcess()
        {
            if (p_size < p_arr.Length)
            {
                T[] array = new T[p_size];
                Array.Copy(p_arr, 0, array, 0, p_size);
                p_arr = array;
                p_version++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 访问或设置内部缓冲区容量
        /// </summary>
        /// <exception cref="ArgumentException">设置的缓冲区容量小于<see cref="Count"/></exception>
        public int Capaity
        {
            get
            {
                return p_arr.Length;
            }
            set
            {
                if (value == p_arr.Length) return;

                if(value > p_arr.Length)
                {
                    T[] array = new T[value];
                    Array.Copy(p_arr, 0, array, 0, p_size);
                    p_arr = array;
                    p_version++;
                }
                else
                {
                    if(value < p_size)
                    {
                        throw new ArgumentException();
                    }

                    T[] array = new T[value];
                    Array.Copy(p_arr, 0, array, 0, p_size);
                    p_arr = array;
                    p_version++;
                }
            }
        }

        /// <summary>
        /// 仅将栈顶元素返回
        /// </summary>
        /// <returns>栈顶元素</returns>
        /// <exception cref="InvalidOperationException">栈内没有任何元素</exception>
        public T Peek()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }

            return p_arr[p_size - 1];
        }

        /// <summary>
        /// 仅将栈顶元素返回
        /// </summary>
        /// <param name="peekItem">接收返回的栈顶元素引用</param>
        /// <returns>成功返回栈顶元素返回true，若栈内没有元素则返回false</returns>
        public bool Peek(out T peekItem)
        {
            if (p_size == 0)
            {
                peekItem = default;
                return false;
            }

            peekItem = p_arr[p_size - 1];
            return true;
        }

        /// <summary>
        /// 将栈顶元素弹出并返回
        /// </summary>
        /// <returns>栈顶元素</returns>
        /// <exception cref="InvalidOperationException">栈内没有任何元素</exception>
        public T Pop()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }

            p_version++;
            T result = p_arr[p_size];
            p_size--;
            p_arr[p_size] = default(T);
            return result;
        }

        /// <summary>
        /// 将栈顶元素弹出并返回
        /// </summary>
        /// <param name="popItem">接收弹出的栈顶元素引用</param>
        /// <returns>成功弹出栈顶元素返回true，若栈内没有元素无法弹出元素则返回false</returns>
        public bool Pop(out T popItem)
        {
            if (p_size == 0)
            {
                popItem = default;
                return false;
            }

            p_version++;
            popItem = p_arr[p_size];
            p_size--;
            p_arr[p_size] = default(T);
            return true;

        }

        /// <summary>
        /// 将元素压入栈顶
        /// </summary>
        /// <param name="item">要压入的元素</param>
        public void Push(T item)
        {
            if (p_size == p_arr.Length)
            {
                T[] array = new T[(p_arr.Length == 0) ? cp_defaultCapacity : (2 * p_arr.Length)];
                Array.Copy(p_arr, 0, array, 0, p_size);
                p_arr = array;
            }

            p_arr[p_size++] = item;
            p_version++;
        }

        /// <summary>
        /// 将所有元素返回为数组
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] array = new T[p_size];
            for (int i = 0; i < p_size; i++)
            {
                array[i] = p_arr[p_size - i - 1];
            }

            return array;
        }

        /// <summary>
        /// 返回一个可循环访问的枚举器
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

    }

}
