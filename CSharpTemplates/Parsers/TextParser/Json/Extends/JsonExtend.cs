using Cheng.Json.Convers;
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

        /// <summary>
        /// 将指定文件内的json文本转化为<typeparamref name="T"/>对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">json解析器</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">从文件中读取文本时要使用的文本编码</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无权限</exception>
        /// <exception cref="NotImplementedException">无法解析json文本或无法将json对象转换到<typeparamref name="T"/>类型对象</exception>
        /// <exception cref="FileNotFoundException">无法找到文件</exception>
        /// <exception cref="DirectoryNotFoundException">无效的路径名称</exception>
        /// <exception cref="Exception">其它异常</exception>
        public static T JsonFileToObj<T>(this IJsonParser parser, string filePath, Encoding encoding)
        {
            if (parser is null || encoding is null) throw new ArgumentNullException();

            JsonVariable js;
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                using (StreamReader sr = new StreamReader(file, encoding, false, 1024 * 2, true))
                {
                    js = parser.ToJsonData(sr);
                }
            }
            return ConverToJsonAndObj<T>.Default.ToObj(js);
        }

        /// <summary>
        /// 将指定文件内的json文本转化为<typeparamref name="T"/>对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">json解析器</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>转换后的对象</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无权限</exception>
        /// <exception cref="NotImplementedException">无法解析json文本或无法将json对象转换到<typeparamref name="T"/>类型对象</exception>
        /// <exception cref="FileNotFoundException">无法找到文件</exception>
        /// <exception cref="DirectoryNotFoundException">无效的路径名称</exception>
        /// <exception cref="Exception">其它异常</exception>
        public static T JsonFileToObj<T>(this IJsonParser parser, string filePath)
        {
            return JsonFileToObj<T>(parser, filePath, Encoding.UTF8);
        }

        /// <summary>
        /// 将指定对象转化为json文本并写入到文本写入器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">json解析器</param>
        /// <param name="obj">要转化为json文本的对象</param>
        /// <param name="writer">要写入的文本写入器</param>
        /// <exception cref="ArgumentNullException">解析器或写入器是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">写入器已释放</exception>
        /// <exception cref="NotImplementedException">无法将json对象转换为<typeparamref name="T"/>对象</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static void WriteTo<T>(this IJsonParser parser, T obj, TextWriter writer)
        {
            if (parser is null || writer is null) throw new ArgumentNullException();
            var json = ConverToJsonAndObj<T>.Default.ToJsonVariable(obj);
            parser.ParsingJson(json, writer);
        }

        /// <summary>
        /// 将指定对象转化为json文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">json解析器</param>
        /// <param name="obj">要转化为json文本的对象</param>
        /// <returns>json文本</returns>
        /// <exception cref="ArgumentNullException">解析器或写入器是null</exception>
        /// <exception cref="NotImplementedException">无法将json对象转换为<typeparamref name="T"/>对象</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static string ObjToJsonText<T>(this IJsonParser parser, T obj)
        {
            using (StringWriter swr = new StringWriter())
            {
                WriteTo<T>(parser, obj, swr);
                return swr.ToString();
            }
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

        #region 层次

        /// <summary>
        /// 获取json对象内最深的层数
        /// </summary>
        /// <param name="json">json对象</param>
        /// <returns>
        /// <para>表示一个层级数</para>
        /// <para>当<paramref name="json"/>属于简单类型时，返回0；若<paramref name="json"/>属于复合类型，则返回复合嵌套最深的层次，返回值最小为0</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int GetDeepestLevel(this JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException(nameof(json));

            return f_deplvl(json, 0);

            int f_deplvl(JsonVariable t_json, int t_lvl)
            {
                var dataType = t_json.DataType;

                if ((int)dataType < (int)JsonType.List)
                {
                    //简单对象
                    return t_lvl;
                }
                else
                {
                    t_lvl++;
                    int parlvl = t_lvl;
                    //复合对象
                    if (dataType == JsonType.List)
                    {
                        foreach (var item in t_json.Array)
                        {
                            parlvl = Math.Max(f_deplvl(item, t_lvl), parlvl);
                        }
                        return parlvl;
                    }
                    else
                    {
                        foreach (var pair in t_json.JsonObject.p_dict)
                        {
                            parlvl = Math.Max(f_deplvl(pair.Value, t_lvl), parlvl);
                        }
                        return parlvl;
                    }
                }
            }
        }

        #endregion

        #region 参数

        /// <summary>
        /// 判断json是否是null对象或者是表示null的json对象
        /// </summary>
        /// <param name="json">要判断的json对象</param>
        /// <returns>当<paramref name="json"/>对象是null引用，或者json对象类型是null，返回true；否则返回false</returns>
        public static bool IsNullable(this JsonVariable json)
        {
            return json is null || json.IsNull;
        }

        #endregion

    }

}
