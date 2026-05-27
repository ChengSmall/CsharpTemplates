using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using strBuf = Cheng.Texts.CMStringBuilder;

namespace Cheng.Texts
{

    /// <summary>
    /// 字符串缓冲区对象<see cref="Cheng.Texts.CMStringBuilder"/>文本写入器
    /// </summary>
    public sealed unsafe class CMStringBuilderWriter : SafeReleaseTextWriter
    {

        #region 构造

        /// <summary>
        /// 实例化字符串缓冲区写入器
        /// </summary>
        public CMStringBuilderWriter() : this(new strBuf())
        {
        }

        /// <summary>
        /// 实例化字符串缓冲区写入器
        /// </summary>
        /// <param name="formatProvider">控制格式的对象</param>
        public CMStringBuilderWriter(IFormatProvider formatProvider) : this(new strBuf(), formatProvider)
        {
        }

        /// <summary>
        /// 实例化字符串缓冲区写入器
        /// </summary>
        /// <param name="stringBuilder">指定写入到的对象</param>
        /// <exception cref="ArgumentNullException">对象为null</exception>
        public CMStringBuilderWriter(strBuf stringBuilder)
        {
            p_buf = stringBuilder ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 实例化字符串缓冲区写入器
        /// </summary>
        /// <param name="stringBuilder">指定写入到的对象</param>
        /// <param name="formatProvider">控制格式的对象</param>
        /// <exception cref="ArgumentNullException">写入目标对象为null</exception>
        public CMStringBuilderWriter(strBuf stringBuilder, IFormatProvider formatProvider) : base(formatProvider)
        {
            p_buf = stringBuilder ?? throw new ArgumentNullException();
        }

        #endregion

        #region

        private strBuf p_buf;

        #endregion

        #region 派生

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char value)
        {
            p_buf.Append(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            p_buf.Append(buffer, index, count);
        }

        public override void Write(char[] buffer)
        {
            p_buf.Append(buffer);
        }

        public override void Write(string value)
        {
            p_buf.Append(value);
        }

        public override void WriteLine()
        {
            p_buf.Append(CoreNewLine);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            p_buf.Append(buffer, index, count);
            p_buf.Append(CoreNewLine);
        }

        public override void WriteLine(char[] buffer)
        {
            p_buf.Append(buffer);
            p_buf.Append(CoreNewLine);
        }

        public override void WriteLine(string value)
        {
            p_buf.Append(value);
            p_buf.Append(CoreNewLine);
        }

        #endregion

        #region 功能

        /// <summary>
        /// 获取内部的写入目标对象
        /// </summary>
        public strBuf StringBuffer
        {
            get => p_buf;
        }

        #endregion

    }

}
