using System;
using System.Collections.Generic;
using System.IO;

using Cheng.Streams;
using Cheng.Memorys;
using System.Collections;
using System.Text;
using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Sorts.Comparers;
using Cheng.DataStructure.Collections;

namespace Cheng.Algorithm.Compressions
{

    /// <summary>
    /// 一个数据读写器基类
    /// </summary>
    /// <remarks>
    /// <para>派生该类以实现各种数据包或压缩算法的实现</para>
    /// <para>注重于读取和访问操作实现</para>
    /// </remarks>
    public abstract class BaseCompressionParser : SafreleaseUnmanagedResources, IDataList, DataStructure.Collections.IReadOnlyList<DataInformation>, System.Collections.Generic.IReadOnlyList<DataInformation>
    {

        #region 构造

        protected BaseCompressionParser()
        {
        }

        #endregion

        #region 释放

        /// <summary>
        /// 该压缩数据是否需要释放资源
        /// </summary>
        /// <returns>
        /// 返回true表示该对象不再使用时需要释放非托管资源，返回false表示不需要手动释放资源；默认返回false
        /// </returns>
        public virtual bool IsNeedToReleaseResources => false;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 当前压缩方法所正在解析的文件或数据块
        /// </summary>
        /// <exception cref="NotSupportedException">没有此权限</exception>
        public virtual Stream ParserData
        {
            get => throw new NotSupportedException();
        }

        #endregion

        #region 压缩功能

        #region 权限

        /// <summary>
        /// 能否获取内部解析的文件或数据块
        /// </summary>
        public virtual bool CanGetParserData => false;

        /// <summary>
        /// 该压缩算法是否支持查看当前已压缩的数据路径
        /// </summary>
        public virtual bool CanProbePath => false;

        /// <summary>
        /// 该压缩算法是否支持按索引查看当前已压缩的数据
        /// </summary>
        public virtual bool CanIndexOf => false;

        /// <summary>
        /// 该压缩算法是否支持添加数据内容
        /// </summary>
        public virtual bool CanAddData => false;

        /// <summary>
        /// 该压缩算法是否支持删除指定路径下的数据内容
        /// </summary>
        public virtual bool CanRemoveData => false;

        /// <summary>
        /// 该压缩算法是否支持设置已有路径下的数据内容
        /// </summary>
        public virtual bool CanSetData => false;

        /// <summary>
        /// 是否允许解压数据到指定流
        /// </summary>
        /// <returns><see cref="CanDeCompressionByIndex"/>和<see cref="CanDeCompressionByPath"/>任意一个为true则该参数为true</returns>
        public virtual bool CanDeCompression => CanDeCompressionByIndex || CanDeCompressionByPath;

        /// <summary>
        /// 是否允许使用索引解压数据到指定流
        /// </summary>
        public virtual bool CanDeCompressionByIndex => false;

        /// <summary>
        /// 是否允许使用路径解压数据到指定流
        /// </summary>
        public virtual bool CanDeCompressionByPath => false;

        /// <summary>
        /// 是否允许打开指定目录下的压缩流数据进行操作
        /// </summary>
        public virtual bool CanOpenCompressedStreamByPath => false;

        /// <summary>
        /// 是否允许打开指定索引下的压缩流数据进行读写操作
        /// </summary>
        public virtual bool CanOpenCompressedStreamByIndex => false;

        /// <summary>
        /// 是否允许创建一个新的路径
        /// </summary>
        public virtual bool CanCreatePath => false;

        /// <summary>
        /// 是否允许将数据信息索引重新排序
        /// </summary>
        public virtual bool CanSortInformationIndex => false;

        /// <summary>
        /// 是否具有将数据压缩的功能
        /// </summary>
        public virtual bool CanBeCompressed => false;

        /// <summary>
        /// 可获取项数据枚举器
        /// </summary>
        public virtual bool CanGetEntryEnumrator => false;

        /// <summary>
        /// 能使用<see cref="GetEnumerator"/>获取数据信息枚举器
        /// </summary>
        public virtual bool CanGetEnumerator => false;

        /// <summary>
        /// 是否允许获取密码
        /// </summary>
        public virtual bool CanGetPassword => false;

        /// <summary>
        /// 是否允许设置密码
        /// </summary>
        public virtual bool CanSetPassword => false;

        #endregion

        #region 操作

        #region 基础操作

        /// <summary>
        /// 将指定数据添加并压缩到压缩数据
        /// </summary>
        /// <param name="stream">待压缩的新数据</param>
        /// <param name="dataPath">
        /// 添加到的路径；格式和文件路径一致，使用以压缩文件为基础路径的相对路径
        /// </param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanAddData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void Add(Stream stream, string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定数据添加并压缩到压缩数据
        /// </summary>
        /// <param name="data">待压缩的新数据块</param>
        /// <param name="dataPath">添加到的路径；格式和文件路径一致，使用以压缩文件为基础路径的相对路径</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanAddData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void Add(byte[] data, string dataPath)
        {
            Add(new MemoryStream(data), dataPath);
        }

        /// <summary>
        /// 将指定路径下的数据移除
        /// </summary>
        /// <param name="dataPath">要移除的压缩数据所在的路径；格式和文件路径一致，使用以压缩文件为基础路径的相对路径</param>
        /// <returns>若数据成功被移除返回true，不存在指定路径返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanRemoveData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual bool Remove(string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定路径下的数据设置为新的数据
        /// </summary>
        /// <param name="stream">要设置的待压缩的数据</param>
        /// <param name="dataPath">要设置到的路径</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanSetData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SetData(Stream stream, string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定路径下的数据设置为新的数据
        /// </summary>
        /// <param name="data">要设置的待压缩的数据</param>
        /// <param name="dataPath">要设置到的路径；格式和文件路径一致，使用以压缩文件为基础路径的相对路径</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanSetData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SetData(byte[] data, string dataPath)
        {
            SetData(new MemoryStream(data), dataPath);
        }

        /// <summary>
        /// 将所有解压或压缩的缓存数据写入基础设备
        /// </summary>
        /// <exception cref="IOException">I/O错误</exception>
        /// <exception cref="ObjectDisposedException">基础流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void Flush()
        {
            this.ParserData.Flush();
        }

        /// <summary>
        /// 清空压缩数据内的所有内容
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="CanSetData"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定路径下的数据解压缩到指定流
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <param name="stream">要将数据解压缩到的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByPath"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionTo(string dataPath, Stream stream)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定路径下的数据解压缩并返回解压后数据
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <returns>解压缩后的数据内容</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompression"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual byte[] DeCompressionToData(string dataPath)
        {
            var inf = this[dataPath];
            var size = inf.BeforeSize;

            if (CanOpenCompressedStreamByPath && (size != -1 && size <= int.MaxValue))
            {
                using (var openStream = OpenCompressedStream(dataPath))
                {
                    if (openStream.CanRead)
                    {
                        byte[] bufs = new byte[size];
                        openStream.ReadBlock(bufs, 0, (int)size);
                        return bufs;
                    }
                }
            }

            MemoryStream ms = new MemoryStream((size > 0 && size < int.MaxValue) ? (int)size : 4096);
            DeCompressionTo(dataPath, ms);
            var msBuf = ms.GetBuffer();
            if (msBuf.Length == ms.Length)
            {
                return msBuf;
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 将指定索引下的数据解压缩并返回解压后的数据
        /// </summary>
        /// <param name="index">数据所在索引</param>
        /// <param name="stream">要将数据解压缩到的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanIndexOf"/>或<see cref="CanDeCompressionByIndex"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionTo(int index, Stream stream)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 将指定索引下的数据解压缩并返回解压后的数据
        /// </summary>
        /// <param name="index">数据所在索引</param>
        /// <returns>解压缩后的数据内容</returns>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException"><see cref="CanIndexOf"/>或<see cref="CanDeCompression"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual byte[] DeCompressionToData(int index)
        {
            var inf = this[index];
            var size = inf.BeforeSize;

            if (CanOpenCompressedStreamByIndex && (size != -1 && size <= int.MaxValue))
            {
                using (var openStream = OpenCompressedStream(index))
                {
                    if (openStream.CanRead)
                    {
                        byte[] bufs = new byte[size];
                        openStream.ReadBlock(bufs, 0, (int)size);
                        return bufs;
                    }
                }
            }
           
            MemoryStream ms = new MemoryStream((size > 0 && size < int.MaxValue) ? (int)size : 4096);
            DeCompressionTo(index, ms);

            var msBuf = ms.GetBuffer();
            if(msBuf.Length == ms.Length)
            {
                return msBuf;
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 打开指定路径下的压缩流数据以进行读写操作
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <returns>
        /// <para>一个路径<paramref name="dataPath"/>下的压缩数据包，读写权限取决于<see cref="Stream"/>内的属性参数；如果指定路径不存在则返回null</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanOpenCompressedStreamByPath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual Stream OpenCompressedStream(string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 打开指定索引下的压缩流数据以进行读写操作
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <returns>
        /// <para>一个索引<paramref name="index"/>下的压缩数据包，读写权限取决于<see cref="Stream"/>内的属性参数</para>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanOpenCompressedStreamByPath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual Stream OpenCompressedStream(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 创建一个新的数据路径
        /// </summary>
        /// <param name="dataPath">新的数据路径</param>
        /// <returns>
        /// 是否创建成功；创建成功返回true，如果已存在相同的路径则返回false
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanCreatePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual bool CreatePath(string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 创建或打开指定路径下的压缩数据
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <returns>
        /// <para>一个路径<paramref name="dataPath"/>下的压缩数据包，读写权限取决于<see cref="Stream"/>内的属性参数</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanCreatePath"/>或<see cref="CanOpenCompressedStreamByPath"/>为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual Stream CreateOrOpenStream(string dataPath)
        {
            CreatePath(dataPath);
            return OpenCompressedStream(dataPath);
        }

        #endregion

        #region 查询

        /// <summary>
        /// 返回指定路径下的数据信息
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <returns>数据信息结构</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="KeyNotFoundException">访问的路径不存在</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual DataInformation this[string dataPath]
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 返回指定路径下的数据信息
        /// </summary>
        /// <typeparam name="T">指定信息数据的类型</typeparam>
        /// <param name="dataPath">数据路径</param>
        /// <returns>数据信息结构，若类型不匹配则返回null</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="KeyNotFoundException">访问的路径不存在</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual T GetInformation<T>(string dataPath) where T : DataInformation
        {
            return this[dataPath] as T;
        }

        /// <summary>
        /// 按索引查找压缩数据信息
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <returns>数据信息</returns>
        /// <exception cref="NotSupportedException"><see cref="CanIndexOf"/>属性为false</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual DataInformation this[int index]
        {
            get => throw new NotSupportedException();
        }

        /// <summary>
        /// 按索引查找压缩数据信息
        /// </summary>
        /// <typeparam name="T">指定数据信息的类型</typeparam>
        /// <param name="index">数据索引</param>
        /// <returns>数据信息，若类型有错误，返回null</returns>
        /// <exception cref="NotSupportedException"><see cref="CanIndexOf"/>属性为false</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual T GetInformation<T>(int index) where T : DataInformation
        {
            return this[index] as T;
        }

        /// <summary>
        /// 按路径索引查找数据信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataPath">数据所在路径</param>
        /// <param name="information">获取的数据信息，如果类型不匹配则为null</param>
        /// <returns>如果查找到了指定路径下的数据返回true，没有找到路径返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool TryGetInformation<T>(string dataPath, out T information) where T : DataInformation
        {
            bool b = this.ContainsData(dataPath);
            if (b)
            {
                information = this[dataPath] as T;
                return true;
            }
            information = null;
            return false;
        }

        /// <summary>
        /// 当前压缩数据内拥有的路径数量
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="CanIndexOf"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual int Count => throw new NotSupportedException();

        /// <summary>
        /// 访问当前压缩数据下所有数据的路径
        /// </summary>
        /// <returns>当前压缩数据下所有数据的路径访问枚举器</returns>
        /// <exception cref="NotSupportedException"><see cref="CanProbePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual IEnumerable<string> EnumatorFilePath()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 访问当前压缩数据下所有数据的路径
        /// </summary>
        /// <returns>当前压缩数据下所有数据的路径</returns>
        /// <exception cref="NotSupportedException"><see cref="CanProbePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual string[] GetAllFilePath()
        {
            return System.Linq.Enumerable.ToArray(EnumatorFilePath());
        }

        /// <summary>
        /// 访问当前压缩数据下所有数据的文件名称
        /// </summary>
        /// <returns>当前压缩数据下所有数据的名称访问枚举器</returns>
        /// <exception cref="NotSupportedException"><see cref="CanProbePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual IEnumerable<string> EnumatorFileName()
        {

            var enumator = EnumatorFilePath();

            using (var enr = enumator.GetEnumerator())
            {
                while (enr.MoveNext())
                {
                    yield return Path.GetFileName(enr.Current);
                }
            }

        }

        /// <summary>
        /// 访问当前压缩数据下所有数据的文件名称
        /// </summary>
        /// <returns>当前压缩数据下所有数据的名称访问枚举器</returns>
        /// <exception cref="NotSupportedException"><see cref="CanProbePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        public virtual string[] GetAllFileName()
        {
            return System.Linq.Enumerable.ToArray(EnumatorFileName());
        }

        #endregion

        #region 判断

        /// <summary>
        /// 判断是否存在指定路径
        /// </summary>
        /// <param name="dataPath">存放数据的路径；格式和文件路径一致，使用以压缩文件为基础路径的相对路径</param>
        /// <returns>
        /// 返回true表示存在<paramref name="dataPath"/>路径，false则不存在
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanProbePath"/>属性为false</exception>
        /// <exception cref="ObjectDisposedException">基础流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual bool ContainsData(string dataPath)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region 集合操作

        /// <summary>
        /// 重新排序数据当前数据信息的index索引
        /// </summary>
        /// <param name="comparer">排序规则</param>
        /// <param name="index">要排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentException">索引错误</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">没有此功能的权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SortInformationIndex(IComparer comparer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 重新排序数据当前数据信息的index索引
        /// </summary>
        /// <param name="comparer">排序规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">没有此功能的权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SortInformationIndex(IComparer comparer)
        {
            SortInformationIndex(comparer, 0, Count);
        }

        /// <summary>
        /// 重新排序数据当前数据信息的index索引
        /// </summary>
        /// <param name="comparer">排序规则</param>
        /// <param name="index">要排序的起始索引</param>
        /// <param name="count">要排序的元素数量</param>
        /// <exception cref="ArgumentException">索引错误</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">没有此功能的权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {
            var cp = new GenericComparer<DataInformation>(comparer);
            SortInformationIndex(cp, 0, Count);
        }

        /// <summary>
        /// 重新排序数据当前数据信息的index索引
        /// </summary>
        /// <param name="comparer">排序规则</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">没有此功能的权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void SortInformationIndex(IComparer<DataInformation> comparer)
        {
            var cp = new GenericComparer<DataInformation>(comparer);
            SortInformationIndex(cp);
        }

        #endregion

        #region 文本读取

        /// <summary>
        /// 将指定路径下的数据解压并写入到指定文本
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <param name="writer">要写入的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByPath"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            char[] buffer;
            ThrowObjectDisposeException();
            if (dataPath is null || writer is null || encoding is null) throw new ArgumentNullException();

            int r;
            if (CanOpenCompressedStreamByPath)
            {
                using(var open = OpenCompressedStream(dataPath))
                {
                    if(open is null)
                    {
                        throw new ArgumentException();
                    }

                    buffer = new char[1024];
                    using (StreamReader sr = new StreamReader(open, encoding, false, 1024))
                    {
                        while (true)
                        {
                            r = sr.Read(buffer, 0, buffer.Length);
                            if (r == 0) break;
                            writer.Write(buffer, 0, r);
                        }
                    }
                }

                return;
            }
            if (CanDeCompressionByPath)
            {
                MemoryStream ms = new MemoryStream(256);
                DeCompressionTo(dataPath, ms);
                ms.Seek(0, SeekOrigin.Begin);

                StreamReader sr = new StreamReader(ms, encoding, false, 256);
                buffer = new char[1024];
                while (true)
                {
                    r = sr.Read(buffer, 0, 1024);
                    if (r == 0) break;
                    writer.Write(buffer, 0, r);
                }
                sr.Close();
                return;
            }

            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        /// <summary>
        /// 将指定索引下的数据解压并写入到指定文本
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <param name="writer">要写入的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByIndex"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            if (writer is null || encoding is null) throw new ArgumentNullException();
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException();

            char[] buffer;
            int r;
            if (CanOpenCompressedStreamByIndex)
            {
                using (var open = OpenCompressedStream(index))
                {
                    StreamReader sr = new StreamReader(open, encoding, false, 1024);
                    buffer = new char[1024];
                    while (true)
                    {
                        r = sr.Read(buffer, 0, 1024);
                        if (r == 0) break;
                        writer.Write(buffer, 0, r);
                    }
                }

                return;
            }
            if (CanDeCompressionByIndex)
            {
                MemoryStream ms = new MemoryStream(256);
                DeCompressionTo(index, ms);
                ms.Seek(0, SeekOrigin.Begin);

                StreamReader sr = new StreamReader(ms, encoding, false, 256);
                buffer = new char[1024];
                while (true)
                {
                    r = sr.Read(buffer, 0, 1024);
                    if (r == 0) break;
                    writer.Write(buffer, 0, r);
                }
                sr.Close();
                return;
            }

            throw new NotSupportedException(Cheng.Properties.Resources.Exception_NotSupportedText);
        }

        /// <summary>
        /// 将指定路径下的数据解压并写入到指定文本
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <param name="append">要写入的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByPath"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionToText(string dataPath, Encoding encoding, StringBuilder append)
        {
            StringWriter swr = new StringWriter(append);
            DeCompressionToText(dataPath, encoding, swr);
            swr.Flush();
        }

        /// <summary>
        /// 将指定索引下的数据解压并写入到指定文本
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <param name="append">要写入的字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByIndex"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual void DeCompressionToText(int index, Encoding encoding, StringBuilder append)
        {
            StringWriter swr = new StringWriter(append);
            DeCompressionToText(index, encoding, swr);
            swr.Flush();
        }

        /// <summary>
        /// 将指定路径下的数据解压并返回为字符串
        /// </summary>
        /// <param name="dataPath">数据路径</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <returns>指定数据的字符串格式</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByPath"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual string DeCompressionToText(string dataPath, Encoding encoding)
        {
            StringWriter swr = new StringWriter();
            DeCompressionToText(dataPath, encoding, swr);
            return swr.ToString();
        }

        /// <summary>
        /// 将指定路径下的数据解压并返回为字符串
        /// </summary>
        /// <param name="index">数据索引</param>
        /// <param name="encoding">解析数据要使用的字符编码</param>
        /// <returns>指定数据的字符串格式</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="NotSupportedException"><see cref="CanDeCompressionByIndex"/>属性为false或流数据未拥有指定权限</exception>
        /// <exception cref="ObjectDisposedException">基础流已关闭</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual string DeCompressionToText(int index, Encoding encoding)
        {
            StringWriter swr = new StringWriter();
            DeCompressionToText(index, encoding, swr);
            return swr.ToString();
        }

        #endregion

        #region 加密

        /// <summary>
        /// 访问数据时需要的密码字符串
        /// </summary>
        /// <value>在访问或设置数据时需要的密码，用于加密数据；仅当<see cref="CanGetPassword"/>或<see cref="CanSetPassword"/>为true时，对应属性功能才会生效</value>
        /// <exception cref="NotSupportedException">没有此功能</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public virtual string Password
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        #endregion

        #region 派生

        IDataEntry IDataList.this[int index]
        {
            get => this[index];
        }

        /// <summary>
        /// 获取一个枚举器，可枚举当前所有项数据路径
        /// </summary>
        /// <returns>一个像数据路径接口的枚举器</returns>
        public virtual IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            return GetEnumerator();
        }

        IEnumerator<IDataEntry> IEnumerable<IDataEntry>.GetEnumerator()
        {
            return GetDataEntryEnumrator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetDataEntryEnumrator();
        }

        /// <summary>
        /// 返回一个能够访问所有数据信息的枚举器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">没有权限</exception>
        public virtual IEnumerator<DataInformation> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #endregion

        #endregion

    }

}
