using Cheng.Algorithm.HashCodes;
using System;

namespace Cheng.DataStructure.Bits
{

    /// <summary>
    /// 64bit位域真值结构
    /// </summary>
    public struct Bit64 : IEquatable<Bit64>, IComparable<Bit64>, IHashCode64
    {

        #region 构造
        /// <summary>
        /// 初始化一个64bit位域结构
        /// </summary>
        /// <param name="value">
        /// 初始值
        /// <para>在C#7.0中，你可以使用0b开头写二进制字面常量，0b11111111_00000000，最右边第一位是索引0，依次向左递进</para>
        /// </param>
        public Bit64(ulong value)
        {
            p_value = value;
        }

        public Bit64(Bit32 b1, Bit32 b2)
        {
            p_value = (((ulong)b1.p_value) | ((ulong)b2.p_value << 32));
        }
        #endregion

        #region 参数

        internal ulong p_value;

        #endregion

        #region 功能

        #region 位域转化

        /// <summary>
        /// 访问指定索引的位值
        /// </summary>
        /// <param name="index">
        /// 位域索引
        /// <para>一般来讲范围在0~7，如果超出此范围，根据移位运算符的特性将会返回取32为模的索引</para>
        /// </param>
        /// <returns>true表示此位是1，false表示此位是0</returns>
        public bool this[int index]
        {
            get
            {
                //if (index < 0 || index > 7) throw new IndexOutOfRangeException();
                return ((p_value >> index) & 0b1) == 1 ? true : false;
            }
        }

        /// <summary>
        /// 获取位域值
        /// </summary>
        public ulong Value
        {
            get => p_value;
        }

        /// <summary>
        /// 设置位值并返回
        /// </summary>
        /// <param name="index">位域索引</param>
        /// <param name="value">位值</param>
        /// <returns>新的值</returns>
        public Bit64 SetBit(int index, bool value)
        {
            const ulong b1 = 1;
            var re = p_value;

            if (value) re |= (p_value | (b1 << index));
            else re &= ~(p_value | (b1 << index));

            return new Bit64(re);
        }

        #endregion

        #region 运算

        public static bool operator ==(Bit64 b1, Bit64 b2)
        {
            return b1.p_value == b2.p_value;
        }

        public static bool operator !=(Bit64 b1, Bit64 b2)
        {
            return b1.p_value != b2.p_value;
        }

        public static bool operator <(Bit64 b1, Bit64 b2)
        {
            return b1.p_value < b2.p_value;
        }

        public static bool operator >(Bit64 b1, Bit64 b2)
        {
            return b1.p_value > b2.p_value;
        }

        public static bool operator <=(Bit64 b1, Bit64 b2)
        {
            return b1.p_value <= b2.p_value;
        }

        public static bool operator >=(Bit64 b1, Bit64 b2)
        {
            return b1.p_value >= b2.p_value;
        }

        public static Bit64 operator >>(Bit64 bit, int offset)
        {
            return new Bit64((bit.p_value >> offset));
        }

        public static Bit64 operator <<(Bit64 bit, int offset)
        {
            return new Bit64((bit.p_value << offset));
        }

        #endregion

        #region 派生

        public override int GetHashCode()
        {
            return p_value.GetHashCode();
        }

        public unsafe override string ToString()
        {
            char* cp = stackalloc char[64];


            for (int i = 0; i < 64; i++)
            {
                cp[63 - i] = ((p_value >> i) & 1) == 1 ? '1' : '0';
            }

            return new string(cp, 0, 64);
        }

        public bool Equals(Bit64 other)
        {
            return p_value == other.p_value;
        }

        public int CompareTo(Bit64 other)
        {
            return p_value < other.p_value ? 1 : (p_value == other.p_value ? 0 : 1);
        }

        public override bool Equals(object obj)
        {
            if (obj is Bit64 b) return p_value == b.p_value;

            return false;
        }

        public long GetHashCode64()
        {
            return (long)this.p_value;
        }

        #endregion

        #endregion

    }


}
