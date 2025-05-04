using Cheng.DataStructure.Hashs;
using Cheng.IO;
using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cheng.DEBUG
{

    public static class DEBUGStreamExd
    {

        #region 流数据拷贝

        /// <summary>
        /// 将流的数据拷贝到指定的文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath">要拷贝到的文件</param>
        public static void CopyToFile(this Stream stream, string filePath)
        {

            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.CopyTo(file, 1024 * 16);
            }

        }

        /// <summary>
        /// 将流的数据拷贝添加到指定的文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath">要添加数据的文件</param>
        public static void AppendToFile(this Stream stream, string filePath)
        {

            using (FileStream file = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                stream.CopyTo(file);
            }

        }

        /// <summary>
        /// 从指定文件读取数据到流
        /// </summary>
        /// <param name="stream">要读取到的流</param>
        /// <param name="filePath">要读取的文件所在路径</param>
        /// <returns>拷贝到流的字节数</returns>
        public static long FileCopyToStream(this Stream stream, string filePath)
        {
            byte[] buffer = new byte[1024 * 16];
            long size = 0;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {              
                foreach (var item in file.CopyToStreamEnumator(stream, buffer))
                {
                    size += item;
                }               
            }
            return size;
        }

        #endregion

        #region foreach

        /// <summary>
        /// 读取流数据并遍历打印到字符串中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fen">分隔符</param>
        /// <param name="lineCount">多少字节后换行</param>
        /// <param name="toByteStr">从byte到字符串的转化方法</param>
        /// <returns>遍历后的字符串</returns>
        public static string ForeachStream(this Stream stream, string fen, int lineCount, Func<byte, string> toByteStr)
        {
            if (stream is null) throw new ArgumentNullException();
            if(toByteStr is null)
            {
                toByteStr = DEBUGTEST.ToX16;
            }
            StringBuilder sb = new StringBuilder(64);
            long count = 0;
            

            while (true)
            {
                int re = stream.ReadByte();

                if(re == -1)
                {
                    return sb.ToString();
                }

                sb.Append(toByteStr.Invoke(((byte)re)));
                sb.Append(fen);
                count++;

                if (count % lineCount == 0)
                {
                    sb.AppendLine();
                }
            }

        }

        /// <summary>
        /// 遍历流数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>遍历后的打印字符串</returns>
        public static string ForeachStream(this Stream stream)
        {
            return ForeachStream(stream, " ", 16, null);
        }

        /// <summary>
        /// 将流的所有数据读取
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>包含流的所有数据</returns>
        public static byte[] ReadAll(this Stream stream)
        {
            byte[] buffer;
            if (stream.CanSeek && stream.Length < int.MaxValue)
            {
                buffer = new byte[stream.Length];

                var re = stream.ReadBlock(buffer, 0, buffer.Length);
                if (re == buffer.Length) return buffer;
                byte[] res = new byte[re];
                Array.Copy(buffer, 0, res, 0, re);
                return res;
            }

            MemoryStream ms = new MemoryStream(1024);

            buffer = new byte[1024 * 4];

            stream.CopyToStream(ms, buffer);

            if(ms.TryGetBuffer(out var arrBuf))
            {
                if (arrBuf.Offset == 0 && arrBuf.Count == arrBuf.Array.Length) return arrBuf.Array;
            }

            return ms.ToArray();
        }

        #endregion

        #region Hash256

        /// <summary>
        /// 获取指定文件的Hash256
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static Hash256 ToHash256ByFile(this string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                return file.ToHash256();
            }
        }

        #endregion

        #region timeTest

        /// <summary>
        /// 测试读取完流的耗时
        /// </summary>
        /// <param name="stream">测试的流</param>
        /// <param name="timer">计时器</param>
        /// <param name="buffer">用于读取流的缓冲区，缓冲区大小决定一次读取多少字节</param>
        /// <returns>一次性读取完毕的耗时</returns>
        public static TimeSpan TimeTestStreamAll(this Stream stream, Stopwatch timer, byte[] buffer)
        {
            timer.Reset();
            int count = buffer.Length;
            int r;

            timer.Restart();
            do
            {
                r = stream.Read(buffer, 0, count);
                
            } while (r != 0);

            timer.Stop();

            return timer.Elapsed;
        }

        /// <summary>
        /// 测试流平均读取速度
        /// </summary>
        /// <param name="stream">要测试的流</param>
        /// <param name="timer">计时器</param>
        /// <param name="buffer">读取时的缓冲区</param>
        /// <param name="readSize">读取的总字节数</param>
        /// <returns>读取总耗时</returns>
        public static TimeSpan TimeTestStreamOnce(this Stream stream, Stopwatch timer, byte[] buffer, out long readSize)
        {
            timer.Reset();
            int count = buffer.Length;
            int r;

            readSize = 0;

            while (true)
            {
                timer.Start();
                r = stream.Read(buffer, 0, count);
                timer.Stop();
                
                if(r == 0)
                {
                    break;
                }
                readSize += r;
            }

            return timer.Elapsed;
        }

        #endregion

        #region 信息

        /// <summary>
        /// 获取流对象的信息
        /// </summary>
        /// <param name="stream">流对象</param>
        /// <param name="append">输出的信息</param>
        public static void GetInformation(this Stream stream, StringBuilder append)
        {
            if (stream is null || append is null) throw new ArgumentNullException();

            bool canRead = stream.CanRead;
            bool canSeek = stream.CanSeek;
            bool canWrite = stream.CanWrite;
            bool canTimeout = stream.CanTimeout;

            append.Append("流类型:");
            append.Append(stream.GetType().AssemblyQualifiedName);
            if((!canRead) && (!canSeek) && (!canWrite))
            {
                append.AppendLine();
                append.Append("无法进行任何操作的流");
                return;
            }
            append.AppendLine();
            append.Append("访问权限:");
            if (canRead)
            {
                append.Append("可读");
            }
            if (canWrite)
            {
                if (canRead) append.Append(',');
                append.Append("可写");
            }
            if (canSeek)
            {
                if (canRead || canWrite) append.Append(',');
                append.Append("可查");
            }
            if (canTimeout)
            {
                if (canRead || canWrite || canSeek) append.Append(',');
                append.Append("可超时");
            }

            if (canSeek)
            {
                append.AppendLine();
                var len = stream.Length;
                var pos = stream.Position;
                append.Append("当前位置:");
                append.Append(pos);
                append.Append(' ');
                append.Append("流长度:");
                append.Append(len);
            }

        }

        /// <summary>
        /// 获取流对象的信息
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>输出的信息</returns>
        public static string GetInformation(this Stream stream)
        {
            StringBuilder sb = new StringBuilder(32);
            GetInformation(stream, sb);
            return sb.ToString();
        }

        #endregion

    }

}
