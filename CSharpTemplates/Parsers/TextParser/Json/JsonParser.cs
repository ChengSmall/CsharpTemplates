using Cheng.Parsers;
using System;
using System.IO;

namespace Cheng.Json
{

    /// <summary>
    /// 将json对象转化为特定文本的公共接口
    /// </summary>
    public interface IJsonParserToText
    {
        /// <summary>
        /// 将json对象转化为特定文本并写入到指定的字符序列编写器中
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="writer">要写入的字符序列对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="Exception">其它错误</exception>
        void ParsingJson(JsonVariable json, TextWriter writer);
    }

    /// <summary>
    /// 一个公开的json解析器接口
    /// </summary>
    /// <remarks>派生此接口以实现完全自定义json解析器；若想要使用默认以实现的解析器，请使用<see cref="JsonParserDefault"/>实例</remarks>
    public interface IJsonParser : ITextParser, IJsonParserToText
    {

        /// <summary>
        /// 将json文本转化为json对象
        /// </summary>
        /// <param name="reader">要读取的文本</param>
        /// <returns>转化完毕的对象</returns>
        /// <exception cref="NotImplementedException">json文本解析失败</exception>
        JsonVariable ToJsonData(TextReader reader);
    }

    /// <summary>
    /// json解析器的公共基类，可派生此类以自定义json解析器
    /// </summary>
    public abstract class JsonParser : IJsonParser, IJsonParserToText
    {
        /// <summary>
        /// 将json对象转化为json文本并写入到指定的字符序列编写器中
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="writer">要写入的字符序列对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public abstract void ParsingJson(JsonVariable json, TextWriter writer);

        /// <summary>
        /// 将json文本转化为json对象
        /// </summary>
        /// <param name="reader">要读取的文本</param>
        /// <returns>转化完毕的对象</returns>
        /// <exception cref="NotImplementedException">json文本解析失败</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public abstract JsonVariable ToJsonData(TextReader reader);

        /// <summary>
        /// 将json文本转化为json对象
        /// </summary>
        /// <param name="reader">要读取的文本</param>
        /// <param name="json">接收转化完毕的对象，若解析失败则返回一个null</param>
        /// <returns>是否成功解析并转化；若成功转化则返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool ToJsonData(TextReader reader, out JsonVariable json)
        {
            try
            {
                json = ToJsonData(reader);
                return true;
            }
            catch (Exception)
            {
                json = null;
                return false;
            }
        }

        /// <summary>
        /// 将指定的json解析器封装为线程安全实例
        /// </summary>
        /// <param name="jsonParser">要封装的实例</param>
        /// <returns>一个线程安全的json解析器</returns>
        public static JsonParser AsyncSafe(JsonParser jsonParser)
        {
            return new ParserThreadSafe(jsonParser);
        }

        /// <summary>
        /// 将json文本转化为json对象
        /// </summary>
        /// <param name="jsonText">要读取的文本</param>
        /// <returns>转化完毕的对象</returns>
        /// <exception cref="NotImplementedException">json文本解析失败</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual JsonVariable ToJsonData(string jsonText)
        {
            using (StringReader sr = new StringReader(jsonText))
            {
                return ToJsonData(sr);
            }
        }

        /// <summary>
        /// 将json文本转化为json对象
        /// </summary>
        /// <param name="jsonText">要读取的文本</param>
        /// <param name="json">接收转化完毕的对象，若解析失败则返回一个null</param>
        /// <returns>是否成功解析并转化；若成功转化则返回true，否则返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public virtual bool ToJsonData(string jsonText, out JsonVariable json)
        {
            StringReader sr = new StringReader(jsonText);
            return ToJsonData(sr, out json);
        }

        object ITextParser.ConverToObject(TextReader reader)
        {
            return ToJsonData(reader);
        }

        void ITextParser.ConverToText(object obj, TextWriter writer)
        {
            if (obj is JsonVariable)
            {
                ParsingJson((JsonVariable)obj, writer);
            }
            else throw new NotImplementedException();
            
        }

        class ParserThreadSafe : JsonParser
        {
            public ParserThreadSafe(JsonParser jsonParser)
            {
                parser = jsonParser ?? throw new ArgumentNullException("jsonParser");
            }

            private JsonParser parser;

            public override void ParsingJson(JsonVariable json, TextWriter writer)
            {
                lock (parser)
                {
                    parser.ParsingJson(json, writer);
                }

            }

            public override JsonVariable ToJsonData(TextReader reader)
            {
                lock (parser)
                {
                    return parser.ToJsonData(reader);
                }
            }

            public override bool ToJsonData(TextReader reader, out JsonVariable json)
            {
                lock (parser)
                {
                    return parser.ToJsonData(reader, out json);
                }
            }

            public override JsonVariable ToJsonData(string jsonText)
            {
                lock (parser)
                {
                    return parser.ToJsonData(jsonText);
                }
            }

            public override bool ToJsonData(string jsonText, out JsonVariable json)
            {
                lock (parser)
                {
                    return parser.ToJsonData(jsonText, out json);
                }
            }

        }
    }

}
