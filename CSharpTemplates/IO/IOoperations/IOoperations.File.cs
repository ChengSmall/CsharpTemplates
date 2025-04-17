using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime;
using System.Security;

namespace Cheng.IO
{
    
    public static partial class IOoperations
    {

        #region File

        internal static class Files
        {

        }

        public static class Path
        {

            /// <summary>
            /// 清理指定目录下的所有空文件夹
            /// </summary>
            /// <remarks>
            /// <para>检测<paramref name="directoryInfo"/>下的子文件夹，如果存在空文件夹则将其从硬盘删除；在删除子文件夹后如果父文件夹同样成为了空文件夹也会被删除，直到目录<paramref name="directoryInfo"/>为止</para>
            /// <para>参数<paramref name="directoryInfo"/>即使是空文件夹也不会被删除</para>
            /// </remarks>
            /// <param name="directoryInfo">指定目录，已确保存在目录</param>
            /// <exception cref="ArgumentNullException">参数是null</exception>
            /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
            /// <exception cref="IOException">IO错误</exception>
            /// <exception cref="SecurityException">没有权限</exception>
            public static void ClearEmptyDirectory(DirectoryInfo directoryInfo)
            {
                if (directoryInfo is null) throw new ArgumentNullException();
                if (!directoryInfo.Exists) throw new DirectoryNotFoundException();

                f_clearEmptyDire(directoryInfo);
            }

            private static void f_clearEmptyDire(DirectoryInfo info)
            {
                foreach (DirectoryInfo subDir in info.GetDirectories())
                {
                    f_clearEmptyDire(subDir); // 递归处理子目录

                    // 处理完子目录后检查是否存在及空状态
                    if (subDir.Exists && f_isDireEmpty(subDir))
                    {
                        try
                        {
                            subDir.Delete(); // 删除空目录
                        }
                        catch (Exception)
                        {
                        }
                       
                    }
                }
            }

            static bool f_isDireEmpty(DirectoryInfo info)
            {
                try
                {
                    var arr = info.GetDirectories("*", SearchOption.TopDirectoryOnly);
                    var fs = info.GetFiles("*", SearchOption.TopDirectoryOnly);
                    return arr.Length == 0 && fs.Length == 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }

        #endregion

    }

}
