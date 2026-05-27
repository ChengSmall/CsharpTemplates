using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Cheng.Windows.Forms
{

    /// <summary>
    /// 消息窗口扩展
    /// </summary>
    public static class MessageBoxExtend
    {

        #region show

        /// <summary>
        /// 在指定对象线程弹出消息窗口
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <param name="icon">要指定的窗口图标</param>
        /// <param name="buttons">要制定的窗口按钮</param>
        /// <returns>窗口关闭后的返回值</returns>
        /// <exception cref="InvalidEnumArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static DialogResult ShowMegBox(this IWin32Window owner, string title, string text, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            return MessageBox.Show(owner, text, title, buttons, icon);
        }

        #endregion

        #region 无返回值

        /// <summary>
        /// 在指定对象线程弹窗显示错误窗口
        /// </summary>
        /// <remarks>
        /// <para>窗口按钮是一个单独的 Yes，图标是<see cref="MessageBoxIcon.Error"/></para>
        /// </remarks>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static void ShowMegError(this IWin32Window owner, string title, string text)
        {
            MessageBox.Show(owner, text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 在指定对象线程弹窗显示消息窗口
        /// </summary>
        /// <remarks>
        /// <para>窗口按钮是一个单独的 Yes，图标是<see cref="MessageBoxIcon.Information"/></para>
        /// </remarks>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static void ShowMegInf(this IWin32Window owner, string title, string text)
        {
            MessageBox.Show(owner, text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region OK Cancel Yes No

        /// <summary>
        /// 在指定对象线程弹窗，显示按钮为 OK 和 Cancel 的窗口
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <param name="icon">窗口图标</param>
        /// <returns>如果用户点击OK按钮，返回true；否则返回false</returns>
        /// <exception cref="InvalidEnumArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static bool ShowMegOC(this IWin32Window owner, string title, string text, MessageBoxIcon icon)
        {
            return MessageBox.Show(owner, text, title, MessageBoxButtons.OKCancel, icon) == DialogResult.OK;
        }

        /// <summary>
        /// 在指定对象线程弹窗，显示按钮为 OK 和 Cancel 的窗口，图标为<see cref="MessageBoxIcon.Warning"/>
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <returns>如果用户点击OK按钮，返回true；否则返回false</returns>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static bool ShowMegOC(this IWin32Window owner, string title, string text)
        {
            return MessageBox.Show(owner, text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
        }

        /// <summary>
        /// 在指定对象线程弹窗，显示按钮为 Yes 和 No 的窗口
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <param name="icon">窗口显示图标</param>
        /// <returns>如果用户点击 Yes 按钮，返回true；否则返回false</returns>
        /// <exception cref="InvalidEnumArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static bool ShowMegYN(this IWin32Window owner, string title, string text, MessageBoxIcon icon)
        {
            return MessageBox.Show(owner, text, title, MessageBoxButtons.YesNo, icon) == DialogResult.Yes;
        }

        /// <summary>
        /// 在指定对象线程弹窗，按钮为 Yes, No, Cancel 的窗口
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <param name="icon">窗口显示图标</param>
        /// <returns>用户点击 Yes 按钮返回true，点击 No 返回false，点击 Cancel 返回null</returns>
        /// <exception cref="InvalidEnumArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static bool? ShowMegYNC(this IWin32Window owner, string title, string text, MessageBoxIcon icon)
        {
            var re = MessageBox.Show(owner, text, title, MessageBoxButtons.YesNoCancel, icon);
            switch (re)
            {
                case DialogResult.Yes:
                    return true;
                case DialogResult.No:
                    return false;
                default:
                    return null;
            }
        }

        #endregion

        #region 其它

        /// <summary>
        /// 在指定对象线程弹窗，按钮为 Retry 和 Cancel 的窗口
        /// </summary>
        /// <param name="owner">要弹窗的父对象</param>
        /// <param name="title">窗口标题</param>
        /// <param name="text">窗口文本</param>
        /// <param name="icon">窗口显示图标</param>
        /// <returns>如果用户点击 Retry 按钮，返回true；否则返回false</returns>
        /// <exception cref="InvalidEnumArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">
        /// 消息窗在没有用户交互模式中运行的进程中进行显示；
        /// 通过<see cref="System.Windows.Forms.SystemInformation.UserInteractive"/>判断属性
        /// </exception>
        public static bool ShowMegRC(this IWin32Window owner, string title, string text, MessageBoxIcon icon)
        {
            return MessageBox.Show(owner, text, title, MessageBoxButtons.RetryCancel, icon) == DialogResult.Retry;
        }

        #endregion

    }

}
