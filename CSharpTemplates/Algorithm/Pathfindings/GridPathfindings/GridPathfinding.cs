using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.Memorys;
using Cheng.DataStructure.Cherrsdinates;

namespace Cheng.Algorithm.Pathfindings.GridPathfindings
{

    /// <summary>
    /// 以网格为地图的寻路算法公共接口
    /// </summary>
    public interface IGridPathfinding
    {

        /// <summary>
        /// 计算从<paramref name="startPos"/>到<paramref name="endPos"/>的最佳路径，并将路径按顺序添加到指定集合中
        /// </summary>
        /// <param name="map">地图数据</param>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">目标位置</param>
        /// <param name="append">
        /// <para>该参数是一个可变集合，当该函数计算完毕后，会将路径从<paramref name="startPos"/>的下一个要行进到的坐标位置开始，按行动路线顺序依次添加坐标点直至<paramref name="endPos"/>的前一个坐标</para>
        /// <para>坐标添加使用<see cref="ICollection{T}.Add(T)"/>函数，不会处理集合已有的元素</para>
        /// </param>
        /// <returns>
        /// <para>如果两个点之间拥有完整的路径则返回true；如果当前无法从<paramref name="startPos"/>到达<paramref name="endPos"/>则返回false</para>
        /// <para>当返回false时，<paramref name="append"/>对象的变化不固定</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">集合是只读集合无法添加元素</exception>
        bool Calculation(IGridMap map, Point2I32 startPos, Point2I32 endPos, ICollection<Point2I32> append);

    }

    /// <summary>
    /// 以网格地图为基础的寻路算法基类
    /// </summary>
    public abstract class BaseGridPathfinding : SafreleaseUnmanagedResources, IGridPathfinding
    {

        /// <summary>
        /// 是否存在可释放的资源
        /// </summary>
        /// <value>当参数是true时，对象存在需要释放的资源，需要调用资源释放的代码；如果是false，则表示不需要释放资源</value>
        public virtual bool HavingDispose => false;

        public abstract bool Calculation(IGridMap map, Point2I32 startPos, Point2I32 endPos, ICollection<Point2I32> append);

        /// <summary>
        /// 计算从<paramref name="startPos"/>到<paramref name="endPos"/>的最佳路径，并返回路径集合
        /// </summary>
        /// <param name="map">地图数据</param>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">目标位置</param>
        /// <returns>返回从<paramref name="startPos"/>的下一个要行进到的坐标位置开始，按行动路线顺序直至<paramref name="endPos"/>的前一个坐标的数组；null表示无法找到通路</returns>
        public virtual Point2I32[] Calculation(IGridMap map, Point2I32 startPos, Point2I32 endPos)
        {
            List<Point2I32> list = new List<Point2I32>();
            if (Calculation(map, startPos, endPos, list)) return list.ToArray();
            return null;
        }

    }

}
