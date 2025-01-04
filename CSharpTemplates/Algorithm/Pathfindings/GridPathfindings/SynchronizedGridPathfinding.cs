using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections.Generic;

namespace Cheng.Algorithm.Pathfindings
{

    /// <summary>
    /// 对网格寻路算法对象进行线程安全封装的类
    /// </summary>
    public sealed class SynchronizedGridPathfinding : GridPathfinding
    {

        /// <summary>
        /// 实例化一个线程安全封装
        /// </summary>
        /// <param name="gridPathfinding">要封装的对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public SynchronizedGridPathfinding(GridPathfinding gridPathfinding)
        {
            pathfinding = gridPathfinding ?? throw new ArgumentNullException();
        }

        private GridPathfinding pathfinding;

        public override bool IsDiagonally
        {
            get
            {
                return pathfinding.IsDiagonally;
            }
        }

        public override bool IsFourDirections
        {
            get
            {
                return pathfinding.IsFourDirections;
            }
        }

        public sealed override bool Calculation(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list)
        {
            lock(pathfinding) return pathfinding.Calculation(gridObjects, startPos, endPos, list);
        }

    }

}
