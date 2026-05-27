using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime;
using System.Reflection;
using System.Resources;

namespace Cheng.Windows.Forms
{

    public static unsafe class ControlExtend
    {

        #region TextBox

        /// <summary>
        /// 获取文本框当前光标所在的行数
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns>文本框当前光标所在的行数，第一行是0，第二行是1，以此类推；若没有开启多行输入则是0</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static int GetCursorLineCount(this TextBox textBox)
        {
            if (textBox is null) throw new ArgumentNullException();

            if (!textBox.Multiline) return 0;

            var text = textBox.Text;

            if (text.Length == 0) return 0;

            int selectStart = textBox.SelectionStart;

            int lineCount = 0;
            for (int i = 0; i < selectStart; )
            {
                int rei = text.IndexOf('\n', i, selectStart - i);
                if(rei < 0)
                {
                    break;
                }
                lineCount++;
                i = rei + 1;
            }

            return lineCount;
        }

        #endregion

    }

}
