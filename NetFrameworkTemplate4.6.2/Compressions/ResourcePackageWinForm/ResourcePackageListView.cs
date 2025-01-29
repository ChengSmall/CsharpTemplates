using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using Cheng.DataStructure.Collections;

using System.IO.Compression;
using System.IO;

using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Trees;

using RePack = Cheng.Algorithm.Compressions.ResourcePackages.ResourcePackageReader;

using BasePack = Cheng.Algorithm.Compressions.BaseCompressionParser;

using RBInf = Cheng.Algorithm.Compressions.ResourcePackages.ResourcePackageReader.BlockInformation;

using BaseInf = Cheng.Algorithm.Compressions.DataInformation;

using ListSubItem = System.Windows.Forms.ListViewItem.ListViewSubItem;

using TNode = Cheng.DataStructure.Collections.TreeNode<Cheng.Algorithm.Trees.TreeNodeData>;

namespace Cheng.Algorithm.Compressions.ResourcePackages.Windows.Forms
{

    /// <summary>
    /// 刷新页面时对添加的每一项的额外操作
    /// </summary>
    /// <param name="node">当前项的节点信息</param>
    /// <param name="item">当前要添加的项</param>
    public delegate void RefreshViewCreateSubItems(TreeNodeData node, ListViewItem item);

    /// <summary>
    /// 封装<see cref="BasePack"/>作为<see cref="ListView"/>的的资源管理器
    /// </summary>
    /// <remarks>
    /// <para>将<see cref="BasePack"/>和 Windows 列表视图<see cref="ListView"/>控件关联以进行查询和读写操作，使用树状结构来访问列表</para>
    /// </remarks>
    public class ResourcePackageListView
    {

        #region ai

        private static TNode f_initTree(ResReaderPack pack)
        {
            TreeAction treeAction = new TreeAction();
            return treeAction.DataListToTreeNode(pack);
        }

        private class ResReaderEntry : IDataEntry
        {
            public ResReaderEntry(BaseInf inf)
            {
                this.inf = inf;
            }

            public readonly BaseInf inf;

            public string FullName
            {
                get => inf.DataPath;
            }

            public string Name
            {
                get => inf.DataName;
            }
            
        }

        private class ResReaderPack : IDataList
        {

            public ResReaderPack(BasePack pack)
            {
                this.pack = pack ?? throw new ArgumentNullException();
                int length = pack.Count;
                p_list = new List<ResReaderEntry>(length);

                for (int i = 0; i < length; i++)
                {
                    p_list.Add(new ResReaderEntry(pack[i]));
                }
            }

            public readonly BasePack pack;

            private List<ResReaderEntry> p_list;

            public IDataEntry this[int index]
            {
                get => p_list[index];
            }

            public int Count
            {
                get => p_list.Count;
            }
        }

        #endregion

        #region 结构

        #endregion

        #region 构造

        /// <summary>
        /// 实例化资源包管理器
        /// </summary>
        /// <param name="pack">数据读取器</param>
        /// <param name="listView">与之相关联的 windows 列表视图控件</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">数据读取器没有基本的查询权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public ResourcePackageListView(BasePack pack, ListView listView)
        {
            if (pack is null || listView is null) throw new ArgumentNullException();
            if (!(pack.CanProbePath && pack.CanIndexOf))
            {
                throw new NotSupportedException();
            }

            p_pack = pack;
            p_listView = listView;
            f_init();
        }

        #region 初始化封装

        private void f_init()
        {
            //初始化树状数据结构

            ResReaderPack resReader = new ResReaderPack(p_pack);

            //p_tree = f_initTreeNode(resReader);           

            //f_initTreeParent(p_tree);

            p_tree = f_initTree(resReader);
            p_nowPage = p_tree;
        }

        #endregion

        #endregion

        #region 参数

        private BasePack p_pack;

        private ListView p_listView;

        /// <summary>
        /// 结构树
        /// </summary>
        private TNode p_tree;

        /// <summary>
        /// 当前页面显示
        /// </summary>
        private TNode p_nowPage;

        private RefreshViewCreateSubItems p_createSubItems;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取内部封装的资源读写器
        /// </summary>
        public BasePack Package
        {
            get => p_pack;
        }

        /// <summary>
        /// 获取内部封装的资源读写器
        /// </summary>
        /// <typeparam name="TP">指定派生类型</typeparam>
        /// <returns>内部封装的资源读写器，若类型不一致则为null</returns>
        public TP GetPackage<TP>() where TP : BasePack
        {
            return p_pack as TP;
        }

        /// <summary>
        /// 获取内部封装的 windows 列表视图控件
        /// </summary>
        public ListView WindowsListView
        {
            get => p_listView;
        }

        /// <summary>
        /// 获取节点树的根节点
        /// </summary>
        public TNode RootNode
        {
            get => p_nowPage;
        }

        /// <summary>
        /// 当前被切换到的页面，初始值为根节点
        /// </summary>
        public TNode NowNodePage
        {
            get => p_nowPage;
        }

        /// <summary>
        /// 在<see cref="ListView"/>刷新页面时调用的额外方法
        /// </summary>
        public RefreshViewCreateSubItems ViewCreateSubItems
        {
            get => p_createSubItems;
            set
            {
                p_createSubItems = value;
            }
        }

        #endregion

        #region 封装

        #endregion

        #region 页面管理

        /// <summary>
        /// 按照当前的节点刷新页面
        /// </summary>
        /// <remarks>
        /// <para>调用该函数后，会根据<see cref="NowNodePage"/>参数重新刷新页面显示信息</para>
        /// </remarks>
        public void RefreshView()
        {
            var list = p_listView.Items;

            var childs = p_nowPage;

            if (childs is null)
            {
                list.Clear();
                return;
            }

            int treeCount = childs.Count;
           
            if(treeCount == 0)
            {
                list.Clear();
                return;
            }

            p_listView.BeginUpdate();
            list.Clear();

            for (int i = 0; i < treeCount; i++)
            {
                var node = childs[i];

                if (node is null) continue;

                //DataType nt = node.DataNodeType;

                ListViewItem viewItem = new ListViewItem(node.Value.name);
                p_createSubItems?.Invoke(node.Value, viewItem);
                //if(lst != null) viewItem.SubItems.AddRange(lst);

                list.Add(viewItem);
            }

            p_listView.EndUpdate();

        }

        /// <summary>
        /// 切页到指定节点下
        /// </summary>
        /// <remarks>该函数仅更改<see cref="NowNodePage"/>参数，如果调用该函数后需要接着调用<see cref="RefreshView"/>函数刷新页面</remarks>
        /// <param name="rootTree">要切换到的节点</param>
        /// <exception cref="ArgumentException">节点不是文件夹</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void OnlySwitchPage(TreeNode<TreeNodeData> rootTree)
        {
            if (rootTree is null) throw new ArgumentNullException();
            if (rootTree.Value.isFile) throw new ArgumentException();
            p_nowPage = rootTree;
        }

        /// <summary>
        /// 切换到指定页面下并刷新视图
        /// </summary>
        /// <param name="node">要切换到的节点</param>
        /// <exception cref="ArgumentException">节点不是文件夹</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void SwitchPage(TreeNode<TreeNodeData> node)
        {
            OnlySwitchPage(node);
            RefreshView();
        }

        /// <summary>
        /// 切换到指定页面下并刷新视图
        /// </summary>
        /// <param name="folderName">当前页面下的某一个文件夹名称</param>
        /// <returns>如果成功找到当前节点下的文件夹名称返回true，如果没在当前节点找到返回false</returns>
        /// <exception cref="ArgumentException">节点不是文件夹</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool SwitchPage(string folderName)
        {

            TreeNode<TreeNodeData> chs = (p_nowPage) as TreeNode<TreeNodeData>;

            if (chs is null || folderName is null) throw new ArgumentNullException();

            int ir = chs.FindIndex((Predicate<TreeNode<TreeNodeData>>)FindFunc);

            if (ir < 0) return false;

            SwitchPage(chs[ir]);
            return true;

            bool FindFunc(TreeNode<TreeNodeData> node)
            {
                return node?.Value.name == folderName;
            }
        }

        /// <summary>
        /// 打开当前页面下的文件信息
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>
        /// 当前页面下的文件信息，若文件名不存在则返回null
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public RBInf OpenFile(string fileName)
        {

            TreeNode<TreeNodeData> chs = (p_nowPage);

            if (chs is null) throw new ArgumentNullException();

            var ir = chs.FindIndex<TreeNode<TreeNodeData>>(FindFunc);

            if (ir < 0)
            {
                return null;
            }

            var inf = chs[ir];

            if (inf.Value.dataPath is null) return null;

            bool b = p_pack.TryGetInformation<RBInf>(inf.Value.dataPath, out RBInf binf);

            if (b) return binf;
            return null;

            bool FindFunc(TreeNode<TreeNodeData> node)
            {
                if(node.Value.isFile) return node.Value.name == fileName;
                return false;
            }
        }

        /// <summary>
        /// 切换到当前文件夹页面的上一层页面
        /// </summary>
        /// <returns>切换完毕返回true，无法切换返回false</returns>
        public bool OnlySwitchParent()
        {            
            if(p_nowPage == p_tree)
            {
                return false;
            }

            var parent = p_nowPage.Parent;
            if (parent is null) return false;
            OnlySwitchPage(parent);
            return true;
        }

        /// <summary>
        /// 切换到当前文件夹页面的上一层页面并刷新视图列表
        /// </summary>
        /// <returns>切换完毕返回true，无法切换返回false</returns>
        public bool SwitchParent()
        {
            var b = OnlySwitchParent();

            if (b)
            {
                RefreshView();
            }

            return b;
        }

        /// <summary>
        /// 重新将页面切换到根目录并刷新视图
        /// </summary>
        public void BackRootNode()
        {
            OnlySwitchPage(p_tree);
            RefreshView();
        }

        #endregion

        #endregion

    }

}
