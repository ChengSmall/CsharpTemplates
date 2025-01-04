using System;
using System.Collections.Generic;
using System.Text;

using Cheng.DataStructure.Collections;

namespace Cheng.Algorithm.Trees
{

    /// <summary>
    /// 一个表示树状图的节点的公共接口
    /// </summary>
    public interface IDataEntry
    {

        /// <summary>
        /// 数据在包内的相对路径
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 此数据的文件或文件夹名称
        /// </summary>
        string Name { get; }

    }

    /// <summary>
    /// 表示节点集合的公共接口
    /// </summary>
    public interface IDataList
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
    public struct TreeNodeData
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
        /// 该节点数据的完整路径或目录，根节点表示为null
        /// </summary>
        public readonly string dataPath;

        /// <summary>
        /// 该节点文件名或文件夹名，根节点表示为null
        /// </summary>
        public readonly string name;

        /// <summary>
        /// 该节点是否为文件，文件表示true，文件夹表示false
        /// </summary>
        public readonly bool isFile;

    }

    /// <summary>
    /// 树状图转换器
    /// </summary>
    /// <remarks>
    /// 用于将文件数据列表转换为
    /// </remarks>
    public class TreeAction
    {

        /// <summary>
        /// 创建树状图
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="nodeLookupBuffer">字典缓冲区</param>
        /// <param name="splitFullNameChars"><分割路径用的可用路径分隔符/param>
        /// <param name="pathSeparator">组装路径时用的路径分隔符</param>
        /// <returns>根节点</returns>
        private static TreeNode<TreeNodeData> f_sDataListToTreeNode(IDataList list, Dictionary<string, TreeNode<TreeNodeData>> nodeLookupBuffer, char[] splitFullNameChars, string pathSeparator)
        {

            //字典来存储所有节点，以便按其完整路径快速查找
            Dictionary<string, TreeNode<TreeNodeData>> nodeLookup = nodeLookupBuffer;
            //nodeLookup.Clear();

            //创建根节点
            TreeNode<TreeNodeData> root = new TreeNode<TreeNodeData>(default);

            TreeNode<TreeNodeData> currentNode = root;

            //使用空字符串作为根节点的键
            nodeLookup[""] = root;

            //填充字典并构建树结构
            for (int t_entryIndex = 0; t_entryIndex < list.Count; t_entryIndex++)
            {
                var entry = list[t_entryIndex];

                if (entry.FullName is null) throw new ArgumentNullException();

                var parts = entry.FullName.Split(splitFullNameChars, StringSplitOptions.RemoveEmptyEntries);
                TreeNode<TreeNodeData> parentNode = currentNode;

                //遍历路径部分以查找或创建正确的父节点
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    //string key = string.Join("/", parts, 0, i + 1);
                    string key = string.Join(pathSeparator, parts, 0, i + 1);
                    if (!nodeLookup.ContainsKey(key))
                    {
                        //为路径的这一部分创建一个新节点
                        //var newData = new TreeNodeData(string.Join("/", parts, 0, i + 1), parts[i], false);
                        var newData = new TreeNodeData(string.Join(pathSeparator, parts, 0, i + 1), parts[i], false);
                        TreeNode<TreeNodeData> newNode = new TreeNode<TreeNodeData> (newData);
                        parentNode.Add(newNode);
                        nodeLookup[key] = newNode;
                    }

                    //移动到路径下一部分的父节点
                    parentNode = nodeLookup[key];
                }

                //创建叶子节点并将其添加到其父节点
                //假设它是一个文件，因为它在列表中
                var leafData = new TreeNodeData(entry.FullName, entry.Name, true); 
                
                //TreeNode<TreeNodeData> leafNode = new TreeNode<TreeNodeData> { Value = leafData };
                TreeNode<TreeNodeData> leafNode = new TreeNode<TreeNodeData>(leafData);
                parentNode.Add(leafNode);
                //将叶子节点存储在查找中以确保完整性，尽管这不是严格必要的
                nodeLookup[entry.FullName] = leafNode; 

            }

            return root;
        }

        #region

        public TreeAction()
        {
            p_nodeLookup = new Dictionary<string, TreeNode<TreeNodeData>>();
            p_sep = new char[] { '/', '\\' };
        }

        #endregion

        #region

        private Dictionary<string, TreeNode<TreeNodeData>> p_nodeLookup;

        private char[] p_sep;

        #endregion

        #region 功能

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">文件数据列表</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list)
        {
            if (list is null) throw new ArgumentNullException();
            p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, p_nodeLookup, p_sep, "\\");
            p_nodeLookup.Clear();

            return re;
        }

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">文件数据列表</param>
        /// <param name="pathSeparator">用于拼接路径的路径分隔符</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list, string pathSeparator)
        {
            if (list is null || pathSeparator is null) throw new ArgumentNullException();
            
            p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, p_nodeLookup, p_sep, pathSeparator);
            p_nodeLookup.Clear();

            return re;
        }

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">文件数据列表</param>
        /// <param name="pathSeparator">用于拼接路径的路径分隔符</param>
        /// <param name="splitFullNameChars">用于分割路径时的可用路径分隔符</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list, string pathSeparator, char[] splitFullNameChars)
        {
            if (list is null || pathSeparator is null || splitFullNameChars is null) throw new ArgumentNullException();
            if (splitFullNameChars.Length == 0) throw new ArgumentException();

            p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, p_nodeLookup, splitFullNameChars, pathSeparator);
            p_nodeLookup.Clear();

            return re;
        }

        #endregion

}

}
