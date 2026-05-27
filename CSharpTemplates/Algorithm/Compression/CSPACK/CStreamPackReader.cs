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
    /// CSPACK包类型
    /// </summary>
    public enum CSPackEncType : byte
    {
        /// <summary>
        /// 非有效类型
        /// </summary>
        None = 0,

        /// <summary>
        /// 默认utf-16类型
        /// </summary>
        CSPack = 1,

        /// <summary>
        /// utf-8类型
        /// </summary>
        U8CSPack = 2
    }

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
                return p_block == ci.p_block;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return p_block.GetHashCode();
        }

    }

    /// <summary>
    /// CSPACK数据包读取器
    /// </summary>
    public unsafe class CStreamPackReader : BaseCompressionParser, IEnumerable<CSPInformation>
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
        {}

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
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize) : this(stream, disposeBaseStream, checkHeader, bufferSize, f_defCreateDict, CSPackEncType.CSPack)
        {}

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <param name="createDictionary">用于创建字典的函数，null表示使用默认实现的方法创建字典</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize, CreateDictionaryByCollection<string, CSPInformation> createDictionary)
            : this(stream, disposeBaseStream, checkHeader, bufferSize, createDictionary, CSPackEncType.CSPack){}

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <param name="createDictionary">用于创建字典的函数，null表示使用默认实现的方法创建字典</param>
        /// <param name="type">以指定的类型读取包数据；当<paramref name="checkHeader"/>参数是true时，该参数无效，读取器将搜索头数据判断包类型</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0；<paramref name="type"/>参数错误</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize, CreateDictionaryByCollection<string, CSPInformation> createDictionary, CSPackEncType type)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek))
            {
                throw new NotSupportedException();
            }
            
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            if (createDictionary is null) createDictionary = Enumerable.ToDictionary;
            if (checkHeader)
            {
                type = CheckHeaderTypeNextLoop(stream);
                if (type == CSPackEncType.None)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (type <= 0 || type > CSPackEncType.U8CSPack) throw new ArgumentOutOfRangeException();
            }
            f_init(stream, disposeBaseStream, bufferSize, null, createDictionary, type);
        }

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <param name="createDictionary">用于创建字典的函数，null表示使用默认实现的方法创建字典</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize, CreateDictionaryByPairs<string, CSPInformation> createDictionary)
             : this(stream, disposeBaseStream, checkHeader, bufferSize, createDictionary, CSPackEncType.CSPack){}

        /// <summary>
        /// 初始化csp包读取器
        /// </summary>
        /// <param name="stream">要从中解析的包数据流</param>
        /// <param name="disposeBaseStream">在释放时是否释放封装的基础流，默认为true</param>
        /// <param name="checkHeader">初始化索引前是否逐步搜寻csp包数据头，默认为true</param>
        /// <param name="bufferSize">读取或拷贝时所用的缓冲区大小，默认为8192</param>
        /// <param name="createDictionary">用于创建字典的函数，null表示使用默认实现的方法创建字典</param>
        /// <param name="type">以指定的类型读取包数据；当<paramref name="checkHeader"/>参数是true时，该参数无效，读取器将搜索头数据判断包类型</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小没有大于0；<paramref name="type"/>参数错误</exception>
        /// <exception cref="NotSupportedException">流没有读取和查找权限</exception>
        /// <exception cref="NotImplementedException">数据不是严格的csp包，无法解析</exception>
        /// <exception cref="Exception">操作流时产生的其它错误</exception>
        public CStreamPackReader(Stream stream, bool disposeBaseStream, bool checkHeader, int bufferSize, CreateDictionaryByPairs<string, CSPInformation> createDictionary, CSPackEncType type)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek))
            {
                throw new NotSupportedException();
            }
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            if (createDictionary is null) createDictionary = f_defCreateDict;
            if (checkHeader)
            {
                type = CheckHeaderTypeNextLoop(stream);
                if (type == CSPackEncType.None)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                if (type <= 0 || type > CSPackEncType.U8CSPack) throw new ArgumentOutOfRangeException();
            }
            f_init(stream, disposeBaseStream, bufferSize, createDictionary, null, type);
        }

        private void f_init(Stream stream, bool disposeBaseStream, int bufferSize, CreateDictionaryByPairs<string, CSPInformation> createDictionaryPair, CreateDictionaryByCollection<string, CSPInformation> createDictionaryCol, CSPackEncType type)
        {
            p_encType = type;
            //首个数据位置
            p_headerNextDataPos = stream.Position;
            p_canWriter = stream.CanWrite;
            this.p_disposeBase = disposeBaseStream;
            this.p_buffer = new byte[bufferSize];

            if (createDictionaryCol is null)
            {
                var col = f_readDataPair(stream, new char[255], p_encType, Encoding.UTF8);

                this.p_dict = createDictionaryPair.Invoke(col, new EqualityStrNotPathSeparator(false, true));
            }
            else
            {
                var col = f_readDataPair(stream, new char[255], p_encType, Encoding.UTF8);
                this.p_dict = createDictionaryCol.Invoke(col.ToOtherItems(f_toColEn), f_getKey, new EqualityStrNotPathSeparator(false, true));
            }
            p_endDataPos = stream.Position - 1;

            if (this.p_dict is null) throw new ArgumentException();
            //@this.p_list = col.ToList();
            this.p_list = new List<CSPInformation>(this.p_dict.Count);
            foreach (var pair in this.p_dict)
            {
                this.p_list.Add(pair.Value);
            }
            this.p_stream = stream;

            p_dictIsWriter = p_dict is IDictionary<string, Cheng.Algorithm.Compressions.CSPACK.CSPInformation> wrDict;

            //return @this;
        }

        static string f_getKey(CSPInformation inf)
        {
            return inf.path;
        }

        static ReadDict f_defCreateDict(IEnumerable<KeyValuePair<string, CSPInformation>> pair, IEqualityComparer<string> eq)
        {
            if(pair is IDictionary<string, CSPInformation> pd)
            {
                return new Dictionary<string, CSPInformation>(pd, eq);
            }
            int cap;
            if(pair is ICollection<CSPInformation>)
            {
                cap = ((ICollection<CSPInformation>)pair).Count;
            }
            else if (pair is ICollection)
            {
                cap = ((ICollection)pair).Count;
            }
            else
            {
                cap = 4;
            }

            var dict = new Dictionary<string, CSPInformation>(cap, eq);
            foreach (var item in pair)
            {
                dict[item.Key] = item.Value;
            }
            return dict;
        }

        private CStreamPackReader()
        {
        }

        #endregion

        #region 参数

        private Stream p_stream;

        internal ReadDict p_dict;

        internal List<CSPInformation> p_list;

        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 首个数据的第一个字节或终止符
        /// </summary>
#endif
        private long p_headerNextDataPos;

#if DEBUG
        /// <summary>
        /// 最后一个数据的终止符位置
        /// </summary>
#endif
        private long p_endDataPos;

        private bool p_canWriter;

#if DEBUG
        /// <summary>
        /// 字典是一个可写的
        /// </summary>
#endif
        private bool p_dictIsWriter;

        private CSPackEncType p_encType;

        private bool p_disposeBase;

        #endregion

        #region 功能

        #region 文件头

        /// <summary>
        /// 读取并检查头数据是否匹配包数据头格式
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
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
            int i;
            for (i = 0; i < 6; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) return false;
                bptr[i] = (byte)re;
            }
            if(CheckHeaderTypeByAddress(bptr, 6) != CSPackEncType.None) return true;
            for (; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) break;
                bptr[i] = (byte)re;
            }
            return CheckHeaderByAddress(bptr, i);
        }

        /// <summary>
        /// 检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的字节缓冲区</param>
        /// <param name="offset">从此处偏移开始读取，向后读取<see cref="HeaderDataSize"/>个字节</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">指定缓冲区范围少于6个字节</exception>
        public static bool CheckHeader(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset + 8 > buffer.Length) throw new ArgumentOutOfRangeException();

            fixed (byte* bp = buffer)
            {
                return CheckHeaderByAddress(bp);
            }
        }

        /// <summary>
        /// 检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的内存首地址，可用数据不少于8字节</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        public static bool CheckHeaderByAddress(byte* buffer)
        {
            return CheckHeaderByAddress(buffer, 8);
        }

        /// <summary>
        /// 检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的内存首地址</param>
        /// <param name="length">可用数据的字节数量</param>
        /// <returns>匹配头数据返回true，否则返回false</returns>
        public static bool CheckHeaderByAddress(byte* buffer, int length)
        {
            return CheckHeaderTypeByAddress(buffer, length) != CSPackEncType.None;
        }

        /// <summary>
        /// 包数据头的字节数
        /// </summary>
        public const int HeaderDataSize = 6;

        /// <summary>
        /// 包数据头的ASCLL文本形式字符串
        /// </summary>
        public const string HeaderString = "CSPACK";

        /// <summary>
        /// U8类型包数据头字节数
        /// </summary>
        public const int U8HeaderDataSize = 8;

        /// <summary>
        /// U8类型包头的ASCLL文本数据
        /// </summary>
        public const string U8HeaderString = "CSPACK";

        /// <summary>
        /// 从流对象持续读取数据并检查到符合包数据头的格式
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <returns>
        /// <para>如果成功找到符合数据头的数据，返回true，并此时流的位置处于CSPACK或U8CSPACK数据头后的一位；如果一直到末尾都没有找到则返回false</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public static bool CheckHeaderNextLoop(Stream stream)
        {
            return CheckHeaderTypeNextLoop(stream) != CSPackEncType.None;
        }

        /// <summary>
        /// 检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的字节缓冲区</param>
        /// <param name="length">可用数据的字节数量</param>
        /// <returns>匹配数据的类型</returns>
        public static CSPackEncType CheckHeaderTypeByAddress(byte* buffer, int length)
        {
            //0x43, 0x53, 0x50, 0x41, 0x43, 0x4B "CSPACK"
            //0x55, 0x38, 0x43, 0x53, 0x50, 0x41, 0x43, 0x4B "U8CSPACK"
            if (length < 6) return CSPackEncType.None;
            if (buffer[0] == 0x43)
            {
                return (buffer[1] == 0x53 &&
                   buffer[2] == 0x50 &&
                   buffer[3] == 0x41 &&
                   buffer[4] == 0x43 &&
                   buffer[5] == 0x4B) ? CSPackEncType.CSPack : CSPackEncType.None;
            }
            if (length < 8) return CSPackEncType.None;
            if (buffer[0] == 0x55)
            {
                return (buffer[0] == 0x55 &&
                   buffer[1] == 0x38 &&
                   buffer[2] == 0x43 &&
                   buffer[3] == 0x53 &&
                   buffer[4] == 0x50 &&
                   buffer[5] == 0x41 &&
                   buffer[6] == 0x43 &&
                   buffer[7] == 0x4B) ? CSPackEncType.U8CSPack : CSPackEncType.None;
            }
            return CSPackEncType.None;
        }

        /// <summary>
        /// 检查数据是否匹配包数据头格式
        /// </summary>
        /// <param name="buffer">要读取其中数据的字节缓冲区</param>
        /// <param name="offset">要从中读取数据的起始位置</param>
        /// <returns>匹配数据的类型</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        public static CSPackEncType CheckHeaderType(byte[] buffer, int offset)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException();
            if (offset < 6) return CSPackEncType.None;
            fixed (byte* bp = buffer)
            {
                return CheckHeaderTypeByAddress(bp, buffer.Length - offset);
            }
        }

        /// <summary>
        /// 读取并检查头数据是否匹配包数据头格式
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <returns>匹配数据的类型</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="Exception">其它可能的IO错误</exception>
        public static CSPackEncType CheckHeaderType(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            long buf;
            byte* bptr = (byte*)&buf;
            int i;
            for (i = 0; i < 6; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) break;
                bptr[i] = (byte)re;
            }
            var t = CheckHeaderTypeByAddress(bptr, 6);
            if (t != CSPackEncType.None) return t;
            for (; i < 8; i++)
            {
                var re = stream.ReadByte();
                if (re == -1) break;
                bptr[i] = (byte)re;
            }
            return CheckHeaderTypeByAddress(bptr, i);
        }

        /// <summary>
        /// 从流对象持续读取数据并检查到符合包数据头的格式
        /// </summary>
        /// <param name="stream">要读取的流对象</param>
        /// <returns>
        /// <para>如果成功找到符合数据头的数据，返回对应数据类型，并此时流的位置处于CSPACK或U8CSPACK数据头后的一位；如果一直到末尾都没有找到则返回<see cref="CSPackEncType.None"/></para>
        /// </returns>
        public static CSPackEncType CheckHeaderTypeNextLoop(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            byte* bufptr = stackalloc byte[8];
            int re;
            int i;

            for (i = 0; i < 6; i++)
            {
                re = stream.ReadByte();
                if (re == -1)
                {
                    return CSPackEncType.None;
                }
                bufptr[i] = (byte)re;
            }

            var cst = CheckHeaderTypeByAddress(bufptr, 6);
            if (cst != CSPackEncType.None) return cst;
            for (; i < 8; i++)
            {
                re = stream.ReadByte();
                if (re == -1)
                {
                    return CheckHeaderTypeByAddress(bufptr, i);
                }
                bufptr[i] = (byte)re;
            }

            Loop:
            var type = CheckHeaderTypeByAddress(bufptr, 8);
            if (type != CSPackEncType.None)
            {
                //找到
                return type;
            }

            //读取
            re = stream.ReadByte();
            if (re == -1) return CSPackEncType.None;

            //集体左移
            for (i = 0; i < 7; i++)
            {
                bufptr[i] = bufptr[i + 1];
            }
            bufptr[7] = (byte)re;
            goto Loop;

        }

        #endregion

        #region 封装

        static bool f_readInt64(Stream stream, out long reValue)
        {
            reValue = default;
            long buf8;
            var re = stream.ReadBlock((byte*)&buf8, 8);
            if (re != 8) return false;
            reValue = IOoperations.OrderToInt64(new IntPtr(&buf8));
            return true;
        }

        #if DEBUG
        #endif
        internal static IEnumerable<CSPInformation> f_readData(Stream stream, char[] cbuf255)
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
                ushort cuv = (byte)(re & 0xFF);
                re = stream.ReadByte();
                if (re == -1)
                {
                    throw new InvalidOperationException(); //预期之外的结构
                }
                cuv |= (ushort)((uint)re << 8);
                cbuf255[i] = (char)cuv;
            }

            string path = new string(cbuf255, 0, keyLen);

            long dataLen;

            //读取长度
            if (!f_readInt64(stream, out dataLen))
            {
                throw new InvalidOperationException(); //预期之外的结构
            }

            CSPInformation inf = new CSPInformation(path, new StreamBlock(stream.Position, dataLen));
            //跳转
            stream.Seek(dataLen, SeekOrigin.Current);
            yield return inf;
            //yield return new KeyValuePair<string, CSPInformation>(path, inf);
            goto Loop;
        }

#if DEBUG
        /// <summary>
        /// 读取数据并使用枚举器返回数据索引
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cbuf255"></param>
        /// <returns></returns>
#endif
        internal static IEnumerable<KeyValuePair<string, CSPInformation>> f_readDataPair(Stream stream, char[] cbuf255, CSPackEncType type, Encoding utf8)
        {
            int i;
            int re;
            int keyLen;
            string path;

            Loop:
            //读取头字节
            keyLen = stream.ReadByte();
            if(keyLen == -1)
            {
                //末尾
                yield break;
            }

            if(type == CSPackEncType.CSPack)
            {
                if (keyLen == 0)
                {
                    //末尾
                    yield break;
                }

                //读取路径
                for (i = 0; i < keyLen; i++)
                {
                    re = stream.ReadByte();
                    if (re == -1)
                    {
                        throw new InvalidOperationException(); //预期之外的结构
                    }
                    ushort cuv = (byte)(re & 0xFF);
                    re = stream.ReadByte();
                    if (re == -1)
                    {
                        throw new InvalidOperationException(); //预期之外的结构
                    }
                    cuv |= (ushort)((uint)re << 8);
                    cbuf255[i] = (char)cuv;
                }
                path = new string(cbuf255, 0, keyLen);
            }
            else
            {
                byte head1, head2;
                head1 = (byte)keyLen;
                if((head1 >> 7) == 0)
                {
                    //末尾
                    yield break;
                }
                keyLen = stream.ReadByte();
                if (keyLen == -1)
                {
                    //末尾
                    yield break;
                }
                head2 = (byte)keyLen;
                //获取字节数
                const byte leftNotBit = (0b0111_1111);
                short u8keyLen = (short)(( (((uint)head1) & (leftNotBit)) << 8) | (((uint)head2) & byte.MaxValue));
                //读取字节数
                byte[] keyBuf = new byte[u8keyLen];
                keyLen = stream.ReadBlock(keyBuf, 0, u8keyLen);
                if(keyLen < u8keyLen)
                {
                    yield break;
                }

                path = utf8.GetString(keyBuf);
            }
            
            

            long dataLen;
            //读取长度
            if (!f_readInt64(stream, out dataLen))
            {
                throw new InvalidOperationException(); //预期之外的结构
            }

            CSPInformation inf = new CSPInformation(path, new StreamBlock(stream.Position, dataLen));
            //跳转
            stream.Seek(dataLen, SeekOrigin.Current);
            //yield return inf;
            yield return new KeyValuePair<string, CSPInformation>(path, inf);
            //yield return new KeyValuePair<string, CSPInformation>(path, inf);

            goto Loop;

        }


        static CSPInformation f_toColEn(KeyValuePair<string, CSPInformation> t_pair)
        {
            return t_pair.Value;
        }

        #endregion

        #region 释放

        public override bool IsNeedToReleaseResources => true;

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing && p_disposeBase)
            {
                p_stream?.Close();
            }
            p_stream = null;
            p_dict = null;
            p_list = null;
            p_buffer = null;
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
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            p_list.Sort(index, count, comparer);
        }

        public override void SortInformationIndex(IComparer<DataInformation> comparer)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            p_list.Sort(comparer);
        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            p_list.Sort(comparer, index, count);
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

        public override T GetInformation<T>(int index)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_list[index] as T;
        }

        /// <summary>
        /// 获取指定索引项的信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>数据信息</returns>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        public CSPInformation GetCSPInformation(int index)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_list[index];
        }

        /// <summary>
        /// 获取指定项的信息
        /// </summary>
        /// <param name="dataPath">项数据的路径索引</param>
        /// <returns>数据信息</returns>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">项路径不匹配</exception>
        public CSPInformation GetCSPInformation(string dataPath)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            return p_dict[dataPath];
        }

        public override bool CanAddData => p_canWriter && p_dictIsWriter;

        public override void Add(Stream stream, string dataPath)
        {
            ThrowObjectDisposeException(nameof(CStreamPackReader));
            if (stream is null || dataPath is null) throw new ArgumentNullException();
            if (dataPath.Length == 0 || dataPath.Length > byte.MaxValue)
            {
                throw new ArgumentException();
            }
            if ((!p_dictIsWriter) || (!p_canWriter))
            {
                throw new NotSupportedException();
            }

            if (!stream.CanRead) throw new NotSupportedException();


            //路径字符数
            byte pathLen = (byte)dataPath.Length;
            //创建路径写入缓冲区
            byte[] bufpathStr = new byte[pathLen * 2];

            for (int i = 0; i < pathLen; i++)
            {
                dataPath[i].OrderToByteArray(bufpathStr, i * 2);
            }

            //最后的终止符位置
            long endPos = p_endDataPos;

            CSPInformation inf;

            lock (p_stream)
            {
                p_stream.Seek(endPos, SeekOrigin.Begin);
                //写入路径长度
                p_stream.WriteByte(pathLen);
                //写入路径数据
                p_stream.Write(bufpathStr, 0, bufpathStr.Length);
                //新数据的起始位
                var newDataPos = endPos + bufpathStr.Length;
                //长度
                long wrSize = 0;
                //获取长度参数写入的位置
                var lenWrPos = p_stream.Position;
                //向后推进8个字节
                p_stream.Seek(8, SeekOrigin.Current);
                //写入数据
                foreach (var wrSizeByte in stream.CopyToStreamEnumator(p_stream, p_buffer))
                {
                    wrSize += wrSizeByte;
                }
                inf = new CSPInformation(dataPath, new StreamBlock(newDataPos, wrSize));
                p_endDataPos = newDataPos + wrSize;
                //写入终止符
                p_stream.WriteByte(0);

                //回头写入长度
                byte* ptr_dataLen = stackalloc byte[8];
                IOoperations.OrderToBytes(wrSize, ptr_dataLen);
                p_stream.Seek(lenWrPos, SeekOrigin.Begin);
                p_stream.WriteToAddress(ptr_dataLen, 8);

                var wrDict = (IDictionary<string, Cheng.Algorithm.Compressions.CSPACK.CSPInformation>)p_dict;

                if (wrDict.TryGetValue(dataPath, out var dinf))
                {
                    //存在旧项
                    var indexOf = p_list.IndexOf(dinf);
                    if (indexOf == -1)
                    {
                        p_list.Add(inf);
                    }
                    else
                    {
                        p_list[indexOf] = inf;
                    }
                    wrDict[dataPath] = inf;
                }
                else
                {
                    //不存在
                    //写入新值
                    wrDict.Add(dataPath, inf);
                    p_list.Add(inf);
                }
            }

        }

        public override void Flush()
        {
            p_stream?.Flush();
        }

        IEnumerator<CSPInformation> IEnumerable<CSPInformation>.GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        #endregion

        #region 扩展功能

        /// <summary>
        /// 返回一个枚举器
        /// </summary>
        /// <param name="stream">要在其中当前位置读取数据的流对象</param>
        /// <returns>数据枚举器，每次推进将寻找一个数据项信息并返回，直至CSPACK数据结尾</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象无法读取和查找</exception>
        /// <exception cref="Exception">枚举器解析时出现错误格式</exception>
        public static IEnumerable<CSPInformation> EnumerateReadCStreamPackData(Stream stream)
        {
            return EnumerateReadCStreamPackData(stream, CSPackEncType.CSPack);
        }

        /// <summary>
        /// 返回一个枚举器
        /// </summary>
        /// <param name="stream">要在其中当前位置读取数据的流对象</param>
        /// <param name="type">要使用指定格式读取包数据</param>
        /// <returns>数据枚举器，每次推进将寻找一个数据项信息并返回，直至CSPACK数据结尾</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">格式参数错误</exception>
        /// <exception cref="NotSupportedException">流对象无法读取和查找</exception>
        /// <exception cref="Exception">枚举器解析时出现错误格式</exception>
        public static IEnumerable<CSPInformation> EnumerateReadCStreamPackData(Stream stream, CSPackEncType type)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek)) throw new NotSupportedException();
            if (type <= 0 || type > CSPackEncType.U8CSPack) throw new ArgumentOutOfRangeException();
            return f_readDataPair(stream, new char[255], type, Encoding.UTF8).ToOtherItems(f_toColEn);
        }

        /// <summary>
        /// 返回一个枚举器
        /// </summary>
        /// <param name="stream">要在其中当前位置读取数据的流对象</param>
        /// <returns>数据枚举器，每次推进将寻找一个数据项信息并返回，直至CSPACK数据结尾</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象无法读取和查找</exception>
        /// <exception cref="Exception">枚举器解析时出现错误格式</exception>
        public static IEnumerable<KeyValuePair<string, CSPInformation>> EnumerateReadCStreamPackPairData(Stream stream)
        {
            return EnumerateReadCStreamPackPairData(stream, CSPackEncType.CSPack);
        }

        /// <summary>
        /// 返回一个枚举器
        /// </summary>
        /// <param name="stream">要在其中当前位置读取数据的流对象</param>
        /// <param name="type">要使用指定格式读取包数据</param>
        /// <returns>数据枚举器，每次推进将寻找一个数据项信息并返回，直至CSPACK数据结尾</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">格式参数错误</exception>
        /// <exception cref="NotSupportedException">流对象无法读取和查找</exception>
        /// <exception cref="Exception">枚举器解析时出现错误格式</exception>
        public static IEnumerable<KeyValuePair<string, CSPInformation>>
        EnumerateReadCStreamPackPairData(Stream stream, CSPackEncType type)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek)) throw new NotSupportedException();
            if (type <= 0 || type > CSPackEncType.U8CSPack) throw new ArgumentOutOfRangeException();
            return f_readDataPair(stream, new char[255], type, Encoding.UTF8);
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif