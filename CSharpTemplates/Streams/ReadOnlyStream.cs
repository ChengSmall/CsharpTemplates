using System;
using System.IO;
using System.Text;

namespace Cheng.Streams
{

    public class ReadOnlyStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化只读流
        /// </summary>
        /// <param name="stream">一个可读取的流对象</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public ReadOnlyStream(Stream stream) : this(stream, true)
        {
        }

        /// <summary>
        /// 实例化只读流
        /// </summary>
        /// <param name="stream">一个可读取的流对象</param>
        /// <param name="onDispose">在释放时是否释放封装的对象；默认为true</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        /// <exception cref="NotSupportedException">流没有读取权限</exception>
        public ReadOnlyStream(Stream stream, bool onDispose)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));
            p_read = stream.CanRead;
            p_seek = stream.CanSeek;
            if (!p_read) throw new NotSupportedException();
            p_stream = stream;
            p_onDispose = onDispose;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 内部封装的对象
        /// </summary>
        protected Stream p_stream;
        private bool p_read;
        private bool p_seek;
        private bool p_onDispose;

        #endregion

        #region 派生

        public override bool CanRead => p_read;

        public override bool CanSeek => p_seek;

        public override bool CanWrite => false;

        public override bool CanTimeout => (p_stream?.CanTimeout).GetValueOrDefault();

        public override void Flush()
        {
            p_stream?.Flush();
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

        public override int ReadByte()
        {
            ThrowIsDispose();
            return p_stream.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        protected override bool Disposing(bool disposing)
        {
            if (disposing && p_onDispose)
            {
                p_stream.Close();
            }
            p_stream = null;
            return true;
        }

        public override long Length
        {
            get
            {
                ThrowIsDispose();
                return p_stream.Length;
            }
        }

        public override long Position 
        {
            get
            {
                ThrowIsDispose();
                return p_stream.Position;
            }
            set
            {
                ThrowIsDispose();
                p_stream.Position = value;
            }
        }

        #endregion

    }

}
