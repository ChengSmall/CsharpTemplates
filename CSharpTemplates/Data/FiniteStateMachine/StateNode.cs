using System;
using System.Collections.Generic;
using System.Collections;
using Cheng.Memorys;

namespace Cheng.DataStructure.FiniteStateMachine
{

    /// <summary>
    /// 有限状态机节点事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node">引发事件的状态节点</param>
    public delegate void StateNodeChangeEvent<T>(StateNode<T> node);

    /// <summary>
    /// 有限状态机节点事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    /// <param name="node">引发事件的状态节点</param>
    /// <param name="obj">事件参数</param>
    public delegate void StateNodeChangeEvent<T, in P>(StateNode<T> node, P obj);

    /// <summary>
    /// 表示一个有限状态机的状态节点
    /// </summary>
    /// <typeparam name="T">表示状态的参数类型</typeparam>
    public sealed class StateNode<T> : SafreleaseUnmanagedResources, IComparable<StateNode<T>>
    {

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                this.NodeAddEvent = null;
                this.NodeRemoveEvent = null;
            }
            return true;
        }

        #endregion

        #region 构造
        /// <summary>
        /// 实例化一个节点
        /// </summary>
        /// <param name="state">节点状态</param>
        public StateNode(T state)
        {
            p_state = state;
            p_links = new List<StateNode<T>>();
        }

        #endregion

        #region 参数

        private T p_state;

        private List<StateNode<T>> p_links;

        #endregion

        #region 功能

        #region 参数访问
        /// <summary>
        /// 访问或设置该状态节点的状态
        /// </summary>
        public T State
        {
            get => p_state;
            set => p_state = value;
        }

        /// <summary>
        /// 按索引访问状态机连接的其它节点
        /// </summary>
        /// <param name="index">连接索引</param>
        /// <returns>指定索引下的其它节点</returns>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public StateNode<T> this[int index]
        {
            get => p_links[index];
        }
        
        /// <summary>
        /// 该节点连接的其它节点数量
        /// </summary>
        public int Count
        {
            get => p_links.Count;
        }

        #endregion

        #region 节点连接

        private void f_notNullAndThis(StateNode<T> node)
        {
            if (node is null) throw new ArgumentNullException();
            if (node == this) throw new ArgumentException("节点不得是自身");
        }

        /// <summary>
        /// 将指定节点连接至指定索引下
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="node">节点</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        public void Insert(int index, StateNode<T> node)
        {
            f_notNullAndThis(node);
            p_links.Insert(index, node);
            NodeAddEvent?.Invoke(this, node);
        }

        /// <summary>
        /// 在此节点下连接一个节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <exception cref="ArgumentNullException">节点为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        public void Add(StateNode<T> node)
        {
            f_notNullAndThis(node);
            p_links.Add(node);
            NodeAddEvent?.Invoke(this, node);
        }

        /// <summary>
        /// 在此节点下断开一个节点
        /// </summary>
        /// <param name="node">要断开的节点</param>
        /// <returns>是否成功断开，若找到节点则断开并返回true，没有找到返回false</returns>
        /// <exception cref="ArgumentNullException">节点为null</exception>
        public bool Remove(StateNode<T> node)
        {
            f_notNullAndThis(node);
            bool flag = p_links.Remove(node);
            if (flag) NodeRemoveEvent?.Invoke(this, node);
            return flag;
        }

        /// <summary>
        /// 弹出指定连接索引的节点
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <returns>弹出的节点</returns>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public StateNode<T> PopAt(int index)
        {
            var node = p_links[index];
            NodeRemoveEvent?.Invoke(this, node);
            p_links.RemoveAt(index);
            return node;
        }

        /// <summary>
        /// 移除连接在指定索引处的状态节点
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveAt(int index)
        {
            var node = p_links[index];
            NodeRemoveEvent?.Invoke(this, node);
            p_links.RemoveAt(index);
        }

        /// <summary>
        /// 在此节点连接处寻找某一个节点
        /// </summary>
        /// <param name="node">要寻找的节点</param>
        /// <returns>是否找到</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Contains(StateNode<T> node)
        {
            if (node is null) throw new ArgumentNullException();
            return p_links.Contains(node);
        }

        /// <summary>
        /// 判断此节点是否有某一个元素匹配
        /// </summary>
        /// <param name="match">谓词</param>
        /// <returns>有匹配项返回true，没有匹配项返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Contains(Predicate<StateNode<T>> match)
        {
            return p_links.FindIndex(match) >= 0;
        }

        /// <summary>
        /// 在该节点连接处寻找指定条件的节点
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>返回第一个匹配的节点，若没有匹配的节点则返回null</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public StateNode<T> Find(Predicate<StateNode<T>> match)
        {
            return p_links.Find(match);
        }

        /// <summary>
        /// 在该节点连接处寻找指定条件的节点并返回素银
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>返回第一个匹配的节点索引，若没有匹配的节点则返回-1</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public int FindIndex(Predicate<StateNode<T>> match)
        {
            return p_links.FindIndex(match);
        }

        /// <summary>
        /// 在该节点连接处寻找指定条件的节点
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <param name="node">获取第一个匹配的节点，若没有匹配的节点则为null</param>
        /// <returns>是否找到匹配，找到匹配节点返回true，没有找到返回false</returns>
        /// <exception cref="ArgumentNullException">谓词为null</exception>
        public bool Find(Predicate<StateNode<T>> match, out StateNode<T> node)
        {
            if (match is null) throw new ArgumentNullException();
            var list = p_links;
            int length = list.Count;
            int i;
            StateNode<T> tn;

            for (i = 0; i < length; i++)
            {
                tn = list[i];
                if (match.Invoke(tn))
                {
                    node = tn;
                    return true;
                }
            }
            node = null;
            return false;
        }

        /// <summary>
        /// 清空连接处节点
        /// </summary>
        public void Clear()
        {
            p_links.ForEach(f_clearAction);
            p_links.Clear();
        }

        /// <summary>
        /// 确定每个连接的节点与指定谓词匹配
        /// </summary>
        /// <param name="match">条件谓词</param>
        /// <returns>若每个连接的节点与指定谓词匹配，返回true；否则返回false；若没有连接节点，返回true</returns>
        public bool TrueForAll(Predicate<StateNode<T>> match)
        {
            return p_links.TrueForAll(match);
        }

        void f_clearAction(StateNode<T> node)
        {
            NodeRemoveEvent?.Invoke(this, node);
        }

        /// <summary>
        /// 将连接的节点按默认比较排序
        /// </summary>
        public void Sort()
        {
            p_links.Sort();
        }

        /// <summary>
        /// 将连接的节点排序
        /// </summary>
        /// <param name="comparer">指定排序方法</param>
        public void Sort(IComparer<StateNode<T>> comparer)
        {
            p_links.Sort(comparer);
        }

        /// <summary>
        /// 节点默认比较实现
        /// </summary>
        /// <remarks>使用状态参数的默认实现比较</remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(StateNode<T> other)
        {
            if (other is null) return 1;
            var comp = Comparer<T>.Default;
            return comp.Compare(this.p_state, other.p_state);
        }

        /// <summary>
        /// 搜索指定节点是否处于该节点的连接节点中，并返回索引
        /// </summary>
        /// <param name="node">要搜索的节点</param>
        /// <returns>如果找到节点返回节点所在索引，没有找到返回-1</returns>
        public int IndexOf(StateNode<T> node)
        {
            return p_links.IndexOf(node);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 在新连接节点时引发的事件，参数为新添加的节点
        /// </summary>
        public event StateNodeChangeEvent<T, StateNode<T>> NodeAddEvent;

        /// <summary>
        /// 在断开节点时引发的事件，参数为要断开的节点
        /// </summary>
        public event StateNodeChangeEvent<T, StateNode<T>> NodeRemoveEvent;

        #endregion

        #endregion

    }

}
