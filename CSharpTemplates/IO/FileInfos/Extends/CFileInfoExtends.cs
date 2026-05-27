using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure;
using Cheng.Streams;

namespace Cheng.IO
{

    /// <summary>
    /// <see cref="CFileSystemInfo"/>扩展功能
    /// </summary>
    public static class CFileSystemInfoExtends
    {

        /// <summary>
        /// 获取文件的字节大小或文件夹下所有文件的字节大小之和
        /// </summary>
        /// <param name="systemInfo">文件或文件夹</param>
        /// <returns>表示字节大小的64位整数</returns>
        public static long GetAllSize(this CFileSystemInfo systemInfo)
        {
            if (systemInfo is null) throw new ArgumentNullException();

            if (systemInfo.IsFile)
            {
                return systemInfo.FileInfo.Length;
            }
            long size = 0;
            var dire = systemInfo.DirectoryInfo;
            foreach (var info in dire.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                size += info.Length;
            }
            return size;
        }

    }

}