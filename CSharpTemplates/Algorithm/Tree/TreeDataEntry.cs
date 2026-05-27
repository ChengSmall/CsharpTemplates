using System;
using System.Collections.Generic;
using System.Text;

using Cheng.DataStructure.Collections;

namespace Cheng.Algorithm.Trees
{

    /// <summary>
    /// 表示树状图节点数据所在位置的公共接口
    /// </summary>
    public interface IDataEntry
    {

        /// <summary>
        /// 数据在包内的相对路径
        /// </summary>
        /// <remarks>
        /// <para>相对路径的路径节点采用符号'\'分割</para>
        /// </remarks>
        string FullName { get; }

        /// <summary>
        /// 此数据的文件名称
        /// </summary>
        string Name { get; }

    }

    /// <summary>
    /// 表示节点集合的公共接口
    /// </summary>
    public interface IDataList : IEnumerable<IDataEntry>
    {

        /// <summary>
        /// 当前节点集合内的节点数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 按索引访问当前节点集合的节点
        /// </summary>
        /// <param name="index">节点索引，范围在[0,<see cref="Count"/>)</param>
        /// <returns>指定索引下的节点</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        IDataEntry this[int index] { get; }

    }

    /// <summary>
    /// 节点数据
    /// </summary>
    public readonly struct TreeNodeData : IEquatable<TreeNodeData>
    {

        /// <summary>
        /// 初始化节点数据
        /// </summary>
        /// <param name="dataPath">该节点数据的完整路径或目录</param>
        /// <param name="name">该节点文件名或文件夹名</param>
        /// <param name="isFile">该节点是否为文件</param>
        public TreeNodeData(string dataPath, string name, bool isFile)
        {
            this.dataPath = dataPath;
            this.name = name;
            this.isFile = isFile;
        }

        /// <summary>
        /// 该节点数据的完整路径或目录，如果是根节点则为null
        /// </summary>
        public readonly string dataPath;

        /// <summary>
        /// 该节点文件名或文件夹名，如果是根节点则为null
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 该节点是否为文件；true表示文件，false表示文件夹
        /// </summary>
        public readonly bool isFile;

        #region 派生

        /// <summary>
        /// 返回当前节点的路径字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (dataPath is null) return "<root>";
            return dataPath;
        }

        public static bool operator ==(in TreeNodeData x, in TreeNodeData y)
        {
            return EqualityStrNotPathSeparator.EqualPath(x.dataPath, y.dataPath, true, false);
        }

        public static bool operator !=(in TreeNodeData x, in TreeNodeData y)
        {
            return !EqualityStrNotPathSeparator.EqualPath(x.dataPath, y.dataPath, true, false);
        }

        public bool Equals(TreeNodeData other)
        {
            return EqualityStrNotPathSeparator.EqualPath(this.dataPath, other.dataPath, true, false);
        }

        public override int GetHashCode()
        {
            return (dataPath is null) ? 0 : dataPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TreeNodeData n) return Equals(n);
            return false;
        }

        #endregion

    }


}
