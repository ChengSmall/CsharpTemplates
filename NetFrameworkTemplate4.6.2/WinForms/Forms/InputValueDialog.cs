using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cheng.Windows.Forms
{

    /// <summary>
    /// 一个可输入文本并返回的对话框窗体
    /// </summary>
    public sealed partial class InputValueDialog : Form
    {

        public InputValueDialog()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region

        #region 初始化

        private void f_lastInit()
        {

        }

        private void f_init()
        {

            ButtonOK.Click += fe_ButtonClick_OK;
            ButtonCancel.Click += fe_ButtonClick_Cancel;
        }

        #endregion

        #region 参数

        #endregion

        #region 控件参数

        private TextBox Col_TextBox_Input
        {
            get => col_textBox_input;
        }

        #endregion

        #region 事件注册

        private void fe_ButtonClick_OK(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void fe_ButtonClick_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 对话框窗体标题
        /// </summary>
        public string Title
        {
            get => this.Text;
            set
            {
                this.Text = value;
            }
        }

        /// <summary>
        /// 充当输入框的文本框控件
        /// </summary>
        public TextBox InputValueTextBox
        {
            get => col_textBox_input;
        }

        /// <summary>
        /// 表示为确定的按钮
        /// </summary>
        public Button ButtonOK
        {
            get => col_button_ok;
        }

        /// <summary>
        /// 表示为取消按钮
        /// </summary>
        public Button ButtonCancel
        {
            get => col_button_cancel;
        }

        /// <summary>
        /// 表示为确定的按钮上的文本
        /// </summary>
        public string ButtonOkText
        {
            get => ButtonOK.Text;
            set
            {
                ButtonOK.Text = value;
            }
        }

        /// <summary>
        /// 表示为取消的按钮上的文本
        /// </summary>
        public string ButtonCancelText
        {
            get => ButtonCancel.Text;
            set
            {
                ButtonCancel.Text = value;
            }
        }

        /// <summary>
        /// 对话框的输入框内容
        /// </summary>
        public string InputValue
        {
            get => Col_TextBox_Input.Text;
            set
            {
                Col_TextBox_Input.Text = value;
            }
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <returns>对话框返回值，对话框点击确定表示为<see cref="DialogResult.OK"/>，取消表示<see cref="DialogResult.Cancel"/></returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public DialogResult ShowInputValueDialog(IWin32Window window)
        {
            if (window is null) throw new ArgumentNullException();

            DialogResult = DialogResult.None;
            Col_TextBox_Input.Text = string.Empty;

            return ShowDialog(window);
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <returns>如果返回的对话框是<see cref="DialogResult.OK"/>则返回输入框数据，如果是其它返回值返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public string ShowInputValueDialogResult(IWin32Window window)
        {
            var re = ShowInputValueDialog(window);
            if(re == DialogResult.OK)
            {
                return InputValue;
            }
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="int"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputInt32Dialog(IWin32Window window, out int value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return int.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="uint"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputUInt32Dialog(IWin32Window window, out uint value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return uint.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="long"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputInt64Dialog(IWin32Window window, out long value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return long.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="ulong"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputUInt64Dialog(IWin32Window window, out ulong value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return ulong.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="long"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputFloatDialog(IWin32Window window, out float value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return float.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="double"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputDoubleDialog(IWin32Window window, out double value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return double.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        /// <summary>
        /// 在指定父窗体上弹出输入框窗口并返回输入内容
        /// </summary>
        /// <param name="window">父窗体</param>
        /// <param name="value">要返回的值</param>
        /// <returns>如果输入的文本可以成功转换为<see cref="decimal"/>返回true，如果无法转换则返回false；如果对话框返回值是<see cref="DialogResult.OK"/>之外的值则返回null</returns>
        /// <exception cref="ArgumentException">窗体错误或是null</exception>
        /// <exception cref="InvalidOperationException">窗体已经可见或是禁用</exception>
        public bool? ShowInputDecimalDialog(IWin32Window window, out decimal value)
        {
            var re = ShowInputValueDialog(window);
            if (re == DialogResult.OK)
            {
                var str = InputValue;

                return decimal.TryParse(str, out value);
            }
            value = 0;
            return null;
        }

        #endregion

        #endregion

    }
}
