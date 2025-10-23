using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Cheng.Streams;
using Cheng.Memorys;
using Cheng.Algorithm.Sorts.Comparers;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure;
using Cheng.DataStructure.Hashs;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure.Streams;
using Cheng.IO;

using ReadDict = System.Collections.Generic.IReadOnlyDictionary<string, Cheng.Algorithm.Compressions.CSPACK.CSPInformation>;
using System.Linq;
using Cheng.Algorithm.Trees;
using Cheng.Texts;

namespace Cheng.Algorithm.Compressions.CSPACK
{

    /// <summary>
    /// 用于创建只读字典的函数委托
    /// </summary>
    /// <param name="collection">要创建字典的数据集合</param>
    /// <param name="dictToKeyFunc">通过集合元素创建对应字典的key</param>
    /// <param name="keyEqualityComparer">指定字典的key比较器和哈希获取值函数</param>
    /// <returns>从<paramref name="collection"/>创建的只读字典</returns>
    public delegate ReadDict CreateDictionaryFunc(IEnumerable<CSPInformation> collection, Func<CSPInformation, string> dictToKeyFunc, IEqualityComparer<string> keyEqualityComparer);

    /// <summary>
    /// CSPACK包项数据信息
    /// </summary>
    public sealed class CSPInformation : DataInformation
    {

        internal CSPInformation(string path, StreamBlock block)
        {
            this.path = path;
            p_block = block;

            var re = path.LastIndexOf('\\');

            if(re == -1)
            {
               re = path.LastIndexOf('/');
            }

            if(re == -1)
            {
                name = path;
            }
            else
            {
                if(re == path.Length - 1)
                {
                    name = string.Empty;
                }
                else
                {
                    name = path.Substring(re + 1);
                }
            }

        }

        internal readonly string path;
        internal readonly string name;
        internal readonly StreamBlock p_block;

        /// <summary>
        /// 当前项数据的所在位置
        /// </summary>
        public StreamBlock DataBlock => p_block;

        public override long BeforeSize
        {
            get => p_block.length;
        }

        public override long CompressedSize
        {
            get => p_block.length;
        }

        public override string DataPath => path;

        public override string DataName => name;

        public override DateTime? ModifiedTime => null;

        public override bool Equals(DataInformation other)
        {
            if(other is CSPInformation ci)
            {
                return EqualityStrNotPathSeparator.EqualPath(path, ci.path, false, true);
            }
            return false;
        }

    }

    /// <summary>
    /// CSPACK数据包读取器
    /// </summary>
    public unsafe class CStreamPackReader : BaseCompressionParser
    {

        #region 初始化

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream) : this(stream, true, true, 1024 * 8)
        {
        }

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream) : this(stream, disposeBaseStream, true, 1024 * 8)
        {
        }

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader) : this(stream, disposeBaseStream, checkHeader, 1024 * 8)
        {
        }

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek))
            {
                throw new NotSupportedException();
            }
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            if (checkHeader)
            {
                if (!CheckHeaderNextLoop(stream))
                {
                    throw new NotImplementedException();
                }
            }

            p_disposeBase = disposeBaseStream;
            p_buffer = new byte[bufferSize];

            var col = f_readData(stream, new char[255], bufferSize >= 8 ? p_buffer : new byte[8]);
            p_dict = col.ToDictionary(f_getKey, new EqualityStrNotPathSeparator(false, true));

            //p_list = col.ToList();
            p_list = new List<CSPInformation>(p_dict.Count);
            foreach (var pair in p_dict)
            {
                p_list.Add(pair.Value);
            }
            if (this.p_list.Capacity > this.p_list.Count) this.p_list.Capacity = p_list.Count;
            p_stream = stream;
        }

        private CStreamPackReader()
        {
        }

        static string f_getKey(CSPInformation inf)
        {
            return inf.path;
        }

        #endregion

        #region 参数

        private Stream p_stream;

        internal ReadDict p_dict;

        internal List<CSPInformation> p_list;

        private byte[] p_buffer;

        private bool p_disposeBase;

        #endregion

        #region 功能

        #region 文件头

        /// <summary>
        /// 读取并检查头数据是否匹配包数据头格式
        /// </summary>
        /// <param name="stream">要读取的流</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="Exception">其它可能的IO错误</exception>
        public static bool CheckHeader(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            long buf;
            byte* bptr = (byte*)&buf;
            for (int i = 0; i < 6; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) return false;
                bptr[i] = (byte)re;
            }
            return CheckHeaderByAddress(bptr);
        }

        /// <summary>
        /// 读取并检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的字节缓冲区</param>
        /// <param name="offset">从此处偏移开始读取，向后读取<see cref="HeaderDataSize"/>个字节</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定缓冲区范围少于6个字节</exception>
        public static bool CheckHeader(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();

            if (offset < 0 || offset + 6 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* bp = buffer)
            {
                return CheckHeaderByAddress(bp);
            }
        }

        /// <summary>
        /// 读取并检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的内存首地址，可用数据不少于6字节</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        public static bool CheckHeaderByAddress(byte* buffer)
        {
            //0x43, 0x53, 0x50, 0x41, 0x43, 0x4B
            return buffer[0] == 0x43 &&
                   buffer[1] == 0x53 &&
                   buffer[2] == 0x50 &&
                   buffer[3] == 0x41 &&
                   buffer[4] == 0x43 &&
                   buffer[5] == 0x4B;
        }

        /// <summary>
        /// 包数据头的字节数
        /// </summary>
        public const int HeaderDataSize = 6;

        /// <summary>
        /// 包数据头的ASCII文本形式字符串
        /// </summary>
        public const string HeaderString = "CSPACK";

        /// <summary>
        /// 从流对象持续读取数据并检查到符合包数据头的格式
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <returns>
        /// <para>如果成功找到符合数据头的数据，返回true，并此时流的位置处于CSPACK数据头后的一位；如果一直到末尾都没有找到则返回false</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public static bool CheckHeaderNextLoop(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            long Int64Buf;
            byte* bufptr = (byte*)&Int64Buf;
            int re;
            int i;

            for (i = 0; i < 6; i++)
            {
                re = stream.ReadByte();
                if (re == -1) return false;
                bufptr[i] = (byte)re;
            }

            Loop:
            if (CheckHeaderByAddress(bufptr))
            {
                //找到
                return true;
            }

            //读取
            re = stream.ReadByte();
            if (re == -1) return false;

            //集体左移
            for (i = 0; i < 5; i++)
            {
                bufptr[i] = bufptr[i + 1];
            }
            bufptr[5] = (byte)re;
            goto Loop;

        }

        #endregion

        #region 封装

        #if DEBUG
        /// <summary>
        /// 读取数据并使用枚举器返回数据索引
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cbuf255"></param>
        /// <param name="buf8"></param>
        /// <returns></returns>
        #endif
        internal static IEnumerable<CSPInformation> f_readData(Stream stream, char[] cbuf255, byte[] buf8)
        {
            int i;
            int re;
            int keyLen;

            Loop:
            //读取头字节以及路径字符数值
            keyLen = stream.ReadByte();
            if (keyLen < 0 || keyLen > byte.MaxValue)
            {
                //路径字符数超出预期
                throw new InvalidOperationException();
            }

            if(keyLen == 0)
            {
                //末尾
                yield break;
            }

            //读取路径
            for (i = 0; i < keyLen; i++)
            {
                re = stream.ReadByte();
                if(re == -1)
                {
                    throw new InvalidOperationException(); //预期之外的结构
                }
                ushort cuv = (byte)re;
                re = stream.ReadByte();
                if (re == -1)
                {
                    throw new InvalidOperationException(); //预期之外的结构
                }
                cuv |= (byte)(re << 8);
                cbuf255[i] = (char)cuv;
            }

            string path = new string(cbuf255, 0, keyLen);

            //读取长度
            re = stream.ReadBlock(buf8, 0, 8);
            if(re != 8)
            {
                throw new InvalidOperationException(); //预期之外的结构
            }
            long dataLen = buf8.OrderToInt64();
            CSPInformation inf = new CSPInformation(path, new StreamBlock(stream.Position, dataLen));
            //跳转
            stream.Seek(dataLen, SeekOrigin.Current);
            yield return inf;
            //yield return new KeyValuePair<string, CSPInformation>(path, inf);

            goto Loop;

        }

        #endregion

        #region 释放

        public override bool IsNeedToReleaseResources => true;

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing && p_disposeBase)
            {
                p_stream.Close();
            }
            p_stream = null;
            p_dict = null;
            p_list = null;
            return true;
        }

        #endregion

        #region 派生

        public override bool CanBeCompressed => false;

        public override bool CanDeCompression => true;

        public override bool CanDeCompressionByIndex => true;

        public override void DeCompressionTo(int index, Stream stream)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            var inf = p_list[index];
            lock (p_stream)
            {
                if (p_stream.Position != inf.p_block.position)
                {
                    p_stream.Seek(inf.p_block.position, SeekOrigin.Begin);
                }
                p_stream.CopyToStream(stream, p_buffer, (ulong)inf.p_block.length);
            }
        }

        public override bool CanDeCompressionByPath => true;

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            var inf = p_dict[dataPath];
            lock (p_stream)
            {
                if (p_stream.Position != inf.p_block.position)
                {
                    p_stream.Seek(inf.p_block.position, SeekOrigin.Begin);
                }
                p_stream.CopyToStream(stream, p_buffer, (ulong)inf.p_block.length);
            }
        }

        public override bool CanIndexOf => true;

        public override int Count => p_list.Count;

        public override DataInformation this[int index]
        {
            get
            {
                ThrowObjectDisposeException(nameof(CStreamPackReader));
                return p_list[index];
            }
        }

        public override bool CanProbePath => true;

        public override bool ContainsData(string dataPath)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_dict.ContainsKey(dataPath);
        }

        public override bool CanGetParserData => true;

        public override Stream ParserData => p_stream;

        /// <summary>
        /// 创建一个截取与基础封装流上的可读取和查找一部分数据的流对象
        /// </summary>
        /// <param name="block">创建的流需要读取的部分</param>
        /// <returns>一个封装于基础流的流对象，用于读取和查找指定部分数据的流，并且关闭流对象后不会影响基础流的操作</returns>
        protected virtual Stream CreatePartialStreamByBaseData(in StreamBlock block)
        {
            return new TruncateStream(p_stream, block.position, block.length, false);
        }

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanOpenCompressedStreamByPath => true;

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            var inf = p_list[index];
            return CreatePartialStreamByBaseData(in inf.p_block);
        }

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            if(!p_dict.TryGetValue(dataPath, out var inf))
            {
                return null;
            }
            return CreatePartialStreamByBaseData(in inf.p_block);
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                ThrowObjectDisposeException(nameof(CStreamPackReader));
                return p_dict[dataPath];
            }
        }

        public override bool TryGetInformation<T>(string dataPath, out T information)
        {
            information = default;
            CSPInformation inf;
            if(!p_dict.TryGetValue(dataPath, out inf))
            {
                return false;
            }
            information = inf as T;
            return true;
        }

        public override byte[] DeCompressionToData(int index)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            var inf = p_list[index];
            byte[] buf = new byte[inf.p_block.length];
            lock (p_stream)
            {
                if (p_stream.Position != inf.p_block.position)
                {
                    p_stream.Seek(inf.p_block.position, SeekOrigin.Begin);
                }
                p_stream.ReadBlock(buf, 0, buf.Length);
            }
            return buf;
        }

        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            var inf = p_dict[dataPath];
            byte[] buf = new byte[inf.p_block.length];
            lock (p_stream)
            {
                if (p_stream.Position != inf.p_block.position)
                {
                    p_stream.Seek(inf.p_block.position, SeekOrigin.Begin);
                }
                p_stream.ReadBlock(buf, 0, buf.Length);
            }
            return buf;
        }

        public override bool CanGetEnumerator => true;

        public override IEnumerator<DataInformation> GetEnumerator()
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_list.GetEnumerator();
        }

        public override bool CanGetEntryEnumrator => true;

        public override IEnumerable<string> EnumatorFilePath()
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_list.ToOtherItems<CSPInformation, string>(toFilePath);
        }

        public override IEnumerable<string> EnumatorFileName()
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_list.ToOtherItems<CSPInformation, string>(toFileName);
        }

        string toFilePath(CSPInformation inf)
        {
            return inf.path;
        }

        string toFileName(CSPInformation inf)
        {
            return inf.name;
        }

        public override IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            return this.GetEnumerator();
        }

        public override bool CanSortInformationIndex => true;

        public override void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {
            p_list.Sort(index, count, comparer);
        }

        public override void SortInformationIndex(IComparer<DataInformation> comparer)
        {
            p_list.Sort(comparer);
        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            p_list.Sort(index, count, comparer);
        }

        public override void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            if (writer is null || encoding is null) throw new ArgumentNullException();
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException();

            using (var open = OpenCompressedStream(index))
            {
                using (StreamReader sr = new StreamReader(open, encoding, false, 1024))
                {
                    sr.CopyToText(writer, new char[1024]);
                }
            }
        }

        public override void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            if (writer is null || encoding is null) throw new ArgumentNullException();
            using (var open = OpenCompressedStream(dataPath))
            {
                using (StreamReader sr = new StreamReader(open, encoding, false, 1024))
                {
                    sr.CopyToText(writer, new char[1024]);
                }
            }
        }

        #endregion

        #region 扩展

        /// <summary>
        /// 初始化csp包读取器，自定义字典创建方案
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="createDictionaryFunc">用于创建字典的函数</param>
        /// <returns>
        /// <para>完成索引初始化的csp包读取器</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">存在参数是null</exception>
        /// <exception cref="ArgumentException">字典创建函数返回的对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public static CStreamPackReader CreateReaderByCustomDictionary(Stream stream, CreateDictionaryFunc createDictionaryFunc)
        {
            return CreateReaderByCustomDictionary(stream, createDictionaryFunc, true, true, 1024 * 8);
        }

        /// <summary>
        /// 初始化csp包读取器，自定义字典创建方案
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="createDictionaryFunc">用于创建字典的函数</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <returns>
        /// <para>完成索引初始化的csp包读取器</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">存在参数是null</exception>
        /// <exception cref="ArgumentException">字典创建函数返回的对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public static CStreamPackReader CreateReaderByCustomDictionary(Stream stream, CreateDictionaryFunc createDictionaryFunc, bool disposeBaseStream)
        {
            return CreateReaderByCustomDictionary(stream, createDictionaryFunc, disposeBaseStream, true, 1024 * 8);
        }

        /// <summary>
        /// 初始化csp包读取器，自定义字典创建方案
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="createDictionaryFunc">用于创建字典的函数</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <returns>
        /// <para>完成索引初始化的csp包读取器</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">存在参数是null</exception>
        /// <exception cref="ArgumentException">字典创建函数返回的对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public static CStreamPackReader CreateReaderByCustomDictionary(Stream stream, CreateDictionaryFunc createDictionaryFunc, bool disposeBaseStream, bool checkHeader)
        {
            return CreateReaderByCustomDictionary(stream, createDictionaryFunc, disposeBaseStream, checkHeader, 1024 * 8);
        }

        /// <summary>
        /// 初始化csp包读取器，自定义字典创建方案
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="createDictionaryFunc">用于创建字典的函数</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <returns>
        /// <para>完成索引初始化的csp包读取器</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">存在参数是null</exception>
        /// <exception cref="ArgumentException">字典创建函数返回的对象是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public static CStreamPackReader CreateReaderByCustomDictionary(Stream stream, CreateDictionaryFunc createDictionaryFunc, bool disposeBaseStream, bool checkHeader, int bufferSize)
        {
            if (stream is null || createDictionaryFunc is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek))
            {
                throw new NotSupportedException();
            }
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            if (checkHeader)
            {
                if (!CheckHeaderNextLoop(stream))
                {
                    throw new NotImplementedException();
                }
            }
            CStreamPackReader @this = new CStreamPackReader();
            @this.p_disposeBase = disposeBaseStream;
            @this.p_buffer = new byte[bufferSize];

            var col = f_readData(stream, new char[255], bufferSize >= 8 ? @this.p_buffer : new byte[8]);
            @this.p_dict = createDictionaryFunc.Invoke(col, f_getKey, new EqualityStrNotPathSeparator(false, true));

            if (@this.p_dict is null) throw new ArgumentException();
            //@this.p_list = col.ToList();
            @this.p_list = new List<CSPInformation>(@this.p_dict.Count);
            foreach (var pair in @this.p_dict)
            {
                @this.p_list.Add(pair.Value);
            }
            @this.p_stream = stream;

            return @this;
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif