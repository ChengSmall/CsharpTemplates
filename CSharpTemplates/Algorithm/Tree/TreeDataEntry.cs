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

        public override string ToString()
        {
            if (dataPath is null) return "<root>";
            return dataPath;
        }

    }

    #region

    //public class TreeNode<T>
    //{

    //    public TreeNode(T node)
    //    {
    //        p_list = new List<TreeNode<T>>();
    //        Value = node;
    //    }

    //    public TreeNode()
    //    {
    //        p_list = new List<TreeNode<T>>();
    //    }

    //    /// <summary>
    //    /// 储存子节点
    //    /// </summary>
    //    private List<TreeNode<T>> p_list;

    //    /// <summary>
    //    /// 当前节点的数据
    //    /// </summary>
    //    public T Value { get; set; }

    //    /// <summary>
    //    /// 子节点数量
    //    /// </summary>
    //    public int Count => p_list.Count;

    //    /// <summary>
    //    /// 按索引访问子节点
    //    /// </summary>
    //    /// <param name="index"></param>
    //    /// <returns></returns>
    //    public TreeNode<T> this[int index]
    //    {
    //        get => p_list[index];
    //        set => p_list[index] = value;
    //    }

    //    /// <summary>
    //    /// 添加一个子节点
    //    /// </summary>
    //    /// <param name="node"></param>
    //    public void Add(TreeNode<T> node)
    //    {
    //        p_list.Add(node);
    //    }

    //    /// <summary>
    //    /// 按索引删除一个子节点
    //    /// </summary>
    //    /// <param name="index"></param>
    //    public void RemoveAt(int index)
    //    {
    //        p_list.RemoveAt(index);
    //    }

    //}

    #endregion

    #region test

    //internal static class TreeFunc
    //{

    //    /// <summary>
    //    /// 将一系列节点集合转化为树状结构
    //    /// </summary>
    //    /// <param name="list">节点集合</param>
    //    /// <param name="splitChars">节点间可用的分隔符</param>
    //    /// <returns>树结构的根节点，包含从<paramref name="list"/>转换的若干子节点和节点数据</returns>
    //    /// <exception cref="ArgumentNullException">参数是null</exception>
    //    /// <exception cref="NotImplementedException">节点路径的分隔符'\'之间存在空节点名</exception>
    //    public static TreeNode<TreeNodeData> ListToTree(IDataList list, char[] splitChars)
    //    {
    //        if (list is null) throw new ArgumentNullException();

    //        //待完成

    //        // 初始化根节点
    //        TreeNode<TreeNodeData> root = new TreeNode<TreeNodeData>(new TreeNodeData(null, null, false));

    //        foreach (IDataEntry entry in list)
    //        {
    //            string fullName = entry.FullName;
    //            string[] parts = fullName.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

    //            // 检查路径是否合法
    //            foreach (string part in parts)
    //            {
    //                if (string.IsNullOrEmpty(part))
    //                    throw new NotImplementedException("节点路径的分隔符'\\'之间存在空节点名");
    //            }

    //            if (parts.Length == 0)
    //                continue; // 忽略空路径（例如FullName为空）

    //            TreeNode<TreeNodeData> currentParent = root;

    //            // 遍历除最后一个部分外的所有路径段（构建文件夹）
    //            for (int i = 0; i < parts.Length - 1; i++)
    //            {
    //                string folderName = parts[i];
    //                TreeNode<TreeNodeData> existingFolder = FindChildFolder(currentParent, folderName);

    //                if (existingFolder == null)
    //                {
    //                    // 构建当前文件夹的完整路径
    //                    string parentPath = currentParent.Value.dataPath;
    //                    string currentPath = parentPath == null ? folderName : $"{parentPath}\\{folderName}";
    //                    TreeNodeData folderData = new TreeNodeData(currentPath, folderName, false);
    //                    TreeNode<TreeNodeData> newFolder = new TreeNode<TreeNodeData>(folderData);
    //                    currentParent.Add(newFolder);
    //                    existingFolder = newFolder;
    //                }

    //                currentParent = existingFolder;
    //            }

    //            // 添加文件节点
    //            //string fileName = parts[parts.Length - 1];
    //            TreeNodeData fileData = new TreeNodeData(entry.FullName, entry.Name, true);
    //            TreeNode<TreeNodeData> fileNode = new TreeNode<TreeNodeData>(fileData);
    //            currentParent.Add(fileNode);
    //        }

    //        return root;

    //    }

    //    private static TreeNode<TreeNodeData> FindChildFolder(TreeNode<TreeNodeData> parent, string folderName)
    //    {
    //        var ind = parent.FindIndex<TreeNode<TreeNodeData>>(findFunc);
    //        if(ind < 0)
    //        {
    //            return null;
    //        }
    //        return parent[ind];

    //        bool findFunc(TreeNode<TreeNodeData> t_treeNode)
    //        {
    //            return ((!t_treeNode.Value.isFile) && t_treeNode.Value.name == folderName);
    //        }

    //        //for (int i = 0; i < parent.Count; i++)
    //        //{
    //        //    TreeNode<TreeNodeData> child = parent[i];
    //        //    if (!child.Value.isFile && child.Value.name == folderName)
    //        //        return child;
    //        //}
    //        //return null;
    //    }

    //}

    #endregion

}
