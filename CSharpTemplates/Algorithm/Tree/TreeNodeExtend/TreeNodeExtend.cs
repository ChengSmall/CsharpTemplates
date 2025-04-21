using Cheng.DataStructure.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Trees
{

    public static class TreeNodeExtend
    {

        #region 查询

        /// <summary>
        /// 查找指定节点是否处于当前节点的子节点内
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="treeNode">当前节点</param>
        /// <param name="node">要查找的节点</param>
        /// <returns>如果节点<paramref name="node"/>确实处于节点<paramref name="treeNode"/>的内部子节点中，返回true；否则返回false</returns>
        /// <exception cref="ArgumentNullException">节点是null</exception>
        public static bool IsChildNode<T>(this TreeNode<T> treeNode, TreeNode<T> node)
        {
            if (treeNode is null || node is null) throw new ArgumentNullException();

            TreeNode<T> tn = node;

            while (tn != null)
            {
                tn = tn.Parent;
                if (tn == treeNode) return true;
            }

            return false;
        }


        /// <summary>
        /// 深度优先遍历多叉树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree">要遍历的多叉树</param>
        /// <param name="action">对每一个元素执行动作，返回值为true则继续遍历，false则终止遍历</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void ForeachDepth<T>(this TreeNode<T> tree, Func<TreeNode<T>, bool> action)
        {
            if (tree is null || action is null) throw new ArgumentNullException();

            f_foeachDepth(tree, action);
            //int length;
            //length = tree.Count;
            //TreeNode<T> node;
            //bool flag;

            //for (int i = 0; i < length; i++)
            //{
            //    node = tree[i];
            //    if(node.Count != 0)
            //    {
            //        ForeachDepth(node, action);
            //    }

            //    flag = action.Invoke(node);
            //    if (!flag) break;
            //}
        }

        static bool f_foeachDepth<T>(TreeNode<T> tree, Func<TreeNode<T>, bool> action)
        {
            int length;
            length = tree.Count;
            TreeNode<T> node;
            bool flag;

            for (int i = 0; i < length; i++)
            {
                node = tree[i];

                flag = action.Invoke(node);
                if (!flag) break;

                if (node.Count != 0)
                {
                    if (!f_foeachDepth(node, action))
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        /// <summary>
        /// 广度优先遍历多叉树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree">要遍历的多叉树</param>
        /// <param name="action">对每一个元素执行动作，返回值为true则继续遍历，false则终止遍历</param>
        /// <param name="buffer">进行广度遍历时需要的队列缓冲区，该参数会在内部修改</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void ForeachBreadth<T>(this TreeNode<T> tree, Func<TreeNode<T>, bool> action, Queue<TreeNode<T>> buffer)
        {
            if (tree is null || action is null) throw new ArgumentNullException();
            if (buffer is null) buffer = new Queue<TreeNode<T>>();

            buffer.Clear();

            buffer.Enqueue(tree);
            TreeNode<T> current;
            int i;
            int count;
            bool flag;
            while (buffer.Count > 0)
            {
                current = buffer.Dequeue();

                flag = action.Invoke(current);
                if (!flag) return;

                count = current.Count;

                for (i = 0; i < count; i++)
                {
                    buffer.Enqueue(current[i]);
                }

            }

        }

        /// <summary>
        /// 深度遍历并统计节点的所有子节点数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="treeNode">节点</param>
        /// <returns><paramref name="treeNode"/>的子节点总数量</returns>
        public static int GetAllCount<T>(this TreeNode<T> treeNode)
        {
            if (treeNode is null) throw new ArgumentNullException();

            return f_getAllCount(treeNode);
        }

        /// <summary>
        /// 深度遍历并统计节点的所有子节点数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="treeNode">节点</param>
        /// <returns><paramref name="treeNode"/>的子节点总数量</returns>
        public static long GetAllCountLong<T>(this TreeNode<T> treeNode)
        {
            if (treeNode is null) throw new ArgumentNullException();

            return f_getAllCountL(treeNode);
        }

        static int f_getAllCount<T>(TreeNode<T> treeNode)
        {
            var count = treeNode.Count;
            for (int i = 0; i < count; i++)
            {
                count += f_getAllCount(treeNode[i]);
            }
            return count;
        }

        static long f_getAllCountL<T>(TreeNode<T> treeNode)
        {
            long count = treeNode.Count;
            for (int i = 0; i < count; i++)
            {
                count += f_getAllCountL(treeNode[i]);
            }
            return count;
        }

        #endregion

    }
}
