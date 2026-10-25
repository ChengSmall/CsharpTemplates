using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

using Cheng.DataStructure;
using Cheng.DataStructure.Collections;

namespace Cheng.Algorithm.Compressions.ZipPackagings
{

    #region 结构

    /// <summary>
    /// zip包数据项的打包时信息
    /// </summary>
    public readonly struct ZipPackingItem : IEquatable<ZipPackingItem>
    {

        /// <summary>
        /// 初始化zip包数据项的打包信息
        /// </summary>
        /// <param name="path">路径索引</param>
        /// <param name="size">字节大小</param>
        public ZipPackingItem(string path, long size)
        {
            this.path = path;
            this.size = size;
        }

        /// <summary>
        /// 项的字节大小
        /// </summary>
        public readonly long size;

        /// <summary>
        /// 项的路径索引
        /// </summary>
        public readonly string path;

        /// <summary>
        /// 返回索引字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return path;
        }

        public override int GetHashCode()
        {
            if (path is null) return size.GetHashCode();
            return path.GetHashCode() ^ size.GetHashCode();
        }

        public bool Equals(ZipPackingItem other)
        {
            return EqualityStrNotPathSeparator.EqualPath(path, other.path, false, true);
        }

        public override bool Equals(object obj)
        {
            if (obj is ZipPackingItem other)
            {
                return EqualityStrNotPathSeparator.EqualPath(path, other.path, false, true);
            }
            return false;
        }

        /// <summary>
        /// 比较项路径是否一致
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ZipPackingItem x, ZipPackingItem y)
        {
            return EqualityStrNotPathSeparator.EqualPath(x.path, y.path, false, true);
        }

        /// <summary>
        /// 比较项路径是否不一致
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ZipPackingItem x, ZipPackingItem y)
        {
            return !EqualityStrNotPathSeparator.EqualPath(x.path, y.path, false, true);
        }

    }

    /// <summary>
    /// zip项的压缩类型
    /// </summary>
    public enum ZipItemPackType
    {

        /// <summary>
        /// 不压缩，直接拷贝原样数据
        /// </summary>
        Stored = 0,

        /// <summary>
        /// 使用zip常用的Deflate压缩算法
        /// </summary>
        Deflate = 1

    }

    #endregion

    /// <summary>
    /// zip数据打包器
    /// </summary>
    /// <remarks>
    /// <para>该打包器将待打包项写入zip格式的数据流，支持仅存储和Deflate压缩两种方式</para>
    /// <para>当项数量、单个项大小或中央目录位置超出经典zip格式上限时，会自动写入zip64扩展结构；打包时使用数据描述符记录校验和与数据大小，接收流只需要可写，不要求可查找</para>
    /// <para>项路径使用UTF-8编码；当前打包器不支持数据加密</para>
    /// </remarks>
    public sealed partial class ZipPackagingOperation : PackagingOperation
    {

        #region 结构

        private readonly struct ZipPackItem
        {
            internal ZipPackItem(IGettingStream getting, ZipItemPackType type)
            {
                gettingStream = getting;
                packType = type;
            }

            internal readonly IGettingStream gettingStream;
            internal readonly ZipItemPackType packType;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 实例化zip打包器
        /// </summary>
        public ZipPackagingOperation() : this(cp_defaultBufferSize)
        {}

        /// <summary>
        /// 使用指定缓冲区长度实例化zip打包器
        /// </summary>
        /// <param name="bufferSize">打包时拷贝数据使用的缓冲区长度</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区长度没有大于0</exception>
        public ZipPackagingOperation(int bufferSize)
        {
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();

            f_init(bufferSize);
        }

#if DEBUG
        /// <summary>
        /// 构造函数初始化时调用的函数，用来初始化打包器、缓冲区和路径判断参数
        /// </summary>
        /// <param name="bufferSize">打包时拷贝数据使用的缓冲区长度</param>
#endif
        private void f_init(int bufferSize)
        {
            p_pathEqual = new EqualityStrNotPathSeparator(false, true);
            p_pack = new Dictionary<string, ZipPackItem>(p_pathEqual);
            p_records = new List<ZipCentralRecord>();

            p_buffer = new byte[bufferSize];
            p_headerBuffer = new byte[cp_headerBufferSize];
            p_utf8enc = new UTF8Encoding(false, true);
            p_notPathChars = f_createNotPathChars();
            p_getItemNowTime = null;
            p_getNowTime = null;
        }

        #endregion

        #region 参数

        private EqualityStrNotPathSeparator p_pathEqual;
        private Dictionary<string, ZipPackItem> p_pack;
        private Func<DateTime> p_getNowTime;
        private Func<IGettingStream, DateTime?> p_getItemNowTime;

#if DEBUG
        /// <summary>
        /// 打包时拷贝项数据使用的复用缓冲区
        /// </summary>
#endif
        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 打包时写入文件头使用的复用缓冲区，长度不小于zip64中央目录结束记录长度
        /// </summary>
#endif
        private byte[] p_headerBuffer;

#if DEBUG
        /// <summary>
        /// 写入项路径名时使用的UTF-8编码器
        /// </summary>
#endif
        private UTF8Encoding p_utf8enc;

#if DEBUG
        /// <summary>
        /// zip项路径中不允许出现的字符集合
        /// </summary>
#endif
        private HashSet<char> p_notPathChars;

#if DEBUG
        /// <summary>
        /// 打包时记录中央目录项信息的复用集合，打包完成后用于写入中央目录
        /// </summary>
#endif
        private List<ZipCentralRecord> p_records;

#if DEBUG
        /// <summary>
        /// 未知长度项在决定是否使用zip64前使用的延迟缓冲区，首次需要时创建
        /// </summary>
#endif
        private byte[] p_unknownBuffer;

#if DEBUG
        /// <summary>
        /// 当前打包器正在打包的判断值
        /// </summary>
#endif
        private bool p_running;

        #endregion

        #region 重写

        public override bool CanContainsData => true;
        public override bool CanRemoveData => true;

        public override bool ContainsData(string path)
        {
            if (p_running) throw new NotImplementedException();
            return p_pack.ContainsKey(path);
        }

        public override int Count => p_pack.Count;

        public override void SetData(IGettingStream data, string path)
        {
            SetDataItem(data, path);
        }

        public override void SetData(IGettingStream data, string path, byte[] password)
        {
            SetDataItem(data, path);
        }

        public override void SetData(IGettingStream data, string path, string password, Encoding encoding)
        {
            SetDataItem(data, path);
        }

        public override bool TryRemoveData(string path)
        {
            if (p_running) throw new NotImplementedException();
            return p_pack.Remove(path);
        }

        public override void RemoveData(string path)
        {
            if (p_running) throw new NotImplementedException();
            if (!p_pack.Remove(path)) throw new ArgumentException();
        }

        public override void PackTo(Stream stream, byte[] password)
        {
            PackTo(stream);
        }

        public override void PackTo(Stream stream)
        {
            using (var emator = PackToEnumator(stream))
            {
                while (emator.MoveNext()) ;
            }
        }

        public override void PackTo(Stream stream, string password, Encoding encoding)
        {
            PackTo(stream);
        }

        public override void PackToFile(string filePath)
        {
            if (filePath is null) throw new ArgumentNullException(nameof(filePath));

            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, Math.Max(p_buffer.Length, 4096), FileOptions.SequentialScan))
            {
                PackTo(file);
            }
        }

        #endregion

        #region 派生

        /// <summary>
        /// 获取当前所有待打包项的路径
        /// </summary>
        public IReadOnlyCollection<string> PackItemPaths
        {
            get
            {
                if (p_running) throw new NotImplementedException();
                return p_pack.Keys;
            }
        }

        /// <summary>
        /// 打包时获取指定项当前时间戳的委托对象
        /// </summary>
        /// <remarks>
        /// <para>
        /// 回调参数为打包项，返回打包项的时间戳；如果无法获取时间则返回null；如果打包时无法获取时间戳，则转用<see cref="GetTimeFunc"/>获取时间戳
        /// </para>
        /// </remarks>
        /// <value>获取指定项当前时间戳的委托，设置null表示没有项时间戳获取，在打包时使用<see cref="GetTimeFunc"/>获取时间戳</value>
        public Func<IGettingStream, DateTime?> GetItemTimeFunc
        {
            get => p_getItemNowTime;
            set
            {
                if (p_running) throw new NotImplementedException();
                p_getItemNowTime = value;
            }
        }

        /// <summary>
        /// 打包时获取中央目录当前时间戳的委托对象
        /// </summary>
        /// <value>获取当前时间戳的委托，设置null表示使用<see cref="DateTime.Now"/>获取时间戳</value>
        public Func<DateTime> GetTimeFunc
        {
            get => p_getNowTime;
            set
            {
                if (p_running) throw new NotImplementedException();
                p_getNowTime = value;
            }
        }

        /// <summary>
        /// 添加或覆盖一个待打包项
        /// </summary>
        /// <param name="data">要打包的数据获取接口</param>
        /// <param name="path">待打包项的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误；或者压缩类型不属于已有参数</exception>
        /// <exception cref="NotImplementedException">打包器正在打包</exception>
        public void SetDataItem(IGettingStream data, string path)
        {
            SetDataItem(data, path, ZipItemPackType.Deflate);
        }

        /// <summary>
        /// 添加或覆盖一个待打包项
        /// </summary>
        /// <param name="data">要打包的数据获取接口</param>
        /// <param name="path">待打包项的路径；<![CDATA[字符'/'或'\'都表示路径分隔符]]></param>
        /// <param name="type">待打包项的压缩类型，默认使用<see cref="ZipItemPackType.Deflate"/></param>
        /// <exception cref="ArgumentNullException">对象或路径是null</exception>
        /// <exception cref="ArgumentException">设置的路径拥有不可打印字符或其它路径错误；或者压缩类型不属于已有参数</exception>
        /// <exception cref="NotImplementedException">打包器正在打包</exception>
        public void SetDataItem(IGettingStream data, string path, ZipItemPackType type)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (p_running) throw new NotImplementedException();
            if (type < ZipItemPackType.Stored || type > ZipItemPackType.Deflate) throw new ArgumentException();
            p_pack[f_addZipPath(path)] = new ZipPackItem(data, type);
        }

        /// <summary>
        /// 当前打包器是否处于打包状态
        /// </summary>
        public bool Packing
        {
            get => p_running;
        }

        /// <summary>
        /// 开始讲当前待打包队列写入流对象
        /// </summary>
        /// <param name="stream">要将zip数据写入的对象</param>
        /// <returns>
        /// <para>一个打包枚举器，每次推进都会返回一个打包完成的项结果</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">流对象没有写入权限</exception>
        /// <exception cref="NotImplementedException">打包器正在打包</exception>
        /// <exception cref="Exception">其它错误</exception>
        public IEnumerator<ZipPackingItem> PackToEnumator(Stream stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite) throw new NotSupportedException();
            if (p_running) throw new NotImplementedException();
            p_running = true;
            return new ZipPackingEnumerator(this, f_packTo(stream));
        }

#if DEBUG
        /// <summary>
        /// 开始打包p_pack内的数据到zip，项目录文本使用UTF-8，无加密方式
        /// </summary>
        /// <param name="stream">要将zip数据写入的流，流必须可写，允许只写流对象</param>
        /// <returns></returns>
#endif
        private IEnumerator<ZipPackingItem> f_packTo(Stream stream)
        {
            try
            {
                int count = p_pack.Count;

                List<ZipCentralRecord> records = p_records;
                records.Clear();
                if (records.Capacity < count) records.Capacity = count;

                ushort dosDate;
                ushort dosTime;
                DateTime bnowTime = (p_getNowTime is null) ? DateTime.Now : p_getNowTime.Invoke();

                // 记录相对于zip数据起始位置的写入位置
                long position = 0;
                // 项数量超出经典zip 直接表示的范围后必须使用zip64结束记录
                bool zip64Archive = count > cp_maxClassicEntryCount;
                DateTime? nowdt;
                foreach (KeyValuePair<string, ZipPackItem> pair in p_pack)
                {
                    byte[] nameBytes = f_getNameBytes(pair.Key);
                    ushort method = f_getPackMethod(pair.Value.packType);
                    long localHeaderOffset = position;

                    // 数据源声明的长度只用于预先决定本地文件头的格式，未知长度时延迟到数据写完再决定
                    long streamLength = f_getStreamLength(pair.Value.gettingStream);
                    bool lengthKnown = streamLength >= 0;
                    bool entryZip64 = lengthKnown && f_isZip64Entry(streamLength, pair.Value.packType);

                    uint crc = cp_crc32Begin;
                    long beforeSize = 0;
                    long compressedSize;
                    ZipEntryBufferStream bufferStream = null;

                    Stream source = pair.Value.gettingStream.OpenStream();
                    if (source is null) throw new IOException(cp_exceptionOpenStream);

                    try
                    {
                        if(p_getItemNowTime is null)
                        {
                            f_dateTimeToDos(bnowTime, out dosDate, out dosTime);
                        }
                        else
                        {
                            nowdt = p_getItemNowTime.Invoke(pair.Value.gettingStream);
                            if (nowdt.HasValue)
                            {
                                f_dateTimeToDos(nowdt.Value,
                            out dosDate, out dosTime);
                            }
                            else
                            {
                                f_dateTimeToDos(bnowTime,
                            out dosDate, out dosTime);
                            }
                        }

                        if (lengthKnown)
                        {
                            position += f_writeLocalHeader(stream, nameBytes, method, dosDate, dosTime, entryZip64);
                        }
                        else
                        {
                            // 未知长度时先缓存数据，未超过缓冲区按经典zip写入，超过后转换为zip64
                            bufferStream = new ZipEntryBufferStream(this, stream, f_getEntryBuffer(), nameBytes, method, dosDate, dosTime);
                        }

                        Stream dataStream = (bufferStream is null) ? stream : (Stream)bufferStream;

                        if (pair.Value.packType == ZipItemPackType.Stored)
                        {
                            // 仅存储，边拷贝边计算校验和
                            int readCount;
                            while ((readCount = source.Read(p_buffer, 0, p_buffer.Length)) > 0)
                            {
                                crc = f_crc32Update(crc, p_buffer, 0, readCount);
                                dataStream.Write(p_buffer, 0, readCount);
                                beforeSize += readCount;

                                if (bufferStream != null && !bufferStream.p_isZip64 && beforeSize > cp_maxEntrySize)
                                {
                                    bufferStream.ForceZip64();
                                }
                            }
                            compressedSize = beforeSize;
                        }
                        else if (lengthKnown)
                        {
                            // Deflate算法只负责产生压缩数据，zip容器结构由当前打包器写入
                            CountingWriteStream counter = new CountingWriteStream(stream);
                            using (DeflateStream deflate = new DeflateStream(counter, CompressionLevel.Optimal, true))
                            {
                                int readCount;
                                while ((readCount = source.Read(p_buffer, 0, p_buffer.Length)) > 0)
                                {
                                    crc = f_crc32Update(crc, p_buffer, 0, readCount);
                                    deflate.Write(p_buffer, 0, readCount);
                                    beforeSize += readCount;
                                }
                            }
                            compressedSize = counter.p_count;
                        }
                        else
                        {
                            using (DeflateStream deflate = new DeflateStream(bufferStream, CompressionLevel.Optimal, true))
                            {
                                int readCount;
                                while ((readCount = source.Read(p_buffer, 0, p_buffer.Length)) > 0)
                                {
                                    crc = f_crc32Update(crc, p_buffer, 0, readCount);
                                    deflate.Write(p_buffer, 0, readCount);
                                    beforeSize += readCount;

                                    if (!bufferStream.p_isZip64 && beforeSize > cp_maxEntrySize)
                                    {
                                        bufferStream.ForceZip64();
                                    }
                                }
                            }
                            compressedSize = bufferStream.p_count;
                        }

                        if (bufferStream != null)
                        {
                            if (!bufferStream.p_isZip64 && beforeSize > cp_maxEntrySize)
                            {
                                bufferStream.ForceZip64();
                            }
                            entryZip64 = bufferStream.Finish();
                            compressedSize = bufferStream.p_count;
                            position += f_getLocalHeaderLength(nameBytes.Length, entryZip64) + compressedSize;
                        }
                        else
                        {
                            if (!entryZip64 && (beforeSize > cp_maxEntrySize || compressedSize > cp_maxEntrySize))
                            {
                                throw new NotSupportedException(cp_exceptionStreamLengthMismatch);
                            }
                            position += compressedSize;
                        }
                    }
                    finally
                    {
                        source.Dispose();
                    }

                    crc = f_crc32End(crc);
                    position += f_writeDataDescriptor(stream, crc, compressedSize, beforeSize, entryZip64);

                    if (entryZip64) zip64Archive = true;

                    records.Add(new ZipCentralRecord(nameBytes, crc, compressedSize, beforeSize, localHeaderOffset, method, entryZip64));

                    yield return new ZipPackingItem(pair.Key, beforeSize); //每次返回打包完毕的一个项
                }

                // 中央目录
                long centralStart = position;
                int recordCount = records.Count;

                f_dateTimeToDos(bnowTime, out dosDate, out dosTime);

                for (int i = 0; i < recordCount; i++)
                {
                    ZipCentralRecord record = records[i];

                    bool before64 = record.beforeSize > cp_maxEntrySize;
                    bool compressed64 = record.compressedSize > cp_maxEntrySize;
                    bool offset64 = record.localHeaderOffset > cp_maxEntrySize;

                    int extraLength = 0;
                    if (before64) extraLength += 8;
                    if (compressed64) extraLength += 8;
                    if (offset64) extraLength += 8;
                    if (extraLength > 0) extraLength += 4;

                    ushort versionNeeded = (record.zip64 || extraLength > 0) ? cp_zip64VersionNeeded : cp_versionNeeded;
                    ushort versionMadeBy = (record.zip64 || extraLength > 0) ? cp_zip64VersionMadeBy : cp_versionMadeBy;

                    f_writeU32(p_headerBuffer, 0, cp_sigCentralHeader);
                    f_writeU16(p_headerBuffer, 4, versionMadeBy);
                    f_writeU16(p_headerBuffer, 6, versionNeeded);
                    f_writeU16(p_headerBuffer, 8, cp_flagDataDescriptor | cp_flagUtf8);
                    f_writeU16(p_headerBuffer, 10, record.method);
                    f_writeU16(p_headerBuffer, 12, dosTime);
                    f_writeU16(p_headerBuffer, 14, dosDate);
                    f_writeU32(p_headerBuffer, 16, record.crc);
                    f_writeU32(p_headerBuffer, 20, compressed64 ? cp_zip64Sentinel32 : (uint)record.compressedSize);
                    f_writeU32(p_headerBuffer, 24, before64 ? cp_zip64Sentinel32 : (uint)record.beforeSize);
                    f_writeU16(p_headerBuffer, 28, (ushort)record.nameBytes.Length);
                    f_writeU16(p_headerBuffer, 30, (ushort)extraLength);
                    f_writeU16(p_headerBuffer, 32, 0);
                    f_writeU16(p_headerBuffer, 34, 0);
                    f_writeU16(p_headerBuffer, 36, 0);
                    f_writeU32(p_headerBuffer, 38, 0);
                    f_writeU32(p_headerBuffer, 42, offset64 ? cp_zip64Sentinel32 : (uint)record.localHeaderOffset);

                    stream.Write(p_headerBuffer, 0, cp_centralHeaderSize);
                    stream.Write(record.nameBytes, 0, record.nameBytes.Length);

                    if (extraLength > 0)
                    {
                        f_writeU16(p_headerBuffer, 0, cp_extraZip64);
                        f_writeU16(p_headerBuffer, 2, (ushort)(extraLength - 4));

                        int extraOffset = 4;
                        if (before64)
                        {
                            f_writeU64(p_headerBuffer, extraOffset, (ulong)record.beforeSize);
                            extraOffset += 8;
                        }
                        if (compressed64)
                        {
                            f_writeU64(p_headerBuffer, extraOffset, (ulong)record.compressedSize);
                            extraOffset += 8;
                        }
                        if (offset64)
                        {
                            f_writeU64(p_headerBuffer, extraOffset, (ulong)record.localHeaderOffset);
                            extraOffset += 8;
                        }

                        stream.Write(p_headerBuffer, 0, extraLength);
                    }

                    position += cp_centralHeaderSize + record.nameBytes.Length + extraLength;

                    if (record.zip64 || extraLength > 0) zip64Archive = true;
                }

                long centralSize = position - centralStart;
                if (recordCount > cp_maxClassicEntryCount || centralStart > cp_maxEntrySize || centralSize > cp_maxEntrySize)
                {
                    zip64Archive = true;
                }

                // zip64中央目录结束记录和定位器
                if (zip64Archive)
                {
                    long zip64Start = position;

                    f_writeU32(p_headerBuffer, 0, cp_sigZip64EndOfCentral);
                    f_writeU64(p_headerBuffer, 4, (ulong)cp_zip64EndOfCentralRecordSize);
                    f_writeU16(p_headerBuffer, 12, cp_zip64VersionMadeBy);
                    f_writeU16(p_headerBuffer, 14, cp_zip64VersionNeeded);
                    f_writeU32(p_headerBuffer, 16, 0);
                    f_writeU32(p_headerBuffer, 20, 0);
                    f_writeU64(p_headerBuffer, 24, (ulong)recordCount);
                    f_writeU64(p_headerBuffer, 32, (ulong)recordCount);
                    f_writeU64(p_headerBuffer, 40, (ulong)centralSize);
                    f_writeU64(p_headerBuffer, 48, (ulong)centralStart);
                    stream.Write(p_headerBuffer, 0, cp_zip64EndOfCentralSize);
                    position += cp_zip64EndOfCentralSize;

                    f_writeU32(p_headerBuffer, 0, cp_sigZip64Locator);
                    f_writeU32(p_headerBuffer, 4, 0);
                    f_writeU64(p_headerBuffer, 8, (ulong)zip64Start);
                    f_writeU32(p_headerBuffer, 16, 1);
                    stream.Write(p_headerBuffer, 0, cp_zip64LocatorSize);
                    position += cp_zip64LocatorSize;
                }

                // 中央目录结束记录
                f_writeU32(p_headerBuffer, 0, cp_sigEndOfCentral);
                f_writeU16(p_headerBuffer, 4, 0);
                f_writeU16(p_headerBuffer, 6, 0);
                if (zip64Archive)
                {
                    f_writeU16(p_headerBuffer, 8, cp_zip64Sentinel16);
                    f_writeU16(p_headerBuffer, 10, cp_zip64Sentinel16);
                    f_writeU32(p_headerBuffer, 12, cp_zip64Sentinel32);
                    f_writeU32(p_headerBuffer, 16, cp_zip64Sentinel32);
                }
                else
                {
                    f_writeU16(p_headerBuffer, 8, (ushort)recordCount);
                    f_writeU16(p_headerBuffer, 10, (ushort)recordCount);
                    f_writeU32(p_headerBuffer, 12, (uint)centralSize);
                    f_writeU32(p_headerBuffer, 16, (uint)centralStart);
                }
                f_writeU16(p_headerBuffer, 20, 0);

                stream.Write(p_headerBuffer, 0, cp_endOfCentralSize);
                stream.Flush();
            }
            finally
            {
                f_packEnd();
            }
        }

#if DEBUG
        /// <summary>
        /// 结束一次打包状态并清理中央目录缓存
        /// </summary>
#endif
        private void f_packEnd()
        {
            p_records.Clear();
            p_running = false;
        }

        #endregion

    }

}
