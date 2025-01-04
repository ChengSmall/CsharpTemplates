using Cheng.Memorys;
using System;
using System.IO;
using System.Text;

namespace Cheng.Streams
{

    /// <summary>
    /// 封装一个流作为二进制读写流
    /// </summary>
    public unsafe class BinaryStream : HEStream
    {

        #region 构造
        /// <summary>
        /// 实例化二进制读写流
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <param name="isFree">在释放资源时是否释放封装的基础流，该参数默认为true</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BinaryStream(Stream stream, bool isFree)
        {
            if (stream is null) throw new ArgumentNullException();
            f_init(stream, isFree);
        }
        /// <summary>
        /// 实例化二进制读写流
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public BinaryStream(Stream stream) : this(stream, true)
        {
        }
        private void f_init(Stream stream, bool isFree)
        {
            p_stream = stream;
            p_structureBuffer = new byte[16];
            p_isFree = isFree;
        }
        #endregion

        #region 释放

        protected override bool Disposing(bool disposing)
        {
            if (p_isFree && disposing)
            {
                p_stream.Close();
            }

            return true;
        }

        #endregion

        #region 参数
        protected Stream p_stream;
        private byte[] p_structureBuffer;
        private bool p_isFree;
        #endregion

        #region 功能

        #region 封装
        private void f_capacityBuffer(ref byte[] buffer, int size)
        {
            int length = buffer.Length;
            if (length >= size) return;
            int ns = length * 2;
            if (ns < size) ns = size;
            buffer = new byte[ns];
        }
        #endregion

        /// <summary>
        /// 获取内部封装的流
        /// </summary>
        public Stream BaseStream => p_stream;

        /// <summary>
        /// 将指定对象写入流并提升对象大小个字节的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">要写入的对象</param>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="NotSupportedException">没有写入权限</exception>
        public void Write<T>(T value) where T : unmanaged
        {
            ThrowIsDispose();
            f_capacityBuffer(ref p_structureBuffer, sizeof(T));

            fixed (byte* ptr = p_structureBuffer)
            {
                (*(T*)ptr) = value;
            }

            p_stream.Write(p_structureBuffer, 0, sizeof(T));
        }

        /// <summary>
        /// 读取指定非托管类型的对象并提升对象大小个字节的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">读取到的对象</param>
        /// <returns>是否成功读取；流读取的字节数等于变量大小返回true；若无法读取到该变量大小返回false</returns>
        /// <param name="value">要写入的对象</param>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        /// <exception cref="NotSupportedException">没有写入权限</exception>
        public bool Read<T>(out T value) where T : unmanaged
        {
            ThrowIsDispose();
            value = default;
            f_capacityBuffer(ref p_structureBuffer, sizeof(T));
            
            int r = p_stream.ReadBlock(p_structureBuffer, 0, sizeof(T));

            if (r != sizeof(T)) return false;

            value = p_structureBuffer.ToStructure<T>(0);
            return true;
        }

        #region 派生

        public override bool CanRead => p_stream.CanRead;

        public override bool CanSeek => p_stream.CanSeek;

        public override bool CanWrite => p_stream.CanWrite;

        public override long Length => p_stream.Length;

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
        public override long Position
        {
            get => p_stream.Position;
            set => p_stream.Position = value;
        }

        public override void Flush()
        {
            p_stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return p_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ThrowIsDispose();
            p_stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            return p_stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            p_stream.Write(buffer, offset, count);
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            return p_stream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            p_stream.WriteByte(value);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIsDispose();
            return p_stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIsDispose();
            return p_stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            return p_stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            p_stream.EndWrite(asyncResult);
        }


        #endregion

        #endregion

    }

}
