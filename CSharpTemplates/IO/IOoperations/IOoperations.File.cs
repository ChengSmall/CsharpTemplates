using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime;
using System.Security;
using Cheng.DataStructure.Hashs;
using Cheng.Memorys;

namespace Cheng.IO
{
    
    public static unsafe partial class IOoperations
    {

        #region File

        #region 文件

        #region Hash256

        unsafe static class Hash256Generator
        {

            static void init_k(uint* k)
            {
                //uint* k = stackalloc uint[64];
                //k[0] = 0x428a2f98; k[1] = 0x71374491; k[2] = 0xb5c0fbcf; k[3] = 0xe9b5dba5;
                //k[4] = 0x3956c25b; k[5] = 0x59f111f1; k[6] = 0x923f82a4; k[7] = 0xab1c5ed5;
                //k[8] = 0xd807aa98; k[9] = 0x12835b01; k[10] = 0x243185be; k[11] = 0x550c7dc3;
                //k[12] = 0x72be5d74; k[13] = 0x80deb1fe; k[14] = 0x9bdc06a7; k[15] = 0xc19bf174;
                //k[16] = 0xe49b69c1; k[17] = 0xefbe4786; k[18] = 0x0fc19dc6; k[19] = 0x240ca1cc;
                //k[20] = 0x2de92c6f; k[21] = 0x4a7484aa; k[22] = 0x5cb0a9dc; k[23] = 0x76f988da;
                //k[24] = 0x983e5152; k[25] = 0xa831c66d; k[26] = 0xb00327c8; k[27] = 0xbf597fc7;
                //k[28] = 0xc6e00bf3; k[29] = 0xd5a79147; k[30] = 0x06ca6351; k[31] = 0x14292967;
                //k[32] = 0x27b70a85; k[33] = 0x2e1b2138; k[34] = 0x4d2c6dfc; k[35] = 0x53380d13;
                //k[36] = 0x650a7354; k[37] = 0x766a0abb; k[38] = 0x81c2c92e; k[39] = 0x92722c85;
                //k[40] = 0xa2bfe8a1; k[41] = 0xa81a664b; k[42] = 0xc24b8b70; k[43] = 0xc76c51a3;
                //k[44] = 0xd192e819; k[45] = 0xd6990624; k[46] = 0xf40e3585; k[47] = 0x106aa070;
                //k[48] = 0x19a4c116; k[49] = 0x1e376c08; k[50] = 0x2748774c; k[51] = 0x34b0bcb5;
                //k[52] = 0x391c0cb3; k[53] = 0x4ed8aa4a; k[54] = 0x5b9cca4f; k[55] = 0x682e6ff3;
                //k[56] = 0x748f82ee; k[57] = 0x78a5636f; k[58] = 0x84c87814; k[59] = 0x8cc70208;
                //k[60] = 0x90befffa; k[61] = 0xa4506ceb; k[62] = 0xbef9a3f7; k[63] = 0xc67178f2;

                k[0] = 0x428a2f98; k[1] = 0x71374491; k[2] = 0xb5c0fbcf; k[3] = 0xe9b5dba5;
                k[4] = 0x3956c25b; k[5] = 0x59f111f1; k[6] = 0x923f82a4; k[7] = 0xab1c5ed5;
                k[8] = 0xd807aa98; k[9] = 0x12835b01; k[10] = 0x243185be; k[11] = 0x550c7dc3;
                k[12] = 0x72be5d74; k[13] = 0x80deb1fe; k[14] = 0x9bdc06a7; k[15] = 0xc19bf174;
                k[16] = 0xe49b69c1; k[17] = 0xefbe4786; k[18] = 0x0fc19dc6; k[19] = 0x240ca1cc;
                k[20] = 0x2de92c6f; k[21] = 0x4a7484aa; k[22] = 0x5cb0a9dc; k[23] = 0x76f988da;
                k[24] = 0x983e5152; k[25] = 0xa831c66d; k[26] = 0xb00327c8; k[27] = 0xbf597fc7;
                k[28] = 0xc6e00bf3; k[29] = 0xd5a79147; k[30] = 0x06ca6351; k[31] = 0x14292967;
                k[32] = 0x27b70a85; k[33] = 0x2e1b2138; k[34] = 0x4d2c6dfc; k[35] = 0x53380d13;
                k[36] = 0x650a7354; k[37] = 0x766a0abb; k[38] = 0x81c2c92e; k[39] = 0x92722c85;
                k[40] = 0xa2bfe8a1; k[41] = 0xa81a664b; k[42] = 0xc24b8b70; k[43] = 0xc76c51a3;
                k[44] = 0xd192e819; k[45] = 0xd6990624; k[46] = 0xf40e3585; k[47] = 0x106aa070;
                k[48] = 0x19a4c116; k[49] = 0x1e376c08; k[50] = 0x2748774c; k[51] = 0x34b0bcb5;
                k[52] = 0x391c0cb3; k[53] = 0x4ed8aa4a; k[54] = 0x5b9cca4f; k[55] = 0x682e6ff3;
                k[56] = 0x748f82ee; k[57] = 0x78a5636f; k[58] = 0x84c87814; k[59] = 0x8cc70208;
                k[60] = 0x90befffa; k[61] = 0xa4506ceb; k[62] = 0xbef9a3f7; k[63] = 0xc67178f2;
            }

            public static void toHash256(Stream stream, byte* buffer32, byte* block64, uint* k64, byte* padding64, byte* lastBlock64, byte* pad64_2, uint* w64i)
            {
                // 初始化哈希状态
                uint h0 = 0x6a09e667;
                uint h1 = 0xbb67ae85;
                uint h2 = 0x3c6ef372;
                uint h3 = 0xa54ff53a;
                uint h4 = 0x510e527f;
                uint h5 = 0x9b05688c;
                uint h6 = 0x1f83d9ab;
                uint h7 = 0x5be0cd19;

                //byte* block = stackalloc byte[64];
                long totalLength = 0;
                int bytesRead;
                uint* k = k64;
                init_k(k);

                // 处理完整的数据块
                while ((bytesRead = stream.ReadBlock(block64, 64)) == 64)
                {
                    ProcessBlock(w64i, k, block64, ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7);
                    totalLength += 64;
                }

                totalLength += bytesRead;
                //byte* lastBlock = stackalloc byte[64];
                byte* lastBlock = lastBlock64;
                for (int i = 0; i < bytesRead; i++)
                    lastBlock[i] = block64[i];

                // 处理填充
                int mod = (int)(totalLength % 64);
                //int ka = (55 - mod + 64) % 64; // 计算填充0的字节数
                ulong totalBits = (ulong)totalLength * 8;

                // 构造填充块
                if (bytesRead > 0 || mod == 0)
                {
                    //byte* padding = stackalloc byte[64];
                    byte* padding = padding64;
                    int paddingOffset = 0;

                    // 如果最后块有数据，复制到填充块
                    if (bytesRead > 0)
                    {
                        for (int i = 0; i < bytesRead; i++)
                            padding[i] = lastBlock[i];
                        paddingOffset = bytesRead;
                    }

                    // 添加0x80
                    padding[paddingOffset++] = 0x80;

                    // 填充0到足够空间或块结束
                    int padEnd = (mod < 56) ? 56 : 64;
                    for (; paddingOffset < padEnd; paddingOffset++)
                        padding[paddingOffset] = 0;

                    // 如果当前块足够放长度
                    if (mod < 56)
                    {
                        WriteUInt64BigEndian(totalBits, padding, 56);
                        ProcessBlock(w64i, k, padding, ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7);
                    }
                    else
                    {
                        // 处理两个填充块
                        ProcessBlock(w64i, k, padding, ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7);
                        byte* padding2 = pad64_2;
                        for (int i = 0; i < 56; i++)
                            padding2[i] = 0;
                        WriteUInt64BigEndian(totalBits, padding2, 56);
                        ProcessBlock(w64i, k, padding2, ref h0, ref h1, ref h2, ref h3, ref h4, ref h5, ref h6, ref h7);
                    }
                }

                // 写入最终结果
                WriteUInt64BigEndian(((ulong)h0 | ((ulong)h1 << 32)), buffer32, 0);
                WriteUInt64BigEndian(((ulong)h2 | ((ulong)h3 << 32)), buffer32, 8);
                WriteUInt64BigEndian(((ulong)h4 | ((ulong)h5 << 32)), buffer32, 16);
                WriteUInt64BigEndian(((ulong)h6 | ((ulong)h7 << 32)), buffer32, 24);

                //WriteUInt32BigEndian(h0, buffer32, 0);
                //WriteUInt32BigEndian(h1, buffer32, 4);
                //WriteUInt32BigEndian(h2, buffer32, 8);
                //WriteUInt32BigEndian(h3, buffer32, 12);
                //WriteUInt32BigEndian(h4, buffer32, 16);
                //WriteUInt32BigEndian(h5, buffer32, 20);
                //WriteUInt32BigEndian(h6, buffer32, 24);
                //WriteUInt32BigEndian(h7, buffer32, 28);
            }

            static void ProcessBlock(uint* w64i, uint* k, byte* block, ref uint h0, ref uint h1, ref uint h2, ref uint h3, ref uint h4, ref uint h5, ref uint h6, ref uint h7)
            {
                uint* w = w64i;
                for (int i = 0; i < 16; i++)
                    w[i] = (uint)block[i * 4] << 24 | (uint)block[i * 4 + 1] << 16 | (uint)block[i * 4 + 2] << 8 | block[i * 4 + 3];

                for (int i = 16; i < 64; i++)
                {
                    uint s0 = RightRotate(w[i - 15], 7) ^ RightRotate(w[i - 15], 18) ^ (w[i - 15] >> 3);
                    uint s1 = RightRotate(w[i - 2], 17) ^ RightRotate(w[i - 2], 19) ^ (w[i - 2] >> 10);
                    w[i] = w[i - 16] + s0 + w[i - 7] + s1;
                }

                uint a = h0, b = h1, c = h2, d = h3, e = h4, f = h5, g = h6, h = h7;

                for (int i = 0; i < 64; i++)
                {
                    uint S1 = RightRotate(e, 6) ^ RightRotate(e, 11) ^ RightRotate(e, 25);
                    uint ch = (e & f) ^ (~e & g);
                    uint temp1 = h + S1 + ch + k[i] + w[i];
                    uint S0 = RightRotate(a, 2) ^ RightRotate(a, 13) ^ RightRotate(a, 22);
                    uint maj = (a & b) ^ (a & c) ^ (b & c);
                    uint temp2 = S0 + maj;

                    h = g; g = f; f = e; e = d + temp1; d = c; c = b; b = a; a = temp1 + temp2;
                }

                h0 += a; h1 += b; h2 += c; h3 += d;
                h4 += e; h5 += f; h6 += g; h7 += h;
            }

            static uint RightRotate(uint value, int bits) => (value >> bits) | (value << (32 - bits));

            static void WriteUInt32BigEndian(uint value, byte* buffer, int offset)
            {
                buffer[offset] = (byte)(value >> 24);
                buffer[offset + 1] = (byte)(value >> 16);
                buffer[offset + 2] = (byte)(value >> 8);
                buffer[offset + 3] = (byte)value;
            }

            static void WriteUInt64BigEndian(ulong value, byte* buffer, int offset)
            {
                buffer[offset] = (byte)(value >> 56);
                buffer[offset + 1] = (byte)(value >> 48);
                buffer[offset + 2] = (byte)(value >> 40);
                buffer[offset + 3] = (byte)(value >> 32);
                buffer[offset + 4] = (byte)(value >> 24);
                buffer[offset + 5] = (byte)(value >> 16);
                buffer[offset + 6] = (byte)(value >> 8);
                buffer[offset + 7] = (byte)value;
            }

        }

        /// <summary>
        /// 采用SHA256计算指定流的Hash256值
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns>对于<paramref name="stream"/>Hash256值的默认计算结果</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        public static Hash256 ToHash256(this Stream stream)
        {
            if (stream is null) throw new ArgumentNullException();
            byte* buffer32 = stackalloc byte[32];
            uint* k64 = stackalloc uint[64];
            byte* block64 = stackalloc byte[64];
            byte* pad64 = stackalloc byte[64];
            byte* b64 = stackalloc byte[64];
            byte* pad2 = stackalloc byte[64];
            uint* w64i = stackalloc uint[64];
            Hash256Generator.toHash256(stream, buffer32, block64, k64, pad64, b64, pad2, w64i);
            return Hash256.BytesToHash256(buffer32);
        }

        /// <summary>
        /// 采用SHA256计算指定流的Hash256值
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="buffer256">用作生成时的缓冲区参数；会根据缓冲区大小动态分配内存占用空间，大小不得少于256字节；最佳可利用缓冲区大小为800字节</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">缓冲区大小不够</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有读取权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        public static Hash256 ToHash256(this Stream stream, byte[] buffer256)
        {
            if (stream is null || buffer256 is null) throw new ArgumentNullException();

            if (buffer256.Length < (64 * 4))
            {
                throw new ArgumentException();
            }

            fixed (byte* bufPtr = buffer256)
            {

                if (buffer256.Length >= ((64 * 4) + (64) + (32) + (64) + (64) + (64) + (64 * 4)))
                {
                    //byte* b64 = stackalloc byte[64];
                    //byte* pad2 = stackalloc byte[64];
                    //uint* w64i = stackalloc uint[64];
                    Hash256Generator.toHash256(stream,
                        (bufPtr + ((64 * 4) + 64)),
                        (bufPtr + (4 * 64)),
                        (uint*)bufPtr,
                        (bufPtr + ((64 * 4) + 64 + 32)),
                        (bufPtr + ((64 * 4) + 64 + 32 + 64)),
                        (bufPtr + ((64 * 4) + 64 + 32 + 64 + 64)),
                        (uint*)(bufPtr + ((64 * 4) + 64 + 32 + 64 + 64 + (64))));

                    return Hash256.BytesToHash256((bufPtr + ((64 * 4) + 64)));

                }
                if (buffer256.Length >= ((64 * 4) + (64) + (32) + (64) + (64) + 64))
                {
                    //byte* b64 = stackalloc byte[64];
                    //byte* pad2 = stackalloc byte[64];
                    uint* w64i = stackalloc uint[64];
                    Hash256Generator.toHash256(stream,
                        (bufPtr + ((64 * 4) + 64)),
                        (bufPtr + (4 * 64)),
                        (uint*)bufPtr,
                        (bufPtr + ((64 * 4) + 64 + 32)),
                        (bufPtr + ((64 * 4) + 64 + 32 + 64)),
                        (bufPtr + ((64 * 4) + 64 + 32 + 64 + 64)),
                        w64i);

                    return Hash256.BytesToHash256((bufPtr + ((64 * 4) + 64)));

                }
                if (buffer256.Length >= ((64 * 4) + (64) + (32) + (64) + 64))
                {
                    //byte* b64 = stackalloc byte[64];
                    uint* w64i = stackalloc uint[64];
                    byte* pad2 = stackalloc byte[64];
                    Hash256Generator.toHash256(stream,
                        (bufPtr + ((64 * 4) + 64)),
                        (bufPtr + (4 * 64)),
                        (uint*)bufPtr,
                        (bufPtr + ((64 * 4) + 64 + 32)),
                        (bufPtr + ((64 * 4) + 64 + 32 + 64)),
                        pad2,
                        w64i);

                    return Hash256.BytesToHash256((bufPtr + ((64 * 4) + 64)));

                }
                if (buffer256.Length >= ((64 * 4) + (64) + (32) + (64)))
                {
                    //byte* pad64 = null;
                    uint* w64i = stackalloc uint[64];
                    byte* pad2 = stackalloc byte[64];
                    byte* b64 = stackalloc byte[64];
                    Hash256Generator.toHash256(stream,
                        (bufPtr + ((64 * 4) + 64)), 
                        (bufPtr + (4 * 64)), 
                        (uint*)bufPtr,
                        (bufPtr + ((64 * 4) + 64 + 32)),
                        b64,
                        pad2,
                        w64i);

                    return Hash256.BytesToHash256((bufPtr + ((64 * 4) + 64)));
                }
                if (buffer256.Length >= ((64 * 4) + (64) + (32)))
                {
                    //缓冲区能呈下3个临时容器
                    uint* w64i = stackalloc uint[64];
                    byte* pad2 = stackalloc byte[64];
                    //byte* buffer32 = stackalloc byte[32];
                    //uint[] k64 = new uint[64];
                    //byte* block64 = stackalloc byte[64];
                    byte* pad64 = stackalloc byte[64];
                    byte* b64 = stackalloc byte[64];
                    Hash256Generator.toHash256(stream, 
                        (bufPtr + ((64 * 4) + 64)), (bufPtr + (4 * 64)), (uint*)bufPtr,
                        pad64, b64, pad2, w64i);

                    return Hash256.BytesToHash256((bufPtr + ((64 * 4) + 64)));

                }
                if (buffer256.Length >= ((64 * 4) + (64)))
                {
                    //缓冲区能呈下两个临时容器
                    uint* w64i = stackalloc uint[64];
                    byte* pad2 = stackalloc byte[64];
                    byte* buffer32 = stackalloc byte[32];
                    //uint[] k64 = new uint[64];
                    //byte* block64 = stackalloc byte[64];
                    byte* pad64 = stackalloc byte[64];
                    byte* b64 = stackalloc byte[64];
                    Hash256Generator.toHash256(stream, buffer32, (bufPtr + (4 * 64)), (uint*)bufPtr,
                         pad64, b64, pad2, w64i);

                    return Hash256.BytesToHash256(buffer32);
                }
                else
                {
                    uint* w64i = stackalloc uint[64];
                    byte* pad2 = stackalloc byte[64];
                    byte* buffer32 = stackalloc byte[32];
                    //uint[] k64 = new uint[64];
                    byte* block64 = stackalloc byte[64];
                    byte* pad64 = stackalloc byte[64];
                    byte* b64 = stackalloc byte[64];
                    Hash256Generator.toHash256(stream, buffer32, block64, (uint*)bufPtr,
                         pad64, b64, pad2, w64i);
                    return Hash256.BytesToHash256(buffer32);
                }

            }

        }

        /// <summary>
        /// 读取一个4字节整数，后续补0
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns>true表示此次有数据可读，false表示已经到头无法读取</returns>
        static bool read32(Stream stream, out ulong value)
        {
            value = 0;
            int re;
            //1字节

            re = stream.ReadByte();
            if(re == -1)
            {
                return false;
            }
            value |= (((byte)re));

            //2字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8));
            
            //3字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 2));

            //4字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 3));

            //5字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 4));

            //6字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 5));

            //7字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 6));

            //8字节
            re = stream.ReadByte();
            if (re == -1)
            {
                return true;
            }
            value |= (((ulong)re) << (8 * 7));


            return true;
        }

        static void f_initEncBuf32(byte* buffer32)
        {
            const ulong A = 0x67452301;
            const ulong B = 0xEFCDAB89;
            const ulong C = 0x98BADCFE;
            const ulong D = 0x10325476;
            const ulong H = 0xC3D2E1F0;

            const ulong HxBCD = B ^ C ^ D;
            const ulong Fx = ~(((B ^ C)) ^ ((~B) ^ D));
            const ulong I = C ^ (B & ~D);

            const ulong s1 = (A | (B << 32)) ^ (HxBCD & ~(I));
            const ulong s2 = (B | (C << 32)) % ~(H ^ I);
            const ulong s3 = (C | (D << 32)) ^ (Fx);
            const ulong s4 = (D | (A << 32)) ^ ~(D ^ HxBCD);

            s1.OrderToBytes(buffer32);
            s2.OrderToBytes(buffer32 + (sizeof(ulong) * 1));
            s3.OrderToBytes(buffer32 + (sizeof(ulong) * 2));
            s4.OrderToBytes(buffer32 + (sizeof(ulong) * 3));
        }

        static void f_encBuf32(byte* buffer32, ulong value)
        {
            //byte* bptr32 = stackalloc byte[32];
            //f_initEncBuf32(bptr32);
            const ulong A = 0x67452301;
            const ulong B = 0xEFCDAB89;
            const ulong C = 0x98BADCFE;
            const ulong D = 0x10325476;
            //const ulong s1 = H0 | (H1 << 32);
            //const ulong s2 = H2 | (H3 << 32);
            //const ulong s3 = H4 | (H0 << 32);
            //const ulong s4 = H3 | (H1 << 32);

            const ulong HxBCD = B ^ C ^ D;

            const ulong Fx = ~(((B ^ C)) ^ ((~B) ^ D));

            const ulong I = C ^ (B & ~D);

            ulong* ip = (ulong*)buffer32;

            ref ulong rb1 = ref ip[0];
            ref ulong rb2 = ref ip[1];
            ref ulong rb3 = ref ip[2];
            ref ulong rb4 = ref ip[3];
            ulong re1, re2, re3, re4;

            re1 = B + (((A + Fx + rb3) ^ value) << (8 * 0));
            re2 = B + (((A + Fx + rb4) ^ value) << (8 * 1));
            re3 = B + (((A + Fx + rb1) ^ value) << (8 * 2));
            re4 = B + (((A + Fx + rb2) ^ value) << (8 * 3));

            //re1 = A + ~((HxBCD ^ (~rb4)) ^ value);
            //re2 = A + ~((Fx + value) ^ rb1);
            //re3 = A + ~(I ^ (~rb2) & value);
            //re4 = A + ((~Fx ^ ~(I - value)) ^ rb3);

            rb1 = re1;
            rb2 = re2;
            rb3 = re3;
            rb4 = re4;

            //ulong* Lp = (ulong*)bptr32;
            //uint* ip = (uint*)buffer32;

            //ulong* ipbuf = (ulong*)buffer32;
            //ipbuf[0] ^= Lp[0];
            //ipbuf[1] ^= Lp[1];
            //ipbuf[2] ^= Lp[2];
            //ipbuf[3] ^= Lp[3];
            //s1.OrderToByteArray(buffer32, 0);
            //s2.OrderToByteArray(buffer32, sizeof(ulong) * 1);
            //s3.OrderToByteArray(buffer32, sizeof(ulong) * 2);
            //s4.OrderToByteArray(buffer32, sizeof(ulong) * 3);
        }

        static void f_encBuf32(byte* buffer32, byte value)
        {
            //byte* bptr32 = stackalloc byte[32];
            //f_initEncBuf32(bptr32);
            const ulong A = 0x67452301;
            const ulong B = 0xEFCDAB89;
            const ulong C = 0x98BADCFE;
            const ulong D = 0x10325476;
            //const ulong s1 = H0 | (H1 << 32);
            //const ulong s2 = H2 | (H3 << 32);
            //const ulong s3 = H4 | (H0 << 32);
            //const ulong s4 = H3 | (H1 << 32);

            const ulong HxBCD = B ^ C ^ D;

            const ulong Fx = ~(((B ^ C)) ^ ((~B) ^ D));

            const ulong I = C ^ (B & ~D);

            ulong* ip = (ulong*)buffer32;

            ref ulong rb1 = ref ip[0];
            ref ulong rb2 = ref ip[1];
            ref ulong rb3 = ref ip[2];
            ref ulong rb4 = ref ip[3];
            ulong re1, re2, re3, re4;

            const ulong shx = (A + ~(Fx) ^ I);

            re1 = (shx * rb1 + HxBCD) ^ value;
            re2 = (shx * rb2 + HxBCD) ^ value;
            re3 = (shx * rb3 + HxBCD) ^ value;
            re4 = (shx * rb4 + HxBCD) ^ value;

            //re1 = B + (((shx ^ ((rb3 * value) + (rb1))) + (rb1 * (value + rb4)) & ~((value + 2UL) * HxBCD) + ( (rb2) ^ (~(value | I)) )) << (8 * 0));

            //re2 = B + (((HxBCD | I) ^ (re1)) << (8 * 1));
            //re3 = B + (((Fx + A) ^ (re2)) << (8 * 2));
            //re4 = B + ( ( ( (~shx) & I) ^ (re3)) << (8 * 3) );

            //re1 = A + ~((HxBCD ^ (~rb4)) ^ value);
            //re2 = A + ~((Fx + value) ^ rb1);
            //re3 = A + ~(I ^ (~rb2) & value);
            //re4 = A + ((~Fx ^ ~(I - value)) ^ rb3);

            rb1 = re1;
            rb2 = re2;
            rb3 = re3;
            rb4 = re4;
        }

        #endregion

        #endregion

        public static class Path
        {

            /// <summary>
            /// 清理指定目录下的所有空文件夹
            /// </summary>
            /// <remarks>
            /// <para>检测<paramref name="directoryInfo"/>下的子文件夹，如果存在空文件夹则将其从硬盘删除；在删除子文件夹后如果父文件夹同样成为了空文件夹也会被删除，直到目录<paramref name="directoryInfo"/>为止</para>
            /// <para>参数<paramref name="directoryInfo"/>即使是空文件夹也不会被删除</para>
            /// </remarks>
            /// <param name="directoryInfo">指定目录，已确保存在目录</param>
            /// <exception cref="ArgumentNullException">参数是null</exception>
            /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
            /// <exception cref="IOException">IO错误</exception>
            /// <exception cref="SecurityException">没有权限</exception>
            public static void ClearEmptyDirectory(DirectoryInfo directoryInfo)
            {
                if (directoryInfo is null) throw new ArgumentNullException();
                if (!directoryInfo.Exists) throw new DirectoryNotFoundException();

                f_clearEmptyDire(directoryInfo);
            }

            private static void f_clearEmptyDire(DirectoryInfo info)
            {
                foreach (DirectoryInfo subDir in info.GetDirectories())
                {
                    f_clearEmptyDire(subDir); // 递归处理子目录

                    // 处理完子目录后检查是否存在及空状态
                    if (subDir.Exists && f_isDireEmpty(subDir))
                    {
                        try
                        {
                            subDir.Delete(); // 删除空目录
                        }
                        catch (Exception)
                        {
                        }
                       
                    }
                }
            }

            static bool f_isDireEmpty(DirectoryInfo info)
            {
                try
                {
                    var arr = info.GetDirectories("*", SearchOption.TopDirectoryOnly);
                    var fs = info.GetFiles("*", SearchOption.TopDirectoryOnly);
                    return arr.Length == 0 && fs.Length == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }

        #endregion

    }

}
