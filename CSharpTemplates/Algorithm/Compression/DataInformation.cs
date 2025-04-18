using Cheng.Algorithm.Trees;
using System;

namespace Cheng.Algorithm.Compressions
{

    /// <summary>
    /// 压缩数据信息，派生该类以实现相应可访问的数据信息
    /// </summary>
    public abstract class DataInformation : IDataEntry
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

        #endregion

    }

}
