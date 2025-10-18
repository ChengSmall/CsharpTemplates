using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.Streams
{

    /// <summary>
    /// 封装一个流并控制是否对非托管资源释放
    /// </summary>
    /// <remarks>
    /// 该类仅用于控制封装的流是否释放非托管资源；
    /// <para>针对某些第三方库，使用<see cref="Stream"/>类型真的是一塌糊涂！竟然将自身api的句柄或资源释放和流封装对象绑定到一起，不释放会导致句柄残留在进程内，释放则必定会调用内部封装的<see cref="Stream.Close"/>，完全没有提供不做内部释放的开关，作为一个C#代码库简直太过分！！！这个类专门给这些离谱的库擦屁股，以控制流对象无法被内部释放</para>
    /// </remarks>
    public class RegulateDisposeStream : HEStream
    {

        #region 构造

        /// <summary>
        /// 实例化控制释放的流并使其禁止释放
        /// </summary>
        /// <param name="stream">封装的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public RegulateDisposeStream(Stream stream) : this(stream, false)
        {
        }

        /// <summary>
        /// 实例化控制释放的流
        /// </summary>
        /// <param name="stream">封装的流</param>
        /// <param name="onDispose">
        /// 是否在释放资源时释放内部封装对象；如果参数为true，则不论释放代码是否处于终结器线程，都会调用内部封装的<see cref="Stream.Close"/>；如果为false，则无论如何也不会调用内部封装的<see cref="Stream.Close"/>；该参数默认为false
        /// </param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public RegulateDisposeStream(Stream stream, bool onDispose)
        {
            this.stream = stream ?? throw new ArgumentNullException();
            this.onDispose = onDispose;
            p_canRead = stream.CanRead;
            p_canSeek = stream.CanSeek;
            p_canWrite = stream.CanWrite;
            p_canTimeout = stream.CanTimeout;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 内部封装的流
        /// </summary>
        private Stream stream;

        private readonly bool p_canRead, p_canSeek, p_canWrite, p_canTimeout;

        /// <summary>
        /// 在释放资源时是否释放内部封装的流
        /// </summary>
        public readonly bool onDispose;

        public override bool CanRead => p_canRead;

        public override bool CanSeek => p_canSeek;

        public override bool CanWrite => p_canWrite;

        public override long Length
        {
            get
            {
                ThrowIsDispose();
                return stream.Length;
            }
        }

        public override long Position 
        {
            get
            {
                ThrowIsDispose();
                return stream.Position;
            }
            set
            {
                ThrowIsDispose();
                stream.Position = value;
            }
        }

        public override bool CanTimeout => p_canTimeout;

        public override int ReadTimeout 
        { 
            get
            {
                ThrowIsDispose();
                return stream.ReadTimeout;
            }
            set
            {
                ThrowIsDispose();
                stream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout 
        {
            get
            {
                ThrowIsDispose();
                return stream.WriteTimeout;
            }
            set
            {
                ThrowIsDispose();
                stream.WriteTimeout = value;
            } 
        }

        /// <summary>
        /// 获取内部封装的流
        /// </summary>
        /// <returns>如果已释放则返回null</returns>
        public Stream BaseStream
        {
            get => stream;
        }

        #endregion

        #region 派生

        protected override bool Disposing(bool disposing)
        {
            if (onDispose && disposing)
            {
                stream.Close();
            }
            stream = null;
            return true;
        }

        public sealed override void Flush()
        {
            stream?.Flush();
        }

        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return stream.Seek(offset, origin);
        }

        public sealed override void SetLength(long value)
        {
            ThrowIsDispose();
            stream.SetLength(value);
        }

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            return stream.Read(buffer, offset, count);
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            stream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIsDispose();
            return stream.BeginRead(buffer, offset, count, callback, state);
        }

        public sealed override int EndRead(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            return stream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            ThrowIsDispose();
            return stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public sealed override void EndWrite(IAsyncResult asyncResult)
        {
            ThrowIsDispose();
            stream.EndWrite(asyncResult);
        }

        public sealed override int ReadByte()
        {
            ThrowIsDispose();
            return stream.ReadByte();
        }

        public sealed override void WriteByte(byte value)
        {
            ThrowIsDispose();
            stream.WriteByte(value);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            ThrowIsDispose();
            return stream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            ThrowIsDispose();
            return stream.FlushAsync(cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIsDispose();
            return stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override unsafe int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose();
            if (stream is HEStream hs) return hs.ReadToAddress(buffer, count);
            return base.ReadToAddress(buffer, count);
        }

        public override unsafe void WriteToAddress(byte* buffer, int count)
        {
            ThrowIsDispose();
            if (stream is HEStream hs) hs.WriteToAddress(buffer, count);
            else base.WriteToAddress(buffer, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ThrowIsDispose();
            return stream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        #endregion

    }

}
