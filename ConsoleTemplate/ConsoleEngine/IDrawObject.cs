using Cheng.DataStructure.Colors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.Consoles.ConsoleUI
{

    /// <summary>
    /// 表示一个按钮UI的控制台文本
    /// </summary>
    public sealed class ConsoleTextUIButton
    {

        #region 构造

        /// <summary>
        /// 实例化一个控制台文本按钮
        /// </summary>
        public ConsoleTextUIButton()
        {
            p_text = "button";
            p_textColor = new Colour(255, 255, 255);
            p_startColor = false;
        }

        /// <summary>
        /// 实例化一个控制台文本按钮
        /// </summary>
        /// <param name="text">表示按钮的文本</param>
        public ConsoleTextUIButton(string text)
        {
            Text = text;
            p_textColor = new Colour(255, 255, 255);
            p_startColor = false;
        }

        #endregion

        #region 参数

        private string p_text;

        private Colour p_textColor;

        private bool p_startColor;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 按钮的默认边框字符
        /// </summary>
        public const char DefaultRectValue = '■';

        /// <summary>
        /// 表示按钮的文本
        /// </summary>
        public string Text
        {
            get => p_text;
            set
            {
                p_text = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 是否开启按钮颜色显示
        /// </summary>
        /// <value>该参数为true则会使用<see cref="ConsoleTextStyle"/>设置文本颜色以控制按钮的边框颜色和文本颜色，false则忽略颜色数据；默认为false</value>
        public bool StartButtonColor
        {
            get => p_startColor;
            set
            {
                p_startColor = value;
            }
        }

        /// <summary>
        /// 按钮文本颜色
        /// </summary>
        /// <value>
        /// 参数<see cref="StartButtonColor"/>为true时该参数才会生效；该参数忽略透明通道
        /// </value>
        public Colour TextColor
        {
            get
            {
                return p_textColor;
            }
            set
            {
                p_textColor = value;
            }
        }

        #endregion

        #region 派生

        /// <summary>
        /// 返回表示按钮的文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        #endregion

        #endregion

    }

}
