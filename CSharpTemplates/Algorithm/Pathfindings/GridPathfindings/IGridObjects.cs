using System;

namespace Cheng.Algorithm.Pathfindings
{

    /// <summary>
    /// 一个用于访问地图格子数据的公共接口
    /// </summary>
    public interface IGridObjects
    {

        /// <summary>
        /// 检测当前地图指定格子的移动代价
        /// </summary>
        /// <param name="x">x坐标，其范围表示为[0,<see cref="Width"/>)</param>
        /// <param name="y">y坐标，其范围表示为[0,<see cref="Height"/>)</param>
        /// <returns>
        /// <para>如果返回值为0，表示该位置可以允许行进；返回值小于0，表示坐标存在障碍物无法行进</para>
        /// <para>
        /// 如果返回值大于0，表示位置可以行进，但是行进时有一定代价，返回值越大，行进的代价越高
        /// </para>
        /// <para>如果坐标<paramref name="x"/>或<paramref name="y"/>超出地图范围，返回值也理应小于0</para>
        /// </returns>
        int GetGridPrice(int x, int y);

        /// <summary>
        /// 最大代价值
        /// </summary>
        /// <returns>函数<see cref="GetGridPrice(int, int)"/>可返回的最大值；在派生类实现该参数时，返回值不能小于0</returns>
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
