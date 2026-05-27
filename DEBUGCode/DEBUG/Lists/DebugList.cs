using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.DEBUG
{


    public class DebugList<T> : IList<T>
    {

        #region 构造
        /// <summary>
        /// 封装集合实例
        /// </summary>
        /// <param name="list"></param>
        public DebugList(IList<T> list)
        {
            this.list = list ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 构造<see cref="List{T}"/>实例
        /// </summary>
        public DebugList()
        {
            list = new List<T>();
        }

        #endregion

        #region 参数
        /// <summary>
        /// 内部集合
        /// </summary>
        public readonly IList<T> list;

        #endregion

        #region 派生
        public T this[int index] 
        {
            get => list[index]; 
            set => list[index] = value;
        }

        public int Count {
            get
            {
                return list.Count;
            }
        }

        public bool IsReadOnly {
            get
            {
                return list.IsReadOnly;
            }
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        

        #endregion

    }


    public class DebugArrayList : System.Collections.IList
    {

        #region 构造

        public DebugArrayList(IList list)
        {
            this.list = list ?? throw new ArgumentNullException();
        }

        public DebugArrayList()
        {
            list = new ArrayList();
        }

        #endregion

        #region 参数

        private IList list;

        #endregion

        #region 派生

        public bool IsReadOnly
        {
            get
            {
                return list.IsReadOnly;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return list.IsFixedSize;
            }
        }

        public int Count
        {
            get
            {

                return list.Count;
            }
        }

        public object SyncRoot 
        {
            get
            {
                return list.SyncRoot;
            }
        }

        public bool IsSynchronized 
        {
            get
            {
                return list.IsSynchronized;
            }
        }

        public object this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public int Add(object value)
        {
            return list.Add(value);
        }

        public bool Contains(object value)
        {
            return list.Contains(value);
        }

        public void Clear()
        {
            list.Clear();
        }

        public int IndexOf(object value)
        {
            return list.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            list.Insert(index, value);
        }

        public void Remove(object value)
        {
            list.Remove(value);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            list.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

    }

}
