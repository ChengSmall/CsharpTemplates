using Cheng.DataStructure.Colors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Cheng.Texts;
using System.IO;
using Cheng.Consoles;

namespace Cheng.Xmls.StandardItemText
{

    /// <summary>
    /// 对Xml标准项数据文本的解析器基本实现的控制台颜色输出实现
    /// </summary>
    /// <remarks>
    /// 使用<see cref="ConsoleTextStyle"/>中控制台颜色实现颜色输出
    /// </remarks>
    public class XmlStandardItemConsole : XmlStandardItemText
    {

        #region

        /// <summary>
        /// 实例化控制台颜色输出标准项数据解析器
        /// </summary>
        public XmlStandardItemConsole()
        {
        }

        #endregion

        #region 派生

        /// <summary>
        /// 使用控制台文本样式<see cref="ConsoleTextStyle"/>实现颜色输出
        /// </summary>
        /// <param name="append">待添加缓冲区</param>
        /// <param name="color">文本颜色</param>
        /// <param name="node">标准项文本包装节点</param>
        protected override void ToColorText(StringBuilder append, Colour color, XmlNode node)
        {
            append.AppendANSIColorText(color.r, color.g, color.b, false);
            ToSTDText(node, append);
            append.AppendANSIResetColorText();
        }

        #endregion

    }

}
