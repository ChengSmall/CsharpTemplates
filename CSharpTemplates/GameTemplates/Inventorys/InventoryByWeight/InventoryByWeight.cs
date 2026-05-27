using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Cheng.Memorys;
using Cheng.Algorithm;

namespace Cheng.GameTemplates.Inventorys.InventoryByWeights
{

    /// <summary>
    /// 物品项
    /// </summary>
    /// <typeparam name="T">物品对象类型</typeparam>
    public struct InventoryItem<T> : IEquatable<InventoryItem<T>>, IComparable<InventoryItem<T>>
    {

        /// <summary>
        /// 初始化背包一项物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="count">物品数量</param>
        public InventoryItem(T item, ulong count)
        {
            this.item = item; this.count = count;
        }

        /// <summary>
        /// 初始化背包一项物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="count">物品数量</param>
        public InventoryItem(T item, int count)
        {
            this.item = item; this.count = (uint)count;
        }

        /// <summary>
        /// 初始化背包一项物品，物品数量是1
        /// </summary>
        /// <param name="item">物品</param>
        public InventoryItem(T item)
        {
            this.item = item; count = 1;
        }

        /// <summary>
        /// 物品数量
        /// </summary>
        public ulong count;

        /// <summary>
        /// 物品
        /// </summary>
        public T item;

        public override bool Equals(object obj)
        {
            if(obj is InventoryItem<T> other)
            {
                var eq = EqualityComparer<T>.Default;
                return count == other.count && eq.Equals(item, other.item);
            }
            return false;
        }

        public bool Equals(InventoryItem<T> other)
        {
            var eq = EqualityComparer<T>.Default;
            return count == other.count && eq.Equals(item, other.item);
        }

        public override int GetHashCode()
        {
            if(item is object)
            {
                return item.GetHashCode() ^ count.GetHashCode();
            }
            return count.GetHashCode();
        }

        public int CompareTo(InventoryItem<T> other)
        {
            var dc = Comparer<T>.Default;
            var re = dc.Compare(item, other.item);
            if (re != 0) return re;
            return count.CompareTo(other.count);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(16);
            sb.Append(item?.ToString());
            sb.Append(" - ");
            sb.Append(count);
            return sb.ToString();
        }

    }

    /// <summary>
    /// 游戏物品栏系统 - 按重量可堆叠物品栏
    /// </summary>
    /// <typeparam name="T">背包物品类型</typeparam>
    public class InventoryByWeight<T> : IList<InventoryItem<T>>
    {

        #region 构造

        /// <summary>
        /// 实例化一个重量背包，使用默认实现的重量获取方法
        /// </summary>
        public InventoryByWeight()  : this(null, 0)
        {
        }

        /// <summary>
        /// 初始化一个指定重力获取方法的集合
        /// </summary>
        /// <param name="weightGeting">重量获取接口，null则使用默认实现</param>
        public InventoryByWeight(IGetItemWeight<T> weightGeting) : this(weightGeting, 0)
        {
        }

        /// <summary>
        /// 初始化一个指定重力获取方法的集合
        /// </summary>
        /// <param name="weightGeting">重量获取接口，null则使用默认实现</param>
        /// <param name="capacity">集合内部初始可用容量</param>
        /// <exception cref="ArgumentOutOfRangeException">初始容量小于0</exception>
        public InventoryByWeight(IGetItemWeight<T> weightGeting, int capacity)
        {
            p_items = new List<InventoryItem<T>>(capacity);
            p_totalWeight = 0;
            p_weightGeting = weightGeting ?? GetItemWeight<T>.Default;
        }

        /// <summary>
        /// 派生类实例化
        /// </summary>
        /// <param name="items">背包集合</param>
        /// <param name="totalWeight">总重量</param>
        /// <param name="weightGeting">重量计算方法接口</param>
        protected InventoryByWeight(List<InventoryItem<T>> items, ulong totalWeight, IGetItemWeight<T> weightGeting)
        {
            p_items = items;
            p_totalWeight = totalWeight;
            p_weightGeting = weightGeting;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 总重量
        /// </summary>
        protected ulong p_totalWeight;

        /// <summary>
        /// 物品集合
        /// </summary>
        protected List<InventoryItem<T>> p_items;

        /// <summary>
        /// 重量获取方法
        /// </summary>
        protected IGetItemWeight<T> p_weightGeting;

        #endregion

        #region 功能

        #region 参数获取

        /// <summary>
        /// 重量获取方法的接口
        /// </summary>
        public IGetItemWeight<T> GetingItemWeight
        {
            get => p_weightGeting;
        }

        /// <summary>
        /// 该物品栏的总重量
        /// </summary>
        /// <value>参数是修改集合时实时记录的缓存数据，可调用<see cref="ResetWeight"/>重新计算总重量值</value>
        public virtual ulong TotalWeight
        {
            get => p_totalWeight;
        }

        /// <summary>
        /// 获取物品项的数量
        /// </summary>
        public virtual int Count => p_items.Count;

        bool ICollection<InventoryItem<T>>.IsReadOnly => false;

        public virtual InventoryItem<T> this[int index]
        {
            get
            {
                return p_items[index];
            }
            set
            {
                var ori = p_items[index];

                var we = p_weightGeting.GetWeight(ori.item);
                var ne = p_weightGeting.GetWeight(value.item);

                p_totalWeight = (p_totalWeight - (we * ori.count) + (ne * value.count));
                p_items[index] = value;
            }
        }

        #endregion

        #region 集合功能

        /// <summary>
        /// 获取或设置内部数据结构在不调整大小的情况下能够容纳的元素总数
        /// </summary>
        /// <value>
        /// <para>在需要调整大小之前集合可包含的元素数目</para>
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">设置了一个小于<see cref="Count"/>的数量</exception>
        public int Capacity
        {
            get => p_items.Capacity;
            set
            {
                p_items.Capacity = value;
            }
        }

        /// <summary>
        /// 添加一个物品项到物品栏
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(InventoryItem<T> item)
        {
            var we = p_weightGeting.GetWeight(item.item) * item.count;
            p_items.Add(item);
            p_totalWeight += we;
        }

        /// <summary>
        /// 移除一个物品项
        /// </summary>
        /// <param name="item">要移除的物品</param>
        /// <returns>是否移除成功</returns>
        public virtual bool Remove(InventoryItem<T> item)
        {
            bool flag = p_items.Remove(item);
            if (flag) p_totalWeight -= (p_weightGeting.GetWeight(item.item) * item.count);
            return flag;
        }

        /// <summary>
        /// 搜索物品栏内指定物品的索引项，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        public virtual int IndexOf(InventoryItem<T> item)
        {
            return (p_items).IndexOf(item);
        }

        /// <summary>
        /// 搜索物品栏内指定范围物品的索引项，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">搜索的物品个数</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int IndexOf(InventoryItem<T> item, int index, int count)
        {
            return p_items.IndexOf(item, index, count);
        }

        /// <summary>
        /// 搜索物品栏内指定范围物品项的索引，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">起始索引</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int IndexOf(InventoryItem<T> item, int index)
        {
            return p_items.IndexOf(item, index);
        }

        public virtual void Insert(int index, InventoryItem<T> item)
        {
            var we = p_weightGeting.GetWeight(item.item);
            p_items.Insert(index, item);
            p_totalWeight += (we * item.count);
        }

        /// <summary>
        /// 移除指定索引的物品项
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void RemoveAt(int index)
        {
            var re = p_items[index];
            (p_items).RemoveAt(index);
            p_totalWeight -= (p_weightGeting.GetWeight(re.item) * re.count);
        }

        /// <summary>
        /// 移出指定索引的物品项
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="reValue">移出的物品</param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void RemoveAt(int index, out InventoryItem<T> reValue)
        {
            reValue = p_items[index];
            (p_items).RemoveAt(index);
            p_totalWeight -= (p_weightGeting.GetWeight(reValue.item) * reValue.count);
        }

        /// <summary>
        /// 移除所有物品项
        /// </summary>
        public virtual void Clear()
        {
            p_items.Clear();
            p_totalWeight = 0;
        }

        public virtual bool Contains(InventoryItem<T> item)
        {
            return (p_items).Contains(item);
        }

        /// <summary>
        /// 查找物品栏中是否包含匹配的物品项，并返回
        /// </summary>
        /// <param name="predicate">谓词查找</param>
        /// <param name="reValue">返回的物品项</param>
        /// <returns>是否包含匹配的物品；包含返回true，不包含返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Contains(Predicate<InventoryItem<T>> predicate, out InventoryItem<T> reValue)
        {
            if (predicate is null) throw new ArgumentNullException();

            int length = p_items.Count;
            
            for (int i = 0; i < length; i++)
            {
                reValue = p_items[i];
                if (predicate.Invoke(reValue))
                {
                    return true;
                }
            }

            reValue = default;
            return false;
        }

        /// <summary>
        /// 查找指定匹配的物品项，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public virtual int FindIndex(Predicate<InventoryItem<T>> match)
        {
            return p_items.FindIndex(match);
        }

        /// <summary>
        /// 查找指定范围匹配的物品项，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索数量</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindIndex(Predicate<InventoryItem<T>> match, int startIndex, int count)
        {
            return p_items.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// 查找指定起始索引到最后一个物品项间匹配的物品项，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindIndex(Predicate<InventoryItem<T>> match, int startIndex)
        {
            return p_items.FindIndex(startIndex, match);
        }

        public void CopyTo(InventoryItem<T>[] array, int arrayIndex)
        {
            ((ICollection<InventoryItem<T>>)p_items).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 循环访问集合内元素
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem<T>>.Enumerator GetEnumerator()
        {
            return p_items.GetEnumerator();
        }

        IEnumerator<InventoryItem<T>> IEnumerable<InventoryItem<T>>.GetEnumerator()
        {
            return (p_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (p_items).GetEnumerator();
        }

        /// <summary>
        /// 对其中的物品项元素排序
        /// </summary>
        public virtual void Sort()
        {
            p_items.Sort();
        }

        /// <summary>
        /// 对其中的物品项元素排序
        /// </summary>
        /// <param name="comparer">指定的排序方法</param>
        public virtual void Sort(IComparer<InventoryItem<T>> comparer)
        {
            p_items.Sort(comparer);
        }

        /// <summary>
        /// 对其中指定范围的物品项排序
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">物品数量</param>
        /// <param name="comparer">排序方法，若为null则使用默认实现排序方法</param>
        public virtual void Sort(int index, int count, IComparer<InventoryItem<T>> comparer)
        {
            p_items.Sort(index, count, comparer);
        }

        /// <summary>
        /// 对其中物品项排序反转
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">数量</param>
        /// <exception cref="ArgumentException">参数错误</exception>
        public virtual void Reverse(int index, int count)
        {
            p_items.Reverse(index, count);
        }

        /// <summary>
        /// 对其中物品项排序反转
        /// </summary>
        public virtual void Reverse()
        {
            p_items.Reverse();
        }

        /// <summary>
        /// 将元素返回到新数组中
        /// </summary>
        /// <returns>新数组</returns>
        public InventoryItem<T>[] ToArray()
        {
            return p_items.ToArray();
        }

        #endregion

        #region 物品项

        /// <summary>
        /// 获取背包的物品总数量
        /// </summary>
        /// <returns>每个物品项内物品数量相加后的值</returns>
        public ulong GetItemCount()
        {
            ulong c = 0;
            var length = p_items.Count;
            for (int i = 0; i < length; i++)
            {
                var item = p_items[i];
                c += item.count;
            }
            return c;
        }

        /// <summary>
        /// 设置指定索引下物品项的物品数量
        /// </summary>
        /// <param name="index">物品项索引</param>
        /// <param name="count">要设置的新数量</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void SetCountByIndex(int index, ulong count)
        {
            var item = p_items[index];
            var we = p_weightGeting.GetWeight(item.item);

            p_items[index] = new InventoryItem<T>(item.item, count);
            p_totalWeight = p_totalWeight - (item.count * we) + (count * we);
        }

        /// <summary>
        /// 动态计算该物品栏内的总重量并返回
        /// </summary>
        public ulong CalculationWeight()
        {
            int length = p_items.Count;
            ulong we = 0;
            for (int i = 0; i < length; i++)
            {
                var item = p_items[i];
                we += (p_weightGeting.GetWeight(item.item) * item.count);
            }
            return we;
        }

        /// <summary>
        /// 重新计算集合内物品重量
        /// </summary>
        public virtual void ResetWeight()
        {
            p_totalWeight = CalculationWeight();
        }

        /// <summary>
        /// 查询指定物品对应的物品项
        /// </summary>
        /// <param name="pre">要查询匹配的函数</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>查询到的索引，没有匹配的项返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindItemIndex(Predicate<T> pre, int startIndex, int count)
        {
            if (pre is null) throw new ArgumentNullException();
            if (startIndex < 0 || count < 0 || (startIndex + count > p_items.Count)) throw new ArgumentOutOfRangeException();

            var length = count + startIndex;
            for (int i = startIndex; i < length; i++)
            {
                if (pre.Invoke(p_items[i].item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从末尾开始查询指定物品对应的物品项
        /// </summary>
        /// <param name="pre">要查询匹配的函数</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">查询的元素数量</param>
        /// <returns>查询到的索引，没有匹配的项返回-1</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindItemLastIndex(Predicate<T> pre, int startIndex, int count)
        {
            if (pre is null) throw new ArgumentNullException();
            if (startIndex < 0 || count < 0 || (startIndex + count > p_items.Count)) throw new ArgumentOutOfRangeException();

            for (int i = (count + startIndex) - 1; i >= startIndex; i--)
            {
                if (pre.Invoke(p_items[i].item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 添加一个物品项，或查找匹配的物品项并添加计数
        /// </summary>
        /// <param name="item">要添加的物品对象</param>
        /// <param name="equalityComparer">比较物品的比较器，null使用默认比较器</param>
        /// <param name="count">要添加的物品数量</param>
        /// <param name="maxCount">该类型物品的物品项中的最大物品数量，默认使用2147483647</param>
        /// <returns>返回非负数表示成功找到集合内存在的相同物品的项，并添加了指定数量；返回-1表示未找到相同类型物品项，并添加新的物品项</returns>
        /// <exception cref="ArgumentOutOfRangeException">此次要添加的数量大于最大物品数量</exception>
        public virtual int AddItem(T item, IEqualityComparer<T> equalityComparer, uint count, ulong maxCount)
        {
            if (maxCount < count) throw new ArgumentOutOfRangeException();
            if(equalityComparer is null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }

            var length = p_items.Count;
            for (int i = 0; i < length; i++)
            {
                var pair = p_items[i];
                if (equalityComparer.Equals(pair.item, item))
                {
                    //相同
                    var newC = pair.count + count;
                    var we = p_weightGeting.GetWeight(item);
                    p_totalWeight += (we * count);
                    if(newC > maxCount)
                    {
                        // 超出最大值
                        p_items[i] = new InventoryItem<T>(pair.item, maxCount);
                        p_items.Add(new InventoryItem<T>(item, newC - maxCount));
                        return i;
                    }
                    p_items[i] = new InventoryItem<T>(pair.item, newC);
                    return i;
                }
            }
            // 没有找到直接添加
            p_items.Add(new InventoryItem<T>(item, count));
            p_totalWeight += (p_weightGeting.GetWeight(item) * count);
            return -1;
        }

        /// <summary>
        /// 添加一个物品项，或查找匹配的物品项并添加计数
        /// </summary>
        /// <param name="item">要添加的物品对象</param>
        /// <param name="equalityComparer">比较物品的比较器，null使用默认比较器</param>
        /// <param name="count">要添加的物品数量</param>
        /// <returns>返回非负数表示成功找到集合内存在的相同物品的项，并添加了指定数量；返回-1表示未找到相同类型物品项，并添加新的物品项</returns>
        /// <exception cref="ArgumentOutOfRangeException">此次要添加的数量大于2147483647</exception>
        public virtual int AddItem(T item, IEqualityComparer<T> equalityComparer, uint count)
        {
            return AddItem(item, equalityComparer, count, int.MaxValue);
        }

        /// <summary>
        /// 添加一个物品项，或查找匹配的物品项并增加一个计数
        /// </summary>
        /// <param name="item">要添加的物品对象</param>
        /// <param name="equalityComparer">比较物品的比较器，null使用默认比较器</param>
        /// <returns>返回非负数表示成功找到集合内存在的相同物品的项，并添加了指定数量；返回-1表示未找到相同类型物品项，并添加新的物品项</returns>
        public int AddItem(T item, IEqualityComparer<T> equalityComparer)
        {
            return AddItem(item, equalityComparer, 1, int.MaxValue);
        }

        /// <summary>
        /// 添加一个物品项，或查找匹配的物品项并增加一个计数
        /// </summary>
        /// <param name="item">要添加的物品对象</param>
        /// <returns>返回非负数表示成功找到集合内存在的相同物品的项，并添加了指定数量；返回-1表示未找到相同类型物品项，并添加新的物品项</returns>
        public int AddItem(T item)
        {
            return AddItem(item, null, 1, int.MaxValue);
        }

        /// <summary>
        /// 添加一个新的物品项
        /// </summary>
        /// <param name="item">物品项中的物品</param>
        public virtual void AddNewItem(T item)
        {
            p_items.Add(new InventoryItem<T>(item));
            p_totalWeight += p_weightGeting.GetWeight(item);
        }

        /// <summary>
        /// 在物品项中删除指定数量物品
        /// </summary>
        /// <param name="item">要删除的匹配物品</param>
        /// <param name="equalityComparer">物品对象比较器，null使用默认比较器</param>
        /// <param name="count">要删除的物品数量</param>
        /// <returns>返回true表示成功找到匹配项并删除指定数量；false表示未找到要删除的物品</returns>
        public virtual bool RemoveItem(T item, IEqualityComparer<T> equalityComparer, uint count)
        {
            if (equalityComparer is null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }

            var length = p_items.Count;
            for (int i = 0; i < length; i++)
            {
                var pair = p_items[i];
                if (equalityComparer.Equals(pair.item, item))
                {
                    if(count < pair.count)
                    {
                        // 减少数量
                        pair.count -= count;
                        p_items[i] = pair;
                        p_totalWeight -= (count * p_weightGeting.GetWeight(pair.item));
                    }
                    else
                    {
                        // 到达已有数量后删除
                        RemoveAt(i);
                    }
                    return true;
                }
            }
            // 没有找到
            return false;
        }

        /// <summary>
        /// 删除指定物品项其中1个物品
        /// </summary>
        /// <param name="item">要删除的匹配物品</param>
        /// <param name="equalityComparer">物品对象比较器，null使用默认比较器</param>
        /// <returns>返回true表示成功找到匹配项并删除指定数量；false表示未找到要删除的物品</returns>
        public virtual bool RemoveItem(T item, IEqualityComparer<T> equalityComparer)
        {
            return RemoveItem(item, equalityComparer, 1);
        }

        /// <summary>
        /// 删除指定物品项其中1个物品
        /// </summary>
        /// <param name="item">要删除的匹配物品</param>
        /// <returns>返回true表示成功找到匹配项并删除指定数量；false表示未找到要删除的物品</returns>
        public bool RemoveItem(T item)
        {
            return RemoveItem(item, null, 1);
        }

        /// <summary>
        /// 删除指定匹配的物品项
        /// </summary>
        /// <param name="item">要删除的物品对象</param>
        /// <param name="equalityComparer">物品对象比较器，null使用默认比较器</param>
        /// <returns>成功找到并删除返回true，没有找到返回false</returns>
        public virtual bool RemoveItemAll(T item, IEqualityComparer<T> equalityComparer)
        {
            if (equalityComparer is null)
            {
                equalityComparer = EqualityComparer<T>.Default;
            }
            var length = p_items.Count;
            for (int i = 0; i < length; i++)
            {
                var pair = p_items[i];
                if (equalityComparer.Equals(pair.item, item))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            // 没有找到
            return false;
        }

        #endregion

        #endregion

    }

}
