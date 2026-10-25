using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cheng.DataStructure;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Streams;
using Cheng.Streams;

namespace Cheng.Algorithm.Compressions.ZipDataParser
{

    /// <summary>
    /// zip包项数据信息
    /// </summary>
    public sealed class ZipDataInformation : DataInformation
    {

        internal ZipDataInformation(string dataPath, long beforeSize, long compressedSize,
            ushort compressionMethod, ushort generalPurposeBitFlag, long localHeaderOffset,
            DateTime? modifiedTime, EqualityStrNotPathSeparator keyComparer, ZipDataCompressionParser parser)
        {
            p_dataPath = dataPath;
            p_dataName = f_getDataName(dataPath);
            p_beforeSize = beforeSize;
            p_compressedSize = compressedSize;
            p_compressionMethod = compressionMethod;
            p_generalPurposeBitFlag = generalPurposeBitFlag;
            p_localHeaderOffset = localHeaderOffset;
            p_modifiedTime = modifiedTime;
            p_keyComparer = keyComparer;
            p_dataPosition = -1;
            p_parser = parser;
        }

        #region 参数
        internal ZipDataCompressionParser p_parser;

        internal readonly string p_dataPath;

        internal readonly string p_dataName;

        internal readonly long p_beforeSize;

        internal readonly long p_compressedSize;

        private readonly DateTime? p_modifiedTime;

        private readonly EqualityStrNotPathSeparator p_keyComparer;

#if DEBUG
        /// <summary>
        /// 项数据在zip包内的原始数据起始位置，-1表示还未计算
        /// </summary>
#endif
        internal long p_dataPosition;

#if DEBUG
        /// <summary>
        /// 项数据本地文件头在zip包内的位置
        /// </summary>
#endif
        internal long p_localHeaderOffset;

        internal readonly ushort p_compressionMethod;

        private readonly ushort p_generalPurposeBitFlag;

        #endregion

        #region 重写

        public override long BeforeSize => p_beforeSize;

        public override long CompressedSize => p_compressedSize;

        public override string DataName => p_dataName;

        public override string DataPath => p_dataPath;

        public override DateTime? ModifiedTime => p_modifiedTime;

        public override bool Equals(DataInformation other)
        {
            if (other is null) return false;
            if ((object)this == (object)other) return true;
            return p_keyComparer.Equals(p_dataPath, other.DataPath);
        }

        public override int GetHashCode()
        {
            return p_keyComparer.GetHashCode(p_dataPath);
        }

        #endregion

        #region 派生

        /// <summary>
        /// 该项数据是否存在压缩
        /// </summary>
        /// <returns>true表示属于已压缩数据，false表示属于仅存储数据</returns>
        public bool IsCompressed => p_compressionMethod != 0;

        /// <summary>
        /// 获取项信息所在的解析器
        /// </summary>
        public ZipDataCompressionParser ZipParser
        {
            get => p_parser;
        }

#if DEBUG
        /// <summary>
        /// 项数据使用的zip压缩方法代码
        /// </summary>
#endif
        internal ushort CompressionMethod => p_compressionMethod;

#if DEBUG
        /// <summary>
        /// 项数据的zip通用标志位
        /// </summary>
#endif
        internal ushort GeneralPurposeBitFlag => p_generalPurposeBitFlag;

        #endregion

        #region 功能

        private static string f_getDataName(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            int length = path.Length;
            int i = length - 1;
            char c = path[i];
            if (c == '/' || c == '\\') return string.Empty;

            for (; i >= 0; i--)
            {
                c = path[i];
                if (c == '/' || c == '\\')
                {
                    return path.Substring(i + 1);
                }
            }

            return path;
        }

        #endregion

    }

    /// <summary>
    /// zip包数据解析器
    /// </summary>
    public sealed partial class ZipDataCompressionParser : BaseCompressionParser
    {

        #region 构造函数

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream) : this(zipStream, true, null, null)
        {}

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream, bool onDispose) : this(zipStream, onDispose, null, null)
        {}

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <param name="createDictionaryFunc">用于创建索引字典的委托，null表示使用默认的<see cref="Dictionary{TKey, TValue}"/>字典对象</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc) : this(zipStream, onDispose, createDictionaryFunc, null)
        {}

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <param name="createDictionaryFunc">用于创建索引字典的委托，null表示使用默认的<see cref="Dictionary{TKey, TValue}"/>字典对象</param>
        /// <param name="entryNameEncoding">读取和解析zip内的项名时使用的文本编码，null表示默认使用UTF-8编码</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding)
        {
            if (zipStream is null) throw new ArgumentNullException();
            if ((!zipStream.CanRead) || (!zipStream.CanSeek)) throw new NotSupportedException();
            if (entryNameEncoding is null)
            {
                entryNameEncoding = new UTF8Encoding(true, true);
            }
            if (createDictionaryFunc is null)
            {
                createDictionaryFunc = DeleagateFunctions.CreateDictionaryByCollection;
            }
            // 初始化zip解析器对象
            f_init(zipStream, onDispose, createDictionaryFunc, entryNameEncoding);
        }

#if DEBUG
        /// <summary>
        /// 构造函数初始化时调用的函数，用来初始化和构建zip项目录
        /// </summary>
        /// <param name="zipStream">操作的zip数据对象</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <param name="createDictionaryFunc">用于创建索引字典的委托，此回调让调用者自定义字典对象类型</param>
        /// <param name="entryNameEncoding">读取和解析zip内的结构和项时可能会使用的文本编码</param>
#endif
        private void f_init(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding)
        {
            p_zipStream = zipStream;
            p_onDispose = onDispose;
            // 使用忽略大小写的路径对比器初始化字典
            p_keyEquals = new EqualityStrNotPathSeparator(false, true);
            p_entryNameEncoding = entryNameEncoding;
            p_buffer = new byte[cp_bufferSize];
            p_headerBuffer = new byte[cp_headerBufferSize];

            p_zipList = new List<ZipDataInformation>();
            p_zipDict = null;

            try
            {
                // 读取zip目录结构
                f_parseZip();

                p_zipDict = createDictionaryFunc.Invoke(p_zipList, f_getDictionaryKey, p_keyEquals);
                if (p_zipDict is null)
                {
                    throw new InvalidDataException(cp_exceptionCreateDictionary);
                }

                if (p_zipList.Count < (p_zipList.Capacity - 16))
                {
                    p_zipList.Capacity = p_zipList.Count;
                }
            }
            catch (Exception)
            {
                p_zipList = null;
                p_zipDict = null;
                throw;
            }
        }

        #endregion

        #region 参数

        private bool p_onDispose;

        internal Stream p_zipStream;
        private EqualityStrNotPathSeparator p_keyEquals;

#if DEBUG
        /// <summary>
        /// zip项集合，提供通过索引访问的条件，如果需要可以修改集合元素的派生类型
        /// </summary>
#endif
        private List<ZipDataInformation> p_zipList;

#if DEBUG
        /// <summary>
        /// zip项目录字典，如果需要可以修改value的派生类型
        /// </summary>
#endif
        private IReadOnlyDictionary<string, ZipDataInformation> p_zipDict;


#if DEBUG
        /// <summary>
        /// 解析项名称时使用的文本编码
        /// </summary>
#endif
        private Encoding p_entryNameEncoding;

#if DEBUG
        /// <summary>
        /// 解析目录和拷贝数据时使用的复用缓冲区
        /// </summary>
#endif
        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 读取固定长度文件头时使用的复用缓冲区
        /// </summary>
#endif
        private byte[] p_headerBuffer;

        #endregion

        #region 释放

        public override bool IsNeedToReleaseResources => true;

        protected override bool Disposeing(bool disposeing)
        {
            // 释放非托管和内部资源
            if (disposeing)
            {
                if (p_onDispose && (p_zipStream is object)) p_zipStream.Close();
            }
            p_zipStream = null;
            p_zipList = null;
            p_zipDict = null;
            p_buffer = null;
            p_headerBuffer = null;
            return true;
        }

        #endregion

        #region 重写

        public override bool CanDeCompression => true;
        public override bool CanDeCompressionByPath => true;
        public override bool CanDeCompressionByIndex => true;

        public override void DeCompressionTo(int index, Stream stream)
        {
            ThrowObjectDisposeException();
            if (stream is null) throw new ArgumentNullException();
            if (!stream.CanWrite) throw new NotSupportedException();

            var inf = p_zipList[index];
            using (var open = f_openEntry(inf))
            {
                open.CopyToStream(stream, p_buffer);
            }
        }
        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            if (dataPath is null || stream is null) throw new ArgumentNullException();
            if (!stream.CanWrite) throw new NotSupportedException();

            var inf = p_zipDict[dataPath];
            using (var open = f_openEntry(inf))
            {
                open.CopyToStream(stream, p_buffer);
            }
        }

        public override byte[] DeCompressionToData(int index)
        {
            ThrowObjectDisposeException();
            return f_deCompressionToData(p_zipList[index]);
        }
        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException();
            if (dataPath is null) throw new ArgumentNullException();
            return f_deCompressionToData(p_zipDict[dataPath]);
        }

        public override bool CanOpenCompressedStreamByPath => true;
        public override bool CanOpenCompressedStreamByIndex => true;

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException();
            if (dataPath is null) throw new ArgumentNullException();
            if (p_zipDict.TryGetValue(dataPath, out var inf))
            {
                return f_openEntry(inf);
            }
            return null;
        }

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException();
            return f_openEntry(p_zipList[index]);
        }

        public override bool CanProbePath => true;

        public override bool ContainsData(string dataPath)
        {
            ThrowObjectDisposeException();
            if (dataPath is null) throw new ArgumentNullException();
            return p_zipDict.ContainsKey(dataPath);
        }

        public override bool CanIndexOf => true;

        public override int Count
        {
            get
            {
                ThrowObjectDisposeException();
                return p_zipList.Count;
            }
        }

        public override DataInformation this[int index]
        {
            get
            {
                ThrowObjectDisposeException();
                return p_zipList[index];
            }
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                ThrowObjectDisposeException();
                if (dataPath is null) throw new ArgumentNullException();
                return p_zipDict[dataPath];
            }
        }

        public override bool CanGetParserData => true;
        public override Stream ParserData => p_zipStream;

        public override bool CanGetEnumerator => true;
        public override IEnumerator<DataInformation> GetEnumerator()
        {
            ThrowObjectDisposeException();
            return p_zipList.GetEnumerator();
        }

        public override bool CanGetEntryEnumrator => true;

        public override IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            ThrowObjectDisposeException();
            return p_zipList.GetEnumerator();
        }

        public override bool CanSortInformationIndex => true;

        public override void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {
            ThrowObjectDisposeException();
            p_zipList.Sort(comparer, index, count);
        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            ThrowObjectDisposeException();
            p_zipList.Sort(comparer, index, count);
        }

        public override bool CanGetPassword => false;

        public override IEnumerable<string> EnumatorFilePath()
        {
            ThrowObjectDisposeException();
            return f_enumerateFilePath(p_zipList);
        }

        public override IEnumerable<string> EnumatorFileName()
        {
            ThrowObjectDisposeException();
            return f_enumerateFileName(p_zipList);
        }

        private static IEnumerable<string> f_enumerateFilePath(List<ZipDataInformation> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                yield return list[i].DataPath;
            }
        }

        private static IEnumerable<string> f_enumerateFileName(List<ZipDataInformation> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                yield return list[i].DataName;
            }
        }

        public override void Flush()
        {
            ThrowObjectDisposeException();
            p_zipStream.Flush();
        }

        #endregion

        #region 派生功能

        /// <summary>
        /// 使用索引访问指定项所在的原始数据块位置
        /// </summary>
        /// <param name="index">项数据索引</param>
        /// <returns>
        /// 原始流数据块，该数据块是<paramref name="index"/>索引对应的项所在<see cref="ParserData"/>对象的原始数据位置，不返回自动解压的读取流
        /// </returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public StreamBlock GetDataBlock(int index)
        {
            ThrowObjectDisposeException();
            var inf = p_zipList[index];
            return f_getDataBlock(inf);
        }

        /// <summary>
        /// 使用路径访问指定项所在的原始数据块位置
        /// </summary>
        /// <param name="path">项数据路径</param>
        /// <returns>
        /// 原始流数据块，该数据块是<paramref name="path"/>对应的项所在<see cref="ParserData"/>对象的原始数据位置，不返回自动解压的读取流
        /// </returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        public StreamBlock GetDataBlock(string path)
        {
            ThrowObjectDisposeException();
            if (path is null) throw new ArgumentNullException();
            var inf = p_zipDict[path];
            return f_getDataBlock(inf);
        }

        /// <summary>
        /// 使用索引获取指定项信息
        /// </summary>
        /// <param name="index">项数据索引</param>
        /// <returns>索引所在的项信息</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public ZipDataInformation GetZipInformation(int index)
        {
            ThrowObjectDisposeException();
            return p_zipList[index];
        }

        /// <summary>
        /// 使用路径获取指定项信息
        /// </summary>
        /// <param name="path">项数据路径</param>
        /// <returns>路径所在的项信息</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        public ZipDataInformation GetZipInformation(string path)
        {
            ThrowObjectDisposeException();
            if (path is null) throw new ArgumentNullException();
            return p_zipDict[path];
        }

        /// <summary>
        /// 使用项数据信息获取对应数据
        /// </summary>
        /// <param name="zipInformation">项信息</param>
        /// <returns>zip项数据</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数不是当前对象的项</exception>
        /// <exception cref="Exception">其它错误</exception>
        public Stream OpenStreamByInformation(ZipDataInformation zipInformation)
        {
            ThrowObjectDisposeException();
            if (zipInformation is null) throw new ArgumentNullException();
            if(((object)zipInformation.p_parser) != this) throw new ArgumentException();

            return f_openEntry(zipInformation);
        }

        /// <summary>
        /// 使用项数据信息获取对应数据块
        /// </summary>
        /// <param name="zipInformation">项信息</param>
        /// <returns>zip项数据块</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数不是当前对象的项</exception>
        /// <exception cref="Exception">其它错误</exception>
        public StreamBlock GetBlockByInformation(ZipDataInformation zipInformation)
        {
            if (zipInformation is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();

            return f_getDataBlock(zipInformation);
        }

        #endregion

    }

}
