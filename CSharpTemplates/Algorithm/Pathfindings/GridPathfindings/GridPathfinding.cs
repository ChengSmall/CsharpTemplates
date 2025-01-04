using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Pathfindings
{


    /// <summary>
    /// 以网格地图为基础的寻路算法基类
    /// </summary>
    public abstract class GridPathfinding
    {

        #region 权限

        /// <summary>
        /// 该算法是否支持可斜向行走（拥有八个行走方位）的地图
        /// </summary>
        public virtual bool IsDiagonally => false;

        /// <summary>
        /// 该算法是否支持四向行走的地图
        /// </summary>
        public virtual bool IsFourDirections => false;

        #endregion

        #region 功能

        /// <summary>
        /// 开始计算当前从<paramref name="startPos"/>位置到<paramref name="endPos"/>位置的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <param name="gridObjects">要计算的地图数据</param>
        /// <param name="startPos">要计算的起始位置</param>
        /// <param name="endPos">要到达的目标位置</param>
        /// <param name="list">
        /// 该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>开始按行动路线顺序依次添加坐标点直至<paramref name="endPos"/>
        /// </param>
        /// <returns>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定点位超出地图范围</exception>
        /// <exception cref="NotSupportedException">给定的集合不是可变集合；<see cref="GridPathfinding.IsFourDirections"/>为false但是<paramref name="gridObjects"/>的<see cref="IGridObjects.CanDiagonally"/>为true，或<see cref="IsDiagonally"/>为false但是<paramref name="gridObjects"/>的<see cref="IGridObjects.CanDiagonally"/>为true</exception>
        public virtual bool Calculation(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list)
        {
            throw new NotSupportedException();
        }



        #endregion

    }

}
