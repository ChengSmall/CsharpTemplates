using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Texts
{

    /// <summary>
    /// 可对指定范围的字符串进行文本读取的读取器
    /// </summary>
    public sealed class StringRangeReader : SafeReleaseTextReader
    {

        #region 构造

        /// <summary>
        /// 实例化字符串范围读取器
        /// </summary>
        /// <param name="value">要读取的字符串</param>
        /// <param name="index">字符串指定起始索引</param>
        /// <param name="count">字符串指定范围长度</param>
        public StringRangeReader(string value, int index, int count)
        {
            if(value is null)
            {
                p_str = value;
                p_next = 0;
                p_index = 0;
                p_count = 0;
                p_isNull = true;
                return;
            }
            
            if (index < 0 || count < 0 || index + count > value.Length)
            {
                throw new ArgumentNullException();
            }
            p_isNull = false;
            p_str = value;
            p_index = index;
            p_next = index;
            p_count = count;
        }

        /// <summary>
        /// 实例化字符串范围读取器，使用全范围读取
        /// </summary>
        /// <param name="value">要读取的字符串</param>
        public StringRangeReader(string value)
        {
            p_str = value;
            p_index = 0;
            p_isNull = value is null;
            p_next = 0;
            if (p_isNull)
            {
                p_count = 0;
            }
            else
            {
                p_count = value.Length;
            }
            
        }

        #endregion

        #region 参数

        private string p_str;

        private int p_index;

        private int p_count;

        private bool p_isNull;

        private int p_next;

        #endregion

        #region 派生

        public override int Read()
        {
            ThrowObjectDisposed();
            if (p_next >= p_index + p_count) return -1;
            return p_str[p_next++];
        }

        public override int Peek()
        {
            ThrowObjectDisposed();
            if (p_next >= p_index + p_count) return -1;
            return p_str[p_next];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || index + count > buffer.Length) throw new ArgumentOutOfRangeException();
            if (p_isNull) return 0;

            //剩余可读数
            int nowCount;
            nowCount = (p_index + p_count - 1) - p_next;
            if (nowCount == 0 || count == 0) return 0;

            //获取要读取的数
            int rCount = Math.Min(nowCount, count);

            p_str.CopyTo(p_next, buffer, index, rCount);

            p_next += rCount;
            return rCount;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Read(buffer, index, count);
        }

        public override string ReadLine()
        {
            ThrowObjectDisposed();
            if (p_isNull) return null;
            if (p_next >= p_index + p_count) return null;
            var newLine = Environment.NewLine;
            int lc = newLine.Length;

            int end = p_index + p_count - 1;
            int i;
            int nc;            

            bool eq;

            for (i = p_next; i <= end; i++)
            {
                eq = true;
                for (nc = 0; nc < lc; nc++)
                {
                    int strIndex = i + nc;

                    if (strIndex > end)
                    {
                        //超出遍历索引
                        break;
                    }

                    if (newLine[nc] != p_str[strIndex])
                    {
                        //不相等
                        eq = false;
                    }
                }

                if (eq)
                {
                    //相等
                    break;
                }
            }

            //要截取的数
            int reCount = i - p_next;

            var str = p_str.Substring(p_next, reCount);

            p_next += reCount + lc;
            return str;
        }

        public override string ReadToEnd()
        {
            ThrowObjectDisposed();
            if (p_isNull) return string.Empty;
            var end = (p_index + p_count);
            var s = p_str.Substring(p_next, end - p_next);
            p_next = end;
            return s;
        }        

        #endregion

    }
}
