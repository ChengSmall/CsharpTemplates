using System;
using System.Collections;
using System.Collections.Generic;

using Cheng.Algorithm.HashCodes;
using Cheng.DataStructure;

namespace Cheng.Algorithm.Pathfindings.NotePathfindings
{

    /// <summary>
    /// 使用路径节点唯一ID进行比较和排序的实现
    /// </summary>
    public sealed class NodePathEqualComparer : EqualityComparerHash64<INodePath>, IComparer<INodePath>
    {

        /// <summary>
        /// 实例化比较器
        /// </summary>
        public NodePathEqualComparer()
        {
        }

        public override bool Equals(INodePath x, INodePath y)
        {
            if (x == y) return true;
            if (x is null || y is null) return false;
            return x.ID == y.ID;
        }

        public override int GetHashCode(INodePath obj)
        {
            if (obj is null) throw new ArgumentNullException();
            return obj.ID.GetHashCode();
        }

        public override long GetHashCode64(INodePath value)
        {
            if (value is null) throw new ArgumentNullException();
            return value.ID.GetHashCode64();
        }

        public int Compare(INodePath x, INodePath y)
        {
            if (x == y) return 0;
            if(x is null)
            {
                return y is null ? 0 : -1;
            }
            else if(y is null)
            {
                return x is null ? 0 : 1;
            }
            return string.CompareOrdinal(x.ID, y.ID);
        }

    }

}