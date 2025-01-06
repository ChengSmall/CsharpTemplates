using Cheng.Streams;
using System;
using System.IO;
using System.Text;

namespace Cheng.Algorithm.Encryptions
{

    /// <summary>
    /// 封装一个异或加密流，在读写时自动加密和解密数据
    /// </summary>
    public sealed class XORStreamEncry : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XORStreamEncry(Stream stream, int key)
        {
            f_tinit(stream, new XOREncryption(key), true);
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XORStreamEncry(Stream stream, long key)
        {
            f_tinit(stream, new XOREncryption(key), true);
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥字符串，使用utf-8编码为加密数据</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="EncoderFallbackException">发生回退</exception>
        public XORStreamEncry(Stream stream, string key) : this(stream, key, Encoding.UTF8, true)
        {
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥字符串</param>
        /// <param name="encoding">将字符串解码为字节序列的编码器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="EncoderFallbackException">发生回退</exception>
        public XORStreamEncry(Stream stream, string key, Encoding encoding) : this(stream, key, encoding, true)
        {
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥字符串</param>
        /// <param name="encoding">将字符串解码为字节序列的编码器</param>
        /// <param name="isDispose">释放时是否一并清理封装对象资源，true表示清理，false不清理；默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="EncoderFallbackException">发生回退</exception>
        public XORStreamEncry(Stream stream, string key, Encoding encoding, bool isDispose)
        {
            f_tinit(stream, new XOREncryption(encoding?.GetBytes(key)), isDispose);
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="key">异或加密密钥</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XORStreamEncry(Stream stream, byte[] key)
        {
            f_tinit(stream, new XOREncryption(key), true);
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="xor">异或加密对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XORStreamEncry(Stream stream, XOREncryption xor)
        {
            f_tinit(stream, xor, true);
        }

        /// <summary>
        /// 实例化一个加密流封装
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="xor">异或加密对象</param>
        /// <param name="isDispose">释放时是否一并清理封装对象资源，true表示清理，false不清理；默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XORStreamEncry(Stream stream, XOREncryption xor, bool isDispose)
        {
            f_tinit(stream, xor, isDispose);
        }

        private void f_tinit(Stream stream, XOREncryption xor, bool free)
        {
            if (stream is null || xor is null) throw new ArgumentNullException();
            p_stream = stream;
            this.p_xor = xor;
            p_isFree = free;
        }

        #endregion

        #region 参数

        private XOREncryption p_xor;
        private Stream p_stream;

        private bool p_isFree;

        #endregion

        #region 参数访问

        /// <summary>
        /// 获取基础封装流对象
        /// </summary>
        public Stream BaseStream => p_stream;

        /// <summary>
        /// 获取基础异或加密对象
        /// </summary>
        public XOREncryption XOR => p_xor;

        public override bool CanRead => p_stream.CanRead;

        public override bool CanSeek => p_stream.CanSeek;

        public override bool CanWrite => p_stream.CanWrite;

        public override long Length => p_stream.Length;

        public override long Position
        {
            get => p_stream.Position;
            set
            {
                var pos = p_stream.Position;
                p_stream.Position = value;
                p_xor.AddPointer(value - pos);
            }
        }

        public override bool CanTimeout => p_stream.CanTimeout;

        public override int ReadTimeout
        {
            get => p_stream.ReadTimeout;
            set => p_stream.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => p_stream.WriteTimeout;
            set => p_stream.WriteTimeout = value;
        }

        #endregion


        #region 派生

        /// <summary>
        /// 清除所有缓冲区并将所有数据写入到基础设备
        /// </summary>
        public override void Flush()
        {
            p_stream?.Flush();
        }

        /// <summary>
        /// 手动重置当前密钥指针
        /// </summary>
        public void Reset()
        {
            p_xor.Reset();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            int r = p_stream.Read(buffer, offset, count);

            p_xor.Encry(buffer, offset, r, false);

            return r;
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            int r = p_stream.ReadByte();

            if (r == -1) return -1;

            return (((byte)r) ^ p_xor.NextIndex());
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            p_xor.Encry(buffer, offset, count, false);
            p_stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            p_stream.WriteByte((byte)(value ^ p_xor.NextIndex()));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();

            var nowPos = p_stream.Position;

            var re = p_stream.Seek(offset, origin);

            p_xor.AddPointer(re - nowPos);

            return re;
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose();
            p_stream.SetLength(value);
        }

        protected override bool Disposing(bool disposing)
        {
            if (p_isFree && disposing)
            {
                p_stream.Close();
            }
            p_xor.Reset();
            p_stream = null;

            return true;
        }

        #endregion

    }
}
