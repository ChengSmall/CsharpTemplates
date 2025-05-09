using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Cheng.Algorithm.HashCodes;

using DNum = Cheng.DataStructure.NumGenerators.DynamicNumber;


namespace Cheng.DataStructure.NumGenerators
{

    #region

    /// <summary>
    /// 数值类型
    /// </summary>
    public enum NumType : int
    {
        /// <summary>
        /// 整数类型
        /// </summary>
        Integer = 0,

        /// <summary>
        /// 浮点数类型
        /// </summary>
        RealNumber = 1
    }

    /// <summary>
    /// 一个整数或浮点数类型的值
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct DynamicNumber : IEquatable<DynamicNumber>, IHashCode64, IFormattable, IComparable<DynamicNumber>
    {

        #region 构造

        /// <summary>
        /// 默认初始化为0
        /// </summary>
        /// <param name="type">数值类型</param>
        public DynamicNumber(NumType type)
        {
            this = default;
            this.type = type;
        }

        /// <summary>
        /// 初始化整数值
        /// </summary>
        /// <param name="value">整数值</param>
        public DynamicNumber(long value)
        {
            this = default;
            this.valueInteger = value;
            type = NumType.Integer;
        }

        /// <summary>
        /// 初始化整数值
        /// </summary>
        /// <param name="value">整数值</param>
        public DynamicNumber(int value)
        {
            this = default;
            this.valueInteger = value;
            type = NumType.Integer;
        }

        /// <summary>
        /// 初始化浮点值
        /// </summary>
        /// <param name="value">浮点值</param>
        public DynamicNumber(double value)
        {
            this = default;
            this.valueRealNum = value;
            type = NumType.RealNumber;
        }

        /// <summary>
        /// 初始化浮点值
        /// </summary>
        /// <param name="value">浮点值</param>
        public DynamicNumber(float value)
        {
            this = default;
            this.valueRealNum = value;
            type = NumType.RealNumber;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="type">值类型</param>
        /// <param name="value">指向一个至少存在8个字节可用内存的首地址，用于初始化值</param>
        public DynamicNumber(NumType type, void* value)
        {
            this = default;
            this.type = type;
            if (type == NumType.Integer)
            {
                this.valueInteger = *(long*)value;
            }
            else
            {
                this.valueRealNum = *(double*)value;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 数值类型
        /// </summary>
        [FieldOffset(0)]
        public readonly NumType type;

        /// <summary>
        /// 整数值，仅在参数<see cref="type"/>为<see cref="NumType.Integer"/>时有意义
        /// </summary>
        [FieldOffset(sizeof(NumType))]
        public readonly long valueInteger;

        /// <summary>
        /// 浮点值，仅在参数<see cref="type"/>为<see cref="NumType.RealNumber"/>时有意义
        /// </summary>
        [FieldOffset(sizeof(NumType))]
        public readonly double valueRealNum;

        #endregion

        #region 功能

        #region 转换

        /// <summary>
        /// 设置新的数据类型并返回新设置的值
        /// </summary>
        /// <param name="type">要使用的新数值类型</param>
        /// <returns>新类型设置的值</returns>
        public DNum SetType(NumType type)
        {
            long v = valueInteger;
            return new DNum(type, &v);
        }

        #region 强转重写

        public static implicit operator DynamicNumber(long value)
        {
            return new DynamicNumber(value);
        }

        public static explicit operator DynamicNumber(ulong value)
        {
            return new DynamicNumber((long)value);
        }

        public static implicit operator DynamicNumber(int value)
        {
            return new DynamicNumber((long)value);
        }

        public static implicit operator DynamicNumber(uint value)
        {
            return new DynamicNumber((long)value);
        }


        public static implicit operator DynamicNumber(short value)
        {
            return new DynamicNumber((long)value);
        }

        public static implicit operator DynamicNumber(ushort value)
        {
            return new DynamicNumber((long)value);
        }

        public static implicit operator DynamicNumber(byte value)
        {
            return new DynamicNumber((long)value);
        }

        public static implicit operator DynamicNumber(sbyte value)
        {
            return new DynamicNumber((long)value);
        }


        public static implicit operator DynamicNumber(char value)
        {
            return new DynamicNumber((long)value);
        }


        public static implicit operator DynamicNumber(double value)
        {
            return new DynamicNumber(value);
        }

        public static implicit operator DynamicNumber(float value)
        {
            return new DynamicNumber((double)value);
        }


        public static explicit operator DynamicNumber(decimal value)
        {
            return new DynamicNumber((double)value);
        }


        public static explicit operator long(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return value.valueInteger;
            return (long)value.valueRealNum;
        }

        public static explicit operator double(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return value.valueInteger;
            return value.valueRealNum;
        }

        public static explicit operator float(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return value.valueInteger;
            return (float)value.valueRealNum;
        }

        public static explicit operator int(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (int)value.valueInteger;
            return (int)value.valueRealNum;
        }

        public static explicit operator uint(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (uint)value.valueInteger;
            return (uint)value.valueRealNum;
        }

        public static explicit operator short(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (short)value.valueInteger;
            return (short)value.valueRealNum;
        }

        public static explicit operator ushort(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (ushort)value.valueInteger;
            return (ushort)value.valueRealNum;
        }

        public static explicit operator byte(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (byte)value.valueInteger;
            return (byte)value.valueRealNum;
        }

        public static explicit operator sbyte(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (sbyte)value.valueInteger;
            return (sbyte)value.valueRealNum;
        }


        public static explicit operator char(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (char)value.valueInteger;
            return (char)value.valueRealNum;
        }

        /// <summary>
        /// 强转为十进制数
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.OverflowException">超出范围</exception>
        public static explicit operator decimal(DynamicNumber value)
        {
            if (value.type == NumType.Integer) return (decimal)value.valueInteger;
            return (decimal)value.valueRealNum;
        }

        #endregion

        #endregion

        #region 运算符重载

        #region 比较

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static bool operator ==(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == n2.type) return n1.valueInteger == n2.valueInteger;
            return false;
        }

        /// <summary>
        /// 比较是否不相等
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static bool operator !=(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == n2.type) return n1.valueInteger != n2.valueInteger;
            return true;
        }

        public static bool operator <(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger < n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger < n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum < n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum < n2.valueRealNum;
                }
            }
        }

        public static bool operator >(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger > n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger > n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum > n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum > n2.valueRealNum;
                }
            }
        }

        public static bool operator <=(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger <= n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger <= n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum <= n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum <= n2.valueRealNum;
                }
            }
        }

        public static bool operator >=(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger >= n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger >= n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum >= n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum >= n2.valueRealNum;
                }
            }
        }

        #endregion

        #region 运算

        public static DynamicNumber operator +(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger + n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger + n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum + n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum + n2.valueRealNum;
                }
            }
        }

        public static DynamicNumber operator -(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger - n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger - n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum - n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum - n2.valueRealNum;
                }
            }
        }

        public static DynamicNumber operator *(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    return n1.valueInteger * n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger * n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum * n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum * n2.valueRealNum;
                }
            }
        }

        public static DynamicNumber operator /(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    if (n2.valueInteger == 0) return new DynamicNumber(double.PositiveInfinity);
                    return n1.valueInteger / n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger / n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum / n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum / n2.valueRealNum;
                }
            }
        }

        public static DynamicNumber operator %(DynamicNumber n1, DynamicNumber n2)
        {
            if (n1.type == NumType.Integer)
            {
                if (n2.type == NumType.Integer)
                {
                    //全 i
                    if (n2.valueInteger == 0) return new DynamicNumber(double.NaN);
                    return n1.valueInteger % n2.valueInteger;
                }
                else
                {
                    //i f
                    return n1.valueInteger % n2.valueRealNum;
                }
            }
            else
            {
                if (n2.type == NumType.Integer)
                {
                    //f i
                    return n1.valueRealNum % n2.valueInteger;
                }
                else
                {
                    //全 f
                    return n1.valueRealNum % n2.valueRealNum;
                }
            }
        }

        public static DynamicNumber operator -(DynamicNumber num)
        {
            if (num.type == NumType.Integer) return new DynamicNumber(-num.valueInteger);
            return new DynamicNumber(-num.valueRealNum);
        }

        public static DynamicNumber operator ++(DynamicNumber num)
        {
            if (num.type == NumType.Integer) return new DynamicNumber(num.valueInteger + 1);
            return new DynamicNumber(num.valueRealNum + 1);
        }

        #endregion

        #endregion

        #region 字符串返回

        /// <summary>
        /// 以字符串的方式返回值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (type == NumType.Integer) return valueInteger.ToString();
            return valueRealNum.ToString();
        }

        /// <summary>
        /// 以字符串的方式返回值
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <returns></returns>
        /// <exception cref="FormatException"><paramref name="format"/>无效或不支持</exception>
        public string ToString(string format)
        {
            if (type == NumType.Integer) return valueInteger.ToString(format);
            return valueRealNum.ToString(format);
        }

        /// <summary>
        /// 以字符串的方式返回值
        /// </summary>
        /// <param name="format">一个数值格式字符串</param>
        /// <param name="formatProvider">提供有关此实例的区域性特定格式设置信息</param>
        /// <returns></returns>
        /// <exception cref="FormatException"><paramref name="format"/>无效或不支持</exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (type == NumType.Integer) return valueInteger.ToString(format, formatProvider);
            return valueRealNum.ToString(format, formatProvider);
        }

        #endregion

        #region 派生比较接口

        /// <summary>
        /// 与另一个值对比是否一致
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DynamicNumber other)
        {
            if (this.type == other.type) return this.valueInteger == other.valueInteger;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is DynamicNumber num) return this == num;
            return false;
        }

        public override int GetHashCode()
        {
            if (type == NumType.Integer) return valueInteger.GetHashCode();
            return ~(valueInteger.GetHashCode());
        }

        public long GetHashCode64()
        {
            if (type == NumType.Integer) return valueInteger;
            return ~valueInteger;
        }

        /// <summary>
        /// 与另一个值比较大小
        /// </summary>
        /// <param name="other"></param>
        /// <returns>当前值较小返回值小于0，当前值较大返回值大于0，两值相等返回0</returns>
        public int CompareTo(DynamicNumber other)
        {
            if (type == NumType.Integer)
            {
                if (other.type == NumType.Integer)
                {
                    return valueInteger.CompareTo(other.valueInteger);
                }
                else
                {
                    //num 和 double
                    return ((double)valueInteger).CompareTo(other.valueRealNum);
                }
            }
            else
            {
                if (other.type == NumType.Integer)
                {
                    //double 和 num
                    return valueRealNum.CompareTo(other.valueInteger);
                }
                else
                {
                    return valueRealNum.CompareTo(other.valueRealNum);
                }
            }

        }

        #endregion

        #endregion

    }

    #endregion

}
