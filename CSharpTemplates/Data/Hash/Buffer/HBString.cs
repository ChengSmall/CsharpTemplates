using System.Text;
using System;

using Cheng.Memorys;
using Cheng.IO;
using Cheng.Texts;

namespace Cheng.Algorithm.HashCodes
{

    /// <summary>
    /// 缓存哈希值的字符串
    /// </summary>
    public sealed class HBString : IEquatable<HBString>, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 实例化一个字符串
        /// </summary>
        /// <param name="value">字符串对象</param>
        public HBString(string value)
        {
            this.value = value ?? throw new ArgumentNullException();
            p_hash = 0;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 字符串值
        /// </summary>
        public readonly string value;

        private ulong p_hash;

        #endregion

        #region 转换

        public static implicit operator string(HBString str)
        {
            return str?.value;
        }

        public static implicit operator HBString(string str)
        {
            return (str is null) ? null : new HBString(str);
        }

        #endregion

        #region 派生

        /// <summary>
        /// 比较字符串相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(HBString a, HBString b)
        {
            return a?.value == b?.value;
        }

        /// <summary>
        /// 比较字符串不相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(HBString a, HBString b)
        {
            return a?.value != b?.value;
        }

        /// <summary>
        /// 返回字符串值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value;
        }

        public bool Equals(HBString other)
        {
            if (other is null) return false;
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if(obj is HBString other) return value == other.value;
            return value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode64().GetHashCode();
        }

        public long GetHashCode64()
        {
            if ((p_hash >> 63) == 0)
            {
                p_hash = (ulong)HashCode64.GetHashCode64(value);
                p_hash = p_hash | ((1UL << 63));
            }
            return (long)(p_hash & (~(1UL << 63)));
        }

        #endregion

    }




}