using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.DataStructure.DynamicVariables
{

    /// <summary>
    /// 集合对象
    /// </summary>
    public sealed class DynList : DynamicObject, IList<DynVariable>, System.Collections.Generic.IReadOnlyList<DynVariable>
    {

        #region 初始化

        /// <summary>
        /// 实例化一个空集合
        /// </summary>
        public DynList()
        {
            p_list = new List<DynVariable>();
            p_open = true;
        }

        /// <summary>
        /// 实例化一个集合，指定初始容量
        /// </summary>
        /// <param name="capacity">初始容量大小</param>
        /// <exception cref="ArgumentOutOfRangeException">容量小于0</exception>
        public DynList(int capacity)
        {
            p_list = new List<DynVariable>(capacity);
            p_open = true;
        }

        /// <summary>
        /// 使用集合初始化
        /// </summary>
        /// <param name="collection">要初始化的集合</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public DynList(IEnumerable<DynVariable> collection)
        {
            if (collection is null) throw new ArgumentNullException();
            p_open = true;

            int count;
            if (collection is ICollection<DynVariable>)
            {
                count = ((ICollection<DynVariable>)collection).Count;
            }
            else
            {
                count = 4;
            }

            p_list = new List<DynVariable>(count);
            foreach (var item in collection)
            {
                p_list.Add(item ?? EmptyValue);
            }
        }

        #endregion

        #region 参数
        internal List<DynVariable> p_list;
        private bool p_open;
        #endregion

        #region 集合

        /// <summary>
        /// 当前集合是否已被锁定
        /// </summary>
        public override bool Locked
        {
            get => !p_open;
        }

        /// <summary>
        /// 将集合锁定
        /// </summary>
        /// <remarks>
        /// <para>锁定后的集合将变成只读集合，无法修改集合元素</para>
        /// </remarks>
        public override void OnLock()
        {
            p_open = false;
            if(p_list.Capacity > p_list.Count)
            {
                p_list.Capacity = p_list.Count;
            }
        }

        public DynVariable this[int index]
        {
            get
            {
                return p_list[index];
            }
            set
            {
                if (!p_open) throw new NotSupportedException();
                p_list[index] = value ?? EmptyValue;
            }
        }

        public int Count
        {
            get => p_list.Count;
        }

        public void Clear()
        {
            if (!p_open) throw new NotSupportedException();
            p_list.Clear();
        }

        public void Add(DynVariable item)
        {
            if (!p_open) throw new NotSupportedException();
            p_list.Add(item ?? EmptyValue);
        }

        /// <summary>
        /// 添加一个32位整数
        /// </summary>
        /// <param name="value"></param>
        public void Add(int value)
        {
            Add(CreateInt32(value));
        }

        /// <summary>
        /// 添加一个64位整数
        /// </summary>
        /// <param name="value"></param>
        public void Add(long value)
        {
            Add(CreateInt64(value));
        }

        /// <summary>
        /// 添加一个单精度浮点数
        /// </summary>
        /// <param name="value"></param>
        public void Add(float value)
        {
            Add(CreateFloat(value));
        }

        /// <summary>
        /// 添加一个双精度浮点数
        /// </summary>
        /// <param name="value"></param>
        public void Add(double value)
        {
            Add(CreateDouble(value));
        }

        /// <summary>
        /// 添加一个字符串
        /// </summary>
        /// <param name="value">要添加的字符串，参数是null将判断为空字符串</param>
        public void Add(string value)
        {
            Add(CreateString(value));
        }

        /// <summary>
        /// 添加一个布尔值
        /// </summary>
        /// <param name="value">要添加的值</param>
        public void Add(bool value)
        {
            Add((value) ? BooleanTrue : BooleanFalse);
        }

        /// <summary>
        /// 添加一个空值
        /// </summary>
        public void AddEmpty()
        {
            if (!p_open) throw new NotSupportedException();
            p_list.Add(EmptyValue);
        }

        public void Insert(int index, DynVariable item)
        {
            if (!p_open) throw new NotSupportedException();
            p_list.Insert(index, item ?? EmptyValue);
        }

        /// <summary>
        /// 将一个集合添加到当前集合
        /// </summary>
        /// <param name="collection">集合数据</param>
        public void AddRange(IEnumerable<DynVariable> collection)
        {
            if (!p_open) throw new NotSupportedException();
            if (collection is null) return;
            foreach (var item in collection)
            {
                p_list.Add(item ?? EmptyValue);
            }
        }

        public void RemoveAt(int index)
        {
            if (!p_open) throw new NotSupportedException();
            p_list.RemoveAt(index);
           
        }

        /// <summary>
        /// 从集合中删除一系列元素
        /// </summary>
        /// <param name="index">要删除元素的起始索引</param>
        /// <param name="count">要删除的元素数量</param>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="NotSupportedException">集合已经被锁定</exception>
        public void RemoveRange(int index, int count)
        {
            if (!p_open) throw new NotSupportedException();
            p_list.RemoveRange(index, count);
        }

        /// <summary>
        /// 移除与指定谓词所定义的条件相匹配的所有元素
        /// </summary>
        /// <param name="match">用于定义要移除的元素应满足的条件</param>
        /// <exception cref="NotSupportedException">集合已锁定</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void RemoveByPredicate(Predicate<DynVariable> match)
        {
            if (!p_open) throw new NotSupportedException();
            p_list.RemoveAll(match);
        }

        /// <summary>
        /// 将当前集合元素拷贝到指定数组
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="ArgumentException">参数错误</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void CopyTo(DynVariable[] array)
        {
            p_list.CopyTo(array);
        }

        public void CopyTo(DynVariable[] array, int arrayIndex)
        {
            p_list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 从目标数组指定的索引处开始，拷贝当前数组元素
        /// </summary>
        /// <param name="index">要拷贝的当前集合的起始索引</param>
        /// <param name="array">要拷贝到的数组</param>
        /// <param name="arrayIndex">目标数组的起始位置</param>
        /// <param name="count">要拷贝的元素数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数错误，索引超出范围</exception>
        public void CopyTo(int index, DynVariable[] array, int arrayIndex, int count)
        {
            p_list.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// 将集合元素拷贝的新数组
        /// </summary>
        /// <returns>包含集合元素的新数组</returns>
        public DynVariable[] ToArray()
        {
            return p_list.ToArray();
        }

        #region 查询

        public int IndexOf(DynVariable item)
        {
            return IndexOf(item, null, 0, p_list.Count);
        }

        /// <summary>
        /// 查询集合中的元素
        /// </summary>
        /// <param name="item">元素</param>
        /// <param name="comparer">查询比较器，null使用默认实现的比较器</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        public int IndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer)
        {
            return IndexOf(item, comparer, 0, p_list.Count);
        }

        /// <summary>
        /// 查询集合中的元素
        /// </summary>
        /// <param name="item">元素</param>
        /// <param name="comparer">查询比较器，null使用默认实现的比较器</param>
        /// <param name="index">起始索引</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public int IndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer, int index)
        {
            return IndexOf(item, comparer, index, p_list.Count - index);
        }

        /// <summary>
        /// 查询集合中的元素
        /// </summary>
        /// <param name="item">元素</param>
        /// <param name="comparer">查询比较器，null使用默认实现的比较器</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public int IndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer, int index, int count)
        {
            if (item is null) return -1;
            if (index < 0 || count < 0 || (index + count > p_list.Count))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (comparer is null) comparer = EqualityComparer<DynVariable>.Default;
            count = index + count;
            for (int i = index; i < count; i++)
            {
                if (comparer.Equals(item, p_list[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        public int LastIndexOf(DynVariable item)
        {
            return LastIndexOf(item, null, 0, p_list.Count);
        }

        /// <summary>
        /// 向前查询集合中的元素
        /// </summary>
        /// <param name="item">要查询的元素</param>
        /// <param name="comparer">元素比较器，null表示使用默认实现的比较器</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        public int LastIndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer)
        {
            return LastIndexOf(item, comparer, 0, p_list.Count);
        }

        /// <summary>
        /// 从指定范围的最后一个索引向前查询集合中的元素
        /// </summary>
        /// <param name="item">要查询的元素</param>
        /// <param name="comparer">元素比较器，null表示使用默认实现的比较器</param>
        /// <param name="index">要查询的从0开始的起始索引</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public int LastIndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer, int index)
        {
            return LastIndexOf(item, comparer, index, p_list.Count - index);
        }

        /// <summary>
        /// 从指定范围的最后一个索引向前查询集合中的元素
        /// </summary>
        /// <param name="item">要查询的元素</param>
        /// <param name="comparer">元素比较器，null表示使用默认实现的比较器</param>
        /// <param name="index">要查询的从0开始的起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>成功查询返回元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public int LastIndexOf(DynVariable item, IEqualityComparer<DynVariable> comparer, int index, int count)
        {
            if (item is null) return -1;
            if (index < 0 || count < 0 || (index + count > p_list.Count))
            {
                throw new ArgumentOutOfRangeException();
            }

            if (comparer is null) comparer = EqualityComparer<DynVariable>.Default;
            count = index + count - 1;

            for (int i = count; i >= index; i--)
            {
                if (comparer.Equals(item, p_list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 按指定条件搜索匹配的第一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public int FindIndex(Predicate<DynVariable> match)
        {
            return p_list.FindIndex(match);
        }

        /// <summary>
        /// 按指定条件搜索匹配的第一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <param name="index">从0开始的起始索引</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引范围</exception>
        public int FindIndex(Predicate<DynVariable> match, int index)
        {
            return p_list.FindIndex(index, match);
        }

        /// <summary>
        /// 按指定条件搜索匹配的第一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <param name="index">从0开始的起始索引</param>
        /// <param name="count">搜索的元素数量</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引范围</exception>
        public int FindIndex(Predicate<DynVariable> match, int index, int count)
        {
            return p_list.FindIndex(index, count, match);
        }

        /// <summary>
        /// 按指定条件搜索匹配的最后一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public int FindLastIndex(Predicate<DynVariable> match)
        {
            return p_list.FindLastIndex(match);
        }

        /// <summary>
        /// 按指定条件搜索匹配的最后一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <param name="index">从0开始的起始索引</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引范围</exception>
        public int FindLastIndex(Predicate<DynVariable> match, int index)
        {
            return p_list.FindLastIndex(index, match);
        }

        /// <summary>
        /// 按指定条件搜索匹配的最后一个元素索引
        /// </summary>
        /// <param name="match">条件委托实例</param>
        /// <param name="index">从0开始的起始索引</param>
        /// <param name="count">搜索的元素数量</param>
        /// <returns>匹配的元素索引，如果没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引范围</exception>
        public int FindLastIndex(Predicate<DynVariable> match, int index, int count)
        {
            return p_list.FindLastIndex(index, count, match);
        }

        #region enumator

        /// <summary>
        /// 返回一个循环访问元素的枚举器
        /// </summary>
        /// <returns></returns>
        public List<DynVariable>.Enumerator GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        IEnumerator<DynVariable> IEnumerable<DynVariable>.GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        #endregion

        bool ICollection<DynVariable>.Contains(DynVariable item)
        {
            return IndexOf(item) != -1;
        }

        bool ICollection<DynVariable>.Remove(DynVariable item)
        {
            var re = IndexOf(item);
            if (re == -1) return false;
            RemoveAt(re);
            return true;
        }

        bool ICollection<DynVariable>.IsReadOnly => !p_open;

        #endregion

        #endregion

        #region 派生

        public override DynVariableType DynType => DynVariableType.List;

        public override DynList DynamicList => this;

        public override string ToString()
        {
            return nameof(DynList);
        }

        public override DynVariable Clone()
        {
            int length = p_list.Count;
            DynList list = new DynList(length);
            for (int i = 0; i < length; i++)
            {
                list.Add(p_list[i].Clone());
            }
            return list;
        }

        #endregion

    }


}