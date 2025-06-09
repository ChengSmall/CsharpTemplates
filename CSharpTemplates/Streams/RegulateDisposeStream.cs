using System;
using System.Collections.Generic;
using System.IO;

namespace Cheng.Streams
{

    /// <summary>
    /// 封装一个流并强制控制是否对非托管资源释放
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
        }

        #endregion

        #region 参数

        /// <summary>
        /// 内部封装的流
        /// </summary>
        public readonly Stream stream;

        /// <summary>
        /// 在释放资源时是否释放内部封装的流
        /// </summary>
        public readonly bool onDispose;

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        public override long Length => stream.Length;

        public override long Position 
        { 
            get => stream.Position; 
            set => stream.Position = value;
        }

        public override bool CanTimeout => base.CanTimeout;

        public override int ReadTimeout 
        { 
            get => stream.ReadTimeout;
            set => stream.ReadTimeout = value;
        }

        public override int WriteTimeout 
        { 
            get => stream.WriteTimeout; 
            set => stream.WriteTimeout = value; 
        }

        #endregion

        #region 派生

        protected override void DisposeUnmanaged()
        {
        }

        protected override bool Disposing(bool disposing)
        {
            if (onDispose)
            {
                stream.Close();
            }
            return true;
        }

        public sealed override void Flush()
        {
            stream.Flush();
        }

        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public sealed override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return stream.BeginRead(buffer, offset, count, callback, state);
        }

        public sealed override int EndRead(IAsyncResult asyncResult)
        {
            return stream.EndRead(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public sealed override void EndWrite(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
        }

        public sealed override int ReadByte()
        {
            return stream.ReadByte();
        }

        public sealed override void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        #endregion

    }

    /// <summary>
    /// 封装一个流并强制控制对非托管资源的释放
    /// </summary>
    /// <remarks>使用对象终结器（析构函数）以保证对象释放</remarks>
    public class DtorRegulateDisposeStream : RegulateDisposeStream
    {

        /// <summary>
        /// 实例化控制释放的流并使其禁止释放
        /// </summary>
        /// <param name="stream">封装的流</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public DtorRegulateDisposeStream(Stream stream) : base(stream, false)
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
        public DtorRegulateDisposeStream(Stream stream, bool onDispose) : base(stream, onDispose)
        {
        }

        /// <summary>
        /// 使用对象终结器以保证在托管实例回收前释放流
        /// </summary>
        ~DtorRegulateDisposeStream()
        {
            Dispose(false);
        }

    }

}
