using System;
using System.Collections.Generic;
using System.IO;

namespace Cheng.IO
{

    /// <summary>
    /// 文件管理扩展功能
    /// </summary>
    public unsafe static class FileSystemExtends
    {

        /// <summary>
        /// 计算指定目录中所有文件大小的总和，包括子目录
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns>目录下所有文件的大小，以字节为单位；若检测到无法访问内容的目录或文件则将其忽略</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static ulong DirectorySize(this string path)
        {
            return DirectorySize(path, new Stack<DirectoryInfo>(4));
        }

        /// <summary>
        /// 计算指定目录中所有文件大小的总和，包括子目录
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="pathStack">计算时用到的缓冲栈</param>
        /// <returns>目录下所有文件的大小，以字节为单位；若检测到无法访问内容的目录或文件则将其忽略</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="System.Security.SecurityException">没有权限</exception>
        /// <exception cref="Exception">其它错误</exception>
        public static ulong DirectorySize(this string path, Stack<DirectoryInfo> pathStack)
        {

            if (pathStack is null || path is null) throw new ArgumentNullException();

            path = Path.GetFullPath(path);

            ulong size;

            FileInfo[] filenums;
            DirectoryInfo[] direnums;

            size = 0;

            DirectoryInfo tempDireInfo;
            //压入根目录
            pathStack.Clear();
            pathStack.Push(new DirectoryInfo(path));
            int i;
            int length;

            while (pathStack.Count != 0)
            {

                //弹栈1个目录
                tempDireInfo = pathStack.Pop();
                try
                {
                    filenums = tempDireInfo.GetFiles();
                }
                catch (Exception)
                {
                    //无法获取则跳转到下一个
                    continue;
                }

                //获取则开始枚举size
                length = filenums.Length;
                for (i = 0; i < length; i++)
                {
                    try
                    {
                        size += (ulong)filenums[i].Length;
                    }
                    catch (Exception)
                    {
                    }
                }
                
                try
                {
                    //获取子目录枚举
                    
                    direnums = tempDireInfo.GetDirectories();
                }
                catch (Exception)
                {
                    //没有权限下一个
                    continue;
                }

                //添加目录栈
                length = direnums.Length;
                for (i = 0; i < length; i++)
                {
                    pathStack.Push(direnums[i]);
                }
                //direnums = null;

            }


            return size;
        }

    }

}
