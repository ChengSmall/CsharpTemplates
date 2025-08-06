using Cheng.Consoles;
using Cheng.DataStructure.Colors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Texts.Consoles
{

    /// <summary>
    /// 封装控制台样式转义字符的文本写入器
    /// </summary>
    /// <remarks>
    /// <para>通过修改参数，对象在写入文本先后写入样式转义字符以达到定制文本样式效果，需要先调用<see cref="Cheng.Consoles.ConsoleSystem.EnableVirtualTerminalProcessingOnWindows"/>开启虚拟终端</para>
    /// </remarks>
    public sealed class ConsoleStyleTextWriter : SafeReleaseTextWriter
    {

        #region

        /// <summary>
        /// 实例化一个控制台样式转义字符写入器
        /// </summary>
        /// <param name="writer">要封装的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public ConsoleStyleTextWriter(TextWriter writer)
        {
            p_writer = writer ?? throw new ArgumentNullException();
            p_enc = p_writer.Encoding;
            f_init();
        }

        private void f_init()
        {
            //p_allToggle = true;
            p_style_color = null;
        }

        #endregion

        #region 参数

        private TextWriter p_writer;
        private Encoding p_enc;
        private Colour? p_style_color;

        #endregion

        #region 功能

        public override Encoding Encoding => p_enc;

        /// <summary>
        /// 写入文本时的颜色样式
        /// </summary>
        /// <value>写入文本时的颜色样式；设为null表示不添加颜色样式，该参数默认为null</value>
        public Colour? ColorStyle
        {
            get => p_style_color;
            set
            {
                ThrowObjectDisposed();
                p_style_color = value;
            }
        }

        #endregion

        #region 派生

        protected override bool Disposing(bool disposeing)
        {
            if (disposeing)
            {
                p_writer.Close();
            }
            p_writer = null;
            return true;
        }

        private void f_writerHeader()
        {
            if (p_style_color.HasValue)
            {
                var c = p_style_color.Value;
                ConsoleTextStyle.ColorToText(c.r, c.g, c.b, false, p_writer);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            f_writerHeader();
            p_writer.Write(buffer, index, count);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void Write(char[] buffer)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            f_writerHeader();
            p_writer.Write(buffer);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void Write(char value)
        {
            ThrowObjectDisposed();
            f_writerHeader();
            p_writer.Write(value);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void Write(string value)
        {
            ThrowObjectDisposed();
            if (value is null) return;
            f_writerHeader();
            p_writer.Write(value);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void WriteLine()
        {
            ThrowObjectDisposed();
            p_writer.WriteLine();
        }

        public override void WriteLine(char value)
        {
            ThrowObjectDisposed();
            f_writerHeader();
            p_writer.WriteLine(value);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            ThrowObjectDisposed();
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            f_writerHeader();
            p_writer.WriteLine(buffer, index, count);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void WriteLine(char[] buffer)
        {
            ThrowObjectDisposed();
            f_writerHeader();
            p_writer.WriteLine(buffer);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override void WriteLine(string value)
        {
            ThrowObjectDisposed();
            if (value is null)
            {
                p_writer.WriteLine();
                return;
            }
            f_writerHeader();
            p_writer.WriteLine(value);
            p_writer.Write(ConsoleTextStyle.ResetStyleText);
        }

        public override string NewLine 
        {
            get => p_writer?.NewLine;
            set
            {
                if (p_writer != null) p_writer.NewLine = value;
            }
        }

        #endregion

    }

}
