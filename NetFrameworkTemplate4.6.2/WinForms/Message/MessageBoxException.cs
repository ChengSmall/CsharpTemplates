using System;
using System.Windows.Forms;

namespace Cheng.Windows.Forms
{

    /// <summary>
    /// 用作进行显示消息窗口的异常
    /// </summary>
    public class MessageBoxShowException : ApplicationException
    {

        #region 构造

        public MessageBoxShowException() : base()
        {
            p_title = null;
            p_icon = MessageBoxIcon.Error;
            p_buttons = MessageBoxButtons.OK;
        }

        /// <summary>
        /// 实例化错误消息框窗口异常
        /// </summary>
        /// <param name="title">错误消息框标题</param>
        /// <param name="message">错误框消息文本</param>
        public MessageBoxShowException(string title, string message) : this(title, message, MessageBoxButtons.OK, MessageBoxIcon.Error){}

        /// <summary>
        /// 实例化消息框窗口异常
        /// </summary>
        /// <param name="title">消息框标题</param>
        /// <param name="message">消息框消息文本</param>
        /// <param name="buttons">指定消息框按钮类型</param>
        /// <param name="icon">指定消息框图标类型</param>
        public MessageBoxShowException(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon) : base(message)
        {
            p_title = title;
            p_buttons = buttons;
            p_icon = icon;
        }

        /// <summary>
        /// 实例化消息框窗口异常
        /// </summary>
        /// <param name="title">消息框标题</param>
        /// <param name="message">消息框消息文本</param>
        /// <param name="buttons">指定消息框按钮类型</param>
        /// <param name="icon">指定消息框图标类型</param>
        /// <param name="innerException">导致当前异常的异常</param>
        public MessageBoxShowException(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon, Exception innerException) : base(message, innerException)
        {
            p_title = title;
            p_buttons = buttons;
            p_icon = icon;
        }

        #endregion

        #region 参数

        private readonly string p_title;
        private readonly MessageBoxButtons p_buttons;
        private readonly MessageBoxIcon p_icon;

        /// <summary>
        /// 消息框标题
        /// </summary>
        public string Title
        {
            get => p_title;
        }

        /// <summary>
        /// 指示消息框按钮类型
        /// </summary>
        public MessageBoxButtons BoxButtons => p_buttons;

        /// <summary>
        /// 指示消息框图标类型
        /// </summary>
        public MessageBoxIcon BoxIcon => p_icon;

        /// <summary>
        /// 表示MessageBox内容的文本
        /// </summary>
        public override string Message => base.Message;

        #endregion

        #region templates
        #if DEBUG

        static void throwTemplate()
        {

            try
            {



            }
            catch (MessageBoxShowException ex)
            {
                MessageBox.Show(null, ex.Message, ex.Title, ex.BoxButtons, ex.BoxIcon);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endif
        #endregion

    }

}