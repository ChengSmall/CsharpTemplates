using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.DEBUG
{

    /// <summary>
    /// 对多个文本写入器整合操作
    /// </summary>
    public class TextMultWriter : TextWriter
    {

        #region

        public TextMultWriter(IEnumerable<TextWriter> writers)
        {
            p_wrs = new List<TextWriter>(writers);
        }

        #endregion

        #region 参数

        private List<TextWriter> p_wrs;

        public override Encoding Encoding
        {
            get
            {
                foreach (var item in p_wrs)
                {
                    if (item?.Encoding != null) return item.Encoding;
                }
                return null;
            }
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                foreach (var item in p_wrs)
                {
                    if (item?.FormatProvider != null) return item.FormatProvider;
                }
                return null;
            }
        }

        public override string NewLine
        {
            get
            {
                foreach (var item in p_wrs)
                {
                    if (item?.NewLine != null) return item.NewLine;
                }
                return null;
            }
            set
            {
                foreach (var item in p_wrs)
                {
                    if(item != null) item.NewLine = value;
                }
            } 
        }

        #endregion

        #region 派生

        public override void Flush()
        {
            foreach (var item in p_wrs)
            {
                item?.Flush();
            }
        }

        public override void Close()
        {
            foreach (var item in p_wrs)
            {
                item?.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Write(char value)
        {
            foreach (var item in p_wrs)
            {
                item?.Write(value);
            }
        }

        public override void Write(char[] buffer)
        {
            foreach (var item in p_wrs)
            {
                item?.Write(buffer);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            foreach (var item in p_wrs)
            {
                item?.Write(buffer, index, count);
            }
        }

        public override void Write(string format, params object[] arg)
        {
            foreach (var item in p_wrs)
            {
                item?.Write(format, arg);
            }
        }

        public override void WriteLine()
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine();
            }
        }

        public override void WriteLine(char value)
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine(value);
            }
        }

        public override void WriteLine(char[] buffer)
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine(buffer);
            }
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine(buffer, index, count);
            }
        }

        public override void WriteLine(string value)
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine(value);
            }
        }

        public override void WriteLine(string format, params object[] arg)
        {
            foreach (var item in p_wrs)
            {
                item?.WriteLine(format, arg);
            }
        }

        #endregion

    }
}
