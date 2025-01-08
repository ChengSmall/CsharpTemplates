using Cheng.DataStructure.Colors;
using Cheng.Texts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Cheng.Xmls.StandardItemText
{

    /// <summary>
    /// 对Xml标准项数据文本的解析器的基本实现
    /// </summary>
    public class XmlStandardItemText : BaseXmlItemParser
    {

        #region 结构

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个标准项数据文本基本解析器
        /// </summary>
        public XmlStandardItemText()
        {
            p_buffer = new StringBuilder();
        }

        #endregion

        #region 参数

        private StringBuilder p_buffer;

        #endregion

        #region 功能

        #region 封装

        static bool f_strToByte(char left, char right, out byte value)
        {
            value = 0;

            #region right

            if (right >= '0' && right <= '9')
            {
                value = (byte)((right) - '0');
            }
            else if (right >= 'A' && right <= 'F')
            {
                value = (byte)(((right) - 'A') + 10);
            }
            else if (right >= 'a' && right <= 'f')
            {
                value = (byte)(((right) - 'a') + 10);
            }
            else
            {
                return false;
            }

            #endregion

            #region left

            if (left >= '0' && left <= '9')
            {
                value |= (byte)(((left) - '0') << 4);
            }
            else if (left >= 'A' && left <= 'F')
            {
                value |= (byte)((((left) - 'A') + 10) << 4);
            }
            else if (left >= 'a' && left <= 'f')
            {
                value |= (byte)((((left) - 'a') + 10) << 4);
            }
            else
            {
                return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 添加一个子项
        /// </summary>
        /// <param name="append"></param>
        /// <param name="node">向后解析并添加的子项</param>
        private void f_parItem(StringBuilder append, XmlNode node)
        {
            string value;
            var nodeType = node.NodeType;
            bool flag;
            if(nodeType == XmlNodeType.Comment || nodeType == XmlNodeType.EndElement)
            {
                return; //忽略注释和结尾项
            }

            if (nodeType == XmlNodeType.Element)
            {
                //节点
                var nodeE = node as XmlElement;
                var nodeName = nodeE.Name;
                var nohavCh = nodeE.IsEmpty;

                var ch = nodeE.ChildNodes;
                int chCount = ch.Count;
                if(nodeName == "color")
                {
                    if (nohavCh)
                    {
                        //无子项，语法错误
                        AppendStringBulider(append, nodeE.InnerText);
                        return;
                    }
                    value = nodeE.GetAttribute("value");
                    if (value.Length != 6)
                    {
                        //不是6个字符，错误语法
                        ToSTDText(nodeE, append);
                        //AppendStringBulider(append, nodeE.Value);
                        return;
                    }
                    byte r, g = 0, b = 0;
                    flag = f_strToByte(value[0], value[1], out r) &&
                        f_strToByte(value[2], value[3], out g) &&
                        f_strToByte(value[4], value[5], out b);

                    if (!flag)
                    {
                        //错误语法
                        //直接忽略吧
                        ToSTDText(nodeE, append);
                        return;
                    }

                    //正确颜色语法
                    ToColorText(append, new Colour(r, g, b), nodeE);
                    return;
                }
                else if (nodeName == "STDText")
                {
                    if (nohavCh)
                    {
                        //没有文本
                        AppendStringBulider(append, nodeE.InnerText);
                        return;
                    }
                    //基本原始编码
                    var at = nodeE.GetAttributeNode("encoding");
                    if (at is null)
                    {
                        AppendStringBulider(append, nodeE.InnerText); //错误的
                        return;
                    }

                    try
                    {
                        var enct = Encoding.GetEncoding(at.Value);
                        p_buffer.Clear();
                        for (int chix = 0; chix < chCount; chix++)
                        {
                            p_buffer.Append(ch[chix].InnerText);
                        }

                        AppendBinaryTextToString(append, enct, p_buffer.ToString());
                    }
                    catch (Exception)
                    {
                        //异常的
                        AppendStringBulider(append, nodeE.InnerText); 
                        return;
                    }

                    return;
                }
                else if (nodeName == "math")
                {
                    if (nohavCh)
                    {
                        //没有文本
                        AppendStringBulider(append, nodeE.InnerText);
                        return;
                    }

                    var at = nodeE.GetAttributeNode("type");
                    var atv = nodeE.GetAttributeNode("value");
                    if (at is null)
                    {
                        AppendStringBulider(append, nodeE.InnerText);
                        return;
                    }

                    //语法完备
                    StringBuilder mathBuf = new StringBuilder();
                    ToSTDText(nodeE, mathBuf);
                    AppendMathTypeToString(append, at.Value, atv?.Value, mathBuf.ToString());

                    return;
                }
                else
                {
                    //其它节点名
                    goto AllNotParser;
                }
            }

            if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
            {
                //可直接添加
                AppendStringBulider(append, node.Value);
                return;
            }

            AllNotParser: //不是基本标准

            if(!DIYParserSTDText(append, node))
            {
                //无法解析
                AppendStringBulider(append, node.InnerText);
            }
        }

        /// <summary>
        /// 数学运算解析功能
        /// </summary>
        /// <param name="append">待添加缓冲区</param>
        /// <param name="mathType">数学运算类型</param>
        /// <param name="value">第二个属性参数，null则无</param>
        /// <param name="text">要运算的文本</param>
        protected virtual void AppendMathTypeToString(StringBuilder append, string mathType, string value, string text)
        {

            try
            {
                if (mathType == "cal")
                {
                    double left, right;

                    int length = text.Length;
                    int ti = 0;
                    char cc;
                    bool isHavPoint = false;
                    p_buffer.Clear();
                    //左侧值
                    for ( ; ti < length; )
                    {
                        cc = text[ti];
                        if (char.IsDigit(cc))
                        {
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }

                        if (ti == 0 && cc == '-')
                        {
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }

                        if(cc == '.' && (!isHavPoint))
                        {
                            isHavPoint = true;
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }
                        break;
                    }

                    if(!double.TryParse(p_buffer.ToString(), out left))
                    {
                        //无效值
                        AppendStringBulider(append, text);
                        return;
                    }
                    
                    for(; ti < length; )
                    {
                        cc = text[ti];
                        if (char.IsWhiteSpace(cc))
                        {
                            ti++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //中间运算符
                    char alg = text[ti++]; //运算符

                    for (; ti < length;)
                    {
                        cc = text[ti];
                        if (char.IsWhiteSpace(cc))
                        {
                            ti++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    p_buffer.Clear();
                    isHavPoint = false;
                    for (; ti < length; )
                    {
                        cc = text[ti];
                        if (char.IsDigit(cc))
                        {
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }

                        if (ti == 0 && cc == '-')
                        {
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }

                        if (cc == '.' && (!isHavPoint))
                        {
                            isHavPoint = true;
                            p_buffer.Append(cc);
                            ti++;
                            continue;
                        }
                        break;
                    }

                    if (!double.TryParse(p_buffer.ToString(), out right))
                    {
                        //无效值
                        AppendStringBulider(append, text);
                        return;
                    }

                    if (alg == '+')
                    {
                        AppendStringBulider(append, (left + right).ToString());
                        return;
                    }
                    else if (alg == '-')
                    {
                        AppendStringBulider(append, (left - right).ToString());
                        return;
                    }
                    else if (alg == '*')
                    {
                        AppendStringBulider(append, (left * right).ToString());
                        return;
                    }
                    else if (alg == '/')
                    {
                        AppendStringBulider(append, (left / right).ToString());
                        return;
                    }
                    else if (alg == '%')
                    {
                        AppendStringBulider(append, (left % right).ToString());
                        return;
                    }
                    else
                    {
                        //无效运算符
                        AppendStringBulider(append, text);
                        return;
                    }

                    //return;
                }

                double re;

                if (!double.TryParse(text, out re))
                {
                    AppendStringBulider(append, text);
                    return;
                }

                if (mathType == "percent")
                {
                    AppendStringBulider(append, (re * 100).ToString());
                    AppendStringBulider(append, '%');
                }
                else if (mathType == "round")
                {
                    AppendStringBulider(append, ((long)Math.Round(re)).ToString());
                }
                else if (mathType == "cutPoint")
                {
                    AppendStringBulider(append, ((long)re).ToString());
                }
                else if (mathType == "keepDecs")
                {
                    if(value is null)
                    {
                        AppendStringBulider(append, text);
                        return;
                    }

                    if(!int.TryParse(value, out int count))
                    {
                        AppendStringBulider(append, text);
                        return;
                    }

                    AppendStringBulider(append, re.ToString("F" + count.ToString()));
                    return;
                }
                else
                {
                    AppendStringBulider(append, text);
                }

            }
            catch (Exception)
            {
                AppendStringBulider(append, text);
            }
        }

        /// <summary>
        /// 二进制标准文本转化并添加到缓冲区
        /// </summary>
        /// <param name="append">待添加到的缓冲区</param>
        /// <param name="encoding">要以指定的编码解析</param>
        /// <param name="binaryString">表示二进制的文本形式</param>
        protected virtual void AppendBinaryTextToString(StringBuilder append, Encoding encoding, string binaryString)
        {
            MemoryStream ms = new MemoryStream(32);

            if (binaryString is null)
            {
                return;
            }

            var length = binaryString.Length;

            try
            {
                int count = 1;
                char left = default, right;
                for (int i = 0; i < length; i++, count++)
                {
                    var cv = binaryString[i];

                    if (count == 1)
                    {
                        left = cv;
                    }
                    else if (count == 2)
                    {
                        right = cv;
                        if (f_strToByte(left, right, out byte bv))
                        {
                            ms.WriteByte(bv);
                        }
                        else
                        {
                            //错误解析
                            AppendStringBulider(append, binaryString);
                            return;
                        }

                    }
                    else if (count == 3)
                    {
                        count = 0;
                    }

                }

                var bs = ms.ToArray();
                encoding.GetString(bs);

                AppendStringBulider(append, encoding.GetString(bs));
                return;
            }
            catch (Exception)
            {
                AppendStringBulider(append, binaryString);
            }
           
        }

        /// <summary>
        /// 即将添加字符串到缓冲区时调用的方法
        /// </summary>
        /// <param name="append">添加到的缓冲区</param>
        /// <param name="value">待添加文本</param>
        protected void AppendStringBulider(StringBuilder append, string value)
        {
            if (value is null) return;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                char c = value[i];
                if (CanAppendChar(c))
                {
                    append.Append(c);
                }
            }
        }

        /// <summary>
        /// 即将添加字符到缓冲区时调用的方法
        /// </summary>
        /// <param name="append">添加到的缓冲区</param>
        /// <param name="value">待添加字符</param>
        protected void AppendStringBulider(StringBuilder append, char value)
        {
            if (CanAppendChar(value))
            {
                append.Append(value);
            }
        }

        /// <summary>
        /// 在添加字符到输出文本前，检查该字符是否能够添加
        /// </summary>
        /// <remarks>默认实现为直接返回true</remarks>
        /// <param name="c">检查字符</param>
        /// <returns>返回true可以添加，返回false则忽略该字符</returns>
        protected virtual bool CanAppendChar(char c)
        {
            return true;
        }

        /// <summary>
        /// 在标准项中检查到颜色定义则调用该函数用于输出带颜色文本
        /// </summary>
        /// <remarks>该函数默认实现为直接将内部标准文本输出</remarks>
        /// <param name="append">将输出文本添加于此的缓冲区</param>
        /// <param name="color">该文本颜色</param>
        /// <param name="node">一个标准文本项包装节点</param>
        protected virtual void ToColorText(StringBuilder append, Colour color, XmlNode node)
        {
            //f_append(append, );
            //AppendStringBulider(append, node.InnerText);
            ToSTDText(node, append);
        }

        /// <summary>
        /// 定义非基本标准的解析功能扩展
        /// </summary>
        /// <remarks>
        /// <para>当解析器检测到语法不是基本标准定义，则调用该函数尝试用扩展定义解析；在派生类重写以实现扩展解析器，该函数默认直接返回false</para>
        /// </remarks>
        /// <param name="append">将输出文本添加于此的缓冲区</param>
        /// <param name="node">一个处于STDText节点内的语法子节点，解析为文本后将添加到<paramref name="append"/></param>
        /// <returns>成功解析返回true，无法解析返回false</returns>
        protected virtual bool DIYParserSTDText(StringBuilder append, XmlNode node)
        {
            return false;
        }

        /// <summary>
        /// 顺序解析node下的标准文本并添加
        /// </summary>
        /// <remarks>
        /// <para>将<paramref name="node"/>始视为STDText包装的节点，从基本标准开始解析并添加，若不是基础标准，则调用<see cref="DIYParserSTDText(StringBuilder, XmlNode)"/>解析，全部无法解析则添加<see cref="XmlNode.InnerText"/>到缓冲区</para>
        /// <para>该函数使用递归来嵌套解析标准文本，当遇到需要嵌套解析时可调用该函数</para>
        /// </remarks>
        /// <param name="node">一个标准文本项包装节点</param>
        /// <param name="append">要添加到的文本缓冲区</param>
        protected void ToSTDText(XmlNode node, StringBuilder append)
        {
            var chs = node.ChildNodes;
            int length = chs.Count;

            for (int i = 0; i < length; i++)
            {
                var c_node = chs[i];
                f_parItem(append, c_node);
            }
        }

        #endregion

        #region 参数访问

        #endregion

        #region 派生

        /// <summary>
        /// 将给定的xml节点以标准项数据文本解析并输出到字符串缓冲区
        /// </summary>
        /// <param name="node">要解析的一个标准文本项包装节点</param>
        /// <param name="append">待输出缓冲区</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public void XmlToStandardItemText(XmlNode node, StringBuilder append)
        {            
            if (node is null || append is null) throw new ArgumentNullException();

            ToSTDText(node, append);
        }

        /// <summary>
        /// 解析后的对象类型
        /// </summary>
        /// <returns>表示字符串类型的反射对象</returns>
        public override Type ToObjectType()
        {
            return typeof(string);
        }

        /// <summary>
        /// 将给定的xml节点以标准项数据文本解析为字符串
        /// </summary>
        /// <param name="node">要解析的一个标准文本项包装节点</param>
        /// <returns>解析后的字符串</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public override object XmlParserToObject(XmlNode node)
        {
            if (node is null) throw new ArgumentNullException();
            StringBuilder sb = new StringBuilder(32);
            ToSTDText(node, sb);
            return sb.ToString();
        }

        #endregion

        #endregion

    }

}
