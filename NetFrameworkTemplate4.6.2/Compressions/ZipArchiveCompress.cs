using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;
using Cheng.Memorys;
using System.Collections;
using Cheng.Algorithm.Collections;

namespace Cheng.Algorithm.Compressions.Systems
{

    internal sealed class ZipArchiveInf : DataInformation
    {
        public ZipArchiveInf(ZipArchiveEntry entry)
        {
            p_entry = entry;
        }

        private ZipArchiveEntry p_entry;

        public override string DataPath
        {
            get
            {
                if(p_entry is null)
                {
                    return null;
                }
                if (p_entry.Archive is null)
                {
                    return null;
                }
                return p_entry.FullName;
            }
        }

        public override string DataName
        {
            get
            {
                if (p_entry is null)
                {
                    return null;
                }
                if (p_entry.Archive is null)
                {
                    return null;
                }
                return p_entry.Name;
            }
        }

        public override long BeforeSize
        {
            get
            {
                if ((p_entry?.Archive) is null) return -1;
                try
                {
                    return p_entry.Length;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public override long CompressedSize
        {
            get
            {
                if ((p_entry?.Archive) is null) return -1;
                try
                {
                    return p_entry.CompressedLength;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public override DateTime? ModifiedTime
        {
            get
            {
                if ((p_entry?.Archive) == null) return null;
                try
                {
                    var time = p_entry.LastWriteTime;
                    return time.DateTime;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }

    /// <summary>
    /// 使用官方zip格式的压缩算法
    /// </summary>
    public sealed class ZipArchiveCompress : BaseCompressionParser
    {

        #region 构造

        /// <summary>
        /// 指定压缩数据流实例化算法
        /// </summary>
        /// <param name="stream">要压缩或解压缩的数据源</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotSupportedException">流没有指定权限</exception>
        /// <exception cref="InvalidDataException">流的内容无法作为zip压缩文件解析</exception>
        public ZipArchiveCompress(Stream stream) : this(stream, ZipArchiveMode.Update, 1024 * 8)
        {
        }

        /// <summary>
        /// 指定压缩数据流实例化算法
        /// </summary>
        /// <param name="stream">要压缩或解压缩的数据源</param>
        /// <param name="mode">压缩模式</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数格式不正确</exception>
        /// <exception cref="NotSupportedException">流没有指定权限</exception>
        /// <exception cref="InvalidDataException">流的内容无法作为zip压缩文件解析</exception>
        public ZipArchiveCompress(Stream stream, ZipArchiveMode mode) : this(stream, mode, 1024 * 8)
        {
        }

        public ZipArchiveCompress(Stream stream, ZipArchiveMode mode, int bufferSize) : this(stream, mode, 1024 * 8, true)
        {
        }

        /// <summary>
        /// 指定压缩数据流实例化算法
        /// </summary>
        /// <param name="stream">要压缩或解压缩的数据源</param>
        /// <param name="mode">压缩模式</param>
        /// <param name="bufferSize">用于解压缩拷贝数据时的缓冲区大小</param>
        /// <param name="closeBaseStream">释放实例时是否关闭封装的数据流，关闭传入true，不关闭则传入false</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数格式不正确</exception>
        /// <exception cref="NotSupportedException">流没有指定权限</exception>
        /// <exception cref="InvalidDataException">流的内容无法作为zip压缩文件解析</exception>
        public ZipArchiveCompress(Stream stream, ZipArchiveMode mode, int bufferSize, bool closeBaseStream)
        {
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            p_zip = new ZipArchive(stream, mode, true);            

            p_buffer = new byte[bufferSize];
            p_stream = stream;
            p_mode = mode;
            p_freeBase = closeBaseStream;
        }

        #endregion

        #region 参数

        private Stream p_stream;

        private ZipArchive p_zip;

        private byte[] p_buffer;

        private ZipArchiveMode p_mode;
        private bool p_freeBase;

        #endregion

        #region 功能

        #region 回收

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                if (p_freeBase)
                {
                    p_stream?.Close();
                }                
                p_zip?.Dispose();
            }
            p_zip = null;
            p_stream = null;

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

        public override bool CanAddData
        {
            get
            {
                return p_mode != ZipArchiveMode.Read;
            }
        }

        public override bool CanIndexOf => true;

        public override bool CanProbePath => true;

        public override bool CanSetData => p_mode != ZipArchiveMode.Read;

        public override bool CanRemoveData => p_mode != ZipArchiveMode.Read;

        public override bool CanDeCompression
        {
            get
            {
                return p_mode != ZipArchiveMode.Create;
            }
        }

        public override bool CanCreatePath => p_mode != ZipArchiveMode.Read;

        public override bool CanOpenCompressedStreamByPath => true;

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanSortInformationIndex => false;

        public override bool CanDeCompressionByIndex
        {
            get
            {
                return p_mode != ZipArchiveMode.Create;
            }
        }

        public override bool CanDeCompressionByPath
        {
            get
            {
                return p_mode != ZipArchiveMode.Create;
            }
        }

        public override bool CanBeCompressed => true;

        #endregion

        #region 派生

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
            var et = p_zip.GetEntry(dataPath);
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
            var ez = p_zip.GetEntry(dataPath);
            if(ez is null)
            {
                information = null;
                return false;
            }

            information = ez as T;
            return true;
        }

        public override Stream CreateOrOpenStream(string dataPath)
        {
            ThrowObjectDisposeException();

            var et = p_zip.GetEntry(dataPath);

            if(et is null) et = p_zip.CreateEntry(dataPath);

            return et.Open();
        }

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException();
            var et = p_zip.GetEntry(dataPath);
            if (et is null) return null;

            return et.Open();
        }

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException();
            var es = p_zip.Entries;
            if (index < 0 || index >= es.Count) throw new ArgumentOutOfRangeException();

            var et = es[index];
            return et.Open();
        }

        public override bool CreatePath(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.GetEntry(dataPath);

            if(entry is null)
            {
                p_zip.CreateEntry(dataPath);
                return true;
            }
            return false;
        }

        public override void Add(Stream stream, string dataPath)
        {
            ThrowObjectDisposeException();
            var zip = p_zip.CreateEntry(dataPath);
            using (var file = zip.Open())
            {
                stream.CopyToStream(file, p_buffer);
            }
        }

        public override void Clear()
        {
            ThrowObjectDisposeException();
            var list = p_zip.Entries;
            int count = list.Count;
            List<ZipArchiveEntry> addzip = new List<ZipArchiveEntry>(count);
            int i;
            
            for (i = 0; i < list.Count; i++)
            {
                addzip.Add(list[i]);
            }
            for (i = 0; i < count; i++)
            {
                addzip[i].Delete();
            }
        }

        public override void SetData(byte[] data, string dataPath)
        {
            ThrowObjectDisposeException();
            if (data is null) throw new ArgumentNullException();

            var et = p_zip.GetEntry(dataPath);

            if(et is null)
            {
                et = p_zip.CreateEntry(dataPath);
            }
            
            using (var open = et.Open())
            {
                open.Write(data, 0, data.Length);
            }
        }

        public override void SetData(Stream stream, string dataPath)
        {
            ThrowObjectDisposeException();
            if (stream is null) throw new ArgumentNullException();

            var et = p_zip.GetEntry(dataPath);

            if (et is null)
            {
                et = p_zip.CreateEntry(dataPath);
            }

            using (var open = et.Open())
            {
                stream.CopyToStream(open, p_buffer);
            }

            throw new NotSupportedException();
        }

        public override bool Remove(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.GetEntry(dataPath);
            if (entry is null) return false;
            entry.Delete();
            return true;
        }

        public override DataInformation this[int index]
        {
            get
            {
                ThrowObjectDisposeException();
                return new ZipArchiveInf(p_zip.Entries[index]);
            }
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                ThrowObjectDisposeException();
                return new ZipArchiveInf(p_zip.GetEntry(dataPath));
            }
        }

        public override T GetInformation<T>(string dataPath)
        {
            ThrowObjectDisposeException();
            return new ZipArchiveInf(p_zip.GetEntry(dataPath)) as T;
        }

        public override int Count
        {
            get
            {
                return p_zip.Entries.Count;
            }
        }

        public override void Flush()
        {
            ThrowObjectDisposeException();
            p_zip.Dispose();
            p_stream.Flush();
            p_zip = new ZipArchive(p_stream, p_mode, true);
        }

        public override bool ContainsData(string dataPath)
        {
            ThrowObjectDisposeException();
            return p_zip.GetEntry(dataPath) != null;
        }

        public override IEnumerable<string> EnumatorFileName()
        {

            var list = p_zip.Entries;

            for (int i = 0; i < list.Count; i++)
            {
                yield return list[i].Name;
            }
        }

        public override IEnumerable<string> EnumatorFilePath()
        {
            var list = p_zip.Entries;

            for (int i = 0; i < list.Count; i++)
            {
                yield return list[i].FullName;
            }
        }

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.GetEntry(dataPath);
            if (entry == null) throw new ArgumentException();

            using (var open = entry.Open())
            {
                open.CopyToStream(stream, p_buffer);
            }
        }

        public override void Add(byte[] data, string dataPath)
        {
            ThrowObjectDisposeException();
            if (data is null || dataPath is null) throw new ArgumentNullException();
            var entry = p_zip.CreateEntry(dataPath);

            using (var open = entry.Open())
            {
                open.Write(data, 0, data.Length);
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
            var entry = p_zip.Entries[index];
            if (entry == null) throw new InvalidOperationException();
            MemoryStream ms;
            using (var open = entry.Open())
            {
                if (entry.Length > int.MaxValue) throw new InsufficientMemoryException();
                ms = new MemoryStream((int)entry.Length);
                open.CopyToStream(ms, p_buffer);
            }
            return (ms.Length == ms.GetBuffer().Length) ? ms.GetBuffer() : ms.ToArray();
        }

        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException();
            var et = p_zip.GetEntry(dataPath);
            if(et is null)
            {
                throw new ArgumentException();
            }
            //var size = et.CompressedLength;
            //var inf = GetInformation(dataPath);
            MemoryStream ms = new MemoryStream(256);

            DeCompressionTo(dataPath, ms);
            return ms.ToArray();
        }

        public override bool IsNeedToReleaseResources => true;

        #endregion

        #region 功能

        /// <summary>
        /// 创建指定路径下的压缩数据并返回
        /// </summary>
        /// <param name="dataPath">数据所在路径</param>
        /// <returns>
        /// 一个路径项下的流，可用于写入或读取<paramref name="dataPath"/>路径下的压缩数据；不再使用后需要关闭；当已存在该项时，该函数同样会返回一个新的流
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="InvalidDataException">zip数据已损坏</exception>
        /// <exception cref="IOException">IO错误</exception>
        public Stream CreateData(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.CreateEntry(dataPath);

            return entry.Open();
        }

        /// <summary>
        /// 获取当前zip压缩包的项集合
        /// </summary>
        /// <returns>当前压缩包内的项集合</returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<ZipArchiveEntry> GetEntrys()
        {
            ThrowObjectDisposeException();
            return p_zip.Entries;
        }

        /// <summary>
        /// 打开或新建指定路径下的压缩数据并返回
        /// </summary>
        /// <param name="dataPath">数据所在路径</param>
        /// <returns>
        /// 一个路径项下的流，可用于写入或读取<paramref name="dataPath"/>路径下的压缩数据；不再使用后需要关闭
        /// <para>当给定的数据路径不存在时，会创建新的数据并返回流；若数据存在，则返回已有的项所在的流；该函数可以有效减少在写入或读取压缩数据时多余的压缩计算</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="NotSupportedException">没有指定权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="InvalidDataException">zip数据已损坏</exception>
        /// <exception cref="IOException">IO错误</exception>
        public Stream OpenOrCreateData(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_zip.GetEntry(dataPath);

            if(entry == null)
            {
                return p_zip.CreateEntry(dataPath).Open();
            }
            return entry.Open();
        }

        #endregion

        #endregion

    }

}
