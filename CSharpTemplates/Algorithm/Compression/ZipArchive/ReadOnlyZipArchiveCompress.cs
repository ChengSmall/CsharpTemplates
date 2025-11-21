using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using Cheng.Memorys;
using System.Collections;
using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Trees;
using Cheng.Streams;

namespace Cheng.Algorithm.Compressions.Systems
{

    /// <summary>
    /// 封装<see cref="ZipArchive"/>对象作为只读对象
    /// </summary>
    /// <remarks>
    /// 封装<see cref="ZipArchive"/>对象作为只读对象，并提供 '/' 和 '\' 分隔符兼容路径的查找模式
    /// </remarks>
    public sealed class ReadOnlyZipArchiveCompress : BaseCompressionParser
    {

        #region 构造

        /// <summary>
        /// 初始化时缓冲区的默认长度
        /// </summary>
        public const int BufferSizeDefault = 1024 * 8;

        /// <summary>
        /// 实例化一个只读<see cref="ZipArchive"/>实例
        /// </summary>
        /// <param name="zipStream">要封装的压缩包数据</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流无法读取或查找</exception>
        /// <exception cref="ArgumentException">该流已关闭</exception>
        /// <exception cref="InvalidDataException">内容无法解析为zip</exception>
        public ReadOnlyZipArchiveCompress(Stream zipStream) : this(zipStream, true, BufferSizeDefault, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 实例化一个只读<see cref="ZipArchive"/>实例
        /// </summary>
        /// <param name="zipStream">要封装的压缩包数据</param>
        /// <param name="onDispose">在释放对象时是否释放基础流，默认为true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流无法读取或查找</exception>
        /// <exception cref="ArgumentException">该流已关闭</exception>
        /// <exception cref="InvalidDataException">内容无法解析为zip</exception>
        public ReadOnlyZipArchiveCompress(Stream zipStream, bool onDispose) : this(zipStream, onDispose, BufferSizeDefault, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 实例化一个只读<see cref="ZipArchive"/>实例
        /// </summary>
        /// <param name="zipStream">要封装的压缩包数据</param>
        /// <param name="onDispose">在释放对象时是否释放基础流，默认为true</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区长度，默认为8192</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流无法读取或查找</exception>
        /// <exception cref="ArgumentException">该流已关闭</exception>
        /// <exception cref="InvalidDataException">内容无法解析为zip</exception>
        public ReadOnlyZipArchiveCompress(Stream zipStream, bool onDispose, int bufferSize) : this(zipStream, onDispose, bufferSize, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 实例化一个只读<see cref="ZipArchive"/>实例
        /// </summary>
        /// <param name="zipStream">要封装的压缩包数据</param>
        /// <param name="onDispose">在释放对象时是否释放基础流，默认为true</param>
        /// <param name="bufferSize">拷贝数据时的缓冲区长度，默认为8192</param>
        /// <param name="entryNameEncoding">在zip包中读取项名时使用的文本编码；默认为utf-8</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流无法读取或查找</exception>
        /// <exception cref="ArgumentException">该流已关闭</exception>
        /// <exception cref="InvalidDataException">内容无法解析为zip</exception>
        public ReadOnlyZipArchiveCompress(Stream zipStream, bool onDispose, int bufferSize, Encoding entryNameEncoding)
        {
            if (zipStream is null) throw new ArgumentNullException();
            if (!(zipStream.CanRead && zipStream.CanSeek)) throw new NotSupportedException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            p_stream = zipStream;
            p_buffer = new byte[bufferSize];
            
            p_zip = new ZipArchive(zipStream, ZipArchiveMode.Read, !onDispose, entryNameEncoding);
            f_init();
        }

        private void f_init()
        {
            var ens = p_zip.Entries;
            int length = ens.Count;
            p_zipDict = new Dictionary<string, ZipArchiveInf>(length, new Cheng.DataStructure.Collections.EqualityStrNotPathSeparator(false, true));
            p_zipList = new List<ZipArchiveInf>(length);
            for (int i = 0; i < length; i++)
            {
                var entry = ens[i];
                if (entry.Length <= 0) continue;
                var enf = new ZipArchiveInf(entry);
                p_zipList.Add(enf);
                p_zipDict.Add(entry.FullName, enf);
            }

        }

        #endregion

        #region 参数

        private Stream p_stream;
        private ZipArchive p_zip;
        private byte[] p_buffer;

        private Dictionary<string, ZipArchiveInf> p_zipDict;
        private List<ZipArchiveInf> p_zipList;
        #endregion

        #region 功能

        #region 回收

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_zip.Dispose();
            }

            p_zip = null;
            p_stream = null;
            p_zipDict = null;
            p_zipList = null;
            return true;
        }

        #endregion

        #region 参数

        public override Stream ParserData => p_stream;

        /// <summary>
        /// 获取内部封装的<see cref="ZipArchive"/>类
        /// </summary>
        public ZipArchive BaseArchive
        {
            get => p_zip;
        }

        #endregion

        #region 权限重写

        public override bool CanGetParserData => true;

        public override bool CanIndexOf => true;

        public override bool CanProbePath => true;

        public override bool CanDeCompression => true;

        public override bool CanOpenCompressedStreamByPath => true;

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanSortInformationIndex => true;

        public override bool CanDeCompressionByIndex => true;

        public override bool CanDeCompressionByPath => true;

        public override bool CanBeCompressed => true;

        #endregion

        #region 派生

        private ZipArchiveEntry f_getEntryBySPath(string path)
        {
            if (p_zipDict.TryGetValue(path, out var inf))
            {
                return inf.p_entry;
            }
            return null;
        }

        public override void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();

            var es = p_zip.Entries;
            var et = es[index];
            if (et is null) throw new ArgumentException();

            using (var open = et.Open())
            {
                char[] buffer = new char[1024];
                StreamReader sr = new StreamReader(open, encoding, false, 1024, true);

                Loop:
                var r = sr.Read(buffer, 0, 1024);
                if (r == 0) goto End;
                writer.Write(buffer, 0, r);
                goto Loop;
                End:

                sr.Close();
            }

        }

        public override void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            var et = f_getEntryBySPath(dataPath);
            if (et is null) throw new ArgumentException();

            using (var open = et.Open())
            {
                char[] buffer = new char[1024];
                StreamReader sr = new StreamReader(open, encoding, false, 1024, true);

                Loop:
                var r = sr.Read(buffer, 0, 1024);
                if (r == 0) goto End;
                writer.Write(buffer, 0, r);
                goto Loop;
                End:

                sr.Close();
            }

        }

        public override bool TryGetInformation<T>(string dataPath, out T information)
        {
            var ez = f_getEntryBySPath(dataPath);
            if (ez is null)
            {
                information = null;
                return false;
            }

            information = ez as T;
            return true;
        }

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException();
            return f_getEntryBySPath(dataPath)?.Open();
        }

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException();
            var es = p_zipList;
            return es[index].p_entry.Open();
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
                return p_zipDict[dataPath];
            }
        }

        public override T GetInformation<T>(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_zipDict[dataPath] as T;
        }

        public override int Count
        {
            get
            {
                return p_zipList.Count;
            }
        }

        public override void Flush()
        {
            ThrowObjectDisposeException();
        }

        public override bool ContainsData(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_zipDict.ContainsKey(dataPath);
        }

        public override IEnumerable<string> EnumatorFileName()
        {
            return p_zipList.ToOtherItems<ZipArchiveInf, string>(infToText);

            string infToText(ZipArchiveInf inf)
            {
                return inf.DataName;
            }
        }

        public override IEnumerable<string> EnumatorFilePath()
        {
            return p_zipList.ToOtherItems<ZipArchiveInf, string>(infToText);
            string infToText(ZipArchiveInf inf)
            {
                return inf.DataPath;
            }
        }

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            var entry = f_getEntryBySPath(dataPath);
            if (entry is null) throw new ArgumentException();

            using (var open = entry.Open())
            {
                open.CopyToStream(stream, p_buffer);
            }
        }

        public override void DeCompressionTo(int index, Stream stream)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.Entries[index];
            if (entry == null) throw new InvalidOperationException();

            using (var open = entry.Open())
            {
                open.CopyToStream(stream, p_buffer);
            }
        }

        public override byte[] DeCompressionToData(int index)
        {
            ThrowObjectDisposeException();
            var entry = p_zipList[index];
            if (entry == null) throw new InvalidOperationException();

            MemoryStream ms;
            using (var open = entry.p_entry.Open())
            {
                if (entry.p_entry.Length > int.MaxValue) throw new InsufficientMemoryException();
                ms = new MemoryStream((int)entry.p_entry.Length);
                open.CopyToStream(ms, p_buffer);
            }
            ms.TryGetBuffer(out var tryBuf);
            return (tryBuf.Count == tryBuf.Array.Length && tryBuf.Offset == 0) ? tryBuf.Array : ms.ToArray();
        }

        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_zipDict[dataPath];
            if (entry is null)
            {
                throw new ArgumentException();
            }

            MemoryStream ms;
            using (var open = entry.p_entry.Open())
            {
                if (entry.p_entry.Length > int.MaxValue) throw new InsufficientMemoryException();
                ms = new MemoryStream((int)entry.p_entry.Length);
                open.CopyToStream(ms, p_buffer);
            }
            ms.TryGetBuffer(out var tryBuf);
            return (tryBuf.Count == tryBuf.Array.Length && tryBuf.Offset == 0) ? tryBuf.Array : ms.ToArray();
        }

        public override bool IsNeedToReleaseResources => true;

        public override IEnumerator<DataInformation> GetEnumerator()
        {
            return p_zipList.GetEnumerator();
        }

        public override T GetInformation<T>(int index)
        {
            return p_zipList[index] as T;
        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            p_zipList.Sort(comparer, index, count);
        }

        public override void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {
            p_zipList.Sort(index, count, comparer);
        }

        public override bool CanGetEntryEnumrator => true;

        public override IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            ThrowObjectDisposeException();
            return p_zipList.GetEnumerator();
        }

        #endregion

        #region 功能

        /// <summary>
        /// 获取内部zip压缩包的项集合
        /// </summary>
        /// <returns>当前压缩包内的项集合</returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<ZipArchiveEntry> GetEntrys()
        {
            ThrowObjectDisposeException();
            return p_zip.Entries;
        }

        #endregion

        #endregion

    }

}
