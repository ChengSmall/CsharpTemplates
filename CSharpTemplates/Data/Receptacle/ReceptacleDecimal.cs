using System;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Receptacles
{

    /// <summary>
    /// 表示一个有最大值的容器结构，十进制数
    /// </summary>
    [Serializable]
    public struct ReceptacleDecimal : IEquatable<ReceptacleDecimal>, IComparable<ReceptacleDecimal>, IHashCode64
    {

        #region 构造
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值和最大值</param>
        public ReceptacleDecimal(decimal value)
        {
            this.value = value;
            maxValue = value;
        }
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        public ReceptacleDecimal(decimal value, decimal maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 值
        /// </summary>
        public readonly decimal value;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly decimal maxValue;
        #endregion      

        #region 功能

        #region 容器变化

        /// <summary>
        /// 返回一个将值重置为最大值的容器
        /// </summary>
        /// <returns>值重置为最大值的新实例</returns>
        public ReceptacleDecimal ResetMax()
        {
            return new ReceptacleDecimal(maxValue, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public ReceptacleDecimal Add(decimal value)
        {
            var v = this.value + value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleDecimal(v, maxValue);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public ReceptacleDecimal Sub(decimal value)
        {
            var v = this.value - value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleDecimal(v, maxValue);
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于0，则值设为0并返回true；否则返回false</returns>
        public bool SubMinZero(decimal value, out ReceptacleDecimal reValue)
        {
            var v = this.value - value;
            bool flag = v <= 0;

            if (flag) v = 0;
            if (v > maxValue) v = maxValue;
            reValue = new ReceptacleDecimal(v, maxValue);
            return flag;
        }

        /// <summary>
        /// 减少值，指定减少到的最小值
        /// </summary>
        /// <param name="subValue">要减少的值</param>
        /// <param name="minValue">要减少最小的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于<paramref name="minValue"/>，则值设为<paramref name="minValue"/>并返回true；否则返回false</returns>
        public bool SubMin(decimal subValue, decimal minValue, out ReceptacleDecimal reValue)
        {
            var v = this.value - subValue;
            bool flag = v <= minValue;

            if (flag) v = minValue;
            if (v > maxValue) v = maxValue;
            reValue = new ReceptacleDecimal(v, maxValue);
            return flag;
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>运算结果</returns>
        public ReceptacleDecimal SubMinZero(decimal value)
        {
            var v = this.value - value;

            if (v < 0) v = 0;
            if (v > maxValue) v = maxValue;
            return new ReceptacleDecimal(v, maxValue);
        }

        /// <summary>
        /// 减少值，指定最小值
        /// </summary>
        /// <param name="subValue">要减去的值</param>
        /// <param name="minValue">指定最小值</param>
        /// <returns>运算结果</returns>
        public ReceptacleDecimal SubMin(decimal subValue, decimal minValue)
        {
            var v = this.value - subValue;

            if (v < minValue) v = minValue;
            if (v > maxValue) v = maxValue;
            return new ReceptacleDecimal(v, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="r1"></param>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public static ReceptacleDecimal operator +(ReceptacleDecimal r1, decimal value)
        {
            return r1.Add(value);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public static ReceptacleDecimal operator -(ReceptacleDecimal r1, decimal value)
        {
            return r1.Sub(value);
        }

        #endregion

        #region 参数判断

        /// <summary>
        /// 当前值是否为0
        /// </summary>
        public bool IsZero
        {
            get => value == 0M;
        }

        /// <summary>
        /// 当前值是否为最大值
        /// </summary>
        public bool IsFull
        {
            get => value == maxValue;
        }

        #endregion

        #region 比较运算

        /// <summary>
        /// 比较两个容器的值是否相等
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value == r2.value && r1.maxValue == r2.maxValue;
        }

        /// <summary>
        /// 比较两个容器的值是否不相等
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value != r2.value || r1.maxValue != r2.maxValue;
        }

        /// <summary>
        /// 比较两个容器的值-小于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value < r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value > r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-小于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <=(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value <= r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >=(ReceptacleDecimal r1, ReceptacleDecimal r2)
        {
            return r1.value >= r2.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReceptacleDecimal r)
            {
                return this == r;
            }
            return false;
        }

        public unsafe override int GetHashCode()
        {
            return value.GetHashCode() ^ maxValue.GetHashCode();
        }

        public bool Equals(ReceptacleDecimal other)
        {
            return this == other;
        }

        /// <summary>
        /// 比较运算
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ReceptacleDecimal other)
        {
            return (value < other.value) ? -1 : (value > other.value ? 1 : 0);
        }
        #endregion

        #region 派生
        /// <summary>
        /// 返回容器的字符串形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value.ToString() + "/" + maxValue.ToString();
        }
        /// <summary>
        /// 返回容器的字符串形式
        /// </summary>
        /// <param name="format">数值的格式字符串</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return value.ToString(format) + "/" + maxValue.ToString(format);
        }
        public long GetHashCode64()
        {
            return (value.GetHashCode64()) ^ (maxValue.GetHashCode64());
        }
        #endregion

        #endregion

    }

}
