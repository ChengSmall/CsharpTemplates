using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 循环链表节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CircularLinkedListNode<T> : IEnumerable<CircularLinkedListNode<T>>
    {

        #region 构造

        /// <summary>
        /// 实例化一个环形链表节点
        /// </summary>
        public CircularLinkedListNode()
        {
            p_value = default;
            p_previous = this;
            p_next = this;
        }

        /// <summary>
        /// 实例化一个环形链表节点
        /// </summary>
        /// <param name="value">当前节点的值</param>
        public CircularLinkedListNode(T value)
        {
            p_value = value;
            p_previous = this;
            p_next = this;
        }

        #endregion

        #region 参数

#if DEBUG
        /// <summary>
        /// 前一个节点
        /// </summary>
#endif
        private CircularLinkedListNode<T> p_previous;

#if DEBUG
        /// <summary>
        /// 后一个节点
        /// </summary>
#endif
        private CircularLinkedListNode<T> p_next;

        private T p_value;

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 当前节点的值
        /// </summary>
        public T Value
        {
            get => p_value;
            set => p_value = value;
        }

        /// <summary>
        /// 该节点的上一个节点
        /// </summary>
        /// <value>链表节点的上一个节点，如果该节点不存在另外节点，该参数返回自身</value>
        public CircularLinkedListNode<T> Previous
        {
            get => p_previous;
        }

        /// <summary>
        /// 该节点的下一个节点
        /// </summary>
        /// <value>链表节点的下一个节点，如果该节点不存在另外节点，该参数返回自身</value>
        public CircularLinkedListNode<T> Next
        {
            get => p_next;
        }

        #endregion

        #region 封装

#if DEBUG
        /// <summary>
        /// 断开当前节点的前后链条并维护前后链条的通畅
        /// </summary>
#endif
        private void f_breakNow()
        {
            var lastNode = p_previous;
            var nextNode = p_next;

            bool last = lastNode != this;
            bool next = nextNode != this;

            p_previous = null;
            p_next = null;

            //判断无连接
            if ((!last) && (!next)) return;

            if (last || next)
            {
                //存在前一个或后一个
                //前连后
                lastNode.p_next = nextNode;
                //后连前
                nextNode.p_previous = lastNode;
            }

            //if (next)
            //{
            //    //存在后一个
            //    nextNode.p_previous = lastNode;
            //    lastNode.p_next = nextNode;
            //}

        }

        #endregion

        /// <summary>
        /// 将指定节点连接到该节点的下一位节点
        /// </summary>
        /// <param name="node">要连接的节点</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void LinkNext(CircularLinkedListNode<T> node)
        {
            if (node is null) throw new ArgumentNullException();
            if (node == this) return;

            //var lastNode = p_previous;
            var nextNode = p_next;

            node.f_breakNow();

            //连接新节点
            p_next = node;
            nextNode.p_previous = node;

            //新节点连接
            node.p_previous = this;
            node.p_next = nextNode;
        }

        /// <summary>
        /// 将指定节点连接到该节点的上一位节点
        /// </summary>
        /// <param name="node">要连接的指定节点</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void LinkPrevious(CircularLinkedListNode<T> node)
        {
            if (node is null) throw new ArgumentNullException();
            if (node == this) return;

            var lastNode = p_previous;
            //var nextNode = p_next;

            node.f_breakNow();

            //连接新节点
            p_previous = node;
            lastNode.p_next = node;

            //新节点连接
            node.p_next = this;
            node.p_previous = lastNode;
        }

        /// <summary>
        /// 将指定值作为节点添加到当前节点的下一位节点
        /// </summary>
        /// <param name="value">新节点内要初始化的值</param>
        /// <returns>下一位节点对象</returns>
        public CircularLinkedListNode<T> LinkNextValue(T value)
        {
            var node = new CircularLinkedListNode<T>(value);
            LinkNext(node);
            return node;
        }

        /// <summary>
        /// 将指定值作为节点添加到当前节点的上一位节点
        /// </summary>
        /// <param name="value">新节点内要初始化的值</param>
        /// <returns>上一位节点对象</returns>
        public CircularLinkedListNode<T> LinkPreviousValue(T value)
        {
            var node = new CircularLinkedListNode<T>(value);
            LinkPrevious(node);
            return node;
        }

        #region 枚举器

        public struct Enumator : IEnumerator<CircularLinkedListNode<T>>
        {
            internal Enumator(CircularLinkedListNode<T> node)
            {
                p_start = node;
                p_cur = null;
            }
            private CircularLinkedListNode<T> p_start;
            private CircularLinkedListNode<T> p_cur;

            public CircularLinkedListNode<T> Current => p_cur;

            public bool MoveNext()
            {
                if (p_start is null) throw new NotImplementedException();

                if(p_cur is null)
                {
                    p_cur = p_start;
                }
                else
                {
                    p_cur = p_cur.Next;
                }

                return true;
            }

            public void Reset()
            {
                p_cur = null;
            }

            public void Dispose()
            {
                p_cur = null;
                p_start = null;
            }
            object IEnumerator.Current => Current;
        }

        public Enumator GetEnumerator()
        {
            return new Enumator(this);
        }

        IEnumerator<CircularLinkedListNode<T>> IEnumerable<CircularLinkedListNode<T>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region 派生

        /// <summary>
        /// 返回当前节点的值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_value?.ToString();
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif