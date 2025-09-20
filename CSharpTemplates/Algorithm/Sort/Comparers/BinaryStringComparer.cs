using System.Collections.Generic;
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
            return string.CompareOrdinal(x, y);
        }

        #region
        ///// <summary>
        ///// 使用值依次比较两个字符串的大小
        ///// </summary>
        ///// <param name="x">前一个字符串</param>
        ///// <param name="y">后一个字符串</param>
        ///// <returns>
        ///// <para>从前到后遍历字符串的每一个字符比较字符的值</para>
        ///// <para>返回值小于0表示<paramref name="x"/>小于<paramref name="y"/>; 返回大于0表示<paramref name="x"/>大于<paramref name="y"/>; 返回等于0表示<paramref name="x"/>和<paramref name="y"/>相等</para>
        ///// </returns>
        //public static int ComparerFunc(string x, string y)
        //{
        //    if ((object)x == (object)y)
        //    {
        //        return 0;
        //    }
        //    if (x is null || y is null)
        //    {
        //        return x is null ? -1 : 1;
        //    }

        //    int xlen = x.Length;
        //    int ylen = y.Length;

        //    fixed (char* xp = x, yp = y)
        //    {
        //        int length = Math.Min(xlen, ylen);

        //        for (int i = 0; i < length; i++)
        //        {
        //            if (xp[i] != yp[i])
        //            {
        //                //不相等字符
        //                return xp[i] < yp[i] ? -1 : 1;
        //            }
        //        }

        //        //全相等
        //        if (xlen == ylen)
        //        {
        //            return 0;
        //        }
        //        return (xlen < ylen) ? -1 : 1;
        //    }
        //}
        #endregion

        #region 单例

        static readonly BinaryStringComparer cp_defComp = new BinaryStringComparer();

        /// <summary>
        /// 获取字符串值比较器的唯一实例
        /// </summary>
        public static new BinaryStringComparer Default
        {
            get
            {
                return cp_defComp;
            }
        }

        #endregion

    }

}
