using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;

using Cheng.Algorithm.Compressions;
using Cheng.Memorys;
using Cheng.Streams;
using Cheng.IO;
using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Sorts.Comparers;
using Cheng.DataStructure.Streams;

namespace Cheng.Algorithm.Compressions.ResourcePackages
{

    /// <summary>
    /// 一个只读资源包读取解析器
    /// </summary>
    /// <remarks>
    /// <para>封装一个流以进行访问其中资源</para>
    /// <para>
    /// 资源格式：顺序存储，头部索引+资源封装，无压缩<br/>
    /// <code>
    /// 初始化机制 => 从头部读取索引：
    /// 
    /// 1、读取1byte 分隔符：
    ///    如果为0，表示已经不存在索引；跳出循环 ->
    ///    如果为255，则表示存在索引；
    ///
    /// 2、读取2byte 转化为16位无符号整型，表示字符串长度；
    /// 3、读取上一次读取到的值的长度，并将内存转化为字符串，转化格式以C#内置的UTF-16为标准
    /// 4、读取16byte 前8byte转化为长整型，表示该资源所在位置；后8字节表示资源长度
    /// 循环执行
    /// </code>
    /// </para>
    /// </remarks>
    public unsafe class ResourcePackageReader : BaseCompressionParser, IEnumerable<ResourcePackageReader.BlockInformation>
    {

        #region 结构

        /// <summary>
        /// 信息参数
        /// </summary>
        public class BlockInformation : DataInformation
        {

            #region 构造

            /// <summary>
            /// 初始化信息参数
            /// </summary>
            /// <param name="index">索引</param>
            internal BlockInformation(StreamIndex index)
            {

                //this.path = index.path;
                this.index = index;
                //this.size = index.block.length;
                try
                {
                    name = Path.GetFileName(index.path);
                }
                catch (Exception)
                {
                    name = null;
                }
            }

            /// <summary>
            /// 初始化信息参数
            /// </summary>
            /// <param name="index">索引</param>
            /// <param name="fileName">数据名</param>
            internal BlockInformation(StreamIndex index, string fileName)
            {
                this.index = index;
                name = fileName;
            }

            #endregion

            #region 参数

            /// <summary>
            /// 数据名
            /// </summary>
            public readonly string name;

            /// <summary>
            /// 数据索引信息
            /// </summary>
            public readonly StreamIndex index;

            #endregion

            #region 派生

            public override long BeforeSize => index.block.length;

            public override long CompressedSize => index.block.length;

            public override string DataPath => index.path;

            public override string DataName => name;

            /// <summary>
            /// 无法访问
            /// </summary>
            public override DateTime? ModifiedTime => null;

            #endregion

        }

        #endregion

        #region 构造

        /// <summary>
        /// 初始化资源流
        /// </summary>
        /// <param name="stream">要解析的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="InvalidDataException">无法解析流数据</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找权限</exception>
        public ResourcePackageReader(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek)) throw new NotSupportedException();
            p_stream = stream;
            f_init(stream, 1024 * 8);
        }

        /// <summary>
        /// 初始化资源流
        /// </summary>
        /// <param name="stream">要解析的流</param>
        /// <param name="bufferSize">进行查找和获取数据时的缓冲区大小，默认为8192</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="InvalidDataException">无法解析流数据</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找权限</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区大小不大于0</exception>
        public ResourcePackageReader(Stream stream, int bufferSize)
        {
            if (stream is null) throw new ArgumentNullException();
            if (!(stream.CanRead && stream.CanSeek)) throw new NotSupportedException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            p_stream = stream;
            f_init(stream, bufferSize);
        }

        /// <summary>
        /// 空构造
        /// </summary>
        protected ResourcePackageReader()
        {
            p_stream = null;
        }

        /// <summary>
        /// 初始化资源，需要在初始化实例时调用
        /// </summary>
        /// <param name="stream">要初始化资源的流</param>
        /// <param name="bufferSize">要初始化的缓冲区大小，不得小于或等于0</param>
        protected void f_init(Stream stream, int bufferSize)
        {
            
            p_lists = new List<BlockInformation>();
            p_datas = new Dictionary<string, BlockInformation>();

            p_buffer = new byte[bufferSize];
            //p_infDicts = new Dictionary<string, BlockInformation>();

            if (stream.Length == 0)
            {
                //p_infs = Array.Empty<BlockInformation>();

                return;
                //throw new InvalidDataException();
            }

            int rib;

            byte[] buffer8 = new byte[8];

            while (true)
            {
                rib = p_stream.ReadByte();
                //if (rib == -1)
                //{
                //    throw new InvalidDataException();
                //}
                if (rib == pc_notIndex)
                {
                    break;
                }
                else if (rib != pc_haveIndex)
                {
                    //错误
                    throw new InvalidDataException();
                }

                //有数据

                //读取字符串长度

                //rib = stream.ReadBlock(buffer8, 0, 2);
                rib = stream.ReadByte();
                if(rib < 0)
                {
                    throw new InvalidDataException();
                }

                byte strLength = (byte)(rib + 1);

                byte[] strBuf = new byte[strLength * 2];
                //读取字符串内存
                rib = stream.ReadBlock(strBuf, 0, (int)strBuf.Length);

                if (rib != strBuf.Length) throw new InvalidDataException();

                string key;

                fixed (byte* strBufPtr = strBuf)
                {
                    //获取key
                    char[] strcs = new char[strLength];
                    for (int i = 0; i < strLength; i++)
                    {
                        strcs[i] = (char)Cheng.IO.IOoperations.OrderToUInt16(new IntPtr(strBufPtr + (i * 2)));
                    }
                    key = new string(strcs);
                    //key = new string((char*)strBufPtr, 0, strLength);
                }

                //读取块

                long pos, len;

                rib = stream.ReadBlock(buffer8, 0, 8);
                if (rib != 8) throw new InvalidDataException();
                //pos = buffer8.ToStructure<long>();
                pos = buffer8.OrderToInt64();

                rib = stream.ReadBlock(buffer8, 0, 8);
                if (rib != 8) throw new InvalidDataException();
                //len = buffer8.ToStructure<long>();
                len = buffer8.OrderToInt64();
                //StreamBlock block = new StreamBlock(pos, len);
                StreamIndex sindex = new StreamIndex(key, new StreamBlock(pos, len));
                //p_datas.Add(key, block);

                BlockInformation binf = new BlockInformation(sindex);

                p_lists.Add(binf);
                p_datas[key] = binf;
                

            }

            //p_infs = new BlockInformation[p_lists.Count];
            
        }

        #endregion

        #region 释放
        // 无
        #endregion

        #region 参数

        #region 常量

        const byte pc_notIndex = 0;
        const byte pc_haveIndex = byte.MaxValue;

        #endregion

        /// <summary>
        /// 要封装的内部流
        /// </summary>
        protected Stream p_stream;

        /// <summary>
        /// 资源索引集合
        /// </summary>
        protected List<BlockInformation> p_lists;

        /// <summary>
        /// 资源路径索引
        /// </summary>
        protected Dictionary<string, BlockInformation> p_datas;
        
        private byte[] p_buffer;

        #endregion

        #region 功能

        #region 参数访问

        public override Stream ParserData => p_stream;

        #endregion

        #region 权限重写

        public override bool CanGetParserData => true;

        public override bool CanAddData => false;

        public override bool CanRemoveData => false;

        public override bool CanSetData => false;

        public override bool CanProbePath => true;

        public override bool CanIndexOf => true;

        public override bool CanDeCompression => true;

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanOpenCompressedStreamByPath => true;

        public override bool CanSortInformationIndex => true;

        public override bool CanDeCompressionByIndex => true;

        public override bool CanDeCompressionByPath => true;

        public override bool CanGetEntryEnumrator => true;

        #endregion

        #region 功能

        /// <summary>
        /// 获取指定目录下的数据信息
        /// </summary>
        /// <param name="path">
        /// 数据所在的目录
        /// </param>
        /// <returns>一个迭代器，每次迭代返回一个直接处于<paramref name="path"/>目录下的信息</returns>
        /// <exception cref="ArgumentException">参数为null或不是目录格式</exception>
        public virtual IEnumerable<BlockInformation> GetDatasByPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException();

            return f_getDatas(this, path);
        }

        private static IEnumerable<BlockInformation> f_getDatas(ResourcePackageReader pack, string path)
        {

            int length = pack.Count;

            for (int i = 0; i < length; i++)
            {
                var inf = pack.GetBlockInformation(i);

                //检查的路径
                var ipath = inf.index.path;

                //比较 path 和 ipath 的前导值一致

                var ir = ipath.IndexOf(path, StringComparison.Ordinal);

                if (ir != 0)
                {
                    //前导不一致
                    continue;
                }
                ir = path.Length;
                var c = path[ir - 1];
                if (c != System.IO.Path.DirectorySeparatorChar && c != System.IO.Path.AltDirectorySeparatorChar)
                {
                    //path最后一位不是分隔符
                    //推进一位索引
                    ir++;
                }

                var re = ipath.LastIndexOf(System.IO.Path.DirectorySeparatorChar, ir);
                if (re >= 0)
                {
                    //仍有分隔符
                    continue;
                }

                re = ipath.LastIndexOf(System.IO.Path.AltDirectorySeparatorChar, ir);
                if (re >= 0)
                {
                    continue;
                }

                //没有后分隔符，前导匹配，查询一个

                yield return inf;

            }
        }

        /// <summary>
        /// 读取流数据并判断数据是否符合数据包标头
        /// </summary>
        /// <param name="stream">要从中读取标头数据的流，需要提前设置到标头数据所在位置</param>
        /// <returns>是否符合标头数据</returns>
        /// <exception cref="ArgumentNullException">流对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        /// <exception cref="Exception">读取数据时有其它错误</exception>
        public static bool EqualDataHeader(Stream stream)
        {
            byte* header = stackalloc byte[9]
            {
                0x43, 0x68, 0x65, 0x6E, 0x67, 0x50, 0x61, 0x63, 0x6B
            };

            byte* buf = stackalloc byte[9];

            var re = stream.ReadBlock(buf, 9);
            if (re != 9) return false;

            return buf[0] == header[0] &&
                buf[1] == header[1] &&
                buf[2] == header[2] &&
                buf[3] == header[3] &&
                buf[4] == header[4] &&
                buf[5] == header[5] &&
                buf[6] == header[6] &&
                buf[7] == header[7] &&
                buf[8] == header[8];
        }

        #endregion

        #region 派生

        public override void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            if (encoding is null || writer is null) throw new ArgumentNullException();

            var id = p_lists[index];
            var len = p_stream.Length;
            if (id.index.block.length + id.index.block.position > len)
            {
                throw new IOException();
            }

            using (var open = CreateGetBaseStream(id.index.block.position, id.index.block.length))
            {
                char[] buffer = new char[1024];
                StreamReader sr = new StreamReader(open, encoding, false, 1024);

                Loop:
                var r = sr.Read(buffer, 0, 1024);
                if (r == 0) return;
                writer.Write(buffer, 0, r);
                goto Loop;

            }

        }

        public override void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            if (dataPath is null || encoding is null || writer is null) throw new ArgumentNullException();

            var b = p_datas.TryGetValue(dataPath, out var inf);
            if (!b)
            {
                throw new ArgumentException();
            }

            var index = inf.index;
            if (index.block.length + index.block.position > p_stream.Length)
            {
                throw new IOException();
            }

            using (var open = CreateGetBaseStream(index.block.position, index.block.length))
            {
                char[] buffer = new char[1024];
                StreamReader sr = new StreamReader(open, encoding, false, 1024);

                Loop:
                var r = sr.Read(buffer, 0, 1024);
                if (r == 0) return;
                writer.Write(buffer, 0, r);
                goto Loop;

            }

        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            ThrowObjectDisposeException();
            p_lists.Sort(index, count, comparer ?? throw new ArgumentNullException());
        }

        public override void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {

            var tc = new ToOtherComparer<BlockInformation, DataInformation>((obj) => obj, comparer);
            p_lists.Sort(index, count, tc);
        }

        /// <summary>
        /// 创建一个可同时访问基础封装流的截取流数据
        /// </summary>
        /// <remarks>在派生类重写该方法用于定制截取流创建方式</remarks>
        /// <param name="position">要截取基础流的初始位置</param>
        /// <param name="length">要截取的字节的长度</param>
        /// <returns>一个可访问基础封装的截取流对象，不再使用时需要释放</returns>
        protected virtual Stream CreateGetBaseStream(long position, long length)
        {
            if (length == 0) return Stream.Null;

            return new TruncateStream(Stream.Synchronized(p_stream), position, length, false);
        }

        /// <summary>
        /// 使用<see cref="CreateGetBaseStream(long, long)"/>创建的同时访问截取流是否独立于基础流
        /// </summary>
        /// <returns>
        /// <para>若返回false，则<see cref="CreateGetBaseStream(long, long)"/>函数创建的多个流之间异步使用时可能会产生线程冲突或阻塞；若返回true，则基础流和创建的流之间读写数据互不影响</para>
        /// </returns>
        public virtual bool CanCreateGetBaseStreamIsIndependent => false;

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException();
            var b = p_datas.TryGetValue(dataPath, out var block);
            if (b)
            {
                var len = p_stream.Length;
                if(block.index.block.length + block.index.block.position > len)
                {
                    throw new IOException();
                }

                return CreateGetBaseStream(block.index.block.position, block.index.block.length);
            }

            return null;
        }

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException();

            var id = p_lists[index];
            var len = p_stream.Length;
            if (id.index.block.length + id.index.block.position > len)
            {
                throw new IOException();
            }

            return CreateGetBaseStream(id.index.block.position, id.index.block.length);
        }

        /// <summary>
        /// 没有该权限
        /// </summary>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override Stream CreateOrOpenStream(string dataPath)
        {
            throw new NotSupportedException();
        }

        public override bool ContainsData(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_datas.ContainsKey(dataPath);
        }

        public override int Count
        {
            get
            {
                ThrowObjectDisposeException();
                return p_lists.Count;
            }
        }

        public override DataInformation this[int index]
        {
            get
            {
                ThrowObjectDisposeException();
                return p_lists[index];
            }
        }

        public override T GetInformation<T>(int index)
        {
            ThrowObjectDisposeException();
            return p_lists[index] as T;
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                ThrowObjectDisposeException();
                return p_datas[dataPath];
            }
        }

        public override T GetInformation<T>(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_datas[dataPath] as T;
        }

        /// <summary>
        /// 返回指定路径下的信息
        /// </summary>
        /// <param name="dataPath">路径</param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="KeyNotFoundException">路径不存在</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual BlockInformation GetBlockInformation(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_datas[dataPath];
        }

        /// <summary>
        /// 返回指定索引下的信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">超出索引范围</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public virtual BlockInformation GetBlockInformation(int index)
        {
            ThrowObjectDisposeException();
            return p_lists[index];
        }

        public override bool TryGetInformation<T>(string dataPath, out T information)
        {
            information = null;
            bool b = p_datas.TryGetValue(dataPath, out var value);
            if (b)
            {
                information = value as T;
            }
            return b;
        }

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            var blockf = p_datas[dataPath];

            var block = blockf.index.block;

            p_stream.Seek(block.position, SeekOrigin.Begin);

            p_stream.CopyToStream(stream, p_buffer, (ulong)block.length);
        }

        public override void DeCompressionTo(int index, Stream stream)
        {
            ThrowObjectDisposeException();
            var dataf = p_lists[index];
            var data = dataf.index;

            p_stream.Seek(data.block.position, SeekOrigin.Begin);

            p_stream.CopyToStream(stream, p_buffer, (ulong)data.block.length);
        }

        public override byte[] DeCompressionToData(int index)
        {
            ThrowObjectDisposeException();
            var size = this[index].BeforeSize;

            MemoryStream ms = new MemoryStream();
            if(size > 0) ms.Capacity = size < int.MaxValue ? (int)size : 64;

            DeCompressionTo(index, ms);
            if(ms.TryGetBuffer(out var buf))
            {
                if (buf.Count == buf.Array.Length) return buf.Array;
            }
            
            return ms.ToArray();
        }

        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException();
            var size = this[dataPath].BeforeSize;

            MemoryStream ms = new MemoryStream(size < int.MaxValue ? (int)size : 64);

            DeCompressionTo(dataPath, ms);
            if (ms.TryGetBuffer(out var buf))
            {
                if (buf.Count == buf.Array.Length) return buf.Array;
            }

            return ms.ToArray();
        }

        public override IEnumerable<string> EnumatorFilePath()
        {
            foreach (var item in p_lists)
            {
                yield return item.DataPath;
            }
        }

        public override IEnumerable<string> EnumatorFileName()
        {
            foreach (var item in p_lists)
            {
                yield return Path.GetFileName(item.DataName);
            }
        }

        public override void Flush()
        {
        }

        public override string[] GetAllFilePath()
        {
            ThrowObjectDisposeException();
            var enr = System.Linq.Enumerable.Select<BlockInformation, string>(p_lists, ToTransPath);
            return System.Linq.Enumerable.ToArray(enr);

            string ToTransPath(BlockInformation index)
            {
                return index.index.path;
            }

        }

        public override string[] GetAllFileName()
        {
            ThrowObjectDisposeException();
            var enr = System.Linq.Enumerable.Select<BlockInformation, string>(p_lists, ToTransFileName);
            return System.Linq.Enumerable.ToArray(enr);

            string ToTransFileName(BlockInformation index)
            {
                return Path.GetFileName(index.DataName);
            }

        }

        /// <summary>
        /// 返回一个循环访问所有数据信息的枚举器
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<BlockInformation> GetEnumerator()
        {
            ThrowObjectDisposeException();
            return p_lists.GetEnumerator();
        }
       
        IEnumerator<BlockInformation> IEnumerable<BlockInformation>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            return p_lists.ToOtherItems(fs_binfoToEnumator).GetEnumerator();
        }

        static IDataEntry fs_binfoToEnumator(BlockInformation binfo)
        {
            return binfo;
        }

        #endregion

        #endregion

    }

}
