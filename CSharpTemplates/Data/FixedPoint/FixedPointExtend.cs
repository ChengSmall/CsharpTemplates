using System;


namespace Cheng.DataStructure.FixedPoints
{

    /// <summary>
    /// 定点数的扩展和测试功能
    /// </summary>
    /// <remarks>
    /// <para>提供一组定点数的扩展方法，用于快速编写测试代码，非稳定运算</para>
    /// </remarks>
    public static class FixedPointUnsafeExtend
    {

        /// <summary>
        /// 将浮点数转换到<see cref="FixedPoint32I24P8">32位定点数</see>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FixedPoint32I24P8 DoubleToFPT(this double value)
        {
            var t = Math.Truncate(value);
            // 小数部分
            var d = (uint)((value - t) * 255);
            return new FixedPoint32I24P8((((uint)t) << 8) | (d & 0xFF));
        }

        /// <summary>
        /// 将<see cref="FixedPoint32I24P8">32位定点数</see>转换到浮点数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FPTToDouble(this FixedPoint32I24P8 value)
        {
            return ((double)value.IntegerRaw) + (value.FractionalRaw / 255D);
        }

        /// <summary>
        /// 将浮点数转换到<see cref="FixedPoint32I22P10">32位定点数</see>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FixedPoint32I22P10 DoubleToFPT10P(this double value)
        {
            var t = Math.Truncate(value);
            // 小数部分
            var d = (uint)((value - t) * 1024);
            return new FixedPoint32I22P10((((uint)t) << 10) | (d & 1023));
        }

        /// <summary>
        /// 将<see cref="FixedPoint32I22P10">32位定点数</see>转换到浮点数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FPTToDouble(this FixedPoint32I22P10 value)
        {
            return ((double)value.IntegerRaw) + (value.FractionalRaw / 1023d);
        }

        /// <summary>
        /// 将浮点数转换到<see cref="FixedPoint64I48P16">64位定点数</see>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FixedPoint64I48P16 DoubleToFPT64(this double value)
        {
            var t = Math.Truncate(value);
            // 小数部分
            var d = (ulong)((value - t) * 65535);
            return new FixedPoint64I48P16((((ulong)t) << 16) | (d & 0xFFFF));
        }

        /// <summary>
        /// 将<see cref="FixedPoint64I48P16">64位定点数</see>转换到浮点数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FPTToDouble(this FixedPoint64I48P16 value)
        {
            return ((double)value.IntegerRaw) + (value.FractionalRaw / 65535D);
        }

    }

}
