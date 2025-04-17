using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cheng.Texts
{
    
    public static partial class TextManipulation
    {

        #region Paths

        /// <summary>
        /// 路径和目录相关字符串扩展
        /// </summary>
        public static class Path
        {

            /// <summary>
            /// 模拟指定目录数据判断并获取指定路径所在的相对位置
            /// </summary>
            /// <remarks>
            /// <para>该函数按照<paramref name="rootPath"/>提供的一个根目录，判断文件路径<paramref name="filePath"/>是否存在于根目录<paramref name="rootPath"/>之下，并返回与<paramref name="rootPath"/>所在的相对位置</para>
            /// </remarks>
            /// <param name="rootPath">表示一个模拟的根目录，分隔符是/或者\；如果该参数不包含卷标，则第一个分隔符前的字符串代表根节点</param>
            /// <param name="filePath">表示一个全局文件路径，分隔符是/或者\，用于模拟的文件所在位置；如果该参数不包含卷标，则第一个分隔符前的字符串代表根节点</param>
            /// <returns>文件<paramref name="filePath"/>与<paramref name="rootPath"/>目录所在的相对位置；如果文件<paramref name="filePath"/>不在<paramref name="rootPath"/>的子目录内，则返回null</returns>
            /// <exception cref="ArgumentNullException">给定的路径参数为null</exception>
            /// <exception cref="ArgumentException">给定的参数是空字符串</exception>
            public static string GetRelativeDirectoryByPath(string rootPath, string filePath)
            {
                if (rootPath is null || filePath is null)
                {
                    throw new ArgumentNullException(rootPath is null ? nameof(rootPath) : nameof(filePath));
                }

                var rootLen = rootPath.Length;
                var filePathLen = filePath.Length;

                if (rootLen == 0 || filePathLen == 0)
                {
                    throw new ArgumentException();
                }

                //文件路径比根目录短或一致
                if (filePathLen <= rootLen) return null;

                int i;

                for (i = 0; i < rootLen; i++)
                {
                    if (!eqs(rootPath[i], filePath[i]))
                    {
                        //不一致
                        return null;
                    }
                }

                //root 符合 filePath 前缀
                char c = rootPath[rootLen - 1];

                if(c == '\\' || c == '/')
                {
                    //根目录结尾是分隔符
                }
                else
                {
                    //根目录结尾不是分隔符
                    //判断路径一致
                    if (filePathLen == rootLen + 1) return null;
                    i++;
                }

                return filePath.Substring(i);

                bool eqs(char t_ca, char t_cb)
                {
                    if (t_ca == t_cb) return true;
                    if ((t_ca == '/' || t_ca == '\\') && (t_cb == '/' || t_cb == '\\'))
                    {
                        return true;
                    }
                    return false;
                }
            }

        }

        #endregion

    }

}
