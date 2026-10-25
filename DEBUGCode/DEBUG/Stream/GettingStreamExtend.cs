using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Cheng.DataStructure;
using Cheng.Streams;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 流对象获取扩展
    /// </summary>
    public static class DEBUGGettingStreamExtend
    {

        #region 封装

        private sealed class f_gettingNullStream : IGettingStream
        {
            internal f_gettingNullStream() {}
            public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.Unknown;
            public long StreamLength => 0;
            public Stream OpenStream() => Stream.Null;

        }

        private sealed class f_gettingFixedBlockStream : IGettingStream
        {
            internal f_gettingFixedBlockStream(byte value, long size)
            {
                p_value = value;
                p_size = size;
            }

            private long p_size;
            private byte p_value;

            public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.RuntimeBytes;
            public long StreamLength => p_size;

            public Stream OpenStream()
            {
                return new FixedByteStream(p_value, p_size);
            }

        }

        #endregion

        /// <summary>
        /// 获取一个无数据流对象打开器
        /// </summary>
        /// <returns></returns>
        public static IGettingStream GetNullStreamGetter()
        {
            return new f_gettingNullStream();
        }

        /// <summary>
        /// 获取一个读取指定字节量的流对象打开器
        /// </summary>
        /// <param name="value">字节值</param>
        /// <param name="size">可读取的字节量大小</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">字节大小没有大于0</exception>
        public static IGettingStream GetBlockStreamGetter(byte value, long size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException();
            return new f_gettingFixedBlockStream(value, size);
        }

    }

}
