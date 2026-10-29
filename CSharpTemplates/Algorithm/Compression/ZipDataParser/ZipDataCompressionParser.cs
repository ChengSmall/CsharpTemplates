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
using Cheng.Algorithm.Compressions.ZipPackagings;

namespace Cheng.Algorithm.Compressions.ZipDataParser
{

    #region 结构

    /// <summary>
    /// zip压缩方法类型
    /// </summary>
    public enum ZipCompressionMethodType
    {

        /// <summary>
        /// 仅存储
        /// </summary>
        Stored = 0,

        /// <summary>
        /// Deflate压缩算法
        /// </summary>
        Deflate = 8,

        /// <summary>
        /// Deflate64压缩算法
        /// </summary>
        Deflate64 = 9
    }

    /// <summary>
    /// zip包项数据信息
    /// </summary>
    public sealed class ZipDataInformation : DataInformation
    {

        #region 构造

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
            p_parser = parser;
            p_dataPosition = -1;
            p_crc = -1;
        }

        #endregion

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

        internal long p_crc;

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

        /// <summary>
        /// 项数据使用的zip压缩方法代码
        /// </summary>
        public int CompressionMethod => p_compressionMethod;

#if DEBUG
        /// <summary>
        /// 项数据的zip通用标志位
        /// </summary>
#endif
        internal ushort GeneralPurposeBitFlag => p_generalPurposeBitFlag;

        /// <summary>
        /// 项数据使用的zip压缩方法类型
        /// </summary>
        public ZipCompressionMethodType CompressionMethodType
        {
            get => (ZipCompressionMethodType)p_compressionMethod;
        }

        /// <summary>
        /// 当前项中央目录的crc32校验值
        /// </summary>
        public uint CRCValue
        {
            get
            {
                return (uint)p_crc;
            }
        }

        #endregion

        #region 功能

        #region 封装

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

        #endregion

    }

    #endregion

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
        public ZipDataCompressionParser(Stream zipStream) : this(zipStream, true, null, null, null)
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
        public ZipDataCompressionParser(Stream zipStream, bool onDispose) : this(zipStream, onDispose, null, null, null)
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
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc) : this(zipStream, onDispose, createDictionaryFunc, null, null)
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
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding) : this(zipStream, onDispose, createDictionaryFunc, entryNameEncoding, null)
        {}

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <param name="createDictionaryFunc">用于创建索引字典的委托，null表示使用默认的<see cref="Dictionary{TKey, TValue}"/>字典对象</param>
        /// <param name="entryNameEncoding">读取和解析zip内的项名时使用的文本编码，null表示默认使用UTF-8编码</param>
        /// <param name="itemDecompress">zip项数据解压缩器，null表示使用默认实现的解压缩器，默认仅实现deflate和deflate64两种算法</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding, IZipItemDecompress itemDecompress) : this(zipStream, onDispose, createDictionaryFunc, entryNameEncoding, itemDecompress, false)
        {}

        /// <summary>
        /// 实例化zip包数据解析器
        /// </summary>
        /// <param name="zipStream">要解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放zip流数据对象</param>
        /// <param name="createDictionaryFunc">用于创建索引字典的委托，null表示使用默认的<see cref="Dictionary{TKey, TValue}"/>字典对象</param>
        /// <param name="entryNameEncoding">读取和解析zip内的项名时使用的文本编码，null表示默认使用UTF-8编码</param>
        /// <param name="itemDecompress">zip项数据解压缩器，null表示使用默认实现的解压缩器，默认仅实现deflate和deflate64两种算法</param>
        /// <param name="needCheckISO_IEC21320_1">在解析项路径时是否需要按 ISO/IEC 21320-1 标准判断通用目位标志的第11位来强制采用UTF-8编码；默认为false</param>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">数据不支持读取和查找</exception>
        /// <exception cref="IOException">数据访问IO错误</exception>
        /// <exception cref="NotImplementedException">无法解析zip包</exception>
        public ZipDataCompressionParser(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding, IZipItemDecompress itemDecompress, bool needCheckISO_IEC21320_1)
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
            if(itemDecompress is null)
            {
                itemDecompress = ZipItemDecompressDefault.CreateDef();
            }
            // 初始化zip解析器对象
            f_init(zipStream, onDispose, createDictionaryFunc, entryNameEncoding, itemDecompress, needCheckISO_IEC21320_1);
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
        private void f_init(Stream zipStream, bool onDispose, CreateDictionaryByCollection<string, ZipDataInformation> createDictionaryFunc, Encoding entryNameEncoding, IZipItemDecompress itemDec, bool isISO_IEC21320_1)
        {

            p_zipStream = zipStream;
            p_onDispose = onDispose;
            p_isISO_IEC21320_1 = isISO_IEC21320_1;
            // 使用忽略大小写的路径对比器初始化字典
            p_keyEquals = new EqualityStrNotPathSeparator(false, true);
            p_entryNameEncoding = entryNameEncoding;
            p_buffer = new byte[cp_bufferSize];
            p_headerBuffer = new byte[cp_headerBufferSize];

            p_zipList = new List<ZipDataInformation>();
            p_zipDict = null;
            p_itemDec = itemDec;
            try
            {
                // 读取zip目录结构
                f_parseZip();

                p_zipDict = createDictionaryFunc.Invoke(p_zipList, f_getDictionaryKey, p_keyEquals);
                if (p_zipDict is null)
                {
                    throw new ArgumentException();
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

#if DEBUG
        /// <summary>
        /// 是否需要判断ISO/IEC 21320-1标准的比特位强制规定
        /// </summary>
#endif
        private bool p_isISO_IEC21320_1;

        internal Stream p_zipStream;
        private EqualityStrNotPathSeparator p_keyEquals;
        private IZipItemDecompress p_itemDec;

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
            foreach (var item in list)
            {
                yield return item.DataPath;
            }
        }

        private static IEnumerable<string> f_enumerateFileName(List<ZipDataInformation> list)
        {
            foreach (var item in list)
            {
                yield return item.DataName;
            }
        }

        /// <summary>
        /// 无实现
        /// </summary>
        public override void Flush()
        {
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
        /// 使用索引判断指定项是否支持解压缩
        /// </summary>
        /// <param name="index">项数据索引</param>
        /// <returns>是否支持解压缩</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public bool CanDecompressStream(int index)
        {
            ThrowObjectDisposeException();
            var inf = p_zipList[index];
            return CanDecompressStreamByInformation(inf);
        }

        /// <summary>
        /// 使用路径判断指定项是否支持解压缩
        /// </summary>
        /// <param name="path">项数据路径</param>
        /// <returns>是否支持解压缩</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        /// <exception cref="Exception">其它错误</exception>
        public bool CanDecompressStream(string path)
        {
            ThrowObjectDisposeException();
            if (path is null) throw new ArgumentNullException();
            var inf = p_zipDict[path];
            return CanDecompressStreamByInformation(inf);
        }

        /// <summary>
        /// 打开指定索引下项的原始数据
        /// </summary>
        /// <param name="index">项数据索引</param>
        /// <returns>项存档的原始数据流，不再使用时释放</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="Exception">其它错误</exception>
        public Stream OpenRawStream(int index)
        {
            ThrowObjectDisposeException();
            var inf = p_zipList[index];
            return OpenRawStreamByInformation(inf);
        }

        /// <summary>
        /// 打开指定路径下项的原始数据
        /// </summary>
        /// <param name="path">项数据路径</param>
        /// <returns>项存档的原始数据流，不再使用时释放</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        /// <exception cref="Exception">其它错误</exception>
        public Stream OpenRawStream(string path)
        {
            ThrowObjectDisposeException();
            if (path is null) throw new ArgumentNullException();
            var inf = p_zipDict[path];
            return OpenRawStreamByInformation(inf);
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

        /// <summary>
        /// 使用项数据信息获取打开对应原始数据流对象
        /// </summary>
        /// <param name="zipInformation"></param>
        /// <returns>zip项原始数据，不再使用后释放</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数不是当前对象的项</exception>
        public Stream OpenRawStreamByInformation(ZipDataInformation zipInformation)
        {
            ThrowObjectDisposeException();
            if (zipInformation is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();

            return f_openOriEntry(zipInformation);
        }

        /// <summary>
        /// 判断指定项的数据是否在该解析器内支持解压缩
        /// </summary>
        /// <param name="zipInformation">要判断的项</param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数不是当前对象的项</exception>
        public bool CanDecompressStreamByInformation(ZipDataInformation zipInformation)
        {
            ThrowObjectDisposeException();
            if (zipInformation is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();
            return p_itemDec.CanDecompressType(zipInformation.p_compressionMethod);
        }

        /// <summary>
        /// 预热zip包数据内的项
        /// </summary>
        /// <param name="zipInformation">该zip内的项</param>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">项不属于当前zip数据内</exception>
        public void InitZipItem(ZipDataInformation zipInformation)
        {
            ThrowObjectDisposeException();
            if (zipInformation is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();
            f_ensureDataPosition(zipInformation);
        }

        private uint f_toCRC(Stream ori, Stream wr)
        {
            uint crc = ZipPackagingOperation.cp_crc32Begin;
            while (true)
            {
                var rc = ori.Read(p_buffer, 0, p_buffer.Length);
                if (rc == 0) break;
                wr?.Write(p_buffer, 0, rc);
                crc = ZipPackagingOperation.f_crc32Update(crc, p_buffer, 0, rc);
            }
            return ZipPackagingOperation.f_crc32End(crc);
        }

        /// <summary>
        /// 对指定项校验CRC值
        /// </summary>
        /// <remarks>可以与<see cref="ZipDataInformation.CRCValue"/>对比检测数据是否损坏</remarks>
        /// <param name="zipInformation">该zip内的项</param>
        /// <returns>该数据实际的CRC32值</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">项不属于当前zip数据内</exception>
        public uint InformationToCRC32(ZipDataInformation zipInformation)
        {
            ThrowObjectDisposeException();
            if (zipInformation is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();

            using (var open = f_openOriEntry(zipInformation))
            {
                return f_toCRC(open, null);
            }
        }

        /// <summary>
        /// 将指定项的数据解压缩到指定流并校验crc32验证码
        /// </summary>
        /// <param name="zipInformation">该zip内的项</param>
        /// <param name="stream">要写入的流数据</param>
        /// <returns>该数据实际的crc32值</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">项不属于当前zip数据内</exception>
        /// <exception cref="NotSupportedException">流对象没有写入权限</exception>
        public uint DeCompressionToCRC32ByItem(ZipDataInformation zipInformation, Stream stream)
        {
            ThrowObjectDisposeException();
            if (stream is null) throw new ArgumentNullException();
            if (((object)zipInformation.p_parser) != this) throw new ArgumentException();

            using (var open = f_openEntry(zipInformation))
            {
                return f_toCRC(open, stream);
            }
        }

        /// <summary>
        /// 将指定索引下的数据解压缩到指定流并校验crc32验证码
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="stream">要写入数据的流</param>
        /// <returns>此数据的crc32校验码</returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentNullException">对象是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        public uint DeCompressionToCRC32(int index, Stream stream)
        {
            ThrowObjectDisposeException();
            if (stream is null) throw new ArgumentNullException();

            var inf = p_zipList[index];
            using (var open = f_openEntry(inf))
            {
                return f_toCRC(open, stream);
            }
        }

        /// <summary>
        /// 将指定路径下的数据解压缩到指定流并校验crc32验证码
        /// </summary>
        /// <param name="dataPath">路径</param>
        /// <param name="stream">要写入数据的流</param>
        /// <returns>此数据的crc32校验码</returns>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="ArgumentNullException">对象是null</exception>
        /// <exception cref="NotSupportedException">流没有写入权限</exception>
        public uint DeCompressionToCRC32(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            if (dataPath is null || stream is null) throw new ArgumentNullException();
            if (!stream.CanWrite) throw new NotSupportedException();

            var inf = p_zipDict[dataPath];
            using (var open = f_openEntry(inf))
            {
                return f_toCRC(open, stream);
            }
        }

        #region CRC32

        internal static unsafe uint f_crc32Update(uint crc, byte* buffer, int count)
        {
            uint[] table = ZipPackagingOperation.cp_crc32Table;
            int end = count;
            for (int i = 0; i < count; i++)
            {
                crc = table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
            }
            return crc;
        }

        /// <summary>
        /// CRC32校验码初始值
        /// </summary>
        public static uint CRC32BeginValue => ZipPackagingOperation.cp_crc32Begin;

        /// <summary>
        /// 为crc32校验后续数据
        /// </summary>
        /// <param name="crc32">当前crc32值</param>
        /// <param name="buffer">读取需要校验的数据</param>
        /// <param name="offset">读取的数据起始偏移</param>
        /// <param name="count">要读取的数据字节数量</param>
        /// <returns>校验之后更新的crc32值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        ///  <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public static unsafe uint CRC32Update(uint crc32, byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || count < 0 || (offset + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            fixed (byte* buf = buffer)
            {
                return f_crc32Update(crc32, buf + offset, count);
            }
        }

        /// <summary>
        /// 为crc32校验后续数据
        /// </summary>
        /// <remarks>
        /// <para>想要校验数据的CRC32值，先从<see cref="CRC32BeginValue"/>获取初始值，然后对需要校验的数据按顺序调用<see cref="CRC32Update(uint, byte[], int, int)"/>更新，最后用<see cref="CRC32End(uint)"/>结束校验即可获取</para>
        /// </remarks>
        /// <param name="crc32">当前crc32值</param>
        /// <param name="buffer">读取需要校验数据的地址</param>
        /// <param name="count">要读取的数据字节数量</param>
        /// <returns>校验之后更新的crc32值</returns>
        /// <exception cref="ArgumentNullException">地址是null</exception>
        public static unsafe uint CRC32UpdateByAddress(uint crc32, byte* buffer, int count)
        {
            if (buffer == null) throw new ArgumentNullException();
            return f_crc32Update(crc32, buffer, count);
        }

        /// <summary>
        /// CRC32校验码结束符号
        /// </summary>
        /// <param name="crc32">校验后的值</param>
        /// <returns>校验完成的值</returns>
        public static uint CRC32End(uint crc32)
        {
            return ZipPackagingOperation.f_crc32End(crc32);
        }

        /// <summary>
        /// 获取指定流对象的CRC32值
        /// </summary>
        /// <param name="stream">要读取的流数据</param>
        /// <param name="buffer">需要用到的缓冲区</param>
        /// <returns>校验后的CRC32值</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">缓冲区长度是0</exception>
        /// <exception cref="ObjectDisposedException">流对象已释放</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static uint GetStreamCRC32(Stream stream, byte[] buffer)
        {
            if (stream is null || buffer is null) throw new ArgumentNullException();
            if (buffer.Length == 0) throw new ArgumentException();
            uint c = CRC32BeginValue;
            Begin:
            var rc = stream.Read(buffer, 0, buffer.Length);
            if (rc == 0) return ZipPackagingOperation.f_crc32End(c);
            c = CRC32Update(c, buffer, 0, rc);
            goto Begin;
        }

        #endregion

        #endregion

    }

}
