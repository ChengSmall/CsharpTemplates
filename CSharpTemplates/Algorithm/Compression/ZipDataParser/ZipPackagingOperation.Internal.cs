using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cheng.DataStructure;
using Cheng.Memorys;

namespace Cheng.Algorithm.Compressions.ZipPackagings
{

    public sealed partial class ZipPackagingOperation
    {

        #region 常量

#if DEBUG
        /// <summary>
        /// 默认拷贝数据缓冲区长度
        /// </summary>
#endif
        private const int cp_defaultBufferSize = 1024 * 8;

#if DEBUG
        /// <summary>
        /// 本地文件头长度
        /// </summary>
#endif
        private const int cp_localHeaderSize = 30;

#if DEBUG
        /// <summary>
        /// 中央目录文件头长度
        /// </summary>
#endif
        private const int cp_centralHeaderSize = 46;

#if DEBUG
        /// <summary>
        /// 中央目录结束记录长度
        /// </summary>
#endif
        private const int cp_endOfCentralSize = 22;

#if DEBUG
        /// <summary>
        /// 数据描述符长度，包含四字节签名
        /// </summary>
#endif
        private const int cp_dataDescriptorSize = 16;

#if DEBUG
        /// <summary>
        /// zip64数据描述符长度，包含四字节签名和两个八字节大小
        /// </summary>
#endif
        private const int cp_zip64DataDescriptorSize = 24;

#if DEBUG
        /// <summary>
        /// 文件头写入缓冲区长度，不小于zip64中央目录结束记录长度
        /// </summary>
#endif
        private const int cp_headerBufferSize = 56;

#if DEBUG
        /// <summary>
        /// 本地文件头内zip64扩展字段的长度
        /// </summary>
#endif
        private const int cp_localZip64ExtraSize = 20;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束记录长度
        /// </summary>
#endif
        private const int cp_zip64EndOfCentralSize = 56;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束记录中大小字段的值，记录总长度减去签名和长度字段的12字节
        /// </summary>
#endif
        private const long cp_zip64EndOfCentralRecordSize = 44;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束定位器长度
        /// </summary>
#endif
        private const int cp_zip64LocatorSize = 20;

#if DEBUG
        /// <summary>
        /// 未知长度项在确定本地文件头格式前最多缓冲的数据字节数
        /// </summary>
#endif
        private const int cp_unknownLengthBufferSize = 1024 * 1024;

#if DEBUG
        /// <summary>
        /// 写入文件头时使用的版本号，2.0表示支持Deflate压缩和数据描述符
        /// </summary>
#endif
        private const ushort cp_versionNeeded = 20;

#if DEBUG
        /// <summary>
        /// 使用zip64扩展时写入文件头的版本号，4.5表示支持zip64
        /// </summary>
#endif
        private const ushort cp_zip64VersionNeeded = 45;

#if DEBUG
        /// <summary>
        /// 中央目录的创建版本号，低字节为主版本，0表示MS-DOS
        /// </summary>
#endif
        private const ushort cp_versionMadeBy = 20;

#if DEBUG
        /// <summary>
        /// 使用zip64扩展时中央目录的创建版本号
        /// </summary>
#endif
        private const ushort cp_zip64VersionMadeBy = 45;

#if DEBUG
        /// <summary>
        /// 通用标志位：本地文件头后跟随数据描述符
        /// </summary>
#endif
        private const ushort cp_flagDataDescriptor = 0x0008;

#if DEBUG
        /// <summary>
        /// 通用标志位：项路径名使用UTF-8编码
        /// </summary>
#endif
        private const ushort cp_flagUtf8 = 0x0800;

#if DEBUG
        /// <summary>
        /// 仅存储的压缩方法代码
        /// </summary>
#endif
        private const ushort cp_methodStored = 0;

#if DEBUG
        /// <summary>
        /// Deflate压缩的压缩方法代码
        /// </summary>
#endif
        private const ushort cp_methodDeflate = 8;

#if DEBUG
        /// <summary>
        /// 本地文件头签名
        /// </summary>
#endif
        private const uint cp_sigLocalHeader = 0x04034B50;

#if DEBUG
        /// <summary>
        /// 数据描述符签名
        /// </summary>
#endif
        private const uint cp_sigDataDescriptor = 0x08074B50;

#if DEBUG
        /// <summary>
        /// 中央目录文件头签名
        /// </summary>
#endif
        private const uint cp_sigCentralHeader = 0x02014B50;

#if DEBUG
        /// <summary>
        /// 中央目录结束记录签名
        /// </summary>
#endif
        private const uint cp_sigEndOfCentral = 0x06054B50;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束记录签名
        /// </summary>
#endif
        private const uint cp_sigZip64EndOfCentral = 0x06064B50;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束定位器签名
        /// </summary>
#endif
        private const uint cp_sigZip64Locator = 0x07064B50;

#if DEBUG
        /// <summary>
        /// zip64扩展字段的标识
        /// </summary>
#endif
        private const ushort cp_extraZip64 = 0x0001;

#if DEBUG
        /// <summary>
        /// zip64格式使用的两字节标记值
        /// </summary>
#endif
        private const ushort cp_zip64Sentinel16 = 0xFFFF;

#if DEBUG
        /// <summary>
        /// zip64格式使用的四字节标记值
        /// </summary>
#endif
        private const uint cp_zip64Sentinel32 = 0xFFFFFFFF;

#if DEBUG
        /// <summary>
        /// 经典zip结束记录能够明确表示的最大项数量，0xFFFF是zip64标记值
        /// </summary>
#endif
        private const int cp_maxClassicEntryCount = 0xFFFE;

#if DEBUG
        /// <summary>
        /// 项路径名允许的最大字节长度
        /// </summary>
#endif
        private const int cp_maxNameLength = 0xFFFF;

#if DEBUG
        /// <summary>
        /// 经典zip格式允许的最大字节长度，0xFFFFFFFF属于zip64标记值不可使用
        /// </summary>
#endif
        private const long cp_maxEntrySize = 0xFFFFFFFE;

#if DEBUG
        /// <summary>
        /// Deflate压缩可能产生的最大膨胀余量，用于在已知原始长度时保守判断是否需要zip64
        /// </summary>
#endif
        private const long cp_deflateBoundMargin = 16 * 1024 * 1024;

#if DEBUG
        /// <summary>
        /// CRC32校验初值
        /// </summary>
#endif
        private const uint cp_crc32Begin = 0xFFFFFFFF;

        private const string cp_exceptionPathInvalid = "路径为空、包含不可打印字符，或以路径分隔符开头结尾";

        private const string cp_exceptionPathTooLong = "路径名的UTF-8字节长度超出zip格式上限";

        private const string cp_exceptionOpenStream = "无法打开待打包项的数据流";

        private const string cp_exceptionStreamLengthMismatch = "数据源报告的流长度不准确，单个项的数据量超出经典zip格式上限";

        #endregion

        #region 结构

#if DEBUG
        /// <summary>
        /// 打包完成一个项后的中央目录信息
        /// </summary>
#endif
        private struct ZipCentralRecord
        {
            internal ZipCentralRecord(byte[] nameBytes, uint crc, long compressedSize, long beforeSize, long localHeaderOffset, ushort method, bool zip64)
            {
                this.nameBytes = nameBytes;
                this.crc = crc;
                this.compressedSize = compressedSize;
                this.beforeSize = beforeSize;
                this.localHeaderOffset = localHeaderOffset;
                this.method = method;
                this.zip64 = zip64;
            }

#if DEBUG
            /// <summary>
            /// 项路径名的UTF-8字节
            /// </summary>
#endif
            internal readonly byte[] nameBytes;

#if DEBUG
            /// <summary>
            /// 数据的CRC32校验和
            /// </summary>
#endif
            internal readonly uint crc;

#if DEBUG
            /// <summary>
            /// 压缩后的字节大小
            /// </summary>
#endif
            internal readonly long compressedSize;

#if DEBUG
            /// <summary>
            /// 压缩前的字节大小
            /// </summary>
#endif
            internal readonly long beforeSize;

#if DEBUG
            /// <summary>
            /// 本地文件头在zip数据内的偏移字节
            /// </summary>
#endif
            internal readonly long localHeaderOffset;

#if DEBUG
            /// <summary>
            /// 项使用的zip压缩方法代码
            /// </summary>
#endif
            internal readonly ushort method;

#if DEBUG
            /// <summary>
            /// 该项是否已经使用zip64本地文件头
            /// </summary>
#endif
            internal readonly bool zip64;
        }

#if DEBUG
        /// <summary>
        /// 只写计数器流，用于统计Deflate压缩后写入的字节数量，不要求基础流可查找
        /// </summary>
#endif
        private sealed class CountingWriteStream : Stream
        {
            internal CountingWriteStream(Stream baseStream)
            {
                p_baseStream = baseStream;
            }

            private readonly Stream p_baseStream;

#if DEBUG
            /// <summary>
            /// 已写入基础流的字节数量
            /// </summary>
#endif
            internal long p_count;

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
                p_baseStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                p_baseStream.Write(buffer, offset, count);
                p_count += count;
            }

            public override void WriteByte(byte value)
            {
                p_baseStream.WriteByte(value);
                p_count++;
            }
        }

#if DEBUG
        /// <summary>
        /// 未知长度项的数据缓冲写入流，在数据量未超过缓冲区时延迟写入经典zip本地文件头，超过后改写为zip64本地文件头
        /// </summary>
#endif
        private sealed class ZipEntryBufferStream : Stream
        {
            internal ZipEntryBufferStream(ZipPackagingOperation operation, Stream baseStream, byte[] buffer, byte[] nameBytes, ushort method, ushort dosDate, ushort dosTime)
            {
                p_operation = operation;
                p_baseStream = baseStream;
                p_buffer = buffer;
                p_nameBytes = nameBytes;
                p_method = method;
                p_dosDate = dosDate;
                p_dosTime = dosTime;
            }

            private readonly ZipPackagingOperation p_operation;

            private readonly Stream p_baseStream;

            private readonly byte[] p_buffer;

            private readonly byte[] p_nameBytes;

            private readonly ushort p_method;

            private readonly ushort p_dosDate;

            private readonly ushort p_dosTime;

#if DEBUG
            /// <summary>
            /// 已写入的数据总字节数量
            /// </summary>
#endif
            internal long p_count;

#if DEBUG
            /// <summary>
            /// 已缓存在p_buffer内的字节数量
            /// </summary>
#endif
            private int p_buffered;

#if DEBUG
            /// <summary>
            /// 是否已经写入本地文件头并进入直接写入状态
            /// </summary>
#endif
            private bool p_direct;

#if DEBUG
            /// <summary>
            /// 当前项是否使用zip64本地文件头
            /// </summary>
#endif
            internal bool p_isZip64;

#if DEBUG
            /// <summary>
            /// 当前项是否已经结束写入
            /// </summary>
#endif
            private bool p_finished;

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
                // 本地文件头尚未确定，不能在此处把缓冲数据写入基础流
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (p_finished) throw new InvalidOperationException();
                if (buffer is null) throw new ArgumentNullException(nameof(buffer));
                if (offset < 0 || count < 0 || offset > buffer.Length - count) throw new ArgumentOutOfRangeException();
                if (count == 0) return;

                if (p_direct)
                {
                    p_baseStream.Write(buffer, offset, count);
                    p_count += count;
                    return;
                }

                int remaining = p_buffer.Length - p_buffered;
                if (count <= remaining)
                {
                    Buffer.BlockCopy(buffer, offset, p_buffer, p_buffered, count);
                    p_buffered += count;
                    p_count += count;
                    return;
                }

                // 超过缓冲区，确定使用zip64本地文件头并直接输出
                f_writeDirectHeader();
                p_baseStream.Write(buffer, offset, count);
                p_count += count;
            }

            public override void WriteByte(byte value)
            {
                if (p_finished) throw new InvalidOperationException();

                if (!p_direct && p_buffered < p_buffer.Length)
                {
                    p_buffer[p_buffered] = value;
                    p_buffered++;
                    p_count++;
                    return;
                }

                if (!p_direct) f_writeDirectHeader();
                p_baseStream.WriteByte(value);
                p_count++;
            }

#if DEBUG
            /// <summary>
            /// 强制当前项改用zip64本地文件头，已经缓存的数据会在此处写入基础流
            /// </summary>
#endif
            internal void ForceZip64()
            {
                if (p_finished || p_direct) return;
                f_writeDirectHeader();
            }

#if DEBUG
            /// <summary>
            /// 结束当前项的写入，返回当前项是否使用zip64本地文件头
            /// </summary>
            /// <returns>true表示使用zip64本地文件头，false表示使用经典zip本地文件头</returns>
#endif
            internal bool Finish()
            {
                if (p_finished) return p_isZip64;
                p_finished = true;

                if (!p_direct)
                {
                    p_operation.f_writeLocalHeader(p_baseStream, p_nameBytes, p_method, p_dosDate, p_dosTime, false);
                    if (p_buffered > 0)
                    {
                        p_baseStream.Write(p_buffer, 0, p_buffered);
                        p_buffered = 0;
                    }
                }

                return p_isZip64;
            }

#if DEBUG
            /// <summary>
            /// 写入zip64本地文件头并将已缓存的数据写入基础流
            /// </summary>
#endif
            private void f_writeDirectHeader()
            {
                p_operation.f_writeLocalHeader(p_baseStream, p_nameBytes, p_method, p_dosDate, p_dosTime, true);
                p_isZip64 = true;
                p_direct = true;

                if (p_buffered > 0)
                {
                    p_baseStream.Write(p_buffer, 0, p_buffered);
                    p_buffered = 0;
                }
            }

        }

#if DEBUG
        /// <summary>
        /// zip打包枚举器封装，保证枚举器在未开始推进就被释放时也能结束打包状态
        /// </summary>
#endif
        private sealed class ZipPackingEnumerator : IEnumerator<ZipPackingItem>
        {
            internal ZipPackingEnumerator(ZipPackagingOperation operation, IEnumerator<ZipPackingItem> inner)
            {
                p_operation = operation;
                p_inner = inner;
            }

            private readonly ZipPackagingOperation p_operation;

            private IEnumerator<ZipPackingItem> p_inner;

#if DEBUG
            /// <summary>
            /// 内部枚举器是否已经完整推进结束
            /// </summary>
#endif
            private bool p_completed;

#if DEBUG
            /// <summary>
            /// 当前枚举器是否已经释放
            /// </summary>
#endif
            private bool p_disposed;

            public ZipPackingItem Current => p_inner.Current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (p_disposed) return false;

                if (p_inner.MoveNext()) return true;

                p_completed = true;
                return false;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                if (p_disposed) return;
                p_disposed = true;

                try
                {
                    p_inner?.Dispose();
                }
                finally
                {
                    p_inner = null;
                    // 完整结束时内部枚举器已在finally内结束打包状态，避免错误结束后续的打包
                    if (!p_completed) p_operation.f_packEnd();
                }
            }
        }

        #endregion

        #region 路径

#if DEBUG
        /// <summary>
        /// 创建zip项路径中不允许出现的字符集合
        /// </summary>
        /// <returns>不允许出现的字符集合</returns>
#endif
        private static HashSet<char> f_createNotPathChars()
        {
            HashSet<char> chars = new HashSet<char>();
            char[] pathChars = Path.GetInvalidPathChars();
            char[] nameChars = Path.GetInvalidFileNameChars();

            int i;
            for (i = 0; i < pathChars.Length; i++)
            {
                chars.Add(pathChars[i]);
            }
            for (i = 0; i < nameChars.Length; i++)
            {
                chars.Add(nameChars[i]);
            }

            // 允许路径分隔符
            chars.Remove('/');
            chars.Remove('\\');

            // 显式排除所有控制字符
            for (i = 0; i < 0x20; i++)
            {
                chars.Add((char)i);
            }
            chars.Add((char)0x7F);

            return chars;
        }

#if DEBUG
        /// <summary>
        /// 检查并转换项路径为zip内使用的路径格式
        /// </summary>
        /// <param name="path">要检查的路径</param>
        /// <returns>使用'/'作为分隔符的路径</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">路径格式错误</exception>
#endif
        private string f_getZipPath(string path)
        {
            if (path is null) throw new ArgumentNullException();

            int length = path.Length;
            if (length == 0) throw new ArgumentException();

            char c;
            HashSet<char> notChars = p_notPathChars;

            for (int i = 0; i < length; i++)
            {
                c = path[i];
                if (notChars.Contains(c)) throw new ArgumentException(cp_exceptionPathInvalid, nameof(path));
            }

            c = path[0];
            if (c == '/' || c == '\\') throw new ArgumentException(cp_exceptionPathInvalid, nameof(path));

            c = path[length - 1];
            if (c == '/' || c == '\\') throw new ArgumentException(cp_exceptionPathInvalid, nameof(path));

            string zipPath = path.Replace('\\', '/');
            return zipPath;
        }

        private string f_addZipPath(string path)
        {
            if (path is null) throw new ArgumentNullException();

            int length = path.Length;
            if (length == 0) throw new ArgumentException();

            char c;
            HashSet<char> notChars = p_notPathChars;

            for (int i = 0; i < length; i++)
            {
                c = path[i];
                if (notChars.Contains(c)) throw new ArgumentException();
            }

            c = path[0];
            if (c == '/' || c == '\\') throw new ArgumentException();

            c = path[length - 1];
            if (c == '/' || c == '\\') throw new ArgumentException();

            return path;
        }

#if DEBUG
        /// <summary>
        /// 获取项路径名的UTF-8字节
        /// </summary>
        /// <param name="path">要转换的路径</param>
        /// <returns>路径名的UTF-8字节</returns>
        /// <exception cref="ArgumentException">路径包含无法编码的字符</exception>
#endif
        private byte[] f_getNameBytes(string path)
        {
            try
            {
                var zpath = f_getZipPath(path);
                var size = p_utf8enc.GetByteCount(zpath);
                if (size > cp_maxNameLength) throw new ArgumentException(cp_exceptionPathTooLong);
                byte[] buf = new byte[size];
                var toer = p_utf8enc.GetBytes(zpath, 0, zpath.Length, buf, size);
                if (toer != size) throw new ArgumentException();
                return buf;
            }
            catch (EncoderFallbackException)
            {
                throw new ArgumentException(cp_exceptionPathInvalid, nameof(path));
            }
        }

        #endregion

        #region 文件头

#if DEBUG
        /// <summary>
        /// 获取压缩类型对应的zip压缩方法代码
        /// </summary>
        /// <param name="type">压缩类型</param>
        /// <returns>zip压缩方法代码</returns>
#endif
        private static ushort f_getPackMethod(ZipItemPackType type)
        {
            if (type == ZipItemPackType.Stored) return cp_methodStored;
            return cp_methodDeflate;
        }

#if DEBUG
        /// <summary>
        /// 获取数据源声明的长度，获取失败或无法确认时返回-1
        /// </summary>
        /// <param name="getting">数据源对象</param>
        /// <returns>数据源长度，无法确认时返回-1</returns>
#endif
        private static long f_getStreamLength(IGettingStream getting)
        {
            var re = getting.StreamLength;
            if (re >= 0) return re;
            try
            {
                using (var open = getting.OpenStream())
                {
                    return open.Length;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

#if DEBUG
        /// <summary>
        /// 根据已知的数据长度判断项是否需要使用zip64本地文件头
        /// </summary>
        /// <param name="length">数据源声明的数据长度</param>
        /// <param name="type">项压缩类型</param>
        /// <returns>true表示需要使用zip64，false表示可以使用经典zip</returns>
#endif
        private static bool f_isZip64Entry(long length, ZipItemPackType type)
        {
            if (length < 0) return true;
            if (type == ZipItemPackType.Stored) return length > cp_maxEntrySize;
            // Deflate可能产生少量膨胀，预留余量避免压缩后大小越过经典zip上限
            return length > cp_maxEntrySize - cp_deflateBoundMargin;
        }

#if DEBUG
        /// <summary>
        /// 获取未知长度项使用的延迟缓冲区，首次调用时创建
        /// </summary>
        /// <returns>延迟缓冲区</returns>
#endif
        private byte[] f_getEntryBuffer()
        {
            byte[] buffer = p_unknownBuffer;
            if (buffer is null)
            {
                buffer = new byte[cp_unknownLengthBufferSize];
                p_unknownBuffer = buffer;
            }
            return buffer;
        }

#if DEBUG
        /// <summary>
        /// 获取本地文件头的总字节长度
        /// </summary>
        /// <param name="nameLength">项路径名的字节长度</param>
        /// <param name="zip64">是否使用zip64本地文件头</param>
        /// <returns>本地文件头的总字节长度</returns>
#endif
        private static int f_getLocalHeaderLength(int nameLength, bool zip64)
        {
            return cp_localHeaderSize + nameLength + (zip64 ? cp_localZip64ExtraSize : 0);
        }

#if DEBUG
        /// <summary>
        /// 写入项本地文件头，大小和校验和由之后的数据描述符记录
        /// </summary>
        /// <param name="stream">要写入的流</param>
        /// <param name="nameBytes">项路径名的UTF-8字节</param>
        /// <param name="method">zip压缩方法代码</param>
        /// <param name="dosDate">DOS日期</param>
        /// <param name="dosTime">DOS时间</param>
        /// <param name="zip64">是否写入zip64本地文件头</param>
        /// <returns>本地文件头的总字节长度</returns>
#endif
        private int f_writeLocalHeader(Stream stream, byte[] nameBytes, ushort method, ushort dosDate, ushort dosTime, bool zip64)
        {
            byte[] buffer = p_headerBuffer;
            int extraLength = zip64 ? cp_localZip64ExtraSize : 0;

            f_writeU32(buffer, 0, cp_sigLocalHeader);
            f_writeU16(buffer, 4, zip64 ? cp_zip64VersionNeeded : cp_versionNeeded);
            f_writeU16(buffer, 6, cp_flagDataDescriptor | cp_flagUtf8);
            f_writeU16(buffer, 8, method);
            f_writeU16(buffer, 10, dosTime);
            f_writeU16(buffer, 12, dosDate);
            f_writeU32(buffer, 14, 0);
            f_writeU32(buffer, 18, zip64 ? cp_zip64Sentinel32 : 0u);
            f_writeU32(buffer, 22, zip64 ? cp_zip64Sentinel32 : 0u);
            f_writeU16(buffer, 26, (ushort)nameBytes.Length);
            f_writeU16(buffer, 28, (ushort)extraLength);

            stream.Write(buffer, 0, cp_localHeaderSize);
            stream.Write(nameBytes, 0, nameBytes.Length);

            if (zip64)
            {
                f_writeU16(buffer, 0, cp_extraZip64);
                f_writeU16(buffer, 2, (ushort)(cp_localZip64ExtraSize - 4));
                f_writeU64(buffer, 4, 0);
                f_writeU64(buffer, 12, 0);
                stream.Write(buffer, 0, cp_localZip64ExtraSize);
            }

            return cp_localHeaderSize + nameBytes.Length + extraLength;
        }

#if DEBUG
        /// <summary>
        /// 写入项的数据描述符
        /// </summary>
        /// <param name="stream">要写入的流</param>
        /// <param name="crc">数据的CRC32校验和</param>
        /// <param name="compressedSize">压缩后的字节大小</param>
        /// <param name="beforeSize">压缩前的字节大小</param>
        /// <param name="zip64">是否写入zip64数据描述符</param>
        /// <returns>数据描述符的字节长度</returns>
#endif
        private int f_writeDataDescriptor(Stream stream, uint crc, long compressedSize, long beforeSize, bool zip64)
        {
            byte[] buffer = p_headerBuffer;

            f_writeU32(buffer, 0, cp_sigDataDescriptor);
            f_writeU32(buffer, 4, crc);

            if (zip64)
            {
                f_writeU64(buffer, 8, (ulong)compressedSize);
                f_writeU64(buffer, 16, (ulong)beforeSize);
                stream.Write(buffer, 0, cp_zip64DataDescriptorSize);
                return cp_zip64DataDescriptorSize;
            }

            f_writeU32(buffer, 8, (uint)compressedSize);
            f_writeU32(buffer, 12, (uint)beforeSize);
            stream.Write(buffer, 0, cp_dataDescriptorSize);
            return cp_dataDescriptorSize;
        }

#if DEBUG
        /// <summary>
        /// 将当前时间转换为zip标准使用的DOS时间和日期
        /// </summary>
        /// <param name="time">要转换的时间</param>
        /// <param name="dosDate">DOS日期</param>
        /// <param name="dosTime">DOS时间</param>
#endif
        private static void f_dateTimeToDos(DateTime time, out ushort dosDate, out ushort dosTime)
        {
            int year = time.Year;
            if (year < 1980) year = 1980;
            else if (year > 2107) year = 2107;

            dosDate = (ushort)(((year - 1980) << 9) | (time.Month << 5) | time.Day);
            dosTime = (ushort)((time.Hour << 11) | (time.Minute << 5) | (time.Second >> 1));
        }

#if DEBUG
        /// <summary>
        /// 以小端格式向缓冲区写入两字节无符号整数
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="offset">写入的起始索引</param>
        /// <param name="value">要写入的值</param>
#endif
        private static void f_writeU16(byte[] buffer, int offset, ushort value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }

#if DEBUG
        /// <summary>
        /// 以小端格式向缓冲区写入四字节无符号整数
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="offset">写入的起始索引</param>
        /// <param name="value">要写入的值</param>
#endif
        private static void f_writeU32(byte[] buffer, int offset, uint value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

#if DEBUG
        /// <summary>
        /// 以小端格式向缓冲区写入八字节无符号整数
        /// </summary>
        /// <param name="buffer">目标缓冲区</param>
        /// <param name="offset">写入的起始索引</param>
        /// <param name="value">要写入的值</param>
#endif
        private static void f_writeU64(byte[] buffer, int offset, ulong value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
            buffer[offset + 4] = (byte)(value >> 32);
            buffer[offset + 5] = (byte)(value >> 40);
            buffer[offset + 6] = (byte)(value >> 48);
            buffer[offset + 7] = (byte)(value >> 56);
        }

        #endregion

        #region 校验

#if DEBUG
        /// <summary>
        /// CRC32校验表
        /// </summary>
#endif
        private static readonly uint[] cp_crc32Table = f_createCrc32Table();

#if DEBUG
        /// <summary>
        /// 创建CRC32校验表
        /// </summary>
        /// <returns>CRC32校验表</returns>
#endif
        private static uint[] f_createCrc32Table()
        {
            uint[] table = new uint[256];
            uint value;
            int i, j;

            for (i = 0; i < 256; i++)
            {
                value = (uint)i;
                for (j = 0; j < 8; j++)
                {
                    if ((value & 1) != 0) value = 0xEDB88320 ^ (value >> 1);
                    else value >>= 1;
                }
                table[i] = value;
            }

            return table;
        }

#if DEBUG
        /// <summary>
        /// 更新CRC32校验值
        /// </summary>
        /// <param name="crc">上次计算的校验值，首次传入<see cref="cp_crc32Begin"/></param>
        /// <param name="buffer">要计算校验的数据</param>
        /// <param name="offset">数据的起始索引</param>
        /// <param name="count">数据的字节数量</param>
        /// <returns>更新后的校验值</returns>
#endif
        private static uint f_crc32Update(uint crc, byte[] buffer, int offset, int count)
        {
            uint[] table = cp_crc32Table;
            int end = offset + count;

            for (int i = offset; i < end; i++)
            {
                crc = table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
            }

            return crc;
        }

#if DEBUG
        /// <summary>
        /// 结束CRC32校验计算
        /// </summary>
        /// <param name="crc">更新后的校验值</param>
        /// <returns>最终的CRC32校验和</returns>
#endif
        private static uint f_crc32End(uint crc)
        {
            return crc ^ 0xFFFFFFFF;
        }

        #endregion

    }

}
