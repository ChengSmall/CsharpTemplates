using System;
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

        #region 文件读取

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

        #endregion

    }

}
