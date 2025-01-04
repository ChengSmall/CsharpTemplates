using System;

namespace Cheng.Algorithm.Pathfindings
{

    /// <summary>
    /// 一个用于访问地图格子数据的公共实现接口
    /// </summary>
    public interface IGridObjects
    {

        /// <summary>
        /// 检测当前地图指定格子的移动代价
        /// </summary>
        /// <param name="x">x坐标，其范围表示为[0,<see cref="Width"/>)</param>
        /// <param name="y">y坐标，其范围表示为[0,<see cref="Height"/>)</param>
        /// <returns>参数为true表示该地点允许行走；false则表示不允许行走或坐标不在地图范围内</returns>
        bool CanMove(int x, int y);

        /// <summary>
        /// 地图x轴索引所在的长度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 地图y轴索引所在的长度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 单位是否拥有斜向行走的能力
        /// </summary>
        /// <returns>
        /// 该参数为true时，表示单位行走时能够以对角线的路径形式行走；false则单位只能以前后左右四个方向行走
        /// </returns>
        bool CanDiagonally { get; }
    }

}
