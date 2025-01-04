

namespace Cheng.Algorithm.Randoms.Point
{
    /// <summary>
    /// 点随机生成器公共接口
    /// </summary>
    public interface IPointRandom
    {
        /// <summary>
        /// 根据给定的点生成一个值
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="z">z坐标</param>
        /// <returns>一个随机波动的值</returns>
        double Generate(double x, double y, double z);
    }


}


