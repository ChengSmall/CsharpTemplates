using System;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Pathfindings.NotePathfindings;
using System.Text;
using System.Collections.Generic;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 测试用网状地图
    /// </summary>
    public sealed class TestNodeNetworkMap : INodeNetworkMap
    {

        /// <summary>
        /// 一个节点
        /// </summary>
        public sealed class ANode : INodePath, IComparable<ANode>, IEquatable<ANode>
        {

            public ANode(string id)
            {
                links = new List<LinkNode>();
                this.id = id;
            }

            public readonly string id;

            public List<LinkNode> links;

            public string ID => id;

            public int NodeCount => links.Count;

            public LinkNode GetNode(int index)
            {
                return links[index];
            }

            public void AddNode(ANode node, int cost)
            {
                links.Add(new LinkNode(cost, node));
            }

            public void RemoveNode(ANode node)
            {
                var i = links.FindIndex( (obj) => obj.node == node );
                links.RemoveAt(i);
            }

            public override string ToString()
            {
                return id;
            }

            public int CompareTo(ANode other)
            {
                return string.CompareOrdinal(id, other?.id);
            }

            public bool Equals(ANode other)
            {
                return id == (other?.id);
            }
        }

        public TestNodeNetworkMap()
        {
            p_nodeList = new List<ANode>();
        }

        /// <summary>
        /// 实例化地图，指定节点数量
        /// </summary>
        /// <param name="count"></param>
        public TestNodeNetworkMap(int count)
        {
            p_nodeList = new List<ANode>(count);
            for (int i = 0; i < count; i++)
            {
                p_nodeList.Add(new ANode("m:" + i.ToString()));
            }
        }

        public List<ANode> p_nodeList;

        public IReadOnlyList<INodePath> Nodes => p_nodeList;

        public bool CanHeuristic => false;

        /// <summary>
        /// 将已有节点连接到另一个节点
        /// </summary>
        /// <param name="node">操作节点索引</param>
        /// <param name="toLinkIndex">要连接到的节点索引</param>
        /// <param name="cost">路径距离</param>
        public void LinkTo(int node, int toLinkIndex, int cost)
        {
            p_nodeList[node].AddNode(p_nodeList[toLinkIndex], cost);
        }

        /// <summary>
        /// 将两个节点相互连接，并设置节点路径成本
        /// </summary>
        /// <param name="n1">第一个节点索引</param>
        /// <param name="n2">第二个节点索引</param>
        /// <param name="cost">节点路径成本</param>
        public void Interconnected(int n1, int n2, int cost)
        {
            p_nodeList[n1].AddNode(p_nodeList[n2], cost);
            p_nodeList[n2].AddNode(p_nodeList[n1], cost);
        }

        /// <summary>
        /// 用索引访问节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ANode this[int index]
        {
            get => p_nodeList[index];
            set
            {
                p_nodeList[index] = value;
            }
        }

        public long Heuristic(INodePath start, INodePath end)
        {
            return 0;
        }

        /// <summary>
        /// 查找节点索引
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int FindIndex(ANode node)
        {
            return p_nodeList.FindIndex((obj) => obj?.id == node?.id
                );
        }

        /// <summary>
        /// 按id查找节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int FindIndexByID(string id)
        {
            return p_nodeList.FindIndex((obj) => (obj?.id) == id
                );
        }

        public void Sort()
        {
            p_nodeList.Sort();
        }

        /// <summary>
        /// 将两个节点相互连接，并设置节点路径成本
        /// </summary>
        /// <param name="nid1">第一个节点id</param>
        /// <param name="nid2">第二个节点id</param>
        /// <param name="cost">路径长度</param>
        public void Interconnected(string nid1, string nid2, int cost)
        {
            Interconnected(FindIndexByID(nid1), FindIndexByID(nid2), cost);
        }

        /// <summary>
        /// 添加一个新的节点
        /// </summary>
        /// <param name="nodeID">节点名称</param>
        public void Add(string nodeID)
        {
            p_nodeList.Add(new ANode(nodeID));
        }

        /// <summary>
        /// 节点组里第一个节点
        /// </summary>
        public ANode FirstNode
        {
            get => p_nodeList[0];
        }

        /// <summary>
        /// 节点组里最后一个节点
        /// </summary>
        public ANode EndNode
        {
            get => p_nodeList[p_nodeList.Count - 1];
        }

    }

}