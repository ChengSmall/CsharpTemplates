using System;
using System.Collections.Generic;
using Cheng.DataStructure.Collections;

namespace Cheng.Algorithm.Trees
{

    #region

    /// <summary>
    /// 树状图转换器
    /// </summary>
    /// <remarks>
    /// 用于将文件数据列表转换为
    /// </remarks>
    public class TreeAction
    {

        #region

        #region

        ///// <summary>
        ///// 创建树状图
        ///// </summary>
        ///// <param name="list">集合</param>
        ///// <param name="nodeLookupBuffer">字典缓冲区</param>
        ///// <param name="splitFullNameChars"><分割路径用的可用路径分隔符/param>
        ///// <param name="pathSeparator">组装路径时用的路径分隔符</param>
        ///// <returns>根节点</returns>
        //private static TreeNode<TreeNodeData> f_sDataListToTreeNodes(IDataList list, Dictionary<string, TreeNode<TreeNodeData>> nodeLookupBuffer, char[] splitFullNameChars, string pathSeparator)
        //{

        //    //字典来存储所有节点，以便按其完整路径快速查找
        //    Dictionary<string, TreeNode<TreeNodeData>> nodeLookup = nodeLookupBuffer;
        //    //nodeLookup.Clear();

        //    //创建根节点
        //    TreeNode<TreeNodeData> root = new TreeNode<TreeNodeData>(default);

        //    TreeNode<TreeNodeData> currentNode = root;

        //    //使用空字符串作为根节点的键
        //    nodeLookup[""] = root;

        //    //填充字典并构建树结构
        //    for (int t_entryIndex = 0; t_entryIndex < list.Count; t_entryIndex++)
        //    {
        //        var entry = list[t_entryIndex];

        //        if (entry.FullName is null) throw new ArgumentNullException();

        //        var parts = entry.FullName.Split(splitFullNameChars, StringSplitOptions.RemoveEmptyEntries);
        //        TreeNode<TreeNodeData> parentNode = currentNode;

        //        //遍历路径部分以查找或创建正确的父节点
        //        for (int i = 0; i < parts.Length - 1; i++)
        //        {
        //            //string key = string.Join("/", parts, 0, i + 1);
        //            string key = string.Join(pathSeparator, parts, 0, i + 1);
        //            if (!nodeLookup.ContainsKey(key))
        //            {
        //                //为路径的这一部分创建一个新节点
        //                //var newData = new TreeNodeData(string.Join("/", parts, 0, i + 1), parts[i], false);
        //                var newData = new TreeNodeData(string.Join(pathSeparator, parts, 0, i + 1), parts[i], false);
        //                TreeNode<TreeNodeData> newNode = new TreeNode<TreeNodeData>(newData);
        //                parentNode.Add(newNode);
        //                nodeLookup[key] = newNode;
        //            }

        //            //移动到路径下一部分的父节点
        //            parentNode = nodeLookup[key];
        //        }

        //        //创建叶子节点并将其添加到其父节点
        //        //假设它是一个文件，因为它在列表中
        //        var leafData = new TreeNodeData(entry.FullName, entry.Name, true);

        //        //TreeNode<TreeNodeData> leafNode = new TreeNode<TreeNodeData> { Value = leafData };
        //        TreeNode<TreeNodeData> leafNode = new TreeNode<TreeNodeData>(leafData);
        //        parentNode.Add(leafNode);
        //        //将叶子节点存储在查找中以确保完整性，尽管这不是严格必要的
        //        nodeLookup[entry.FullName] = leafNode;

        //    }
        //    return root;
        //}

        #endregion

        /// <summary>
        /// 将一系列节点集合转化为树状结构
        /// </summary>
        /// <param name="list">节点集合</param>
        /// <param name="splitChars">节点间可用的分隔符</param>
        /// <param name="pathSeparator">组装路径的分隔符</param>
        /// <returns>树结构的根节点，包含从<paramref name="list"/>转换的若干子节点和节点数据</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotImplementedException">节点路径的分隔符'\'之间存在空节点名</exception>
        private static TreeNode<TreeNodeData> f_sDataListToTreeNode(IDataList list, char[] splitChars, string pathSeparator)
        {
            if (list is null) throw new ArgumentNullException();

            // 初始化根节点
            TreeNode<TreeNodeData> root = new TreeNode<TreeNodeData>(new TreeNodeData(null, null, false));

            int listCount = list.Count;
            for (int list_i = 0; list_i < listCount; list_i++)
            {
                IDataEntry entry = list[list_i];

                string fullName = entry.FullName;
                string[] parts = fullName.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    continue; // 忽略空路径（例如FullName为空）

                // 检查路径是否合法
                foreach (string part in parts)
                {
                    //节点路径的分隔符'\\'之间存在空节点名
                    if (string.IsNullOrEmpty(part))
                        throw new NotImplementedException();
                }

                TreeNode<TreeNodeData> currentParent = root;

                // 遍历除最后一个部分外的所有路径段（构建文件夹）
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string folderName = parts[i];
                    TreeNode<TreeNodeData> existingFolder = FindChildFolder(currentParent, folderName);

                    if (existingFolder == null)
                    {
                        // 构建当前文件夹的完整路径
                        string parentPath = currentParent.Value.dataPath;
                        string currentPath = parentPath == null ? folderName : $"{parentPath}{pathSeparator}{folderName}";
                        TreeNodeData folderData = new TreeNodeData(currentPath, folderName, false);
                        TreeNode<TreeNodeData> newFolder = new TreeNode<TreeNodeData>(folderData);
                        currentParent.Add(newFolder);
                        existingFolder = newFolder;
                    }

                    currentParent = existingFolder;
                }

                // 添加文件节点
                //string fileName = parts[parts.Length - 1];
                TreeNodeData fileData = new TreeNodeData(entry.FullName, entry.Name, true);
                TreeNode<TreeNodeData> fileNode = new TreeNode<TreeNodeData>(fileData);
                currentParent.Add(fileNode);
            }

            //foreach (IDataEntry entry in list)
            //{
            //    string fullName = entry.FullName;
            //    string[] parts = fullName.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

            //    if (parts.Length == 0)
            //        continue; // 忽略空路径（例如FullName为空）

            //    // 检查路径是否合法
            //    foreach (string part in parts)
            //    {
            //        //节点路径的分隔符'\\'之间存在空节点名
            //        if (string.IsNullOrEmpty(part))
            //            throw new NotImplementedException();
            //    }

            //    TreeNode<TreeNodeData> currentParent = root;

            //    // 遍历除最后一个部分外的所有路径段（构建文件夹）
            //    for (int i = 0; i < parts.Length - 1; i++)
            //    {
            //        string folderName = parts[i];
            //        TreeNode<TreeNodeData> existingFolder = FindChildFolder(currentParent, folderName);

            //        if (existingFolder == null)
            //        {
            //            // 构建当前文件夹的完整路径
            //            string parentPath = currentParent.Value.dataPath;
            //            string currentPath = parentPath == null ? folderName : $"{parentPath}{pathSeparator}{folderName}";
            //            TreeNodeData folderData = new TreeNodeData(currentPath, folderName, false);
            //            TreeNode<TreeNodeData> newFolder = new TreeNode<TreeNodeData>(folderData);
            //            currentParent.Add(newFolder);
            //            existingFolder = newFolder;
            //        }

            //        currentParent = existingFolder;
            //    }

            //    // 添加文件节点
            //    //string fileName = parts[parts.Length - 1];
            //    TreeNodeData fileData = new TreeNodeData(entry.FullName, entry.Name, true);
            //    TreeNode<TreeNodeData> fileNode = new TreeNode<TreeNodeData>(fileData);
            //    currentParent.Add(fileNode);
            //}

            return root;

        }

        private static TreeNode<TreeNodeData> FindChildFolder(TreeNode<TreeNodeData> parent, string folderName)
        {
            var ind = parent.FindIndex<TreeNode<TreeNodeData>>(findFunc);
            if (ind < 0)
            {
                return null;
            }
            return parent[ind];

            bool findFunc(TreeNode<TreeNodeData> t_treeNode)
            {
                return ((!t_treeNode.Value.isFile) && t_treeNode.Value.name == folderName);
            }

            //for (int i = 0; i < parent.Count; i++)
            //{
            //    TreeNode<TreeNodeData> child = parent[i];
            //    if (!child.Value.isFile && child.Value.name == folderName)
            //        return child;
            //}
            //return null;
        }

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">数据列表</param>
        /// <param name="splitChars">可用于分割路径的分隔符组</param>
        /// <param name="pathSeparator">在组合路径时使用的路径分隔符</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">分隔符数组为空</exception>
        /// <exception cref="NotImplementedException">路径语法错误</exception>
        public static TreeNode<TreeNodeData> ListToTreeNode(IDataList list, char[] splitChars, string pathSeparator)
        {
            if(splitChars is null || pathSeparator is null)
            {
                throw new ArgumentNullException();
            }
            if (splitChars.Length == 0) throw new ArgumentException();

            return f_sDataListToTreeNode(list, splitChars, pathSeparator);
        }

        #endregion

        #region

        public TreeAction()
        {
            //p_nodeLookup = new Dictionary<string, TreeNode<TreeNodeData>>();
            p_sep = new char[] { '/', '\\' };
        }

        #endregion

        #region

        //private Dictionary<string, TreeNode<TreeNodeData>> p_nodeLookup;

        private char[] p_sep;

        #endregion

        #region 功能

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">文件数据列表</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">路径语法错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list)
        {
            if (list is null) throw new ArgumentNullException();
            //p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, p_sep, "\\");
            //p_nodeLookup.Clear();

            return re;
        }

        /// <summary>
        /// 使用数据列表来创建树状图
        /// </summary>
        /// <param name="list">文件数据列表</param>
        /// <param name="pathSeparator">用于拼接路径的路径分隔符</param>
        /// <returns>一个可按节点访问的树状结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">路径语法错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list, string pathSeparator)
        {
            if (list is null || pathSeparator is null) throw new ArgumentNullException();
            
            //p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, p_sep, pathSeparator);
            //p_nodeLookup.Clear();

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
        /// <exception cref="NotImplementedException">路径语法错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public TreeNode<TreeNodeData> DataListToTreeNode(IDataList list, string pathSeparator, char[] splitFullNameChars)
        {
            if (list is null || pathSeparator is null || splitFullNameChars is null) throw new ArgumentNullException();
            if (splitFullNameChars.Length == 0) throw new ArgumentException();

            //p_nodeLookup.Clear();
            var re = f_sDataListToTreeNode(list, splitFullNameChars, pathSeparator);
            //p_nodeLookup.Clear();

            return re;
        }

        #endregion

}

    #endregion

}
