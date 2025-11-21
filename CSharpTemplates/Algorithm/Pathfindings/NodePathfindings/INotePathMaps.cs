using System;
using System.Collections;
using System.Collections.Generic;
using Cheng.Algorithm.HashCodes;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Pathfindings.NotePathfindings
{

    /// <summary>
    /// 所连接的节点
    /// </summary>
    public readonly struct LinkNode : IEquatable<LinkNode>, IHashCode64, IComparable<LinkNode>
    {

        #region 参数

        /// <summary>
        /// 初始化连接的节点
        /// </summary>
        /// <param name="cost">到达此处的成本</param>
        /// <param name="node">连接的节点</param>
        public LinkNode(double cost, INotePath node)
        {
            this.cost = cost;
            this.node = node;
        }

        /// <summary>
        /// 到达此节点的成本
        /// </summary>
        /// <remarks>
        /// <para>表示到达此节点的成本或者路程，值越大成本越高，越小成本越低，最小值为0</para>
        /// </remarks>
        public readonly double cost;

        /// <summary>
        /// 连接的节点对象
        /// </summary>
        public readonly INotePath node;

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
            return node.ID == other.node.ID;
        }

        public override bool Equals(object obj)
        {
            if (obj is LinkNode other) return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            if (node is null) return cost.GetHashCode();
            return node.ID.GetHashCode() ^ cost.GetHashCode();
        }

        public long GetHashCode64()
        {
            if (node is null) return 0;
            return node.ID ^ cost.GetHashCode64();
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
            else if(other.node is null)
            {
                return node is null ? 0 : 1;
            }

            return node.ID.CompareTo(other.node.ID);
        }

        #endregion

    }

    /// <summary>
    /// 一个路径节点
    /// </summary>
    public interface INotePath
    {

        /// <summary>
        /// 表示当前路径节点的唯一id
        /// </summary>
        long ID { get; }

        /// <summary>
        /// 当前节点所连接的其他节点数量
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        /// 使用索引查找其他节点信息
        /// </summary>
        /// <param name="index">连接到其他节点的索引，范围在[0,<see cref="NodeCount"/>)</param>
        /// <returns>表示从此节点连接到的其他节点信息</returns>
        LinkNode GetNode(int index);

    }

    /// <summary>
    /// 使用路径节点唯一ID进行相等比较的比较器
    /// </summary>
    public sealed class NodePathEqualComparer : EqualityComparerHash64<INotePath>
    {

        /// <summary>
        /// 实例化比较器
        /// </summary>
        public NodePathEqualComparer()
        {
        }

        public override bool Equals(INotePath x, INotePath y)
        {
            if (x == y) return true;
            if (x is null || y is null) return false;
            return x.ID == y.ID;
        }

        public override int GetHashCode(INotePath obj)
        {
            if (obj is null) throw new ArgumentNullException();
            return obj.ID.GetHashCode();
        }

        public override long GetHashCode64(INotePath value)
        {
            if (value is null) throw new ArgumentNullException();
            return value.ID;
        }

    }

}