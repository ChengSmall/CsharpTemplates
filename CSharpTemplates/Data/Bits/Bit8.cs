using System;

namespace Cheng.DataStructure.Bits
{

    /// <summary>
    /// 8bit位域真值结构
    /// </summary>
    [Serializable]
    public struct Bit8 : IEquatable<Bit8>, IComparable<Bit8>
    {

        #region 构造

        /// <summary>
        /// 初始化一个8比特位域结构
        /// </summary>
        /// <param name="value">
        /// 初始值
        /// </param>
        public Bit8(byte value)
        {
            p_value = value;
        }

        /// <summary>
        /// 初始化一个8比特位域结构
        /// </summary>
        /// <param name="bit1">索引0位域值</param>
        /// <param name="bit2">索引1位域值</param>
        /// <param name="bit3">索引2位域值</param>
        /// <param name="bit4">索引3位域值</param>
        /// <param name="bit5">索引4位域值</param>
        /// <param name="bit6">索引5位域值</param>
        /// <param name="bit7">索引6位域值</param>
        /// <param name="bit8">索引7位域值</param>
        public Bit8(bool bit1, bool bit2, bool bit3, bool bit4,
            bool bit5, bool bit6, bool bit7, bool bit8)
        {
            p_value = (byte)((bit1 ? 1 : 0));
            p_value |= (byte)((bit2 ? 1 : 0) << 1);
            p_value |= (byte)((bit3 ? 1 : 0) << 2);
            p_value |= (byte)((bit4 ? 1 : 0) << 3);
            p_value |= (byte)((bit5 ? 1 : 0) << 4);
            p_value |= (byte)((bit6 ? 1 : 0) << 5);
            p_value |= (byte)((bit7 ? 1 : 0) << 6);
            p_value |= (byte)((bit8 ? 1 : 0) << 7);
        }

        #endregion

        #region 参数

        internal byte p_value;

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
        public byte Value
        {
            get => p_value;
        }

        /// <summary>
        /// 设置位值并返回
        /// </summary>
        /// <param name="index">位域索引</param>
        /// <param name="value">位值</param>
        /// <returns>新的值</returns>
        public Bit8 SetBit(int index, bool value)
        {
            const byte b1 = 1;
            var re = p_value;

            if (value) re |= (byte)(p_value | (b1 << index));
            else re &= (byte)(~((uint)(p_value | (b1 << index))));

            return new Bit8(re);
        }

        #endregion

        #region 运算

        public static implicit operator Bit8(byte value)
        {
            return new Bit8(value);
        }

        public static explicit operator Bit8(int value)
        {
            return new Bit8((byte)value);
        }

        public static implicit operator byte(Bit8 value)
        {
            return value.p_value;
        }

        public static implicit operator int(Bit8 value)
        {
            return value.p_value;
        }

        public static bool operator ==(Bit8 b1, Bit8 b2)
        {
            return b1.p_value == b2.p_value;
        }

        public static bool operator !=(Bit8 b1, Bit8 b2)
        {
            return b1.p_value != b2.p_value;
        }

        public static bool operator <(Bit8 b1, Bit8 b2)
        {
            return b1.p_value < b2.p_value;
        }

        public static bool operator >(Bit8 b1, Bit8 b2)
        {
            return b1.p_value > b2.p_value;
        }

        public static bool operator <=(Bit8 b1, Bit8 b2)
        {
            return b1.p_value <= b2.p_value;
        }

        public static bool operator >=(Bit8 b1, Bit8 b2)
        {
            return b1.p_value >= b2.p_value;
        }

        public static Bit8 operator >>(Bit8 bit, int offset)
        {
            return new Bit8((byte)(bit.p_value >> offset));
        }

        public static Bit8 operator <<(Bit8 bit, int offset)
        {
            return new Bit8((byte)(bit.p_value << offset));
        }

        #endregion

        #region 派生

        public override int GetHashCode()
        {
            return p_value;
        }

        public unsafe override string ToString()
        {
            char* cp = stackalloc char[8];


            for (int i = 0; i < 8; i++)
            {
                cp[7 - i] = ((p_value >> i) & 1) == 1 ? '1' : '0';
            }

            return new string(cp, 0, 8);
        }

        public bool Equals(Bit8 other)
        {
            return p_value == other.p_value;
        }

        public int CompareTo(Bit8 other)
        {
            return p_value - other.p_value;
        }

        public override bool Equals(object obj)
        {
            if(obj is Bit8 b) return p_value == b.p_value;

            return false;
        }

        #endregion

        #endregion

    }

}
