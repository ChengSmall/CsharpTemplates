
using Cheng.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 封装DEBUG测试流
    /// </summary>
    public sealed class DEBUGStream : HEStream
    {

        #region 构造

        public DEBUGStream(Stream stream)
        {
            p_stream = stream ?? throw new ArgumentNullException();
            p_print = Console.Out;
            init();
        }

        public DEBUGStream(Stream stream, TextWriter print)
        {
            p_stream = stream ?? throw new ArgumentNullException();
            p_print = print ?? Console.Out;
            init();
        }

        private void init()
        {
            p_count = 0;
            p_readCount = 0;
            p_seekCount = 0;
            p_writeCount = 0;
            p_buffer = new StringBuilder();
        }

        #endregion

        #region 参数

        private Stream p_stream;        
        private StringBuilder p_buffer;
        private TextWriter p_print;
        private long p_count;

        private uint p_readCount;
        private uint p_seekCount;
        private uint p_writeCount;

        private bool p_foreach = true;

        #endregion

        #region 派生

        #region

        public override bool CanRead
        {
            get
            {
                var re = p_stream.CanRead;
                p_print?.WriteLine("获取CanRead:" + re);
                return re;
            }
        }

        public override bool CanSeek
        {
            get
            {
                var re = p_stream.CanSeek;
                p_print?.WriteLine("获取CanSeek:" + re);
                return re;
            }
        }

        public override bool CanWrite
        {
            get
            {
                var re = p_stream.CanWrite;
                p_print?.WriteLine("获取CanWrite:" + re);
                return re;
            }
        }

        public override long Length
        {
            get
            {
                var re = p_stream.Length;
                p_print?.WriteLine("获取Length:" + re);
                return re;
            }
        }

        public override long Position
        {
            get
            {
                var re = p_stream.Position;
                p_print?.WriteLine("获取位置:" + re);
                return re;
            }
            set
            {
                p_print?.WriteLine("设置位置:" + value.ToString());
                p_stream.Position = value;
            }
        }

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

        public override bool CanTimeout => p_stream.CanTimeout;

        #endregion

        #region

        public override void Flush()
        {
            p_print?.WriteLine("Flush");
            p_stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var re = p_stream.Seek(offset, origin);
            p_print?.WriteLine(p_seekCount + " => " + re + " 调用Seek(" + offset + "," + origin + ");");
            p_count++;
            p_seekCount++;
            return re;
        }

        public override void SetLength(long value)
        {            
            p_stream.SetLength(value);
            p_print?.WriteLine(p_writeCount + ": => 调用SetLength(" + value.ToString() + ");");
            p_count++;
            p_writeCount++;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var re = p_stream.Read(buffer, offset, count);
            StringBuilder sb = p_buffer;
            sb.Clear();
            sb.AppendLine(("使用Read读取:" + re.ToString() + "字节:"));
            if (p_foreach)
            {
                int end = re - 1;
                for (int i = 0; i < re; i++)
                {
                    if (i != 0 && i % 10 == 0) sb.AppendLine();

                    sb.Append(buffer[i]);

                    if (i != end) sb.Append(" ");

                }
                sb.AppendLine();
            }
           
            sb.Append("------------------- ");
            sb.Append(p_readCount);
            sb.Append(" -------------------");
            p_count++;
            p_readCount++;
            p_print?.WriteLine(sb.ToString());
            return re;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            p_stream.Write(buffer, offset, count);
            StringBuilder sb = p_buffer;
            sb.Clear();
            sb.AppendLine(("使用Write写入:" + count.ToString() + "字节:"));
            if (p_foreach)
            {
                int end = count - 1;
                for (int i = 0; i < count; i++)
                {
                    if (i != 0 && i % 10 == 0) sb.AppendLine();

                    sb.Append(buffer[i]);

                    if (i != end) sb.Append(" ");

                }
                sb.AppendLine();
            }

            sb.Append("------------------- ");
            sb.Append(p_writeCount);
            sb.Append(" -------------------");
            p_count++;
            p_writeCount++;
            p_print?.WriteLine(sb.ToString());
        }

        public override int ReadByte()
        {
            int re = p_stream.ReadByte();
            p_print?.WriteLine(p_readCount + ": => 读取单字节:" + (re == -1 ? "[End]" : re.ToString()));
            p_count++;
            p_readCount++;
            return re;
        }

        public override void WriteByte(byte value)
        {
            p_stream.WriteByte(value);
            p_print?.WriteLine(p_writeCount + ": => 写入单字节:" + value.ToString());
            p_count++;
            p_writeCount++;
        }

        protected override bool Disposing(bool disposing)
        {
            if (disposing)
            {
                p_stream.Close();
            }
            return true;
        }

        /// <summary>
        /// 当前流的使用状态
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = p_buffer;

            sb.Clear();
            sb.AppendLine("当前流状态：");
            sb.Append("读取操作次数：");
            sb.AppendLine(p_readCount.ToString());
            sb.Append("写入操作次数:");
            sb.AppendLine(p_writeCount.ToString());
            sb.Append("查找操作次数：");
            sb.AppendLine(p_seekCount.ToString());
            sb.Append("封装流类型：");
            var t = p_stream.GetType();
            sb.Append(t.FullName);

            return sb.ToString();
        }

        #endregion

        #endregion

        #region 功能

        /// <summary>
        /// 是否遍历
        /// </summary>
        public bool IsForeach
        {
            get => p_foreach;
            set => p_foreach = value;
        }

        /// <summary>
        /// 执行读写或查找的总次数
        /// </summary>
        public long PrintCount
        {
            get => p_count;
        }

        /// <summary>
        /// 使用读取操作的次数
        /// </summary>
        public uint ReadCount
        {
            get => p_readCount;
        }

        /// <summary>
        /// 使用写入操作的次数
        /// </summary>
        public uint WriteCount
        {
            get => p_writeCount;
        }

        /// <summary>
        /// 使用查找操作的次数
        /// </summary>
        public uint SeekCount
        {
            get => p_seekCount;
        }

        /// <summary>
        /// 重置操作次数
        /// </summary>
        public void ClearCount()
        {
            p_count = 0;
            p_readCount = 0;
            p_seekCount = 0;
            p_writeCount = 0;
        }

        public Stream BaseStream => p_stream;

        /// <summary>
        /// 封装一个内存数据流到测试流
        /// </summary>
        /// <returns></returns>
        public static DEBUGStream CreateMS()
        {
            return new DEBUGStream(new MemoryStream());
        }

        #endregion

    }

}
