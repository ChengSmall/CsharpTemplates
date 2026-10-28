using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

using Cheng.DataStructure.Streams;
using Cheng.Streams;

namespace Cheng.Algorithm.Compressions.ZipDataParser
{

    partial class ZipDataCompressionParser
    {

        #region 常量

#if DEBUG
        /// <summary>
        /// 解析目录和拷贝数据时使用的复用缓冲区长度
        /// </summary>
#endif
        private const int cp_bufferSize = 1024 * 8;

        private const int cp_headerBufferSize = 64;

#if DEBUG
        /// <summary>
        /// 中央目录文件头长度
        /// </summary>
#endif
        private const int cp_centralHeaderSize = 46;

#if DEBUG
        /// <summary>
        /// 本地文件头长度
        /// </summary>
#endif
        private const int cp_localHeaderSize = 30;

#if DEBUG
        /// <summary>
        /// 中央目录结束记录长度
        /// </summary>
#endif
        private const int cp_endOfCentralHeaderSize = 22;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束记录基础长度
        /// </summary>
#endif
        private const int cp_zip64EndOfCentralHeaderSize = 56;

#if DEBUG
        /// <summary>
        /// zip64中央目录结束记录定位器长度
        /// </summary>
#endif
        private const int cp_zip64LocatorSize = 20;

#if DEBUG
        /// <summary>
        /// 查找中央目录结束记录时向后搜索的最大字节量
        /// </summary>
#endif
        private const int cp_endOfCentralSearchSize = cp_endOfCentralHeaderSize + 0xFFFF;

#if DEBUG
        /// <summary>
        /// 查找zip64中央目录结束记录时向后搜索的最大字节量
        /// </summary>
#endif
        private const int cp_zip64SearchSize = 1024 * 1024;

        private const uint cp_sigLocalHeader = 0x04034B50;

        private const uint cp_sigCentralHeader = 0x02014B50;

        private const uint cp_sigEndOfCentral = 0x06054B50;

        private const uint cp_sigZip64EndOfCentral = 0x06064B50;

        private const uint cp_sigZip64Locator = 0x07064B50;

        private const ushort cp_extraZip64 = 0x0001;

        private const ushort cp_flagUtf8 = 0x0800;

        private const ushort cp_zip64Sentinel16 = 0xFFFF;

        private const uint cp_zip64Sentinel32 = 0xFFFFFFFF;

        private const string cp_exceptionInvalidData = "不是有效的zip包";

        private const string cp_exceptionEndOfCentralNotFound = "找不到中央目录的结束记录";

        private const string cp_exceptionCentralDirectoryCorrupt = "中央目录已损坏";

        private const string cp_exceptionLocalHeaderCorrupt = "本地文件头已损坏";

        private const string cp_exceptionEntryNameInvalid = "项名称无效";

        internal const string cp_exceptionUnsupportedCompression = "无法读取不受支持的压缩项";

        private const string cp_exceptionMultiDisk = "不支持拆分数据";

        #endregion

        #region 解析

        private static string f_getDictionaryKey(ZipDataInformation inf)
        {
            return inf.DataPath;
        }

#if DEBUG
        /// <summary>
        /// 读取zip的中央目录并构建项集合
        /// </summary>
#endif
        private void f_parseZip()
        {
            long streamLength = p_zipStream.Length;
            if (streamLength < cp_endOfCentralHeaderSize)
            {
                throw new InvalidDataException(cp_exceptionEndOfCentralNotFound);
            }

            long endOfCentralPosition = f_findEndOfCentral(streamLength);
            f_readAt(endOfCentralPosition, p_headerBuffer, 0, cp_endOfCentralHeaderSize);

            ushort diskNumber = f_u16(p_headerBuffer, 4);
            ushort centralDiskNumber = f_u16(p_headerBuffer, 6);
            long entriesOnDisk = f_u16(p_headerBuffer, 8);
            long totalEntries = f_u16(p_headerBuffer, 10);
            long centralSize = f_u32(p_headerBuffer, 12);
            long centralOffset = f_u32(p_headerBuffer, 16);

            bool needZip64 =
                entriesOnDisk == cp_zip64Sentinel16 ||
                totalEntries == cp_zip64Sentinel16 ||
                centralSize == cp_zip64Sentinel32 ||
                centralOffset == cp_zip64Sentinel32 ||
                diskNumber == cp_zip64Sentinel16 ||
                centralDiskNumber == cp_zip64Sentinel16;

            long tailStart = endOfCentralPosition;

            if (needZip64)
            {
                long locatorPosition = endOfCentralPosition - cp_zip64LocatorSize;
                bool hasLocator = false;

                if (locatorPosition >= 0 && f_readSignatureAt(locatorPosition) == cp_sigZip64Locator)
                {
                    f_readAt(locatorPosition, p_headerBuffer, 0, cp_zip64LocatorSize);
                    uint zip64Disk = f_u32(p_headerBuffer, 4);
                    ulong zip64Offset = f_u64(p_headerBuffer, 8);
                    uint totalDisks = f_u32(p_headerBuffer, 16);

                    if (totalDisks != 1 || zip64Disk != 0) throw new InvalidDataException(cp_exceptionMultiDisk);

                    long zip64Position = f_findZip64EndOfCentral(locatorPosition, zip64Offset);
                    if (zip64Position < 0)
                    {
                        throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                    }

                    f_readAt(zip64Position, p_headerBuffer, 0, cp_zip64EndOfCentralHeaderSize);
                    if (f_u32(p_headerBuffer, 0) != cp_sigZip64EndOfCentral)
                    {
                        throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                    }

                    ulong recordSize = f_u64(p_headerBuffer, 4);
                    if (recordSize < 44 || (ulong)zip64Position + 12UL + recordSize > (ulong)locatorPosition)
                    {
                        throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                    }

                    uint zip64DiskNumber = f_u32(p_headerBuffer, 16);
                    uint zip64CentralDiskNumber = f_u32(p_headerBuffer, 20);
                    if (zip64DiskNumber != 0 || zip64CentralDiskNumber != 0)
                    {
                        throw new InvalidDataException(cp_exceptionMultiDisk);
                    }

                    ulong zip64TotalEntries = f_u64(p_headerBuffer, 32);
                    ulong zip64CentralSize = f_u64(p_headerBuffer, 40);
                    ulong zip64CentralOffset = f_u64(p_headerBuffer, 48);

                    if (zip64TotalEntries > int.MaxValue ||
                        zip64CentralSize > long.MaxValue ||
                        zip64CentralOffset > long.MaxValue)
                    {
                        throw new InvalidDataException(cp_exceptionInvalidData);
                    }

                    totalEntries = (long)zip64TotalEntries;
                    centralSize = (long)zip64CentralSize;
                    centralOffset = (long)zip64CentralOffset;
                    tailStart = zip64Position;
                    diskNumber = 0;
                    centralDiskNumber = 0;
                    hasLocator = true;
                }

                if (!hasLocator)
                {
                    // 没有zip64定位器时，只有项数达到16位上限是合法的（恰好0xFFFF项且没有zip64记录）
                    if (diskNumber == cp_zip64Sentinel16 || centralDiskNumber == cp_zip64Sentinel16 ||
                        centralSize == cp_zip64Sentinel32 || centralOffset == cp_zip64Sentinel32)
                    {
                        throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                    }
                }
            }

            if (diskNumber != 0 || centralDiskNumber != 0)
            {
                throw new InvalidDataException(cp_exceptionMultiDisk);
            }

            if (totalEntries > int.MaxValue)
            {
                throw new InvalidDataException(cp_exceptionInvalidData);
            }

            if (totalEntries == 0)
            {
                return;
            }

            if (centralSize < 0 || centralOffset < 0)
            {
                throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
            }

            long actualCentralStart = f_findCentralDirectoryStart(centralOffset, centralSize, tailStart, streamLength);
            long offsetDelta = actualCentralStart - centralOffset;

            f_readCentralDirectory(actualCentralStart, centralSize, (int)totalEntries, offsetDelta);
        }

#if DEBUG
        /// <summary>
        /// 从文件尾部查找中央目录结束记录
        /// </summary>
#endif
        private long f_findEndOfCentral(long streamLength)
        {
            long end = streamLength;
            long fallback = -1;

            while (end > 0)
            {
                int searchSize = (int)Math.Min(end, cp_endOfCentralSearchSize);
                long position = f_findSignatureBackward(cp_sigEndOfCentral, end, searchSize);
                if (position < 0) break;

                if (position + cp_endOfCentralHeaderSize <= streamLength)
                {
                    f_readAt(position, p_headerBuffer, 0, cp_endOfCentralHeaderSize);
                    int commentLength = f_u16(p_headerBuffer, 20);
                    long recordEnd = position + cp_endOfCentralHeaderSize + commentLength;
                    if (recordEnd == streamLength)
                    {
                        // 没有尾随数据时，注释长度必须刚好到达流末尾
                        return position;
                    }
                    if (recordEnd < streamLength && fallback < 0)
                    {
                        // 存在尾随数据时，记录第一个可用的位置作为备选
                        fallback = position;
                    }
                }

                end = position;
            }

            if (fallback >= 0) return fallback;
            throw new InvalidDataException(cp_exceptionEndOfCentralNotFound);
        }

#if DEBUG
        /// <summary>
        /// 查找zip64中央目录结束记录
        /// </summary>
        /// <param name="locatorPosition">zip64定位器的位置</param>
        /// <param name="offset">定位器内记录的结束记录位置</param>
#endif
        private long f_findZip64EndOfCentral(long locatorPosition, ulong offset)
        {
            if (offset <= long.MaxValue)
            {
                long position = (long)offset;
                if (f_isZip64EndOfCentral(position, locatorPosition))
                {
                    return position;
                }
            }

            long end = locatorPosition;
            while (end > 0)
            {
                int searchSize = (int)Math.Min(end, cp_zip64SearchSize);
                long position = f_findSignatureBackward(cp_sigZip64EndOfCentral, end, searchSize);
                if (position < 0) return -1;

                if (f_isZip64EndOfCentral(position, locatorPosition))
                {
                    return position;
                }

                end = position;
            }

            return -1;
        }

        private bool f_isZip64EndOfCentral(long position, long limit)
        {
            if (position < 0 || position > limit - cp_zip64EndOfCentralHeaderSize) return false;

            f_readAt(position, p_headerBuffer, 0, cp_zip64EndOfCentralHeaderSize);
            if (f_u32(p_headerBuffer, 0) != cp_sigZip64EndOfCentral) return false;

            ulong recordSize = f_u64(p_headerBuffer, 4);
            if (recordSize < 44) return false;
            if (recordSize > (ulong)(limit - position)) return false;
            if (12UL + recordSize > (ulong)(limit - position)) return false;

            return true;
        }

#if DEBUG
        /// <summary>
        /// 定位中央目录在流内的实际起始位置，兼容在zip数据前附加了数据的自解压包
        /// </summary>
#endif
        private long f_findCentralDirectoryStart(long centralOffset, long centralSize, long tailStart, long streamLength)
        {
            if (centralOffset <= streamLength && centralSize <= streamLength - centralOffset)
            {
                if (f_readSignatureAt(centralOffset) == cp_sigCentralHeader)
                {
                    return centralOffset;
                }
            }

            long fallback = tailStart - centralSize;
            if (fallback >= 0 && fallback + cp_centralHeaderSize <= streamLength)
            {
                if (f_readSignatureAt(fallback) == cp_sigCentralHeader)
                {
                    return fallback;
                }
            }

            throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
        }

#if DEBUG
        /// <summary>
        /// 读取中央目录内的所有项
        /// </summary>
#endif
        private void f_readCentralDirectory(long centralStart, long centralSize, int count, long offsetDelta)
        {
            long streamLength = p_zipStream.Length;
            long position = centralStart;

            int capacity = (int)Math.Min(count, Math.Max(0L, centralSize / cp_centralHeaderSize));
            if (capacity > 0) p_zipList = new List<ZipDataInformation>(capacity);

            for (int i = 0; i < count; i++)
            {
                if (position < 0 || position > streamLength - cp_centralHeaderSize)
                {
                    throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                }

                f_readAt(position, p_headerBuffer, 0, cp_centralHeaderSize);
                if (f_u32(p_headerBuffer, 0) != cp_sigCentralHeader)
                {
                    throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                }

                ushort generalPurposeBitFlag = f_u16(p_headerBuffer, 8);
                ushort compressionMethod = f_u16(p_headerBuffer, 10);
                ushort lastModifyTime = f_u16(p_headerBuffer, 12);
                ushort lastModifyDate = f_u16(p_headerBuffer, 14);
                uint compressedSize32 = f_u32(p_headerBuffer, 20);
                uint beforeSize32 = f_u32(p_headerBuffer, 24);
                int nameLength = f_u16(p_headerBuffer, 28);
                int extraLength = f_u16(p_headerBuffer, 30);
                int commentLength = f_u16(p_headerBuffer, 32);
                ushort diskStart = f_u16(p_headerBuffer, 34);
                uint localHeaderOffset32 = f_u32(p_headerBuffer, 42);

                int variableLength = nameLength + extraLength + commentLength;
                long variablePosition = position + cp_centralHeaderSize;
                if (variableLength < 0 || variablePosition > streamLength ||
                    variableLength > streamLength - variablePosition)
                {
                    throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                }

                byte[] variableBuffer;
                int variableOffset;
                if (variableLength <= p_buffer.Length)
                {
                    variableBuffer = p_buffer;
                    variableOffset = 0;
                    f_readAt(variablePosition, variableBuffer, 0, variableLength);
                }
                else
                {
                    variableBuffer = new byte[variableLength];
                    variableOffset = 0;
                    f_readAt(variablePosition, variableBuffer, 0, variableLength);
                }

                long beforeSize;
                long compressedSize;
                long localHeaderOffset;
                f_readZip64Extra(variableBuffer, variableOffset + nameLength, extraLength,
                    beforeSize32, compressedSize32, localHeaderOffset32, diskStart,
                    out beforeSize, out compressedSize, out localHeaderOffset);

                string dataPath;
                try
                {
                    dataPath = f_decodeEntryName(variableBuffer, variableOffset, nameLength, generalPurposeBitFlag);
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(cp_exceptionEntryNameInvalid, ex);
                }
                if (dataPath.Length == 0)
                {
                    throw new InvalidDataException(cp_exceptionEntryNameInvalid);
                }

                position = variablePosition + variableLength;

                char lastChar = dataPath[dataPath.Length - 1];
                if (lastChar == '/' || lastChar == '\\')
                {
                    // 目录项不参与数据访问
                    continue;
                }

                long finalLocalHeaderOffset = localHeaderOffset + offsetDelta;
                if (finalLocalHeaderOffset < 0)
                {
                    throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                }

                DateTime? modifiedTime = f_dosTimeToDateTime(lastModifyTime, lastModifyDate);

                p_zipList.Add(new ZipDataInformation(dataPath, beforeSize, compressedSize,
                    compressionMethod, generalPurposeBitFlag, finalLocalHeaderOffset,
                    modifiedTime, p_keyEquals, this));
            }
        }

#if DEBUG
        /// <summary>
        /// 解析中央目录项内的扩展字段，获取zip64补充数据
        /// </summary>
#endif
        private static void f_readZip64Extra(byte[] buffer, int offset, int length,
            uint beforeSize32, uint compressedSize32, uint localHeaderOffset32, ushort diskStart,
            out long beforeSize, out long compressedSize, out long localHeaderOffset)
        {
            beforeSize = beforeSize32;
            compressedSize = compressedSize32;
            localHeaderOffset = localHeaderOffset32;

            bool needBeforeSize = beforeSize32 == cp_zip64Sentinel32;
            bool needCompressedSize = compressedSize32 == cp_zip64Sentinel32;
            bool needLocalHeaderOffset = localHeaderOffset32 == cp_zip64Sentinel32;
            bool needDiskStart = diskStart == cp_zip64Sentinel16;

            if ((!needBeforeSize) && (!needCompressedSize) && (!needLocalHeaderOffset) && (!needDiskStart))
            {
                return;
            }

            bool gotBeforeSize = false;
            bool gotCompressedSize = false;
            bool gotLocalHeaderOffset = false;

            int i = 0;
            while (i + 4 <= length)
            {
                ushort extraId = f_u16(buffer, offset + i);
                int extraSize = f_u16(buffer, offset + i + 2);
                int extraPosition = offset + i + 4;
                if (extraSize < 0 || extraPosition > offset + length - extraSize) break;

                if (extraId == cp_extraZip64)
                {
                    int p = extraPosition;
                    int end = extraPosition + extraSize;

                    if (needBeforeSize)
                    {
                        if (p + 8 > end) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                        ulong value = f_u64(buffer, p);
                        p += 8;
                        if (value > long.MaxValue) throw new InvalidDataException(cp_exceptionInvalidData);
                        beforeSize = (long)value;
                        gotBeforeSize = true;
                    }

                    if (needCompressedSize)
                    {
                        if (p + 8 > end) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                        ulong value = f_u64(buffer, p);
                        p += 8;
                        if (value > long.MaxValue) throw new InvalidDataException(cp_exceptionInvalidData);
                        compressedSize = (long)value;
                        gotCompressedSize = true;
                    }

                    if (needLocalHeaderOffset)
                    {
                        if (p + 8 > end) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
                        ulong value = f_u64(buffer, p);
                        p += 8;
                        if (value > long.MaxValue) throw new InvalidDataException(cp_exceptionInvalidData);
                        localHeaderOffset = (long)value;
                        gotLocalHeaderOffset = true;
                    }

                    if (needDiskStart && p + 4 <= end)
                    {
                        // 起始磁盘编号不参与单卷数据访问，缺失时按0处理
                        p += 4;
                    }

                    break;
                }

                i += 4 + extraSize;
            }

            if (needBeforeSize && (!gotBeforeSize)) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
            if (needCompressedSize && (!gotCompressedSize)) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);
            if (needLocalHeaderOffset && (!gotLocalHeaderOffset)) throw new InvalidDataException(cp_exceptionCentralDirectoryCorrupt);

        }

#if DEBUG
        /// <summary>
        /// 解码项名称
        /// </summary>
#endif
        private string f_decodeEntryName(byte[] buffer, int offset, int length, ushort generalPurposeBitFlag)
        {
            Encoding encoding;
            if (p_isISO_IEC21320_1)
            {
                if ((generalPurposeBitFlag & cp_flagUtf8) != 0)
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = p_entryNameEncoding;
                }
            }
            else
            {
                encoding = p_entryNameEncoding;
            }
            return encoding.GetString(buffer, offset, length);
        }

#if DEBUG
        /// <summary>
        /// 将dos时间转换为本地时间
        /// </summary>
#endif
        private static DateTime? f_dosTimeToDateTime(ushort dosTime, ushort dosDate)
        {
            int year = ((dosDate >> 9) & 0x7F) + 1980;
            int month = (dosDate >> 5) & 0x0F;
            int day = dosDate & 0x1F;
            int hour = (dosTime >> 11) & 0x1F;
            int minute = (dosTime >> 5) & 0x3F;
            int second = (dosTime & 0x1F) * 2;

            if (month < 1 || month > 12 || day < 1 || day > 31 ||
                hour > 23 || minute > 59 || second > 59)
            {
                return null;
            }

            try
            {
                return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        #endregion

        #region 数据访问

#if DEBUG
        /// <summary>
        /// 获取项数据在原始流内的位置，若还未读取本地文件头则读取并缓存
        /// </summary>
#endif
        internal void f_ensureDataPosition(ZipDataInformation inf)
        {
            if (inf.p_dataPosition >= 0) return;

            long streamLength = p_zipStream.Length;
            long localHeaderOffset = inf.p_localHeaderOffset;
            if (localHeaderOffset < 0 || localHeaderOffset > streamLength - cp_localHeaderSize)
            {
                throw new InvalidDataException(cp_exceptionLocalHeaderCorrupt);
            }

            f_readAt(localHeaderOffset, p_headerBuffer, 0, cp_localHeaderSize);
            if (f_u32(p_headerBuffer, 0) != cp_sigLocalHeader)
            {
                throw new InvalidDataException(cp_exceptionLocalHeaderCorrupt);
            }

            int nameLength = f_u16(p_headerBuffer, 26);
            int extraLength = f_u16(p_headerBuffer, 28);
            long dataPosition = localHeaderOffset + cp_localHeaderSize + nameLength + extraLength;
            long dataEnd = dataPosition + inf.p_compressedSize;

            if (dataPosition < 0 || dataEnd < dataPosition || dataEnd > streamLength)
            {
                throw new InvalidDataException(cp_exceptionLocalHeaderCorrupt);
            }

            inf.p_dataPosition = dataPosition;
        }

        private StreamBlock f_getDataBlock(ZipDataInformation inf)
        {
            f_ensureDataPosition(inf);
            return new StreamBlock(inf.p_dataPosition, inf.p_compressedSize);
        }

#if DEBUG
        /// <summary>
        /// 打开项数据的读取流
        /// </summary>
#endif
        private Stream f_openEntry(ZipDataInformation inf)
        {
            f_ensureDataPosition(inf);
            int bufsize;
            if (inf.p_compressedSize == 0)
            {
                return new MemoryStream(Array.Empty<byte>(), false);
            }
            if ((inf.p_compressionMethod != 0) && (inf.p_compressedSize > (1024 * 1024 * 256)))
            {
                bufsize = 1024 * 8;
            }
            else
            {
                bufsize = 1024 * 4;
            }
            TruncateStream truncate = new TruncateStream(p_zipStream, inf.p_dataPosition, inf.p_compressedSize, false, bufsize);
            return p_itemDec.CreateDecompressStream(truncate, inf.p_compressionMethod);
        }

#if DEBUG
        /// <summary>
        /// 将项数据解压为字节数组
        /// </summary>
#endif
        private byte[] f_deCompressionToData(ZipDataInformation inf)
        {
            long beforeSize = inf.BeforeSize;
            if (beforeSize < 0 || beforeSize > int.MaxValue)
            {
                throw new InsufficientMemoryException();
            }

            int size = (int)beforeSize;
            byte[] data = new byte[size];

            using (var open = f_openEntry(inf))
            {
                if (size == 0) return data;

                int readCount = open.ReadBlock(data, 0, size);
                if (readCount != size)
                {
                    Array.Resize(ref data, readCount);
                }
            }

            return data;
        }

        private Stream f_openOriEntry(ZipDataInformation inf)
        {
            f_ensureDataPosition(inf);
            if (inf.p_compressedSize == 0)
            {
                return new MemoryStream(Array.Empty<byte>(), false);
            }
            return new TruncateStream(p_zipStream, inf.p_dataPosition, inf.p_compressedSize, false, 4096);
        }

        #endregion

        #region 数据读取

#if DEBUG
        /// <summary>
        /// 从指定位置读取指定长度的数据
        /// </summary>
#endif
        private void f_readAt(long position, byte[] buffer, int offset, int count)
        {
            if (position < 0 || count < 0) throw new InvalidDataException(cp_exceptionInvalidData);
            if (count == 0) return;

            lock (p_zipStream)
            {
                p_zipStream.Position = position;
                int readCount = p_zipStream.ReadBlock(buffer, offset, count);
                if (readCount != count) throw new InvalidDataException(cp_exceptionInvalidData);
            }
        }

#if DEBUG
        /// <summary>
        /// 尝试读取指定位置的四字节签名
        /// </summary>
#endif
        private uint f_readSignatureAt(long position)
        {
            if (position < 0) return 0;
            if (position > p_zipStream.Length - 4) return 0;

            f_readAt(position, p_headerBuffer, 0, 4);
            return f_u32(p_headerBuffer, 0);
        }

#if DEBUG
        /// <summary>
        /// 从指定范围的尾部向前查找四字节签名
        /// </summary>
        /// <param name="signature">要查找的签名</param>
        /// <param name="endPosition">查找范围结束位置（不包含）</param>
        /// <param name="searchSize">向后查找的最大字节量</param>
        /// <returns>找到的签名位置，没有找到返回-1</returns>
#endif
        private long f_findSignatureBackward(uint signature, long endPosition, int searchSize)
        {
            if (endPosition <= 0 || searchSize < 4) return -1;

            long lower = endPosition - searchSize;
            if (lower < 0) lower = 0;

            long end = endPosition;
            while (end > lower)
            {
                long start = end - p_buffer.Length;
                if (start < lower) start = lower;

                int length = (int)(end - start);
                f_readAt(start, p_buffer, 0, length);

                for (int i = length - 4; i >= 0; i--)
                {
                    if (f_u32(p_buffer, i) == signature) return start + i;
                }

                if (start <= lower) break;
                // 保留3字节重叠区，防止签名跨越缓冲区边界
                end = start + 3;
            }

            return -1;
        }

        private static ushort f_u16(byte[] buffer, int offset)
        {
            return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
        }

        private static uint f_u32(byte[] buffer, int offset)
        {
            return (uint)(buffer[offset] |
                (buffer[offset + 1] << 8) |
                (buffer[offset + 2] << 16) |
                (buffer[offset + 3] << 24));
        }

        private static ulong f_u64(byte[] buffer, int offset)
        {
            return (ulong)f_u32(buffer, offset) | ((ulong)f_u32(buffer, offset + 4) << 32);
        }

        #endregion

    }

}
