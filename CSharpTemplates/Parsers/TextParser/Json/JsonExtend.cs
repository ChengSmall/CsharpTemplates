using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cheng.Json
{

    /// <summary>
    /// Json扩展功能
    /// </summary>
    public static class JsonExtend
    {

        #region 解析器

        /// <summary>
        /// 读取指定文件的Json文本并转化到Json对象
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="fileInfo">文件</param>
        /// <param name="encoding">读取Json文本时使用的字符编码</param>
        /// <returns>读取的json对象</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无权限</exception>
        /// <exception cref="NotImplementedException">无法实现解析操作</exception>
        /// <exception cref="FileNotFoundException">无法找到文件</exception>
        /// <exception cref="DirectoryNotFoundException">无效的路径名称</exception>
        /// <exception cref="Exception">其它异常</exception>
        public static JsonVariable FileToJson(this IJsonParser parser, FileInfo fileInfo, Encoding encoding)
        {
            if (parser is null || fileInfo is null || encoding is null) throw new ArgumentNullException();

            JsonVariable js;

            using (StreamReader sr = new StreamReader(fileInfo.FullName, encoding, false, 1024 * 2))
            {
                js = parser.ToJsonData(sr);
            }

            return js;
        }

        /// <summary>
        /// 读取指定文件的Json文本并转化到Json对象
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">读取Json文本时使用的字符编码</param>
        /// <returns>读取的json对象</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无权限</exception>
        /// <exception cref="NotImplementedException">无法实现解析操作</exception>
        /// <exception cref="FileNotFoundException">无法找到文件</exception>
        /// <exception cref="DirectoryNotFoundException">无效的路径名称</exception>
        /// <exception cref="Exception">其它异常</exception>
        public static JsonVariable FileToJson(this IJsonParser parser, string filePath, Encoding encoding)
        {
            if (parser is null || encoding is null) throw new ArgumentNullException();

            JsonVariable js;
            
            using (StreamReader sr = new StreamReader(filePath, encoding, false, 1024 * 2))
            {
                js = parser.ToJsonData(sr);
            }

            return js;
        }

        /// <summary>
        /// 以UTF-8的编码读取指定文件的Json文本转化到Json对象
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>读取的json对象</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无权限</exception>
        /// <exception cref="NotImplementedException">无法实现解析操作</exception>
        /// <exception cref="FileNotFoundException">无法找到文件</exception>
        /// <exception cref="DirectoryNotFoundException">无效的路径名称</exception>
        /// <exception cref="Exception">其它异常</exception>
        public static JsonVariable FileToJson(this IJsonParser parser, string filePath)
        {
            if (parser is null) throw new ArgumentNullException();

            JsonVariable js;

            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, false, 1024 * 2))
            {
                js = parser.ToJsonData(sr);
            }

            return js;
        }

        /// <summary>
        /// 将json对象以json文本写入指定文件
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="json">待转化的json对象</param>
        /// <param name="filePath">要写入的文件路径</param>
        /// <param name="encoding">写入时的文本编码</param>
        /// <param name="append">true表示在已有文件内追加内容，false表示从零创建新文件覆盖旧数据；如果文件不存在，则该参数无效，并创建新文件并添加内容</param>
        /// <exception cref="System.ArgumentException">参数无效</exception>
        /// <exception cref="System.UnauthorizedAccessException">拒绝访问</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">路径名无效</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="System.Security.SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="Exception">可能出现的其它解析错误</exception>
        public static void WriterToFile(this IJsonParser parser, JsonVariable json, string filePath, Encoding encoding, bool append)
        {
            if (json is null || encoding is null) throw new ArgumentNullException();
            using (StreamWriter swr = new StreamWriter(filePath, append, encoding))
            {
                parser.ParsingJson(json, swr);
            }
        }

        /// <summary>
        /// 将json对象以json文本写入指定文件
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="json">待转化的json对象</param>
        /// <param name="filePath">要写入的文件路径，若文件不存在则创建文件</param>
        /// <param name="encoding">写入时的文本编码</param>
        /// <exception cref="System.ArgumentException">参数无效</exception>
        /// <exception cref="System.UnauthorizedAccessException">拒绝访问</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">路径名无效</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="System.Security.SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="Exception">可能出现的其它解析错误</exception>
        public static void WriterToFile(this IJsonParser parser, JsonVariable json, string filePath, Encoding encoding)
        {
            WriterToFile(parser, json, filePath, encoding, false);
        }

        /// <summary>
        /// 将json对象以UTF-8编码的json文本写入指定文件
        /// </summary>
        /// <param name="parser">json解析器</param>
        /// <param name="json">待转化的json对象</param>
        /// <param name="filePath">要写入的文件路径，若文件不存在则创建文件</param>
        /// <exception cref="System.ArgumentException">参数无效</exception>
        /// <exception cref="System.UnauthorizedAccessException">拒绝访问</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">路径名无效</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="PathTooLongException">指定的路径或文件名超过了系统定义的最大长度</exception>
        /// <exception cref="System.Security.SecurityException">调用方没有所要求的权限</exception>
        /// <exception cref="Exception">可能出现的其它解析错误</exception>
        public static void WriterToFile(this IJsonParser parser, JsonVariable json, string filePath)
        {
            WriterToFile(parser, json, filePath, Encoding.UTF8, false);
        }

        #endregion

        #region 容量调整

        /// <summary>
        /// 将json对象以及内部的所有集合与键值对未使用的多余空间舍去
        /// </summary>
        /// <param name="json">json对象</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void TrimExcessAll(this JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();
            f_jsonTrimExcess(json);
        }

        static void f_jsonTrimExcess(JsonVariable json)
        {
            var jt = json.DataType;

            switch (jt)
            {
                case JsonType.Dictionary:
                    f_jsonTrimDict(json.JsonObject);
                    return;
                case JsonType.List:
                    f_jsonTrimList(json.Array);
                    return;
                default:
                    return;
            }
        }

        static void f_jsonTrimList(JsonList jlist)
        {
            int length = jlist.Count;
            jlist.Capacity = length;
            for (int i = 0; i < length; i++)
            {
                f_jsonTrimExcess(jlist[i]);
            }
        }

        static void f_jsonTrimDict(JsonDictionary jdict)
        {
            jdict.p_dict = new Dictionary<string, JsonVariable>(jdict.p_dict, jdict.Comparer);

            foreach (var item in jdict)
            {
                f_jsonTrimExcess(item.Value);
            }
        }

        #endregion

    }

}
