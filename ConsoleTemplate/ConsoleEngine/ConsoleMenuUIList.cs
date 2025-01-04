using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Cheng.DataStructure.Colors;

namespace Cheng.Consoles.ConsoleUI
{

    /// <summary>
    /// 控制台菜单UI场景
    /// </summary>
    /// <remarks>
    /// <para>向控制台打印自定义文本列表的UI</para>
    /// </remarks>
    public sealed class ConsoleMenuUIList
    {

        #region 构造

        /// <summary>
        /// 实例化一个控制台菜单UI场景
        /// </summary>
        public ConsoleMenuUIList()
        {
            p_drawBuf = new StringBuilder();
            p_butList = new List<ConsoleTextUIButton>();
            p_selectIndex = 0;
            p_buttonInterval = 1;
            p_selectValue = SelectValueDefault;
            p_buttonLateText = LateButtonTextDefault;
            p_selectValueColor = new Colour(255, 255, 255);
            p_startSelectColor = false;
        }

        #endregion

        #region 参数

        private StringBuilder p_drawBuf;

        private List<ConsoleTextUIButton> p_butList;

        private string p_selectValue;

        public string p_buttonLateText;

        private int p_buttonInterval;

        private int p_selectIndex;

        private Colour p_selectValueColor;

        private bool p_startSelectColor;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置选择标识颜色，仅在参数<see cref="BeginSelectColor"/>为true时有效
        /// </summary>
        private Colour SelectValueColor
        {
            get => p_selectValueColor;
            set
            {
                p_selectValueColor = value;
            }
        }

        /// <summary>
        /// 选择标识是否使用<see cref="ConsoleTextStyle"/>的颜色文本，默认为false
        /// </summary>
        public bool BeginSelectColor
        {
            get => p_startSelectColor;
            set
            {
                p_startSelectColor = value;
            }
        }

        /// <summary>
        /// 默认样式的选择标识
        /// </summary>
        public const string SelectValueDefault = "=>";

        /// <summary>
        /// 按钮集合
        /// </summary>
        /// <value>
        /// <para>该集合从第一项开始，从上到下依次绘制集合内的按钮，按钮为null时忽略该项跳到下一项</para>
        /// </value>
        public List<ConsoleTextUIButton> Buttons
        {
            get => p_butList;
        }

        /// <summary>
        /// 表示UI当前选择的按钮
        /// </summary>
        /// <value>
        /// <para>设置一个集合索引，表示当前选择的按钮，在绘制时会在指定位置按钮的侧边绘制出选择标识</para>
        /// <para>若值超出索引范围，则不绘制选择标识，因此若不想显示标识，可以将值设为-1</para>
        /// </value>
        public int SelectButtonIndex
        {
            get => p_selectIndex;
            set
            {
                p_selectIndex = value;
            }
        }

        /// <summary>
        /// 按钮间上下空行的行数
        /// </summary>
        /// <value>默认为1</value>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public int ButtonInterval
        {
            get => p_buttonInterval;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                p_buttonInterval = value;
            }
        }

        /// <summary>
        /// 表示选择标识的字符串，在绘制选择标识时将使用该字符串打印
        /// </summary>
        /// <value>设为空则不会绘制，默认为<see cref="SelectValueDefault"/></value>
        public string SelectValue
        {
            get => p_selectValue;
            set
            {
                p_selectValue = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 获取当前选择的按钮，若没有按钮被选中则为null
        /// </summary>
        public ConsoleTextUIButton SelectButton
        {
            get
            {
                var index = p_selectIndex;
                if (index < 0 || index >= p_butList.Count) return null;
                return p_butList[index];
            }
        }

        /// <summary>
        /// 空行文本
        /// </summary>
        /// <value>为了使按钮居中，在打印按钮之前要填充的表示空行的文本</value>
        public string LateButtonText
        {
            get => p_buttonLateText;
            set
            {
                p_buttonLateText = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 默认的空行文本
        /// </summary>
        public const string LateButtonTextDefault = "                    ";

        #endregion

        #region 绘制

        /// <summary>
        /// 将当前的UI字符文本添加到写入器
        /// </summary>
        /// <param name="append">文本写入器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void AppendUIText(TextWriter append)
        {
            if (append is null) throw new ArgumentNullException();

            var list = p_butList;

            int length = list.Count;

            var sei = p_selectIndex;

            var bitl = ButtonInterval;
            //bool beginColor = BeginSelectColor;

            int end = length - 1;
            for (int i = 0; i < length; i++)
            {
                
                var late = LateButtonText;

                if (i == sei)
                {
                    //要打印选择器
                    var sev = SelectValue;
                    p_drawBuf.Clear();
                    
                    p_drawBuf.Append(late, 0, late.Length - sev.Length);
                    var bgs = BeginSelectColor;
                    if (bgs)
                    {
                        var selc = SelectValueColor;
                        p_drawBuf.AppendANSIColorText(selc.r, selc.g, selc.b, false);
                    }
                    p_drawBuf.Append(sev);
                    if (bgs)
                    {
                        p_drawBuf.AppendANSIResetColorText();
                    }
                    append.Write(p_drawBuf.ToString());
                }
                else
                {
                    append.Write(late);
                }

                var but = list[i];
                if(but is null)
                {
                    goto ButtonOver;
                }

                if (but.StartButtonColor)
                {
                    var color = but.TextColor;
                    ConsoleTextStyle.ColorToText(color.r, color.g, color.b, false, append);
                }

                append.Write(but.Text);

                if (but.StartButtonColor)
                {
                    append.Write(ConsoleTextStyle.ResetColorStyleText);
                }

                ButtonOver:

                //换行一次
                append.WriteLine();
                if(i != end)
                {
                    for (int line = 0; line < bitl; line++)
                    {
                        append.WriteLine();
                    }
                }

            }

        }

        /// <summary>
        /// 将当前的UI字符文本添加到缓冲区
        /// </summary>
        /// <param name="append">字符缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void AppendUIText(StringBuilder append)
        {
            if (append is null) throw new ArgumentNullException();

            var list = p_butList;

            int length = list.Count;

            var sei = p_selectIndex;

            var bitl = ButtonInterval;
            int end = length - 1;
            for (int i = 0; i < length; i++)
            {

                var late = LateButtonText;

                if (i == sei)
                {
                    //要打印选择器
                    var sev = SelectValue;
                    
                    append.Append(late, 0, late.Length - sev.Length);
                    var bgs = BeginSelectColor;
                    if (bgs)
                    {
                        var selc = SelectValueColor;
                        append.AppendANSIColorText(selc.r, selc.g, selc.b, false);
                    }
                    append.Append(sev);
                    if (bgs)
                    {
                        append.AppendANSIResetColorText();
                    }
                    //append.Write(p_drawBuf.ToString());
                }
                else
                {
                    append.Append(late);
                }

                var but = list[i];
                if (but is null)
                {
                    goto ButtonOver;
                }

                if (but.StartButtonColor)
                {
                    var color = but.TextColor;
                    ConsoleTextStyle.ColorToText(color.r, color.g, color.b, false, append);
                }

                append.Append(but.Text);

                if (but.StartButtonColor)
                {
                    append.Append(ConsoleTextStyle.ResetColorStyleText);
                }

                ButtonOver:

                //换行一次
                append.AppendLine();
                if (i != end)
                {
                    for (int line = 0; line < bitl; line++)
                    {
                        append.AppendLine();
                    }
                }

            }

        }

        /// <summary>
        /// 返回将当前的UI字符文本
        /// </summary>
        /// <returns></returns>
        public string ToUIText()
        {
            var swr = new StringBuilder(128);
            AppendUIText(swr);
            return swr.ToString();
        }

        /// <summary>
        /// 将当前的UI字符文本打印到控制台
        /// </summary>
        public void DrawToConsole()
        {
            p_drawBuf.Clear();
            AppendUIText(p_drawBuf);
            Console.Write(p_drawBuf.ToString());
        }

        #endregion

        #endregion

    }

}
