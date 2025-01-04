using Cheng.Algorithm.HashCodes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cheng.Unitys
{

    /// <summary>
    /// Unity GUI <see cref="decimal"/> 绘制类型投影
    /// </summary>
    [Serializable]
    public unsafe struct UDecimal : IEquatable<UDecimal>, IComparable<UDecimal>, IFormattable, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 使用<see cref="decimal"/>初始化GUI值
        /// </summary>
        /// <param name="dec"></param>
        public UDecimal(decimal dec)
        {
            this = *(UDecimal*)&dec;
        }

        /// <summary>
        /// 使用4个int初始化
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="i3"></param>
        /// <param name="i4"></param>
        public UDecimal(int i1, int i2, int i3, int i4)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.i3 = i3;
            this.i4 = i4;
        }

        #endregion

        #region 参数

#pragma warning disable IDE0044

        [SerializeField] private int i1;
        [SerializeField] private int i2;
        [SerializeField] private int i3;
        [SerializeField] private int i4;

#pragma warning restore IDE0044

        #endregion

        #region 功能

        #region 转化功能

        /// <summary>
        /// 获取内部参数
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="i3"></param>
        /// <param name="i4"></param>
        public void GetValue(out int i1, out int i2, out int i3, out int i4)
        {
            i1 = this.i1;
            i2 = this.i2;
            i3 = this.i3;
            i4 = this.i4;
        }

        /// <summary>
        /// 转化为十进制数<see cref="decimal"/>
        /// </summary>
        /// <returns></returns>
        public decimal ToDec()
        {
            var t = this;
            UDecimal* up = &t;
            return *(decimal*)up;            
        }

        #endregion

        #region 派生

        public bool Equals(UDecimal other)
        {
            return this.i1 == other.i1 && this.i2 == other.i2 && this.i3 == other.i3 && this.i4 == other.i4;
        }

        public override bool Equals(object obj)
        {
            if (obj is UDecimal other)
            {
                return this.i1 == other.i1 && this.i2 == other.i2 && this.i3 == other.i3 && this.i4 == other.i4;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToDec().GetHashCode();
        }

        public int CompareTo(UDecimal other)
        {
            return this.ToDec().CompareTo(other.ToDec());
        }

        /// <summary>
        /// 将值转换等效的字符串表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToDec().ToString();
        }

        /// <summary>
        /// 使用指定的格式，将此实例的数值转换为它的等效字符串表示形式
        /// </summary>
        /// <param name="format">标准或自定义的数值格式字符串</param>
        /// <returns>由 <paramref name="format"/> 指定的值字符串表示形式</returns>
        public string ToString(string format)
        {
            return this.ToDec().ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToDec().ToString(format, formatProvider);
        }

        public long GetHashCode64()
        {
            return ToDec().GetHashCode64();
        }

        #endregion

        #region 类型转换

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="ud"></param>
        public static implicit operator decimal(UDecimal ud)
        {
            return ud.ToDec();
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="dec"></param>
        public static implicit operator UDecimal(decimal dec)
        {
            return new UDecimal(dec);
        }

        public static implicit operator UDecimal(int i)
        {
            return new UDecimal((decimal)i);
        }

        public static implicit operator UDecimal(long i)
        {
            return new UDecimal((decimal)i);
        }

        public static explicit operator UDecimal(float f)
        {
            return new UDecimal((decimal)f);
        }

        public static explicit operator UDecimal(double d)
        {
            return new UDecimal((decimal)d);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 编辑器数据方法扩展
    /// </summary>
    public static partial class DataEditorExd
    {

        /// <summary>
        /// 转化为 Unity GUI 对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UDecimal ToDec(this decimal value)
        {
            return new UDecimal(value);
        }

    }

}
