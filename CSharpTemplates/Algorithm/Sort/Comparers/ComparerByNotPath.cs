using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 忽略路径分隔符的字符比较器
    /// </summary>
    /// <remarks>
    /// <para>在比较两个字符时，如果两个字符都是'/'或'\'任意一个字符，则判定为相等</para>
    /// </remarks>
    public sealed class ComparerByNotPathSeparator : Comparer<char>
    {

        #region

        /// <summary>
        /// 忽略路径分隔符的字符比较器
        /// </summary>
        public ComparerByNotPathSeparator()
        {
        }

        public override int Compare(char x, char y)
        {
            var re = x - y;
            if ((re != 0) && ((x == '/' || x == '\\') && (y == '/' || y == '\\')))
            {
                return 0;
            }
            return re;
        }

        #endregion

    }




}
