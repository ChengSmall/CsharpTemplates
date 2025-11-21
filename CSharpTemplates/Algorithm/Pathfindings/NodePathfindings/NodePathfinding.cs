using Cheng.DataStructure;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure.Cherrsdinates;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Cheng.Algorithm.Pathfindings.NotePathfindings
{

    /// <summary>
    /// 以节点路径为地图的寻路算法公共接口
    /// </summary>
    public interface INotePathfinding
    {

        /// <summary>
        /// 计算从某个节点到另一个节点的路径并将结果填充到集合
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">要到达的节点</param>
        /// <param name="append">
        /// <para>在其中添加从起始节点<paramref name="start"/>开始到<paramref name="end"/>节点需要访问的索引列表</para>
        /// <para>例如集合为{1,2}则表示从<paramref name="start"/>开始，访问索引1得到的<see cref="INotePath"/>，再访问索引2得到的节点<see cref="INotePath.ID"/>值和<paramref name="end"/>相同</para>
        /// <para>没有添加新元素表示<paramref name="start"/>和<paramref name="end"/>是同一个节点</para>
        /// </param>
        /// <returns>是否成功寻找到目标；成功寻找到目标返回true，无法寻找到目标返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        bool Calculation(INotePath start, INotePath end, IList<int> append);

    }

    /// <summary>
    /// 以节点路径为地图的寻路算法基类
    /// </summary>
    public abstract class NotePathfinding : INotePathfinding
    {

        #region

        /// <summary>
        /// 计算从某个节点到另一个节点的路径并将结果填充到集合
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">要到达的节点</param>
        /// <param name="append">
        /// <para>在其中添加从起始节点<paramref name="start"/>开始到<paramref name="end"/>节点需要访问的索引列表</para>
        /// <para>例如集合为{1,2}则表示从<paramref name="start"/>开始，访问索引1得到的<see cref="INotePath"/>，再访问索引2得到的节点<see cref="INotePath.ID"/>值和<paramref name="end"/>相同</para>
        /// <para>没有添加新元素表示<paramref name="start"/>和<paramref name="end"/>是同一个节点</para>
        /// </param>
        /// <returns>是否成功寻找到目标；成功寻找到目标返回true，无法寻找到目标返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public abstract bool Calculation(INotePath start, INotePath end, IList<int> append);

        /// <summary>
        /// 计算从某个节点到另一个节点的路径并返回结果
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">要到达的节点</param>
        /// <returns>从起始节点<paramref name="start"/>开始到<paramref name="end"/>节点需要访问的索引列表，如果无法找到有效路径返回null</returns>
        public virtual int[] Calculation(INotePath start, INotePath end)
        {
            List<int> list = new List<int>();
            if (Calculation(start, end, list)) return list.ToArray();
            return null;
        }

        #endregion

    }

}