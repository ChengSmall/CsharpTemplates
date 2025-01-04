using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.GameTemplates.Inventorys.Weights
{

    /// <summary>
    /// 游戏物品栏---重量背包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InventoryByWeight<T> : IList<T>
    {

        #region 构造

        /// <summary>
        /// 实例化一个重量背包，使用默认实现的重量获取方法
        /// </summary>
        public InventoryByWeight()
        {
            p_items = new List<T>();
            p_totalWeight = 0;
            p_weightGeting = GetItemWeight<T>.Default;
        }

        /// <summary>
        /// 初始化一个指定重力获取方法的集合
        /// </summary>
        /// <param name="weightGeting">重量获取接口，null则使用默认实现</param>
        public InventoryByWeight(IGetItemWeight<T> weightGeting)
        {
            p_items = new List<T>();
            p_totalWeight = 0;
            p_weightGeting = weightGeting ?? GetItemWeight<T>.Default;
        }

        /// <summary>
        /// 派生类实例化
        /// </summary>
        /// <param name="items">背包集合</param>
        /// <param name="totalWeight">总重量</param>
        /// <param name="weightGeting">重量计算方法接口</param>
        protected InventoryByWeight(List<T> items, ulong totalWeight, IGetItemWeight<T> weightGeting)
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
        protected List<T> p_items;
        /// <summary>
        /// 重量获取方法
        /// </summary>
        protected IGetItemWeight<T> p_weightGeting;
        #endregion

        #region 功能

        #region 参数获取

        /// <summary>
        /// 获取重量获取接口实例
        /// </summary>
        public IGetItemWeight<T> GetingItemWeight
        {
            get => p_weightGeting;
        }

        /// <summary>
        /// 该物品栏的总重量
        /// </summary>
        public virtual ulong TotalWeight
        {
            get => p_totalWeight;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        public virtual int Count => p_items.Count;

        bool ICollection<T>.IsReadOnly => false;

        public virtual T this[int index]
        {
            get
            {
                return p_items[index];
            }
            set
            {
                T ori = p_items[index];

                var we = p_weightGeting.GetWeight(ori);
                var ne = p_weightGeting.GetWeight(value);
                var or = p_totalWeight - we + ne;

                p_totalWeight = or;

                p_items[index] = value;
            }
        }

        /// <summary>
        /// 动态计算该物品栏内的总重量并返回
        /// </summary>
        public ulong CalculationWeight
        {
            get
            {
                int length = p_items.Count;
                ulong we = 0;
                for (int i = 0; i < length; i++)
                {
                    we += p_weightGeting.GetWeight(p_items[i]);
                }
                return we;
            }
        }

        #endregion

        #region 集合功能
        /// <summary>
        /// 重新计算集合内物品重量
        /// </summary>
        public virtual void ResetWeight()
        {
            p_totalWeight = CalculationWeight;
        }

        /// <summary>
        /// 添加一个物品到物品栏
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            var we = p_weightGeting.GetWeight(item);
            p_items.Add(item);
            p_totalWeight += we;
        }

        /// <summary>
        /// 移除一个物品到物品栏
        /// </summary>
        /// <param name="item">要移除的物品</param>
        /// <returns>是否移除成功</returns>
        public virtual bool Remove(T item)
        {
            bool flag = p_items.Remove(item);

            if (flag) p_totalWeight -= p_weightGeting.GetWeight(item);

            return flag;
        }

        /// <summary>
        /// 搜索物品栏内指定物品的索引，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        public virtual int IndexOf(T item)
        {
            return (p_items).IndexOf(item);
        }

        /// <summary>
        /// 搜索物品栏内指定范围物品的索引，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">搜索的物品个数</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int IndexOf(T item, int index, int count)
        {
            return p_items.IndexOf(item, index, count);
        }

        /// <summary>
        /// 搜索物品栏内指定范围物品的索引，若没有则返回-1
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">起始索引</param>
        /// <returns>第一个物品所在位置，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int IndexOf(T item, int index)
        {
            return p_items.IndexOf(item, index);
        }


        public virtual void Insert(int index, T item)
        {
            var we = p_weightGeting.GetWeight(item);
            p_items.Insert(index, item);
            p_totalWeight += we;
        }

        /// <summary>
        /// 移除指定索引的物品
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void RemoveAt(int index)
        {
            T re = p_items[index];
            (p_items).RemoveAt(index);
            p_totalWeight -= p_weightGeting.GetWeight(re);
        }
        /// <summary>
        /// 移出指定索引的物品
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="reValue">移出的物品</param>
        /// <exception cref="ArgumentException">索引超出范围</exception>
        public virtual void RemoveAt(int index, out T reValue)
        {
            reValue = p_items[index];
            (p_items).RemoveAt(index);
            p_totalWeight -= p_weightGeting.GetWeight(reValue);
        }
        /// <summary>
        /// 移除所有物品
        /// </summary>
        public virtual void Clear()
        {
            p_items.Clear();
            p_totalWeight = 0;
        }

        public virtual bool Contains(T item)
        {
            return (p_items).Contains(item);
        }

        /// <summary>
        /// 查找物品栏中是否包含匹配的物品，并返回
        /// </summary>
        /// <param name="predicate">谓词查找</param>
        /// <param name="reValue">返回的物品</param>
        /// <returns>是否包含匹配的物品；包含返回true，不包含返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool Contains(Predicate<T> predicate, out T reValue)
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
        /// 查找指定匹配的物品，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public virtual int FindIndex(Predicate<T> match)
        {
            return p_items.FindIndex(match);
        }

        /// <summary>
        /// 查找指定范围匹配的物品，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索数量</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindIndex(Predicate<T> match, int startIndex, int count)
        {
            return p_items.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// 查找指定起始索引到最后一个物品间匹配的物品，并返回索引
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <returns>搜索到的物品索引，没有则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public virtual int FindIndex(Predicate<T> match, int startIndex)
        {
            return p_items.FindIndex(startIndex, match);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)p_items).CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return (p_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (p_items).GetEnumerator();
        }

        /// <summary>
        /// 对其中的物品元素排序
        /// </summary>
        public virtual void Sort()
        {
            p_items.Sort();
        }

        /// <summary>
        /// 对其中的物品元素排序
        /// </summary>
        /// <param name="comparer">指定的排序方法</param>
        public virtual void Sort(IComparer<T> comparer)
        {
            p_items.Sort(comparer);
        }

        /// <summary>
        /// 对其中指定范围的物品排序
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">物品数量</param>
        /// <param name="comparer">排序方法，若为null则使用默认实现排序方法</param>
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            p_items.Sort(index, count, comparer);
        }

        /// <summary>
        /// 对其中物品排序反转
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">数量</param>
        /// <exception cref="ArgumentException">参数错误</exception>
        public virtual void Reverse(int index, int count)
        {
            p_items.Reverse(index, count);
        }

        /// <summary>
        /// 对其中物品排序反转
        /// </summary>
        public virtual void Reverse()
        {
            p_items.Reverse();
        }

        /// <summary>
        /// 将元素返回到新数组中
        /// </summary>
        /// <returns>新数组</returns>
        public T[] ToArray()
        {
            return p_items.ToArray();
        }

        #endregion

        #endregion

    }

}
