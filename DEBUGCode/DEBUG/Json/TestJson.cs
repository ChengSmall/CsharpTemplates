using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cheng.Json;

namespace Cheng.DEBUG
{

    /// <summary>
    /// DEBUG测试Json解析器
    /// </summary>
    public static class TestJson
    {

        #region 单例

        private static JsonParser p_jp = null;

        #endregion

        #region 参数访问

        /// <summary>
        /// DEBUG专用Json解析器，默认为<see cref="JsonParserDefault"/>
        /// </summary>
        public static JsonParser Parser
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if(p_jp is null)
                {
                    p_jp = new JsonParserDefault();
                }
                return p_jp;
            }
            set
            {
                p_jp = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 使用默认类型获取DEBUG专用Json解析器，类型不正确则为null
        /// </summary>
        public static JsonParserDefault DefaultParser
        {
            get => Parser as JsonParserDefault;
        }

        /// <summary>
        /// 以指定类型获取DEBUG专用Json解析器
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>Json解析器，若类型不正确则为null</returns>
        public static T GetParser<T>() where T : JsonParser
        {
            return p_jp as T;
        }

        /// <summary>
        /// 以指定类型设置DEBUG专用Json解析器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">要设置的DEBUG专用Json解析器</param>
        public static void SetParser<T>(T parser) where T : JsonParser
        {
            p_jp = parser ?? throw new ArgumentNullException();
        }

        #endregion

        #region 功能
        
        /// <summary>
        /// （DEBUG）将json对象转化为文本
        /// </summary>
        /// <param name="json"></param>
        /// <param name="writer"></param>
        public static void ParserToText(this JsonVariable json, TextWriter writer)
        {
            Parser?.ParsingJson(json, writer);
        }

        /// <summary>
        /// （DEBUG）将json对象转化为文本
        /// </summary>
        /// <param name="json"></param>
        /// <returns>转化的文本</returns>
        public static string ParserToText(this JsonVariable json)
        {
            StringWriter swr = new StringWriter(new StringBuilder(32));

            ParserToText(json, swr);

            return swr.ToString();
        }

        /// <summary>
        /// （DEBUG）将json对象转化为文本并写入文件
        /// </summary>
        /// <param name="json"></param>
        /// <param name="filePath">要写入的文件路径</param>
        public static void ParserToFile(this JsonVariable json, string filePath)
        {
            using (StreamWriter swr = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                ParserToText(json, swr);
            }
        }

        /// <summary>
        /// （DEBUG）读取文本并转化为Json对象
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <returns>解析后的对象</returns>
        public static JsonVariable ParserToJsonObj(this TextReader reader)
        {
            return Parser.ToJsonData(reader);
        }

        /// <summary>
        /// （DEBUG）读取文本并转化为Json对象
        /// </summary>
        /// <param name="jsonText">要读取的文本</param>
        /// <returns>转化的Json对象</returns>
        public static JsonVariable ParserToJsonObj(this string jsonText)
        {
            return ParserToJsonObj(new StringReader(jsonText));
        }

        /// <summary>
        /// （DEBUG）读取文件的文本并转化为Json对象
        /// </summary>
        /// <param name="filePath">要读取的文件路径</param>
        /// <returns>解析后的对象</returns>
        public static JsonVariable FileParserToJsonObj(this string filePath)
        {
            using(FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8, false, 1024 * 2, true))
            {
                return ParserToJsonObj(sr);
            }
        }

        /// <summary>
        /// （DEBUG）读取文件的文本并转化为Json对象
        /// </summary>
        /// <param name="filePath">要读取的文件路径</param>
        /// <param name="encoding">使用指定编码读取文本</param>
        /// <param name="detectEncodingFromByteOrderMarks">是否在文件头查找字节顺序标记</param>
        /// <returns></returns>
        public static JsonVariable FileParserToJsonObj(this string filePath, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader sr = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks))
            {
                return ParserToJsonObj(sr);
            }
        }

        /// <summary>
        /// （DEBUG）读取文件的文本并转化为Json对象
        /// </summary>
        /// <param name="filePath">要读取的文件路径</param>
        /// <param name="encoding">使用指定编码读取文本</param>
        /// <returns></returns>
        public static JsonVariable FileParserToJsonObj(this string filePath, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(filePath, encoding))
            {
                return ParserToJsonObj(sr);
            }
        }

        #endregion

    }

}
