using System.IO;

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
        }
        
        private Stream p_stream;
        private BaseRandom p_random;

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

        protected override void DisposeUnmanaged()
        {
            p_stream.Close();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return p_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            p_stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int t;
            if(count > 1 && count < int.MaxValue)
            {
                t = p_random.Next(1, count + 1);
            }
            else
            {
                t = count;
            }
            return p_stream.Read(buffer, offset, t);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            p_stream.Write(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return p_stream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            p_stream.WriteByte(value);
        }

    }

}
