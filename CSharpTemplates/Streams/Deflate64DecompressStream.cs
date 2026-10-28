using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Cheng.Streams
{

    /// <summary>
    /// 解压缩 Deflate64 数据的只读流
    /// </summary>
    public unsafe sealed class Deflate64DecompressStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化 Deflate64 解压缩流
        /// </summary>
        /// <param name="stream">要解压的 Deflate64 数据基础流</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> 是 null</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/> 不支持读取</exception>
        public Deflate64DecompressStream(Stream stream) : this(stream, true)
        {}

        /// <summary>
        /// 实例化 Deflate64 解压缩流
        /// </summary>
        /// <param name="stream">要解压的 Deflate64 数据基础流</param>
        /// <param name="onDispose">在释放时是否一并释放基础流，默认为true</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> 是 null</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/> 不支持读取</exception>
        public Deflate64DecompressStream(Stream stream, bool onDispose)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead) throw new NotSupportedException();

            p_stream = stream;
            p_onDispose = onDispose;

            p_inputBuffer = new byte[InputBufferSize];
            p_outputBuffer = new byte[OutputBufferSize];
            p_literalLengths = new byte[288];
            p_distanceLengths = new byte[32];
            p_codeLengths = new byte[19];
            p_blCount = new int[16];
            p_nextCode = new int[16];

            p_literalTable = new int[HuffmanTableSize];
            p_distanceTable = new int[HuffmanTableSize];
            p_codeLengthTable = new int[HuffmanTableSize];
            p_fixedLiteralTable = new int[HuffmanTableSize];
            p_fixedDistanceTable = new int[HuffmanTableSize];

            BuildFixedTables();
        }

        #endregion

        #region 参数

        #region 只读参数

        private const int WindowSize = 65536;
        private const int OutputBufferSize = 262144;
        private const int OutputBufferMask = OutputBufferSize - 1;
        private const int InputBufferSize = 8192;
        private const int HuffmanTableBits = 15;
        private const int HuffmanTableSize = 1 << HuffmanTableBits;

        private static readonly int[] LengthBase = new int[29]
        {
            3, 4, 5, 6, 7, 8, 9, 10,
            11, 13, 15, 17, 19, 23, 27, 31,
            35, 43, 51, 59, 67, 83, 99, 115,
            131, 163, 195, 227, 3
        };

        private static readonly int[] LengthExtra = new int[29]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 2, 2, 2, 2,
            3, 3, 3, 3, 4, 4, 4, 4,
            5, 5, 5, 5, 16
        };

        private static readonly int[] DistanceBase = new int[32]
        {
            1, 2, 3, 4, 5, 7, 9, 13,
            17, 25, 33, 49, 65, 97, 129, 193,
            257, 385, 513, 769, 1025, 1537, 2049, 3073,
            4097, 6145, 8193, 12289, 16385, 24577, 32769, 49153
        };

        private static readonly int[] DistanceExtra = new int[32]
        {
            0, 0, 0, 0, 1, 1, 2, 2,
            3, 3, 4, 4, 5, 5, 6, 6,
            7, 7, 8, 8, 9, 9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14
        };

        #endregion

        #region 参数

        private Stream p_stream;

        private byte[] p_inputBuffer;
        private int p_inputPosition;
        private int p_inputLength;

        private ulong p_bitBuffer;
        private int p_bitCount;

        private byte[] p_outputBuffer;
        private int p_outputRead;
        private int p_outputWrite;
        private int p_outputCount;
        private long p_totalOutput;

        private int p_blockType;
        private int p_storedRemaining;

        private int[] p_literalTable;
        private int[] p_distanceTable;
        private int[] p_codeLengthTable;
        private int[] p_fixedLiteralTable;
        private int[] p_fixedDistanceTable;

        private byte[] p_literalLengths;
        private byte[] p_distanceLengths;
        private byte[] p_codeLengths;
        private int[] p_blCount;
        private int[] p_nextCode;

        private bool p_hasDistanceTree;
        private bool p_onDispose;
        private bool p_inputEnd;
        private bool p_finished;
        private bool p_blockActive;
        private bool p_blockFinal;

        #endregion

        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if (disposing && p_onDispose && p_stream != null)
            {
                p_stream.Dispose();
            }
            p_stream = null;

            return true;
        }

        #endregion

        #region 流属性

        /// <summary>
        /// 获取此流是否支持读取
        /// </summary>
        public override bool CanRead
        {
            get => !IsDispose;
        }

        #endregion

        #region 读取

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose(nameof(Deflate64DecompressStream));
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || offset > buffer.Length || buffer.Length - offset < count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count == 0) return 0;

            fixed (byte* pBuffer = buffer)
            {
                return ReadToAddress(pBuffer + offset, count);
            }
        }

        public override int ReadByte()
        {
            ThrowIsDispose(nameof(Deflate64DecompressStream));

            if (p_outputCount == 0 && p_finished) return -1;

            EnsureOutput(1);
            if (p_outputCount == 0) return -1;

            byte value = p_outputBuffer[p_outputRead];
            p_outputRead = (p_outputRead + 1) & OutputBufferMask;
            p_outputCount--;
            return value;
        }

        public override int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose(nameof(Deflate64DecompressStream));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (count == 0) return 0;

            if (p_outputCount == 0 && p_finished) return 0;

            if (count > WindowSize) count = WindowSize;
            EnsureOutput(count);

            int readCount = count < p_outputCount ? count : p_outputCount;
            if (readCount == 0) return 0;

            int firstLength = OutputBufferSize - p_outputRead;
            if (firstLength > readCount) firstLength = readCount;
            
            fixed (byte* outputBufPtr = p_outputBuffer)
            {
                if (p_outputRead >= p_outputBuffer.Length) throw new ArgumentException();
                Buffer.MemoryCopy(outputBufPtr + p_outputRead, buffer, firstLength, firstLength);
                if (firstLength < readCount)
                {
                    Buffer.MemoryCopy(outputBufPtr, buffer + firstLength, firstLength, firstLength);
                }

            }

            p_outputRead = (p_outputRead + readCount) & OutputBufferMask;
            p_outputCount -= readCount;
            return readCount;
        }

        #endregion

        #region 封装

        #region 解压控制

        private void EnsureOutput(int target)
        {
            while (p_outputCount < target && !p_finished)
            {
                if (!p_blockActive)
                {
                    ReadBlockHeader();
                    continue;
                }

                if (p_blockType == 0)
                {
                    CopyStoredBlock(target);
                }
                else
                {
                    DecodeCompressedBlock(target);
                }
            }
        }

        private void ReadBlockHeader()
        {
            p_blockFinal = ReadBits(1) != 0;
            p_blockType = (int)ReadBits(2);

            if (p_blockType == 0)
            {
                AlignToByte();
                int length = ReadAlignedByte() | (ReadAlignedByte() << 8);
                int complement = ReadAlignedByte() | (ReadAlignedByte() << 8);
                if ((length ^ 0xFFFF) != complement)
                    throw new InvalidDataException("Deflate64 存储块长度校验失败。");

                p_storedRemaining = length;
                p_blockActive = true;
                if (length == 0) FinishBlock();
            }
            else if (p_blockType == 1)
            {
                p_blockActive = true;
            }
            else if (p_blockType == 2)
            {
                ReadDynamicTables();
                p_blockActive = true;
            }
            else
            {
                throw new InvalidDataException("Deflate64 数据包含无效的块类型。");
            }
        }

        private void FinishBlock()
        {
            p_blockActive = false;
            if (p_blockFinal) p_finished = true;
        }

        private void CopyStoredBlock(int target)
        {
            while (p_storedRemaining > 0 && p_outputCount < target)
            {
                int length = target - p_outputCount;
                if (length > p_storedRemaining) length = p_storedRemaining;

                for (int i = 0; i < length; i++)
                {
                    p_outputBuffer[p_outputWrite] = (byte)ReadAlignedByte();
                    p_outputWrite = (p_outputWrite + 1) & OutputBufferMask;
                }

                p_outputCount += length;
                p_totalOutput += length;
                p_storedRemaining -= length;
            }

            if (p_storedRemaining == 0) FinishBlock();
        }

        private void DecodeCompressedBlock(int target)
        {
            int[] literalTable = p_blockType == 1 ? p_fixedLiteralTable : p_literalTable;
            int[] distanceTable = p_blockType == 1 ? p_fixedDistanceTable : p_distanceTable;

            while (p_outputCount < target)
            {
                int symbol = DecodeSymbol(literalTable);

                if (symbol < 256)
                {
                    p_outputBuffer[p_outputWrite] = (byte)symbol;
                    p_outputWrite = (p_outputWrite + 1) & OutputBufferMask;
                    p_outputCount++;
                    p_totalOutput++;
                }
                else if (symbol == 256)
                {
                    FinishBlock();
                    return;
                }
                else if (symbol <= 285)
                {
                    if (p_blockType == 2 && !p_hasDistanceTree)
                        throw new InvalidDataException("Deflate64 数据使用了不存在的距离编码树");

                    int lengthIndex = symbol - 257;
                    int length = LengthBase[lengthIndex] + (int)ReadBits(LengthExtra[lengthIndex]);

                    int distanceSymbol = DecodeSymbol(distanceTable);
                    if (distanceSymbol > 31)
                        throw new InvalidDataException("Deflate64 数据包含无效的距离代码");

                    int distance = DistanceBase[distanceSymbol] + (int)ReadBits(DistanceExtra[distanceSymbol]);
                    if (distance > p_totalOutput || distance > WindowSize)
                        throw new InvalidDataException("Deflate64 数据包含超出窗口范围的距离");

                    CopyMatch(distance, length);
                }
                else
                {
                    throw new InvalidDataException("Deflate64 数据包含无效的长度代码");
                }
            }
        }

        private void CopyMatch(int distance, int length)
        {
            int source = (p_outputWrite - distance) & OutputBufferMask;
            int destination = p_outputWrite;

            for (int i = 0; i < length; i++)
            {
                p_outputBuffer[destination] = p_outputBuffer[source];
                source = (source + 1) & OutputBufferMask;
                destination = (destination + 1) & OutputBufferMask;
            }

            p_outputWrite = destination;
            p_outputCount += length;
            p_totalOutput += length;
        }

        #endregion

        #region Huffman 编码树

        private void BuildFixedTables()
        {
            for (int i = 0; i < 144; i++) p_literalLengths[i] = 8;
            for (int i = 144; i < 256; i++) p_literalLengths[i] = 9;
            for (int i = 256; i < 280; i++) p_literalLengths[i] = 7;
            for (int i = 280; i < 288; i++) p_literalLengths[i] = 8;

            for (int i = 0; i < 32; i++) p_distanceLengths[i] = 5;

            BuildHuffmanTable(p_fixedLiteralTable, p_literalLengths, 288);
            BuildHuffmanTable(p_fixedDistanceTable, p_distanceLengths, 32);
        }

        private void ReadDynamicTables()
        {
            int literalCount = (int)ReadBits(5) + 257;
            int distanceCount = (int)ReadBits(5) + 1;
            int codeLengthCount = (int)ReadBits(4) + 4;

            if (literalCount > 286)
                throw new InvalidDataException("Deflate64 数据的长度编码数量无效");

            Array.Clear(p_codeLengths, 0, p_codeLengths.Length);

            int[] codeLengthOrder = CodeLengthOrder;
            for (int i = 0; i < codeLengthCount; i++)
            {
                p_codeLengths[codeLengthOrder[i]] = (byte)ReadBits(3);
            }

            BuildHuffmanTable(p_codeLengthTable, p_codeLengths, 19);

            Array.Clear(p_literalLengths, 0, literalCount);
            Array.Clear(p_distanceLengths, 0, distanceCount);

            int total = literalCount + distanceCount;
            int index = 0;
            int previous = 0;

            while (index < total)
            {
                int symbol = DecodeSymbol(p_codeLengthTable);

                if (symbol <= 15)
                {
                    SetCodeLength(index, literalCount, symbol);
                    previous = symbol;
                    index++;
                }
                else if (symbol == 16)
                {
                    if (index == 0)
                        throw new InvalidDataException("Deflate64 数据包含无效的重复长度代码。");

                    int repeat = (int)ReadBits(2) + 3;
                    if (index + repeat > total)
                        throw new InvalidDataException("Deflate64 数据的重复长度超出范围。");

                    for (int i = 0; i < repeat; i++)
                    {
                        SetCodeLength(index, literalCount, previous);
                        index++;
                    }
                }
                else if (symbol == 17)
                {
                    int repeat = (int)ReadBits(3) + 3;
                    if (index + repeat > total)
                        throw new InvalidDataException("Deflate64 数据的重复长度超出范围。");

                    previous = 0;
                    index += repeat;
                }
                else if (symbol == 18)
                {
                    int repeat = (int)ReadBits(7) + 11;
                    if (index + repeat > total)
                        throw new InvalidDataException("Deflate64 数据的重复长度超出范围。");

                    previous = 0;
                    index += repeat;
                }
                else
                {
                    throw new InvalidDataException("Deflate64 数据包含无效的代码长度符号。");
                }
            }

            if (p_literalLengths[256] == 0)
                throw new InvalidDataException("Deflate64 数据缺少块结束代码。");

            BuildHuffmanTable(p_literalTable, p_literalLengths, literalCount);

            p_hasDistanceTree = false;
            for (int i = 0; i < distanceCount; i++)
            {
                if (p_distanceLengths[i] != 0)
                {
                    p_hasDistanceTree = true;
                    break;
                }
            }

            if (p_hasDistanceTree)
            {
                BuildHuffmanTable(p_distanceTable, p_distanceLengths, distanceCount);
            }
        }

        private void SetCodeLength(int index, int literalCount, int value)
        {
            if (index < literalCount)
            {
                p_literalLengths[index] = (byte)value;
            }
            else
            {
                p_distanceLengths[index - literalCount] = (byte)value;
            }
        }

        private void BuildHuffmanTable(int[] table, byte[] lengths, int count)
        {
            Array.Clear(table, 0, table.Length);
            Array.Clear(p_blCount, 0, p_blCount.Length);

            for (int i = 0; i < count; i++)
            {
                int length = lengths[i];
                if (length > HuffmanTableBits)
                {
                    throw new InvalidDataException("Deflate64 数据的 Huffman 代码长度超出范围");
                }
                p_blCount[length]++;
            }

            int left = 1;
            for (int bits = 1; bits <= HuffmanTableBits; bits++)
            {
                left <<= 1;
                left -= p_blCount[bits];
                if (left < 0)
                {
                    throw new InvalidDataException("Deflate64 数据的 Huffman 编码树已超限");
                }
            }

            p_blCount[0] = 0;
            int code = 0;
            p_nextCode[0] = 0;
            for (int bits = 1; bits <= HuffmanTableBits; bits++)
            {
                code = (code + p_blCount[bits - 1]) << 1;
                p_nextCode[bits] = code;
            }

            for (int symbol = 0; symbol < count; symbol++)
            {
                int length = lengths[symbol];
                if (length == 0) continue;

                int canonicalCode = p_nextCode[length]++;
                int reversedCode = ReverseBits(canonicalCode, length);
                int entry = (length << 16) | symbol;
                int step = 1 << length;

                for (int i = reversedCode; i < table.Length; i += step)
                {
                    table[i] = entry;
                }
            }
        }

        private int DecodeSymbol(int[] table)
        {
            int index = PeekBits(HuffmanTableBits);
            int entry = table[index];
            int length = entry >> 16;

            if (length == 0)
                throw new InvalidDataException("Deflate64 数据包含无效的 Huffman 编码");
            if (length > p_bitCount)
                throw new EndOfStreamException("Deflate64 数据意外结束");

            p_bitBuffer >>= length;
            p_bitCount -= length;
            return entry & 0xFFFF;
        }

        private static int ReverseBits(int value, int length)
        {
            int result = 0;
            for (int i = 0; i < length; i++)
            {
                result = (result << 1) | (value & 1);
                value >>= 1;
            }
            return result;
        }

        private static readonly int[] CodeLengthOrder = new int[19]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5,
            11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        #endregion

        #region 位读取

        private void AlignToByte()
        {
            int discard = p_bitCount & 7;
            if (discard == 0) return;

            p_bitBuffer >>= discard;
            p_bitCount -= discard;
        }

        private int ReadAlignedByte()
        {
            if (p_bitCount >= 8)
            {
                int value = (int)(p_bitBuffer & 0xFF);
                p_bitBuffer >>= 8;
                p_bitCount -= 8;
                return value;
            }

            if (p_inputPosition >= p_inputLength)
            {
                FillInputBuffer();
                if (p_inputLength == 0)
                    throw new EndOfStreamException("Deflate64 数据意外结束。");
            }

            return p_inputBuffer[p_inputPosition++];
        }

        private ulong ReadBits(int count)
        {
            if (count == 0) return 0;

            while (p_bitCount < count && !p_inputEnd)
            {
                FillInputByte();
            }

            if (p_bitCount < count)
                throw new EndOfStreamException("Deflate64 数据意外结束。");

            ulong value = p_bitBuffer & ((1UL << count) - 1);
            p_bitBuffer >>= count;
            p_bitCount -= count;
            return value;
        }

        private int PeekBits(int count)
        {
            while (p_bitCount < count && !p_inputEnd)
            {
                FillInputByte();
            }

            return (int)(p_bitBuffer & ((1UL << count) - 1));
        }

        private void FillInputByte()
        {
            if (p_inputPosition >= p_inputLength)
            {
                FillInputBuffer();
                if (p_inputLength == 0)
                {
                    p_inputEnd = true;
                    return;
                }
            }

            p_bitBuffer |= ((ulong)p_inputBuffer[p_inputPosition]) << p_bitCount;
            p_inputPosition++;
            p_bitCount += 8;
        }

        private void FillInputBuffer()
        {
            p_inputPosition = 0;
            p_inputLength = p_stream.Read(p_inputBuffer, 0, p_inputBuffer.Length);
            if (p_inputLength <= 0)
            {
                p_inputLength = 0;
                p_inputEnd = true;
            }
        }

        #endregion

        #endregion

    }

}
