using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 可循环访问的二叉树节点
    /// </summary>
    public class BinaryTreeNode<T> : IEnumerable<BinaryTreeNode<T>>
    {

        #region 构造
        /// <summary>
        /// 实例化一个二叉树节点
        /// </summary>
        public BinaryTreeNode()
        {
            p_value = default;
            p_leftNode = null;
            p_rightNode = null;
            p_parent = null;
        }
        /// <summary>
        /// 实例化一个二叉树节点
        /// </summary>
        /// <param name="value">节点元素</param>
        public BinaryTreeNode(T value)
        {
            p_value = value;
            p_leftNode = null;
            p_rightNode = null;
            p_parent = null;
        }
        /// <summary>
        /// 实例化一个二叉树节点
        /// </summary>
        /// <param name="left">左侧子节点</param>
        /// <param name="right">右侧子节点</param>
        public BinaryTreeNode(BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            p_value = default;
            p_parent = null;
            LeftNode = left;
            RightNode = right;
        }
        /// <summary>
        /// 实例化一个二叉树节点
        /// </summary>
        /// <param name="value">节点元素</param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="left">左侧子节点</param>
        /// <param name="right">右侧子节点</param>
        public BinaryTreeNode(T value, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
        {
            p_value = value;
            p_parent = null;
            LeftNode = left;
            RightNode = right;
        }
        #endregion

        #region 参数
        /// <summary>
        /// 该节点元素
        /// </summary>
        protected T p_value;
        /// <summary>
        /// 该节点左侧子节点
        /// </summary>
        protected BinaryTreeNode<T> p_leftNode;
        /// <summary>
        /// 该节点右侧子节点
        /// </summary>
        protected BinaryTreeNode<T> p_rightNode;
        /// <summary>
        /// 该节点的父节点
        /// </summary>
        protected BinaryTreeNode<T> p_parent;
        #endregion

        #region 功能

        #region 访问
        /// <summary>
        /// 访问或设置节点元素
        /// </summary>
        public virtual T Value
        {
            get => p_value;
            set => p_value = value;
        }
        /// <summary>
        /// 访问或设置左侧子节点
        /// </summary>
        public virtual BinaryTreeNode<T> LeftNode
        {
            get => p_leftNode;
            set
            {
                if (value is null)
                {
                    if ((object)p_leftNode != null) p_leftNode.p_parent = null;
                    return;
                }

                p_leftNode = value;
                value.p_parent = this;
            }
        }
        /// <summary>
        /// 访问或设置右侧子节点
        /// </summary>
        public virtual BinaryTreeNode<T> RightNode
        {
            get => p_rightNode;
            set
            {
                
                if (value is null)
                {
                    if((object)p_rightNode != null) p_rightNode.p_parent = null;
                    return;
                }

                p_rightNode = value;
                value.p_parent = this;
            }
        }
        /// <summary>
        /// 使用索引访问或设置子节点
        /// </summary>
        /// <param name="index">0表示左侧节点，1表示右侧节点</param>
        /// <returns>子节点</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public BinaryTreeNode<T> this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return LeftNode;
                    case 1:
                        return RightNode;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        LeftNode = value;
                        break;
                    case 1:
                        RightNode = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        /// <summary>
        /// 访问该节点的父节点，若没有父节点则为null
        /// </summary>
        public BinaryTreeNode<T> Parent
        {
            get => p_parent;
        }
        #endregion

        #region 操作
        /// <summary>
        /// 返回该节点的顶级父节点
        /// </summary>
        /// <returns>循环访问父节点，直到没有父节点后，返回此处节点对象</returns>
        public BinaryTreeNode<T> BaseParent
        {
            get
            {
                BinaryTreeNode<T> node = this;

                while ((object)node.p_parent != null)
                {
                    node = node.p_parent;
                }

                return node;
            }
        }

        /// <summary>
        /// 移除该节点下的两个子节点
        /// </summary>
        public void Clear()
        {
            p_leftNode.p_parent = null;
            p_leftNode = null;
            p_rightNode.p_parent = null;
            p_rightNode = null;
        }
        /// <summary>
        /// 返回一个访问左右两侧子节点的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<BinaryTreeNode<T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion

    }
}
