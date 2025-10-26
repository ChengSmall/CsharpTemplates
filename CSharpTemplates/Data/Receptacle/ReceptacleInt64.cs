using Cheng.Algorithm.HashCodes;
using System;
using System.Runtime.InteropServices;
using ty = System.Int64;
using Rec = Cheng.DataStructure.Receptacles.ReceptacleInt64;

namespace Cheng.DataStructure.Receptacles
{

    /// <summary>
    /// 表示一个有最大值的容器结构，64位整形
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ReceptacleInt64 : IEquatable<ReceptacleInt64>, IComparable<ReceptacleInt64>, IHashCode64
    {

        #region 构造
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值和最大值</param>
        public ReceptacleInt64(long value)
        {
            this.value = value;
            maxValue = value;
        }
        /// <summary>
        /// 初始化容器，指定值和最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        public ReceptacleInt64(long value, long maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 值
        /// </summary>
        public readonly long value;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly long maxValue;
        #endregion

        #region 功能

        #region 容器变化
        /// <summary>
        /// 返回一个将值重置为最大值的容器
        /// </summary>
        /// <returns>值重置为最大值的新实例</returns>
        public ReceptacleInt64 ResetMax()
        {
            return new ReceptacleInt64(maxValue, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public ReceptacleInt64 Add(long value)
        {
            var v = this.value + value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleInt64(v, maxValue);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public ReceptacleInt64 Sub(long value)
        {
            var v = this.value - value;
            if (v > maxValue) v = maxValue;
            return new ReceptacleInt64(v, maxValue);
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于或等于0，则值设为0并返回true；否则返回false</returns>
        public bool SubMinZero(long value, out ReceptacleInt64 reValue)
        {
            var v = this.value - value;
            bool flag = v <= 0 || v > maxValue;

            if (flag) v = 0;
            if (v > maxValue) v = maxValue;
            reValue = new ReceptacleInt64(v, maxValue);
            return flag;
        }

        /// <summary>
        /// 减少值，指定减少到的最小值
        /// </summary>
        /// <param name="subValue">要减少的值</param>
        /// <param name="minValue">要减少最小的值</param>
        /// <param name="reValue">运算结果</param>
        /// <returns>当此次运算后的值小于或等于<paramref name="minValue"/>，则值设为<paramref name="minValue"/>并返回true；否则返回false</returns>
        public bool SubMin(long subValue, long minValue, out ReceptacleInt64 reValue)
        {
            var v = this.value - subValue;
            bool flag = v <= minValue || v > maxValue;

            if (flag) v = minValue;
            if (v > maxValue) v = maxValue;
            reValue = new ReceptacleInt64(v, maxValue);
            return flag;
        }

        /// <summary>
        /// 减少值，最小值指定为0
        /// </summary>
        /// <param name="value">要减少的值</param>
        /// <returns>运算结果</returns>
        public Rec SubMinZero(long value)
        {
            var v = this.value - value;
            if (v < 0 || v > this.maxValue) v = 0;
            return new Rec(v, maxValue);
        }

        /// <summary>
        /// 减少值，指定最小值
        /// </summary>
        /// <param name="subValue">要减去的值</param>
        /// <param name="minValue">指定最小值</param>
        /// <returns>运算结果</returns>
        public Rec SubMin(long subValue, long minValue)
        {
            var v = this.value - subValue;
            if (v < minValue || v > this.maxValue) v = minValue;
            return new Rec(v, maxValue);
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <remarks>值增加到最大值时便不会再增加</remarks>
        /// <param name="r1"></param>
        /// <param name="value">要增加的值</param>
        /// <returns>增加后的容器</returns>
        public static ReceptacleInt64 operator +(ReceptacleInt64 r1, long value)
        {
            return r1.Add(value);
        }

        /// <summary>
        /// 减少值
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="value">要减少的值</param>
        /// <returns>减少后的容器结构</returns>
        public static ReceptacleInt64 operator -(ReceptacleInt64 r1, long value)
        {
            return r1.Sub(value);
        }

        /// <summary>
        /// 通过最大值收束容器值并返回
        /// </summary>
        /// <returns>
        /// <para>容器对象，如果当前<see cref="value"/>大于<see cref="maxValue"/>，则返回一个值为<see cref="maxValue"/>的容器；否则直接返回对象副本</para>
        /// </returns>
        public Rec Clamp()
        {
            if (value > maxValue) return new Rec(maxValue);
            return this;
        }

        /// <summary>
        /// 设置最大值并保证容器值不超过最大值
        /// </summary>
        /// <param name="maxValue">要设置的最大值</param>
        /// <returns>
        /// <para>容器对象，如果当前容器的<see cref="value"/>大于<paramref name="maxValue"/>，则返回容器值为<paramref name="maxValue"/>的新对象；否则返回容器值是当前对象的<see cref="value"/>值，最大值为<paramref name="maxValue"/>的容器对象</para>
        /// </returns>
        public Rec SetMaxValue(ty maxValue)
        {
            if (this.value > maxValue) return new Rec(maxValue);
            return new Rec(this.value, maxValue);
        }

        /// <summary>
        /// 设置新值并使用最大值约束防止溢出
        /// </summary>
        /// <param name="value">要设置的新值</param>
        /// <returns>
        /// <para>如果<paramref name="value"/>大于当前容器的最大值，则返回以当前容器的最大值为参数的新对象；否则返回容器值为<paramref name="value"/>最大值不变的新对象</para>
        /// </returns>
        public Rec SetValue(ty value)
        {
            if (value > this.maxValue) return new Rec(this.maxValue);
            return new Rec(value, this.maxValue);
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
        public static bool operator ==(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value == r2.value && r1.maxValue == r2.maxValue;
        }
        /// <summary>
        /// 比较两个容器是否不相等
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value != r2.value || r1.maxValue != r2.maxValue;
        }

        /// <summary>
        /// 比较两个容器的值-小于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value < r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value > r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-小于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator <=(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value <= r2.value;
        }
        /// <summary>
        /// 比较两个容器的值-大于等于
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator >=(ReceptacleInt64 r1, ReceptacleInt64 r2)
        {
            return r1.value >= r2.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReceptacleInt64 r)
            {
                return this == r;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (value ^ maxValue).GetHashCode();
        }

        public bool Equals(ReceptacleInt64 other)
        {
            return this == other;
        }

        /// <summary>
        /// 有符号比较
        /// </summary>
        /// <param name="other">比较对象</param>
        /// <returns></returns>
        public int CompareTo(ReceptacleInt64 other)
        {
            return this.value < other.value ? -1 : (this.value == other.value ? 0 : 1);
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
            return value ^ maxValue;
        }
        #endregion

        #region 转化

        #region 元组转化

        /// <summary>
        /// 使用元组值初始化容器
        /// </summary>
        /// <param name="valueTuple">第一个值初始化为value，第二个值初始化为maxValue</param>
        public static implicit operator Rec((ty, ty) valueTuple)
        {
            return new Rec(valueTuple.Item1, valueTuple.Item2);
        }

        /// <summary>
        /// 将容器转化为元组值
        /// </summary>
        /// <param name="rec">value设为第一个参数，maxValue设为第二个参数</param>
        public static implicit operator (ty, ty)(Rec rec)
        {
            return (rec.value, rec.maxValue);
        }

        #endregion

        /// <summary>
        /// 转化为双浮点类型容器
        /// </summary>
        /// <param name="rec"></param>
        public static implicit operator ReceptacleDouble(Rec rec)
        {
            return new ReceptacleDouble(rec.value, rec.maxValue);
        }

        /// <summary>
        /// 指定最大值的满容器
        /// </summary>
        /// <param name="value">设置最大值和当前值</param>
        public static implicit operator Rec(ty value)
        {
            return new Rec(value);
        }

        #endregion

        #endregion

    }

}
