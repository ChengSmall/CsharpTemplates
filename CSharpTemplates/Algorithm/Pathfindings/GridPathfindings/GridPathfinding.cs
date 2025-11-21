using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Pathfindings
{

    /// <summary>
    /// 以网格为地图的寻路算法公共接口
    /// </summary>
    public interface IGridPathfinding
    {

        /// <summary>
        /// 该算法是否可计算允许斜向行走的路径（拥有八个行走方向）
        /// </summary>
        bool CanDiagonally { get; }

        /// <summary>
        /// 该算法是否可计算四向行走的地图（仅允许上下左右四个方向）
        /// </summary>
        bool CanFourDirections { get; }

        /// <summary>
        /// 开始计算当前从<paramref name="startPos"/>位置到<paramref name="endPos"/>位置的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <remarks>
        /// 以四向行走模式开始计算路径
        /// </remarks>
        /// <param name="gridObjects">要计算的地图数据</param>
        /// <param name="startPos">要计算的起始位置</param>
        /// <param name="endPos">要到达的目标位置</param>
        /// <param name="list">
        /// <para>该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>开始按行动路线顺序依次添加坐标点直至<paramref name="endPos"/></para>
        /// </param>
        /// <returns>
        /// <para>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</para>
        /// <para>当返回false时，<paramref name="list"/>实例的变化不固定</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定点位超出地图范围</exception>
        /// <exception cref="NotSupportedException">给定的集合不是可变集合，或不支持四向行走模式</exception>
        bool CalculationByFourDirections(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list);

        /// <summary>
        /// 开始计算当前从<paramref name="startPos"/>位置到<paramref name="endPos"/>位置的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <remarks>
        /// 以八向行走模式开始计算路径
        /// </remarks>
        /// <param name="gridObjects">要计算的地图数据</param>
        /// <param name="startPos">要计算的起始位置</param>
        /// <param name="endPos">要到达的目标位置</param>
        /// <param name="list">
        /// <para>该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>开始按行动路线顺序依次添加坐标点直至<paramref name="endPos"/></para>
        /// </param>
        /// <returns>
        /// <para>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</para>
        /// <para>当返回false时，<paramref name="list"/>实例的变化不固定</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定点位超出地图范围</exception>
        /// <exception cref="NotSupportedException">给定的集合不是可变集合，或不支持八向行走模式</exception>
        bool CalculationByDiagonally(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list);


    }


    /// <summary>
    /// 以网格地图为基础的寻路算法基类
    /// </summary>
    public abstract class GridPathfinding : IGridPathfinding
    {

        #region 权限

        /// <summary>
        /// 该算法是否可计算允许斜向行走的路径（拥有八个行走方向）
        /// </summary>
        public virtual bool CanDiagonally => false;

        /// <summary>
        /// 该算法是否可计算四向行走的地图（仅允许上下左右四个方向）
        /// </summary>
        public virtual bool CanFourDirections => false;

        #endregion

        #region 功能

        /// <summary>
        /// 开始计算当前从<paramref name="startPos"/>位置到<paramref name="endPos"/>位置的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <remarks>
        /// 以四向行走模式开始计算路径
        /// </remarks>
        /// <param name="gridObjects">要计算的地图数据</param>
        /// <param name="startPos">要计算的起始位置</param>
        /// <param name="endPos">要到达的目标位置</param>
        /// <param name="list">
        /// <para>该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>开始按行动路线顺序依次添加坐标点直至<paramref name="endPos"/></para>
        /// </param>
        /// <returns>
        /// <para>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</para>
        /// <para>当返回false时，<paramref name="list"/>实例的变化不固定</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定点位超出地图范围</exception>
        /// <exception cref="NotSupportedException">给定的集合不是可变集合，或不支持四向行走模式</exception>
        public virtual bool CalculationByFourDirections(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 开始计算当前从<paramref name="startPos"/>位置到<paramref name="endPos"/>位置的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <remarks>
        /// 以八向行走模式开始计算路径
        /// </remarks>
        /// <param name="gridObjects">要计算的地图数据</param>
        /// <param name="startPos">要计算的起始位置</param>
        /// <param name="endPos">要到达的目标位置</param>
        /// <param name="list">
        /// <para>该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>开始按行动路线顺序依次添加坐标点直至<paramref name="endPos"/></para>
        /// </param>
        /// <returns>
        /// <para>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</para>
        /// <para>当返回false时，<paramref name="list"/>实例的变化不固定</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定点位超出地图范围</exception>
        /// <exception cref="NotSupportedException">给定的集合不是可变集合，或不支持八向行走模式</exception>
        public virtual bool CalculationByDiagonally(IGridObjects gridObjects, PointInt2 startPos, PointInt2 endPos, IList<PointInt2> list)
        {
            throw new NotSupportedException();
        }
        #endregion

    }

}
