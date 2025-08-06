using Cheng.Algorithm.HashCodes;
using System;
using System.Runtime.InteropServices;

namespace Cheng.DataStructure.Receptacles
{

    /// <summary>
    /// 表示一个有最大值的容器结构，64位无符号整形
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ReceptacleUInt64 : IEquatable<ReceptacleUInt64>, IComparable<ReceptacleUInt64>, IHashCode64
    {

        #region 构造
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值和最大值</param>
        public ReceptacleUInt64(ulong value)
        {
            this.value = value;
            maxValue = value;
        }
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        public ReceptacleUInt64(ulong value, ulong maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 值
        /// </summary>
        public readonly ulong value;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly ulong maxValue;
        #endregion

        #region 功能

        #region 容器变化
        /// <summary>
        /// 返回一个将值重置为最大值的容器
        /// </summary>
        /// <returns>值重置为最大值的新实例</returns>
        public ReceptacleUInt64 ResetMax()
        {
            return new ReceptacleUInt64(maxValue, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public ReceptacleUInt64 Add(ulong value)
        {
            var v = this.value + value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleUInt64(v, maxValue);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public ReceptacleUInt64 Sub(ulong value)
        {
            var v = this.value - value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleUInt64(v, maxValue);
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于0，则值设为0并返回true；否则返回false</returns>
        public bool SubMinZero(ulong value, out ReceptacleUInt64 reValue)
        {
            if (value >= this.value)
            {
                reValue = new ReceptacleUInt64(0, maxValue);
                return true;
            }

            var v = this.value - value;

            //if (v > maxValue) v = maxValue;
            reValue = new ReceptacleUInt64(v, maxValue);
            return false;
        }

        /// <summary>
        /// 减少值，指定减少到的最小值
        /// </summary>
        /// <param name="subValue">要减少的值</param>
        /// <param name="minValue">要减少最小的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于<paramref name="minValue"/>，则值设为<paramref name="minValue"/>并返回true；否则返回false</returns>
        public bool SubMin(ulong subValue, ulong minValue, out ReceptacleUInt64 reValue)
        {

            if (this.value <= subValue)
            {
                reValue = new ReceptacleUInt64(minValue, maxValue);
                return true;
            }

            var v = this.value - subValue;
            if(v <= minValue)
            {
                reValue = new ReceptacleUInt64(minValue, maxValue);
                return true;
            }

            reValue = new ReceptacleUInt64(v, maxValue);
            return false;
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>运算结果</returns>
        public ReceptacleUInt64 SubMinZero(ulong value)
        {
            if(value >= this.value)
            {
                return new ReceptacleUInt64(0, maxValue);
            }

            var v = this.value - value;

            //if (v > maxValue) v = maxValue;
            return new ReceptacleUInt64(v, maxValue);
        }

        /// <summary>
        /// 减少值，指定最小值
        /// </summary>
        /// <param name="subValue">要减去的值</param>
        /// <param name="minValue">指定最小值</param>
        /// <returns>运算结果</returns>
        public ReceptacleUInt64 SubMin(ulong subValue, ulong minValue)
        {
            if (this.value <= subValue)
            {
                return new ReceptacleUInt64(minValue, maxValue);
            }

            var v = this.value - subValue;
            if (v <= minValue)
            {
                return new ReceptacleUInt64(minValue, maxValue);                
            }

            return new ReceptacleUInt64(v, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="r1"></param>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public static ReceptacleUInt64 operator +(ReceptacleUInt64 r1, ulong value)
        {
            return r1.Add(value);
        }
        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public static ReceptacleUInt64 operator -(ReceptacleUInt64 r1, ulong value)
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
            get => value == 0;
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
        /// 比较两个容器是否相等
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value == r2.value && r1.maxValue == r2.maxValue;
        }
        /// <summary>
        /// 比较两个容器是否不相等
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value != r2.value || r1.maxValue != r2.maxValue;
        }

        /// <summary>
        /// 比较两个容器的值-小于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value < r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value > r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-小于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <=(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value <= r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >=(ReceptacleUInt64 r1, ReceptacleUInt64 r2)
        {
            return r1.value >= r2.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReceptacleUInt64 r)
            {
                return this == r;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (value ^ maxValue).GetHashCode();
        }
        public bool Equals(ReceptacleUInt64 other)
        {
            return this == other;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">比较对象</param>
        /// <returns></returns>
        public int CompareTo(ReceptacleUInt64 other)
        {
            return this.value < other.value ? -1 : (this.value == other.value ? 0 : 1);
        }
        #endregion

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
            return (long)(value ^ maxValue);
        }
        #endregion

    }

}
