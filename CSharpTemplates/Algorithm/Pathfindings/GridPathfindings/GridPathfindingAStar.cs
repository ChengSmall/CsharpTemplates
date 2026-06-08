using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.Algorithm.Pathfindings.GridPathfindings
{

    /// <summary>
    /// 网格地图寻路算法 - AStar寻路
    /// </summary>
    public sealed class GridPathfindingAStar : BaseGridPathfinding
    {

        #region 结构

#if DEBUG
        /// <summary>
        /// 最小二叉堆实现的优先队列
        /// </summary>
#endif
        private struct HeapNode
        {
            internal int x, y;
            internal long g, f;
        }

        private class MinHeap
        {
            public MinHeap()
            {
                p_list = new List<HeapNode>();
            }

            public List<HeapNode> p_list;

            public bool IsEmpty => p_list.Count == 0;

            public void Enqueue(int x, int y, long g, long f)
            {
                p_list.Add(new HeapNode { x = x, y = y, g = g, f = f });
                int i = p_list.Count - 1;
                while (i > 0)
                {
                    int parent = (i - 1) / 2;
                    if (p_list[i].f < p_list[parent].f ||
                        (p_list[i].f == p_list[parent].f && p_list[i].g < p_list[parent].g))
                    {
                        (p_list[i], p_list[parent]) = (p_list[parent], p_list[i]);
                        i = parent;
                    }
                    else break;
                }
            }

            public void Dequeue(out int x, out int y, out long g, out long f)
            {
                var root = p_list[0];
                x = root.x; y = root.y; g = root.g; f = root.f;

                p_list[0] = p_list[p_list.Count - 1];
                p_list.RemoveAt(p_list.Count - 1);

                int i = 0;
                int count = p_list.Count;
                while (true)
                {
                    int left = 2 * i + 1;
                    int right = 2 * i + 2;
                    int smallest = i;

                    if (left < count &&
                        (p_list[left].f < p_list[smallest].f ||
                        (p_list[left].f == p_list[smallest].f && p_list[left].g < p_list[smallest].g)))
                        smallest = left;
                    if (right < count &&
                        (p_list[right].f < p_list[smallest].f ||
                        (p_list[right].f == p_list[smallest].f && p_list[right].g < p_list[smallest].g)))
                        smallest = right;

                    if (smallest != i)
                    {
                        (p_list[i], p_list[smallest]) = (p_list[smallest], p_list[i]);
                        i = smallest;
                    }
                    else break;
                }
            }

            public void Clear()
            {
                p_list.Clear();
            }

        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化算法对象
        /// </summary>
        public GridPathfindingAStar() : this(0)
        {
        }

        /// <summary>
        /// 创建 A* 寻路实例并指定启发式最小单步代价
        /// </summary>
        /// <param name="minHeuristicCost">
        /// <para>地图可通行格子的最小代价</para>
        /// <para>值越大搜索越集中，但不能大于实际最小代价才会尽可能保证最优解</para>
        /// <para>若不确定最小值，可使用0退化为 Dijkstra 算法；默认是0</para>
        /// </param>
        public GridPathfindingAStar(int minHeuristicCost)
        {
            p_minCost = Math.Max(0, minHeuristicCost);
            p_openList = new MinHeap();
            p_path = new List<Point2I32>();
            p_gScore = null;
            p_parent = null;
            p_dirXCD = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            p_dirYCD = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
            p_dirX = new[] { 0, -1, 1, 0 };
            p_dirY = new[] { -1, 0, 0, 1 };
        }

        #endregion

        #region 参数

        private MinHeap p_openList;

        private List<Point2I32> p_path;

        private long[,] p_gScore;

        private Point2I32[,] p_parent;

        private int[] p_dirXCD;

        private int[] p_dirYCD;

        private int[] p_dirX;

        private int[] p_dirY;

        private int p_minCost;

        #endregion

        #region 实现

        private long f_Heuristic(int x1, int y1, int x2, int y2, bool canDiagonally)
        {
            if (p_minCost == 0) return 0;
            long dx = Math.Abs((long)x1 - x2);
            long dy = Math.Abs((long)y1 - y2);
            if (canDiagonally)
                return (((long)p_minCost)) * Math.Max(dx, dy);
            else
                return (((long)p_minCost)) * (dx + dy);
        }

        private bool f_Calculation(IGridMap map, Point2I32 startPos, Point2I32 endPos, ICollection<Point2I32> append)
        {

            if (map.GetGridPrice(startPos.x, startPos.y) < 0 ||
                map.GetGridPrice(endPos.x, endPos.y) < 0)
                return false;

            // 相同位置
            if (startPos == endPos) return true;

            int width = map.Width;
            int height = map.Height;

            p_openList.Clear();

            if (p_gScore is null)
            {
                p_gScore = new long[width, height];
            }
            else
            {
                if ((p_gScore.GetLength(0) == width) && (p_gScore.GetLength(1) == height))
                {
                    Array.Clear(p_gScore, 0, p_gScore.Length);
                }
                else
                {
                    p_gScore = new long[width, height];
                }
            }

            if (p_parent is null)
            {
                p_parent = new Point2I32[width, height];
            }
            else
            {
                if ((p_parent.GetLength(0) == width) && (p_parent.GetLength(1) == height))
                {
                    Array.Clear(p_parent, 0, p_gScore.Length);
                }
                else
                {
                    p_parent = new Point2I32[width, height];
                }
            }

            long[,] gScore = p_gScore;
            Point2I32[,] parent = p_parent;

            // 记录各格子的最小 g 值和父节点
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gScore[x, y] = long.MaxValue;
                }
            }

            gScore[startPos.x, startPos.y] = 0;
            long hStart = f_Heuristic(startPos.x, startPos.y, endPos.x, endPos.y, map.CanDiagonally);
            MinHeap openList = p_openList;
            openList.Enqueue(startPos.x, startPos.y, 0, hStart);

            int[] dirX, dirY;
            if (map.CanDiagonally)
            {
                dirX = p_dirXCD;
                dirY = p_dirYCD;
            }
            else
            {
                dirX = p_dirX;
                dirY = p_dirY;
            }

            while (!openList.IsEmpty)
            {
                openList.Dequeue(out int cx, out int cy, out long cg, out _);

                if (cg > gScore[cx, cy])
                    continue;

                if (cx == endPos.x && cy == endPos.y)
                {
                    // 重建路径（不含起点和终点）
                    List<Point2I32> path = p_path;
                    path.Clear();
                    int px = cx, py = cy;
                    while (!(px == startPos.x && py == startPos.y))
                    {
                        path.Add(new Point2I32(px, py));
                        Point2I32 p = parent[px, py];
                        px = p.x;
                        py = p.y;
                    }
                    // 反转并移除多余添加
                    path.Reverse();
                    path.RemoveAt(path.Count - 1); // 移除终点
                    foreach (var point in path)
                    {
                        append.Add(point);
                    }
                    p_openList.Clear();
                    path.Clear();
                    return true;
                }

                for (int i = 0; i < dirX.Length; i++)
                {
                    int nx = cx + dirX[i];
                    int ny = cy + dirY[i];

                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        continue;

                    int price = map.GetGridPrice(nx, ny);
                    if (price < 0)
                        continue;

                    long newG = cg + price;
                    if (newG < gScore[nx, ny])
                    {
                        gScore[nx, ny] = newG;
                        parent[nx, ny] = new Point2I32(cx, cy);
                        long h = f_Heuristic(nx, ny, endPos.x, endPos.y, map.CanDiagonally);
                        openList.Enqueue(nx, ny, newG, newG + h);
                    }
                }
            }

            p_openList.Clear();
            return false;
        }

        public override bool Calculation(IGridMap map, Point2I32 startPos, Point2I32 endPos, ICollection<Point2I32> append)
        {
            if (map is null || append is null) throw new ArgumentNullException();
            return f_Calculation(map, startPos, endPos, append);
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            p_openList.Clear();
            p_openList.p_list.Capacity = 0;
            p_path.Clear();
            p_path.Capacity = 0;
            p_gScore = null;
            p_parent = null;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 地图可通行格子的最小代价
        /// </summary>
        /// <value>
        /// <para>地图可通行格子的最小代价</para>
        /// <para>值越大搜索越集中，但不能大于实际最小代价才会尽可能保证最优解</para>
        /// <para>若不确定最小值，可使用0退化为 Dijkstra 算法；值最小是0</para>
        /// </value>
        public int MinHeuristicCost
        {
            get => p_minCost;
            set
            {
                p_minCost = Math.Min(value, 0);
            }
        }

        #endregion

    }

}
