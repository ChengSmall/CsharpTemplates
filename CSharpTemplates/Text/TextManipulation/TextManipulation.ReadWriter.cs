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

        #endregion

        #endregion


    }

}
