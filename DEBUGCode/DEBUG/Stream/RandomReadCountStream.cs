using System;
using System.IO;
using System.Threading;

using Cheng.Algorithm.Randoms;
using Cheng.Streams;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 不稳定流读写模拟器
    /// </summary>
    /// <remarks>在调用<see cref="Read(byte[], int, int)"/>时模拟读取不稳定状态，每次读取会读取不同的字节量</remarks>
    public class RandomReadCountStream : HEStream
    {
        /// <summary>
        /// 初始化封装
        /// </summary>
        /// <param name="stream">封装流</param>
        /// <param name="random">随机器</param>
        public RandomReadCountStream(Stream stream, BaseRandom random)
        {
            p_stream = stream;
            p_random = random;
            p_rwMinMS = 0;
            p_rwMaxMS = 10;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stream">封装流</param>
        /// <param name="random">随机器</param>
        /// <param name="minWaitMS">每次读取最小等待毫秒数</param>
        /// <param name="maxWaitMS">每次读取最大等待毫秒数</param>
        public RandomReadCountStream(Stream stream, BaseRandom random, int minWaitMS, int maxWaitMS)
        {
            if (minWaitMS >= maxWaitMS) throw new ArgumentOutOfRangeException();
            p_stream = stream;
            p_random = random;
            p_rwMinMS = minWaitMS;
            p_rwMaxMS = maxWaitMS;
        }

        private Stream p_stream;
        private BaseRandom p_random;

        private int p_rwMaxMS;
        private int p_rwMinMS;

        #region 派生

        public override bool CanRead => p_stream.CanRead;

        public override bool CanSeek => p_stream.CanSeek;

        public override bool CanWrite => p_stream.CanWrite;

        public override long Length => p_stream.Length;

        public override long Position
        {
            get => p_stream.Position;
            set => p_stream.Position = value;
        }

        public override void Flush()
        {
            p_stream.Flush();
        }

        protected override bool Disposing(bool disposing)
        {
            if (disposing)
            {
                p_stream.Close();
            }

            return true;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIsDispose();
            return p_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            p_stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            int t;
            if(count > 1 && count < int.MaxValue)
            {
                t = p_random.Next(1, count + 1);
            }
            else
            {
                t = count;
            }
            var wait = p_random.Next(p_rwMinMS, p_rwMaxMS);
            if(wait >= 0) Thread.Sleep(wait);
            return p_stream.Read(buffer, offset, t);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowIsDispose();
            p_stream.Write(buffer, offset, count);
        }

        public override int ReadByte()
        {
            ThrowIsDispose();
            var re = p_stream.ReadByte();
            if (re < 0) return -1;
            var wait = p_random.Next(p_rwMinMS, p_rwMaxMS);
            if (wait >= 0) Thread.Sleep(wait);
            return re;
        }

        public override void WriteByte(byte value)
        {
            ThrowIsDispose();
            p_stream.WriteByte(value);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 内部封装的随机器
        /// </summary>
        public BaseRandom Random
        {
            get => p_random;
        }

        #endregion

    }

}
