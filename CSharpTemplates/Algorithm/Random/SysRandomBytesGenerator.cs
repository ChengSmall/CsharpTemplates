using System;
using System.Security.Cryptography;

using Cheng.Memorys;

namespace Cheng.Algorithm.Randoms
{

    /// <summary>
    /// 使用系统提供的安全随机数生成器生成随机序列
    /// </summary>
    public sealed unsafe class SysRandomBytesGenerator : SafreleaseUnmanagedResources, IRandomGenerateBytes
    {

        #region 构造

        /// <summary>
        /// 实例化一个默认的随机数生成器封装
        /// </summary>
        public SysRandomBytesGenerator() : this(null, true, 1024 * 8)
        {}

        /// <summary>
        /// 实例化一个随机数生成器封装
        /// </summary>
        /// <param name="generator">要使用的随机数生成器，null使用<see cref="RandomNumberGenerator.Create"/>返回的对象</param>
        public SysRandomBytesGenerator(RandomNumberGenerator generator) : this(generator, true, 1024 * 8)
        {}

        /// <summary>
        /// 实例化一个随机数生成器封装
        /// </summary>
        /// <param name="generator">要使用的随机数生成器，null使用<see cref="RandomNumberGenerator.Create"/>返回的对象</param>
        /// <param name="disposeing">在释放时是否释放随机数生成器对象</param>
        public SysRandomBytesGenerator(RandomNumberGenerator generator, bool disposeing) : this(generator, disposeing, 1024 * 8)
        {}

        /// <summary>
        /// 实例化一个随机数生成器封装
        /// </summary>
        /// <param name="generator">要使用的随机数生成器，null使用<see cref="RandomNumberGenerator.Create"/>返回的对象</param>
        /// <param name="disposeing">在释放时是否释放随机数生成器对象</param>
        /// <param name="bufferSize">随机字节缓冲区大小</param>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        public SysRandomBytesGenerator(RandomNumberGenerator generator, bool disposeing, int bufferSize)
        {
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            p_rng = generator ?? RandomNumberGenerator.Create();
            p_buffer = new byte[bufferSize];
            p_ofs = p_buffer.Length;
            p_disposed = disposeing;
            p_genc = 0;
        }


        #endregion

        #region 参数

        private RandomNumberGenerator p_rng;

        private byte[] p_buffer;

#if DEBUG
        /// <summary>
        /// 缓存已使用偏移
        /// </summary>
#endif
        private int p_ofs;

        private int p_genc;

        private bool p_disposed;

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                if (p_disposed) p_rng.Dispose();
            }
            p_rng = null;
            return true;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 填充随机序列到指定地址
        /// </summary>
        /// <param name="ptr">要填充到的位置</param>
        /// <param name="length">要填充的字节数量</param>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public void NextAddress(byte* ptr, int length)
        {
            ThrowObjectDisposeException(nameof(SysRandomBytesGenerator));
            if (length <= 0) return;

            lock (p_buffer)
            {
                fixed (byte* bptr = p_buffer)
                {
                    while (length > 0)
                    {
                        // 重新填充缓存
                        if (p_ofs >= p_buffer.Length)
                        {
                            p_rng.GetBytes(p_buffer);
                            p_ofs = 0;
                            p_genc++;
                        }

                        // 本次最多复制多少字节
                        int copyLength = Math.Min(length, (p_buffer.Length - p_ofs));

                        // 从缓存复制到目标指针
                        Buffer.MemoryCopy(bptr + p_ofs, ptr, copyLength, copyLength);

                        // 更新数据
                        p_ofs += copyLength;
                        ptr += copyLength;
                        length -= copyLength;
                    }
                }
            }

        }

        public void NextPtr(CPtr<byte> ptr, int length)
        {
            if (ptr.IsEmpty) throw new ArgumentNullException();
            NextAddress(ptr, length);
        }

        /// <summary>
        /// 填充随机序列到指定字节数组
        /// </summary>
        /// <param name="buffer">要填充的数组对象</param>
        /// <param name="offset">要填充的起始偏移</param>
        /// <param name="length">要填充的字节长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">超出范围</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public void NextBytes(byte[] buffer, int offset, int length)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (offset < 0 || length < 0 || (offset + length > buffer.Length)) throw new ArgumentOutOfRangeException();

            fixed (byte* bptr = buffer)
            {
                NextAddress(bptr + offset, length);
            }
        }

        /// <summary>
        /// 填充随机序列到指定字节数组
        /// </summary>
        /// <param name="buffer">要填充的数组对象</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public void NextBytes(byte[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();
            fixed (byte* bptr = buffer)
            {
                NextAddress(bptr, buffer.Length);
            }
        }

        #endregion

    }

}
