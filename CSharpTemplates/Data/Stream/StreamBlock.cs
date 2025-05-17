using Cheng.Algorithm.HashCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.DataStructure.Streams
{

    /// <summary>
    /// 表示流数据块位置
    /// </summary>
    public struct StreamBlock : IEquatable<StreamBlock>, IHashCode64
    {

        #region 构造

        /// <summary>
        /// 初始化流数据块位置结构
        /// </summary>
        /// <param name="position">起始位</param>
        /// <param name="length">数据长度</param>
        public StreamBlock(long position, long length)
        {
            this.position = position;
            this.length = length;
        }

        #endregion

        #region 字段

        /// <summary>
        /// 数据起始位置
        /// </summary>
        public readonly long position;

        /// <summary>
        /// 数据长度
        /// </summary>
        public readonly long length;

        #endregion

        #region 功能

        /// <summary>
        /// 比较相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator ==(StreamBlock s1, StreamBlock s2)
        {
            return s1.position == s2.position && s1.length == s2.length;
        }

        /// <summary>
        /// 比较不相等
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool operator !=(StreamBlock s1, StreamBlock s2)
        {
            return s1.position != s2.position || s1.length != s2.length;
        }

        public override bool Equals(object obj)
        {
            if (obj is StreamBlock s) return this == s;
            return false;
        }

        public bool Equals(StreamBlock other)
        {
            return this.position == other.position && this.length == other.length;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ length.GetHashCode();
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            sb.Append(nameof(position));
            sb.Append(':');
            sb.Append(position);
            sb.Append(' ');
            sb.Append(nameof(length));
            sb.Append(':');
            sb.Append(length);
            return sb.ToString();
        }

        public long GetHashCode64()
        {
            return position ^ length;
        }

        #endregion

    }


}
