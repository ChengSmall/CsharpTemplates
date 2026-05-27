using System;

namespace Cheng.Algorithm.Pathfindings.GridPathfindings
{

    /// <summary>
    /// 用于访问地图格子数据的公共接口
    /// </summary>
    public interface IGridMap
    {

        /// <summary>
        /// 该地图允许斜向行进
        /// </summary>
        /// <value>
        /// <para>该参数是true时，允许以对角线行进；例如当坐标(0,0)的上下左右全部是死路时，可以直接前往(1,1)</para>
        /// <para>当参数是false时，只能直线行进；例如当坐标(0,0)的上下左右全部是死路时，坐标(0,0)是一个无法行进的独立空间</para>
        /// </value>
        bool CanDiagonally { get; }

        /// <summary>
        /// 检测当前地图指定格子的移动代价
        /// </summary>
        /// <param name="x">x坐标，可用范围区间在[0,<see cref="Width"/>)</param>
        /// <param name="y">y坐标，可用围区间在[0,<see cref="Height"/>)</param>
        /// <returns>
        /// <para>如果返回值为0，表示该位置可完全通行；返回值小于0，表示此处无法同行</para>
        /// <para>
        /// 如果返回值大于0，表示位置可通行，但是有一定通行代价；返回值越大，行进的代价越高<br/>
        /// 代价值是一个抽象参数，可通行坐标的代价值的区间在[0,<see cref="MaxPrice"/>)；可视为路面崎岖、地形不同带来的前进成本，值越大通行成本越大，返回值0是所有地图格代价最小的路面，表示无代价通行。无法通行的路面返回值一定小于0
        /// </para>
        /// <para>如果坐标<paramref name="x"/>或<paramref name="y"/>超出地图范围，返回值也小于0</para>
        /// </returns>
        int GetGridPrice(int x, int y);

        /// <summary>
        /// 最大代价值
        /// </summary>
        /// <value>
        /// <para>函数<see cref="GetGridPrice(int, int)"/>可返回的最大值；在派生类实现该参数时，值不能小于0</para>
        /// </value>
        int MaxPrice { get; }

        /// <summary>
        /// 地图x轴的长度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 地图y轴的长度
        /// </summary>
        int Height { get; }

    }

}
