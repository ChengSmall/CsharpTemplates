using Cheng.DataStructure.Collections;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.DataStructure.Cherrsdinates;
using Cheng.Algorithm.Collections;

namespace Cheng.DataStructure
{

    /// <summary>
    /// 树状图节点参数
    /// </summary>
    public struct MindMap
    {
        /// <summary>
        /// 树状图节点位置x坐标
        /// </summary>
        public double x;
        /// <summary>
        /// 树状图节点位置y坐标
        /// </summary>
        public double y;
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name;
        /// <summary>
        /// 节点附加文本
        /// </summary>
        public string text;
    }

    /// <summary>
    /// 树状图节点
    /// </summary>
    public class MindMapNode : TreeNode<MindMap>
    {
        #region 结构
        private struct DBSbyName
        {

            public MindMapNode node;
            public string name;

            public bool f_findNameDBS(TreeNode<MindMap> tree)
            {
                var node = tree as MindMapNode;

                if (node is null) return true;

                if(node.p_value.name == name)
                {
                    this.node = node;
                    return false;
                }
                
                return true;
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 实例化一个树状图
        /// </summary>
        public MindMapNode() : base()
        {
        }
        /// <summary>
        /// 实例化一个树状图
        /// </summary>
        /// <param name="capacity">节点初始容量</param>
        public MindMapNode(int capacity) : base(new MindMap() , capacity)
        {
        }

        #endregion

        #region 功能

        /// <summary>
        /// 访问指定名称的子节点
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>指定名称的子节点，若无法找到返回null</returns>
        public MindMapNode this[string name]
        {
            get
            {
                var list = p_nodes;
                MindMapNode node;

                for (int i = 0; i < list.Count; i++)
                {
                    node = list[i] as MindMapNode;

                    if (node != null && node.p_value.name == name) return node;
                }
                return null;
            }
        }

        /// <summary>
        /// 深度遍历所有的子节点，返回指定名称的节点
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns>指定名称的节点，没有找到返回null</returns>
        public MindMapNode DepthByName(string name)
        {
            DBSbyName d = default;
            d.name = name;
            d.node = null;
            this.ForeachDepth(d.f_findNameDBS);
            return d.node;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.name ?? string.Empty;
        }

        #endregion

    }

}
