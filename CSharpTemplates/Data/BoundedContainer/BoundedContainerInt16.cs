using System;
using Cheng.Algorithm.HashCodes;

using BC = Cheng.DataStructure.BoundedContainers.BoundedContainerInt16;
using bc = System.Int16;

namespace Cheng.DataStructure.BoundedContainers
{

    /// <summary>
    /// 被限制最小值和最大值的容器——16位整型
    /// </summary>
    /// <remarks>
    /// 能够增加或减少，并限定在指定范围内的值
    /// </remarks>
    public struct BoundedContainerInt16 : IEquatable<BC>, IComparable<BC>, IHashCode64
    {

        #region 初始化

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="value">初始值</param>
        /// <param name="min">指定最小值</param>
        /// <param name="max">指定最大值</param>
        public BoundedContainerInt16(bc value, bc min, bc max)
        {
            this.value = value;
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// 初始化容器，以最小值为初始值
        /// </summary>
        /// <param name="min">指定最小值</param>
        /// <param name="max">指定最大值</param>
        public BoundedContainerInt16(bc min, bc max)
        {
            this.value = min;
            this.min = min;
            this.max = max;
        }

        #endregion

        #region 值

        /// <summary>
        /// 当前值
        /// </summary>
        public readonly bc value;

        /// <summary>
        /// 最小值
        /// </summary>
        public readonly bc min;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly bc max;

        #endregion

        #region 功能

        #region 运算

        #region 函数

        /// <summary>
        /// 增加值
        /// </summary>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的值</returns>
        public BC Add(bc value)
        {
            var re = this.value + value;

            if (re > this.max || re < this.min) re = this.max;

            return new BC((short)re, this.min, this.max);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的值</returns>
        public BC Sub(bc value)
        {
            var re = this.value - value;

            if (re > this.max || re < this.min) re = this.min;

            return new BC((short)re, this.min, this.max);
        }

        /// <summary>
        /// 增加值，并判断是否溢出
        /// </summary>
        /// <param name="value">要增加的值</param>
        /// <param name="reValue">增加后的值</param>
        /// <returns>如果增加后的值超出限制范围返回true，没有超出返回false</returns>
        public bool Add(bc value, out BC reValue)
        {
            var re = this.value + value;

            bool reb = (re > this.max || re < this.min);

            if (reb) re = this.max;

            reValue = new BC((short)re, this.min, this.max);
            return reb;
        }

        /// <summary>
        /// 减少值，并判断是否溢出
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <param name="reValue">减少后的值</param>
        /// <returns>如果减少后的值超出限制范围返回true，没有超出返回false</returns>
        public bool Sub(bc value, out BC reValue)
        {
            var re = this.value - value;

            bool reb = (re > this.max || re < this.min);

            if (reb) re = this.min;

            reValue = new BC((short)re, this.min, this.max);
            return reb;
        }

        /// <summary>
        /// 增加值，并返回溢出的量
        /// </summary>
        /// <param name="value">要增加的值</param>
        /// <param name="overflow">此次增加后溢出的值</param>
        /// <returns>增加后的值</returns>
        public BC Add(bc value, out bc overflow)
        {
            var re = this.value + value;

            if (re > this.max || re < this.min)
            {
                overflow = (short)(re - this.max);
                re = this.max;
            }
            else
            {
                overflow = 0;
            }

            return new BC((short)re, this.min, this.max);
        }

        /// <summary>
        /// 减少值，并返回溢出的量
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <param name="overflow">此次减少后溢出的值</param>
        /// <returns>减少后的值</returns>
        public BC Sub(bc value, out bc overflow)
        {
            var re = this.value - value;

            if (re > this.max || re < this.min)
            {
                overflow = (short)(this.min - re);
                re = this.max;
            }
            else
            {
                overflow = 0;
            }

            return new BC((short)re, this.min, this.max);
        }

        #endregion

        #region 参数获取

        /// <summary>
        /// 判断当前值等于指定的最小值
        /// </summary>
        public bool IsMin
        {
            get => this.value <= this.min;
        }

        /// <summary>
        /// 判断当前值等于指定的最大值
        /// </summary>
        public bool IsMax
        {
            get => this.value >= this.max;
        }

        /// <summary>
        /// 返回最小值与当前值的差
        /// </summary>
        public int OverFlowMin
        {
            get => this.value - this.min;
        }

        /// <summary>
        /// 返回当前值与最大值的差
        /// </summary>
        public int OverFlowMax
        {
            get => (this.max - this.value);
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="bc"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BC operator +(BC bc, bc value)
        {
            return bc.Add(value);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="bc"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BC operator -(BC bc, bc value)
        {
            return bc.Sub(value);
        }

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(BC a, BC b)
        {
            return a.value == b.value && a.max == b.max && a.min == b.min;
        }

        /// <summary>
        /// 判断不相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(BC a, BC b)
        {
            return a.value != b.value || a.max != b.max || a.min != b.min;
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 比较与另一个值是否一致
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BC other)
        {
            return this.value == other.value && this.max == other.max && this.min == other.min;
        }

        public override bool Equals(object obj)
        {
            if (obj is BC other)
            {
                return this.value == other.value && this.max == other.max && this.min == other.min;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ max.GetHashCode() ^ min.GetHashCode();
        }

        public int CompareTo(BC other)
        {
            return this.value.CompareTo(other.value);
        }

        public long GetHashCode64()
        {
            return value.GetHashCode64() ^ max.GetHashCode64() ^ min.GetHashCode64();
        }

        /// <summary>
        /// 以字符串的形式返回当前值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(24);
            sb.Append(min.ToString());
            sb.Append(" <- ");
            sb.Append(value.ToString());
            sb.Append(" -> ");
            sb.Append(max.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// 使用指定格式返回当前值
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(24);
            sb.Append(min.ToString(format));
            sb.Append(" <- ");
            sb.Append(value.ToString(format));
            sb.Append(" -> ");
            sb.Append(max.ToString(format));
            return sb.ToString();
        }

        #endregion

        #endregion

    }

}
