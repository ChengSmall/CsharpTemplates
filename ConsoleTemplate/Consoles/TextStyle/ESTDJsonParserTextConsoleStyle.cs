using System;
using System.IO;
using Cheng.Consoles;

namespace Cheng.Json.ExpandableStandardText
{

    /// <summary>
    /// json可扩展标准文本转化器 - 控制台文本样式扩展
    /// </summary>
    public class ESTDJsonParserTextConsoleStyle : ExpandableStandardJsonParserText
    {

        /// <summary>
        /// 实例化json控制台文本样式扩展标准转化器
        /// </summary>
        public ESTDJsonParserTextConsoleStyle()
        {
        }

        /// <summary>
        /// 在此处添加styles字符串
        /// </summary>
        /// <param name="styles">样式参数</param>
        /// <param name="writer">要写入的样式字符串</param>
        protected virtual void AppendStyleString(JsonSTDStylesText styles, TextWriter writer)
        {
            writer.WriteANSIColorText(styles.color.Value, false);
        }

        protected override void ToStylesText(JsonVariable value, JsonSTDStylesText styles, TextWriter writer)
        {
            if (styles.color.HasValue)
            {
                AppendStyleString(styles, writer);
            }

            ToSTDNodeText(value, writer);

            if (styles.color.HasValue)
            {
                writer.WriteANSIStyleResetText();
            }
        }

    }

}