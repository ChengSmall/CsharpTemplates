using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Cheng.DataStructure.Collections
{

    /// <summary>
    /// 忽略路径分隔符的字符比较器
    /// </summary>
    /// <remarks>
    /// <para>在比较两个字符时，如果两个字符都是'/'或'\'其中一个字符，则判定为相等</para>
    /// </remarks>
    public sealed class EqualityNotPathSeparator : EqualityComparer<char>
    {

        /// <summary>
        /// 忽略路径分隔符的字符比较器
        /// </summary>
        public EqualityNotPathSeparator()
        {
        }

        public override bool Equals(char x, char y)
        {
            if (x == y) return true;
            if ((x == '/' || x == '\\') && (y == '/' || y == '\\'))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode(char obj)
        {
            if(obj == '/')
            {
                return '\\'.GetHashCode();
            }
            return obj.GetHashCode();
        }
    }
}
