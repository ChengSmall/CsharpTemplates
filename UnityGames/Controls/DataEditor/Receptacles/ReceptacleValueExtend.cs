using Cheng.Unitys;
using System;


namespace Cheng.DataStructure.Receptacles
{

    /// <summary>
    /// Unity GUI结构<see cref="ReceptacleValue{T}"/> 和 Receptacle容器 转化方法
    /// </summary>
    public static class ReceptacleValueExtend
    {

        #region

        #region ToRec

        /// <summary>
        /// 将结构转化为<see cref="float"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleFloat ToReceptacle(this ReceptacleValue<float> value)
        {
            return new ReceptacleFloat(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="double"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleDouble ToReceptacle(this ReceptacleValue<double> value)
        {
            return new ReceptacleDouble(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="Int32"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleInt32 ToReceptacle(this ReceptacleValue<int> value)
        {
            return new ReceptacleInt32(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="Int64"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleInt64 ToReceptacle(this ReceptacleValue<long> value)
        {
            return new ReceptacleInt64(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="UInt32"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleUInt32 ToReceptacle(this ReceptacleValue<uint> value)
        {
            return new ReceptacleUInt32(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="UInt64"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleUInt64 ToReceptacle(this ReceptacleValue<ulong> value)
        {
            return new ReceptacleUInt64(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="short"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleInt16 ToReceptacle(this ReceptacleValue<short> value)
        {
            return new ReceptacleInt16(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="ushort"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleUInt16 ToReceptacle(this ReceptacleValue<ushort> value)
        {
            return new ReceptacleUInt16(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="byte"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleByte ToReceptacle(this ReceptacleValue<byte> value)
        {
            return new ReceptacleByte(value.value, value.maxValue);
        }

        /// <summary>
        /// 将结构转化为<see cref="decimal"/>容器
        /// </summary>
        /// <returns></returns>
        public static ReceptacleDecimal ToReceptacle(this ReceptacleValue<UDecimal> value)
        {
            return new ReceptacleDecimal(value.value, value.maxValue);
        }

        #endregion

        #region ToValue

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<byte> ToReceptacle(this ReceptacleByte rec)
        {
            return new ReceptacleValue<byte>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<short> ToReceptacle(this ReceptacleInt16 rec)
        {
            return new ReceptacleValue<short>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<ushort> ToReceptacle(this ReceptacleUInt16 rec)
        {
            return new ReceptacleValue<ushort>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<int> ToReceptacle(this ReceptacleInt32 rec)
        {
            return new ReceptacleValue<int>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<uint> ToReceptacle(this ReceptacleUInt32 rec)
        {
            return new ReceptacleValue<uint>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<long> ToReceptacle(this ReceptacleInt64 rec)
        {
            return new ReceptacleValue<long>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<ulong> ToReceptacle(this ReceptacleUInt64 rec)
        {
            return new ReceptacleValue<ulong>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<float> ToReceptacle(this ReceptacleFloat rec)
        {
            return new ReceptacleValue<float>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<double> ToReceptacle(this ReceptacleDouble rec)
        {
            return new ReceptacleValue<double>(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 将容器转化为Unity GUI数据结构
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>转化后的值</returns>
        public static ReceptacleValue<UDecimal> ToReceptacle(this ReceptacleDecimal rec)
        {
            return new ReceptacleValue<UDecimal>(rec.value, rec.maxValue);
        }

        #endregion

        #endregion

    }

}
