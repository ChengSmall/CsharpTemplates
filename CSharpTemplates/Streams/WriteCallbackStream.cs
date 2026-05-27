using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using Cheng.Memorys;
using Cheng.IO;
using Cheng.DataStructure;

namespace Cheng.Streams
{

    /// <summary>
    /// 写入时回调的只写流
    /// </summary>
    public sealed class WriteCallbackStream : HEStream
    {

        #region

        /// <summary>
        /// 每次写入一个字节都会调用一次的回调函数
        /// </summary>
        /// <param name="value">此次写入的字节</param>
        public delegate void WriteFunction(byte value);

        #endregion

        #region
        
        /// <summary>
        /// 实例化一个写入回调流
        /// </summary>
        /// <param name="function">在写入数据时调用的回调函数</param>
        public WriteCallbackStream(WriteFunction function)
        {
            p_func = function ?? throw new ArgumentNullException();
        }

        #endregion

        #region

        private WriteFunction p_func;

        #endregion

        #region 派生

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override bool CanTimeout => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        protected override bool Disposing(bool disposing)
        {
            if (disposing)
            {
                p_func = null;
            }
            return true;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            if (buffer is null) throw new ArgumentNullException();
            int length = offset + count;
            if (length > buffer.Length) throw new ArgumentOutOfRangeException();
            for (int i = offset; i < length; i++)
            {
                p_func.Invoke(buffer[i]);
            }
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            p_func.Invoke(value);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        #endregion

    }

}
