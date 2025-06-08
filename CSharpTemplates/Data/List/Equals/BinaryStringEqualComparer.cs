using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 字符串值比较器
    /// </summary>
    /// <remarks>
    /// 实现一个使用字符值比较的字符串比较器
    /// </remarks>
    public sealed unsafe class BinaryStringEqualComparer : EqualityComparer<string>
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
            return (obj?.GetHashCode()).GetValueOrDefault();
        }

        static BinaryStringEqualComparer binEqual = new BinaryStringEqualComparer();

        /// <summary>
        /// 获取一个全局唯一实例的字符串值比较器
        /// </summary>
        public static BinaryStringEqualComparer DefaultEqualComparer
        {
            get => binEqual;
        }

    }

}
