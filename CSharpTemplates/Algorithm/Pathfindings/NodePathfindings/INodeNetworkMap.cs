using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Cheng.Algorithm.HashCodes;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Pathfindings.NotePathfindings
{

    /// <summary>
    /// 所连接到的其它节点
    /// </summary>
    public readonly struct LinkNode : IEquatable<LinkNode>, IComparable<LinkNode>, IHashCode64
    {

        #region 参数

        /// <summary>
        /// 初始化连接的节点
        /// </summary>
        /// <param name="cost">到达此处的成本</param>
        /// <param name="node">连接的节点</param>
        public LinkNode(int cost, INodePath node)
        {
            this.cost = cost;
            this.node = node;
        }

        /// <summary>
        /// 连接到的节点对象
        /// </summary>
        public readonly INodePath node;

        /// <summary>
        /// 到达此节点的成本
        /// </summary>
        /// <remarks>
        /// <para>表示到达此节点的成本或距离的值，值越大成本越高，越小成本越低，0表示无成本；当值小于0时表示无法通过</para>
        /// </remarks>
        public readonly int cost;

        #endregion

        #region 派生

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(LinkNode a, LinkNode b)
        {
           return a.Equals(b);
        }

        /// <summary>
        /// 判断不相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(LinkNode a, LinkNode b)
        {
            return !a.Equals(b);
        }

        public bool Equals(LinkNode other)
        {
            if(node == other.node)
            {
                return true;
            }
            if(node is null || other.node is null)
            {
                return false;
            }
            return node.ID == other.node.ID && cost == other.cost;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinkNode other) return Equals(other);
            return false;
        }

        /// <summary>
        /// 优先使用成本值<see cref="LinkNode.cost"/>进行比较的比较器
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(LinkNode other)
        {
            var re = cost.CompareTo(other.cost);
            if (re != 0) return re;
            if (node == other.node)
            {
                return 0;
            }
            if (node is null)
            {
                return other.node is null ? 0 : -1;
            }
            else if (other.node is null)
            {
                return node is null ? 0 : 1;
            }

            return string.CompareOrdinal(node.ID, other.node.ID);
        }

        public override int GetHashCode()
        {
            if (node is null) return cost.GetHashCode();
            return node.ID.GetHashCode() ^ cost.GetHashCode();
        }

        public long GetHashCode64()
        {
            if (node is null) return 0;
            return node.ID.GetHashCode64() ^ cost.GetHashCode64();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(12);
            if(node is null)
            {
                sb.Append("[null]");
            }
            else
            {
                sb.Append(node.ToString());
            }
            sb.Append(" - ");
            sb.Append(cost.ToString());
            return sb.ToString();
        }

        #endregion

    }

    /// <summary>
    /// 一个路径节点
    /// </summary>
    public interface INodePath
    {

        /// <summary>
        /// 表示当前路径节点的唯一id字符串，按每个字符值确认唯一性
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 当前节点所连接的其他节点数量
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        /// 使用索引查找其他节点信息
        /// </summary>
        /// <param name="index">连接到其他节点的索引，最小是0，最大不超过<see cref="NodeCount"/></param>
        /// <returns>表示从此节点连接到的其他节点信息</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        LinkNode GetNode(int index);

    }

    /// <summary>
    /// 访问一个网状节点地图的公开接口
    /// </summary>
    public interface INodeNetworkMap
    {

        /// <summary>
        /// 包含地图中所有节点的集合
        /// </summary>
        IReadOnlyList<INodePath> Nodes { get; }

        /// <summary>
        /// 是否支持用<see cref="Heuristic"/>快速估算节点间的最短距离
        /// </summary>
        bool CanHeuristic { get; }

        /// <summary>
        /// 快速估算从<paramref name="start"/>到<paramref name="end"/>的距离，估算距离不超过实际最短距离
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">目标节点</param>
        /// <returns>
        /// <para>要估算的从<paramref name="start"/>到<paramref name="end"/>的距离值，距离值与<see cref="LinkNode.cost"/>是相同单位，值不超过最短的实际距离，估算的最小距离值是0</para>
        /// <para>估算的距离值不会超过实际最短距离；假设从<paramref name="start"/>到<paramref name="end"/>必需经过三条路径，路径成本分别是 3 + 1 + 2 == 6，那么返回值一定不会大于6</para>
        /// <para>当返回值小于0时，表示两节点之间一定没有连通路径</para>
        /// </returns>
        /// <exception cref="NotSupportedException"><see cref="CanHeuristic"/>是false</exception>
        long Heuristic(INodePath start, INodePath end);

    }

}