using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Memorys;
using System.IO;
using Cheng.DataStructure.Texts;

namespace Cheng.Texts
{

    public unsafe static partial class TextManipulation
    {

        #region 读写器

        #region 拷贝

        /// <summary>
        /// 将读取器的文本读取并写入到另一个文本写入器中
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <param name="writer">待写入文本的写入器</param>
        /// <param name="buffer">文本读取缓冲区，容量必须大于0</param>
        /// <exception cref="ArgumentNullException">参数有null对象</exception>
        /// <exception cref="ArgumentException">缓冲区大小为空</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void CopyToText(this TextReader reader, TextWriter writer, char[] buffer)
        {
            if (reader is null || writer is null || buffer is null)
            {
                throw new ArgumentNullException();
            }

            int length = buffer.Length;

            if (length == 0) throw new ArgumentException();

            Loop:
            var re = reader.Read(buffer, 0, length);
            if (re == 0) return;
            writer.Write(buffer, 0, re);
            goto Loop;
        }

        /// <summary>
        /// 将读取器的文本读取并写入到另一个文本写入器中
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <param name="writer">待写入文本的写入器</param>
        /// <exception cref="ArgumentNullException">参数有null对象</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void CopyToText(this TextReader reader, TextWriter writer)
        {
            CopyToText(reader, writer, new char[1024 * 2]);
        }

        #endregion

        #region 读写扩展

        /// <summary>
        /// 从读取器读取一个32位字符，并将读取器推进到合适的位置
        /// </summary>
        /// <param name="reader">文本读取器</param>
        /// <param name="value">读取的值</param>
        /// <returns>是否成功读取，如果成功读取返回true；如果已到达末尾无法读取出一个字符返回false</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static bool ReadUnichar(this TextReader reader, out Unichar value)
        {
            if (reader is null) throw new ArgumentNullException();
            value = default;
            var re = reader.Read();
            if (re == -1) return false;

            var hi = (char)re;

            re = reader.Peek();
            if (re == -1)
            {
                value = new Unichar(hi);
            }
            else
            {
                char low = (char)re;
                if(char.IsSurrogatePair(hi, low))
                {
                    value = new Unichar(hi, low);
                    reader.Read();
                }
                else
                {
                    value = new Unichar(hi);
                }
            }

            return true;
        }

        /// <summary>
        /// 将一个32位字符写入到文本写入器
        /// </summary>
        /// <param name="writer">文本写入器</param>
        /// <param name="value">要写入的字符</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void WriteUnichar(this TextWriter writer, Unichar value)
        {
            if (writer is null) throw new ArgumentNullException();
            writer.Write(value.high);
            if (value.IsSurrogatePair())
            {
                writer.Write(value.low);
            }
        }

        /// <summary>
        /// 从读取器读取文本并写入字符串缓冲区
        /// </summary>
        /// <param name="reader">要从中读取字符的读取器</param>
        /// <param name="sb">要将读取的字符写入的字符串缓冲区</param>
        /// <returns>成功读取的字符数量，0表示已到达末尾</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static int ReadToBuffer(this TextReader reader, CMStringBuilder sb)
        {
            if (reader is null || sb is null) throw new ArgumentNullException();

            if (sb.Capacity == sb.Length)
            {
                sb.Capacity = sb.Capacity * 2;
            }
            var cbuf = sb.GetCharBuffer();
            var buf = cbuf.Array;
            var re = reader.Read(buf, cbuf.Count, buf.Length - cbuf.Count);
            if (re == 0) return 0;
            sb.OnlySetLength(sb.Length + re);
            return re;
        }

        /// <summary>
        /// 将字符串写入文本写入器
        /// </summary>
        /// <param name="writer">要写入文本的写入器对象</param>
        /// <param name="sb">准备写入到<paramref name="writer"/>的字符串</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void WriteByBuffer(this TextWriter writer, CMStringBuilder sb)
        {
            if (writer is null || sb is null) throw new ArgumentNullException();
            var cbuf = sb.GetCharBuffer();
            writer.Write(cbuf.Array, cbuf.Offset, cbuf.Count);
        }

        /// <summary>
        /// 将字符串写入文本写入器
        /// </summary>
        /// <param name="writer">要写入文本的写入器对象</param>
        /// <param name="sb">准备写入到<paramref name="writer"/>的字符串缓冲区</param>
        /// <param name="index">指定<paramref name="sb"/>的起始位置</param>
        /// <param name="count">要写入的长度</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public static void WriteByBuffer(this TextWriter writer, CMStringBuilder sb, int index, int count)
        {
            if (writer is null || sb is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > sb.Length))
            {
                throw new ArgumentOutOfRangeException();
            }

            var cbuf = sb.GetCharBuffer();
            writer.Write(cbuf.Array, index, count);
        }

        #endregion

        #endregion


    }

}
