using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cheng.Memorys;
using Cheng.DataStructure.Collections;

namespace Cheng.Texts
{
    
    public static partial class TextManipulation
    {

        #region Paths

        /// <summary>
        /// 判断字符是否属于路径分隔符
        /// </summary>
        /// <param name="c"></param>
        /// <returns>如果字符属于 '/' 或 '\' 返回true，否则返回false</returns>
        public static bool IsPathSeparatorChar(this char c)
        {
            return c == '/' || c == '\\';
        }

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

            /// <summary>
            /// 将多个字符串按照路径节点依次拼接
            /// </summary>
            /// <param name="paths">要拼接的路径节点数组</param>
            /// <returns>
            /// <para>拼接完毕的路径字符串</para>
            /// </returns>
            /// <exception cref="ArgumentException">路径节点数组内存在不合规路径参数，例如两个分隔符并列，使用上级目录访问符号".."访问的节点超出了参数范围</exception>
            public static string PathCombine(params string[] paths)
            {
                if (paths is null) return string.Empty;
                StringWriter swr = new StringWriter();
                PathCombine(paths, new ImmediatelyStack<string>(paths.Length * 2), '\\', swr);
                return swr.ToString();
            }

            /// <summary>
            /// 将多个字符串按照路径节点依次拼接
            /// </summary>
            /// <param name="paths">要拼接的路径节点集合</param>
            /// <param name="separator">进行拼接时的路径分隔符</param>
            /// <returns>拼接完毕路径字符串</returns>
            /// <exception cref="ArgumentNullException">参数为null</exception>
            /// <exception cref="ArgumentException">路径节点数组内存在不合规路径参数，例如两个分隔符并列，".."符号访问到了提供的路径节点之外</exception>
            public static string PathCombine(IEnumerable<string> paths, char separator)
            {
                if (paths is null) return string.Empty;
                StringWriter swr = new StringWriter();
                PathCombine(paths, new ImmediatelyStack<string>((paths is ICollection<string> ico) ? ico.Count : 4), separator, swr);
                return swr.ToString();
            }

            /// <summary>
            /// 将多个字符串按照路径节点依次拼接并写入文本写入器
            /// </summary>
            /// <remarks>
            /// <para>
            /// 从集合<paramref name="paths"/>中依次访问字符串参数，以'/'和'\'作为路径分隔符，按照标准的相对路径语法拼接路径节点；<br/>
            /// 例如参数<paramref name="paths"/>是 <code>{ "root\\path\\path2", "..\\dire", "file.txt" }</code> 则拼接后的字符串为 <code>"root\\path\\dire\\file.txt"</code>
            /// </para>
            /// </remarks>
            /// <param name="paths">要拼接的路径节点集合</param>
            /// <param name="buffer">需要准备的栈结构缓冲区对象</param>
            /// <param name="separator">进行拼接时的路径分隔符</param>
            /// <param name="append">要将结果输出到的写入器</param>
            /// <exception cref="ArgumentNullException">参数为null</exception>
            /// <exception cref="ArgumentException">路径节点数组内存在不合规路径参数，例如两个分隔符并列，".."符号访问到了提供的路径节点之外</exception>
            public static void PathCombine(IEnumerable<string> paths, ImmediatelyStack<string> buffer, char separator, TextWriter append)
            {
                //throw new NotImplementedException();

                if (buffer is null || paths is null || append is null) throw new ArgumentNullException();
                buffer.Clear();

                Predicate<char> isPathSep = IsPathSeparatorChar;
                int L, N;
                string pop;
                string fpath;

                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path)) continue;

                    int plenso = path.Length - 1;

                    bool pfirstSep = IsPathSeparatorChar(path[0]);
                    bool pendSep = IsPathSeparatorChar(path[plenso]);
                   

                    //删除前后分隔符
                    if (pfirstSep)
                    {
                        if (pendSep)
                        {
                            //前后
                            fpath = path.Substring(1, plenso - 1);
                        }
                        else
                        {
                            //前
                            fpath = path.Substring(1);
                        }
                    }
                    else
                    {
                        if (pendSep)
                        {
                            //后
                            fpath = path.Substring(0, plenso);
                        }
                        else
                        {
                            //无
                            fpath = path;
                        }
                    }

                    N = 0;
                    L = 0;

                    Loop:
                    //获取路径索引位置
                    
                    N = fpath.FindIndex(N, IsPathSeparatorChar);

                    //if (N == -1)
                    //{
                    //    //到达末尾
                    //    continue;
                    //}

                    //从last到next分隔符之间的字符串
                    string sepPath;
                    if(N == -1)
                    {
                        sepPath = fpath.Substring(L); //最后一位节点
                    }
                    else
                    {
                        sepPath = fpath.Substring(L, N - L);
                    }

                    //推入栈
                    if (sepPath == "..")
                    {
                        //撤回标识符弹出路径
                        if (!buffer.Pop(out pop))
                        {
                            throw new ArgumentException(Cheng.Properties.Resources.Exception_ParentDireOutIndex);
                        }
                    }
                    else
                    {
                        //推入路径
                        buffer.Push(sepPath);
                    }

                    if (N == -1)
                    {
                        //到达末尾
                        continue;
                    }

                    //分隔符下一位索引
                    N++;
                    //缓存上一个索引
                    L = N;

                    if (IsPathSeparatorChar(fpath[N]))
                    {
                        //两个分隔符相连
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_PathSepIsLink);
                    }

                    goto Loop;
                }

                N = buffer.Count - 1;
                //buffer.Clear();
                //string pop;
                //sb.Clear();
                for (L = 0; L <= N; L++)
                {
                    fpath = buffer.GetStackLast(L);

                    append.Write(fpath);
                    if (L != N) append.Write(separator);
                }
            }

        }

        #endregion

    }

}
