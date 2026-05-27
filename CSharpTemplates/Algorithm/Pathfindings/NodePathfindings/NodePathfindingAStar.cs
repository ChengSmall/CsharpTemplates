using System;
using System.Collections;
using System.Collections.Generic;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Pathfindings.NotePathfindings
{

    /// <summary>
    /// 节点路径地图 - AStar寻路算法
    /// </summary>
    public sealed class NodePathfindingAStar : NodePathfinding
    {

        #region 结构

#if DEBUG
        /// <summary>
        /// A* 算法中的节点封装，包含 f 值用于排序
        /// </summary>
#endif
        private readonly struct AStarNode
        {
            public AStarNode(INodePath node, long f)
            {
                this.node = node; this.f = f;
            }
            public readonly long f;
            public readonly INodePath node;

            public override string ToString()
            {
                return (node?.ToString()) + " - " + f.ToString();
            }
        }

#if DEBUG
        /// <summary>
        /// A* 节点比较器，按 f 值升序排列，f 相同按节点ID排序
        /// </summary>
#endif
        private sealed class AStarNodeComparer : IComparer<AStarNode>, IEqualityComparer<AStarNode>
        {

            public int Compare(AStarNode x, AStarNode y)
            {
                //if (x == y) return 0;
                //if (x is null)
                //{
                //    return -1;
                //}
                //if (y is null)
                //{
                //    return 1;
                //}

                int cmp = x.f.CompareTo(y.f);
                if (cmp != 0) return cmp;

                // f 相同按节点 ID 排序，保证确定性
                return string.CompareOrdinal(x.node?.ID, y.node?.ID);
            }

            public bool Equals(AStarNode x, AStarNode y)
            {
                //if (x == y) return true;
                //if (x is null || y is null)
                //{
                //    return false;
                //}

                return x.f == y.f && (x.node?.ID) == (y.node?.ID);
            }

            public int GetHashCode(AStarNode obj)
            {
                if (obj.node is null) return obj.f.GetHashCode();
                return obj.f.GetHashCode() ^ obj.node.GetHashCode();
            }
        }

#if DEBUG
        /// <summary>
        /// 父节点信息，用于路径回溯
        /// </summary>
#endif
        private readonly struct ParentInfo
        {
            public ParentInfo(INodePath parent, int index)
            {
                this.parent = parent; this.index = index;
            }
            public readonly INodePath parent;
            public readonly int index;
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化A*算法
        /// </summary>
        public NodePathfindingAStar()
        {
            p_comp = new NodePathEqualComparer();
            p_asComparer = new AStarNodeComparer();

            // 开放列表（优先队列），按 f = g + h 排序，f 小的优先
            p_openSet = new SortedSet<AStarNode>(p_asComparer);

            // 关闭集合，记录已访问节点
            p_closedSet = new HashSet<INodePath>(p_comp);

            // 父节点记录，用于回溯路径
            p_cameFrom = new Dictionary<INodePath, ParentInfo>(p_comp);

            // g值：从起点到当前节点的实际代价
            p_gScore = new Dictionary<INodePath, long>(p_comp);

            p_pathStack = new Stack<int>();
        }

        #endregion

        #region 参数

        private readonly AStarNodeComparer p_asComparer;

        private readonly NodePathEqualComparer p_comp;

        private Stack<int> p_pathStack;

        private SortedSet<AStarNode> p_openSet;

        private HashSet<INodePath> p_closedSet;

        private Dictionary<INodePath, ParentInfo> p_cameFrom;

        private Dictionary<INodePath, long> p_gScore;

        #endregion

        #region 实现

        public override bool Calculation(INodeNetworkMap map, INodePath start, INodePath end, IList<int> append)
        {
            if (map is null || start is null || end is null || append is null)
            {
                throw new ArgumentNullException();
            }

            // 起点终点相同，直接返回空路径
            if (start.ID == end.ID) return true;

            p_openSet.Clear();
            p_closedSet.Clear();
            p_cameFrom.Clear();
            p_gScore.Clear();

            // 开放列表（优先队列），按 f = g + h 排序，f 小的优先
            var openSet = p_openSet;

            // 关闭集合，记录已访问节点
            var closedSet = p_closedSet;

            // 父节点记录，用于回溯路径
            var cameFrom = p_cameFrom;

            // g值：从起点到当前节点的实际代价
            var gScore = p_gScore;

            // 起点初始化
            gScore[start] = 0;

            openSet.Add(new AStarNode(start, map.CanHeuristic ? map.Heuristic(start, end) : 0));

            while (openSet.Count > 0)
            {
                // 取出 f 值最小的节点
                var current = openSet.Min;
                openSet.Remove(current);

                // 到达目标，回溯路径
                if (current.node.ID == end.ID)
                {
                    ReturnPath(cameFrom, end, append);
                    p_openSet.Clear();
                    p_closedSet.Clear();
                    p_cameFrom.Clear();
                    p_gScore.Clear();
                    return true;
                }

                // 加入关闭集合
                closedSet.Add(current.node);

                // 遍历当前节点的所有邻居
                for (int i = 0; i < current.node.NodeCount; i++)
                {
                    var link = current.node.GetNode(i);

                    // 跳过已关闭的节点
                    if (closedSet.Contains(link.node)) continue;

                    // cost 小于 0 表示无法通过
                    if (link.cost < 0) continue;

                    // 计算从起点经过当前节点到邻居的代价
                    long tentativeG = gScore[current.node] + link.cost;

                    // 判断新路径更优，或邻居不在开放列表
                    if ((!gScore.ContainsKey(link.node)) || tentativeG < gScore[link.node])
                    {
                        cameFrom[link.node] = new ParentInfo(current.node, i);
                        gScore[link.node] = tentativeG;

                        long h = 0;
                        if (map.CanHeuristic)
                        {
                            h = map.Heuristic(link.node, end);
                            if (h < 0) continue; // 启发式返回负数表示不可达
                        }

                        long f = tentativeG + h;

                        // 移除旧节点（如果存在），加入新节点
                        var oldNode = new AStarNode(link.node, f);
                        if (openSet.Contains(oldNode))
                        {
                            openSet.Remove(oldNode);
                        }
                        openSet.Add(oldNode);
                    }
                }
            }

            p_openSet.Clear();
            p_closedSet.Clear();
            p_cameFrom.Clear();
            p_gScore.Clear();

            // 开放列表为空，未找到路径
            return false;
        }

#if DEBUG
        /// <summary>
        /// 回溯并填充路径索引
        /// </summary>
#endif
        private void ReturnPath(Dictionary<INodePath, ParentInfo> cameFrom, INodePath end, IList<int> append)
        {
            var path = p_pathStack;
            path.Clear();
            var current = end;

            while (cameFrom.ContainsKey(current))
            {
                var info = cameFrom[current];
                path.Push(info.index);
                current = info.parent;
            }

            while (path.Count > 0)
            {
                append.Add(path.Pop());
            }
        }

        /// <summary>
        /// 清理缓冲区参数
        /// </summary>
        public void ClearBuffer()
        {
            p_pathStack = new Stack<int>();
            p_openSet = new SortedSet<AStarNode>(p_asComparer);
            p_closedSet = new HashSet<INodePath>(p_comp);
            p_cameFrom = new Dictionary<INodePath, ParentInfo>(p_comp);
            p_gScore = new Dictionary<INodePath, long>(p_comp);
        }

        #endregion

    }

}
