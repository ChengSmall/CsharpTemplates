using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;
using Cheng.Algorithm.HashCodes;
using Cheng.Streams;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 依次比较字符数组中字符的相等比较器
    /// </summary>
    public sealed class CharArrayEqualityComparer : EqualityComparerHash64<char[]>
    {
        /// <summary>
        /// 实例化依次比较字符数组中字符的相等比较器，使用默认字符比较
        /// </summary>
        public CharArrayEqualityComparer()
        {
            p_charComparer = EqualityComparer<char>.Default;
        }

        /// <summary>
        /// 实例化依次比较字符数组中字符的相等比较器
        /// </summary>
        /// <param name="charComparer">比较字符是的比较器，null表示使用默认比较器</param>
        public CharArrayEqualityComparer(IEqualityComparer<char> charComparer)
        {
            p_charComparer = charComparer ?? EqualityComparer<char>.Default;
        }

        private IEqualityComparer<char> p_charComparer;

        public sealed override bool Equals(char[] x, char[] y)
        {
            if (x == (object)y) return true;

            if (x is null || y is null) return false;

            var len = x.Length;
            if (len != y.Length) return false;

            for (int i = 0; i < len; i++)
            {
                if (!p_charComparer.Equals(x[i], y[i])) return false;
            }

            return true;
        }

        public sealed override int GetHashCode(char[] obj)
        {
            return GetHashCode64(obj).GetHashCode();
        }

        public unsafe sealed override long GetHashCode64(char[] value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            int length = value.Length;
            if (length == 0) return 0;

            const ulong FNV_OFFSET_BASIS = 14695981039346656037UL;
            const ulong FNV_PRIME = 1099511628211UL;

            fixed (char* cp = value)
            {
                ulong hash = FNV_OFFSET_BASIS;
                for (int i = 0; i < length; i++)
                {
                    //var c = cp[i];
                    hash = (hash ^ cp[i]) * FNV_PRIME;
                }
                return (long)hash;
            }

        }

    }

}
