using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cheng.DataStructure.Hashs;
using Cheng.Algorithm.HashCodes;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 字符串值比较器
    /// </summary>
    /// <remarks>
    /// 实现一个使用字符值比较的字符串比较器，内部实现使用字符串的 == 运算符重载
    /// </remarks>
    public sealed unsafe class BinaryStringEqualComparer : EqualityComparer<string>, IEqualityComparerHash64<string>, IComparer<string>
    {

        /// <summary>
        /// 实例化二进制码位字符串比较器
        /// </summary>
        public BinaryStringEqualComparer()
        {
        }

        public override bool Equals(string x, string y)
        {
            return x == y;
        }

        public override int GetHashCode(string obj)
        {
            if (obj is null) throw new ArgumentNullException();
            return obj.GetHashCode();
        }

        public long GetHashCode64(string value)
        {
            if (value is null) throw new ArgumentNullException();
            return value.GetHashCode64();
        }

        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }

        static readonly BinaryStringEqualComparer binEqual = new BinaryStringEqualComparer();

        /// <summary>
        /// 获取一个全局唯一实例的字符串值比较器
        /// </summary>
        public static new BinaryStringEqualComparer Default
        {
            get => binEqual;
        }

        /// <summary>
        /// 比较两个指针指向同样长度的字符串并返回比较的值
        /// </summary>
        /// <param name="x">字符串1</param>
        /// <param name="y">字符串2</param>
        /// <param name="length">两个字符串的字符数量</param>
        /// <returns>依次比较字符值的比较结果</returns>
        public static int CompareFixedBuffer(char* x, char* y, int length)
        {
            for (int i = 0; i < length; i++)
            {
                var re = x[i] - y[i];
                if (re != 0) return re;
            }
            return 0;
        }

    }

}
