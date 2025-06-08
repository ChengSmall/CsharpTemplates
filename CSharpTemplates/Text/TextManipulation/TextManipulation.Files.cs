using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cheng.Memorys;

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
            /// 判断给定路径是否属于一个硬盘根目录
            /// </summary>
            /// <param name="path">表示路径的字符串</param>
            /// <returns>
            /// <para>当<paramref name="path"/>的开头3个字符属于类似"C:\"的逻辑驱动器卷标时返回true，其它任何情况返回false</para>
            /// </returns>
            public static bool IsRootPath(string path)
            {
                if (string.IsNullOrEmpty(path)) return false;

                if(path.Length > 2)
                {
                    var first = path[0];
                    char g = path[1];
                    char f = path[2];

                    if (g != ':') return false;
                    if (f != '/' && f != '\\') return false;
                    first = first.ToLower();
                    if (first >= 'a' && first < 'z') return true;
                }

                return false;
            }

            /// <summary>
            /// 模拟指定目录数据判断并获取指定路径所在的相对位置
            /// </summary>
            /// <remarks>
            /// <para>该函数按照<paramref name="rootPath"/>提供的一个根目录，判断文件路径<paramref name="filePath"/>是否存在于根目录<paramref name="rootPath"/>之下，并返回与<paramref name="rootPath"/>所在的相对位置</para>
            /// </remarks>
            /// <param name="rootPath">表示一个模拟的根目录，分隔符是/或者\；如果该参数不包含卷标，则第一个分隔符前的字符串代表根节点</param>
            /// <param name="filePath">表示一个全局文件路径，分隔符是/或者\，用于模拟的文件所在位置；如果该参数不包含卷标，则第一个分隔符前的字符串代表根节点</param>
            /// <returns>
            /// <para>文件<paramref name="filePath"/>与<paramref name="rootPath"/>目录所在的相对位置</para>
            /// <para>如果文件<paramref name="filePath"/>不在<paramref name="rootPath"/>的子目录内，则返回null；如果两个路径相同，返回空字符串</para>
            /// <para>如果<paramref name="rootPath"/>属于空路径则直接返回<paramref name="filePath"/></para>
            /// </returns>
            /// <exception cref="ArgumentNullException">给定的路径参数为null</exception>
            public static string GetRelativeDirectoryByPath(string rootPath, string filePath)
            {
                if (rootPath is null || filePath is null)
                {
                    throw new ArgumentNullException(rootPath is null ? nameof(rootPath) : nameof(filePath));
                }

                var rootLen = rootPath.Length;
                var filePathLen = filePath.Length;

                if (rootLen == 0 && filePathLen == 0)
                {
                    return string.Empty;
                }

                if(rootLen > 0)
                {
                    if (rootPath[rootLen - 1] == '/' || rootPath[rootLen - 1] == '\\')
                    {
                        rootPath = rootPath.Substring(0, rootLen - 1);
                        rootLen -= 1;
                    }
                }

                if(rootLen == 0)
                {
                    return filePath;
                }

                //文件路径比根目录短
                if (filePathLen < rootLen) return null;

                if(filePathLen == rootLen)
                {
                    //文件路径长度一致
                    return rootPath == filePath ? string.Empty : null;
                }

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

            /// <summary>
            /// 获取路径的目录
            /// </summary>
            /// <param name="path">表示一个路径的字符串</param>
            /// <returns>
            /// <para>返回路径<paramref name="path"/>中的父级目录，仅判断字符串</para>
            /// <para>如果<paramref name="path"/>是null则返回null</para>
            /// <para>如果<paramref name="path"/>是以类似 <![CDATA["C:\"]]> 的根目录字符串则返回空字符串</para>
            /// <para>如果<paramref name="path"/>不包含分隔符 / 或 \ 则返回空字符串</para>
            /// </returns>
            public static string GetOnlyDirectoryPath(string path)
            {
                if (string.IsNullOrEmpty(path)) return path;

                var i = path.LastIndexOf('\\');
                if (i < 0) i = path.LastIndexOf('/');
                if (i < 0) return string.Empty;
                if (i == 2)
                {
                    if (path[1] == ':') return string.Empty;
                }

                return path.Substring(0, i);
            }

            /// <summary>
            /// 获取路径最后一节路径名
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns>
            /// <para>返回<paramref name="path"/>的最后一个路径节点名；如果<paramref name="path"/>是一个文件路径返回 "文件名.后缀"，是目录则返回最后一级目录名</para>
            /// <para>没有路径分隔符则返回<paramref name="path"/>本身</para>
            /// <para><paramref name="path"/>是null返回null</para>
            /// </returns>
            public static string GetOnlySubPath(string path)
            {
                if (string.IsNullOrEmpty(path)) return path;
                if (path[path.Length - 1] == '\\' || path[path.Length - 1] == '/')
                {
                    path = path.Substring(0, path.Length - 1);
                }

                var i = path.LastIndexOf('\\');
                if (i < 0) i = path.LastIndexOf('/');
                if (i < 0) return path;
                return path.Substring(i + 1);
            }


        }

        #endregion

    }

}
