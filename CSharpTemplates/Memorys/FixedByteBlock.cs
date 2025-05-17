using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cheng.Memorys
{

    /// <summary>
    /// 固定位置的字节块数据
    /// </summary>
    public sealed unsafe class FixedByteBlock : ReleaseDestructor
    {

        #region 构造

        /// <summary>
        /// 创建一个固定位置的字节块
        /// </summary>
        /// <param name="byteSize">字节大小</param>
        /// <exception cref="ArgumentOutOfRangeException">长度小于0</exception>
        public FixedByteBlock(int byteSize)
        {
            if (byteSize < 0) throw new ArgumentOutOfRangeException();
            p_buffer = new byte[byteSize];
            if(byteSize == 0)
            {
                p_gc = default;
                p_ptr = null;
            }
            else
            {
                p_gc = GCHandle.Alloc(p_buffer, GCHandleType.Pinned);
                p_ptr = (byte*)p_gc.AddrOfPinnedObject();
            }

            p_length = byteSize;
        }

        /// <summary>
        /// 使用已有的字节块拷贝一个新的固定位置的字节块
        /// </summary>
        /// <param name="bytes">字节块</param>
        /// <param name="offset">要拷贝的起始位置</param>
        /// <param name="byteSize">要拷贝的字节长度</param>
        /// <exception cref="ArgumentNullException">字节块是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0或指定范围超出拷贝字节块范围</exception>
        public FixedByteBlock(byte[] bytes, int offset, int byteSize)
        {
            if (bytes is null) throw new ArgumentNullException();
            if(offset < 0 || byteSize < 0 || (byteSize + offset) > bytes.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            if(offset == 0 && byteSize == bytes.Length)
            {
                p_buffer = (byte[])bytes.Clone();
            }
            else
            {
                p_buffer = new byte[byteSize];
                Array.Copy(bytes, offset, p_buffer, 0, byteSize);
            }

            if (byteSize == 0)
            {
                p_gc = default;
                p_ptr = null;
            }
            else
            {
                p_gc = GCHandle.Alloc(p_buffer, GCHandleType.Pinned);
                p_ptr = (byte*)p_gc.AddrOfPinnedObject();
            }

            p_length = byteSize;
        }

        #endregion

        #region 参数

        private GCHandle p_gc;
        private byte[] p_buffer;
        private byte* p_ptr;
        private int p_length;

        #endregion

        #region 功能

        #region 回收

        protected override bool Disposeing(bool disposeing)
        {
            if (p_ptr != null)
            {
                p_gc.Free();
            }
            if (disposeing)
            {
                p_buffer = null;
            }
            p_ptr = null;
            p_gc = default;
            p_length = -1;
            return true;
        }

        #endregion

        #region 访问

        /// <summary>
        /// 字节块首地址，如果已释放返回null；如果长度为0也返回null
        /// </summary>
        public byte* Pointer
        {
            get => p_ptr;
        }

        /// <summary>
        /// 字节块长度，如果已释放返回-1
        /// </summary>
        public int Length
        {
            get => p_length;
        }

        /// <summary>
        /// 访问内部固定的字节块对象
        /// </summary>
        public byte[] Block
        {
            get
            {
                ThrowObjectDisposeException();
                return p_buffer;
            }
        }

        #endregion

        #region 索引访问

        /// <summary>
        /// 按字节偏移访问或设置字节块数据
        /// </summary>
        /// <param name="offset">字节偏移值</param>
        /// <returns>指定偏移下的字节</returns>
        /// <exception cref="ArgumentOutOfRangeException">偏移超出范围</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public byte this[int offset]
        {
            get
            {
                ThrowObjectDisposeException();
                if (offset < 0 || offset >= p_length) throw new ArgumentOutOfRangeException();
                return p_ptr[offset];
            }
            set
            {
                ThrowObjectDisposeException();
                if (offset < 0 || offset >= p_length) throw new ArgumentOutOfRangeException();
                p_ptr[offset] = value;
            }
        }

        #endregion

        #endregion

    }

}
