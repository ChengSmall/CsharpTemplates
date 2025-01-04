using System;
using System.Collections.Generic;
using System.Text;


namespace Cheng.DataStructure.BoundedContainers
{

    /// <summary>
    /// BoundedContainerInt32 Unity GUI 扩展方法
    /// </summary>
    public static class UBoundedContainerExtend
    {

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerInt32 ToBC(this UBoundedContainer<int> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerInt32(value, min, max);
        }

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerInt64 ToBC(this UBoundedContainer<long> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerInt64(value, min, max);
        }

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerInt16 ToBC(this UBoundedContainer<short> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerInt16(value, min, max);
        }

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerFloat ToBC(this UBoundedContainer<float> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerFloat(value, min, max);
        }

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerDouble ToBC(this UBoundedContainer<double> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerDouble(value, min, max);
        }

        /// <summary>
        /// 将Unity GUI转化为可用结构值
        /// </summary>
        /// <param name="ubc"></param>
        /// <returns></returns>
        public static BoundedContainerDecimal ToBC(this UBoundedContainer<decimal> ubc)
        {
            ubc.GetValue(out var value, out var min, out var max);
            return new BoundedContainerDecimal(value, min, max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<int> ToBC(this BoundedContainerInt32 bc)
        {
            return new UBoundedContainer<int>(bc.value, bc.min, bc.max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<long> ToBC(this BoundedContainerInt64 bc)
        {
            return new UBoundedContainer<long>(bc.value, bc.min, bc.max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<short> ToBC(this BoundedContainerInt16 bc)
        {
            return new UBoundedContainer<short>(bc.value, bc.min, bc.max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<float> ToBC(this BoundedContainerFloat bc)
        {
            return new UBoundedContainer<float>(bc.value, bc.min, bc.max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<double> ToBC(this BoundedContainerDouble bc)
        {
            return new UBoundedContainer<double>(bc.value, bc.min, bc.max);
        }

        /// <summary>
        /// 将结构值转化为 Unity GUI 可存值
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        public static UBoundedContainer<decimal> ToBC(this BoundedContainerDecimal bc)
        {
            return new UBoundedContainer<decimal>(bc.value, bc.min, bc.max);
        }

    }

}
