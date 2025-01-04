using Cheng.Memorys;
using System;
using System.IO;

namespace Cheng.Algorithm.Encryptions
{
    /// <summary>
    /// 一个异或加密对象，使用简单的异或密钥进行加密或解密
    /// </summary>
    public unsafe class XOREncryption
    {

        #region 构造
        /// <summary>
        /// 实例化一个异或加密对象
        /// </summary>
        /// <param name="key">32为密钥</param>
        public XOREncryption(int key)
        {
            byte[] bs = new byte[sizeof(int)];
            key.ToByteArray(bs, 0);
            f_init(bs, 1024);
        }
        /// <summary>
        /// 实例化一个异或加密对象
        /// </summary>
        /// <param name="key">64为密钥</param>
        public XOREncryption(long key)
        {
            byte[] bs = new byte[sizeof(long)];
            key.ToByteArray(bs, 0);
            f_init(bs, 1024);
        }
        /// <summary>
        /// 实例化一个异或加密对象，指定密钥
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public XOREncryption(byte[] key)
        {
            f_tinit(key, 1024);
        }

        /// <summary>
        /// 实例化一个异或加密对象，指定密钥和最大缓冲区长度
        /// </summary>
        /// <param name="key">异或加密密钥</param>
        /// <param name="bufferMaxSize">最大缓冲区长度，默认为1024</param>
        /// <exception cref="ArgumentException">缓冲区长度小于或等于0或密钥为空</exception>
        /// <exception cref="ArgumentNullException">密钥为null</exception>
        public XOREncryption(byte[] key, int bufferMaxSize)
        {
            if (key is null) throw new ArgumentNullException("key");
            if (key.Length == 0 || bufferMaxSize <= 0) throw new ArgumentException();

            p_key = key;
            p_buffer = new byte[bufferMaxSize];
        }
        private void f_tinit(byte[] key, int bufferMaxSize)
        {
            if (key is null) throw new ArgumentNullException("key");
            if (key.Length == 0 || bufferMaxSize <= 0) throw new ArgumentException();

            p_key = key;
            p_buffer = new byte[bufferMaxSize];
            p_index = 0;
        }
        private void f_init(byte[] key, int bufferMaxSize)
        {
            p_key = key;
            p_buffer = new byte[bufferMaxSize];
            p_index = 0;
        }

        #endregion

        #region 参数
        private int p_index;
        private byte[] p_key;
        private byte[] p_buffer;
        #endregion

        #region 功能
        /// <summary>
        /// 获取密钥的字节数
        /// </summary>
        public int Length => p_key.Length;
        /// <summary>
        /// 重新设置密钥
        /// </summary>
        /// <param name="key">新的密钥</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">密钥字节长度是0</exception>
        public void SetKey(byte[] key)
        {
            if (key is null) throw new ArgumentNullException();
            if (key.Length == 0) throw new ArgumentException();

            p_key = key;
        }

        /// <summary>
        /// 获取或设置密钥指针
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">值不得超过密钥数组的最大索引</exception>
        public int KeyPointer
        {
            get => p_index;
            set
            {
                if (value < 0 || value >= p_key.Length) throw new ArgumentOutOfRangeException();
                p_index = value;
            }
        }

        /// <summary>
        /// 获取要异或的字节值，并推进指针
        /// </summary>
        /// <returns>要异或的字节值</returns>
        public byte NextIndex()
        {
            return nextIndex();
        }
        /// <summary>
        /// 获取要异或的字节值，不会推进指针位置
        /// </summary>
        /// <returns>要异或的字节值</returns>
        public byte PeekIndex()
        {
            return p_key[p_index];
        }
        /// <summary>
        /// 对密钥指针添加偏移
        /// </summary>
        /// <param name="offset">要添加的偏移</param>
        public void AddPointer(int offset)
        {
            p_index += offset % p_key.Length;
        }
        private byte nextIndex()
        {
            byte b = p_key[p_index++];
            if (p_index == p_key.Length) p_index = 0;
            return b;
        }
        private void enc(byte[] buffer, int index, int count)
        {
            int i;
            int end = index + count;
            for (i = index; i < end; i++)
            {
                buffer[i] ^= nextIndex();
            }

        }

        /// <summary>
        /// 将流数据进行异或加密到另一个流当中
        /// </summary>
        /// <param name="encStream">要加密的原数据</param>
        /// <param name="toStream">加密到的目标流</param>
        /// <param name="reset">是否在加密后重置加密密钥指针</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void Encry(Stream encStream, Stream toStream, bool reset)
        {

            if (encStream is null || toStream is null) throw new ArgumentNullException();

            Loop:
            //读取
            int r = encStream.Read(p_buffer, 0, p_buffer.Length);

            if (r == 0)
            {
                if(reset) p_index = 0;
                return;
            }
            //加密
            enc(p_buffer, 0, r);

            //写入
            encStream.Write(p_buffer, 0, r);
            goto Loop;
        }
        /// <summary>
        /// 将流数据进行异或加密到另一个流当中，加密后重置密钥指针
        /// </summary>
        /// <param name="encStream">要加密的原数据</param>
        /// <param name="toStream">加密到的目标流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void Encry(Stream encStream, Stream toStream)
        {
            Encry(encStream, toStream, true);
        }

        /// <summary>
        /// 将字节数组进行异或加密计算
        /// </summary>
        /// <param name="buffer">要加密的数组</param>
        /// <param name="offset">数组的起始位置</param>
        /// <param name="count">要加密的字符</param>
        /// <param name="reset">是否在加密后重置加密密钥指针</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public void Encry(byte[] buffer, int offset, int count, bool reset)
        {
            if (buffer is null) throw new ArgumentNullException("buffer");
            if (offset + count > buffer.Length) throw new ArgumentOutOfRangeException();

            enc(buffer, offset, count);
            if (reset) Reset();
        }
        /// <summary>
        /// 将字节数组进行异或加密计算，加密后重置加密密钥指针
        /// </summary>
        /// <param name="buffer">要加密的数组</param>
        /// <param name="offset">数组的起始位置</param>
        /// <param name="count">要加密的字符</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数范围超出</exception>
        public void Encry(byte[] buffer, int offset, int count)
        {
            Encry(buffer, offset, count, true);
        }
        /// <summary>
        /// 重置密钥指针位置
        /// </summary>
        public void Reset()
        {
            p_index = 0;
        }

        #endregion

    }

    

}
