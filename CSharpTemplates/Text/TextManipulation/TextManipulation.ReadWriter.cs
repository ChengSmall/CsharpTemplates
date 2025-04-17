using System;
using System.Collections.Generic;
using System.Text;
using Cheng.Memorys;
using System.IO;

namespace Cheng.Texts
{

    public unsafe static partial class TextManipulation
    {


        #region 读写器

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

            lock (buffer)
            {
                Loop:
                var re = reader.Read(buffer, 0, length);
                if (re == 0) return;
                writer.Write(buffer, 0, re);
                goto Loop;
            }
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


    }

}
