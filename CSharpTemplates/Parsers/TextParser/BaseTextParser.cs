using System;
using System.IO;

namespace Cheng.Parsers
{

    /// <summary>
    /// 一个公共文本解析器接口，能够将对象解析为文本，也可以将文本转化为对象
    /// </summary>
    public interface ITextParser
    {

        /// <summary>
        /// 将文本解析并转化为对象
        /// </summary>
        /// <param name="reader">要解析的文本</param>
        /// <returns>转化的对象</returns>
        /// <exception cref="NotImplementedException">解析失败</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        object ConverToObject(TextReader reader);

        /// <summary>
        /// 将对象解析并转化为文本
        /// </summary>
        /// <param name="obj">要解析的对象</param>
        /// <param name="writer">转化到的文本</param>
        /// <exception cref="NotImplementedException">解析失败</exception>
        /// <exception cref="ArgumentNullException">文本写入器为null</exception>
        void ConverToText(object obj, TextWriter writer);

    }

}
