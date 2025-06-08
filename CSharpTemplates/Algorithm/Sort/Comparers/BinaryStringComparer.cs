using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 使用二进制字符值比较的字符串比较器
    /// </summary>
    public sealed unsafe class BinaryStringComparer : Comparer<string>
    {
        /// <summary>
        /// 实例化字符串二进制字符值比较器
        /// </summary>
        public BinaryStringComparer()
        {
        }

        public override int Compare(string x, string y)
        {
            if((object)x == (object)y)
            {
                return 0;
            }
            if(x is null || y is null)
            {
                return x is null ? -1 : 1;
            }

            int xlen = x.Length;
            int ylen = y.Length;

            fixed (char* xp = x, yp = y)
            {

                int length = Math.Min(xlen, ylen);

                for (int i = 0; i < length; i++)
                {
                    if (xp[i] != yp[i])
                    {
                        //不相等字符
                        return xp[i] < yp[i] ? -1 : 1;
                    }
                }

                //全相等
                if(xlen == ylen)
                {
                    return 0;
                }

                return (xlen < ylen) ? -1 : 1;
            }

        }

    }

}
