using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using Cheng.DataStructure.Collections.Unitys.UnityEditors;
using UnityEditor;
#endif

namespace Cheng.DataStructure.Collections
{

#if UNITY_EDITOR
    /// <summary>
    /// Unity集合
    /// </summary>
    /// <remarks>
    /// <para>在检查器实现了GUI绘制，使用<see cref="UListGUIEditorDraw.OnGUIDraw(Rect, SerializedProperty, GUIContent, bool, string, Func{int, string}, Func{Rect, SerializedProperty, GUIContent, bool}, Func{UnityEditor.SerializedProperty, float})"/>方法可自行定义GUI绘制</para>
    /// </remarks>
    /// <typeparam name="T">元素类型</typeparam>
#else
    /// <summary>
    /// Unity集合
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
#endif
    [Serializable]
    public class UList<T> : IList<T>
    {

        #region 构造

        /// <summary>
        /// 实例化集合
        /// </summary>
        public UList()
        {
            p_arr = Array.Empty<T>();
            p_length = 0;
            p_changeCount = 0;
        }

        /// <summary>
        /// 初始化集合，并指定初始预留容量
        /// </summary>
        /// <param name="capacity">预留可填充元素容量</param>
        public UList(int capacity)
        {
            p_length = 0;
            p_changeCount = 0;
            if (capacity == 0) p_arr = Array.Empty<T>();
            else p_arr = new T[capacity];
        }

        /// <summary>
        /// 使用集合进行初始化集合
        /// </summary>
        /// <param name="collection">要添加的集合</param>
        public UList(IEnumerable<T> collection)
        {
            p_arr = Array.Empty<T>();
            p_length = 0;
            p_changeCount = 0;

            AddRange(collection);
        }

        #endregion

        #region 参数

        [SerializeField] private T[] p_arr;

        [SerializeField] private int p_length;

        [NonSerialized] private uint p_changeCount;

        #region Editor
#if UNITY_EDITOR

        /// <summary>
        /// 集合字段名称——基本数组<see cref="T"/>[]
        /// </summary>
        public const string fieldName_array = nameof(p_arr);

        /// <summary>
        /// 集合字段名称——集合元素数量<see cref="int"/>
        /// </summary>
        public const string fieldName_count = nameof(p_length);

        //public const string fieldName_changeCount = nameof(p_changeCount);

#endif
        #endregion

        #endregion

        #region 功能

        #region 集合功能

        /// <summary>
        /// 获取集合中包含的元素数
        /// </summary>
        public int Count => p_length;

        /// <summary>
        /// 获取或设置指定索引的元素
        /// </summary>
        /// <param name="index">指定索引，范围虚在[0,<see cref="Count"/>)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= p_length)
                {
                    throw new ArgumentOutOfRangeException("index", "超出索引范围");
                }
                return p_arr[index];
            }
            set
            {
                if (index < 0 || index >= p_length)
                {
                    throw new ArgumentOutOfRangeException("index", "超出索引范围");
                }
                p_changeCount++;
                p_arr[index] = value;
            }
        }

        /// <summary>
        /// 当前集合的可扩充容量
        /// </summary>
        /// <value>该值是集合内部拥有的定长数组的总长度，设置该值可在不修改当前集合元素的情况下扩容，如果值小于当前元素数则会舍弃多余的元素</value>
        /// <returns>集合内部拥有的定长数组的总长度</returns>
        public int Capacity
        {
            get => p_arr.Length;
            set
            {

                if (value == p_length) return;
                p_changeCount++;

                if (value == 0)
                {
                    p_arr = Array.Empty<T>();
                    p_length = 0;
                    return;
                }

                //新建集合
                T[] newArr = new T[value];
                //拷贝现有数据
                int len = Math.Min(value, p_length);
                Array.Copy(p_arr, 0, newArr, 0, len);
                p_length = len;
                p_arr = newArr;
            }
        }

        private void setCapacity(int value)
        {
            if (value == p_length) return;

            //新建集合
            T[] newArr = new T[value];
            //拷贝现有数据
            Array.Copy(p_arr, 0, newArr, 0, p_length);
            p_arr = newArr;
        }

        /// <summary>
        /// 将元素添加到最后一位
        /// </summary>
        /// <param name="item">要添加的值</param>
        public void Add(T item)
        {
            int c = Capacity;
            if (p_length == c)
            {
                if (p_length == 0) setCapacity(4);
                else setCapacity(c * 2);
            }

            p_arr[p_length++] = item;
            p_changeCount++;
        }

        /// <summary>
        /// 清空集合所有元素
        /// </summary>
        public void Clear()
        {
            Array.Clear(p_arr, 0, p_arr.Length);
            p_length = 0;
            p_changeCount++;
        }

        /// <summary>
        /// 清空集合并舍弃内部所有的预留空间
        /// </summary>
        public void ClearCapacity()
        {
            p_changeCount++;
            p_length = 0;
            p_arr = Array.Empty<T>();
        }

        /// <summary>
        /// 在某个索引处插入一个元素
        /// </summary>
        /// <param name="index">索引，如果该值等于元素数量则会向后添加元素</param>
        /// <param name="item">要插入的值</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定的索引超出范围");
            }
            int cap = Capacity;
            if (p_length == cap)
            {
                if (p_length == 0) setCapacity(4);
                else setCapacity(cap * 2);
            }

            p_changeCount++;

            if (index == p_length)
            {
                p_arr[p_length++] = item;
                return;
            }

            //移动后面的
            //int moveCount = (p_length - index);

            Array.Copy(p_arr, index, p_arr, index + 1, (p_length - index));

            p_arr[index] = item;

            p_length++;
        }

        /// <summary>
        /// 删除某个索引下的元素
        /// </summary>
        /// <param name="index">要删除的索引</param>
        /// <exception cref="ArgumentOutOfRangeException">元素超出范围</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= p_length)
            {
                throw new ArgumentOutOfRangeException("index", "超出索引范围");
            }

            p_changeCount++;

            if (index + 1 == p_length)
            {
                //移动
                p_arr[index] = default;
            }
            else
            {
                Array.Copy(p_arr, index + 1, p_arr, index, p_length - (index + 1));
                p_arr[p_length - 1] = default;
            }

            p_length--;

        }

        /// <summary>
        /// 从当前集合移除指定的元素
        /// </summary>
        /// <param name="item">要移除的元素</param>
        /// <returns>如果找到元素并移除，返回true；未找到指定元素返回false</returns>
        public bool Remove(T item)
        {
            var i = IndexOf(item);

            if (i < 0) return false;

            RemoveAt(i);

            return true;
        }

        /// <summary>
        /// 在集合中寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(p_arr, item, 0, p_length);
        }

        /// <summary>
        /// 在集合中寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="startIndex">从指定索引开始搜寻</param>
        /// <param name="count">要搜寻的数量</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引参数超出范围</exception>
        public int IndexOf(T item, int startIndex, int count)
        {
            if (startIndex + count > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }

            return Array.IndexOf(p_arr, item, startIndex, count);
        }

        /// <summary>
        /// 在集合中寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="equality">定义的集合元素比较器，如果是null则使用默认方法搜寻</param>
        /// <param name="startIndex">从指定索引开始搜寻</param>
        /// <param name="count">要搜寻的数量</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引参数超出范围</exception>
        public int IndexOf(T item, IEqualityComparer<T> equality, int startIndex, int count)
        {
            if (startIndex < 0 || count < 0) throw new ArgumentOutOfRangeException("index", "给定参数小于0");

            int length = startIndex + count;

            if (length > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }

            if (equality is null)
            {
                return Array.IndexOf(p_arr, item, startIndex, count);
            }

            for (int i = startIndex; i < length; i++)
            {
                if (equality.Equals(p_arr[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 在集合中寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="equality">定义的集合元素比较器，如果是null则使用默认方法搜寻</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        public int IndexOf(T item, IEqualityComparer<T> equality)
        {
            if (equality is null)
            {
                return Array.IndexOf(p_arr, item, 0, p_length);
            }

            for (int i = 0; i < p_length; i++)
            {
                if (equality.Equals(p_arr[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 在集合中从末尾开始寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        public int LastIndexOf(T item)
        {
            return Array.LastIndexOf(p_arr, item, 0, p_length);
        }

        /// <summary>
        /// 在集合中从末尾开始寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="startIndex">从指定索引开始搜寻</param>
        /// <param name="count">要搜寻的数量</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引参数超出范围</exception>
        public int LastIndexOf(T item, int startIndex, int count)
        {
            if (startIndex + count > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }

            return Array.LastIndexOf(p_arr, item, startIndex, count);
        }

        /// <summary>
        /// 在集合中从末尾开始寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="equality">定义的集合元素比较器，如果是null则使用默认方法搜寻</param>
        /// <param name="startIndex">从指定索引开始搜寻</param>
        /// <param name="count">要搜寻的数量</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引参数超出范围</exception>
        public int LastIndexOf(T item, IEqualityComparer<T> equality, int startIndex, int count)
        {
            if (startIndex < 0 || count < 0) throw new ArgumentOutOfRangeException("index", "给定参数小于0");

            int length = startIndex + count;

            if (length > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }
            if (count == 0) return -1;

            if (equality is null)
            {
                return Array.LastIndexOf(p_arr, item, startIndex, count);
            }

            for (int i = length - 1; i >= startIndex; i--)
            {
                if (equality.Equals(p_arr[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 在集合中寻找指定元素，如果成功搜寻返回则索引
        /// </summary>
        /// <param name="item">要搜寻的元素</param>
        /// <param name="equality">定义的集合元素比较器，如果是null则使用默认方法搜寻</param>
        /// <returns>该元素的索引，如果无法寻找到则返回-1</returns>
        public int LastIndexOf(T item, IEqualityComparer<T> equality)
        {
            if (equality is null)
            {
                return Array.LastIndexOf(p_arr, item, 0, p_length);
            }

            for (int i = p_length - 1; i >= 0; i--)
            {
                if (equality.Equals(p_arr[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 在集合中搜索与指定谓词匹配的元素
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索的元素数</param>
        /// <returns>匹配的元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public int FindIndex(Predicate<T> match, int startIndex, int count)
        {
            if (startIndex + count > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }

            return Array.FindIndex(p_arr, startIndex, count, match);
        }

        /// <summary>
        /// 在集合中搜索与指定谓词匹配的元素，从最后一个元素开始搜索
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索的元素数</param>
        /// <returns>匹配的元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public int FindLastIndex(Predicate<T> match, int startIndex, int count)
        {
            if (startIndex + count > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "给定参数超出范围");
            }

            return Array.FindLastIndex(p_arr, startIndex, count, match);
        }

        /// <summary>
        /// 在集合中搜索与指定谓词匹配的元素
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索的元素数</param>
        /// <returns>匹配的元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public int FindIndex(Predicate<T> match)
        {
            return Array.FindIndex(p_arr, 0, p_length, match);
        }

        /// <summary>
        /// 在集合中搜索与指定谓词匹配的元素，从最后一个元素开始搜索
        /// </summary>
        /// <param name="match">谓词</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="count">搜索的元素数</param>
        /// <returns>匹配的元素索引，没有则返回-1</returns>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public int FindLastIndex(Predicate<T> match)
        {
            return Array.FindLastIndex(p_arr, 0, p_length, match);
        }

        /// <summary>
        /// 将集合的所有元素复制到新数组中
        /// </summary>
        /// <returns>新的数组</returns>
        public T[] ToArray()
        {
            if(p_length != p_arr.Length)
            {
                T[] re = new T[p_length];
                Array.Copy(p_arr, 0, re, 0, p_length);
                return re;
            }

            return (T[])p_arr.Clone();
        }

        /// <summary>
        /// 将集合的所有元素复制到集合中
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            return new List<T>(this);
        }

        /// <summary>
        /// 确定集合中是否包含指定的值
        /// </summary>
        /// <param name="item">要确定的值</param>
        /// <returns>如果集合中包含确定的<paramref name="item"/>返回true，否则返回false</returns>
        public bool Contains(T item)
        {
            return Array.IndexOf(p_arr, item, 0, p_length) >= 0;
        }

        /// <summary>
        /// 从当前集合拷贝元素到指定数组
        /// </summary>
        /// <param name="index">当前集合拷贝的起始索引</param>
        /// <param name="array">要拷贝到的目标数组</param>
        /// <param name="arrayIndex">目标数组拷贝的起始位置</param>
        /// <param name="count">要拷贝的数量</param>
        /// <exception cref="ArgumentNullException">目标数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数错误或超出范围</exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (array is null) throw new ArgumentNullException("array", "要拷贝的数组为null");

            if(arrayIndex < 0 || count < 0 || index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "给定的参数小于0");
            }

            if ((arrayIndex + count) > array.Length || index + count > p_length)
            {
                throw new ArgumentOutOfRangeException("length", "参数超出集合或数组的范围");
            }

            Array.Copy(p_arr, index, array, arrayIndex, count);
        }

        /// <summary>
        /// 从当前集合拷贝元素到指定数组
        /// </summary>
        /// <param name="array">要拷贝到的目标数组</param>
        /// <param name="arrayIndex">目标数组拷贝的起始位置</param>
        /// <exception cref="ArgumentNullException">目标数组为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数错误或超出范围</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null) throw new ArgumentNullException("array");
            CopyTo(0, array, arrayIndex, array.Length - arrayIndex);
        }

        /// <summary>
        /// 从当前集合拷贝元素到指定数组
        /// </summary>
        /// <param name="array">要拷贝到的目标数组，拷贝的长度是目标数组长度与集合长度的较小值</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void CopyTo(T[] array)
        {
            if (array is null) throw new ArgumentNullException("array", "要拷贝的数组为null");

            var count = Math.Min(array.Length, p_length);
            Array.Copy(p_arr, 0, array, 0, count);
        }

        private void add(T item)
        {
            int c = Capacity;
            if (p_length == c)
            {
                if (p_length == 0) setCapacity(4);
                else setCapacity(c * 2);
            }

            p_arr[p_length++] = item;
        }

        /// <summary>
        /// 添加集合元素到末尾
        /// </summary>
        /// <param name="collection">集合元素</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null) return;

            int count = -1;
            if(collection is ICollection<T>)
            {
                count = ((ICollection<T>)collection).Count;
            }
            else if (collection is ICollection)
            {
                count = ((ICollection)collection).Count;
            }
            p_changeCount++;

            if (count == 0)
            {
                return;
            }
            else if (count != -1)
            {
                
                int c = Capacity;
                int endL = count + p_length;
                if (endL >= c)
                {
                    if (endL == 0) setCapacity(4);
                    else
                    {
                        int caps = c * 2;
                        if (caps > endL) setCapacity(caps);
                        else setCapacity(endL);
                    }
                }

                int i = 0;
                foreach (var tp in collection)
                {                    
                    p_arr[p_length + i] = tp;

                    i++;
                }

                p_length += count;

            }
            else
            {
                p_changeCount++;
                foreach (var item in collection)
                {
                    add(item);
                }
            }
            
        }

        /// <summary>
        /// 返回一个循环访问集合元素的枚举器
        /// </summary>
        /// <returns>可循环访问集合元素的枚举器</returns>
        public Enumator GetEnumerator()
        {
            return new Enumator(this);
        }

        /// <summary>
        /// 在指定索引处插入一个集合的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="collection">要插入的集合</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if(index < 0 || index > p_length)
            {
                throw new ArgumentOutOfRangeException("index", "超出索引范围");
            }

            if (collection is null) return;

            if(index == p_length)
            {
                AddRange(collection);
                return;
            }
            
            int count = -1;
            if (collection is ICollection<T>)
            {
                count = ((ICollection<T>)collection).Count;
            }

            p_changeCount++;

            if (count == 0)
            {
                return;
            }
            else if(count > 0)
            {
                int c = Capacity;
                int endL = count + p_length;
                if (endL >= c)
                {
                    if (endL == 0) setCapacity(4);
                    else
                    {
                        int caps = c * 2;
                        if (caps > endL) setCapacity(caps);
                        else setCapacity(endL);
                    }
                }
                
                //向后挪
                Array.Copy(p_arr, index, p_arr, index + count, p_length - (index));

                int i = index;
                foreach (var item in collection)
                {
                    p_arr[i] = item;
                    i++;
                }

                p_length += count;

            }
            else
            {
                int i = index;
                foreach (var item in collection)
                {
                    Insert(i, item);
                    i++;
                }

            }

        }

        /// <summary>
        /// 对数组排序
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">排序的元素数量</param>
        /// <param name="comparer">数据比较器</param>
        /// <exception cref="ArgumentException">参数超出范围或发生错误</exception>
        /// <exception cref="InvalidOperationException">用于排序的比较器实现有错误或冲突</exception>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if(index < 0 || count < 0 || (index + count) > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            p_changeCount++;
            Array.Sort<T>(p_arr, index, count, comparer);
        }

        /// <summary>
        /// 对数组排序
        /// </summary>
        /// <param name="comparer">数据比较器</param>
        /// <exception cref="InvalidOperationException">用于排序的比较器实现有错误或冲突</exception>
        public void Sort(IComparer<T> comparer)
        {
            p_changeCount++;
            Array.Sort<T>(p_arr, 0, p_length, comparer);
        }

        /// <summary>
        /// 对数组排序
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">排序的元素数量</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围或发生错误</exception>
        public void Sort(int index, int count)
        {
            if (index < 0 || count < 0 || (index + count) > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            p_changeCount++;
            Array.Sort<T>(p_arr, index, count);
        }

        /// <summary>
        /// 对数组排序
        /// </summary>
        /// <exception cref="InvalidOperationException">默认的排序器没有被类型<typeparamref name="T"/>实现</exception>
        public void Sort()
        {
            p_changeCount++;
            Array.Sort(p_arr, 0, p_length);
        }

        /// <summary>
        /// 反转当前集合的所有元素的顺序
        /// </summary>
        public void Reverse()
        {
            p_changeCount++;
            Array.Reverse(p_arr, 0, p_length);
        }

        /// <summary>
        /// 反转集合内部分元素的顺序
        /// </summary>
        /// <param name="index">起始索引</param>
        /// <param name="count">要反转的元素数量</param>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public void Reverse(int index, int count)
        {
            if (index < 0 || count < 0 || (index + count) > p_length)
            {
                throw new ArgumentOutOfRangeException();
            }
            p_changeCount++;
            Array.Reverse(p_arr, index, count);
        }

        #endregion

        #region 派生

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<T>.IsReadOnly => false;

        #endregion

        #region 结构

        /// <summary>
        /// 集合枚举器
        /// </summary>
        public struct Enumator : IEnumerator<T>
        {
            internal Enumator(UList<T> list)
            {
                p_list = list;
                p_ch = list.p_changeCount;
                value = default;
                p_index = -1;
            }

            private UList<T> p_list;
            private int p_index;
            private uint p_ch;
            private T value;

            /// <summary>
            /// 当前枚举器访问到的集合元素
            /// </summary>
            public T Current => value;

            /// <summary>
            /// 将枚举数推进到下一个元素，如果到达末尾则返回false
            /// </summary>
            /// <returns>如果成功推进到下一个元素，返回true；推进到末尾则返回false</returns>
            /// <exception cref="InvalidOperationException">集合发生改变</exception>
            public bool MoveNext()
            {
                if (p_list is null) throw new ArgumentException();
                if(p_ch != p_list.p_changeCount)
                {
                    throw new InvalidOperationException("无法对已经发生改变的集合进行访问");
                }

                value = default;

                if (p_index >= p_list.p_length) return false;

                p_index++;

                if (p_index >= p_list.p_length) return false;

                value = p_list.p_arr[p_index];
                return true;
            }

            /// <summary>
            /// 将枚举数设置为初始值
            /// </summary>
            /// <exception cref="InvalidOperationException">集合发生改变</exception>
            public void Reset()
            {
                if (p_list is null) throw new ArgumentException();
                if (p_ch != p_list.p_changeCount)
                {
                    throw new InvalidOperationException("无法对已经发生改变的集合进行访问");
                }
                p_index = -1;
            }

            object IEnumerator.Current => Current;
            void IDisposable.Dispose()
            {
            }

        }

        #endregion

        #endregion

    }

}
