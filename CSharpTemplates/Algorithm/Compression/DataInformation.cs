using Cheng.Algorithm.Trees;
using Cheng.DataStructure;
using System;
using System.IO;

namespace Cheng.Algorithm.Compressions
{

    /// <summary>
    /// 压缩数据信息，派生该类以实现相应可访问的数据信息
    /// </summary>
    public abstract class DataInformation : IDataEntry, IEquatable<DataInformation>
    {

        #region 结构

        /// <summary>
        /// 空数据信息
        /// </summary>
        public sealed class EmptyInformation : DataInformation
        {
            public EmptyInformation()
            {
            }
        }

        #endregion

        #region 构造

        protected DataInformation() { }

        #endregion

        #region 访问

        /// <summary>
        /// 压缩数据所在路径，无法访问则返回null
        /// </summary>
        public virtual string DataPath => null;

        /// <summary>
        /// 数据的名称，无法访问则返回null
        /// </summary>
        /// <returns>数据名表示为被'\'分隔符分割的最后一块字符串</returns>
        public virtual string DataName => null;

        /// <summary>
        /// 数据压缩前大小，无法访问则返回-1
        /// </summary>
        public virtual long BeforeSize => -1;

        /// <summary>
        /// 数据压缩后大小，无法访问返回-1
        /// </summary>
        public virtual long CompressedSize => -1;

        /// <summary>
        /// 该数据的修改时间，无法访问返回null
        /// </summary>
        public virtual DateTime? ModifiedTime => null;

        string IDataEntry.FullName
        {
            get
            {
                var path = DataPath;
                if (path is null) throw new NotSupportedException();
                return path;
            }
        }

        string IDataEntry.Name
        {
            get
            {
                var path = DataName;
                if (path is null) throw new NotSupportedException();
                return path;
            }
        }

        /// <summary>
        /// 对比是否为同一个项数据
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(DataInformation other)
        {
            if (other is null) return false;
            return DataPath == other.DataPath;
        }

        public override bool Equals(object obj)
        {
            if(obj is DataInformation inf)
            {
                return Equals(inf);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (DataPath?.GetHashCode()).GetValueOrDefault();
        }

        public static bool operator ==(DataInformation a, DataInformation b)
        {
            if ((object)a == (object)b) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(DataInformation a, DataInformation b)
        {
            if ((object)a == (object)b) return false;
            if (a is null || b is null) return true;
            return !a.Equals(b);
        }

        #endregion

    }


}
