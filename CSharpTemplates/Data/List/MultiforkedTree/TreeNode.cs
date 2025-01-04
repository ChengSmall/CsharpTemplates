using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 一个可循环遍历的多叉树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : IEnumerable<TreeNode<T>>, IEquatable<TreeNode<T>>
    {

        #region 构造

        /// <summary>
        /// 实例化一个多叉树节点
        /// </summary>
        public TreeNode()
        {
            p_nodes = new List<TreeNode<T>>();
        }

        /// <summary>
        /// 实例化一个多叉树节点
        /// </summary>
        /// <param name="value">该节点的值</param>
        public TreeNode(T value)
        {
            p_nodes = new List<TreeNode<T>>();
            p_value = value;
        }

        /// <summary>
        /// 实例化一个多叉树节点
        /// </summary>
        /// <param name="value">该节点的值</param>
        /// <param name="capacity">子节点集合的初始容量</param>
        public TreeNode(T value, int capacity)
        {
            p_nodes = new List<TreeNode<T>>(capacity);
            p_value = value;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 储存子节点的集合
        /// </summary>
        protected List<TreeNode<T>> p_nodes;

        /// <summary>
        /// 节点元素
        /// </summary>
        protected T p_value;

        /// <summary>
        /// 此节点的父节点
        /// </summary>
        protected TreeNode<T> p_parent;

        #endregion

        #region 节点功能

        /// <summary>
        /// 访问或设置此节点对象的元素
        /// </summary>
        public virtual T Value
        {
            get => p_value;
            set => p_value = value;
        }

        /// <summary>
        /// 访问或设置子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">超出索引</exception>
        /// <exception cref="ArgumentNullException">参数设为null</exception>
        public virtual TreeNode<T> this[int index]
        {
            get
            {
                return p_nodes[index];
            }
            set
            {
                if (value is null) throw new ArgumentNullException();
                p_nodes[index].p_parent = null;
                p_nodes[index] = value;
                value.p_parent = this;
            }
        }

        /// <summary>
        /// 获取此节点的子节点数量
        /// </summary>
        public virtual int Count => p_nodes.Count;

        /// <summary>
        /// 访问此节点对象的父节点
        /// </summary>
        public virtual TreeNode<T> Parent
        {
            get => p_parent;
        }

        /// <summary>
        /// 返回此节点对象的父节点所在索引
        /// </summary>
        /// <returns>此节点对象的父节点所在索引，若没有父节点则返回-1</returns>
        public virtual int ParentIndex
        {
            get
            {
                if (p_parent is null) return -1;
                return p_parent.IndexOf<TreeNode<T>>(this);
            }
        }

        /// <summary>
        /// 返回此节点的根节点
        /// </summary>
        /// <returns>循环访问父节点，直到没有父节点时，返回此处的节点对象；当此实例的父节点为null时返回实例本身</returns>
        public virtual TreeNode<T> BaseParent
        {
            get
            {
                var node = this;
                while ((object)node != null)
                {
                    node = node.p_parent;
                }
                return node;
            }
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="node"></param>
        /// <exception cref="ArgumentNullException">节点参数为null</exception>
        public virtual void Add<TN>(TN node) where TN : TreeNode<T>
        {
            if (node is null) throw new ArgumentNullException();
            p_nodes.Add(node);
            node.p_parent = this;
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="value">节点的值</param>
        public virtual void AddValue(T value)
        {
            p_nodes.Add(new TreeNode<T>(value));
        }

        /// <summary>
        /// 搜索与指定谓词匹配的子节点并返回
        /// </summary>
        /// <param name="predicate">谓词</param>
        /// <returns>指定谓词匹配的子节点，若没有则返回null</returns>
        public virtual TreeNode<T> Find(Predicate<TreeNode<T>> predicate)
        {
            return p_nodes.Find(predicate);
        }

        /// <summary>
        /// 移除所有子节点
        /// </summary>
        public virtual void Clear()
        {
            for (int i = 0; i < p_nodes.Count; i++)
            {
                p_nodes[i].p_parent = null;
            }
            p_nodes.Clear();
        }

        /// <summary>
        /// 确定某节点处于子节点中
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>存在子节点返回true，不存在返回false</returns>
        public virtual bool Contains(TreeNode<T> node)
        {
            return p_nodes.Contains(node);
        }

        /// <summary>
        /// 插入指定元素
        /// </summary>
        /// <param name="index">插入的索引</param>
        /// <param name="node">要插入的节点</param>
        /// <exception cref="ArgumentNullException">节点参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        public virtual void Insert<TN>(int index, TN node) where TN : TreeNode<T>
        {
            if (node is null) throw new ArgumentNullException();
            p_nodes.Insert(index, node);
            node.p_parent = this;
        }

        /// <summary>
        /// 插入指定元素
        /// </summary>
        /// <param name="index">插入的索引</param>
        /// <param name="value">要插入的节点值</param>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        public virtual void InsertValue(int index, T value)
        {
            Insert(index, new TreeNode<T>(value));
        }

        /// <summary>
        /// 删除指定索引子节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        public virtual void RemoveAt(int index)
        {
            var node = p_nodes[index];
            p_nodes.RemoveAt(index);
            node.p_parent = null;
        }

        /// <summary>
        /// 返回一个循环访问所有子节点的迭代器
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return p_nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 移除指定节点
        /// </summary>
        /// <param name="node">要移除的节点</param>
        /// <returns>是否成功移除</returns>
        public virtual bool Remove(TreeNode<T> node)
        {
            bool flag = p_nodes.Remove(node);
            if (flag) node.p_parent = null;
            return flag;
        }

        #region 查询

        /// <summary>
        /// 返回指定子节点索引
        /// </summary>
        /// <param name="item">指定子节点</param>
        /// <returns>指定子节点索引，无法找到则返回-1</returns>
        public virtual int IndexOf<TN>(TN item) where TN : TreeNode<T>
        {
            return p_nodes.IndexOf(item);
        }

        /// <summary>
        /// 返回指定子节点索引，从最后一个索引向前寻找
        /// </summary>
        /// <param name="item">指定子节点</param>
        /// <returns>指定子节点索引，无法找到则返回-1</returns>
        public virtual int LastIndexOf<TN>(TN item) where TN : TreeNode<T>
        {
            return p_nodes.LastIndexOf(item);
        }
        
        /// <summary>
        /// 返回指定子节点索引
        /// </summary>
        /// <param name="item">指定子节点</param>
        /// <param name="index">从该索引开始</param>
        /// <param name="count">查找的范围数量</param>
        /// <returns>指定子节点索引，无法找到则返回-1</returns>
        public virtual int IndexOf<TN>(TN item, int index, int count) where TN : TreeNode<T>
        {
            return p_nodes.IndexOf(item, index, count);
        }

        /// <summary>
        /// 返回指定子节点索引，从最后一个索引向前寻找
        /// </summary>
        /// <param name="item">指定子节点</param>
        /// <param name="index">从该索引开始</param>
        /// <param name="count">查找的范围数量</param>
        /// <returns>指定子节点索引，无法找到则返回-1</returns>
        public virtual int LastIndexOf<TN>(TN item, int index, int count) where TN : TreeNode<T>
        {
            return p_nodes.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// 查找指定范围内匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询个数</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public virtual int FindIndex<TN>(Predicate<TN> predicate, int index, int count) where TN : TreeNode<T>
        {
            if (predicate is null) throw new ArgumentNullException();
            //if (count == 0) return -1;

            if (index < 0 || (index + count) > p_nodes.Count) throw new ArgumentOutOfRangeException();

            int i;
            TN tn;
            int length = index + count;

            for (i = index; i < length; i++)
            {
                tn = p_nodes[i] as TN;
                if ((tn != null) && predicate.Invoke(tn)) return i;
            }
            return -1;
        }

        /// <summary>
        /// 反向查找指定范围内匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询个数</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public virtual int FindLastIndex<TN>(Predicate<TN> predicate, int index, int count) where TN : TreeNode<T>
        {
            if (predicate is null) throw new ArgumentNullException();
            //if (count == 0) return -1;

            if (index < 0 || (index + count) > p_nodes.Count) throw new ArgumentOutOfRangeException();

            int i;

            int length = index + count - 1;
            TN tn;
            for (i = length; i >= 0; i--)
            {
                tn = p_nodes[i] as TN;
                if ((tn != null) && predicate.Invoke(tn)) return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        public virtual int FindIndex<TN>(Predicate<TN> predicate) where TN : TreeNode<T>
        {
            return FindIndex(predicate, 0, p_nodes.Count);
        }

        /// <summary>
        /// 反向查找匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        public virtual int FindLastIndex<TN>(Predicate<TN> predicate) where TN : TreeNode<T>
        {
            return FindLastIndex(predicate, 0, p_nodes.Count);
        }

        /// <summary>
        /// 查找指定范围内匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询个数</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public virtual int FindIndex<TN>(IPredicate<TN> predicate, int index, int count) where TN : TreeNode<T>
        {
            if (predicate is null) throw new ArgumentNullException();
            //if (count == 0) return -1;
            
            if (index < 0 || (index + count) > p_nodes.Count) throw new ArgumentOutOfRangeException();

            int i;

            int length = index + count;

            for (i = index; i < length; i++)
            {
                var t = p_nodes[i] as TN;
                if (t != null && predicate.Predicate(t)) return i;
            }
            return -1;
        }

        /// <summary>
        /// 反向查找指定范围内匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <param name="index">起始索引</param>
        /// <param name="count">查询个数</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">给定索引超出范围</exception>
        public virtual int FindLastIndex<TN>(IPredicate<TN> predicate, int index, int count) where TN : TreeNode<T>
        {
            if (predicate is null) throw new ArgumentNullException();
            //if (count == 0) return -1;

            if (index < 0 || (index + count) > p_nodes.Count) throw new ArgumentOutOfRangeException();

            int i;

            int length = index + count - 1;

            for (i = length; i >= 0; i--)
            {
                var t = p_nodes[i] as TN;
                if (t != null && predicate.Predicate(t)) return i;
            }
            return -1;

        }

        /// <summary>
        /// 查找匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        public virtual int FindIndex<TN>(IPredicate<TN> predicate) where TN : TreeNode<T>
        {
            return FindIndex(predicate, 0, p_nodes.Count);
        }

        /// <summary>
        /// 反向查找匹配的子节点，并返回索引
        /// </summary>
        /// <param name="predicate">条件谓词</param>
        /// <returns>若找到匹配的子节点返回该节点的索引，若找不到返回-1</returns>
        /// <exception cref="ArgumentNullException">条件谓词参数为null</exception>
        public virtual int FindLastIndex<TN>(IPredicate<TN> predicate) where TN : TreeNode<T>
        {
            return FindLastIndex(predicate, 0, p_nodes.Count);
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 返回此节点对象元素的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_value?.ToString();
        }

        public override int GetHashCode()
        {
            return (p_value?.GetHashCode()).GetValueOrDefault();
        }

        public override bool Equals(object obj)
        {
            if(obj is TreeNode<T> tn)
            {
                var eq = EqualityComparer<T>.Default;
                return eq.Equals(p_value, tn.p_value);
            }
            return false;
        }

        public bool Equals(TreeNode<T> other)
        {
            if (other is null) return false;
            var eq = EqualityComparer<T>.Default;
            return eq.Equals(p_value, other.p_value);
        }

        #endregion

    }

}
