using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;

using Cheng.Texts;
using Cheng.Algorithm;
using Cheng.DataStructure.Colors;

namespace Cheng.Json.ExpandableStandardText
{

    /// <summary>
    /// json标准文本转化样式参数
    /// </summary>
    public struct JsonSTDStylesText
    {

        /// <summary>
        /// 启用下划线
        /// </summary>
        public bool underline;

        /// <summary>
        /// 启用斜体
        /// </summary>
        public bool italic;

        /// <summary>
        /// 启用删除线
        /// </summary>
        public bool strikethrough;

        /// <summary>
        /// 文本颜色（忽略透明度），null表示没有颜色
        /// </summary>
        public Colour? color;

    }

    /// <summary>
    /// json可扩展标准文本转化器
    /// </summary>
    public class ExpandableStandardJsonParserText : IJsonParserToText
    {

        #region 初始化

        /// <summary>
        /// 实例化一个json可扩展标准文本转化器
        /// </summary>
        public ExpandableStandardJsonParserText()
        {
        }

        #endregion

        #region 参数

        #endregion

        #region 实现

        /// <summary>
        /// 将json集合依次调用<see cref="ToSTDNodeText(JsonVariable, TextWriter)"/>转化并写入
        /// </summary>
        /// <param name="jlist">json集合</param>
        /// <param name="writer">要写入的文本写入器</param>
        protected void ToListText(JsonList jlist, TextWriter writer)
        {
            var length = jlist.Count;
            for (int i = 0; i < length; i++)
            {
                ToSTDNodeText(jlist[i], writer);
            }
        }

        /// <summary>
        /// 以指定样式参数转化value值
        /// </summary>
        /// <param name="value">样式复合对象内的value值，表示要转化的json结构</param>
        /// <param name="styles">样式参数</param>
        /// <param name="writer">转化后要写入的文本写入器</param>
        protected virtual void ToStylesText(JsonVariable value, JsonSTDStylesText styles, TextWriter writer)
        {
            ToSTDNodeText(value, writer);
        }

        private JsonSTDStylesText f_toStyles(JsonDictionary json)
        {
            JsonVariable tj;
            JsonSTDStylesText re = default;

            if (json.TryGetValue("underline", out tj))
            {
                if(tj.DataType == JsonType.Boolean)
                {
                    re.underline = tj.Boolean;
                }
            }
            if (json.TryGetValue("italic", out tj))
            {
                if (tj.DataType == JsonType.Boolean)
                {
                    re.italic = tj.Boolean;
                }
            }
            if (json.TryGetValue("strikethrough", out tj))
            {
                if (tj.DataType == JsonType.Boolean)
                {
                    re.strikethrough = tj.Boolean;
                }
            }

            if (json.TryGetValue("color", out tj))
            {
                if (tj.DataType == JsonType.String)
                {
                    var colorStr = tj.String;
                    Colour c = default;
                    if (colorStr.Length >= 6)
                    {
                        byte tb;
                        if(TextManipulation.X16TextToByte(colorStr[0], colorStr[1], out tb))
                        {
                            c.r = tb;
                        }
                        if (TextManipulation.X16TextToByte(colorStr[2], colorStr[3], out tb))
                        {
                            c.g = tb;
                        }
                        if (TextManipulation.X16TextToByte(colorStr[4], colorStr[5], out tb))
                        {
                            c.b = tb;
                        }
                        re.color = c;
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// 如果转化器无法将json结构识别为标准结构，则调用该函数扩展转化文本
        /// </summary>
        /// <param name="json">要转化的一个复合对象json结构</param>
        /// <param name="writer">转化后要写入的文本写入器</param>
        /// <param name="type">复合对象的type值</param>
        protected virtual void ToNotSTDNodeText(JsonDictionary json, TextWriter writer, string type)
        {
            writer.Write(json.ToString());
        }

        /// <summary>
        /// 将一个json结构转化为可扩展标准文本并写入文本写入器
        /// </summary>
        /// <param name="json">要转化的一个json对象，会将看作一个可扩展标准文本结构（不能是null）</param>
        /// <param name="writer">文本写入器（不能是null）</param>
        protected void ToSTDNodeText(JsonVariable json, TextWriter writer)
        {
            var jtype = json.DataType;

            //简单值转文本
            switch (jtype)
            {
                case JsonType.String:
                    writer.Write(json.String);
                    return;
                case JsonType.RealNum:
                    writer.Write(json.RealNum);
                    return;
                case JsonType.Integer:
                    writer.Write(json.Integer);
                    return;
                default:
                    break;
            }

            if (jtype == JsonType.Dictionary)
            {
                var jd = json.JsonObject;
                JsonVariable jvtype;
                if (jd.TryGetValue("type", out jvtype))
                {
                    var jvtt = jvtype.DataType;
                    if (jvtt != JsonType.String)
                    {
                        //type不属于字符串
                        writer.Write(json.ToString());
                        return;
                    }

                    var jtvalue = jvtype.String;
                    //获取复合对象的value值
                    bool JdIsValue = jd.TryGetValue("value", out var valueJson);

                    switch (jtvalue)
                    {
                        case "styles":
                            if (!JdIsValue)
                            {
                                writer.Write(json.ToString());
                                return;
                            }
                            var style = f_toStyles(jd);
                            ToStylesText(valueJson, style, writer);
                            return;
                        case "text":
                            if (valueJson.DataType == JsonType.String)
                            {
                                writer.Write(valueJson.String);
                            }
                            else
                            {
                                writer.Write(json.ToString());
                            }
                            return;
                        case "list":
                            if (valueJson.DataType == JsonType.List)
                            {
                                ToListText(valueJson.Array, writer);
                            }
                            else
                            {
                                writer.Write(valueJson.ToString());
                            }
                            return;
                        default:
                            break;
                    }

                    //不是基础 type 值
                    ToNotSTDNodeText(jd, writer, jtvalue);

                }
                else
                {
                    //没有type值，输出原始json文本
                    writer.Write(json.ToString());
                    return;
                }
            }

            if (jtype == JsonType.List)
            {
                //多项集合
                ToListText(json.Array, writer);
                return;
            }

            //不是特定类型
            writer.Write(json.ToString());
        }

        #endregion

        #region 功能

        /// <summary>
        /// 转化json结构为可扩展标准文本到文本写入器
        /// </summary>
        /// <param name="json">要转化的json对象</param>
        /// <param name="writer">转化后要写入的文本写入器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">写入器已被释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="Exception">其它错误</exception>
        public void ToText(JsonVariable json, TextWriter writer)
        {
            if (json is null || writer is null) throw new ArgumentNullException();
            ToSTDNodeText(json, writer);
        }

        /// <summary>
        /// 转化json结构为可扩展标准文本到字符串缓冲区
        /// </summary>
        /// <param name="json">要转化的json对象</param>
        /// <param name="append">转化后要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(JsonVariable json, StringBuilder append)
        {
            using (var strwr = new StringWriter(append))
            {
                ToText(json, strwr);
            }
        }

        /// <summary>
        /// 转化json结构为可扩展标准文本到字符串缓冲区
        /// </summary>
        /// <param name="json">要转化的json对象</param>
        /// <param name="append">转化后要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToText(JsonVariable json, CMStringBuilder append)
        {
            using (var strwr = new CMStringBuilderWriter(append))
            {
                ToText(json, strwr);
            }
        }

        void IJsonParserToText.ParsingJson(JsonVariable json, TextWriter writer)
        {
            ToText(json, writer);
        }

        #endregion

    }

}
#if DEBUG
#endif