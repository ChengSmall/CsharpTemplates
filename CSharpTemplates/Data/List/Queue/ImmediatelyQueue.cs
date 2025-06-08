using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 能够访问任意元素的先进先出队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class ImmediatelyQueue<T> : IReadOnlyCollection<T>, IList<T>
    {

        #region 构造

        /// <summary>
        /// 实例化一个空的队列集合
        /// </summary>
        public ImmediatelyQueue()
        {
            p_array = sp_emptyArray;
        }

        /// <summary>
        /// 实例化队列集合，指定初始容量
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <exception cref="ArgumentOutOfRangeException">容量小于0</exception>
        public ImmediatelyQueue(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            if (capacity == 0) p_array = sp_emptyArray;
            else p_array = new T[capacity];
            p_head = 0;
            p_tail = 0;
            p_size = 0;
        }

        /// <summary>
        /// 使用集合初始化队列集合元素
        /// </summary>
        /// <param name="collection">要初始化到队列的集合，null表示空队列初始化</param>
        public ImmediatelyQueue(IEnumerable<T> collection)
        {
            p_size = 0;
            _version = 0;
            if (collection is null)
            {
                p_array = sp_emptyArray;
            }
            else
            {
                if (collection is ICollection<T> c)
                {
                    p_array = new T[c.Count];
                }
                else
                {
                    p_array = new T[cp_DefaultCapacity];
                }

                foreach (T item in collection)
                {
                    Enqueue(item);
                }
            }
           
        }

        #endregion

        #region 参数

        private const int cp_DefaultCapacity = 4;

        private static T[] sp_emptyArray = new T[0];

        private T[] p_array;

        /// <summary>
        /// 头索引
        /// </summary>
        private int p_head;

        /// <summary>
        /// 尾索引
        /// </summary>
        private int p_tail;

        /// <summary>
        /// 整体长度
        /// </summary>
        private int p_size;

        /// <summary>
        /// 变更参数
        /// </summary>
        [NonSerialized] private int _version;

        #endregion

        #region 功能

        #region

        internal T GetElement(int i)
        {
            return p_array[(p_head + i) % p_array.Length];
        }

        internal void SetElement(int i, T value)
        {
            p_array[(p_head + i) % p_array.Length] = value;
        }

        private void SetCapacity(int capacity)
        {
            T[] array = new T[capacity];

            if (p_size > 0)
            {
                if (p_head < p_tail)
                {
                    Array.Copy(p_array, p_head, array, 0, p_size);
                }
                else
                {
                    Array.Copy(p_array, p_head, array, 0, p_array.Length - p_head);
                    Array.Copy(p_array, 0, array, p_array.Length - p_head, p_tail);
                }
            }

            p_array = array;
            p_head = 0;
            p_tail = ((p_size != capacity) ? p_size : 0);
            _version++;
        }

        #endregion

        #region 访问

        /// <summary>
        /// 队列的元素数量
        /// </summary>
        public int Count
        {
            get
            {
                return p_size;
            }
        }

        /// <summary>
        /// 使用索引访问或设置已有元素
        /// </summary>
        /// <param name="index">从第一个元素到最后一个元素范围的索引</param>
        /// <returns>索引<paramref name="index"/>下的元素</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
                return GetElement(index);
            }
            set
            {
                if (index < 0 || index >= p_size) throw new ArgumentOutOfRangeException();
                SetElement(index, value);
            }
        }

        /// <summary>
        /// 使用索引从最末尾向前访问已有元素
        /// </summary>
        /// <param name="index">反向索引，0表示最末端元素</param>
        /// <returns>索引<paramref name="index"/>下的元素</returns>
        public T LastGetElement(int index)
        {
            if (index < 0 || index >= p_size) throw new ArgumentNullException();
            return GetElement((p_size - 1) - index);
        }

        /// <summary>
        /// 使用索引从最末尾向前设置已有元素
        /// </summary>
        /// <param name="index">反向索引，0表示最末端元素</param>
        /// <param name="item">索引<paramref name="index"/>下的元素</param>
        public void LastSetElement(int index, T item)
        {
            if (index < 0 || index >= p_size) throw new ArgumentNullException();
            SetElement((p_size - 1) - index, item);
        }

        #endregion

        #region 队列操作

        /// <summary>
        /// 清除队列内所有元素
        /// </summary>
        public void Clear()
        {
            if (p_head < p_tail)
            {
                Array.Clear(p_array, p_head, p_size);
            }
            else
            {
                Array.Clear(p_array, p_head, p_array.Length - p_head);
                Array.Clear(p_array, 0, p_tail);
            }

            p_head = 0;
            p_tail = 0;
            p_size = 0;
            _version++;
        }

        /// <summary>
        /// 从队列清除并返回最前端的元素
        /// </summary>
        /// <returns>最前端元素</returns>
        /// <exception cref="InvalidOperationException">队列没有元素</exception>
        public T Dequeue()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }

            T result = p_array[p_head];
            p_array[p_head] = default(T);
            p_head = (p_head + 1) % p_array.Length;
            p_size--;
            _version++;
            return result;
        }

        /// <summary>
        /// 获取队列最前端元素但不删除
        /// </summary>
        /// <returns>最前端元素</returns>
        /// <exception cref="InvalidOperationException">队列没有元素</exception>
        public T Peek()
        {
            if (p_size == 0)
            {
                throw new InvalidOperationException();
            }

            return p_array[p_head];
        }

        /// <summary>
        /// 确定当前队列是否包含指定元素
        /// </summary>
        /// <param name="item">要判断的元素</param>
        /// <returns>如果包含元素返回true，否则返回false</returns>
        public bool Contains(T item)
        {
            int num = p_head;
            int size = p_size;
            EqualityComparer<T> eq = EqualityComparer<T>.Default;
            while (size-- > 0)
            {

                if (eq.Equals(p_array[num], item))
                {
                    return true;
                }

                num = (num + 1) % p_array.Length;
            }

            return false;
        }

        /// <summary>
        /// 确定当前队列是否包含指定元素
        /// </summary>
        /// <param name="item">要判断的元素</param>
        /// <param name="equalityComparer">要使用的比较方法；null表示使用默认比较器</param>
        /// <returns>如果包含元素返回true，否则返回false</returns>
        public bool Contains(T item, IEqualityComparer<T> equalityComparer)
        {
            int num = p_head;
            int size = p_size;
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            while (size-- > 0)
            {
                if (eq.Equals(p_array[num], item))
                {
                    return true;
                }

                num = (num + 1) % p_array.Length;
            }

            return false;
        }

        /// <summary>
        /// 将元素推入队列
        /// </summary>
        /// <param name="item">要推入的元素</param>
        public void Enqueue(T item)
        {
            if (p_size == p_array.Length)
            {
                int num = (int)((long)p_array.Length * 200L / 100);
                if (num < p_array.Length + 4)
                {
                    num = p_array.Length + 4;
                }

                SetCapacity(num);
            }

            p_array[p_tail] = item;
            p_tail = (p_tail + 1) % p_array.Length;
            p_size++;
            _version++;
        }

        /// <summary>
        /// 将当前队列的所有元素拷贝到新的数组中
        /// </summary>
        /// <returns>包含队列所有元素的数组</returns>
        public T[] ToArray()
        {
            T[] array = new T[p_size];
            if (p_size == 0)
            {
                return array;
            }

            if (p_head < p_tail)
            {
                Array.Copy(p_array, p_head, array, 0, p_size);
            }
            else
            {
                Array.Copy(p_array, p_head, array, 0, p_array.Length - p_head);
                Array.Copy(p_array, 0, array, p_array.Length - p_head, p_tail);
            }

            return array;
        }

        /// <summary>
        /// 清除队列的剩余空余容量
        /// </summary>
        public void TrimExcess()
        {
            int num = (p_array.Length);
            if (p_size < num)
            {
                SetCapacity(p_size);
            }
        }

        /// <summary>
        /// 访问或设置队列容器的总容量
        /// </summary>
        /// <value></value>
        /// <exception cref="ArgumentOutOfRangeException">设置的容量小于当前队列元素数量</exception>
        public int Capacity
        {
            get => p_array.Length;
            set
            {
                if (value < p_size) throw new ArgumentOutOfRangeException();
                if (value == p_array.Length) return;
                SetCapacity(value);
            }
        }

        /// <summary>
        /// 查找指定元素的索引
        /// </summary>
        /// <param name="item">要查找的元素</param>
        /// <param name="equalityComparer">比较方法，null表示默认比较器</param>
        /// <returns>元素所在索引，如果没有则为-1</returns>
        public int IndexOf(T item, IEqualityComparer<T> equalityComparer)
        {
            int i = p_head;
            int index = 0;
            int size = p_size;
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            while (size-- > 0)
            {
                if (eq.Equals(p_array[i], item))
                {
                    return index;
                }

                i = (i + 1) % p_array.Length;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// 从最后一个元素向前查找指定元素的索引
        /// </summary>
        /// <param name="item">要查找的元素</param>
        /// <param name="equalityComparer">比较方法，null表示默认比较器</param>
        /// <returns>元素所在索引，如果没有则为-1</returns>
        public int LastIndexOf(T item, IEqualityComparer<T> equalityComparer)
        {
            int size = p_size;
            var eq = equalityComparer ?? EqualityComparer<T>.Default;

            for (int i = size - 1; i >= 0; i++)
            {
                if(eq.Equals(GetElement(i), item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 使用谓词查找匹配的项并返回索引
        /// </summary>
        /// <param name="predicate">要使用的匹配函数</param>
        /// <returns>第一个匹配的元素所在索引；没有返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词函数是null</exception>
        public int FindIndexOf(Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException();
            int i = p_head;
            int index = 0;
            int size = p_size;
            
            while (size-- > 0)
            {
                if (predicate.Invoke(p_array[i]))
                {
                    return index;
                }

                i = (i + 1) % p_array.Length;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// 使用谓词从最后一个元素向前查找匹配的项并返回索引
        /// </summary>
        /// <param name="predicate">要使用的匹配函数</param>
        /// <returns>最后一个匹配的元素所在索引；没有返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词函数是null</exception>
        public int LastFindIndexOf(Predicate<T> predicate)
        {
            if (predicate is null) throw new ArgumentNullException();
            int size = p_size;

            for (int i = size - 1; i >= 0; i++)
            {
                if (predicate.Invoke(GetElement(i)))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region 派生

        /// <summary>
        /// 队列枚举器
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            internal Enumerator(ImmediatelyQueue<T> q)
            {
                p_queue = q;
                p_version = p_queue._version;
                p_index = -1;
                p_cut = default(T);
            }

            private ImmediatelyQueue<T> p_queue;

            private int p_index;

            private int p_version;

            private T p_cut;

            public T Current
            {
                get
                {
                    //if (_index < 0)
                    //{
                    //    throw new InvalidOperationException();
                    //}

                    return p_cut;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    //if (_index < 0)
                    //{
                    //    throw new InvalidOperationException();
                    //}

                    return p_cut;
                }
            }

            public bool MoveNext()
            {
                if ((p_queue is null) || p_version != p_queue._version)
                {
                    throw new InvalidOperationException();
                }

                if (p_index == -2)
                {
                    return false;
                }

                p_index++;
                if (p_index == p_queue.p_size)
                {
                    p_index = -2;
                    p_cut = default(T);
                    return false;
                }

                p_cut = p_queue.GetElement(p_index);
                return true;
            }

            public void Reset()
            {

                if ((p_queue is null) || p_version != p_queue._version)
                {
                    throw new InvalidOperationException();
                }

                p_index = -1;
                p_cut = default(T);
            }

            void IDisposable.Dispose()
            {
                p_index = -2;
                p_cut = default(T);
            }

        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException();
            }

            int length = array.Length;

            if (arrayIndex < 0 || arrayIndex > array.Length || length - arrayIndex < p_size)
            {
                throw new ArgumentOutOfRangeException();
            }

            int size = (length - arrayIndex < p_size) ? (length - arrayIndex) : p_size;
            if (size != 0)
            {
                int hlen = (p_array.Length - p_head < size) ? (p_array.Length - p_head) : size;

                Array.Copy(p_array, p_head, array, arrayIndex, hlen);
                size -= hlen;
                if (size > 0)
                {
                    Array.Copy(p_array, 0, array, arrayIndex + p_array.Length - p_head, size);
                }
            }
        }

        /// <summary>
        /// 返回一个能够循环访问队列集合的枚举器
        /// </summary>
        /// <returns>用于循环访问的枚举器</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.IsReadOnly => true;

        int IList<T>.IndexOf(T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

    }

}
