using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml;

using Cheng.Texts;

namespace Cheng.Xmls
{

    /// <summary>
    /// 一个Xml节点解析器公共接口
    /// </summary>
    public interface IXmlItemParser
    {

        /// <summary>
        /// 将Xml节点解析为指定对象
        /// </summary>
        /// <param name="node">要解析的xml节点</param>
        /// <returns>解析后的对象</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        object XmlParserToObject(XmlNode node);

        /// <summary>
        /// 获取解析后对象的类型
        /// </summary>
        /// <returns>解析后对象的类型</returns>
        Type ToObjectType();
    }

    /// <summary>
    /// 一个Xml节点解析器公共基类
    /// </summary>
    public abstract class BaseXmlItemParser : IXmlItemParser
    {

        #region 构造

        protected BaseXmlItemParser()
        {
        }

        #endregion

        #region 派生

        /// <summary>
        /// 获取解析后对象的类型
        /// </summary>
        /// <returns>解析后对象的类型</returns>
        public abstract Type ToObjectType();

        /// <summary>
        /// 将Xml节点解析为指定对象
        /// </summary>
        /// <param name="node">要解析的xml节点</param>
        /// <returns>解析后的对象</returns>
        /// <exception cref="ArgumentException">参数错误</exception>
        public abstract object XmlParserToObject(XmlNode node);

        #endregion

    }

}

